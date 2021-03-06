using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LangPrint.Cpp;
using Xunit;
using Xunit.Abstractions;

namespace LangPrint.Tests
{
    public class CppTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public CppTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Test_OneFileGenerate()
        {
            string file = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "CppExample.json"));

            var cppOpts = new CppLangOptions()
            {
                GeneratePackageSyntax = false,
                PrintSectionName = true
            };

            var cpp = new CppProcessor();
            cpp.Init(cppOpts);

            CppPackage package = cpp.ModelFromJson(file);

            Dictionary<string, string> cppGen = cpp.GenerateFiles(package);
            string savePath = Path.Combine(Environment.CurrentDirectory, "Test_OneFileGenerate");
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            foreach ((string fileName, string fileContent) in cppGen)
            {
                File.WriteAllText(Path.Combine(savePath, fileName), fileContent);
            }

            _testOutputHelper.WriteLine(cppGen.ElementAt(0).Value);
        }

        [Fact]
        public void Test_PackageSyntaxGenerate()
        {
            string file = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "CppExample.json"));

            var cppOpts = new CppLangOptions()
            {
                GeneratePackageSyntax = true,
                PrintSectionName = true
            };

            var cpp = new CppProcessor();
            cpp.Init(cppOpts);

            CppPackage package = cpp.ModelFromJson(file);

            Dictionary<string, string> cppGen = cpp.GenerateFiles(package);
            string savePath = Path.Combine(Environment.CurrentDirectory, "Test_PackageSyntaxGenerate");
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            foreach ((string fileName, string fileContent) in cppGen)
            {
                File.WriteAllText(Path.Combine(savePath, fileName), fileContent);
            }
        }
    }
}
