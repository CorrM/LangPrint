using System.Collections.Generic;

namespace LangPrint;

public interface ILangProcessor<TModel, TOptions> where TModel : ILang where TOptions : LangOptions
{
    TOptions Options { get; }
    //TModel Model { get; }

    void Init(TOptions options);
    TModel ModelFromJson(string jsonData);
    Dictionary<string, string> GenerateFiles(TModel cppModel);
}
