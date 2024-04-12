using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

/*
 * ToDo:
 * - Add string above and before class/struct/enums
 * - Add Virtual functions to class/struct
 */

namespace LangPrint.Cpp;

[DebuggerDisplay("{" + nameof(Name) + "}" + " {" + nameof(Value) + "}")]
public class CppDefine : PackageItemBase
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

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public class CppEnum : PackageItemBase
{
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("IsClass")]
    public bool IsClass { get; set; }

    [JsonProperty("Values")]
    public List<PackageNameValue> Values { get; set; } = new();

    [JsonProperty("HexValues")]
    public bool HexValues { get; set; }
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public class CppParameter : PackageItemBase
{
    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }
}

// TODO: Add virtual functions

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public class CppFunction : PackageItemBase
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
public class CppField : PackageItemBase
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

    [JsonProperty("ForceUnion")]
    public bool ForceUnion { get; set; }
}

[DebuggerDisplay("{" + nameof(Name) + "}")]
public class CppStruct : PackageItemBase
{
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Supers")]
    public List<string> Supers { get; set; } = new();

    [JsonProperty("Alignas")]
    public int Alignas { get; set; }

    [JsonProperty("IsClass")]
    public bool IsClass { get; set; }

    [JsonProperty("IsUnion")]
    public bool IsUnion { get; set; }

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

    [JsonProperty("TypeDefs")]
    public List<string> TypeDefs { get; set; } = new();

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
    public List<CppField> Fields { get; set; } = new();

    [JsonProperty("Functions")]
    public List<CppFunction> Functions { get; set; } = new();

    [JsonProperty("Lang")]
    public string Lang { get; private set; }
}
