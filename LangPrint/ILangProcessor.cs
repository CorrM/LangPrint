using System.Collections.Generic;

namespace LangPrint
{
    public interface ILangProcessor<in TModel, TOptions> where TModel : ILangModel where TOptions : LangOptions
    {
        TOptions Options { get; }
        //TModel Model { get; }

        void Init(TOptions options);
        Dictionary<string, string> GenerateFiles(TModel cppModel);
        Dictionary<string, string> GenerateFiles(string jsonData);
    }
}