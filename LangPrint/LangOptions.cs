using System;

namespace LangPrint;

public enum NewLineType
{
    CRLF,
    LF
}

public class LangOptions
{
    public NewLineType NewLine { get; init; } = NewLineType.CRLF;
    public bool PrintSectionName { get; init; } = true;

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