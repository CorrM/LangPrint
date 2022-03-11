using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LangPrint.Utils;
using Newtonsoft.Json;

namespace LangPrint.Cpp;

// Todo: add virtual functions to CppStruct
public class CppProcessor : ILangProcessor<CppPackage, CppLangOptions>
{
    public CppLangOptions Options { get; private set; }

    private string MakeHeaderFile(CppPackage package)
    {
        var sb = new StringBuilder();

        // File header
        sb.Append(GetFileHeader(package.HeadingComment, package.NameSpace, package.Pragmas, package.Includes, package.Defines, package.BeforeNameSpace, out int indentLvl));

        // Forwards
        sb.Append(GenerateForwards(package.Forwards, indentLvl));

        // Constants
        sb.Append(GenerateConstants(package.Constants, indentLvl, package.Conditions));

        // Variables
        sb.Append(GenerateVariables(package.Variables, indentLvl, package.Conditions));

        // Global functions
        sb.Append(GenerateFunctions(package.Functions, null, true, indentLvl, package.Conditions));

        // Enums
        sb.Append(GenerateEnums(package.Enums, indentLvl, package.Conditions));

        // Structs
        sb.Append(GenerateStructs(package.Structs, indentLvl, package.Conditions));

        // File footer
        sb.Append(GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

        return sb.ToString();
    }

    private string MakeCppFile(CppPackage package)
    {
        var sb = new StringBuilder();

        List<CppStruct> validStructs = package.Structs
            .Where(s => ResolveConditions(package.Conditions, s.Conditions))
            .ToList();

        // File header
        List<string> includes = package.CppIncludes;

        // Don't change 'package.CppIncludes'
        if (Options.AddPackageHeaderToCppFile)
            includes = includes.Append($"\"{package.Name}.h\"").ToList();

        sb.Append(GetFileHeader(package.HeadingComment, package.NameSpace, null, includes, null, package.CppBeforeNameSpace, out int indentLvl));

        // Static variables
        IEnumerable<CppVariable> staticVars = validStructs
            .SelectMany(s => s.Variables)
            .Where(v => ResolveConditions(package.Conditions, v.Conditions));

        if (staticVars.Any(v => v.Static))
        {
            var varsStr = new List<string>();
            sb.Append(GetSectionHeading("Structs Static Variables", indentLvl));

            foreach (CppStruct @struct in validStructs)
            {
                List<string> variables = @struct.Variables
                    .Where(v => v.Static && !v.Constexpr)
                    .Select(v => GetVariableString(v, indentLvl, @struct, true))
                    .ToList();

                varsStr.AddRange(variables);
            }

            sb.Append(Helper.JoinString(Options.GetNewLineText(), varsStr));
            sb.Append(Options.GetNewLineText() + Options.GetNewLineText());
        }

        // Global functions
        sb.Append(GenerateFunctions(package.Functions.Where(f => f.TemplateParams.Count == 0), null, false, indentLvl, package.Conditions));

        // Structs functions
        if (validStructs.Any(s => s.Methods.Count > 0))
        {
            var methodsStr = new List<string>();
            sb.Append(GetSectionHeading("Structs Functions", indentLvl));

            foreach (CppStruct @struct in validStructs.Where(s => s.TemplateParams.Count == 0))
            {
                int lvl = indentLvl;
                IEnumerable<string> methodsToAdd = @struct.Methods
                    .Where(m => !string.IsNullOrWhiteSpace(m.Name) && ResolveConditions(package.Conditions, m.Conditions))
                    .Where(m => !m.Friend && m.TemplateParams.Count == 0)
                    .Select(structMethod => GetFunctionString(structMethod, @struct, false, lvl, package.Conditions));
                methodsStr.AddRange(methodsToAdd);
            }

            sb.Append(string.Join(Options.GetNewLineText(), methodsStr));
            sb.Append(Options.GetNewLineText());
        }

        // File footer
        sb.Append(GetFileFooter(package.NameSpace, package.CppAfterNameSpace, ref indentLvl));

        return sb.ToString();
    }

    private string MakeStructsFile(CppPackage package)
    {
        var sb = new StringBuilder();

        var pragmas = new List<string>()
            {
                "once"
            };

        // File header
        sb.Append(GetFileHeader(package.HeadingComment, package.NameSpace, pragmas, null, null, package.BeforeNameSpace, out int indentLvl));

        // Constants
        sb.Append(GenerateConstants(package.Constants, indentLvl, package.Conditions));

        // Enums
        sb.Append(GenerateEnums(package.Enums, indentLvl, package.Conditions));

        // Structs
        sb.Append(GenerateStructs(package.Structs.Where(s => !s.IsClass), indentLvl, package.Conditions));

        // File footer
        sb.Append(GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

        return sb.ToString();
    }

    private string MakeClassesFile(CppPackage package)
    {
        var sb = new StringBuilder();

        var pragmas = new List<string>()
            {
                "once"
            };

        // File header
        sb.Append(GetFileHeader(package.HeadingComment, package.NameSpace, pragmas, null, null, package.BeforeNameSpace, out int indentLvl));

        // Structs
        sb.Append(GenerateStructs(package.Structs.Where(s => s.IsClass), indentLvl, package.Conditions));

        // File footer
        sb.Append(GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

        return sb.ToString();
    }

    private string MakePackageHeaderFile(CppPackage package)
    {
        var sb = new StringBuilder();

        // File header
        sb.Append(GetFileHeader(package.HeadingComment, package.NameSpace, package.Pragmas, package.Includes, package.Defines, package.BeforeNameSpace, out int indentLvl));

        // Forwards
        sb.Append(GenerateForwards(package.Forwards, indentLvl));

        // Constants
        sb.Append(GenerateConstants(package.Constants, indentLvl, package.Conditions));

        // Variables
        sb.Append(GenerateVariables(package.Variables, indentLvl, package.Conditions));

        // Global functions
        sb.Append(GenerateFunctions(package.Functions, null, true, indentLvl, package.Conditions));

        // File footer
        sb.Append(GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

        sb.Append(Options.GetNewLineText());

        // Package include
        var packHeaders = new List<string>()
        {
            $"\"{package.Name}_Structs.h\"",
            $"\"{package.Name}_Classes.h\""
        };
        packHeaders.AddRange(package.PackageHeaderIncludes);
        sb.Append(GenerateIncludes(packHeaders, indentLvl));

        return sb.ToString();
    }

    private string MakePackageCppFile(CppPackage package)
    {
        var sb = new StringBuilder();

        List<CppStruct> validStructs = package.Structs
            .Where(s => ResolveConditions(package.Conditions, s.Conditions))
            .ToList();

        // File header
        List<string> includes = package.CppIncludes;

        // Don't change 'package.CppIncludes'
        if (Options.AddPackageHeaderToCppFile)
            includes = includes.Append($"\"{package.Name}_Package.h\"").ToList();

        sb.Append(GetFileHeader(package.HeadingComment, package.NameSpace, null, includes, null, package.CppBeforeNameSpace, out int indentLvl));

        // Static variables
        IEnumerable<CppVariable> staticVars = validStructs
            .SelectMany(s => s.Variables)
            .Where(v => ResolveConditions(package.Conditions, v.Conditions));

        if (staticVars.Any(v => v.Static))
        {
            var varsStr = new List<string>();
            sb.Append(GetSectionHeading("Structs Static Variables", indentLvl));

            foreach (CppStruct @struct in validStructs)
            {
                List<string> variables = @struct.Variables
                    .Where(v => ResolveConditions(package.Conditions, v.Conditions))
                    .Where(v => v.Static && !v.Constexpr)
                    .Select(v => GetVariableString(v, indentLvl, @struct, true))
                    .ToList();

                varsStr.AddRange(variables);
            }

            sb.Append(Helper.JoinString(Options.GetNewLineText(), varsStr));
            sb.Append(Options.GetNewLineText() + Options.GetNewLineText());
        }

        // Global functions
        sb.Append(GenerateFunctions(package.Functions.Where(f => f.TemplateParams.Count == 0), null, false, indentLvl, package.Conditions));

        // Structs functions
        if (validStructs.Any(s => s.Methods.Count > 0))
        {
            var methodsStr = new List<string>();
            sb.Append(GetSectionHeading("Structs Functions", indentLvl));

            foreach (CppStruct @struct in validStructs.Where(s => s.TemplateParams.Count == 0))
            {
                int lvl = indentLvl;
                IEnumerable<string> methodsToAdd = @struct.Methods
                    .Where(m => !string.IsNullOrWhiteSpace(m.Name) && ResolveConditions(package.Conditions, m.Conditions))
                    .Where(m => !m.Friend && m.TemplateParams.Count == 0)
                    .Select(structMethod => GetFunctionString(structMethod, @struct, false, lvl, package.Conditions));
                methodsStr.AddRange(methodsToAdd);
            }

            sb.Append(string.Join(Options.GetNewLineText(), methodsStr));
            sb.Append(Options.GetNewLineText());
        }

        // File footer
        sb.Append(GetFileFooter(package.NameSpace, package.CppAfterNameSpace, ref indentLvl));

        return sb.ToString();
    }

    public bool ResolveConditions(List<string> conditions, List<string> conditionsToResolve)
    {
        if (!Options.ResolveConditions)
            return true;

        return LangPrint.ResolveConditions(conditions, conditionsToResolve);
    }

    public string GetFileHeader(IEnumerable<string> headingComment, string nameSpace, List<string> pragmas, List<string> includes, List<CppDefine> defines, string beforeNameSpace, out int indentLvl)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        indentLvl = 0;
        var sb = new StringBuilder();

        // Pragmas
        if (pragmas?.Count > 0)
            sb.Append(GeneratePragmas(pragmas, indentLvl));

        // HeadingComment
        sb.Append(GetMultiCommentString(headingComment, indentLvl));

        // Includes
        if (includes?.Count > 0)
            sb.Append(GenerateIncludes(includes, indentLvl));

        // Defines
        if (defines?.Count > 0)
            sb.Append(GenerateDefines(defines, indentLvl));

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

    public string GetMultiCommentString(IEnumerable<string> comments, int baseIndentLvl, bool finalizeReturn = true)
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

    public string GetParamString(CppParameter parameter)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        return $"{parameter.Type} {parameter.Name}";
    }

    public string GetVariableString(CppVariable variable, int baseIndentLvl, CppStruct parent = null, bool definition = false)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        var sb = new StringBuilder();

        // Comment
        sb.Append(GetMultiCommentString(variable.Comments, baseIndentLvl, false));

        sb.Append(Helper.GetIndent(baseIndentLvl));

        // Extern
        if (!definition && variable.Extern)
            sb.Append("extern ");

        // Static
        if (!definition && variable.Static)
            sb.Append("static ");

        // Friend
        if (!definition && variable.Friend)
            sb.Append("friend ");

        // Const
        if (variable.Const)
            sb.Append("const ");

        // Constexpr
        if (!definition && variable.Constexpr)
            sb.Append("constexpr ");

        // Type
        sb.Append(variable.Type);
        string prefix = sb.ToString();
        sb.Clear();
        sb.Append($"{prefix.PadRight(Options.VariableMemberTypePadSize)} ");

        var nameSb = new StringBuilder();

        // Parent name
        if (definition && parent is not null)
            nameSb.Append($"{parent.Name}::");

        // Name
        nameSb.Append(variable.Name);

        // ArrayDim
        if (!string.IsNullOrWhiteSpace(variable.ArrayDim))
            nameSb.Append($"[{variable.ArrayDim}]");

        // Bitfield
        else if (!string.IsNullOrWhiteSpace(variable.Bitfield))
            nameSb.Append($" : {variable.Bitfield}");

        // Value
        if ((!string.IsNullOrWhiteSpace(variable.Value) && definition) || (!definition && variable.Constexpr))
            nameSb.Append($" = {variable.Value}");

        nameSb.Append(';');

        // Inline comment
        if (!string.IsNullOrEmpty(variable.InlineComment))
            sb.Append(nameSb.ToString().PadRight(Options.InlineCommentPadSize) + $" // {variable.InlineComment}");
        else
            sb.Append(nameSb);

        return sb.ToString();
    }

    public string GetEnumString(CppEnum @enum, int baseIndentLvl)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        var sb = new StringBuilder();

        // Comment
        sb.Append(GetMultiCommentString(@enum.Comments, baseIndentLvl, false));

        // Name
        sb.Append(@enum.IsClass
            ? $"{Helper.GetIndent(baseIndentLvl)}enum class {@enum.Name}"
            : $"{Helper.GetIndent(baseIndentLvl)}enum {@enum.Name}");

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
        int biggestName = @enum.Values.Max(ev => ev.Name.Length);
        string values = Helper.JoinString(
            "," + Options.GetNewLineText(),
            @enum.Values.Select(ev => $"{ev.Name.PadRight(biggestName)} = {ev.Value}"),
            Helper.GetIndent(baseIndentLvl));

        sb.Append(values);
        sb.Append(Options.GetNewLineText());

        // Close
        baseIndentLvl--;
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}}};{Options.GetNewLineText()}");

        return sb.ToString();
    }

    public string GetFunctionString(CppFunction func, CppStruct parent, bool signature, int baseIndentLvl, List<string> modelConditions)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        bool forceAddBody = signature;
        forceAddBody &= (parent is not null && parent.TemplateParams.Count > 0)
                        || func.TemplateParams.Count > 0
                        || func.Friend;

        var sb = new StringBuilder();

        // Comment
        if (!signature)
            sb.Append(GetMultiCommentString(func.Comments, baseIndentLvl, false));

        // Template
        if (func.TemplateParams.Count > 0)
        {
            sb.Append(Helper.GetIndent(baseIndentLvl));
            sb.Append($"template<{string.Join(", ", func.TemplateParams)}>");
            sb.Append(Options.GetNewLineText());
        }

        sb.Append(Helper.GetIndent(baseIndentLvl));

        // Static
        if (signature && func.Static)
            sb.Append("static ");

        // Friend
        if (signature && func.Friend)
            sb.Append("friend ");

        // Inline
        if (func.Inline && signature)
            sb.Append("inline ");

        // Type
        if (!string.IsNullOrWhiteSpace(func.Type))
            sb.Append($"{func.Type} ");

        // Name
        if (parent is not null && parent.TemplateParams.Count == 0 && !signature && func.TemplateParams.Count == 0 && !func.Friend)
            sb.Append($"{parent.Name}::");

        sb.Append(func.Name);

        // Params
        sb.Append('(');
        sb.Append(string.Join(", ", func.Params.Where(p => ResolveConditions(modelConditions, p.Conditions)).Select(GetParamString)));
        sb.Append(')');

        // Const
        if (func.Const)
            sb.Append(" const");

        if (signature && !forceAddBody)
        {
            sb.Append(';');
            return sb.ToString();
        }

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

    public string GetStructString(CppStruct @struct, int baseIndentLvl, List<string> conditions)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        var sb = new StringBuilder();

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
        sb.Append(@struct.IsClass ? "class " : "struct ");

        // Name
        sb.Append(@struct.Name);

        // Supers
        if (@struct.Supers.Count > 0)
            sb.Append($" : {Helper.JoinString(", ", @struct.Supers, "public ")}");

        // Inline comment
        if (!string.IsNullOrEmpty(@struct.InlineComment))
            sb.Append($" // {@struct.InlineComment}");

        sb.Append(Options.GetNewLineText());

        // Open struct scope
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}{{{Options.GetNewLineText()}");
        baseIndentLvl++;

        // Variables
        if (@struct.Variables.Count > 0)
        {
            bool lastVarIsPrivate = false;
            bool lastVarIsUnion = false;
            List<CppVariable> variables = @struct.Variables.Where(v => !string.IsNullOrWhiteSpace(v.Name) && ResolveConditions(conditions, v.Conditions)).ToList();

            // Force write "private" or "public"
            if (variables.Count > 0)
                lastVarIsPrivate = !variables.First().Private;

            foreach (CppVariable structVar in variables)
            {
                // Private or Public
                if (structVar.Private != lastVarIsPrivate)
                {
                    lastVarIsPrivate = structVar.Private;
                    sb.Append($"{Helper.GetIndent(baseIndentLvl - 1)}");
                    sb.Append(structVar.Private ? "private:" : "public:");
                    sb.Append(Options.GetNewLineText());
                }

                // Open union
                if (structVar.Union && !lastVarIsUnion)
                {
                    lastVarIsUnion = true;
                    sb.Append($"{Helper.GetIndent(baseIndentLvl)}union{Options.GetNewLineText()}");
                    sb.Append($"{Helper.GetIndent(baseIndentLvl)}{{{Options.GetNewLineText()}");
                    baseIndentLvl++;
                }

                // Close union
                else if (!structVar.Union && lastVarIsUnion)
                {
                    lastVarIsUnion = false;
                    baseIndentLvl--;
                    sb.Append($"{Helper.GetIndent(baseIndentLvl)}}};{Options.GetNewLineText()}");
                }

                // Print variable
                sb.Append(GetVariableString(structVar, baseIndentLvl));
                sb.Append(Options.GetNewLineText());
            }

            // Close union
            if (lastVarIsUnion)
            {
                baseIndentLvl--;
                sb.Append($"{Helper.GetIndent(baseIndentLvl)}}};{Options.GetNewLineText()}");
            }

            sb.Append(Options.GetNewLineText());
        }

        // Friends
        if (@struct.Friends.Count > 0)
        {
            sb.Append(Helper.JoinString(Options.GetNewLineText(), @struct.Friends, $"{Helper.GetIndent(baseIndentLvl)}friend ", ";"));
            sb.Append(Options.GetNewLineText());
        }

        // Methods
        if (@struct.Methods.Count > 0)
        {
            bool lastMethodIsPrivate = false;
            List<CppFunction> methods = @struct.Methods.Where(m => !string.IsNullOrWhiteSpace(m.Name) && ResolveConditions(conditions, m.Conditions)).ToList();

            // Force write "private" or "public"
            if (methods.Count > 0)
                lastMethodIsPrivate = !methods.First().Private;

            foreach (CppFunction structMethod in methods)
            {
                // Private or Public
                if (structMethod.Private != lastMethodIsPrivate)
                {
                    lastMethodIsPrivate = structMethod.Private;
                    sb.Append($"{Helper.GetIndent(baseIndentLvl - 1)}");
                    sb.Append(structMethod.Private ? "private:" : "public:");
                    sb.Append(Options.GetNewLineText());
                }

                // Print method
                sb.Append(GetFunctionString(structMethod, @struct, true, baseIndentLvl, conditions));
                sb.Append(Options.GetNewLineText());
            }
        }

        // Close struct scope
        baseIndentLvl--;
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}}};{Options.GetNewLineText()}");

        return sb.ToString();
    }

    public string GeneratePragmas(IEnumerable<string> pragmas, int baseIndentLvl)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        string ret = Helper.JoinString(Options.GetNewLineText(), pragmas, $"{Helper.GetIndent(baseIndentLvl)}#pragma ");
        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateIncludes(IEnumerable<string> includes, int baseIndentLvl)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        string ret = Helper.JoinString(Options.GetNewLineText(), includes, $"{Helper.GetIndent(baseIndentLvl)}#include ");
        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateDefines(IEnumerable<CppDefine> defines, int baseIndentLvl)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        string ret = Helper.JoinString(
            Options.GetNewLineText(),
            defines.Select(d => d.Name + (string.IsNullOrWhiteSpace(d.Value) ? string.Empty : $" {d.Value}")),
            $"{Helper.GetIndent(baseIndentLvl)}#define ");

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateForwards(IEnumerable<string> forwards, int baseIndentLvl)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        string ret = Helper.JoinString(Options.GetNewLineText(), forwards, Helper.GetIndent(baseIndentLvl), ";");
        ret += Options.GetNewLineText();

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateVariables(IEnumerable<CppVariable> variables, int baseIndentLvl, List<string> conditions)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        List<CppVariable> vars = variables
            .Where(v => !string.IsNullOrWhiteSpace(v.Name) && !string.IsNullOrWhiteSpace(v.Type) && ResolveConditions(conditions, v.Conditions))
            .ToList();

        if (vars.Count == 0)
            return string.Empty;

        string ret = Helper.JoinString(Options.GetNewLineText(), vars.Select(v => GetVariableString(v, baseIndentLvl)), Helper.GetIndent(baseIndentLvl));

        if (Options.PrintSectionName)
            ret = GetSectionHeading("Variables", baseIndentLvl) + ret;

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateFunctions(IEnumerable<CppFunction> functions, CppStruct parent, bool signatureOnly, int baseIndentLvl, List<string> conditions)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        List<CppFunction> funcs = functions
            .Where(f => !string.IsNullOrWhiteSpace(f.Name) && !string.IsNullOrWhiteSpace(f.Type) && ResolveConditions(conditions, f.Conditions))
            .ToList();

        if (funcs.Count == 0)
            return string.Empty;

        string ret = string.Join(Options.GetNewLineText(), funcs.Select(f => GetFunctionString(f, parent, signatureOnly, baseIndentLvl, conditions)));

        if (Options.PrintSectionName)
            ret = GetSectionHeading("Global Functions", baseIndentLvl) + ret;

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateEnums(IEnumerable<CppEnum> enums, int baseIndentLvl, List<string> conditions)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        List<CppEnum> vEnums = enums
            .Where(e => !string.IsNullOrWhiteSpace(e.Name) && ResolveConditions(conditions, e.Conditions))
            .ToList();

        if (vEnums.Count == 0)
            return string.Empty;

        string ret = string.Join(Options.GetNewLineText(), vEnums.Select(e => GetEnumString(e, baseIndentLvl)));

        if (Options.PrintSectionName)
            ret = GetSectionHeading("Enums", baseIndentLvl) + ret;

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateConstants(IEnumerable<CppConstant> constants, int baseIndentLvl, List<string> conditions)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        List<string> values = constants
            .Where(c => !string.IsNullOrWhiteSpace(c.Name) && !string.IsNullOrWhiteSpace(c.Type) && !string.IsNullOrWhiteSpace(c.Value) && ResolveConditions(conditions, c.Conditions))
            .Select(c => $"static constexpr {c.Type} {c.Name} = {c.Value}")
            .ToList();

        if (values.Count == 0)
            return string.Empty;

        string ret = Helper.JoinString(
            Options.GetNewLineText(),
            values,
            Helper.GetIndent(baseIndentLvl),
            ";");

        if (Options.PrintSectionName && values.Count > 0)
            ret = GetSectionHeading("Constants", baseIndentLvl) + ret;

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateStructs(IEnumerable<CppStruct> structs, int baseIndentLvl, List<string> conditions)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        List<CppStruct> vStruct = structs
            .Where(s => !string.IsNullOrWhiteSpace(s.Name) && ResolveConditions(conditions, s.Conditions))
            .ToList();

        if (vStruct.Count == 0)
            return string.Empty;

        string ret = string.Join(Options.GetNewLineText(), vStruct.Select(s => GetStructString(s, baseIndentLvl, conditions)));

        if (Options.PrintSectionName)
            ret = GetSectionHeading(vStruct.All(s => s.IsClass) ? "Classes" : "Structs", baseIndentLvl) + ret;

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public void Init(CppLangOptions options = null)
    {
        Options = options ?? new CppLangOptions();
    }

    public CppPackage ModelFromJson(string jsonData)
    {
        return JsonConvert.DeserializeObject<CppPackage>(jsonData);
    }

    public Dictionary<string, string> GenerateFiles(CppPackage cppPackage)
    {
        if (Options is null)
            throw new Exception($"Call '{nameof(Init)}' function first");

        cppPackage.Conditions = cppPackage.Conditions.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

        var ret = new Dictionary<string, string>();

        if (!Options.GeneratePackageSyntax)
        {
            ret.Add($"{cppPackage.Name}.h", MakeHeaderFile(cppPackage));
            ret.Add($"{cppPackage.Name}.cpp", MakeCppFile(cppPackage));

            return ret;
        }

        ret.Add($"{cppPackage.Name}_Structs.h", MakeStructsFile(cppPackage));
        ret.Add($"{cppPackage.Name}_Classes.h", MakeClassesFile(cppPackage));
        ret.Add($"{cppPackage.Name}_Package.h", MakePackageHeaderFile(cppPackage));
        ret.Add($"{cppPackage.Name}_Package.cpp", MakePackageCppFile(cppPackage));

        return ret;
    }
}
