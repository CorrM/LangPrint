using System.Collections.Generic;
using System.Linq;

namespace LangPrint.Utils;

public static class Helper
{
    public static string JoinString(
        string separator,
        IEnumerable<string> values,
        string prefix = null,
        string suffix = null
    )
    {
        return string.Join(separator, values.Select(s => $"{prefix}{s}{suffix}"));
    }

    public static string GetIndent(int lvl)
    {
        return lvl <= 0
            ? string.Empty
            : string.Concat(Enumerable.Repeat("\t", lvl));
        //: string.Concat(Enumerable.Repeat(new string(' ', Options.IndentSize), lvl));
    }

    public static string FinalizeSection(string sectionStr, string newLineStr)
    {
        if (string.IsNullOrWhiteSpace(sectionStr))
        {
            return sectionStr;
        }

        if (sectionStr.EndsWith(newLineStr + newLineStr))
        {
            return sectionStr;
        }

        if (sectionStr.EndsWith(newLineStr))
        {
            return sectionStr + newLineStr;
        }

        return sectionStr + newLineStr + newLineStr;
    }
}
