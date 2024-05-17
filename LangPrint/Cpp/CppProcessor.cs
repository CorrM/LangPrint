using System;
using System.Collections.Generic;
using System.Linq;
using LangPrint.Utils;
using Newtonsoft.Json;

namespace LangPrint.Cpp;

public sealed class CppProcessor : LangProcessor<CppLangOptions>
{
    public override CppLangOptions Options { get; protected set; }

    public CppProcessor(CppLangOptions? options = null)
    {
        Options = options ?? new CppLangOptions();
    }

    private string MakeHeaderFile(CppPackage package)
    {
        var sb = new LangStringWriter(Options);

        // File header
        sb.Append(
            GetFileHeader(
                package.HeadingComment,
                package.NameSpace,
                package.Pragmas,
                package.Includes,
                package.Defines,
                package.TypeDefs,
                package.BeforeNameSpace,
                out int indentLvl
            )
        );

        // Forwards
        sb.Append(GenerateForwards(package.Forwards, indentLvl));

        // Constants
        sb.Append(GenerateConstants(package.Constants, indentLvl, package.Conditions));

        // Global fields
        sb.Append(GenerateFields(package.Fields, true, indentLvl, package.Conditions));

        // Global functions
        sb.Append(GenerateFunctions(package.Functions, null, true, indentLvl, package.Conditions));

        // Enums
        sb.Append(GenerateEnums(package.Enums, indentLvl, package.Conditions));

        // Structs
        sb.Append(GenerateStructs(package.Structs, indentLvl, package.Conditions));

        // File footer
        sb.Append(GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

        return sb.ToString();
    }

    private string MakeCppFile(CppPackage package)
    {
        var sb = new LangStringWriter(Options);

        List<CppStruct> validStructs = package.Structs
            .Where(s => ResolveConditions(package.Conditions, s.Conditions))
            .ToList();

        // File header
        List<string> includes = package.CppIncludes;

        // Don't change 'package.CppIncludes'
        if (Options.AddPackageHeaderToCppFile)
        {
            includes = includes.Append($"\"{package.Name}.h\"").ToList();
        }

        sb.Append(
            GetFileHeader(
                package.HeadingComment,
                package.NameSpace,
                null,
                includes,
                null,
                null,
                package.CppBeforeNameSpace,
                out int indentLvl
            )
        );

        // Static fields
        IEnumerable<CppField> staticVars = validStructs.SelectMany(s => s.Fields)
            .Where(v => ResolveConditions(package.Conditions, v.Conditions));

        if (staticVars.Any(v => v.Static))
        {
            var varsStr = new List<string>();
            sb.Append(GetSectionHeading("Structs Static Fields", indentLvl));

            foreach (CppStruct @struct in validStructs)
            {
                List<string> variables = @struct.Fields.Where(v => v.Static && !v.Constexpr)
                    .Select(v => GetFieldString(v, false, indentLvl, @struct))
                    .ToList();

                varsStr.AddRange(variables);
            }

            sb.Append(Helper.JoinString(Options.GetNewLineText(), varsStr));
            sb.Append(Options.GetNewLineText() + Options.GetNewLineText());
        }

        // Global fields
        sb.Append(GenerateFields(package.Fields, false, indentLvl, package.Conditions));

        // Global functions
        sb.Append(
            GenerateFunctions(
                package.Functions.Where(f => f.TemplateParams.Count == 0),
                null,
                false,
                indentLvl,
                package.Conditions
            )
        );

        // Structs functions
        if (validStructs.Any(s => s.Methods.Count > 0 && s.TemplateParams.Count == 0))
        {
            var methodsStr = new List<string>();
            sb.Append(GetSectionHeading("Structs functions", indentLvl));

            foreach (CppStruct @struct in validStructs.Where(s => s.TemplateParams.Count == 0))
            {
                int lvl = indentLvl;
                IEnumerable<string> methodsToAdd = @struct.Methods
                    .Where(
                        m => !string.IsNullOrWhiteSpace(m.Name) && ResolveConditions(package.Conditions, m.Conditions)
                    )
                    .Where(m => !m.Friend && m.TemplateParams.Count == 0)
                    .Select(structMethod => GetFunctionString(structMethod, @struct, false, lvl, package.Conditions));
                methodsStr.AddRange(methodsToAdd);
            }

            sb.Append(string.Join(Options.GetNewLineText(), methodsStr));
            sb.Append(Options.GetNewLineText());
        }

        // File footer
        sb.Append(GetFileFooter(package.NameSpace, package.CppAfterNameSpace, ref indentLvl));

        return sb.ToString();
    }

    private string MakeStructsFile(CppPackage package)
    {
        var sb = new LangStringWriter(Options);

        var pragmas = new List<string> { "once" };

        // File header
        sb.Append(
            GetFileHeader(
                package.HeadingComment,
                package.NameSpace,
                pragmas,
                null,
                null,
                null,
                package.BeforeNameSpace,
                out int indentLvl
            )
        );

        // Constants
        sb.Append(GenerateConstants(package.Constants, indentLvl, package.Conditions));

        // Enums
        sb.Append(GenerateEnums(package.Enums, indentLvl, package.Conditions));

        // Structs
        sb.Append(GenerateStructs(package.Structs.Where(s => !s.IsClass), indentLvl, package.Conditions));

        // File footer
        sb.Append(GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

        return sb.ToString();
    }

    private string MakeClassesFile(CppPackage package)
    {
        var sb = new LangStringWriter(Options);

        var pragmas = new List<string> { "once" };

        // File header
        sb.Append(
            GetFileHeader(
                package.HeadingComment,
                package.NameSpace,
                pragmas,
                null,
                null,
                null,
                package.BeforeNameSpace,
                out int indentLvl
            )
        );

        // Structs
        sb.Append(GenerateStructs(package.Structs.Where(s => s.IsClass), indentLvl, package.Conditions));

        // File footer
        sb.Append(GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

        return sb.ToString();
    }

    private string MakePackageHeaderFile(CppPackage package)
    {
        var sb = new LangStringWriter(Options);

        // File header
        sb.Append(
            GetFileHeader(
                package.HeadingComment,
                package.NameSpace,
                package.Pragmas,
                package.Includes,
                package.Defines,
                package.TypeDefs,
                package.BeforeNameSpace,
                out int indentLvl
            )
        );

        // Forwards
        sb.Append(GenerateForwards(package.Forwards, indentLvl));

        // Constants
        sb.Append(GenerateConstants(package.Constants, indentLvl, package.Conditions));

        // Global fields
        sb.Append(GenerateFields(package.Fields, true, indentLvl, package.Conditions));

        // Global functions
        sb.Append(GenerateFunctions(package.Functions, null, true, indentLvl, package.Conditions));

        // File footer
        sb.Append(GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

        sb.Append(Options.GetNewLineText());

        // Package include
        var packHeaders = new List<string>
        {
            $"\"{package.Name}_Structs.h\"",
            $"\"{package.Name}_Classes.h\"",
        };
        packHeaders.AddRange(package.PackageHeaderIncludes);
        sb.Append(GenerateIncludes(packHeaders, indentLvl));

        return sb.ToString();
    }

    private string MakePackageCppFile(CppPackage package)
    {
        var sb = new LangStringWriter(Options);

        List<CppStruct> validStructs = package.Structs
            .Where(s => ResolveConditions(package.Conditions, s.Conditions))
            .ToList();

        // File header
        List<string> includes = package.CppIncludes;

        // Don't change 'package.CppIncludes'
        if (Options.AddPackageHeaderToCppFile)
        {
            includes = includes.Append($"\"{package.Name}_Package.h\"").ToList();
        }

        sb.Append(
            GetFileHeader(
                package.HeadingComment,
                package.NameSpace,
                null,
                includes,
                null,
                null,
                package.CppBeforeNameSpace,
                out int indentLvl
            )
        );

        // Static fields
        IEnumerable<CppField> staticVars = validStructs.SelectMany(s => s.Fields)
            .Where(v => ResolveConditions(package.Conditions, v.Conditions));

        if (staticVars.Any(v => v.Static))
        {
            var varsStr = new List<string>();
            sb.Append(GetSectionHeading("Structs Static Fields", indentLvl));

            foreach (CppStruct @struct in validStructs)
            {
                List<string> variables = @struct.Fields.Where(v => ResolveConditions(package.Conditions, v.Conditions))
                    .Where(v => v.Static && !v.Constexpr)
                    .Select(v => GetFieldString(v, false, indentLvl, @struct))
                    .ToList();

                varsStr.AddRange(variables);
            }

            sb.Append(Helper.JoinString(Options.GetNewLineText(), varsStr));
            sb.Append(Options.GetNewLineText() + Options.GetNewLineText());
        }

        // Global fields
        sb.Append(GenerateFields(package.Fields, false, indentLvl, package.Conditions));

        // Global functions
        sb.Append(
            GenerateFunctions(
                package.Functions.Where(f => f.TemplateParams.Count == 0),
                null,
                false,
                indentLvl,
                package.Conditions
            )
        );

        // Structs functions
        if (validStructs.Any(s => s.Methods.Count > 0 && s.TemplateParams.Count == 0))
        {
            var methodsStr = new List<string>();
            sb.Append(GetSectionHeading("Structs Functions", indentLvl));

            foreach (CppStruct @struct in validStructs.Where(s => s.TemplateParams.Count == 0))
            {
                int lvl = indentLvl;
                IEnumerable<string> methodsToAdd = @struct.Methods
                    .Where(
                        m => !string.IsNullOrWhiteSpace(m.Name) && ResolveConditions(package.Conditions, m.Conditions)
                    )
                    .Where(m => !m.Friend && m.TemplateParams.Count == 0)
                    .Select(structMethod => GetFunctionString(structMethod, @struct, false, lvl, package.Conditions));
                methodsStr.AddRange(methodsToAdd);
            }

            sb.Append(string.Join(Options.GetNewLineText(), methodsStr));
            sb.Append(Options.GetNewLineText());
        }

        // File footer
        sb.Append(GetFileFooter(package.NameSpace, package.CppAfterNameSpace, ref indentLvl));

        return sb.ToString();
    }

    public string GetFileHeader(
        IEnumerable<string>? headingComment,
        string nameSpace,
        List<string>? pragmas,
        List<string>? includes,
        List<CppDefine>? defines,
        List<string>? typeDefs,
        string beforeNameSpace,
        out int indentLvl
    )
    {
        indentLvl = 0;
        var sb = new LangStringWriter(Options);

        // Pragmas
        if (pragmas?.Count > 0)
        {
            sb.Append(GeneratePragmas(pragmas, indentLvl));
        }

        // HeadingComment
        sb.Append(GetMultiCommentString(headingComment, indentLvl));

        // Includes
        if (includes?.Count > 0)
        {
            sb.Append(GenerateIncludes(includes, indentLvl));
        }

        // Defines
        if (defines?.Count > 0)
        {
            sb.Append(GenerateDefines(defines, indentLvl));
        }

        // TypeDefs
        if (typeDefs?.Count > 0)
        {
            sb.Append(GenerateTypeDefs(typeDefs, indentLvl));
        }

        // BeforeNameSpace
        if (!string.IsNullOrWhiteSpace(beforeNameSpace))
        {
            sb.Append(beforeNameSpace + Options.GetNewLineText());
            sb.Append(Options.GetNewLineText());
        }

        // NameSpace
        if (!string.IsNullOrWhiteSpace(nameSpace))
        {
            sb.Append(Helper.GetIndent(indentLvl));
            sb.Append($"namespace {nameSpace}{Options.GetNewLineText()}{{{Options.GetNewLineText()}");
            indentLvl++;
        }

        return sb.ToString();
    }

    public string GetFileFooter(string nameSpace, string afterNameSpace, ref int indentLvl)
    {
        var sb = new LangStringWriter(Options);

        // Close NameSpace
        if (!string.IsNullOrWhiteSpace(nameSpace))
        {
            indentLvl--;
            sb.Append($"{Helper.GetIndent(indentLvl)}}}{Options.GetNewLineText()}");
            sb.Append(Options.GetNewLineText());
        }

        // AfterNameSpace
        if (!string.IsNullOrWhiteSpace(afterNameSpace))
        {
            sb.Append(afterNameSpace);
        }

        sb.Append(Options.GetNewLineText());

        return sb.ToString();
    }

    public string GetSectionHeading(string name, int indentLvl)
    {
        string headLine = string.Concat(Enumerable.Repeat("-", 50));

        string ret = Helper.GetIndent(indentLvl) +
                     "// " +
                     headLine +
                     Options.GetNewLineText() +
                     Helper.GetIndent(indentLvl) +
                     "// " +
                     "# " +
                     name +
                     Options.GetNewLineText() +
                     Helper.GetIndent(indentLvl) +
                     "// " +
                     headLine +
                     Options.GetNewLineText();

        return new LangStringWriter(Options, ret).ToString();
    }

    public string GetMultiCommentString(IEnumerable<string>? comments, int baseIndentLvl, bool finalizeReturn = true)
    {
        if (comments is null)
        {
            return string.Empty;
        }

        IEnumerable<string> eComments = comments.ToList();
        if (!eComments.Any())
        {
            return string.Empty;
        }

        string ret = $"{Helper.GetIndent(baseIndentLvl)}/**{Options.GetNewLineText()}" +
                     Helper.JoinString(Options.GetNewLineText(), eComments, $"{Helper.GetIndent(baseIndentLvl)} * ") +
                     $"{Options.GetNewLineText()}{Helper.GetIndent(baseIndentLvl)} */";

        ret = finalizeReturn ? Helper.FinalizeSection(ret, Options.GetNewLineText()) : ret + Options.GetNewLineText();

        return new LangStringWriter(Options, ret).ToString();
    }

    public string GetParamString(CppParameter parameter)
    {
        var sb = new LangStringWriter(Options);
        sb.Append(GetBeforePrint(parameter, 0));

        sb.Append($"{parameter.Type} {parameter.Name}");

        sb.Append(GetAfterPrint(parameter, 0));
        return sb.ToString();
    }

    public string GetFieldString(CppField field, bool declaration, int baseIndentLvl, CppStruct? parent = null)
    {
        var sb = new LangStringWriter(Options);

        sb.Append(GetBeforePrint(field, baseIndentLvl));

        // Comment
        sb.Append(GetMultiCommentString(field.Comments, baseIndentLvl, false));
        sb.Append(Helper.GetIndent(baseIndentLvl));

        var decSb = new LangStringWriter(Options);

        // Extern
        if (declaration && field.Extern)
        {
            decSb.Append("extern ");
        }

        // Static
        if (declaration && field.Static)
        {
            decSb.Append("static ");
        }

        // Friend
        if (declaration && field.Friend)
        {
            decSb.Append("friend ");
        }

        // Const
        if (field.Const)
        {
            decSb.Append("const ");
        }

        // Constexpr
        if (declaration && field.Constexpr)
        {
            decSb.Append("constexpr ");
        }

        // Type
        decSb.Append(field.Type);
        sb.Append($"{decSb.ToString().PadRight(Options.VariableMemberTypePadSize)} ");

        var nameSb = new LangStringWriter(Options);

        // Parent name
        if (!declaration && parent is not null)
        {
            nameSb.Append($"{parent.Name}::");
        }

        // Name
        nameSb.Append(field.Name);

        // ArrayDim
        if (!string.IsNullOrWhiteSpace(field.ArrayDim))
        {
            nameSb.Append($"[{field.ArrayDim}]");
        }

        // Bitfield
        else if (!string.IsNullOrWhiteSpace(field.Bitfield))
        {
            nameSb.Append($" : {field.Bitfield}");
        }

        // Value
        if ((!string.IsNullOrWhiteSpace(field.Value) && !declaration) || (declaration && field.Constexpr))
        {
            nameSb.Append($" = {field.Value}");
        }

        nameSb.Append(';');

        // Inline comment
        if (!string.IsNullOrEmpty(field.InlineComment))
        {
            sb.Append(nameSb.ToString().PadRight(Options.InlineCommentPadSize) + $" // {field.InlineComment}");
        }
        else
        {
            sb.Append(nameSb);
        }

        sb.Append(GetAfterPrint(field, baseIndentLvl));

        return sb.ToString();
    }

    public string GetEnumString(CppEnum @enum, int baseIndentLvl)
    {
        var sb = new LangStringWriter(Options);
        sb.Append(GetBeforePrint(@enum, baseIndentLvl));

        // Comment
        sb.Append(GetMultiCommentString(@enum.Comments, baseIndentLvl, false));

        // Name
        sb.Append(
            @enum.IsClass
                ? $"{Helper.GetIndent(baseIndentLvl)}enum class {@enum.Name}"
                : $"{Helper.GetIndent(baseIndentLvl)}enum {@enum.Name}"
        );

        // Type
        if (!string.IsNullOrWhiteSpace(@enum.Type))
        {
            sb.Append($" : {@enum.Type}");
        }

        // Inline comment
        if (!string.IsNullOrEmpty(@enum.InlineComment))
        {
            sb.Append($" // {@enum.InlineComment}");
        }

        sb.Append(Options.GetNewLineText());

        // Open
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}{{{Options.GetNewLineText()}");
        baseIndentLvl++;

        // Values
        if (@enum.Values.Count > 0)
        {
            int biggestName = @enum.Values.Max(ev => ev.Name.Length);
            IEnumerable<string> vals = @enum.Values.Select(
                ev =>
                {
                    string value = @enum.HexValues && long.TryParse(ev.Value, out long iValue) && iValue >= 0
                        ? $"0x{iValue:X16}"
                        : ev.Value;

                    return $"{ev.Name.PadRight(biggestName)} = {value}";
                }
            );

            string values = Helper.JoinString("," + Options.GetNewLineText(), vals, Helper.GetIndent(baseIndentLvl));

            sb.Append(values);
        }

        sb.Append(Options.GetNewLineText());

        // Close
        baseIndentLvl--;
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}}};{Options.GetNewLineText()}");

        sb.Append(GetAfterPrint(@enum, baseIndentLvl));
        return sb.ToString();
    }

    public string GetFunctionString(
        CppFunction func,
        CppStruct? parent,
        bool declaration,
        int baseIndentLvl,
        List<string>? modelConditions
    )
    {
        bool forceAddBody = declaration;
        forceAddBody &= (parent is not null && parent.TemplateParams.Count > 0) ||
                        func.TemplateParams.Count > 0 ||
                        func.Friend;

        var sb = new LangStringWriter(Options);
        sb.Append(GetBeforePrint(func, baseIndentLvl));

        // Comment
        if (!declaration)
        {
            sb.Append(GetMultiCommentString(func.Comments, baseIndentLvl, false));
        }

        // Template
        if (func.TemplateParams.Count > 0)
        {
            sb.Append(Helper.GetIndent(baseIndentLvl));
            sb.Append($"template<{string.Join(", ", func.TemplateParams)}>");
            sb.Append(Options.GetNewLineText());
        }

        sb.Append(Helper.GetIndent(baseIndentLvl));

        // Static
        if (declaration && func.Static)
        {
            sb.Append("static ");
        }

        // Friend
        if (declaration && func.Friend)
        {
            sb.Append("friend ");
        }

        // Inline
        if (func.Inline && declaration)
        {
            sb.Append("inline ");
        }

        // Type
        if (!string.IsNullOrWhiteSpace(func.Type))
        {
            sb.Append($"{func.Type} ");
        }

        // Name
        if (parent is not null &&
            parent.TemplateParams.Count == 0 &&
            !declaration &&
            func.TemplateParams.Count == 0 &&
            !func.Friend)
        {
            sb.Append($"{parent.Name}::");
        }

        sb.Append(func.Name);

        // Params
        sb.Append('(');
        sb.Append(
            string.Join(
                ", ",
                func.Params.Where(p => modelConditions is null || ResolveConditions(modelConditions, p.Conditions))
                    .Select(GetParamString)
            )
        );
        sb.Append(')');

        // Const
        if (func.Const)
        {
            sb.Append(" const");
        }

        if (declaration && !forceAddBody)
        {
            sb.Append(';');
            sb.Append(GetAfterPrint(func, baseIndentLvl));
            sb.Append(Options.GetNewLineText());
            return sb.ToString();
        }

        // Inline comment
        if (!string.IsNullOrEmpty(func.InlineComment))
        {
            sb.Append($" // {func.InlineComment}");
        }

        sb.Append(Options.GetNewLineText());

        // Body
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}{{{Options.GetNewLineText()}");
        baseIndentLvl++;

        sb.Append(Helper.JoinString(Options.GetNewLineText(), func.Body, Helper.GetIndent(baseIndentLvl)));

        baseIndentLvl--;
        sb.Append($"{Options.GetNewLineText()}{Helper.GetIndent(baseIndentLvl)}}}{Options.GetNewLineText()}");

        sb.Append(GetAfterPrint(func, baseIndentLvl));

        return sb.ToString();
    }

    public string GetStructString(CppStruct @struct, int baseIndentLvl, List<string>? conditions)
    {
        var sb = new LangStringWriter(Options);
        sb.Append(GetBeforePrint(@struct, baseIndentLvl));

        // Comment
        sb.Append(GetMultiCommentString(@struct.Comments, baseIndentLvl, false));

        // Template
        if (@struct.TemplateParams.Count > 0)
        {
            sb.Append(Helper.GetIndent(baseIndentLvl));
            sb.Append($"template<{string.Join(", ", @struct.TemplateParams)}>{Options.GetNewLineText()}");
        }

        // Kind
        sb.Append(Helper.GetIndent(baseIndentLvl));
        if (@struct.IsClass)
        {
            sb.Append("class ");
        }
        else if (@struct.IsUnion)
        {
            sb.Append("union ");
        }
        else
        {
            sb.Append("struct ");
        }

        // Aligns
        if (@struct.Alignas > 0)
        {
            sb.Append($"__declspec(align({@struct.Alignas})) ");
        }

        // Name
        sb.Append(@struct.Name);

        // Supers
        if (@struct.Supers.Count > 0)
        {
            sb.Append($" : {Helper.JoinString(", ", @struct.Supers, "public ")}");
        }

        // Inline comment
        if (!string.IsNullOrEmpty(@struct.InlineComment))
        {
            sb.Append($" // {@struct.InlineComment}");
        }

        sb.Append(Options.GetNewLineText());

        // Open struct scope
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}{{");
        baseIndentLvl++;

        bool isEmpty = @struct.Fields.Count == 0 && @struct.Friends.Count == 0 && @struct.Methods.Count == 0;
        if (isEmpty)
        {
            sb.Append(Options.GetNewLineText());
        }

        // Fields
        if (@struct.Fields.Count > 0)
        {
            sb.Append(Options.GetNewLineText());

            bool lastVarIsPrivate = false;
            bool lastVarIsUnion = false;
            List<CppField> variables = @struct.Fields.Where(
                    v => !string.IsNullOrWhiteSpace(v.Name) &&
                         (conditions is null || ResolveConditions(conditions, v.Conditions))
                )
                .ToList();

            // Force write "private" or "public"
            if (variables.Count > 0)
            {
                lastVarIsPrivate = !variables.First().Private;
            }

            foreach (CppField structVar in variables)
            {
                // Private or Public
                if (structVar.Private != lastVarIsPrivate)
                {
                    lastVarIsPrivate = structVar.Private;
                    sb.Append($"{Helper.GetIndent(baseIndentLvl - 1)}");
                    sb.Append(structVar.Private ? "private:" : "public:");
                    sb.Append(Options.GetNewLineText());
                }

                // Close union
                if ((structVar.ForceUnion && lastVarIsUnion) || (!structVar.Union && lastVarIsUnion))
                {
                    lastVarIsUnion = false;
                    baseIndentLvl--;
                    sb.Append($"{Helper.GetIndent(baseIndentLvl)}}};{Options.GetNewLineText()}");
                }

                // Open union
                if (structVar.Union && !lastVarIsUnion)
                {
                    lastVarIsUnion = true;
                    sb.Append($"{Helper.GetIndent(baseIndentLvl)}union{Options.GetNewLineText()}");
                    sb.Append($"{Helper.GetIndent(baseIndentLvl)}{{{Options.GetNewLineText()}");
                    baseIndentLvl++;
                }

                // Print field
                sb.Append(GetFieldString(structVar, true, baseIndentLvl));
                sb.Append(Options.GetNewLineText());
            }

            // Close union
            if (lastVarIsUnion)
            {
                baseIndentLvl--;
                sb.Append($"{Helper.GetIndent(baseIndentLvl)}}};{Options.GetNewLineText()}");
            }
        }

        // Friends
        if (@struct.Friends.Count > 0)
        {
            sb.Append(Options.GetNewLineText());

            sb.Append(
                Helper.JoinString(
                    Options.GetNewLineText(),
                    @struct.Friends,
                    $"{Helper.GetIndent(baseIndentLvl)}friend ",
                    ";"
                )
            );
            sb.Append(Options.GetNewLineText());
        }

        // Methods
        if (@struct.Methods.Count > 0)
        {
            sb.Append(Options.GetNewLineText());

            bool lastMethodIsPrivate = false;
            List<CppFunction> methods = @struct.Methods.Where(
                    m => !string.IsNullOrWhiteSpace(m.Name) &&
                         (conditions is null || ResolveConditions(conditions, m.Conditions))
                )
                .ToList();

            // Force write "private" or "public"
            if (methods.Count > 0)
            {
                lastMethodIsPrivate = !methods.First().Private;
            }

            foreach (CppFunction structMethod in methods)
            {
                // Private or Public
                if (structMethod.Private != lastMethodIsPrivate)
                {
                    lastMethodIsPrivate = structMethod.Private;
                    sb.Append($"{Helper.GetIndent(baseIndentLvl - 1)}");
                    sb.Append(structMethod.Private ? "private:" : "public:");
                    sb.Append(Options.GetNewLineText());
                }

                // Print method
                sb.Append(GetFunctionString(structMethod, @struct, true, baseIndentLvl, conditions));
            }
        }

        // Close struct scope
        baseIndentLvl--;
        sb.Append($"{Helper.GetIndent(baseIndentLvl)}}};{Options.GetNewLineText()}");

        sb.Append(GetAfterPrint(@struct, baseIndentLvl));
        return sb.ToString();
    }

    public string GeneratePragmas(IEnumerable<string> pragmas, int baseIndentLvl)
    {
        string ret = Helper.JoinString(Options.GetNewLineText(), pragmas, $"{Helper.GetIndent(baseIndentLvl)}#pragma ");
        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateIncludes(IEnumerable<string> includes, int baseIndentLvl)
    {
        string ret = Helper.JoinString(
            Options.GetNewLineText(),
            includes,
            $"{Helper.GetIndent(baseIndentLvl)}#include "
        );
        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateDefines(IEnumerable<CppDefine> defines, int baseIndentLvl)
    {
        string ret = Helper.JoinString(
            Options.GetNewLineText(),
            defines.Select(d => d.Name + (string.IsNullOrWhiteSpace(d.Value) ? string.Empty : $" {d.Value}")),
            $"{Helper.GetIndent(baseIndentLvl)}#define "
        );

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateTypeDefs(IEnumerable<string> typeDefs, int baseIndentLvl)
    {
        string ret = Helper.JoinString(
            Options.GetNewLineText(),
            typeDefs,
            $"{Helper.GetIndent(baseIndentLvl)}typedef ",
            ";"
        );

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateForwards(IEnumerable<string> forwards, int baseIndentLvl)
    {
        IEnumerable<string> forwardsList = forwards.ToList();
        if (!forwardsList.Any())
        {
            return string.Empty;
        }

        string ret = Helper.JoinString(Options.GetNewLineText(), forwardsList, Helper.GetIndent(baseIndentLvl), ";");
        ret += Options.GetNewLineText();

        if (Options.PrintSectionName)
        {
            ret = GetSectionHeading("Forwards", baseIndentLvl) + ret;
        }

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateFields(
        IEnumerable<CppField> fields,
        bool declarationOnly,
        int baseIndentLvl,
        List<string> conditions
    )
    {
        List<CppField> vars = fields.Where(
                v => !string.IsNullOrWhiteSpace(v.Name) &&
                     !string.IsNullOrWhiteSpace(v.Type) &&
                     ResolveConditions(conditions, v.Conditions)
            )
            .ToList();

        if (vars.Count == 0)
        {
            return string.Empty;
        }

        string ret = string.Join(
            Options.GetNewLineText(),
            vars.Select(v => GetFieldString(v, declarationOnly, baseIndentLvl))
        );

        if (Options.PrintSectionName)
        {
            ret = GetSectionHeading("Global fields", baseIndentLvl) + ret;
        }

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateFunctions(
        IEnumerable<CppFunction> functions,
        CppStruct? parent,
        bool declarationOnly,
        int baseIndentLvl,
        List<string> conditions
    )
    {
        List<CppFunction> funcs = functions.Where(
                f => !string.IsNullOrWhiteSpace(f.Name) &&
                     !string.IsNullOrWhiteSpace(f.Type) &&
                     ResolveConditions(conditions, f.Conditions)
            )
            .ToList();

        if (funcs.Count == 0)
        {
            return string.Empty;
        }

        string ret = string.Join(
            Options.GetNewLineText(),
            funcs.Select(f => GetFunctionString(f, parent, declarationOnly, baseIndentLvl, conditions))
        );

        if (Options.PrintSectionName)
        {
            ret = GetSectionHeading("Global functions", baseIndentLvl) + ret;
        }

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateEnums(IEnumerable<CppEnum> enums, int baseIndentLvl, List<string> conditions)
    {
        List<CppEnum> vEnums = enums
            .Where(e => !string.IsNullOrWhiteSpace(e.Name) && ResolveConditions(conditions, e.Conditions))
            .ToList();

        if (vEnums.Count == 0)
        {
            return string.Empty;
        }

        string ret = string.Join(Options.GetNewLineText(), vEnums.Select(e => GetEnumString(e, baseIndentLvl)));

        if (Options.PrintSectionName)
        {
            ret = GetSectionHeading("Enums", baseIndentLvl) + ret;
        }

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateConstants(IEnumerable<CppConstant> constants, int baseIndentLvl, List<string> conditions)
    {
        List<string> values = constants
            .Where(
                c => !string.IsNullOrWhiteSpace(c.Name) &&
                     !string.IsNullOrWhiteSpace(c.Type) &&
                     !string.IsNullOrWhiteSpace(c.Value) &&
                     ResolveConditions(conditions, c.Conditions)
            )
            .Select(c => $"static constexpr {c.Type} {c.Name} = {c.Value}")
            .ToList();

        if (values.Count == 0)
        {
            return string.Empty;
        }

        string ret = Helper.JoinString(Options.GetNewLineText(), values, Helper.GetIndent(baseIndentLvl), ";");

        if (Options.PrintSectionName && values.Count > 0)
        {
            ret = GetSectionHeading("Constants", baseIndentLvl) + ret;
        }

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public string GenerateStructs(IEnumerable<CppStruct> structs, int baseIndentLvl, List<string>? conditions)
    {
        List<CppStruct> vStruct = structs.Where(
                s => !string.IsNullOrWhiteSpace(s.Name) &&
                     (conditions is null || ResolveConditions(conditions, s.Conditions))
            )
            .ToList();

        if (vStruct.Count == 0)
        {
            return string.Empty;
        }

        string ret = string.Join(
            Options.GetNewLineText(),
            vStruct.Select(s => GetStructString(s, baseIndentLvl, conditions))
        );

        if (Options.PrintSectionName)
        {
            ret = GetSectionHeading(vStruct.All(s => s.IsClass) ? "Classes" : "Structs", baseIndentLvl) + ret;
        }

        return Helper.FinalizeSection(ret, Options.GetNewLineText());
    }

    public CppPackage? ModelFromJson(string jsonData)
    {
        return JsonConvert.DeserializeObject<CppPackage>(jsonData);
    }

    public Dictionary<string, string> GenerateFiles(CppPackage cppPackage)
    {
        cppPackage.Conditions = cppPackage.Conditions.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

        var ret = new Dictionary<string, string>(StringComparer.Ordinal);

        if (!Options.GeneratePackageSyntax)
        {
            ret.Add($"{cppPackage.Name}.h", MakeHeaderFile(cppPackage));
            ret.Add($"{cppPackage.Name}.cpp", MakeCppFile(cppPackage));

            return ret;
        }

        ret.Add($"{cppPackage.Name}_Structs.h", MakeStructsFile(cppPackage));
        ret.Add($"{cppPackage.Name}_Classes.h", MakeClassesFile(cppPackage));
        ret.Add($"{cppPackage.Name}_Package.h", MakePackageHeaderFile(cppPackage));
        ret.Add($"{cppPackage.Name}_Package.cpp", MakePackageCppFile(cppPackage));

        return ret;
    }
}
