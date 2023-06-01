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

    [JsonProperty("BeforePrint")]
    public List<string> BeforePrint { get; set; } = new();

    [JsonProperty("AfterPrint")]
    public List<string> AfterPrint { get; set; } = new();
}
