using System.Linq;
using System.Text;

namespace LangPrint.Utils;

/// <summary>
/// Custom <see cref="StringBuilder"/>
/// </summary>
public class StringBuilder2
{
    /// <summary>
    /// <see cref="StringBuilder"/> that wrapped by this class
    /// </summary>
    public StringBuilder BaseBuilder { get; }

    /// <summary>
    /// Current indent level
    /// </summary>
    public int IndentLevel { get; set; }

    /// <summary>
    /// String length
    /// </summary>
    public int Length => BaseBuilder.Length;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="str">Text to init with</param>
    public StringBuilder2(string str)
    {
        BaseBuilder = new StringBuilder(str);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public StringBuilder2() : this(string.Empty)
    {
    }

    private static string GetIndent(int lvl)
    {
        return lvl == 0
            ? string.Empty
            : string.Concat(Enumerable.Repeat("\t", lvl));
        //: string.Concat(Enumerable.Repeat(new string(' ', Options.IndentSize), lvl));
    }

    /// <summary>
    /// Removes the specified range of characters from this instance.
    /// </summary>
    /// <param name="index">The zero-based position in this instance where removal begins.</param>
    /// <param name="len">The number of characters to remove.</param>
    public void Remove(int index, int len) => BaseBuilder.Remove(index, len);

    /// <summary>
    /// Replaces all occurrences of a specified string in this instance with another specified string.
    /// </summary>
    /// <param name="old">Text to be replaced</param>
    /// <param name="newStr">Text to replace with</param>
    public void Replace(string old, string newStr) => BaseBuilder.Replace(old, newStr);

    /// <summary>
    /// Appends a copy of the specified string to this instance.
    /// </summary>
    /// <param name="str">Text to append</param>
    public void Append(string str) => BaseBuilder.Append(GetIndent(IndentLevel) + str);

    /// <summary>
    /// Appends a copy of the specified string to this instance.
    /// </summary>
    /// <param name="str">Text to append</param>
    public void AppendLine(string str = null) => BaseBuilder.AppendLine(GetIndent(IndentLevel) + str);

    /// <summary>
    /// Converts the value of this instance to a System.String.
    /// </summary>
    /// <returns>A string whose value is the same as this instance.</returns>
    public override string ToString() => BaseBuilder.ToString();

    /// <summary>
    /// Cast to <see cref="string"/>
    /// </summary>
    public static implicit operator string(StringBuilder2 builder) => builder.ToString();
}