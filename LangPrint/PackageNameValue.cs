using System.Diagnostics;
using Newtonsoft.Json;

namespace LangPrint;

[DebuggerDisplay("{" + nameof(Name) + "}" + " {" + nameof(Value) + "}")]
public class PackageNameValue : PackageItemBase
{
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Value")]
    public string Value { get; set; }
}
