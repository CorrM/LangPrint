using System.Collections.Generic;

namespace LangPrint;

public interface ILangProcessor<TModel, TOptions> where TModel : ILangModel where TOptions : LangOptions
{
    TOptions Options { get; }
    //TModel Model { get; }

    void Init(TOptions options);
    TModel ModelFromJson(string jsonData);
    Dictionary<string, string> GenerateFiles(TModel cppModel);
}
