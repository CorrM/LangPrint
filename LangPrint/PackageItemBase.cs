using System.Collections.Generic;
using Newtonsoft.Json;

namespace LangPrint;

public class PackageItemBase
{
    [JsonProperty("Comments")]
    public List<string> Comments { get; set; } = new();

    [JsonProperty("InlineComment")]
    public string InlineComment { get; set; }

    [JsonProperty("Conditions")]
    public List<string> Conditions { get; set; } = new();
}