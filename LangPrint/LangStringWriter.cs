using System.Collections.Generic;
using System.Text;
using LangPrint.Utils;

namespace LangPrint;

/// <summary>
/// Custom <see cref="StringBuilder"/>
/// </summary>
public sealed class LangStringWriter
{
    private readonly StringBuilder _sb;
    private readonly LangOptions _langOptions;

    /// <summary>
    /// Current indent level
    /// </summary>
    public int IndentLevel { get; set; }

    /// <summary>
    /// String length
    /// </summary>
    public int Length => _sb.Length;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="langOptions">Language processor options</param>
    /// <param name="str">String to init builder with</param>
    public LangStringWriter(LangOptions langOptions, string? str = null)
    {
        _langOptions = langOptions;
        _sb = str is not null ? new StringBuilder(ResolveVariables(str)) : new StringBuilder();
    }

    private string ResolveVariables(string str)
    {
        foreach (KeyValuePair<string, string> variable in _langOptions.Variables)
        {
            str = str.Replace($"{{{_langOptions.VariablePrefix}{variable.Key}}}", variable.Value);
        }

        return str;
    }

    /// <summary>
    /// Removes the specified range of characters from this instance.
    /// </summary>
    /// <param name="index">The zero-based position in this instance where removal begins.</param>
    /// <param name="len">The number of characters to remove.</param>
    public LangStringWriter Remove(int index, int len)
    {
        _sb.Remove(index, len);
        return this;
    }

    /// <summary>
    /// Replaces all occurrences of a specified string in this instance with another specified string.
    /// </summary>
    /// <param name="old">Text to be replaced</param>
    /// <param name="newStr">Text to replace with</param>
    public LangStringWriter Replace(string old, string newStr)
    {
        _sb.Replace(old, ResolveVariables(newStr));
        return this;
    }

    /// <summary>
    /// Appends a copy of the specified string to this instance.
    /// </summary>
    /// <param name="value">Text to append</param>
    public void Append(string value)
    {
        string val = Helper.GetIndent(IndentLevel) + value;
        _sb.Append(ResolveVariables(val));
    }

    /// <summary>
    /// Appends a copy of the specified char to this instance.
    /// </summary>
    /// <param name="value">Char to add</param>
    public void Append(char value)
    {
        string val = Helper.GetIndent(IndentLevel) + value;
        _sb.Append(ResolveVariables(val));
    }

    /// <summary>
    /// Appends a copy of the specified string to this instance.
    /// </summary>
    /// <param name="value">Text to append</param>
    public void AppendLine(string? value = null)
    {
        string val = Helper.GetIndent(IndentLevel) + value;
        _sb.AppendLine(ResolveVariables(val));
    }

    /// <summary>
    /// Appends a copy of the specified char to this instance.
    /// </summary>
    /// <param name="value">Char to append</param>
    public void AppendLine(char value)
    {
        string val = Helper.GetIndent(IndentLevel) + value;
        _sb.AppendLine(ResolveVariables(val));
    }

    /// <summary>
    /// Removes all characters from the current StringWriter instance
    /// </summary>
    public void Clear()
    {
        _sb.Clear();
    }

    /// <summary>
    /// Converts the value of this instance to a System.String.
    /// </summary>
    /// <returns>A string whose value is the same as this instance.</returns>
    public override string ToString()
    {
        return _sb.ToString();
    }

    /// <summary>
    /// Cast to <see cref="string"/>
    /// </summary>
    public static implicit operator string(LangStringWriter builder)
    {
        return builder.ToString();
    }
}
