// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using System.Windows.Forms.Analyzers;
using System.Windows.Forms.Analyzers.Tests;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using Xunit;
using static System.Windows.Forms.Analyzers.ApplicationConfig;

namespace System.Windows.Forms.Generators.Tests
{
    public partial class ApplicationConfigurationGeneratorTests
    {
        private const string SourceCompilable = @"
namespace MyProject
{
    class Program
    {
        static void Main()
        {
             ApplicationConfiguration.Initialize();
        }
    }
}";
        private const string SourceCompilationFailed = @"
namespace MyProject
{
    class Program
    {
        static void Main()
        {
             {|CS0103:ApplicationConfiguration|}.Initialize();
        }
    }
}";

        public static IEnumerable<object[]> UnsupportedProjectTypes_TestData()
        {
            foreach (OutputKind projectType in Enum.GetValues(typeof(OutputKind)))
            {
                if (projectType != OutputKind.ConsoleApplication &&
                    projectType != OutputKind.WindowsApplication)
                {
                    yield return new object[] { projectType };
                }
            }
        }

        [Theory]
        [MemberData(nameof(UnsupportedProjectTypes_TestData))]
        public async Task ApplicationConfigurationGenerator_GenerateInitialize_fails_if_project_type_unsupported(OutputKind projectType)
        {
            var test = new CSharpIncrementalSourceGeneratorVerifier<ApplicationConfigurationGenerator>.Test
            {
                TestState =
                {
                    OutputKind = projectType,
                    Sources = { SourceCompilationFailed },
                    ExpectedDiagnostics =
                    {
                        DiagnosticResult.CompilerError("WFAC001").WithArguments("WindowsApplication"),
                    }
                },
            };

            await test.RunAsync().ConfigureAwait(false);
        }

        [Theory]
        [InlineData(OutputKind.ConsoleApplication)]
        [InlineData(OutputKind.WindowsApplication)]
        public async Task ApplicationConfigurationGenerator_GenerateInitialize_pass_if_supported_project_type(OutputKind projectType)
        {
            SourceText generatedCode = LoadFileContent("GenerateInitialize_default_boilerplate");

            var test = new CSharpIncrementalSourceGeneratorVerifier<ApplicationConfigurationGenerator>.Test
            {
                TestState =
                {
                    OutputKind = projectType,
                    Sources = { SourceCompilable },
                    GeneratedSources =
                    {
                        (typeof(ApplicationConfigurationGenerator), "ApplicationConfiguration.g.cs", generatedCode),
                    },
                },
            };

            await test.RunAsync().ConfigureAwait(false);
        }

        [Fact]
        public async Task ApplicationConfigurationGenerator_GenerateInitialize_default_boilerplate()
        {
            SourceText generatedCode = LoadFileContent("GenerateInitialize_default_boilerplate");

            var test = new CSharpIncrementalSourceGeneratorVerifier<ApplicationConfigurationGenerator>.Test
            {
                TestState =
                {
                    OutputKind = OutputKind.WindowsApplication,
                    Sources = { SourceCompilable },
                    GeneratedSources =
                    {
                        (typeof(ApplicationConfigurationGenerator), "ApplicationConfiguration.g.cs", generatedCode),
                    },
                },
            };

            await test.RunAsync().ConfigureAwait(false);
        }

        [Fact]
        public async Task ApplicationConfigurationGenerator_GenerateInitialize_user_settings_boilerplate()
        {
            SourceText generatedCode = LoadFileContent("GenerateInitialize_user_settings_boilerplate");

            var test = new CSharpIncrementalSourceGeneratorVerifier<ApplicationConfigurationGenerator>.Test
            {
                GlobalOptions =
                {
                    ($"build_property.{PropertyNameCSharp.DefaultFont}", "Microsoft Sans Serif, 8.25px"),
                    ($"build_property.{PropertyNameCSharp.EnableVisualStyles}", ""),
                    ($"build_property.{PropertyNameCSharp.HighDpiMode}", HighDpiMode.DpiUnawareGdiScaled.ToString()),
                    ($"build_property.{PropertyNameCSharp.UseCompatibleTextRendering}", "true"),
                },
                TestState =
                {
                    OutputKind = OutputKind.WindowsApplication,
                    Sources = { SourceCompilable },
                    GeneratedSources =
                    {
                        (typeof(ApplicationConfigurationGenerator), "ApplicationConfiguration.g.cs", generatedCode),
                    },
                },
            };

            await test.RunAsync().ConfigureAwait(false);
        }

        [Fact]
        public async Task ApplicationConfigurationGenerator_GenerateInitialize_default_top_level()
        {
            const string source = @"
ApplicationConfiguration.Initialize();
";

            SourceText generatedCode = LoadFileContent("GenerateInitialize_default_top_level");

            var test = new CSharpIncrementalSourceGeneratorVerifier<ApplicationConfigurationGenerator>.Test
            {
                TestState =
                {
                    OutputKind = OutputKind.WindowsApplication,
                    Sources = { source },
                    GeneratedSources =
                    {
                        (typeof(ApplicationConfigurationGenerator), "ApplicationConfiguration.g.cs", generatedCode),
                    },
                },
            };

            await test.RunAsync().ConfigureAwait(false);
        }

        [Fact]
        public async Task ApplicationConfigurationGenerator_GenerateInitialize_user_settings_top_level()
        {
            const string source = @"
ApplicationConfiguration.Initialize();
";

            SourceText generatedCode = LoadFileContent("GenerateInitialize_user_top_level");

            var test = new CSharpIncrementalSourceGeneratorVerifier<ApplicationConfigurationGenerator>.Test
            {
                GlobalOptions =
                {
                    ($"build_property.{PropertyNameCSharp.DefaultFont}", "Microsoft Sans Serif, 8.25px"),
                    ($"build_property.{PropertyNameCSharp.EnableVisualStyles}", ""),
                    ($"build_property.{PropertyNameCSharp.HighDpiMode}", HighDpiMode.DpiUnawareGdiScaled.ToString()),
                    ($"build_property.{PropertyNameCSharp.UseCompatibleTextRendering}", "true"),
                },
                TestState =
                {
                    OutputKind = OutputKind.WindowsApplication,
                    Sources = { source },
                    GeneratedSources =
                    {
                        (typeof(ApplicationConfigurationGenerator), "ApplicationConfiguration.g.cs", generatedCode),
                    },
                },
            };

            await test.RunAsync().ConfigureAwait(false);
        }

        private SourceText LoadFileContent(string testName)
            => SourceText.From(
                    File.ReadAllText($@"System\Windows\Forms\Generators\MockData\{GetType().Name}.{testName}.cs"),
                    Encoding.UTF8);
    }
}
