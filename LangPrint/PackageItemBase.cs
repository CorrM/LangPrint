using System.Collections.Generic;

namespace LangPrint;

public class PackageItemBase
{
    public List<string> Comments { get; set; }
    public string? InlineComment { get; set; }
    public List<string> BeforePrint { get; set; }
    public List<string> AfterPrint { get; set; }

    protected PackageItemBase()
    {
        Comments = new List<string>();
        BeforePrint = new List<string>();
        AfterPrint = new List<string>();
    }
}
