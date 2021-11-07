using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LangPrint.Utils;
using Newtonsoft.Json;

namespace LangPrint.Cpp
{
    public class CppProcessor : ILangProcessor<CppModel, CppLangOptions>
    {
        public CppLangOptions Options { get; private set; }
        public CppModel Model { get; private set; }

        private string GenerateHeaderFile()
        {
            var sb = new StringBuilder();

            // File header
            sb.Append(GetFileHeader(Model.Pragmas, Model.Includes, Model.Defines, Model.BeforeNameSpace, out int indentLvl));

            // Forwards
            sb.Append(ProcessForwards(Model.Forwards, indentLvl));

            // Constants
            sb.Append(ProcessConstants(Model.Constants, indentLvl));

            // Variables
            sb.Append(ProcessVariables(Model.Variables, indentLvl));

            // Global functions
            sb.Append(ProcessFunctions(Model.Functions, null, true, indentLvl));

            // Enums
            sb.Append(ProcessEnums(Model.Enums, indentLvl));

            // Structs
            sb.Append(ProcessStructs(Model.Structs, indentLvl));

            // File footer
            sb.Append(GetFileFooter(Model.AfterNameSpace, ref indentLvl));

            return sb.ToString();
        }

        private string GenerateCppFile()
        {
            var sb = new StringBuilder();

            // File header
            List<string> includes = Model.CppIncludes.Append($"\"{Model.Name}.h\"").ToList();
            sb.Append(GetFileHeader(null, includes, null, Model.CppBeforeNameSpace, out int indentLvl));

            // Global functions
            sb.Append(ProcessFunctions(Model.Functions, null, false, indentLvl));

            // Static variables
            if (Model.Structs.Any(s => s.Variables.Any(v => v.Static)))
            {
                var methodsStr = new List<string>();
                sb.Append(GetSectionHeading("Structs Static Variables", indentLvl));

                foreach (CppStruct @struct in Model.Structs)
                {
                    IEnumerable<string> variables = @struct.Variables
                        .Where(v => v.Static && !v.Const && !v.Constexpr)
                        .Select(v => GetVariableString(v, @struct, true));

                    methodsStr.AddRange(variables);
                }

                sb.Append(Helper.JoinString(Options.GetNewLineText(), methodsStr, Helper.GetIndent(indentLvl)));
                sb.Append(Options.GetNewLineText() + Options.GetNewLineText());
            }

            // Structs functions
            if (Model.Structs.Any(s => s.Methods.Count > 0))
            {
                var methodsStr = new List<string>();
                sb.Append(GetSectionHeading("Structs Functions", indentLvl));

                foreach (CppStruct @struct in Model.Structs)
                {
                    methodsStr.AddRange(@struct.Methods.Select(structMethod => GetFunctionString(structMethod, @struct, false, indentLvl)));
                }

                sb.Append(string.Join(Options.GetNewLineText(), methodsStr));
                sb.Append(Options.GetNewLineText());
            }

            // File footer
            sb.Append(GetFileFooter(Model.CppAfterNameSpace, ref indentLvl));

            return sb.ToString();
        }

        private string GenerateStructsFile()
        {
            var sb = new StringBuilder();

            var pragmas = new List<string>()
            {
                "once"
            };

            // File header
            sb.Append(GetFileHeader(pragmas, null, null, Model.BeforeNameSpace, out int indentLvl));

            // Enums
            sb.Append(ProcessEnums(Model.Enums, indentLvl));

            // Structs
            sb.Append(ProcessStructs(Model.Structs.Where(s => !s.IsClass), indentLvl));

            // File footer
            sb.Append(GetFileFooter(Model.AfterNameSpace, ref indentLvl));

            return sb.ToString();
        }

        private string GenerateClassesFile()
        {
            var sb = new StringBuilder();

            var pragmas = new List<string>()
            {
                "once"
            };

            // File header
            sb.Append(GetFileHeader(pragmas, null, null, Model.BeforeNameSpace, out int indentLvl));

            // Structs
            sb.Append(ProcessStructs(Model.Structs.Where(s => s.IsClass), indentLvl));

            // File footer
            sb.Append(GetFileFooter(Model.AfterNameSpace, ref indentLvl));

            return sb.ToString();
        }

        private string GeneratePackageFile()
        {
            var sb = new StringBuilder();

            // File header
            sb.Append(GetFileHeader(Model.Pragmas, Model.Includes, Model.Defines, Model.BeforeNameSpace, out int indentLvl));

            // Forwards
            sb.Append(ProcessForwards(Model.Forwards, indentLvl));

            // Constants
            sb.Append(ProcessConstants(Model.Constants, indentLvl));

            // Variables
            sb.Append(ProcessVariables(Model.Variables, indentLvl));

            // Global functions
            sb.Append(ProcessFunctions(Model.Functions, null, true, indentLvl));

            // Package include
            var packHeaders = new List<string>()
            {
                $"{Model.Name}_Structs.h",
                $"{Model.Name}_Classes.h"
            };
            sb.Append(GetSectionHeading("Structs & Classes", indentLvl));
            sb.Append(Helper.JoinString(Options.GetNewLineText(), packHeaders, $"{Helper.GetIndent(indentLvl)}#include \"", "\""));
            sb.Append(Options.GetNewLineText());

            // File footer
            sb.Append(GetFileFooter(Model.AfterNameSpace, ref indentLvl));

            return sb.ToString();
        }

        private string GenerateFunctionsFile()
        {
            var sb = new StringBuilder();

            var includes = new List<string>(Model.CppIncludes)
            {
                $"\"{Model.Name}_Package.h\""
            };

            // File header
            sb.Append(GetFileHeader(null, includes, null, Model.CppBeforeNameSpace, out int indentLvl));

            // Global functions
            sb.Append(ProcessFunctions(Model.Functions, null, false, indentLvl));

            // Static variables
            if (Model.Structs.Any(s => s.Variables.Any(v => v.Static)))
            {
                var methodsStr = new List<string>();
                sb.Append(GetSectionHeading("Structs Static Variables", indentLvl));

                foreach (CppStruct @struct in Model.Structs)
                {
                    IEnumerable<string> variables = @struct.Variables
                        .Where(v => v.Static && !v.Const && !v.Constexpr)
                        .Select(v => GetVariableString(v, @struct, true));

                    methodsStr.AddRange(variables);
                }

                sb.Append(Helper.JoinString(Options.GetNewLineText(), methodsStr, Helper.GetIndent(indentLvl)));
                sb.Append(Options.GetNewLineText() + Options.GetNewLineText());
            }

            // Structs functions
            if (Model.Structs.Any(s => s.Methods.Count > 0))
            {
                var methodsStr = new List<string>();
                sb.Append(GetSectionHeading("Structs Functions", indentLvl));

                foreach (CppStruct @struct in Model.Structs)
                {
                    methodsStr.AddRange(@struct.Methods.Select(structMethod => GetFunctionString(structMethod, @struct, false, indentLvl)));
                }

                sb.Append(string.Join(Options.GetNewLineText(), methodsStr));
                sb.Append(Options.GetNewLineText());
            }

            // File footer
            sb.Append(GetFileFooter(Model.CppAfterNameSpace, ref indentLvl));

            return sb.ToString();
        }

        public bool ResolveConditions(List<string> conditions)
        {
            if (conditions is null || conditions.Count == 0)
                return true;

            // ! conditions
            foreach (string condition in conditions.Where(c => !string.IsNullOrWhiteSpace(c) && c.StartsWith("!")))
            {
                if (Model.Conditions.Any(gCondition => condition[1..] == gCondition))
                    return false;
            }

            // All conditions must to be fitted
            return conditions
                .Where(c => !string.IsNullOrWhiteSpace(c) && !c.StartsWith("!"))
                .All(c => Model.Conditions.Contains(c));
        }

        public string GetFileHeader(List<string> pragmas, List<string> includes, List<CppDefine> defines, string beforeNameSpace, out int indentLvl)
        {
            if (Options is null)
                throw new Exception($"Call '{nameof(Init)}' function first");

            indentLvl = 0;
            var sb = new StringBuilder();

            // Pragmas
            if (pragmas?.Count > 0)
                sb.Append(ProcessPragmas(pragmas, indentLvl));

            // HeadingComment
            sb.Append(GetMultiCommentString(Model.HeadingComment, indentLvl));

            // Includes
            if (includes?.Count > 0)
                sb.Append(ProcessIncludes(includes, indentLvl));

            // Defines
            if (defines?.Count > 0)
                sb.Append(ProcessDefines(defines, indentLvl));

            // BeforeNameSpace
            if (!string.IsNullOrWhiteSpace(beforeNameSpace))
            {
                sb.Append(beforeNameSpace + Options.GetNewLineText());
                sb.Append(Options.GetNewLineText());
            }

            // NameSpace
            if (!string.IsNullOrWhiteSpace(Model.NameSpace))
            {
                sb.Append(Helper.GetIndent(indentLvl));
                sb.Append($"namespace {Model.NameSpace}{Options.GetNewLineText()}{{{Options.GetNewLineText()}");
                indentLvl++;
            }

            return sb.ToString();
        }

        public string GetFileFooter(string afterNameSpace, ref int indentLvl)
        {
            if (Options is null)
                throw new Exception($"Call '{nameof(Init)}' function first");

            var sb = new StringBuilder();

            // Close NameSpace
            if (!string.IsNullOrWhiteSpace(Model.NameSpace))
            {
                indentLvl--;
                sb.Append($"{Helper.GetIndent(indentLvl)}}}{Options.GetNewLineText()}");
                sb.Append(Options.GetNewLineText());
            }

            // AfterNameSpace
            if (!string.IsNullOrWhiteSpace(afterNameSpace))
            {
                sb.AppendLine(afterNameSpace);
                sb.Append(Options.GetNewLineText());
            }

            return sb.ToString();
        }

        public string GetParamString(CppParameter parameter)
        {
            if (Options is null)
                throw new Exception($"Call '{nameof(Init)}' function first");

            return $"{parameter.Type} {parameter.Name}";
        }

        public string GetVariableString(CppVariable variable, CppStruct parent = null, bool definition = false)
        {
            if (Options is null)
                throw new Exception($"Call '{nameof(Init)}' function first");

            var sb = new StringBuilder();

            // Extern
            if (!definition && variable.Extern)
                sb.Append("extern ");

            // Static
            if (!definition && variable.Static)
                sb.Append("static ");

            // Const
            if (variable.Const)
                sb.Append("const ");

            // Constexpr
            if (!definition && variable.Constexpr)
                sb.Append("constexpr ");

            // Type
            sb.Append($"{variable.Type.PadLeft(Options.VariableMemberTypePadSize)} ");

            // Parent name
            if (definition && parent is not null)
                sb.Append($"{parent.Name}::");

            // Name
            sb.Append(variable.Name);

            // ArrayDim
            if (!string.IsNullOrWhiteSpace(variable.ArrayDim))
                sb.Append($"[{variable.ArrayDim}]");

            // Bitfield
            else if (!string.IsNullOrWhiteSpace(variable.Bitfield))
                sb.Append($" : {variable.Bitfield}");

            // Value
            if (!string.IsNullOrWhiteSpace(variable.Value) && definition)
                sb.Append($" = {variable.Value}");

            sb.Append(';');

            // Inline comment
            if (string.IsNullOrEmpty(variable.InlineComment))
                sb.Append($" {"//".PadLeft(Options.InlineCommentPadSize)} {variable.InlineComment}");

            return sb.ToString();
        }

        public string GetSectionHeading(string name, int indentLvl)
        {
            if (Options is null)
                throw new Exception($"Call '{nameof(Init)}' function first");

            string headLine = string.Concat(Enumerable.Repeat("=", 50));

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

        public string GetEnumString(CppEnum @enum, int baseIndentLvl)
        {
            if (Options is null)
                throw new Exception($"Call '{nameof(Init)}' function first");

            var sb = new StringBuilder();

            // Comment
            sb.Append(GetMultiCommentString(@enum.Comment, baseIndentLvl, false));

            // Name
            sb.Append(@enum.IsClass
                ? $"{Helper.GetIndent(baseIndentLvl)}enum class {@enum.Name}"
                : $"{Helper.GetIndent(baseIndentLvl)}enum {@enum.Name}");

            // Type
            if (!string.IsNullOrWhiteSpace(@enum.Type))
                sb.Append($" : {@enum.Type}");

            // Inline comment
            if (string.IsNullOrEmpty(@enum.InlineComment))
                sb.Append($" {"//".PadLeft(Options.InlineCommentPadSize)} {@enum.InlineComment}");

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

        public string GetFunctionString(CppFunction func, CppStruct parent, bool signature, int baseIndentLvl)
        {
            if (Options is null)
                throw new Exception($"Call '{nameof(Init)}' function first");

            var sb = new StringBuilder();

            // Comment
            sb.Append(GetMultiCommentString(func.Comment, baseIndentLvl, false));

            sb.Append(Helper.GetIndent(baseIndentLvl));

            // Static
            if (func.Static)
                sb.Append("static ");

            // Inline
            if (func.Inline)
                sb.Append("inline ");

            // Type
            if (!string.IsNullOrWhiteSpace(func.Type))
                sb.Append($"{func.Type} ");

            // Name
            if (parent is not null && !signature)
                sb.Append($"{parent.Name}::");

            sb.Append(func.Name);

            // Params
            sb.Append('(');
            sb.Append(string.Join(", ", func.Params.Where(p => ResolveConditions(p.Condition)).Select(GetParamString)));
            sb.Append(')');

            // Const
            if (func.Const)
                sb.Append(" const");

            if (signature)
            {
                sb.Append(';');
                return sb.ToString();
            }

            // Inline comment
            if (string.IsNullOrEmpty(func.InlineComment))
                sb.Append($" {"//".PadLeft(Options.InlineCommentPadSize)} {func.InlineComment}");

            sb.Append(Options.GetNewLineText());

            // Body
            sb.Append($"{Helper.GetIndent(baseIndentLvl)}{{{Options.GetNewLineText()}");
            baseIndentLvl++;

            sb.Append(Helper.JoinString(Options.GetNewLineText(), func.Body, Helper.GetIndent(baseIndentLvl)));

            baseIndentLvl--;
            sb.Append($"{Options.GetNewLineText()}{Helper.GetIndent(baseIndentLvl)}}}{Options.GetNewLineText()}");

            return sb.ToString();
        }

        public string GetStructString(CppStruct @struct, int baseIndentLvl)
        {
            if (Options is null)
                throw new Exception($"Call '{nameof(Init)}' function first");

            var sb = new StringBuilder();

            // Comment
            sb.Append(GetMultiCommentString(@struct.Comment, baseIndentLvl, false));

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
            if (string.IsNullOrEmpty(@struct.InlineComment))
                sb.Append($" {"//".PadLeft(Options.InlineCommentPadSize)} {@struct.InlineComment}");

            sb.Append(Options.GetNewLineText());

            // Open struct scope
            sb.Append($"{Helper.GetIndent(baseIndentLvl)}{{{Options.GetNewLineText()}");
            baseIndentLvl++;

            // Variables
            if (@struct.Variables.Count > 0)
            {
                bool lastVarIsPrivate = false;
                bool lastVarIsUnion = false;
                List<CppVariable> variables = @struct.Variables.Where(v => !string.IsNullOrWhiteSpace(v.Name) && ResolveConditions(v.Condition)).ToList();

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
                        sb.Append($"{Helper.GetIndent(baseIndentLvl)}}}{Options.GetNewLineText()}");
                    }

                    // Print variable
                    sb.Append(Helper.GetIndent(baseIndentLvl));
                    sb.Append(GetVariableString(structVar));
                    sb.Append(Options.GetNewLineText());
                }

                // Close union
                if (lastVarIsUnion)
                {
                    baseIndentLvl--;
                    sb.Append($"{Helper.GetIndent(baseIndentLvl)}}}{Options.GetNewLineText()}");
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
                List<CppFunction> methods = @struct.Methods.Where(m => !string.IsNullOrWhiteSpace(m.Name) && ResolveConditions(m.Condition)).ToList();

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
                    sb.Append(GetFunctionString(structMethod, @struct, true, baseIndentLvl));
                    sb.Append(Options.GetNewLineText());
                }
            }

            // Close struct scope
            baseIndentLvl--;
            sb.Append($"{Helper.GetIndent(baseIndentLvl)}}};{Options.GetNewLineText()}");

            return sb.ToString();
        }

        public string ProcessPragmas(IEnumerable<string> pragmas, int baseIndentLvl)
        {
            if (Options is null)
                throw new Exception($"Call '{nameof(Init)}' function first");

            string ret = Helper.JoinString(Options.GetNewLineText(), pragmas, $"{Helper.GetIndent(baseIndentLvl)}#pragma ");
            return Helper.FinalizeSection(ret, Options.GetNewLineText());
        }

        public string ProcessIncludes(IEnumerable<string> includes, int baseIndentLvl)
        {
            if (Options is null)
                throw new Exception($"Call '{nameof(Init)}' function first");

            string ret = Helper.JoinString(Options.GetNewLineText(), includes, $"{Helper.GetIndent(baseIndentLvl)}#include ");
            return Helper.FinalizeSection(ret, Options.GetNewLineText());
        }

        public string ProcessDefines(IEnumerable<CppDefine> defines, int baseIndentLvl)
        {
            if (Options is null)
                throw new Exception($"Call '{nameof(Init)}' function first");

            string ret = Helper.JoinString(
                Options.GetNewLineText(),
                defines.Select(d => $"{d.Name} {d.Value}"),
                $"{Helper.GetIndent(baseIndentLvl)}#define ");

            return Helper.FinalizeSection(ret, Options.GetNewLineText());
        }

        public string ProcessForwards(IEnumerable<string> forwards, int baseIndentLvl)
        {
            if (Options is null)
                throw new Exception($"Call '{nameof(Init)}' function first");

            string ret = Helper.JoinString(Options.GetNewLineText(), forwards, Helper.GetIndent(baseIndentLvl), ";");
            ret += Options.GetNewLineText();

            return Helper.FinalizeSection(ret, Options.GetNewLineText());
        }

        public string ProcessVariables(IEnumerable<CppVariable> variables, int baseIndentLvl)
        {
            if (Options is null)
                throw new Exception($"Call '{nameof(Init)}' function first");

            List<CppVariable> vars = variables
                .Where(v => !string.IsNullOrWhiteSpace(v.Name) && !string.IsNullOrWhiteSpace(v.Type) && ResolveConditions(v.Condition))
                .ToList();

            if (vars.Count == 0)
                return string.Empty;

            string ret = Helper.JoinString(Options.GetNewLineText(), vars.Select(v => GetVariableString(v)), Helper.GetIndent(baseIndentLvl));

            if (Options.PrintSectionName)
                ret = GetSectionHeading("Variables", baseIndentLvl) + ret;

            return Helper.FinalizeSection(ret, Options.GetNewLineText());
        }

        public string ProcessFunctions(IEnumerable<CppFunction> functions, CppStruct parent, bool signatureOnly, int baseIndentLvl)
        {
            if (Options is null)
                throw new Exception($"Call '{nameof(Init)}' function first");

            List<CppFunction> funcs = functions
                .Where(f => !string.IsNullOrWhiteSpace(f.Name) && !string.IsNullOrWhiteSpace(f.Type) && ResolveConditions(f.Condition))
                .ToList();

            if (funcs.Count == 0)
                return string.Empty;

            string ret = string.Join(Options.GetNewLineText(), funcs.Select(f => GetFunctionString(f, parent, signatureOnly, baseIndentLvl)));

            if (Options.PrintSectionName)
                ret = GetSectionHeading("Global Functions", baseIndentLvl) + ret;

            return Helper.FinalizeSection(ret, Options.GetNewLineText());
        }

        public string ProcessEnums(IEnumerable<CppEnum> enums, int baseIndentLvl)
        {
            if (Options is null)
                throw new Exception($"Call '{nameof(Init)}' function first");

            List<CppEnum> vEnums = enums
                .Where(e => !string.IsNullOrWhiteSpace(e.Name) && ResolveConditions(e.Condition))
                .ToList();

            if (vEnums.Count == 0)
                return string.Empty;

            string ret = string.Join(Options.GetNewLineText(), vEnums.Select(e => GetEnumString(e, baseIndentLvl)));

            if (Options.PrintSectionName)
                ret = GetSectionHeading("Enums", baseIndentLvl) + ret;

            return Helper.FinalizeSection(ret, Options.GetNewLineText());
        }

        public string ProcessConstants(IEnumerable<CppConstant> constants, int baseIndentLvl)
        {
            if (Options is null)
                throw new Exception($"Call '{nameof(Init)}' function first");

            List<string> values = constants
                .Where(c => !string.IsNullOrWhiteSpace(c.Name) && !string.IsNullOrWhiteSpace(c.Type) && !string.IsNullOrWhiteSpace(c.Value) && ResolveConditions(c.Condition))
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

        public string ProcessStructs(IEnumerable<CppStruct> structs, int baseIndentLvl)
        {
            if (Options is null)
                throw new Exception($"Call '{nameof(Init)}' function first");

            List<CppStruct> vStruct = structs
                .Where(s => !string.IsNullOrWhiteSpace(s.Name) && ResolveConditions(s.Condition))
                .ToList();

            if (vStruct.Count == 0)
                return string.Empty;

            string ret = string.Join(Options.GetNewLineText(), vStruct.Select(s => GetStructString(s, baseIndentLvl)));

            if (Options.PrintSectionName)
                ret = GetSectionHeading("Structs", baseIndentLvl) + ret;

            return Helper.FinalizeSection(ret, Options.GetNewLineText());
        }

        public void Init(CppLangOptions options = null)
        {
            Options = options ?? new CppLangOptions();
        }

        public Dictionary<string, string> GenerateFiles(CppModel cppModel)
        {
            if (Options is null)
                throw new Exception($"Call '{nameof(Init)}' function first");

            Model = cppModel;
            Model.Conditions = Model.Conditions.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

            var ret = new Dictionary<string, string>();

            if (!Options.GeneratePackageSyntax)
            {
                ret.Add($"{Model.Name}.h", GenerateHeaderFile());
                ret.Add($"{Model.Name}.cpp", GenerateCppFile());

                return ret;
            }

            ret.Add($"{Model.Name}_Structs.h", GenerateStructsFile());
            ret.Add($"{Model.Name}_Classes.h", GenerateClassesFile());
            ret.Add($"{Model.Name}_Package.h", GeneratePackageFile());
            ret.Add($"{Model.Name}_Package.cpp", GenerateFunctionsFile());

            return ret;
        }

        public Dictionary<string, string> GenerateFiles(string jsonData)
        {
            return GenerateFiles(JsonConvert.DeserializeObject<CppModel>(jsonData));
        }
    }
}
