using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangPrint.Utils
{
    public static class Helper
    {
        public static string JoinString(string separator, IEnumerable<string> values, string prefix = null)
        {
            return string.Join(separator, values.Select(s => $"{prefix}{s}"));
        }
    }
}
