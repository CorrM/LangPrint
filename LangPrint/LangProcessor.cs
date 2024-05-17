using System.Collections.Generic;
using System.Linq;
using LangPrint.Utils;

namespace LangPrint;

public abstract class LangProcessor<TOptions> where TOptions : LangOptions
{
    public abstract TOptions Options { get; protected set; }
    // TModel? Model { get; }

    public string GetBeforePrint(PackageItemBase item, int baseIndentLvl)
    {
        return Helper.JoinString(Options.GetNewLineText(), item.BeforePrint, Helper.GetIndent(baseIndentLvl));
    }

    public string GetAfterPrint(PackageItemBase item, int baseIndentLvl)
    {
        return Helper.JoinString(Options.GetNewLineText(), item.AfterPrint, Helper.GetIndent(baseIndentLvl));
    }

    public bool ResolveConditions(List<string> conditions, List<string>? conditionsToResolve)
    {
        if (!Options.ResolveConditions)
        {
            return true;
        }

        if (conditionsToResolve is null || conditionsToResolve.Count == 0)
        {
            return true;
        }

        // ! conditions
        IEnumerable<string> conds = conditionsToResolve.Where(c => !string.IsNullOrWhiteSpace(c) && c.StartsWith('!'));
        foreach (string condition in conds)
        {
            if (conditions.Exists(
                    gCondition => string.Equals(condition[1..], gCondition, System.StringComparison.Ordinal)
                ))
            {
                return false;
            }
        }

        // All conditions must be fitted
        return conditionsToResolve.Where(c => !string.IsNullOrWhiteSpace(c) && !c.StartsWith('!'))
            .All(conditions.Contains);
    }
}
