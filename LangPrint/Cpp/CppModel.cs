using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

/*
 * ToDo:
 * - Add string above and before class/struct/enums
 * - Add Virtual functions to class/struct
 */

namespace LangPrint.Cpp
{
    public class CppItemBase
    {

        [JsonProperty("Comment")]
        public List<string> Comment { get; set; }

        [JsonProperty("InlineComment")]
        public string InlineComment { get; set; }

        [JsonProperty("Condition")]
        public List<string> Condition { get; set; }
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
        public List<CppNameValue> Values { get; set; }
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
        public List<string> TemplateParams { get; set; }

        [JsonProperty("Params")]
        public List<CppParameter> Params { get; set; }

        [JsonProperty("Private")]
        public bool Private { get; set; }

        [JsonProperty("Static")]
        public bool Static { get; set; }

        [JsonProperty("Const")]
        public bool Const { get; set; }

        [JsonProperty("Inline")]
        public bool Inline { get; set; }

        [JsonProperty("Body")]
        public List<string> Body { get; set; }
    }

    [DebuggerDisplay("{" + nameof(Type) + "}" + " {" + nameof(Name) + "}")]
    public class CppVariable : CppItemBase
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
        public List<string> Supers { get; set; }

        [JsonProperty("IsClass")]
        public bool IsClass { get; set; }

        [JsonProperty("TemplateParams")]
        public List<string> TemplateParams { get; set; }

        [JsonProperty("Friends")]
        public List<string> Friends { get; set; }

        [JsonProperty("Variables")]
        public List<CppVariable> Variables { get; set; }

        [JsonProperty("Methods")]
        public List<CppFunction> Methods { get; set; }
    }

    public class CppModel : ILangModel
    {
        [JsonProperty("Lang")]
        public string Lang { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Pragmas")]
        public List<string> Pragmas { get; set; }

        [JsonProperty("Includes")]
        public List<string> Includes { get; set; }

        [JsonProperty("CppIncludes")]
        public List<string> CppIncludes { get; set; }

        [JsonProperty("HeadingComment")]
        public List<string> HeadingComment { get; set; }

        [JsonProperty("Defines")]
        public List<CppDefine> Defines { get; set; }

        [JsonProperty("Conditions")]
        public List<string> Conditions { get; set; }

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
        public List<string> Forwards { get; set; }

        [JsonProperty("Enums")]
        public List<CppEnum> Enums { get; set; }

        [JsonProperty("Constants")]
        public List<CppConstant> Constants { get; set; }

        [JsonProperty("Structs")]
        public List<CppStruct> Structs { get; set; }

        [JsonProperty("Variables")]
        public List<CppVariable> Variables { get; set; }

        [JsonProperty("Functions")]
        public List<CppFunction> Functions { get; set; }
    }
}
