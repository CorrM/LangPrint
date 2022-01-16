namespace LangPrint.Cpp;

public class CppLangOptions : LangOptions
{
    public bool GeneratePackageSyntax { get; set; } = false;
    public int VariableMemberTypePadSize { get; set; } = 0;
    public int InlineCommentPadSize { get; set; } = 0;
    public bool ResolveConditions { get; set; } = true;
    public bool AddPackageHeaderToCppFile { get; set; } = true;
}
