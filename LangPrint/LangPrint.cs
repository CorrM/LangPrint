using System.Collections.Generic;
using System.Linq;

namespace LangPrint;

public static class LangPrint
{
    public static bool ResolveConditions(List<string> conditions, List<string>? conditionsToResolve)
    {
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