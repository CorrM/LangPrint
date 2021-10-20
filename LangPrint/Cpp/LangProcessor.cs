using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LangPrint.Utils;
using Newtonsoft.Json;

namespace LangPrint.Cpp
{
    public class CppProcessor : ILangProcessor<CppModel>
    {
        public string JsonData { get; private set; }
        public LangPrintOptions Options { get; private set; }
        public CppModel Model { get; private set; }

        private string ProcessPragmas(List<string> pragmaList)
        {
            return Helper.JoinString(Options.GetNewLineText(), pragmaList, "#pragma ");
        }

        private string ProcessMultiComment(List<string> commentList)
        {
            return $"/*{Options.GetNewLineText()}" +
                   Helper.JoinString(Options.GetNewLineText(), commentList, " * ") +
                   $"{Options.GetNewLineText()} */";
        }

        private string GenerateClasses()
        {
            var sb = new StringBuilder();

            // Pragmas
            sb.AppendLine(ProcessPragmas(Model.Pragmas));
            sb.Append(Options.GetNewLineText());

            // HeadingComment
            sb.AppendLine(ProcessMultiComment(Model.HeadingComment));
            sb.Append(Options.GetNewLineText());

            return sb.ToString();
        }

        public CppModel Process(string jsonData, LangPrintOptions options = null)
        {
            JsonData = jsonData;
            Options = options ?? new LangPrintOptions();
            Model = JsonConvert.DeserializeObject<CppModel>(JsonData);
            return Model;
        }

        public Dictionary<string, string> Generate()
        {
            if (Model is null)
                throw new Exception($"Call '{nameof(Process)}' first");

            return new Dictionary<string, string>
            {
                { $"{Model.Name}_classes.h", GenerateClasses() }
            };
        }
    }
}
