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
        public void Test1()
        {
            string file = File.ReadAllText(@"D:\Projects\CheatGear\src\CheatGear\Settings\Engines\UnrealEngine\Packages\BasicTypes.json");

            var cpp = new CppProcessor();
            cpp.Process(file);

            Dictionary<string, string> cppGen = cpp.Generate();
            string genStr = cppGen.ElementAt(0).Value;

            _testOutputHelper.WriteLine(genStr);
        }
    }
}
