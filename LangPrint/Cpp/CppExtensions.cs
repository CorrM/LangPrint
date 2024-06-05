namespace LangPrint.Cpp;

public static class CppExtensions
{
    /*
    public static string MakeHeaderFile(this CppProcessor processor, CppPackage package)
    {
        var sb = new LangStringWriter(processor.Options);

        // File header
        sb.Append(
            processor.GetFileHeader(
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
        sb.Append(processor.GenerateForwards(package.Forwards, indentLvl));

        // Constants
        sb.Append(processor.GenerateConstants(package.Constants, indentLvl));

        // Global fields
        sb.Append(processor.GenerateFields(package.Fields, declarationOnly: true, indentLvl));

        // Global functions
        sb.Append(processor.GenerateFunctions(package.Functions, parent: null, declarationOnly: true, indentLvl));

        // Enums
        sb.Append(processor.GenerateEnums(package.Enums, indentLvl));

        // Structs
        sb.Append(processor.GenerateStructs(package.Structs, indentLvl));

        // File footer
        sb.Append(processor.GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

        return sb.ToString();
    }

    public static string MakeCppFile(this CppProcessor processor, CppPackage package)
    {
        var sb = new LangStringWriter(processor.Options);

        // File header
        List<string> includes = package.CppIncludes;

        // Don't change 'package.CppIncludes'
        if (processor.Options.AddPackageHeaderToCppFile)
        {
            includes = includes.Append($"\"{package.Name}.h\"").ToList();
        }

        sb.Append(
            processor.GetFileHeader(
                package.HeadingComment,
                package.NameSpace,
                pragmas: null,
                includes,
                defines: null,
                typeDefs: null,
                package.CppBeforeNameSpace,
                out int indentLvl
            )
        );

        // Static fields
        IEnumerable<CppField> staticVars = package.Structs.SelectMany(s => s.Fields);

        if (staticVars.Any(v => v.IsStatic))
        {
            var varsStr = new List<string>();
            sb.Append(processor.GetSectionHeading("Structs Static Fields", indentLvl));

            foreach (CppStruct @struct in package.Structs)
            {
                List<string> variables = @struct.Fields.Where(v => v.IsStatic && !v.IsConstexpr)
                    .Select(v => processor.GetFieldString(v, false, indentLvl, @struct))
                    .ToList();

                varsStr.AddRange(variables);
            }

            sb.Append(Helper.JoinString(processor.Options.GetNewLineText(), varsStr));
            sb.Append(processor.Options.GetNewLineText() + processor.Options.GetNewLineText());
        }

        // Global fields
        sb.Append(processor.GenerateFields(package.Fields, declarationOnly: false, indentLvl));

        // Global functions
        sb.Append(
            processor.GenerateFunctions(
                package.Functions.Where(f => f.TemplateParameters.Count == 0),
                parent: null,
                declarationOnly: false,
                indentLvl
            )
        );

        // Structs functions
        if (package.Structs.Exists(s => s.Methods.Count > 0 && s.TemplateParams.Count == 0))
        {
            var methodsStr = new List<string>();
            sb.Append(processor.GetSectionHeading("Structs functions", indentLvl));

            foreach (CppStruct @struct in package.Structs.Where(s => s.TemplateParams.Count == 0))
            {
                int lvl = indentLvl;
                IEnumerable<string> methodsToAdd = @struct.Methods
                    .Where(
                        m => !string.IsNullOrWhiteSpace(m.Name) && m is { IsFriend: false, TemplateParameters.Count: 0 }
                    )
                    .Select(
                        structMethod => processor.GetFunctionString(structMethod, @struct, declaration: false, lvl)
                    );
                methodsStr.AddRange(methodsToAdd);
            }

            sb.Append(string.Join(processor.Options.GetNewLineText(), methodsStr));
            sb.Append(processor.Options.GetNewLineText());
        }

        // File footer
        sb.Append(processor.GetFileFooter(package.NameSpace, package.CppAfterNameSpace, ref indentLvl));

        return sb.ToString();
    }

    public static string MakeStructsFile(this CppProcessor processor, CppPackage package)
    {
        var sb = new LangStringWriter(processor.Options);

        var pragmas = new List<string> { "once" };

        // File header
        sb.Append(
            processor.GetFileHeader(
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
        sb.Append(processor.GenerateConstants(package.Constants, indentLvl));

        // Enums
        sb.Append(processor.GenerateEnums(package.Enums, indentLvl));

        // Structs
        sb.Append(processor.GenerateStructs(package.Structs.Where(s => !s.IsClass), indentLvl));

        // File footer
        sb.Append(processor.GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

        return sb.ToString();
    }

    public static string MakeClassesFile(this CppProcessor processor, CppPackage package)
    {
        var sb = new LangStringWriter(processor.Options);

        var pragmas = new List<string> { "once" };

        // File header
        sb.Append(
            processor.GetFileHeader(
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
        sb.Append(processor.GenerateStructs(package.Structs.Where(s => s.IsClass), indentLvl));

        // File footer
        sb.Append(processor.GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

        return sb.ToString();
    }

    public static string MakePackageHeaderFile(this CppProcessor processor, CppPackage package)
    {
        var sb = new LangStringWriter(processor.Options);

        // File header
        sb.Append(
            processor.GetFileHeader(
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
        sb.Append(processor.GenerateForwards(package.Forwards, indentLvl));

        // Constants
        sb.Append(processor.GenerateConstants(package.Constants, indentLvl));

        // Global fields
        sb.Append(processor.GenerateFields(package.Fields, declarationOnly: true, indentLvl));

        // Global functions
        sb.Append(processor.GenerateFunctions(package.Functions, parent: null, declarationOnly: true, indentLvl));

        // File footer
        sb.Append(processor.GetFileFooter(package.NameSpace, package.AfterNameSpace, ref indentLvl));

        sb.Append(processor.Options.GetNewLineText());

        // Package include
        var packHeaders = new List<string>
        {
            $"\"{package.Name}_Structs.h\"",
            $"\"{package.Name}_Classes.h\"",
        };
        packHeaders.AddRange(package.PackageHeaderIncludes);
        sb.Append(processor.GenerateIncludes(packHeaders, indentLvl));

        return sb.ToString();
    }

    public static string MakePackageCppFile(this CppProcessor processor, CppPackage package)
    {
        var sb = new LangStringWriter(processor.Options);

        // File header
        List<string> includes = package.CppIncludes;

        // Don't change 'package.CppIncludes'
        if (processor.Options.AddPackageHeaderToCppFile)
        {
            includes = includes.Append($"\"{package.Name}_Package.h\"").ToList();
        }

        sb.Append(
            processor.GetFileHeader(
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
        IEnumerable<CppField> staticVars = package.Structs.SelectMany(s => s.Fields);

        if (staticVars.Any(v => v.IsStatic))
        {
            var varsStr = new List<string>();
            sb.Append(processor.GetSectionHeading("Structs Static Fields", indentLvl));

            foreach (CppStruct @struct in package.Structs)
            {
                List<string> variables = @struct.Fields.Where(v => v is { IsStatic: true, IsConstexpr: false })
                    .Select(v => processor.GetFieldString(v, declaration: false, indentLvl, @struct))
                    .ToList();

                varsStr.AddRange(variables);
            }

            sb.Append(Helper.JoinString(processor.Options.GetNewLineText(), varsStr));
            sb.Append(processor.Options.GetNewLineText() + processor.Options.GetNewLineText());
        }

        // Global fields
        sb.Append(processor.GenerateFields(package.Fields, declarationOnly: false, indentLvl));

        // Global functions
        sb.Append(
            processor.GenerateFunctions(
                package.Functions.Where(f => f.TemplateParameters.Count == 0),
                parent: null,
                declarationOnly: false,
                indentLvl
            )
        );

        // Structs functions
        if (package.Structs.Exists(s => s.Methods.Count > 0 && s.TemplateParams.Count == 0))
        {
            var methodsStr = new List<string>();
            sb.Append(processor.GetSectionHeading("Structs Functions", indentLvl));

            foreach (CppStruct @struct in package.Structs.Where(s => s.TemplateParams.Count == 0))
            {
                int lvl = indentLvl;
                IEnumerable<string> methodsToAdd = @struct.Methods
                    .Where(
                        m => !string.IsNullOrWhiteSpace(m.Name) && m is { IsFriend: false, TemplateParameters.Count: 0 }
                    )
                    .Select(
                        structMethod => processor.GetFunctionString(structMethod, @struct, declaration: false, lvl)
                    );
                methodsStr.AddRange(methodsToAdd);
            }

            sb.Append(string.Join(processor.Options.GetNewLineText(), methodsStr));
            sb.Append(processor.Options.GetNewLineText());
        }

        // File footer
        sb.Append(processor.GetFileFooter(package.NameSpace, package.CppAfterNameSpace, ref indentLvl));

        return sb.ToString();
    }

    public static Dictionary<string, string> GenerateFiles(
        this CppProcessor processor,
        CppPackage cppPackage,
        bool generatePackageSyntax
    )
    {
        var ret = new Dictionary<string, string>(StringComparer.Ordinal);

        if (!generatePackageSyntax)
        {
            ret.Add($"{cppPackage.Name}.h", processor.MakeHeaderFile(cppPackage));
            ret.Add($"{cppPackage.Name}.cpp", processor.MakeCppFile(cppPackage));

            return ret;
        }

        ret.Add($"{cppPackage.Name}_Structs.h", processor.MakeStructsFile(cppPackage));
        ret.Add($"{cppPackage.Name}_Classes.h", processor.MakeClassesFile(cppPackage));
        ret.Add($"{cppPackage.Name}_Package.h", processor.MakePackageHeaderFile(cppPackage));
        ret.Add($"{cppPackage.Name}_Package.cpp", processor.MakePackageCppFile(cppPackage));

        return ret;
    }
    */
}
