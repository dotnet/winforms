// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace System.Windows.Forms.PrivateSourceGenerators.Tests;

public class EnumValidationTests
{
    [Fact]
    public void SequentialEnum()
    {
        string source = @"
namespace People
{
    enum Names
    {
        David,
        Igor,
        Jeremy,
        Hugh,
        Tobias,
        Olia,
        Merrie
    }

    class C
    {
        void M(Names value)
        {
            SourceGenerated.EnumValidator.Validate(value);
        }
    }
}";
        string expected =
@"if (intValue >= 0 && intValue <= 6) return;";

        VerifyGeneratedMethodLines(source, "People.Names", expected);
    }

    [Fact]
    public void MultipleCalls_OneValidateMethod()
    {
        string source = @"
namespace People
{
    enum Names
    {
        David,
        Igor,
        Jeremy,
        Hugh,
        Tobias,
        Olia,
        Merrie
    }

    class C
    {
        void M(Names value)
        {
            SourceGenerated.EnumValidator.Validate(value);
        }

        void N(Names input)
        {
            SourceGenerated.EnumValidator.Validate(input, nameof(input));
        }

        C(Names name)
        {
            SourceGenerated.EnumValidator.Validate(name, nameof(name));
            SourceGenerated.EnumValidator.Validate(name);
        }
    }
}";
        string expected =
@"if (intValue >= 0 && intValue <= 6) return;";

        VerifyGeneratedMethodLines(source, "People.Names", expected);
    }

    [Fact]
    public void DuplicateValues()
    {
        string source = @"
namespace People
{
    enum Names
    {
        David = 1,
        Igor = 2,
        Jeremy = 2,
        Hugh = 4,
        Tobias = 4
    }

    class C
    {
        void M(Names value)
        {
            SourceGenerated.EnumValidator.Validate(value);
        }
    }
}";
        string expected =
@"if (intValue >= 1 && intValue <= 2) return;
if (intValue == 4) return;";

        VerifyGeneratedMethodLines(source, "People.Names", expected);
    }

    [Fact]
    public void ValuesFromConstants()
    {
        string source = @"
namespace People
{
    static class Win32
    {
        public const int HResult = 2;
        public const int E_FAIL = 4;
    }

    enum Names
    {
        David = Win32.HResult,
        Igor = Win32.E_FAIL,
        Jeremy = Win32.HResult - 1,
        Hugh = Win32.E_FAIL + Win32.HResult,
        Tobias = Hugh
    }

    class C
    {
        void M(Names value)
        {
            SourceGenerated.EnumValidator.Validate(value);
        }
    }
}";
        string expected =
@"if (intValue >= 1 && intValue <= 2) return;
if (intValue == 4) return;
if (intValue == 6) return;";

        VerifyGeneratedMethodLines(source, "People.Names", expected);
    }

    [Fact]
    public void NonSequentialEnum()
    {
        string source = @"
namespace People
{
    enum Names
    {
        David = 1,
        Igor = 7,
        Jeremy = 6,
        Hugh = 9,
        Tobias = 2,
        Olia = 15,
        Merrie = 3
    }

    class C
    {
        void M(Names value)
        {
            SourceGenerated.EnumValidator.Validate(value);
        }
    }
}";
        string expected =
@"if (intValue >= 1 && intValue <= 3) return;
if (intValue >= 6 && intValue <= 7) return;
if (intValue == 9) return;
if (intValue == 15) return;";

        VerifyGeneratedMethodLines(source, "People.Names", expected);
    }

    [Fact]
    public void SequentialEnumWithPowersOf2()
    {
        string source = @"
namespace People
{
    enum Names
    {
        David = 1,
        Igor = 2,
        Jeremy = 4,
        Hugh = 8,
        Tobias = 16,
        Olia = 32,
        Merrie = 64
    }

    class C
    {
        void M(Names value)
        {
            SourceGenerated.EnumValidator.Validate(value);
        }
    }
}";
        string expected =
@"if (intValue >= 1 && intValue <= 2) return;
if (intValue == 4) return;
if (intValue == 8) return;
if (intValue == 16) return;
if (intValue == 32) return;
if (intValue == 64) return;";

        VerifyGeneratedMethodLines(source, "People.Names", expected);
    }

    [Fact]
    public void SequentialEnumWithPowersOf2_BinaryNotation()
    {
        string source = @"
namespace People
{
    enum Names
    {
        David =  0b0000001,
        Igor =   0b0000010,
        Jeremy = 0b0000100,
        Hugh =   0b0001000,
        Tobias = 0b0010000,
        Olia =   0b0100000,
        Merrie = 0b1000000
    }

    class C
    {
        void M(Names value)
        {
            SourceGenerated.EnumValidator.Validate(value);
        }
    }
}";
        string expected =
@"if (intValue >= 1 && intValue <= 2) return;
if (intValue == 4) return;
if (intValue == 8) return;
if (intValue == 16) return;
if (intValue == 32) return;
if (intValue == 64) return;";

        VerifyGeneratedMethodLines(source, "People.Names", expected);
    }

    [Fact]
    public void FlagsEnum()
    {
        string source = @"
namespace Paint
{
    [System.Flags]
    enum Colours
    {
        Red = 1,
        Green = 2,
        Blue = 4,
        Purple = 8
    }

    class C
    {
        void M(Colours value)
        {
            SourceGenerated.EnumValidator.Validate(value);
        }
    }
}";
        string expected =
@"if ((intValue & 15) == intValue) return;";

        VerifyGeneratedMethodLines(source, "Paint.Colours", expected);
    }

    private static void VerifyGeneratedMethodLines(string source, string expectedEnumName, string expectedBody)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        List<MetadataReference> references = [];
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            if (!assembly.IsDynamic)
            {
                references.Add(MetadataReference.CreateFromFile(assembly.Location));
            }
        }

        CSharpCompilation compilation = CSharpCompilation.Create("original", new SyntaxTree[] { syntaxTree }, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        ISourceGenerator generator = new EnumValidationGenerator().AsSourceGenerator();

        CSharpGeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);
        Assert.False(diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error), $"Failed: {diagnostics.FirstOrDefault()?.GetMessage()}");

        string output = outputCompilation.SyntaxTrees.Skip(2).First().ToString();

        List<string> lines = [.. output.Split("\r\n")];

        AssertFirstLineAndRemove(lines, "// <auto-generated />");
        AssertFirstLineAndRemove(lines, "namespace SourceGenerated");
        AssertFirstLineAndRemove(lines, "{");
        AssertFirstLineAndRemove(lines, "internal static partial class EnumValidator");
        AssertFirstLineAndRemove(lines, "{");

        AssertFirstLineAndRemove(lines, "/// <summary>Validates that the enum value passed in is valid for the enum type.</summary>");
        AssertFirstLineAndRemove(lines, $"public static void Validate({expectedEnumName} enumToValidate, string parameterName = \"value\")");
        AssertFirstLineAndRemove(lines, "{");
        AssertFirstLineAndRemove(lines, "int intValue = (int)enumToValidate;");

        foreach (string line in expectedBody.Split("\r\n"))
        {
            AssertFirstLineAndRemove(lines, line.Trim());
        }

        AssertFirstLineAndRemove(lines, $"ReportEnumValidationError(parameterName, intValue, typeof({expectedEnumName}));");

        AssertFirstLineAndRemove(lines, "}");

        static void AssertFirstLineAndRemove(List<string> lines, string expected)
        {
            Assert.True(lines.Count > 0);

            string line = lines[0].Trim();
            lines.RemoveAt(0);
            Assert.Equal(expected, line);
        }
    }
}
