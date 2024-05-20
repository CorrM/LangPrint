using System.Collections.Generic;
using System.Diagnostics;

namespace LangPrint.CSharp;

[DebuggerDisplay(" {" + nameof(Name) + "}")]
public sealed class CSharpAttribute : PackageItemBase
{
    public string AppliesTo { get; set; }
    public string Name { get; set; }
    public List<string> Arguments { get; set; } = new();
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public sealed class CSharpDelegate : PackageItemBase
{
    public List<CSharpAttribute> Attributes { get; set; } = new();
    public string AccessModifier { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public List<string> GenericParams { get; set; } = new();
    public List<CSharpParameter> Params { get; set; } = new();
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public sealed class CSharpEvent : PackageItemBase
{
    public List<CSharpAttribute> Attributes { get; set; } = new();
    public string AccessModifier { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public bool IsStatic { get; set; }
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public sealed class CSharpEnum : PackageItemBase
{
    public List<CSharpAttribute> Attributes { get; set; }
    public string? AccessModifier { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public Dictionary<string, string> Values { get; set; }
    public bool HexValues { get; set; }

    public CSharpEnum()
    {
        Attributes = new List<CSharpAttribute>();
        Values = new Dictionary<string, string>(System.StringComparer.Ordinal);
    }
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public sealed class CSharpParameter : PackageItemBase
{
    public List<CSharpAttribute> Attributes { get; set; } = new();
    public string Type { get; set; }
    public string Name { get; set; }
    public bool IsRef { get; set; }
    public bool IsOut { get; set; }
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public sealed class CSharpFunction : PackageItemBase
{
    public List<CSharpAttribute> Attributes { get; set; } = new();
    public string AccessModifier { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public List<string> GenericParams { get; set; } = new();
    public List<CSharpParameter> Params { get; set; } = new();
    public bool IsStatic { get; set; }
    public bool IsAbstract { get; set; }
    public bool IsVirtual { get; set; }
    public bool IsUnsafe { get; set; }
    public bool IsOverride { get; set; }
    public bool IsExtern { get; set; }
    public bool IsAsync { get; set; }
    public List<string> Body { get; set; } = new();
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public sealed class CSharpField : PackageItemBase
{
    public List<CSharpAttribute> Attributes { get; set; } = new();
    public string AccessModifier { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public bool IsArray { get; set; }
    public bool IsReadOnly { get; set; }
    public bool IsVolatile { get; set; }
    public bool IsStatic { get; set; }
    public bool IsConst { get; set; }
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public sealed class CSharpProperty : PackageItemBase
{
    public List<CSharpAttribute> Attributes { get; set; } = new();
    public string AccessModifier { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public bool IsArray { get; set; }
    public bool IsStatic { get; set; }
    public bool IsAbstract { get; set; }
    public bool IsOverride { get; set; }
    public bool IsVirtual { get; set; }
    public bool HaveGetter { get; set; }
    public bool HaveSetter { get; set; }
    public List<string> GetterCode { get; set; } = new();
    public List<string> SetterCode { get; set; } = new();
}

[DebuggerDisplay("{" + nameof(Name) + "}")]
public sealed class CSharpStruct : PackageItemBase
{
    public List<CSharpAttribute> Attributes { get; set; } = new();
    public string AccessModifier { get; set; }
    public string Name { get; set; }
    public string Super { get; set; }
    public List<string> Interfaces { get; set; } = new();
    public bool IsClass { get; set; }
    public bool IsInterface { get; set; }
    public bool IsStatic { get; set; }
    public bool IsAbstract { get; set; }
    public bool IsSealed { get; set; }
    public bool IsReadOnly { get; set; }
    public List<string> GenericParams { get; set; } = new();
    public List<CSharpDelegate> Delegates { get; set; } = new();
    public List<CSharpEvent> Events { get; set; } = new();
    public List<CSharpField> Fields { get; set; } = new();
    public List<CSharpProperty> Properties { get; set; } = new();
    public List<CSharpFunction> Methods { get; set; } = new();
}

public sealed class CSharpPackage
{
    public string Name { get; set; }
    public List<string> Usings { get; set; } = new();
    public List<string> HeadingComment { get; set; } = new();
    public string NameSpace { get; set; }
    public string BeforeNameSpace { get; set; }
    public string AfterNameSpace { get; set; }
    public List<CSharpDelegate> Delegates { get; set; } = new();
    public List<CSharpEnum> Enums { get; set; } = new();
    public List<CSharpStruct> Structs { get; set; } = new();
    public string Lang { get; private set; }
}
