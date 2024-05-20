using LangPrint.Utils;

namespace LangPrint;

public abstract class LangProcessor<TOptions> where TOptions : LangOptions
{
    public abstract TOptions Options { get; protected set; }

    public string GetBeforePrint(PackageItemBase item, int baseIndentLvl)
    {
        return Helper.JoinString(Options.GetNewLineText(), item.BeforePrint, Helper.GetIndent(baseIndentLvl));
    }

    public string GetAfterPrint(PackageItemBase item, int baseIndentLvl)
    {
        return Helper.JoinString(Options.GetNewLineText(), item.AfterPrint, Helper.GetIndent(baseIndentLvl));
    }
}
