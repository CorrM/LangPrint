using System.Collections.Generic;

namespace LangPrint
{
    public interface ILangProcessor<TModel, TOptions>
    {
        TOptions Options { get; }
        TModel Model { get; }

        void Init(TModel model, TOptions options);
        void Init(string jsonData, TOptions options);
        Dictionary<string, string> GenerateFiles();
    }
}