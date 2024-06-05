using System.Collections.Generic;
using System.Diagnostics;

/*
 * ToDo:
 * - Add string above and before class/struct/enums
 * - Add Virtual functions to class/struct
 */

namespace LangPrint.Cpp;

[DebuggerDisplay("{" + nameof(Name) + "}" + " {" + nameof(Value) + "}")]
public class CppDefine : PackageItemBase
{
    public string Name { get; set; }
    public string Value { get; set; }

    public CppDefine(string name, string value)
    {
        Name = name;
        Value = value;
    }
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public sealed class CppConstant : CppDefine
{
    public string Type { get; set; }

    public CppConstant(string type, string name, string value) : base(name, value)
    {
        Type = type;
    }
}

[DebuggerDisplay("{" + nameof(UnderlyingType) + "}" + " {" + nameof(Name) + "}")]
public sealed class CppEnum : PackageItemBase
{
    public string Name { get; set; }
    public string? UnderlyingType { get; set; }
    public bool IsClass { get; set; }
    public Dictionary<string, string> Items { get; set; }
    public bool UseHexValues { get; set; }

    public CppEnum(string name)
    {
        Name = name;
        Items = new Dictionary<string, string>(System.StringComparer.Ordinal);
    }
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public sealed class CppParameter : PackageItemBase
{
    public string Type { get; set; }
    public string Name { get; set; }

    public CppParameter(string type, string name)
    {
        Type = type;
        Name = name;
    }
}

// TODO: Add virtual functions

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public sealed class CppFunction : PackageItemBase
{
    public string Type { get; set; }
    public string Name { get; set; }
    public List<string> TemplateParameters { get; set; }
    public List<CppParameter> Parameters { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsStatic { get; set; }
    public bool IsConst { get; set; }
    public bool IsFriend { get; set; }
    public bool IsInline { get; set; }
    public List<string> Body { get; set; }

    public CppFunction(string type, string name)
    {
        Type = type;
        Name = name;
        TemplateParameters = [];
        Parameters = [];
        Body = [];
    }
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public sealed class CppField : PackageItemBase
{
    public string Type { get; set; }
    public string Name { get; set; }
    public string? DefaultValue { get; set; }
    public string? ArrayDim { get; set; }
    public string? Bitfield { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsExtern { get; set; }
    public bool IsStatic { get; set; }
    public bool IsFriend { get; set; }
    public bool IsConst { get; set; }
    public bool IsConstexpr { get; set; }
    public bool IsUnion { get; set; }
    public bool ForceUnion { get; set; }

    public CppField(string type, string name)
    {
        Type = type;
        Name = name;
    }
}

[DebuggerDisplay("{" + nameof(Name) + "}")]
public sealed class CppStruct : PackageItemBase
{
    public string Name { get; set; }
    public List<string> Supers { get; set; }
    public int Alignas { get; set; }
    public bool IsClass { get; set; }
    public bool IsUnion { get; set; }
    public List<string> TemplateParams { get; set; }
    public List<string> Friends { get; set; }
    public List<CppField> Fields { get; set; }
    public List<CppFunction> Methods { get; set; }

    public CppStruct(string name)
    {
        Name = name;
        TemplateParams = [];
        Friends = [];
        Fields = [];
        Methods = [];
        Supers = [];
    }
}
