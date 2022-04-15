using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangPrint.CSharp;
using Xunit;
using Xunit.Abstractions;

namespace LangPrint.Tests;

public class CSharpTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public CSharpTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Test_OneFileGenerate()
    {
        string file = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "CSharpExample.json"));

        var cppOpts = new CSharpLangOptions()
        {
            GeneratePackageSyntax = false,
            PrintSectionName = true,
            FieldMemberTypePadSize = 40
        };

        var cSharp = new CSharpProcessor();
        cSharp.Init(cppOpts);

        CSharpPackage package = cSharp.ModelFromJson(file);

        Dictionary<string, string> cppGen = cSharp.GenerateFiles(package);
        string savePath = Path.Combine(Environment.CurrentDirectory, "Test_OneFileGenerate");
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        foreach ((string fileName, string fileContent) in cppGen)
        {
            File.WriteAllText(Path.Combine(savePath, fileName), fileContent);
        }

        _testOutputHelper.WriteLine(cppGen.ElementAt(0).Value);
    }
}
