using System;
using System.Collections.Generic;

namespace LangPrint;

public enum NewLineType
{
    CRLF,
    LF
}

public abstract class LangOptions
{
    public NewLineType NewLine { get; init; } = NewLineType.CRLF;
    public bool PrintSectionName { get; init; } = true;
    public bool ResolveConditions { get; set; } = true;
    public string VariablePrefix { get; init; } = "$VAR_";
    public Dictionary<string, string> Variables { get; } = new();

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