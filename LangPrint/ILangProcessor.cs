using System.Collections.Generic;

namespace LangPrint
{
    public interface ILangProcessor<out T>
    {
        T Model { get; }

        T Process(string jsonData, LangPrintOptions options);
        Dictionary<string, string> Generate();
    }
}