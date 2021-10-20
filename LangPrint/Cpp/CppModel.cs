using System.Collections.Generic;
using Newtonsoft.Json;

/*
 * ToDo:
 * - Add comments
 * - Add string above and before class/struct/enums
 * - Add Virtual functions to class/struct
 */

namespace LangPrint.Cpp
{
    public class CppDefine
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Value")]
        public string Value { get; set; }
    }

    public class CppConstant
    {
        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Value")]
        public string Value { get; set; }
    }

    public class CppValue
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Value")]
        public string Value { get; set; }
    }

    public class CppEnum
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("IsClass")]
        public bool IsClass { get; set; }

        [JsonProperty("Values")]
        public List<CppValue> Values { get; set; }

        [JsonProperty("Condition")]
        public List<string> Condition { get; set; }
    }

    public class CppParam
    {
        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }
    }

    public class CppFunction
    {
        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Params")]
        public List<CppParam> Params { get; set; }

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

        [JsonProperty("Condition")]
        public List<string> Condition { get; set; }
    }

    public class CppVar
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

        [JsonProperty("Static")]
        public bool Static { get; set; }

        [JsonProperty("Const")]
        public bool Const { get; set; }

        [JsonProperty("Constexpr")]
        public bool Constexpr { get; set; }

        [JsonProperty("Union")]
        public bool Union { get; set; }

        [JsonProperty("Condition")]
        public List<string> Condition { get; set; }
    }

    public class CppMethod
    {
        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Params")]
        public List<CppParam> Params { get; set; }

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

        [JsonProperty("Condition")]
        public List<string> Condition { get; set; }
    }

    public class CppStruct
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Supers")]
        public List<string> Supers { get; set; }

        [JsonProperty("TemplateParams")]
        public List<string> TemplateParams { get; set; }

        [JsonProperty("Friends")]
        public List<string> Friends { get; set; }

        [JsonProperty("Vars")]
        public List<CppVar> Vars { get; set; }

        [JsonProperty("Methods")]
        public List<CppMethod> Methods { get; set; }

        [JsonProperty("Condition")]
        public List<string> Condition { get; set; }
    }

    public class CppClass : CppStruct
    {
    }

    public class CppModel : ILanguage
    {
        [JsonProperty("Lang")]
        public string Lang { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Pragmas")]
        public List<string> Pragmas { get; set; }

        [JsonProperty("HeadingComment")]
        public List<string> HeadingComment { get; set; }

        [JsonProperty("Includes")]
        public List<string> Includes { get; set; }

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

        [JsonProperty("ClassForwards")]
        public List<string> ClassForwards { get; set; }

        [JsonProperty("Constants")]
        public List<CppConstant> Constants { get; set; }

        [JsonProperty("Enums")]
        public List<CppEnum> Enums { get; set; }

        [JsonProperty("Functions")]
        public List<CppFunction> Functions { get; set; }

        [JsonProperty("Structs")]
        public List<CppStruct> Structs { get; set; }

        [JsonProperty("Classes")]
        public List<CppClass> Classes { get; set; }
    }
}
