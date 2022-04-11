# LangPrint


## Usage

```csharp
// Initialize
CppProcessor cppProcessor = new CppProcessor();
var cppOpts = new CppLangOptions()
{
    NewLine = NewLineType.CRLF,
    PrintSectionName = true,
    InlineCommentPadSize = 56,
    VariableMemberTypePadSize = 60,
    GeneratePackageSyntax = true,
    AddPackageHeaderToCppFile = false
};
_cppProcessor.Init(cppOpts);

// Generate
var cppPackage = new CppPackage()
{
    Name = "PackageName",
    NameSpace = "MyNameSpace",
    Pragmas = new List<string>() { "once" },
    Forwards = ...,
    TypeDefs = ...,
    Defines = ...,
    Constants = ...,
    Fields = ...,
    Functions = ...,
    Enums = ...,
    Structs = ... // Classes/Structs
};

// Generate FileName:FileContect dictionary
Dictionary<string, string> cppFiles = _cppProcessor.GenerateFiles(cppPackage);

// OR
Dictionary<string, string> cppFiles = _cppProcessor.GenerateFiles(_cppProcessor.ModelFromJson("JSON_STRING"));
```
