namespace LangPrint.CSharp;

public class CSharpLangOptions : LangOptions
{
    public bool GeneratePackageSyntax { get; set; } = false;
    public int FieldMemberTypePadSize { get; set; } = 0;
    public int InlineCommentPadSize { get; set; } = 0;
    public bool ResolveConditions { get; set; } = true;
}
