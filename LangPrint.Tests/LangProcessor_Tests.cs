using LangPrint.Cpp;
using Xunit;

namespace LangPrint.Tests;

public class LangProcessorTests
{
    public LangProcessorTests()
    {
        
    }
    
    [Fact]
    public void ResolveVariables_ExpectedBehavior()
    {
        // Processor
        var cppOpts = new CppLangOptions()
        {
            NewLine = NewLineType.CRLF,
            PrintSectionName = true,
            InlineCommentPadSize = 56,
            VariableMemberTypePadSize = 60,
            GeneratePackageSyntax = true,
            AddPackageHeaderToCppFile = false,
            Variables =
            {
                { "T1", "int" },
                { "V1", "MyVar" },
            }
        };
        
        var cppProcessor = new CppProcessor();
        cppProcessor.Init(cppOpts);
        
        string resolveVariables = cppProcessor.GetParamString(new CppParameter() { Type = "{$VAR_T1}", Name = "{$VAR_V1}"});

        Assert.Equal("int MyVar", resolveVariables);
    }
}
