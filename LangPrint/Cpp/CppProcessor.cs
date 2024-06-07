using System.Collections.Generic;
using System.Linq;
using LangPrint.Utils;

namespace LangPrint.Cpp;

public sealed class CppProcessor : LangProcessor<CppLangOptions>
{
    public override CppLangOptions Options { get; protected set; }

    public CppProcessor(CppLangOptions? options = null)
    {
        Options = options ?? new CppLangOptions();
    }

    public string GetFileHeader(
        IEnumerable<string>? headingComment,
        string? @namespace,
        List<string>? pragmas,
        List<string>? includes,
        List<string>? forwardDeclarations,
        List<CppDefine>? defines,
        List<string>? typeDefs,
        string? beforeNameSpace,
        out int indentLvl
    )
    {
        indentLvl = 0;
        var sb = new LangStringWriter(Options);

        // Pragmas
        if (pragmas?.Count > 0)
        {
            sb.Append(GeneratePragmas(pragmas, indentLvl));
        }

        // HeadingComment
        sb.Append(GetMultiCommentString(headingComment, indentLvl));

        // Includes
        if (includes?.Count > 0)
        {
            sb.Append(GenerateIncludes(includes, indentLvl));
        }

        // Forward declarations
        if (forwardDeclarations?.Count > 0)
        {
            sb.Append(GenerateForwards(forwardDeclarations, indentLvl));
        }

        // Defines
        if (defines?.Count > 0)
        {
            sb.Append(GenerateDefines(defines, indentLvl));
        }

        // TypeDefs
        if (typeDefs?.Count > 0)
        {
            sb.Append(GenerateTypeDefs(typeDefs, indentLvl));
        }

        // BeforeNameSpace
        if (!string.IsNullOrWhiteSpace(beforeNameSpace))
        {
            sb.Append(beforeNameSpace + Options.GetNewLineText());
            sb.Append(Options.GetNewLineText());
        }

        // NameSpace
        if (!string.IsNullOrWhiteSpace(@namespace))
        {
            sb.Append(Helper.GetIndent(indentLvl));
            sb.Append($"namespace {@namespace}{Options.GetNewLineText()}{{{Options.GetNewLineText()}");
            indentLvl++;
        }

        return sb.ToString();
    }

    public string GetFileFooter(bool namespaceClose, string? afterNamespace, ref int indentLvl)
    {
        var sb = new LangStringWriter(Options);

        // Close NameSpace
        if (namespaceClose)
        {
            indentLvl--;
            sb.Append($"{Helper.GetIndent(indentLvl)}}}{Options.GetNewLineText()}");
            sb.Append(Options.GetNewLineText());
        }

        // AfterNameSpace
        if (!string.IsNullOrWhiteSpace(afterNamespace))
        {
            sb.Append(afterNamespace);
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

    public string GetParamString(CppParameter parameter)
    {
        var sb = new LangStringWriter(Options);
        sb.Append(GetBeforePrint(parameter, 0));

        sb.Append($"{parameter.Type} {parameter.Name}");

        sb.Append(GetAfterPrint(parameter, 0));
        return sb.ToString();
    }

    public string GetFieldString(CppField field, bool declaration, int baseIndentLvl, CppStruct? parent = null)
    {
        var sb = new LangStringWriter(Options);

        sb.Append(GetBeforePrint(field, baseIndentLvl));

        // Comment
        sb.Append(GetMultiCommentString(field.Comments, baseIndentLvl, false));
        sb.Append(Helper.GetIndent(baseIndentLvl));

        var decSb = new LangStringWriter(Options);

        // Extern
        if (declaration && field.IsExtern)
        {
            decSb.Append("extern ");
        }

        // Static
        if (declaration && field.IsStatic)
        {
            decSb.Append("static ");
        }

        // Friend
        if (declaration && field.IsFriend)
        {
            decSb.Append("friend ");
        }

        // Const
        if (field.IsConst)
        {
            decSb.Append("const ");
        }

        // Constexpr
        if (declaration && field.IsConstexpr)
        {
            decSb.Append("constexpr ");
        }

        // Type
        decSb.Append(field.Type);
        sb.Append($"{decSb.ToString().PadRight(Options.VariableMemberTypePadSize)} ");

        var nameSb = new LangStringWriter(Options);

        // Parent name
        if (!declaration && parent is not null)
        {
            nameSb.Append($"{parent.Name}::");
        }

        // Name
        nameSb.Append(field.Name);

        // ArrayDim
        if (!string.IsNullOrWhiteSpace(field.ArrayDim))
        {
            nameSb.Append($"[{field.ArrayDim}]");
        }

        // Bitfield
        else if (!string.IsNullOrWhiteSpace(field.Bitfield))
        {
            nameSb.Append($" : {field.Bitfield}");
        }

        // Value
        if ((!string.IsNullOrWhiteSpace(field.DefaultValue) && !declaration) || (declaration && field.IsConstexpr))
        {
            nameSb.Append($" = {field.DefaultValue}");
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

    public string GetEnumString(CppEnum @enum, int baseIndentLvl)
    {
        var sb = new LangStringWriter(Options);
        sb.Append(GetBeforePrint(@enum, baseIndentLvl));

        // Comment
        sb.Append(GetMultiCommentString(@enum.Comments, baseIndentLvl, false));

        // Name
        sb.Append(
            @enum.IsClass
                ? $"{Helper.GetIndent(baseIndentLvl)}enum class {@enum.Name}"
                : $"{Helper.GetIndent(baseIndentLvl)}enum {@enum.Name}"
        );

        // Type
        if (!string.IsNullOrWhiteSpace(@enum.UnderlyingType))
        {
            sb.Append($" : {@enum.UnderlyingType}");
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
        if (@enum.Items.Count > 0)
        {
            int biggestName = @enum.Items.Max(ev => ev.Key.Length);
            IEnumerable<string> vals = @enum.Items.Select(
                ev =>
                {
                    string value = @enum.UseHexValues && long.TryParse(ev.Value, out long iValue) && iValue >= 0
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
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}}};{Options.GetNewLineText()}");

        sb.Append(GetAfterPrint(@enum, baseIndentLvl));
        return sb.ToString();
    }

    public string GetFunctionString(CppFunction func, CppStruct? parent, bool declaration, int baseIndentLvl)
    {
        bool forceAddBody = declaration;
        forceAddBody &= (parent is not null && parent.TemplateParams.Count > 0) ||
                        func.TemplateParameters.Count > 0 ||
                        func.IsFriend;

        var sb = new LangStringWriter(Options);
        sb.Append(GetBeforePrint(func, baseIndentLvl));

        // Comment
        if (!declaration)
        {
            sb.Append(GetMultiCommentString(func.Comments, baseIndentLvl, false));
        }

        // Template
        if (func.TemplateParameters.Count > 0)
        {
            sb.Append(Helper.GetIndent(baseIndentLvl));
            sb.Append($"template<{string.Join(", ", func.TemplateParameters)}>");
            sb.Append(Options.GetNewLineText());
        }

        sb.Append(Helper.GetIndent(baseIndentLvl));

        // Static
        if (declaration && func.IsStatic)
        {
            sb.Append("static ");
        }

        // Friend
        if (declaration && func.IsFriend)
        {
            sb.Append("friend ");
        }

        // Inline
        if (func.IsInline && declaration)
        {
            sb.Append("inline ");
        }

        // Type
        if (!string.IsNullOrWhiteSpace(func.Type))
        {
            sb.Append($"{func.Type} ");
        }

        // Name
        if (parent is not null &&
            parent.TemplateParams.Count == 0 &&
            !declaration &&
            func.TemplateParameters.Count == 0 &&
            !func.IsFriend)
        {
            sb.Append($"{parent.Name}::");
        }

        sb.Append(func.Name);

        // Params
        sb.Append('(');
        sb.Append(string.Join(", ", func.Parameters.Select(GetParamString)));
        sb.Append(')');

        // Const
        if (func.IsConst)
        {
            sb.Append(" const");
        }

        if (declaration && !forceAddBody)
        {
            sb.Append(';');
            sb.Append(GetAfterPrint(func, baseIndentLvl));
            sb.Append(Options.GetNewLineText());
            return sb.ToString();
        }

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

    public string GetStructString(CppStruct @struct, int baseIndentLvl)
    {
        var sb = new LangStringWriter(Options);
        sb.Append(GetBeforePrint(@struct, baseIndentLvl));

        // Comment
        sb.Append(GetMultiCommentString(@struct.Comments, baseIndentLvl, false));

        // Template
        if (@struct.TemplateParams.Count > 0)
        {
            sb.Append(Helper.GetIndent(baseIndentLvl));
            sb.Append($"template<{string.Join(", ", @struct.TemplateParams)}>{Options.GetNewLineText()}");
        }

        // Kind
        sb.Append(Helper.GetIndent(baseIndentLvl));
        if (@struct.IsClass)
        {
            sb.Append("class ");
        }
        else if (@struct.IsUnion)
        {
            sb.Append("union ");
        }
        else
        {
            sb.Append("struct ");
        }

        // Aligns
        if (@struct.Alignas > 0)
        {
            sb.Append($"__declspec(align({@struct.Alignas})) ");
        }

        // Name
        sb.Append(@struct.Name);

        // Supers
        if (@struct.Supers.Count > 0)
        {
            sb.Append($" : {Helper.JoinString(", ", @struct.Supers, "public ")}");
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

        bool isEmpty = @struct.Fields.Count == 0 && @struct.Friends.Count == 0 && @struct.Methods.Count == 0;
        if (isEmpty)
        {
            sb.Append(Options.GetNewLineText());
        }

        // Fields
        if (@struct.Fields.Count > 0)
        {
            sb.Append(Options.GetNewLineText());

            bool lastVarIsPrivate = false;
            bool lastVarIsUnion = false;
            List<CppField> fields = @struct.Fields.Where(v => !string.IsNullOrWhiteSpace(v.Name)).ToList();

            // Force write "private" or "public"
            if (fields.Count > 0)
            {
                lastVarIsPrivate = !fields[0].IsPrivate;
            }

            foreach (CppField f in fields)
            {
                // Private or Public
                if (f.IsPrivate != lastVarIsPrivate)
                {
                    lastVarIsPrivate = f.IsPrivate;
                    sb.Append($"{Helper.GetIndent(baseIndentLvl - 1)}");
                    sb.Append(f.IsPrivate ? "private:" : "public:");
                    sb.Append(Options.GetNewLineText());
                }

                // Close union
                if ((f.ForceUnion && lastVarIsUnion) || (!f.IsUnion && lastVarIsUnion))
                {
                    lastVarIsUnion = false;
                    baseIndentLvl--;
                    sb.Append($"{Helper.GetIndent(baseIndentLvl)}}};{Options.GetNewLineText()}");
                }

                // Open union
                if (f.IsUnion && !lastVarIsUnion)
                {
                    lastVarIsUnion = true;
                    sb.Append($"{Helper.GetIndent(baseIndentLvl)}union{Options.GetNewLineText()}");
                    sb.Append($"{Helper.GetIndent(baseIndentLvl)}{{{Options.GetNewLineText()}");
                    baseIndentLvl++;
                }

                // Print field
                sb.Append(GetFieldString(f, true, baseIndentLvl));
                sb.Append(Options.GetNewLineText());
            }

            // Close union
            if (lastVarIsUnion)
            {
                baseIndentLvl--;
                sb.Append($"{Helper.GetIndent(baseIndentLvl)}}};{Options.GetNewLineText()}");
            }
        }

        // Friends
        if (@struct.Friends.Count > 0)
        {
            sb.Append(Options.GetNewLineText());

            sb.Append(
                Helper.JoinString(
                    Options.GetNewLineText(),
                    @struct.Friends,
                    $"{Helper.GetIndent(baseIndentLvl)}friend ",
                    ";"
                )
            );
            sb.Append(Options.GetNewLineText());
        }

        // Methods
        if (@struct.Methods.Count > 0)
        {
            sb.Append(Options.GetNewLineText());

            bool lastMethodIsPrivate = false;
            List<CppFunction> methods = @struct.Methods.Where(m => !string.IsNullOrWhiteSpace(m.Name)).ToList();

            // Force write "private" or "public"
            if (methods.Count > 0)
            {
                lastMethodIsPrivate = !methods[0].IsPrivate;
            }

            foreach (CppFunction structMethod in methods)
            {
                // Private or Public
                if (structMethod.IsPrivate != lastMethodIsPrivate)
                {
                    lastMethodIsPrivate = structMethod.IsPrivate;
                    sb.Append($"{Helper.GetIndent(baseIndentLvl - 1)}");
                    sb.Append(structMethod.IsPrivate ? "private:" : "public:");
                    sb.Append(Options.GetNewLineText());
                }

                // Print method
                sb.Append(GetFunctionString(structMethod, @struct, declaration: true, baseIndentLvl));
            }
        }

        // Close struct scope
        baseIndentLvl--;
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}}};{Options.GetNewLineText()}");

        sb.Append(GetAfterPrint(@struct, baseIndentLvl));
        return sb.ToString();
    }

    public string GeneratePragmas(IEnumerable<string> pragmas, int baseIndentLvl)
    {
        string ret = Helper.JoinString(Options.GetNewLineText(), pragmas, $"{Helper.GetIndent(baseIndentLvl)}#pragma ");
        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateIncludes(IEnumerable<string> includes, int baseIndentLvl)
    {
        string ret = Helper.JoinString(
            Options.GetNewLineText(),
            includes,
            $"{Helper.GetIndent(baseIndentLvl)}#include "
        );
        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateDefines(IEnumerable<CppDefine> defines, int baseIndentLvl)
    {
        string ret = Helper.JoinString(
            Options.GetNewLineText(),
            defines.Select(d => d.Name + (string.IsNullOrWhiteSpace(d.Value) ? string.Empty : $" {d.Value}")),
            $"{Helper.GetIndent(baseIndentLvl)}#define "
        );

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateTypeDefs(IEnumerable<string> typeDefs, int baseIndentLvl)
    {
        string ret = Helper.JoinString(
            Options.GetNewLineText(),
            typeDefs,
            $"{Helper.GetIndent(baseIndentLvl)}typedef ",
            ";"
        );

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateForwards(IEnumerable<string> forwards, int baseIndentLvl)
    {
        IEnumerable<string> forwardsList = forwards.ToList();
        if (!forwardsList.Any())
        {
            return string.Empty;
        }

        string ret = Helper.JoinString(Options.GetNewLineText(), forwardsList, Helper.GetIndent(baseIndentLvl), ";");
        ret += Options.GetNewLineText();

        if (Options.PrintSectionName)
        {
            ret = GetSectionHeading("Forwards", baseIndentLvl) + ret;
        }

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateFields(IEnumerable<CppField> fields, bool declarationOnly, int baseIndentLvl)
    {
        List<CppField> vars = fields.Where(
                v => !string.IsNullOrWhiteSpace(v.Name) && !string.IsNullOrWhiteSpace(v.Type)
            )
            .ToList();

        if (vars.Count == 0)
        {
            return string.Empty;
        }

        string ret = string.Join(
            Options.GetNewLineText(),
            vars.Select(v => GetFieldString(v, declarationOnly, baseIndentLvl))
        );

        if (Options.PrintSectionName)
        {
            ret = GetSectionHeading("Global fields", baseIndentLvl) + ret;
        }

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateFunctions(
        IEnumerable<CppFunction> functions,
        CppStruct? parent,
        bool declarationOnly,
        int baseIndentLvl
    )
    {
        List<CppFunction> funcs = functions.Where(
                f => !string.IsNullOrWhiteSpace(f.Name) && !string.IsNullOrWhiteSpace(f.Type)
            )
            .ToList();

        if (funcs.Count == 0)
        {
            return string.Empty;
        }

        string ret = string.Join(
            Options.GetNewLineText(),
            funcs.Select(f => GetFunctionString(f, parent, declarationOnly, baseIndentLvl))
        );

        if (Options.PrintSectionName)
        {
            ret = GetSectionHeading("Global functions", baseIndentLvl) + ret;
        }

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateEnums(IEnumerable<CppEnum> enums, int baseIndentLvl)
    {
        List<CppEnum> vEnums = enums.Where(e => !string.IsNullOrWhiteSpace(e.Name)).ToList();

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

    public string GenerateConstants(IEnumerable<CppConstant> constants, int baseIndentLvl)
    {
        List<string> values = constants
            .Where(
                c => !string.IsNullOrWhiteSpace(c.Name) &&
                     !string.IsNullOrWhiteSpace(c.Type) &&
                     !string.IsNullOrWhiteSpace(c.Value)
            )
            .Select(c => $"static constexpr {c.Type} {c.Name} = {c.Value}")
            .ToList();

        if (values.Count == 0)
        {
            return string.Empty;
        }

        string ret = Helper.JoinString(Options.GetNewLineText(), values, Helper.GetIndent(baseIndentLvl), ";");

        if (Options.PrintSectionName && values.Count > 0)
        {
            ret = GetSectionHeading("Constants", baseIndentLvl) + ret;
        }

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateStructs(IEnumerable<CppStruct> structs, int baseIndentLvl)
    {
        List<CppStruct> vStruct = structs.Where(s => !string.IsNullOrWhiteSpace(s.Name)).ToList();

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
}
