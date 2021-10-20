using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangPrint
{
    public enum NewLineType
    {
        CRLF,
        LF
    }

    public class LangPrintOptions
    {
        public NewLineType NewLine { get; init; } = NewLineType.CRLF;

        public string GetNewLineText()
        {
            return NewLine switch
            {
                NewLineType.CRLF => "\r\n",
                NewLineType.LF => "\n",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
