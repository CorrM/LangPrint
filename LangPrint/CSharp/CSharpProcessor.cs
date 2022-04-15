using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LangPrint.Utils;
using Newtonsoft.Json;

namespace LangPrint.CSharp;

public class CSharpProcessor : ILangProcessor<CSharpPackage, CSharpLangOptions>
{
    public CSharpLangOptions Options { get; private set; } = null!;

    private string MakeCSharpFile(CSharpPackage package)
    {
        var sb = new StringBuilder();

        // File header
        sb.Append(GetFileHeader(package.HeadingComment, package.NameSpace, package.Usings, package.BeforeNameSpace, out int indentLvl));

        // Delegates
        if (package.Delegates.Count > 0)
            sb.Append(GenerateDelegates(package.Delegates, indentLvl, package.Conditions));

        // Enums
        if (package.Enums.Count > 0)
            sb.Append(GenerateEnums(package.Enums, indentLvl, package.Conditions));

        // Structs
        if (package.Structs.Count > 0)
            sb.Append(GenerateStructs(package.Structs, indentLvl, package.Conditions));

        // File footer
        sb.Append(GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

        return sb.ToString();
    }

    //private string MakeCppFile(CSharpPackage package)
    //{
    //    var sb = new StringBuilder();

    //    List<CSharpStruct> validStructs = package.Structs
    //        .Where(s => ResolveConditions(package.Conditions, s.Conditions))
    //        .ToList();

    //    // File header
    //    List<string> usings = package.Usings;

    //    // Don't change 'package.CppIncludes'
    //    if (Options.AddPackageHeaderToCppFile)
    //        usings = usings.Append($"\"{package.Name}.h\"").ToList();

    //    sb.Append(GetFileHeader(package.HeadingComment, package.NameSpace, usings, package.BeforeNameSpace, out int indentLvl));

    //    // Static fields
    //    IEnumerable<CSharpField> staticVars = validStructs
    //        .SelectMany(s => s.Fields)
    //        .Where(v => ResolveConditions(package.Conditions, v.Conditions));

    //    if (staticVars.Any(v => v.Static))
    //    {
    //        var varsStr = new List<string>();
    //        sb.Append(GetSectionHeading("Structs Static Fields", indentLvl));

    //        foreach (CSharpStruct @struct in validStructs)
    //        {
    //            List<string> variables = @struct.Fields
    //                .Where(v => v.Static && !v.Constexpr)
    //                .Select(v => GetFieldString(v, false, indentLvl, @struct))
    //                .ToList();

    //            varsStr.AddRange(variables);
    //        }

    //        sb.Append(Helper.JoinString(Options.GetNewLineText(), varsStr));
    //        sb.Append(Options.GetNewLineText() + Options.GetNewLineText());
    //    }

    //    // Global fields
    //    sb.Append(GenerateFields(package.Fields, false, indentLvl, package.Conditions));

    //    // Global functions
    //    sb.Append(GenerateFunctions(package.Functions.Where(f => f.TemplateParams.Count == 0), null, false, indentLvl, package.Conditions));

    //    // Structs functions
    //    if (validStructs.Any(s => s.Methods.Count > 0 && s.TemplateParams.Count == 0))
    //    {
    //        var methodsStr = new List<string>();
    //        sb.Append(GetSectionHeading("Structs functions", indentLvl));

    //        foreach (CSharpStruct @struct in validStructs.Where(s => s.TemplateParams.Count == 0))
    //        {
    //            int lvl = indentLvl;
    //            IEnumerable<string> methodsToAdd = @struct.Methods
    //                .Where(m => !string.IsNullOrWhiteSpace(m.Name) && ResolveConditions(package.Conditions, m.Conditions))
    //                .Where(m => !m.Friend && m.TemplateParams.Count == 0)
    //                .Select(structMethod => GetFunctionString(structMethod, @struct, false, lvl, package.Conditions));
    //            methodsStr.AddRange(methodsToAdd);
    //        }

    //        sb.Append(string.Join(Options.GetNewLineText(), methodsStr));
    //        sb.Append(Options.GetNewLineText());
    //    }

    //    // File footer
    //    sb.Append(GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

    //    return sb.ToString();
    //}

    //private string MakeStructsFile(CSharpPackage package)
    //{
    //    var sb = new StringBuilder();

    //    // File header
    //    sb.Append(GetFileHeader(package.HeadingComment, package.NameSpace, package.Usings, package.BeforeNameSpace, out int indentLvl));

    //    // Enums
    //    sb.Append(GenerateEnums(package.Enums, indentLvl, package.Conditions));

    //    // Structs
    //    sb.Append(GenerateStructs(package.Structs.Where(s => !s.IsClass), indentLvl, package.Conditions));

    //    // File footer
    //    sb.Append(GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

    //    return sb.ToString();
    //}

    //private string MakeClassesFile(CSharpPackage package)
    //{
    //    var sb = new StringBuilder();

    //    // File header
    //    sb.Append(GetFileHeader(package.HeadingComment, package.NameSpace, package.Usings, package.BeforeNameSpace, out int indentLvl));

    //    // Structs
    //    sb.Append(GenerateStructs(package.Structs.Where(s => s.IsClass), indentLvl, package.Conditions));

    //    // File footer
    //    sb.Append(GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

    //    return sb.ToString();
    //}

    //private string MakePackageHeaderFile(CSharpPackage package)
    //{
    //    var sb = new StringBuilder();

    //    // File header
    //    sb.Append(GetFileHeader(package.HeadingComment, package.NameSpace, package.Usings, package.BeforeNameSpace, out int indentLvl));

    //    // Forwards
    //    sb.Append(GenerateForwards(package.Forwards, indentLvl));

    //    // Constants
    //    sb.Append(GenerateConstants(package.Constants, indentLvl, package.Conditions));

    //    // Global fields
    //    sb.Append(GenerateFields(package.Fields, true, indentLvl, package.Conditions));

    //    // Global functions
    //    sb.Append(GenerateFunctions(package.Functions, null, true, indentLvl, package.Conditions));

    //    // File footer
    //    sb.Append(GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

    //    sb.Append(Options.GetNewLineText());

    //    // Package include
    //    var packHeaders = new List<string>()
    //    {
    //        $"\"{package.Name}_Structs.h\"",
    //        $"\"{package.Name}_Classes.h\""
    //    };
    //    packHeaders.AddRange(package.PackageHeaderIncludes);
    //    sb.Append(GenerateUsings(packHeaders, indentLvl));

    //    return sb.ToString();
    //}

    //private string MakePackageCppFile(CSharpPackage package)
    //{
    //    var sb = new StringBuilder();

    //    List<CSharpStruct> validStructs = package.Structs
    //        .Where(s => ResolveConditions(package.Conditions, s.Conditions))
    //        .ToList();

    //    // File header
    //    List<string> includes = package.CppIncludes;

    //    // Don't change 'package.CppIncludes'
    //    if (Options.AddPackageHeaderToCppFile)
    //        includes = includes.Append($"\"{package.Name}_Package.h\"").ToList();

    //    sb.Append(GetFileHeader(package.HeadingComment, package.NameSpace, null, includes, null, null, package.CppBeforeNameSpace, out int indentLvl));

    //    // Static fields
    //    IEnumerable<CSharpField> staticVars = validStructs
    //        .SelectMany(s => s.Fields)
    //        .Where(v => ResolveConditions(package.Conditions, v.Conditions));

    //    if (staticVars.Any(v => v.Static))
    //    {
    //        var varsStr = new List<string>();
    //        sb.Append(GetSectionHeading("Structs Static Fields", indentLvl));

    //        foreach (CSharpStruct @struct in validStructs)
    //        {
    //            List<string> variables = @struct.Fields
    //                .Where(v => ResolveConditions(package.Conditions, v.Conditions))
    //                .Where(v => v.Static && !v.Constexpr)
    //                .Select(v => GetFieldString(v, false, indentLvl, @struct))
    //                .ToList();

    //            varsStr.AddRange(variables);
    //        }

    //        sb.Append(Helper.JoinString(Options.GetNewLineText(), varsStr));
    //        sb.Append(Options.GetNewLineText() + Options.GetNewLineText());
    //    }

    //    // Global fields
    //    sb.Append(GenerateFields(package.Fields, false, indentLvl, package.Conditions));

    //    // Global functions
    //    sb.Append(GenerateFunctions(package.Functions.Where(f => f.TemplateParams.Count == 0), null, false, indentLvl, package.Conditions));

    //    // Structs functions
    //    if (validStructs.Any(s => s.Methods.Count > 0 && s.TemplateParams.Count == 0))
    //    {
    //        var methodsStr = new List<string>();
    //        sb.Append(GetSectionHeading("Structs Functions", indentLvl));

    //        foreach (CSharpStruct @struct in validStructs.Where(s => s.TemplateParams.Count == 0))
    //        {
    //            int lvl = indentLvl;
    //            IEnumerable<string> methodsToAdd = @struct.Methods
    //                .Where(m => !string.IsNullOrWhiteSpace(m.Name) && ResolveConditions(package.Conditions, m.Conditions))
    //                .Where(m => !m.Friend && m.TemplateParams.Count == 0)
    //                .Select(structMethod => GetFunctionString(structMethod, @struct, false, lvl, package.Conditions));
    //            methodsStr.AddRange(methodsToAdd);
    //        }

    //        sb.Append(string.Join(Options.GetNewLineText(), methodsStr));
    //        sb.Append(Options.GetNewLineText());
    //    }

    //    // File footer
    //    sb.Append(GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

    //    return sb.ToString();
    //}

    public bool ResolveConditions(List<string> conditions, List<string> conditionsToResolve)
    {
        if (!Options.ResolveConditions)
            return true;

        return LangPrint.ResolveConditions(conditions, conditionsToResolve);
    }

    public string GetFileHeader(IEnumerable<string> headingComment, string nameSpace, List<string>? usings, string beforeNameSpace, out int indentLvl)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        indentLvl = 0;
        var sb = new StringBuilder();

        // HeadingComment
        sb.Append(GetMultiCommentString(headingComment, indentLvl));

        // Includes
        if (usings?.Count > 0)
            sb.Append(GenerateUsings(usings, indentLvl));

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
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        var sb = new StringBuilder();

        // Close NameSpace
        if (!string.IsNullOrWhiteSpace(nameSpace))
        {
            indentLvl--;
            sb.Append($"{Helper.GetIndent(indentLvl)}}}{Options.GetNewLineText()}");
            sb.Append(Options.GetNewLineText());
        }

        // AfterNameSpace
        if (!string.IsNullOrWhiteSpace(afterNameSpace))
            sb.Append(afterNameSpace);

        sb.Append(Options.GetNewLineText());

        return sb.ToString();
    }

    public string GetSectionHeading(string name, int indentLvl)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        string headLine = string.Concat(Enumerable.Repeat("-", 50));

        return Helper.GetIndent(indentLvl) + "// " + headLine + Options.GetNewLineText() +
               Helper.GetIndent(indentLvl) + "// " + "# " + name + Options.GetNewLineText() +
               Helper.GetIndent(indentLvl) + "// " + headLine + Options.GetNewLineText();
    }

    public string GetMultiCommentString(IEnumerable<string>? comments, int baseIndentLvl, bool finalizeReturn = true)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        if (comments is null)
            return string.Empty;

        IEnumerable<string> eComments = comments.ToList();
        if (!eComments.Any())
            return string.Empty;

        string ret = $"{Helper.GetIndent(baseIndentLvl)}/**{Options.GetNewLineText()}" +
                     Helper.JoinString(Options.GetNewLineText(), eComments, $"{Helper.GetIndent(baseIndentLvl)} * ") +
                     $"{Options.GetNewLineText()}{Helper.GetIndent(baseIndentLvl)} */";

        return finalizeReturn
            ? Helper.FinalizeSection(ret, Options.GetNewLineText())
            : ret + Options.GetNewLineText();
    }

    public string GetAttributeString(CSharpAttribute attribute, int baseIndentLvl)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        var sb = new StringBuilder();

        // Comment
        sb.Append(GetMultiCommentString(attribute.Comments, baseIndentLvl, false));

        // Indent
        sb.Append(Helper.GetIndent(baseIndentLvl));

        // Open
        sb.Append('[');

        // AppliesTo
        if (!string.IsNullOrWhiteSpace(attribute.AppliesTo))
            sb.Append($"{attribute.AppliesTo}: ");

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
            sb.Append($" // {attribute.InlineComment}");

        return sb.ToString();
    }

    public string GetDelegateString(CSharpDelegate @delegate, int baseIndentLvl, List<string> modelConditions)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        var sb = new StringBuilder();

        // Comment
        sb.Append(GetMultiCommentString(@delegate.Comments, baseIndentLvl, false));

        // Attributes
        if (@delegate.Attributes.Count > 0)
        {
            sb.Append(GenerateAttributes(@delegate.Attributes, baseIndentLvl, modelConditions));
            sb.Append(Options.GetNewLineText());
        }

        // Indent
        sb.Append(Helper.GetIndent(baseIndentLvl));

        // Access
        if (!string.IsNullOrWhiteSpace(@delegate.AccessModifier))
            sb.Append($"{@delegate.AccessModifier} ");

        // Delegate keyword
        sb.Append("delegate ");

        // Type
        if (!string.IsNullOrWhiteSpace(@delegate.Type))
            sb.Append($"{@delegate.Type} ");

        // Name
        sb.Append(@delegate.Name);

        // Generic
        if (@delegate.GenericParams.Count > 0)
            sb.Append($"<{string.Join(", ", @delegate.GenericParams)}>");

        // Params
        sb.Append('(');
        sb.Append(string.Join(", ", @delegate.Params.Where(p => ResolveConditions(modelConditions, p.Conditions)).Select(GetParamString)));
        sb.Append(");");

        // Inline comment
        if (!string.IsNullOrEmpty(@delegate.InlineComment))
            sb.Append($" // {@delegate.InlineComment}");

        sb.Append(Options.GetNewLineText());

        return sb.ToString();
    }

    public string GetParamString(CSharpParameter parameter)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        var sb = new StringBuilder();

        // Attributes
        if (parameter.Attributes.Count > 0)
        {
            sb.Append(GenerateAttributes(parameter.Attributes, 0, parameter.Conditions));
            sb.Append(' ');
        }

        // Param
        sb.Append($"{parameter.Type} {parameter.Name}");

        return sb.ToString();
    }

    public string GetEventString(CSharpEvent @event, int baseIndentLvl, List<string> conditions)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        var sb = new StringBuilder();

        // Comment
        sb.Append(GetMultiCommentString(@event.Comments, baseIndentLvl, false));

        // Attributes
        if (@event.Attributes.Count > 0)
        {
            sb.Append(GenerateAttributes(@event.Attributes, baseIndentLvl, conditions));
            sb.Append(Options.GetNewLineText());
        }

        // Indent
        sb.Append(Helper.GetIndent(baseIndentLvl));

        var firstPartSb = new StringBuilder();

        // Access
        if (!string.IsNullOrWhiteSpace(@event.AccessModifier))
            firstPartSb.Append($"{@event.AccessModifier} ");

        // Static
        if (@event.IsStatic)
            firstPartSb.Append("static ");

        // Event keyword
        firstPartSb.Append("event ");

        // Type
        firstPartSb.Append(@event.Type);

        // Type padding
        string typePrefix = firstPartSb.ToString();
        sb.Append($"{typePrefix.PadRight(Options.FieldMemberTypePadSize)} ");

        var nameSb = new StringBuilder();

        // Name
        nameSb.Append($"{@event.Name};");

        // Inline comment
        if (!string.IsNullOrEmpty(@event.InlineComment))
            sb.Append(nameSb.ToString().PadRight(Options.InlineCommentPadSize) + $" // {@event.InlineComment}");
        else
            sb.Append(nameSb);

        return sb.ToString();
    }

    public string GetFieldString(CSharpField field, int baseIndentLvl, List<string> conditions)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        var sb = new StringBuilder();

        // Comment
        sb.Append(GetMultiCommentString(field.Comments, baseIndentLvl, false));

        // Attributes
        if (field.Attributes.Count > 0)
        {
            sb.Append(GenerateAttributes(field.Attributes, baseIndentLvl, conditions));
            sb.Append(Options.GetNewLineText());
        }

        // Indent
        sb.Append(Helper.GetIndent(baseIndentLvl));

        var firstPartSb = new StringBuilder();

        // Access
        if (!string.IsNullOrWhiteSpace(field.AccessModifier))
            firstPartSb.Append($"{field.AccessModifier} ");

        // Static
        if (field.IsStatic)
            firstPartSb.Append("static ");

        // ReadOnly
        if (field.IsReadOnly)
            firstPartSb.Append("readonly ");

        // Type
        firstPartSb.Append(field.Type);

        // Array
        if (field.IsArray)
            firstPartSb.Append("[]");

        // Type padding
        string typePrefix = firstPartSb.ToString();
        sb.Append($"{typePrefix.PadRight(Options.FieldMemberTypePadSize)} ");

        var nameSb = new StringBuilder();

        // Name
        nameSb.Append(field.Name);

        // Value
        if (!string.IsNullOrWhiteSpace(field.Value))
            nameSb.Append($" = {field.Value}");

        nameSb.Append(';');

        // Inline comment
        if (!string.IsNullOrEmpty(field.InlineComment))
            sb.Append(nameSb.ToString().PadRight(Options.InlineCommentPadSize) + $" // {field.InlineComment}");
        else
            sb.Append(nameSb);

        return sb.ToString();
    }

    public string GetEnumString(CSharpEnum @enum, int baseIndentLvl)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        var sb = new StringBuilder();

        // Comment
        sb.Append(GetMultiCommentString(@enum.Comments, baseIndentLvl, false));

        // Attributes
        if (@enum.Attributes.Count > 0)
        {
            sb.Append(GenerateAttributes(@enum.Attributes, baseIndentLvl, @enum.Conditions));
            sb.Append(Options.GetNewLineText());
        }

        // Indent
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}");

        // Access
        if (!string.IsNullOrWhiteSpace(@enum.AccessModifier))
            sb.Append($"{@enum.AccessModifier} ");

        // Name
        sb.Append($"enum {@enum.Name}");

        // Type
        if (!string.IsNullOrWhiteSpace(@enum.Type))
            sb.Append($" : {@enum.Type}");

        // Inline comment
        if (!string.IsNullOrEmpty(@enum.InlineComment))
            sb.Append($" // {@enum.InlineComment}");

        sb.Append(Options.GetNewLineText());

        // Open
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}{{{Options.GetNewLineText()}");
        baseIndentLvl++;

        // Values
        if (@enum.Values.Count > 0)
        {
            int biggestName = @enum.Values.Max(ev => ev.Name.Length);
            IEnumerable<string> vals = @enum.Values.Select(ev =>
            {
                string value = @enum.HexValues && long.TryParse(ev.Value, out long iValue) && iValue >= 0
                    ? $"0x{iValue:X16}"
                    : ev.Value;

                return $"{ev.Name.PadRight(biggestName)} = {value}";
            });

            string values = Helper.JoinString(
                "," + Options.GetNewLineText(),
                vals,
                Helper.GetIndent(baseIndentLvl));

            sb.Append(values);
        }

        sb.Append(Options.GetNewLineText());

        // Close
        baseIndentLvl--;
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}}};{Options.GetNewLineText()}");

        return sb.ToString();
    }

    public string GetFunctionString(CSharpFunction func, int baseIndentLvl, List<string> modelConditions)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        var sb = new StringBuilder();

        // Comment
        sb.Append(GetMultiCommentString(func.Comments, baseIndentLvl, false));

        // Attributes
        if (func.Attributes.Count > 0)
        {
            sb.Append(GenerateAttributes(func.Attributes, baseIndentLvl, modelConditions));
            sb.Append(Options.GetNewLineText());
        }

        // Indent
        sb.Append(Helper.GetIndent(baseIndentLvl));

        // Access
        if (!string.IsNullOrWhiteSpace(func.AccessModifier))
            sb.Append($"{func.AccessModifier} ");

        // Static
        if (func.IsStatic)
            sb.Append("static ");

        // Type
        if (!string.IsNullOrWhiteSpace(func.Type))
            sb.Append($"{func.Type} ");

        // Name
        sb.Append(func.Name);

        // Generic
        if (func.GenericParams.Count > 0)
            sb.Append($"<{string.Join(", ", func.GenericParams)}>");

        // Params
        sb.Append('(');
        sb.Append(string.Join(", ", func.Params.Where(p => ResolveConditions(modelConditions, p.Conditions)).Select(GetParamString)));
        sb.Append(')');

        // Inline comment
        if (!string.IsNullOrEmpty(func.InlineComment))
            sb.Append($" // {func.InlineComment}");

        sb.Append(Options.GetNewLineText());

        // Body
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}{{{Options.GetNewLineText()}");
        baseIndentLvl++;

        sb.Append(Helper.JoinString(Options.GetNewLineText(), func.Body, Helper.GetIndent(baseIndentLvl)));

        baseIndentLvl--;
        sb.Append($"{Options.GetNewLineText()}{Helper.GetIndent(baseIndentLvl)}}}{Options.GetNewLineText()}");

        return sb.ToString();
    }

    public string GetStructString(CSharpStruct @struct, int baseIndentLvl, List<string> conditions)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        var sb = new StringBuilder();

        // Comment
        sb.Append(GetMultiCommentString(@struct.Comments, baseIndentLvl, false));

        // Attributes
        if (@struct.Attributes.Count > 0)
        {
            sb.Append(GenerateAttributes(@struct.Attributes, baseIndentLvl, conditions));
            sb.Append(Options.GetNewLineText());
        }

        // Indent
        sb.Append(Helper.GetIndent(baseIndentLvl));

        // Access
        if (!string.IsNullOrWhiteSpace(@struct.AccessModifier))
            sb.Append($"{@struct.AccessModifier} ");

        // ReadOnly
        if (/*!@struct.IsClass && */@struct.IsReadOnly)
            sb.Append("readonly ");

        // Static
        if (@struct.IsStatic)
            sb.Append("static ");

        // Kind
        if (@struct.IsClass)
            sb.Append("class ");
        else
            sb.Append("struct ");

        // Name
        sb.Append(@struct.Name);

        // Generic
        if (@struct.GenericParams.Count > 0)
            sb.Append($"<{string.Join(", ", @struct.GenericParams)}>");

        if (!string.IsNullOrWhiteSpace(@struct.Super) || @struct.Interfaces.Count > 0)
            sb.Append(" : ");

        // Super
        if (!string.IsNullOrWhiteSpace(@struct.Super))
            sb.Append(@struct.Super);

        // Interfaces
        if (@struct.Interfaces.Count > 0)
        {
            if (!string.IsNullOrWhiteSpace(@struct.Super))
                sb.Append(", ");

            sb.Append($"{Helper.JoinString(", ", @struct.Interfaces)}");
        }

        // Inline comment
        if (!string.IsNullOrEmpty(@struct.InlineComment))
            sb.Append($" // {@struct.InlineComment}");

        sb.Append(Options.GetNewLineText());

        // Open struct scope
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}{{");
        baseIndentLvl++;

        if (@struct.Fields.Count > 0 || @struct.Methods.Count > 0)
            sb.Append(Options.GetNewLineText());

        // Delegates
        if (@struct.Delegates.Count > 0)
            sb.Append(GenerateDelegates(@struct.Delegates, baseIndentLvl, conditions));

        // Events
        if (@struct.Events.Count > 0)
            sb.Append(GenerateEvents(@struct.Events, baseIndentLvl, conditions));

        // Fields
        if (@struct.Fields.Count > 0)
            sb.Append(GenerateFields(@struct.Fields, baseIndentLvl, conditions));

        // Methods
        if (@struct.Methods.Count > 0)
            sb.Append(GenerateFunctions(@struct.Methods, baseIndentLvl, conditions));

        // Close struct scope
        baseIndentLvl--;
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}}};{Options.GetNewLineText()}");

        return sb.ToString();
    }

    public string GenerateUsings(IEnumerable<string> usings, int baseIndentLvl)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        string ret = Helper.JoinString(Options.GetNewLineText(), usings, $"{Helper.GetIndent(baseIndentLvl)}using ", ";");
        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateAttributes(IEnumerable<CSharpAttribute> attributes, int baseIndentLvl, List<string> conditions)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        List<CSharpAttribute> attribute = attributes
            .Where(v => !string.IsNullOrWhiteSpace(v.Name) && ResolveConditions(conditions, v.Conditions))
            .ToList();

        if (attribute.Count == 0)
            return string.Empty;

        string ret = string.Join(Options.GetNewLineText(), attribute.Select(a => GetAttributeString(a, baseIndentLvl)));

        return ret;
    }

    public string GenerateDelegates(IEnumerable<CSharpDelegate> delegates, int baseIndentLvl, List<string> conditions)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        List<CSharpDelegate> dels = delegates
            .Where(f => !string.IsNullOrWhiteSpace(f.Name) && !string.IsNullOrWhiteSpace(f.Type) && ResolveConditions(conditions, f.Conditions))
            .ToList();

        if (dels.Count == 0)
            return string.Empty;

        string ret = string.Join(Options.GetNewLineText(), dels.Select(f => GetDelegateString(f, baseIndentLvl, conditions)));

        if (Options.PrintSectionName)
            ret = GetSectionHeading("Delegates", baseIndentLvl) + ret;

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateEvents(IEnumerable<CSharpEvent> events, int baseIndentLvl, List<string> conditions)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        List<CSharpEvent> evs = events
            .Where(f => !string.IsNullOrWhiteSpace(f.Name) && !string.IsNullOrWhiteSpace(f.Type) && ResolveConditions(conditions, f.Conditions))
            .ToList();

        if (evs.Count == 0)
            return string.Empty;

        string ret = string.Join(Options.GetNewLineText(), evs.Select(f => GetEventString(f, baseIndentLvl, conditions)));

        if (Options.PrintSectionName)
            ret = GetSectionHeading("Events", baseIndentLvl) + ret;

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateFields(IEnumerable<CSharpField> fields, int baseIndentLvl, List<string> conditions)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        List<CSharpField> vars = fields
            .Where(v => !string.IsNullOrWhiteSpace(v.Name) && !string.IsNullOrWhiteSpace(v.Type) && ResolveConditions(conditions, v.Conditions))
            .ToList();

        if (vars.Count == 0)
            return string.Empty;

        string ret = string.Join(Options.GetNewLineText(), vars.Select(v => GetFieldString(v, baseIndentLvl, conditions)));

        if (Options.PrintSectionName)
            ret = GetSectionHeading("Fields", baseIndentLvl) + ret;

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateFunctions(IEnumerable<CSharpFunction> functions, int baseIndentLvl, List<string> conditions)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        List<CSharpFunction> funcs = functions
            .Where(f => !string.IsNullOrWhiteSpace(f.Name) && !string.IsNullOrWhiteSpace(f.Type) && ResolveConditions(conditions, f.Conditions))
            .ToList();

        if (funcs.Count == 0)
            return string.Empty;

        string ret = string.Join(Options.GetNewLineText(), funcs.Select(f => GetFunctionString(f, baseIndentLvl, conditions)));

        if (Options.PrintSectionName)
            ret = GetSectionHeading("Functions", baseIndentLvl) + ret;

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateEnums(IEnumerable<CSharpEnum> enums, int baseIndentLvl, List<string> conditions)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        List<CSharpEnum> vEnums = enums
            .Where(e => !string.IsNullOrWhiteSpace(e.Name) && ResolveConditions(conditions, e.Conditions))
            .ToList();

        if (vEnums.Count == 0)
            return string.Empty;

        string ret = string.Join(Options.GetNewLineText(), vEnums.Select(e => GetEnumString(e, baseIndentLvl)));

        if (Options.PrintSectionName)
            ret = GetSectionHeading("Enums", baseIndentLvl) + ret;

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateStructs(IEnumerable<CSharpStruct> structs, int baseIndentLvl, List<string> conditions)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        List<CSharpStruct> vStruct = structs
            .Where(s => !string.IsNullOrWhiteSpace(s.Name) && ResolveConditions(conditions, s.Conditions))
            .ToList();

        if (vStruct.Count == 0)
            return string.Empty;

        string ret = string.Join(Options.GetNewLineText(), vStruct.Select(s => GetStructString(s, baseIndentLvl, conditions)));

        if (Options.PrintSectionName)
            ret = GetSectionHeading(vStruct.All(s => s.IsClass) ? "Classes" : "Structs", baseIndentLvl) + ret;

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public void Init(CSharpLangOptions? options = null)
    {
        Options = options ?? new CSharpLangOptions();
    }

    public CSharpPackage ModelFromJson(string jsonData)
    {
        return JsonConvert.DeserializeObject<CSharpPackage>(jsonData);
    }

    public Dictionary<string, string> GenerateFiles(CSharpPackage cSharpPackage)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        cSharpPackage.Conditions = cSharpPackage.Conditions.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

        var ret = new Dictionary<string, string>();

        if (!Options.GeneratePackageSyntax)
        {
            ret.Add($"{cSharpPackage.Name}.cs", MakeCSharpFile(cSharpPackage));

            return ret;
        }

        //ret.Add($"{cSharpPackage.Name}_Structs.cs", MakeStructsFile(cSharpPackage));
        //ret.Add($"{cSharpPackage.Name}_Classes.cs", MakeClassesFile(cSharpPackage));

        return ret;
    }
}
