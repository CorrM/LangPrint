using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace LangPrint.CSharp;

[DebuggerDisplay(" {" + nameof(Name) + "}")]
public class CSharpAttribute : PackageItemBase
{
    [JsonProperty("AppliesTo")]
    public string AppliesTo { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Arguments")]
    public List<string> Arguments { get; set; } = new();
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public class CSharpDelegate : PackageItemBase
{
    [JsonProperty("Attributes")]
    public List<CSharpAttribute> Attributes { get; set; } = new();

    [JsonProperty("AccessModifier")]
    public string AccessModifier { get; set; }

    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("GenericParams")]
    public List<string> GenericParams { get; set; } = new();

    [JsonProperty("Params")]
    public List<CSharpParameter> Params { get; set; } = new();
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public class CSharpEvent : PackageItemBase
{
    [JsonProperty("Attributes")]
    public List<CSharpAttribute> Attributes { get; set; } = new();

    [JsonProperty("AccessModifier")]
    public string AccessModifier { get; set; }

    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("IsStatic")]
    public bool IsStatic { get; set; }
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public class CSharpEnum : PackageItemBase
{
    [JsonProperty("Attributes")]
    public List<CSharpAttribute> Attributes { get; set; } = new();

    [JsonProperty("AccessModifier")]
    public string AccessModifier { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("Values")]
    public List<PackageNameValue> Values { get; set; } = new();

    [JsonProperty("HexValues")]
    public bool HexValues { get; set; }
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public class CSharpParameter : PackageItemBase
{
    [JsonProperty("Attributes")]
    public List<CSharpAttribute> Attributes { get; set; } = new();

    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("IsRef")]
    public bool IsRef { get; set; }

    [JsonProperty("IsOut")]
    public bool IsOut { get; set; }
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public class CSharpFunction : PackageItemBase
{
    [JsonProperty("Attributes")]
    public List<CSharpAttribute> Attributes { get; set; } = new();

    [JsonProperty("AccessModifier")]
    public string AccessModifier { get; set; }

    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("GenericParams")]
    public List<string> GenericParams { get; set; } = new();

    [JsonProperty("Params")]
    public List<CSharpParameter> Params { get; set; } = new();

    [JsonProperty("IsStatic")]
    public bool IsStatic { get; set; }

    [JsonProperty("IsAbstract")]
    public bool IsAbstract { get; set; }

    [JsonProperty("IsVirtual")]
    public bool IsVirtual { get; set; }

    [JsonProperty("IsUnsafe")]
    public bool IsUnsafe { get; set; }

    [JsonProperty("IsOverride")]
    public bool IsOverride { get; set; }

    [JsonProperty("IsExtern")]
    public bool IsExtern { get; set; }

    [JsonProperty("IsAsync")]
    public bool IsAsync { get; set; }

    [JsonProperty("Body")]
    public List<string> Body { get; set; } = new();
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public class CSharpField : PackageItemBase
{
    [JsonProperty("Attributes")]
    public List<CSharpAttribute> Attributes { get; set; } = new();

    [JsonProperty("AccessModifier")]
    public string AccessModifier { get; set; }

    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Value")]
    public string Value { get; set; }

    [JsonProperty("IsArray")]
    public bool IsArray { get; set; }

    [JsonProperty("IsReadOnly")]
    public bool IsReadOnly { get; set; }

    [JsonProperty("IsVolatile")]
    public bool IsVolatile { get; set; }

    [JsonProperty("IsStatic")]
    public bool IsStatic { get; set; }

    [JsonProperty("IsConst")]
    public bool IsConst { get; set; }
}

[DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
public class CSharpProperty : PackageItemBase
{
    [JsonProperty("Attributes")]
    public List<CSharpAttribute> Attributes { get; set; } = new();

    [JsonProperty("AccessModifier")]
    public string AccessModifier { get; set; }

    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Value")]
    public string Value { get; set; }

    [JsonProperty("IsArray")]
    public bool IsArray { get; set; }

    [JsonProperty("IsStatic")]
    public bool IsStatic { get; set; }

    [JsonProperty("IsAbstract")]
    public bool IsAbstract { get; set; }

    [JsonProperty("IsOverride")]
    public bool IsOverride { get; set; }

    [JsonProperty("IsVirtual")]
    public bool IsVirtual { get; set; }

    [JsonProperty("HaveGetter")]
    public bool HaveGetter { get; set; }

    [JsonProperty("HaveSetter")]
    public bool HaveSetter { get; set; }

    [JsonProperty("GetterCode")]
    public List<string> GetterCode { get; set; } = new();

    [JsonProperty("SetterCode")]
    public List<string> SetterCode { get; set; } = new();
}

[DebuggerDisplay("{" + nameof(Name) + "}")]
public class CSharpStruct : PackageItemBase
{
    [JsonProperty("Attributes")]
    public List<CSharpAttribute> Attributes { get; set; } = new();

    [JsonProperty("AccessModifier")]
    public string AccessModifier { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Super")]
    public string Super { get; set; }

    [JsonProperty("Interfaces")]
    public List<string> Interfaces { get; set; } = new();

    [JsonProperty("IsClass")]
    public bool IsClass { get; set; }

    [JsonProperty("IsInterface")]
    public bool IsInterface { get; set; }

    [JsonProperty("IsStatic")]
    public bool IsStatic { get; set; }

    [JsonProperty("IsAbstract")]
    public bool IsAbstract { get; set; }

    [JsonProperty("IsSealed")]
    public bool IsSealed { get; set; }

    [JsonProperty("IsReadOnly")]
    public bool IsReadOnly { get; set; }

    [JsonProperty("GenericParams")]
    public List<string> GenericParams { get; set; } = new();

    [JsonProperty("Delegates")]
    public List<CSharpDelegate> Delegates { get; set; } = new();

    [JsonProperty("Events")]
    public List<CSharpEvent> Events { get; set; } = new();

    [JsonProperty("Fields")]
    public List<CSharpField> Fields { get; set; } = new();

    [JsonProperty("Properties")]
    public List<CSharpProperty> Properties { get; set; } = new();

    [JsonProperty("Methods")]
    public List<CSharpFunction> Methods { get; set; } = new();
}

public class CSharpPackage : ILang
{
    [JsonProperty("Lang")]
    public string Lang { get; private set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Usings")]
    public List<string> Usings { get; set; } = new();

    [JsonProperty("HeadingComment")]
    public List<string> HeadingComment { get; set; } = new();

    [JsonProperty("Conditions")]
    public List<string> Conditions { get; set; } = new();

    [JsonProperty("NameSpace")]
    public string NameSpace { get; set; }

    [JsonProperty("BeforeNameSpace")]
    public string BeforeNameSpace { get; set; }

    [JsonProperty("AfterNameSpace")]
    public string AfterNameSpace { get; set; }

    [JsonProperty("Delegates")]
    public List<CSharpDelegate> Delegates { get; set; } = new();

    [JsonProperty("Enums")]
    public List<CSharpEnum> Enums { get; set; } = new();

    [JsonProperty("Structs")]
    public List<CSharpStruct> Structs { get; set; } = new();
}