using System.Collections.Generic;

namespace LangPrint
{
    public interface ILangProcessor<TModel, in TOptions>
    {
        LangOptions Options { get; }
        TModel Model { get; }

        void Process(TModel model, TOptions options);
        void Process(string jsonData, TOptions options);
        Dictionary<string, string> Generate();
    }
}