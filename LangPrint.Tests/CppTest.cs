using System;
using System.IO;
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
        public void Test1()
        {
            string file = File.ReadAllText(@"D:\Projects\CheatGear\src\CheatGear\Settings\Engines\UnrealEngine\Packages\BasicTypes.json");

            var cpp = new CppProcessor();
            cpp.Process(file);

            string genStr = cpp.Generate();

            _testOutputHelper.WriteLine(genStr);
        }
    }
}
