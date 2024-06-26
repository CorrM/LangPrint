﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LangPrint.Utils;

namespace LangPrint.CSharp;

public sealed class CSharpProcessor : LangProcessor<CSharpLangOptions>
{
    public override CSharpLangOptions Options { get; protected set; }

    public CSharpProcessor(CSharpLangOptions? options = null)
    {
        Options = options ?? new CSharpLangOptions();
    }

    private string MakeCSharpFile(CSharpPackage package)
    {
        var sb = new LangStringWriter(Options);

        // File header
        sb.Append(
            GetFileHeader(
                package.HeadingComment,
                package.NameSpace,
                package.Usings,
                package.BeforeNameSpace,
                out int indentLvl
            )
        );

        // Delegates
        if (package.Delegates.Count > 0)
        {
            sb.Append(GenerateDelegates(package.Delegates, indentLvl));
        }

        // Enums
        if (package.Enums.Count > 0)
        {
            sb.Append(GenerateEnums(package.Enums, indentLvl));
        }

        // Structs
        if (package.Structs.Count > 0)
        {
            sb.Append(GenerateStructs(package.Structs, indentLvl));
        }

        // File footer
        sb.Append(GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

        return sb.ToString();
    }

    private string MakePackageStructsFile(CSharpPackage package)
    {
        var sb = new LangStringWriter(Options);

        // File header
        sb.Append(
            GetFileHeader(
                package.HeadingComment,
                package.NameSpace,
                package.Usings,
                package.BeforeNameSpace,
                out int indentLvl
            )
        );

        // Delegates
        if (package.Delegates.Count > 0)
        {
            sb.Append(GenerateDelegates(package.Delegates, indentLvl));
        }

        // Enums
        if (package.Enums.Count > 0)
        {
            sb.Append(GenerateEnums(package.Enums, indentLvl));
        }

        // Structs
        if (package.Structs.Count > 0)
        {
            sb.Append(
                GenerateStructs(package.Structs.Where(s => s is { IsClass: false, IsInterface: false }), indentLvl)
            );
        }

        // File footer
        sb.Append(GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

        return sb.ToString();
    }

    private string MakePackageClassesFile(CSharpPackage package)
    {
        var sb = new LangStringWriter(Options);

        // File header
        sb.Append(
            GetFileHeader(
                package.HeadingComment,
                package.NameSpace,
                package.Usings,
                package.BeforeNameSpace,
                out int indentLvl
            )
        );

        // Classes
        if (package.Structs.Count > 0)
        {
            sb.Append(GenerateStructs(package.Structs.Where(s => s.IsClass), indentLvl));
        }

        // File footer
        sb.Append(GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

        return sb.ToString();
    }

    private string MakePackageInterfacesFile(CSharpPackage package)
    {
        var sb = new LangStringWriter(Options);

        // File header
        sb.Append(
            GetFileHeader(
                package.HeadingComment,
                package.NameSpace,
                package.Usings,
                package.BeforeNameSpace,
                out int indentLvl
            )
        );

        // Interfaces
        if (package.Structs.Count > 0)
        {
            sb.Append(GenerateStructs(package.Structs.Where(s => s.IsInterface), indentLvl));
        }

        // File footer
        sb.Append(GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

        return sb.ToString();
    }

    public string GetFileHeader(
        IEnumerable<string> headingComment,
        string nameSpace,
        List<string>? usings,
        string beforeNameSpace,
        out int indentLvl
    )
    {
        indentLvl = 0;
        var sb = new LangStringWriter(Options);

        // HeadingComment
        sb.Append(GetMultiCommentString(headingComment, indentLvl));

        // Includes
        if (usings?.Count > 0)
        {
            sb.Append(GenerateUsings(usings, indentLvl));
        }

        // BeforeNameSpace
        if (!string.IsNullOrWhiteSpace(beforeNameSpace))
        {
            sb.Append(beforeNameSpace + Options.GetNewLineText());
            sb.Append(Options.GetNewLineText());
        }

        // NameSpace
        if (!string.IsNullOrWhiteSpace(nameSpace))
        {
            sb.Append(Helper.GetIndent(indentLvl));
            sb.Append($"namespace {nameSpace}{Options.GetNewLineText()}{{{Options.GetNewLineText()}");
            indentLvl++;
        }

        return sb.ToString();
    }

    public string GetFileFooter(string nameSpace, string afterNameSpace, ref int indentLvl)
    {
        var sb = new LangStringWriter(Options);

        // Close NameSpace
        if (!string.IsNullOrWhiteSpace(nameSpace))
        {
            indentLvl--;
            sb.Append($"{Helper.GetIndent(indentLvl)}}}{Options.GetNewLineText()}");
            sb.Append(Options.GetNewLineText());
        }

        // AfterNameSpace
        if (!string.IsNullOrWhiteSpace(afterNameSpace))
        {
            sb.Append(afterNameSpace);
        }

        sb.Append(Options.GetNewLineText());

        return sb.ToString();
    }

    public string GetSectionHeading(string name, int indentLvl)
    {
        string headLine = string.Concat(Enumerable.Repeat("-", 50));

        string ret = Helper.GetIndent(indentLvl) +
                     "// " +
                     headLine +
                     Options.GetNewLineText() +
                     Helper.GetIndent(indentLvl) +
                     "// " +
                     "# " +
                     name +
                     Options.GetNewLineText() +
                     Helper.GetIndent(indentLvl) +
                     "// " +
                     headLine +
                     Options.GetNewLineText();

        return new LangStringWriter(Options, ret).ToString();
    }

    public string GetMultiCommentString(IEnumerable<string>? comments, int baseIndentLvl, bool finalizeReturn = true)
    {
        if (comments is null)
        {
            return string.Empty;
        }

        IEnumerable<string> eComments = comments.ToList();
        if (!eComments.Any())
        {
            return string.Empty;
        }

        string ret = $"{Helper.GetIndent(baseIndentLvl)}/**{Options.GetNewLineText()}" +
                     Helper.JoinString(Options.GetNewLineText(), eComments, $"{Helper.GetIndent(baseIndentLvl)} * ") +
                     $"{Options.GetNewLineText()}{Helper.GetIndent(baseIndentLvl)} */";

        ret = finalizeReturn ? Helper.FinalizeSection(ret, Options.GetNewLineText()) : ret + Options.GetNewLineText();

        return new LangStringWriter(Options, ret).ToString();
    }

    public string GetAttributeString(CSharpAttribute attribute, int baseIndentLvl)
    {
        var sb = new LangStringWriter(Options);
        sb.Append(GetBeforePrint(attribute, baseIndentLvl));

        // Comment
        sb.Append(GetMultiCommentString(attribute.Comments, baseIndentLvl, false));

        // Indent
        sb.Append(Helper.GetIndent(baseIndentLvl));

        // Open
        sb.Append('[');

        // AppliesTo
        if (!string.IsNullOrWhiteSpace(attribute.AppliesTo))
        {
            sb.Append($"{attribute.AppliesTo}: ");
        }

        // Name
        sb.Append(attribute.Name);

        // Args
        if (attribute.Arguments.Count > 0)
        {
            sb.Append('(');
            sb.Append(string.Join(", ", attribute.Arguments));
            sb.Append(')');
        }

        // Close
        sb.Append(']');

        // Inline comment
        if (!string.IsNullOrEmpty(attribute.InlineComment))
        {
            sb.Append($" // {attribute.InlineComment}");
        }

        sb.Append(GetAfterPrint(attribute, baseIndentLvl));
        return sb.ToString();
    }

    public string GetDelegateString(CSharpDelegate @delegate, int baseIndentLvl)
    {
        var sb = new LangStringWriter(Options);
        sb.Append(GetBeforePrint(@delegate, baseIndentLvl));

        // Comment
        sb.Append(GetMultiCommentString(@delegate.Comments, baseIndentLvl, false));

        // Attributes
        if (@delegate.Attributes.Count > 0)
        {
            sb.Append(GenerateAttributes(@delegate.Attributes, baseIndentLvl));
            sb.Append(Options.GetNewLineText());
        }

        // Indent
        sb.Append(Helper.GetIndent(baseIndentLvl));

        // Access
        if (!string.IsNullOrWhiteSpace(@delegate.AccessModifier))
        {
            sb.Append($"{@delegate.AccessModifier} ");
        }

        // Delegate keyword
        sb.Append("delegate ");

        // Type
        if (!string.IsNullOrWhiteSpace(@delegate.Type))
        {
            sb.Append($"{@delegate.Type} ");
        }

        // Name
        sb.Append(@delegate.Name);

        // Generic
        if (@delegate.GenericParams.Count > 0)
        {
            sb.Append($"<{string.Join(", ", @delegate.GenericParams)}>");
        }

        // Params
        sb.Append('(');
        sb.Append(string.Join(", ", @delegate.Params.Select(GetParamString)));
        sb.Append(");");

        // Inline comment
        if (!string.IsNullOrEmpty(@delegate.InlineComment))
        {
            sb.Append($" // {@delegate.InlineComment}");
        }

        sb.Append(Options.GetNewLineText());

        sb.Append(GetAfterPrint(@delegate, baseIndentLvl));
        return sb.ToString();
    }

    public string GetParamString(CSharpParameter parameter)
    {
        var sb = new LangStringWriter(Options);
        sb.Append(GetBeforePrint(parameter, 0));

        // Attributes
        if (parameter.Attributes.Count > 0)
        {
            sb.Append(GenerateAttributes(parameter.Attributes, 0));
            sb.Append(' ');
        }

        // Ref
        if (parameter.IsRef)
        {
            sb.Append("ref ");
        }

        // Ref
        if (parameter.IsOut)
        {
            sb.Append("out ");
        }

        // Param
        sb.Append($"{parameter.Type} {parameter.Name}");

        sb.Append(GetAfterPrint(parameter, 0));
        return sb.ToString();
    }

    public string GetEventString(CSharpEvent @event, int baseIndentLvl)
    {
        var sb = new LangStringWriter(Options);
        sb.Append(GetBeforePrint(@event, baseIndentLvl));

        // Comment
        sb.Append(GetMultiCommentString(@event.Comments, baseIndentLvl, finalizeReturn: false));

        // Attributes
        if (@event.Attributes.Count > 0)
        {
            sb.Append(GenerateAttributes(@event.Attributes, baseIndentLvl));
            sb.Append(Options.GetNewLineText());
        }

        // Indent
        sb.Append(Helper.GetIndent(baseIndentLvl));

        var firstPartSb = new LangStringWriter(Options);

        // Access
        if (!string.IsNullOrWhiteSpace(@event.AccessModifier))
        {
            firstPartSb.Append($"{@event.AccessModifier} ");
        }

        // Static
        if (@event.IsStatic)
        {
            firstPartSb.Append("static ");
        }

        // Event keyword
        firstPartSb.Append("event ");

        // Type
        firstPartSb.Append(@event.Type);

        // Type padding
        string typePrefix = firstPartSb.ToString();
        sb.Append($"{typePrefix.PadRight(Options.FieldMemberTypePadSize)} ");

        var nameSb = new LangStringWriter(Options);

        // Name
        nameSb.Append($"{@event.Name};");

        // Inline comment
        if (!string.IsNullOrEmpty(@event.InlineComment))
        {
            sb.Append(nameSb.ToString().PadRight(Options.InlineCommentPadSize) + $" // {@event.InlineComment}");
        }
        else
        {
            sb.Append(nameSb);
        }

        sb.Append(GetAfterPrint(@event, baseIndentLvl));
        return sb.ToString();
    }

    public string GetFieldString(CSharpField field, int baseIndentLvl)
    {
        var sb = new LangStringWriter(Options);
        sb.Append(GetBeforePrint(field, baseIndentLvl));

        // Comment
        sb.Append(GetMultiCommentString(field.Comments, baseIndentLvl, finalizeReturn: false));

        // Attributes
        if (field.Attributes.Count > 0)
        {
            sb.Append(GenerateAttributes(field.Attributes, baseIndentLvl));
            sb.Append(Options.GetNewLineText());
        }

        // Indent
        sb.Append(Helper.GetIndent(baseIndentLvl));

        var firstPartSb = new LangStringWriter(Options);

        // Access
        if (!string.IsNullOrWhiteSpace(field.AccessModifier))
        {
            firstPartSb.Append($"{field.AccessModifier} ");
        }

        // Volatile
        if (field.IsVolatile)
        {
            firstPartSb.Append("volatile ");
        }

        // Static
        if (field.IsStatic)
        {
            firstPartSb.Append("static ");
        }

        // ReadOnly
        if (field.IsReadOnly)
        {
            firstPartSb.Append("readonly ");
        }

        // Type
        firstPartSb.Append(field.Type);

        // Array
        if (field.IsArray)
        {
            firstPartSb.Append("[]");
        }

        // Type padding
        string typePrefix = firstPartSb.ToString();
        sb.Append($"{typePrefix.PadRight(Options.FieldMemberTypePadSize)} ");

        var nameSb = new LangStringWriter(Options);

        // Name
        nameSb.Append(field.Name);

        // Value
        if (!string.IsNullOrWhiteSpace(field.Value))
        {
            nameSb.Append($" = {field.Value}");
        }

        nameSb.Append(';');

        // Inline comment
        if (!string.IsNullOrEmpty(field.InlineComment))
        {
            sb.Append(nameSb.ToString().PadRight(Options.InlineCommentPadSize) + $" // {field.InlineComment}");
        }
        else
        {
            sb.Append(nameSb);
        }

        sb.Append(GetAfterPrint(field, baseIndentLvl));
        return sb.ToString();
    }

    public string GetPropertyString(CSharpProperty property, int baseIndentLvl)
    {
        var sb = new LangStringWriter(Options);
        sb.Append(GetBeforePrint(property, baseIndentLvl));

        // Comment
        sb.Append(GetMultiCommentString(property.Comments, baseIndentLvl, finalizeReturn: false));

        // Attributes
        if (property.Attributes.Count > 0)
        {
            sb.Append(GenerateAttributes(property.Attributes, baseIndentLvl));
            sb.Append(Options.GetNewLineText());
        }

        // Indent
        sb.Append(Helper.GetIndent(baseIndentLvl));

        var firstPartSb = new LangStringWriter(Options);

        // Access
        if (!string.IsNullOrWhiteSpace(property.AccessModifier))
        {
            firstPartSb.Append($"{property.AccessModifier} ");
        }

        // Abstract
        if (property.IsAbstract)
        {
            firstPartSb.Append("abstract ");
        }

        // Override
        if (property.IsOverride)
        {
            firstPartSb.Append("override ");
        }

        // Virtual
        if (property.IsVirtual)
        {
            firstPartSb.Append("virtual ");
        }

        // Static
        if (property.IsStatic)
        {
            firstPartSb.Append("static ");
        }

        // Type
        firstPartSb.Append(property.Type);

        // Array
        if (property.IsArray)
        {
            firstPartSb.Append("[]");
        }

        // Type padding
        string typePrefix = firstPartSb.ToString();
        sb.Append($"{typePrefix.PadRight(Options.FieldMemberTypePadSize)} ");

        var nameSb = new LangStringWriter(Options);

        // Name
        nameSb.Append(property.Name);

        // Getter, Setter open
        if (property.HaveGetter || property.HaveSetter)
        {
            if (property.GetterCode.Count > 0 || property.SetterCode.Count > 0)
            {
                nameSb.Append($"{Options.GetNewLineText()}{Helper.GetIndent(baseIndentLvl)}");
                baseIndentLvl++;
            }
            else
            {
                nameSb.Append(' ');
            }

            nameSb.Append("{ ");
        }

        for (int i = 0; i < 2; i++)
        {
            string methodName = i == 0 ? "get" : "set";

            switch (i)
            {
                case 0 when !property.HaveGetter:
                case 1 when !property.HaveSetter:
                    continue;

                case 0 when property.GetterCode.Count == 0:
                    nameSb.Append($"{methodName}; ");
                    continue;

                case 1 when property.SetterCode.Count == 0:
                    nameSb.Append($"{methodName}; ");
                    continue;
            }

            nameSb.Append($"{Options.GetNewLineText()}{Helper.GetIndent(baseIndentLvl)}");
            nameSb.Append(methodName);
            nameSb.Append($"{Options.GetNewLineText()}{Helper.GetIndent(baseIndentLvl)}");
            nameSb.Append("{ ");

            baseIndentLvl++;
            nameSb.Append($"{Options.GetNewLineText()}{Helper.GetIndent(baseIndentLvl)}");
            nameSb.Append(
                string.Join(
                    $"{Options.GetNewLineText()}{Helper.GetIndent(baseIndentLvl)}",
                    i == 0 ? property.GetterCode : property.SetterCode
                )
            );

            baseIndentLvl--;
            nameSb.Append($"{Options.GetNewLineText()}{Helper.GetIndent(baseIndentLvl)}");
            nameSb.Append('}');
        }

        // Getter, Setter close
        if (property.HaveGetter || property.HaveSetter)
        {
            if (property.GetterCode.Count > 0 || property.SetterCode.Count > 0)
            {
                baseIndentLvl--;
                nameSb.Append($"{Options.GetNewLineText()}{Helper.GetIndent(baseIndentLvl)}");
            }

            nameSb.Append('}');
        }

        // Value
        if (!string.IsNullOrWhiteSpace(property.Value))
        {
            nameSb.Append($" = {property.Value};");
        }

        // Inline comment
        if (!string.IsNullOrEmpty(property.InlineComment))
        {
            sb.Append(nameSb.ToString().PadRight(Options.InlineCommentPadSize) + $" // {property.InlineComment}");
        }
        else
        {
            sb.Append(nameSb);
        }

        sb.Append(GetAfterPrint(property, baseIndentLvl));
        return sb.ToString();
    }

    public string GetEnumString(CSharpEnum @enum, int baseIndentLvl)
    {
        var sb = new LangStringWriter(Options);
        sb.Append(GetBeforePrint(@enum, baseIndentLvl));

        // Comment
        sb.Append(GetMultiCommentString(@enum.Comments, baseIndentLvl, finalizeReturn: false));

        // Attributes
        if (@enum.Attributes.Count > 0)
        {
            sb.Append(GenerateAttributes(@enum.Attributes, baseIndentLvl));
            sb.Append(Options.GetNewLineText());
        }

        // Indent
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}");

        // Access
        if (!string.IsNullOrWhiteSpace(@enum.AccessModifier))
        {
            sb.Append($"{@enum.AccessModifier} ");
        }

        // Name
        sb.Append($"enum {@enum.Name}");

        // Type
        if (!string.IsNullOrWhiteSpace(@enum.Type))
        {
            sb.Append($" : {@enum.Type}");
        }

        // Inline comment
        if (!string.IsNullOrEmpty(@enum.InlineComment))
        {
            sb.Append($" // {@enum.InlineComment}");
        }

        sb.Append(Options.GetNewLineText());

        // Open
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}{{{Options.GetNewLineText()}");
        baseIndentLvl++;

        // Values
        if (@enum.Values.Count > 0)
        {
            int biggestName = @enum.Values.Max(ev => ev.Key.Length);
            IEnumerable<string> vals = @enum.Values.Select(
                ev =>
                {
                    string value =
                        @enum.HexValues &&
                        long.TryParse(ev.Value, NumberFormatInfo.InvariantInfo, out long iValue) &&
                        iValue >= 0
                            ? $"0x{iValue:X16}"
                            : ev.Value;

                    return $"{ev.Key.PadRight(biggestName)} = {value}";
                }
            );

            string values = Helper.JoinString("," + Options.GetNewLineText(), vals, Helper.GetIndent(baseIndentLvl));

            sb.Append(values);
        }

        sb.Append(Options.GetNewLineText());

        // Close
        baseIndentLvl--;
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}}}{Options.GetNewLineText()}");

        sb.Append(GetAfterPrint(@enum, baseIndentLvl));
        return sb.ToString();
    }

    public string GetFunctionString(CSharpFunction func, int baseIndentLvl)
    {
        var sb = new LangStringWriter(Options);
        sb.Append(GetBeforePrint(func, baseIndentLvl));

        // Comment
        sb.Append(GetMultiCommentString(func.Comments, baseIndentLvl, finalizeReturn: false));

        // Attributes
        if (func.Attributes.Count > 0)
        {
            sb.Append(GenerateAttributes(func.Attributes, baseIndentLvl));
            sb.Append(Options.GetNewLineText());
        }

        // Indent
        sb.Append(Helper.GetIndent(baseIndentLvl));

        // Access
        if (!string.IsNullOrWhiteSpace(func.AccessModifier))
        {
            sb.Append($"{func.AccessModifier} ");
        }

        // Abstract
        if (func.IsAbstract)
        {
            sb.Append("abstract ");
        }

        // Virtual
        if (func.IsVirtual)
        {
            sb.Append("virtual ");
        }

        // Unsafe
        if (func.IsUnsafe)
        {
            sb.Append("unsafe ");
        }

        // Override
        if (func.IsOverride)
        {
            sb.Append("override ");
        }

        // Extern
        if (func.IsExtern)
        {
            sb.Append("extern ");
        }

        // Async
        if (func.IsAsync)
        {
            sb.Append("async ");
        }

        // Static
        if (func.IsStatic)
        {
            sb.Append("static ");
        }

        // Type
        if (!string.IsNullOrWhiteSpace(func.Type))
        {
            sb.Append($"{func.Type} ");
        }

        // Name
        sb.Append(func.Name);

        // Generic
        if (func.GenericParams.Count > 0)
        {
            sb.Append($"<{string.Join(", ", func.GenericParams)}>");
        }

        // Params
        sb.Append('(');
        sb.Append(string.Join(", ", func.Params.Select(GetParamString)));
        sb.Append(')');

        // Inline comment
        if (!string.IsNullOrEmpty(func.InlineComment))
        {
            sb.Append($" // {func.InlineComment}");
        }

        sb.Append(Options.GetNewLineText());

        // Body
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}{{{Options.GetNewLineText()}");
        baseIndentLvl++;

        sb.Append(Helper.JoinString(Options.GetNewLineText(), func.Body, Helper.GetIndent(baseIndentLvl)));

        baseIndentLvl--;
        sb.Append($"{Options.GetNewLineText()}{Helper.GetIndent(baseIndentLvl)}}}{Options.GetNewLineText()}");

        sb.Append(GetAfterPrint(func, baseIndentLvl));
        return sb.ToString();
    }

    public string GetStructString(CSharpStruct @struct, int baseIndentLvl)
    {
        var sb = new LangStringWriter(Options);
        sb.Append(GetBeforePrint(@struct, baseIndentLvl));

        // Comment
        sb.Append(GetMultiCommentString(@struct.Comments, baseIndentLvl, finalizeReturn: false));

        // Attributes
        if (@struct.Attributes.Count > 0)
        {
            sb.Append(GenerateAttributes(@struct.Attributes, baseIndentLvl));
            sb.Append(Options.GetNewLineText());
        }

        // Indent
        sb.Append(Helper.GetIndent(baseIndentLvl));

        // Access
        if (!string.IsNullOrWhiteSpace(@struct.AccessModifier))
        {
            sb.Append($"{@struct.AccessModifier} ");
        }

        // ReadOnly
        if ( /*!@struct.IsClass && */@struct.IsReadOnly)
        {
            sb.Append("readonly ");
        }

        // Abstract
        if (@struct.IsAbstract)
        {
            sb.Append("abstract ");
        }

        // Sealed
        if (@struct.IsSealed)
        {
            sb.Append("sealed ");
        }

        // Static
        if (@struct.IsStatic)
        {
            sb.Append("static ");
        }

        // Kind
        if (@struct.IsClass)
        {
            sb.Append("class ");
        }
        else if (@struct.IsInterface)
        {
            sb.Append("interface ");
        }
        else
        {
            sb.Append("struct ");
        }

        // Name
        sb.Append(@struct.Name);

        // Generic
        if (@struct.GenericParams.Count > 0)
        {
            sb.Append($"<{string.Join(", ", @struct.GenericParams)}>");
        }

        if (!string.IsNullOrWhiteSpace(@struct.Super) || @struct.Interfaces.Count > 0)
        {
            sb.Append(" : ");
        }

        // Super
        if (!string.IsNullOrWhiteSpace(@struct.Super))
        {
            sb.Append(@struct.Super);
        }

        // Interfaces
        if (@struct.Interfaces.Count > 0)
        {
            if (!string.IsNullOrWhiteSpace(@struct.Super))
            {
                sb.Append(", ");
            }

            sb.Append($"{Helper.JoinString(", ", @struct.Interfaces)}");
        }

        // Inline comment
        if (!string.IsNullOrEmpty(@struct.InlineComment))
        {
            sb.Append($" // {@struct.InlineComment}");
        }

        sb.Append(Options.GetNewLineText());

        // Open struct scope
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}{{");
        baseIndentLvl++;

        if (@struct.Fields.Count > 0 || @struct.Properties.Count > 0 || @struct.Methods.Count > 0)
        {
            sb.Append(Options.GetNewLineText());
        }

        // Delegates
        if (@struct.Delegates.Count > 0)
        {
            sb.Append(GenerateDelegates(@struct.Delegates, baseIndentLvl));
        }

        // Events
        if (@struct.Events.Count > 0)
        {
            sb.Append(GenerateEvents(@struct.Events, baseIndentLvl));
        }

        // Fields
        if (@struct.Fields.Count > 0)
        {
            sb.Append(GenerateFields(@struct.Fields, baseIndentLvl));
        }

        // Properties
        if (@struct.Properties.Count > 0)
        {
            sb.Append(GenerateProperties(@struct.Properties, baseIndentLvl));
        }

        // Methods
        if (@struct.Methods.Count > 0)
        {
            sb.Append(GenerateFunctions(@struct.Methods, baseIndentLvl));
        }

        // Close struct scope
        baseIndentLvl--;
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}}}{Options.GetNewLineText()}");

        sb.Append(GetAfterPrint(@struct, baseIndentLvl));
        return sb.ToString();
    }

    public string GenerateUsings(IEnumerable<string> usings, int baseIndentLvl)
    {
        string ret = Helper.JoinString(
            Options.GetNewLineText(),
            usings,
            $"{Helper.GetIndent(baseIndentLvl)}using ",
            ";"
        );
        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateAttributes(IEnumerable<CSharpAttribute> attributes, int baseIndentLvl)
    {
        List<CSharpAttribute> attribute = attributes.Where(v => !string.IsNullOrWhiteSpace(v.Name)).ToList();

        if (attribute.Count == 0)
        {
            return string.Empty;
        }

        string ret = string.Join(Options.GetNewLineText(), attribute.Select(a => GetAttributeString(a, baseIndentLvl)));

        return ret;
    }

    public string GenerateDelegates(IEnumerable<CSharpDelegate> delegates, int baseIndentLvl)
    {
        List<CSharpDelegate> dels = delegates.Where(
                f => !string.IsNullOrWhiteSpace(f.Name) && !string.IsNullOrWhiteSpace(f.Type)
            )
            .ToList();

        if (dels.Count == 0)
        {
            return string.Empty;
        }

        string ret = string.Join(Options.GetNewLineText(), dels.Select(f => GetDelegateString(f, baseIndentLvl)));

        if (Options.PrintSectionName)
        {
            ret = GetSectionHeading("Delegates", baseIndentLvl) + ret;
        }

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateEvents(IEnumerable<CSharpEvent> events, int baseIndentLvl)
    {
        List<CSharpEvent> evs = events.Where(
                f => !string.IsNullOrWhiteSpace(f.Name) && !string.IsNullOrWhiteSpace(f.Type)
            )
            .ToList();

        if (evs.Count == 0)
        {
            return string.Empty;
        }

        string ret = string.Join(Options.GetNewLineText(), evs.Select(f => GetEventString(f, baseIndentLvl)));
        if (Options.PrintSectionName)
        {
            ret = GetSectionHeading("Events", baseIndentLvl) + ret;
        }

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateFields(IEnumerable<CSharpField> fields, int baseIndentLvl)
    {
        List<CSharpField> vars = fields.Where(
                v => !string.IsNullOrWhiteSpace(v.Name) && !string.IsNullOrWhiteSpace(v.Type)
            )
            .ToList();

        if (vars.Count == 0)
        {
            return string.Empty;
        }

        string ret = string.Join(Options.GetNewLineText(), vars.Select(v => GetFieldString(v, baseIndentLvl)));

        if (Options.PrintSectionName)
        {
            ret = GetSectionHeading("Fields", baseIndentLvl) + ret;
        }

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateProperties(IEnumerable<CSharpProperty> properties, int baseIndentLvl)
    {
        List<CSharpProperty> props = properties.Where(
                v => !string.IsNullOrWhiteSpace(v.Name) && !string.IsNullOrWhiteSpace(v.Type)
            )
            .ToList();

        if (props.Count == 0)
        {
            return string.Empty;
        }

        string ret = string.Join(Options.GetNewLineText(), props.Select(p => GetPropertyString(p, baseIndentLvl)));

        if (Options.PrintSectionName)
        {
            ret = GetSectionHeading("Properties", baseIndentLvl) + ret;
        }

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateFunctions(IEnumerable<CSharpFunction> functions, int baseIndentLvl)
    {
        List<CSharpFunction> funcs = functions.Where(
                f => !string.IsNullOrWhiteSpace(f.Name) && !string.IsNullOrWhiteSpace(f.Type)
            )
            .ToList();

        if (funcs.Count == 0)
        {
            return string.Empty;
        }

        string ret = string.Join(Options.GetNewLineText(), funcs.Select(f => GetFunctionString(f, baseIndentLvl)));

        if (Options.PrintSectionName)
        {
            ret = GetSectionHeading("Functions", baseIndentLvl) + ret;
        }

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateEnums(IEnumerable<CSharpEnum> enums, int baseIndentLvl)
    {
        List<CSharpEnum> vEnums = enums.Where(e => !string.IsNullOrWhiteSpace(e.Name)).ToList();

        if (vEnums.Count == 0)
        {
            return string.Empty;
        }

        string ret = string.Join(Options.GetNewLineText(), vEnums.Select(e => GetEnumString(e, baseIndentLvl)));

        if (Options.PrintSectionName)
        {
            ret = GetSectionHeading("Enums", baseIndentLvl) + ret;
        }

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateStructs(IEnumerable<CSharpStruct> structs, int baseIndentLvl)
    {
        List<CSharpStruct> vStruct = structs.Where(s => !string.IsNullOrWhiteSpace(s.Name)).ToList();

        if (vStruct.Count == 0)
        {
            return string.Empty;
        }

        string ret = string.Join(Options.GetNewLineText(), vStruct.Select(s => GetStructString(s, baseIndentLvl)));

        if (Options.PrintSectionName)
        {
            ret = GetSectionHeading(vStruct.TrueForAll(s => s.IsClass) ? "Classes" : "Structs", baseIndentLvl) + ret;
        }

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public Dictionary<string, string> GenerateFiles(CSharpPackage cSharpPackage)
    {
        var ret = new Dictionary<string, string>(StringComparer.Ordinal);

        if (!Options.GeneratePackageSyntax)
        {
            ret.Add($"{cSharpPackage.Name}.cs", MakeCSharpFile(cSharpPackage));

            return ret;
        }

        ret.Add($"{cSharpPackage.Name}_Structs.cs", MakePackageStructsFile(cSharpPackage));
        ret.Add($"{cSharpPackage.Name}_Classes.cs", MakePackageClassesFile(cSharpPackage));
        ret.Add($"{cSharpPackage.Name}_Interfaces.cs", MakePackageInterfacesFile(cSharpPackage));

        return ret;
    }
}
