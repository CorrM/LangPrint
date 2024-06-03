namespace LangPrint.Cpp;

public sealed class CppLangOptions : LangOptions
{
    public int VariableMemberTypePadSize { get; set; } = 0;
    public int InlineCommentPadSize { get; set; } = 0;
    public bool AddPackageHeaderToCppFile { get; set; } = true;
}
