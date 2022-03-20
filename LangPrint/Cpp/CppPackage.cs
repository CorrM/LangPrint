using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

/*
 * ToDo:
 * - Add string above and before class/struct/enums
 * - Add Virtual functions to class/struct
 */

namespace LangPrint.Cpp;

public class CppItemBase
{

    [JsonProperty("Comments")]
    public List<string> Comments { get; set; } = new();

    [JsonProperty("InlineComment")]
    public string InlineComment { get; set; }

    [JsonProperty("Conditions")]
    public List<string> Conditions { get; set; } = new();
}

[DebuggerDisplay("{" + nameof(Name) + "}" + " {" + nameof(Value) + "}")]
public class CppDefine : CppItemBase
{
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Value")]
    public string Value { get; set; }
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public class CppConstant : CppDefine
{
    [JsonProperty("Type")]
    public string Type { get; set; }
}

[DebuggerDisplay("{" + nameof(Name) + "}" + " {" + nameof(Value) + "}")]
public class CppNameValue : CppItemBase
{
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Value")]
    public string Value { get; set; }
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public class CppEnum : CppItemBase
{
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("IsClass")]
    public bool IsClass { get; set; }

    [JsonProperty("Values")]
    public List<CppNameValue> Values { get; set; } = new();

    [JsonProperty("HexValues")]
    public bool HexValues { get; set; }
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public class CppParameter : CppItemBase
{
    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public class CppFunction : CppItemBase
{
    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("TemplateParams")]
    public List<string> TemplateParams { get; set; } = new();

    [JsonProperty("Params")]
    public List<CppParameter> Params { get; set; } = new();

    [JsonProperty("Private")]
    public bool Private { get; set; }

    [JsonProperty("Static")]
    public bool Static { get; set; }

    [JsonProperty("Const")]
    public bool Const { get; set; }

    [JsonProperty("Friend")]
    public bool Friend { get; set; }

    [JsonProperty("Inline")]
    public bool Inline { get; set; }

    [JsonProperty("Body")]
    public List<string> Body { get; set; } = new();
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public class CppField : CppItemBase
{
    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Value")]
    public string Value { get; set; }

    [JsonProperty("ArrayDim")]
    public string ArrayDim { get; set; }

    [JsonProperty("Bitfield")]
    public string Bitfield { get; set; }

    [JsonProperty("Private")]
    public bool Private { get; set; }

    [JsonProperty("Extern")]
    public bool Extern { get; set; }

    [JsonProperty("Static")]
    public bool Static { get; set; }

    [JsonProperty("Friend")]
    public bool Friend { get; set; }

    [JsonProperty("Const")]
    public bool Const { get; set; }

    [JsonProperty("Constexpr")]
    public bool Constexpr { get; set; }

    [JsonProperty("Union")]
    public bool Union { get; set; }
}

[DebuggerDisplay("{" + nameof(Name) + "}")]
public class CppStruct : CppItemBase
{
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Supers")]
    public List<string> Supers { get; set; } = new();

    [JsonProperty("IsClass")]
    public bool IsClass { get; set; }

    [JsonProperty("TemplateParams")]
    public List<string> TemplateParams { get; set; } = new();

    [JsonProperty("Friends")]
    public List<string> Friends { get; set; } = new();

    [JsonProperty("Fields")]
    public List<CppField> Fields { get; set; } = new();

    [JsonProperty("Methods")]
    public List<CppFunction> Methods { get; set; } = new();
}

public class CppPackage : ILang
{
    [JsonProperty("Lang")]
    public string Lang { get; private set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Pragmas")]
    public List<string> Pragmas { get; set; } = new();

    [JsonProperty("Includes")]
    public List<string> Includes { get; set; } = new();

    [JsonProperty("CppIncludes")]
    public List<string> CppIncludes { get; set; } = new();

    [JsonProperty("PackageHeaderIncludes")]
    public List<string> PackageHeaderIncludes { get; set; } = new();

    [JsonProperty("HeadingComment")]
    public List<string> HeadingComment { get; set; } = new();

    [JsonProperty("Defines")]
    public List<CppDefine> Defines { get; set; } = new();

    [JsonProperty("Conditions")]
    public List<string> Conditions { get; set; } = new();

    [JsonProperty("NameSpace")]
    public string NameSpace { get; set; }

    [JsonProperty("BeforeNameSpace")]
    public string BeforeNameSpace { get; set; }

    [JsonProperty("AfterNameSpace")]
    public string AfterNameSpace { get; set; }

    [JsonProperty("CppBeforeNameSpace")]
    public string CppBeforeNameSpace { get; set; }

    [JsonProperty("CppAfterNameSpace")]
    public string CppAfterNameSpace { get; set; }

    [JsonProperty("Forwards")]
    public List<string> Forwards { get; set; } = new();

    [JsonProperty("Enums")]
    public List<CppEnum> Enums { get; set; } = new();

    [JsonProperty("Constants")]
    public List<CppConstant> Constants { get; set; } = new();

    [JsonProperty("Structs")]
    public List<CppStruct> Structs { get; set; } = new();

    [JsonProperty("Fields")]
    public List<CppField> Variables { get; set; } = new();

    [JsonProperty("Functions")]
    public List<CppFunction> Functions { get; set; } = new();
}
