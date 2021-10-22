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
        public LangOptions Options { get; private set; }
        public CppModel Model { get; private set; }

        private static string GetIndent(int lvl)
        {
            return lvl == 0
                ? string.Empty
                : string.Concat(Enumerable.Repeat("\t", lvl));
            //: string.Concat(Enumerable.Repeat(new string(' ', Options.IndentSize), lvl));
        }

        private bool ProcessConditions(List<string> conditions)
        {
            if (conditions is null || conditions.Count == 0)
                return true;

            // ! conditions
            foreach (string condition in conditions.Where(c => !string.IsNullOrWhiteSpace(c)))
            {
                if (Model.Conditions.Any(gCondition => condition.StartsWith("!") && condition[1..] == gCondition))
                    return false;
            }

            // All conditions must to be fitted
            return conditions
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .All(c => Model.Conditions.Contains(c));
        }

        private string GetSectionHeading(string name, int indentLvl)
        {
            string headLine = string.Concat(Enumerable.Repeat("=", 50));

            return GetIndent(indentLvl) + "// " + headLine + Options.GetNewLineText() +
                   GetIndent(indentLvl) + "// " + "# " + name + Options.GetNewLineText() +
                   GetIndent(indentLvl) + "// " + headLine + Options.GetNewLineText();
        }

        private string GetEnumString(CppEnum @enum, int baseIndentLvl)
        {
            var sb = new StringBuilder();

            // Name
            sb.Append(@enum.IsClass
                ? $"{GetIndent(baseIndentLvl)}enum class {@enum.Name}"
                : $"{GetIndent(baseIndentLvl)}enum {@enum.Name}");

            // Type
            if (!string.IsNullOrWhiteSpace(@enum.Type))
                sb.Append($" : {@enum.Type}");

            sb.Append(Options.GetNewLineText());

            // Open
            sb.Append($"{GetIndent(baseIndentLvl)}{{{Options.GetNewLineText()}");
            baseIndentLvl++;

            // Values
            int biggestName = @enum.Values.Max(ev => ev.Name.Length);
            string values = Helper.JoinString(
                "," + Options.GetNewLineText(),
                @enum.Values.Select(ev => $"{ev.Name.PadRight(biggestName)} = {ev.Value}"),
                GetIndent(baseIndentLvl));

            sb.Append(values);
            sb.Append(Options.GetNewLineText());

            // Close
            baseIndentLvl--;
            sb.Append($"{GetIndent(baseIndentLvl)}}}{Options.GetNewLineText()}");

            return sb.ToString();
        }

        private static string GetParamString(CppParam param)
        {
            return $"{param.Type} {param.Name}";
        }

        private string GetFunctionsString(CppFunction func, CppStruct parent, int baseIndentLvl, bool signature)
        {
            var sb = new StringBuilder();

            sb.Append(GetIndent(baseIndentLvl));

            // Static
            if (signature)
                sb.Append("static ");

            // Inline
            if (func.Inline)
                sb.Append("inline ");

            // Type
            sb.Append($"{func.Type} ");

            // Name
            if (parent is not null)
                sb.Append($"{parent.Name}::");

            sb.Append(func.Name);

            // Params
            sb.Append('(');
            sb.Append(string.Join(", ", func.Params.Select(GetParamString)));
            sb.Append(')');

            // Const
            if (func.Const)
                sb.Append("const");

            if (signature)
            {
                sb.Append(';');
                return sb.ToString();
            }

            // Body
            sb.Append($"{Options.GetNewLineText()}{GetIndent(baseIndentLvl)}{{{Options.GetNewLineText()}");
            baseIndentLvl++;

            sb.Append(Helper.JoinString(Options.GetNewLineText(), func.Body, GetIndent(baseIndentLvl)));

            baseIndentLvl--;
            sb.Append($"{Options.GetNewLineText()}{GetIndent(baseIndentLvl)}}}{Options.GetNewLineText()}");

            return sb.ToString();
        }

        private string FinalizeSection(string sectionStr)
        {
            if (string.IsNullOrWhiteSpace(sectionStr))
                return sectionStr;

            if (sectionStr.EndsWith(Options.GetNewLineText() + Options.GetNewLineText()))
                return sectionStr;

            if (sectionStr.EndsWith(Options.GetNewLineText()))
                return sectionStr + Options.GetNewLineText();

            return sectionStr + Options.GetNewLineText() + Options.GetNewLineText();
        }

        private string ProcessPragmas(List<string> pragmas, int baseIndentLvl)
        {
            string ret = Helper.JoinString(Options.GetNewLineText(), pragmas, $"{GetIndent(baseIndentLvl)}#pragma ");
            return FinalizeSection(ret);
        }

        private string ProcessMultiComment(List<string> comments, int baseIndentLvl)
        {
            string ret = $"{GetIndent(baseIndentLvl)}/*{Options.GetNewLineText()}" +
                   Helper.JoinString(Options.GetNewLineText(), comments, $"{GetIndent(baseIndentLvl)} * ") +
                   $"{GetIndent(baseIndentLvl)}{Options.GetNewLineText()} */";

            return FinalizeSection(ret);
        }

        private string ProcessDefines(List<CppDefine> defines, int baseIndentLvl)
        {
            string ret = Helper.JoinString(
                Options.GetNewLineText(),
                defines.Select(d => $"{d.Name} {d.Value}"),
                $"{GetIndent(baseIndentLvl)}#define ");

            return FinalizeSection(ret);
        }

        private string ProcessForwards(List<string> forwards, int baseIndentLvl)
        {
            string ret = Helper.JoinString(Options.GetNewLineText(), forwards, GetIndent(baseIndentLvl), ";");
            ret += Options.GetNewLineText();

            return FinalizeSection(ret);
        }

        private string ProcessEnums(List<CppEnum> enums, int baseIndentLvl)
        {
            List<CppEnum> vEnums = enums
                .Where(e => !string.IsNullOrWhiteSpace(e.Name) && ProcessConditions(e.Condition))
                .ToList();

            if (vEnums.Count == 0)
                return string.Empty;

            string ret = string.Join(Options.GetNewLineText(), vEnums.Select(e => GetEnumString(e, baseIndentLvl)));

            if (Options.PrintSectionName)
                ret = GetSectionHeading("Enums", baseIndentLvl) + ret;

            return FinalizeSection(ret);
        }

        private string ProcessConstants(List<CppConstant> constants, int baseIndentLvl)
        {
            List<string> values = constants
                .Where(c => !string.IsNullOrWhiteSpace(c.Name) && !string.IsNullOrWhiteSpace(c.Type) && !string.IsNullOrWhiteSpace(c.Value) && ProcessConditions(c.Condition))
                .Select(c => $"static constexpr {c.Type} {c.Name} = {c.Value}")
                .ToList();

            if (values.Count == 0)
                return string.Empty;

            string ret = Helper.JoinString(
                Options.GetNewLineText(),
                values,
                GetIndent(baseIndentLvl),
                ";");

            if (Options.PrintSectionName && values.Count > 0)
                ret = GetSectionHeading("Constants", baseIndentLvl) + ret;

            return FinalizeSection(ret);
        }

        private string ProcessFunctions(List<CppFunction> functions, CppStruct parent, int baseIndentLvl)
        {
            List<CppFunction> funcs = functions
                .Where(e => !string.IsNullOrWhiteSpace(e.Name) && !string.IsNullOrWhiteSpace(e.Type) && ProcessConditions(e.Condition))
                .ToList();

            if (funcs.Count == 0)
                return string.Empty;

            string ret = string.Join(Options.GetNewLineText(), funcs.Select(f => GetFunctionsString(f, parent, baseIndentLvl, false)));

            if (Options.PrintSectionName)
                ret = GetSectionHeading("Functions", baseIndentLvl) + ret;

            return FinalizeSection(ret);
        }

        private string GenerateStructsFile()
        {
            int indentLvl = 0;
            var sb = new StringBuilder();

            // Pragmas
            sb.Append(ProcessPragmas(Model.Pragmas, indentLvl));

            // HeadingComment
            sb.Append(ProcessMultiComment(Model.HeadingComment, indentLvl));

            // Defines
            sb.Append(ProcessDefines(Model.Defines, indentLvl));

            // BeforeNameSpace
            sb.Append(Model.BeforeNameSpace + Options.GetNewLineText());
            sb.Append(Options.GetNewLineText());

            // NameSpace
            if (!string.IsNullOrWhiteSpace(Model.NameSpace))
            {
                sb.Append(GetIndent(indentLvl));
                sb.Append($"namespace {Model.NameSpace}{Options.GetNewLineText()}{{{Options.GetNewLineText()}");
                indentLvl++;
            }

            // Forwards
            sb.Append(ProcessForwards(Model.Forwards, indentLvl));

            // Enums
            sb.Append(ProcessEnums(Model.Enums, indentLvl));

            // Constants
            sb.Append(ProcessConstants(Model.Constants, indentLvl));

            // Functions
            sb.Append(ProcessFunctions(Model.Functions, null, indentLvl));

            // Close NameSpace
            if (!string.IsNullOrWhiteSpace(Model.NameSpace))
            {
                indentLvl--;
                sb.Append($"{GetIndent(indentLvl)}}}{Options.GetNewLineText()}");
                sb.Append(Options.GetNewLineText());
            }

            // AfterNameSpace
            sb.AppendLine(Model.AfterNameSpace);
            sb.Append(Options.GetNewLineText());

            return sb.ToString();
        }

        public void Process(CppModel cppModel, CppLangOptions options = null)
        {
            Options = options ?? new CppLangOptions();
            Model = cppModel;
            Model.Conditions = Model.Conditions.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
        }

        public void Process(string jsonData, CppLangOptions options = null)
        {
            Process(JsonConvert.DeserializeObject<CppModel>(jsonData), options);
        }

        public Dictionary<string, string> Generate()
        {
            if (Model is null)
                throw new Exception($"Call '{nameof(Process)}' first");

            return new Dictionary<string, string>
            {
                { $"{Model.Name}_Structs.h", GenerateStructsFile() }
            };
        }
    }
}
