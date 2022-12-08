using System.Collections.Generic;
using System.Linq;
using LangPrint.Utils;

namespace LangPrint;

public abstract class LangProcessor<TModel, TOptions> where TModel : ILang where TOptions : LangOptions
{
    public abstract TOptions Options { get; protected set; }
    // TModel? Model { get; }

    public abstract void Init(TOptions? options = null);

    public abstract TModel? ModelFromJson(string jsonData);

    public abstract Dictionary<string, string> GenerateFiles(TModel cppModel);

    public string GetBeforePrint(PackageItemBase item, int baseIndentLvl)
    {
        return Helper.JoinString(Options.GetNewLineText(), item.BeforePrint, Helper.GetIndent(baseIndentLvl)) + Options.GetNewLineText();
    }
    
    public string GetAfterPrint(PackageItemBase item, int baseIndentLvl)
    {
        return Helper.JoinString(Options.GetNewLineText(), item.AfterPrint, Helper.GetIndent(baseIndentLvl)) + Options.GetNewLineText();
    }
    
    public bool ResolveConditions(List<string> conditions, List<string>? conditionsToResolve)
    {
        if (!Options.ResolveConditions)
            return true;

        if (conditionsToResolve is null || conditionsToResolve.Count == 0)
            return true;

        // ! conditions
        foreach (string condition in conditionsToResolve.Where(c => !string.IsNullOrWhiteSpace(c) && c.StartsWith("!")))
        {
            if (conditions.Any(gCondition => condition[1..] == gCondition))
                return false;
        }

        // All conditions must to be fitted
        return conditionsToResolve
            .Where(c => !string.IsNullOrWhiteSpace(c) && !c.StartsWith("!"))
            .All(conditions.Contains);
    }
}
