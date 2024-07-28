// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Testing;
using Xunit;

namespace System.Windows.Forms.Analyzers.Tests;

public class AppManifestAnalyzerTests
{
    private const string CSharpCode =
        """
        namespace ConsoleApplication1
        {
            class {|#0:TypeName|}
            {   
            }
        }
        """;

    private const string VbCode =
        """
        Namespace ConsoleApplication1
            Class {|#0:TypeName|}
            End Class
        End Namespace
        """;

    [Fact]
    public async Task AppManifestAnalyzer_no_op_if_no_manifest_file() =>
        await new CSharpAnalyzerTest<AppManifestAnalyzer, DefaultVerifier>()
        {
            TestCode = CSharpCode,
            TestState =
            {
                AdditionalFiles = { }
            }
        }.RunAsync();

    [Fact]
    public async Task AppManifestAnalyzer_no_op_if_manifest_file_has_no_dpi_info()
    {
        SourceText manifestFile = SourceText.From(
            File.ReadAllText(@"System\Windows\Forms\Analyzers\MockData\nodpi.manifest"));

        await new CSharpAnalyzerTest<AppManifestAnalyzer, DefaultVerifier>()
        {
            TestCode = CSharpCode,
            TestState =
            {
                 AdditionalFiles = { (@"C:\temp\app.manifest", manifestFile) }
            }
        }.RunAsync();
    }

    [Fact]
    public async Task AppManifestAnalyzer_noop_if_manifest_file_corrupt()
    {
        SourceText manifestFile = SourceText.From(
            File.ReadAllText(@"System\Windows\Forms\Analyzers\MockData\invalid.manifest"));

        await new CSharpAnalyzerTest<AppManifestAnalyzer, DefaultVerifier>()
        {
            TestCode = CSharpCode,
            TestState =
            {
                 AdditionalFiles = { (@"C:\temp\app.manifest", manifestFile) }
            }
        }.RunAsync();
    }

    [Fact]
    public async Task AppManifestAnalyzer_warn_if_manifest_file_has_dpi_info_CSharp()
    {
        const string manifestFilePath = @"C:\temp\app.manifest";

        SourceText manifestFile = SourceText.From(
            File.ReadAllText(@"System\Windows\Forms\Analyzers\MockData\dpi.manifest"));

        await new CSharpAnalyzerTest<AppManifestAnalyzer, DefaultVerifier>()
        {
            TestCode = CSharpCode,
            TestState =
            {
                 AdditionalFiles = { (manifestFilePath, manifestFile) }
            },
            ExpectedDiagnostics =
            {
                new DiagnosticResult(SharedDiagnosticDescriptors.s_cSharpMigrateHighDpiSettings)
                    .WithArguments(manifestFilePath, ApplicationConfig.PropertyNameCSharp.HighDpiMode)
            }
        }.RunAsync();
    }

    [Fact]
    public async Task AppManifestAnalyzer_warn_if_manifest_file_has_dpi_info_VB()
    {
        const string manifestFilePath = @"C:\temp\app.manifest";

        SourceText manifestFile = SourceText.From(
            File.ReadAllText(@"System\Windows\Forms\Analyzers\MockData\dpi.manifest"));

        await new VisualBasicAnalyzerTest<AppManifestAnalyzer, DefaultVerifier>()
        {
            TestCode = VbCode,
            TestState =
            {
                 AdditionalFiles = { (manifestFilePath, manifestFile) }
            },
            ExpectedDiagnostics =
            {
                new DiagnosticResult(SharedDiagnosticDescriptors.s_visualBasicMigrateHighDpiSettings)
                    .WithArguments(manifestFilePath, ApplicationConfig.PropertyNameVisualBasic.HighDpiMode)
            }
        }.RunAsync();
    }

    [Fact]
    public async Task AppManifestAnalyzer_can_suppressed_if_manifest_file_has_dpi_info_CSharp()
    {
        const string manifestFilePath = @"C:\temp\app.manifest";

        SourceText manifestFile = SourceText.From(
            File.ReadAllText(@"System\Windows\Forms\Analyzers\MockData\dpi.manifest"));

        await new CSharpAnalyzerTest<AppManifestAnalyzer, DefaultVerifier>()
        {
            TestCode = CSharpCode,
            TestState =
            {
                AdditionalFiles = { (manifestFilePath, manifestFile) },
                AnalyzerConfigFiles = { ("/.globalconfig", $"is_global = true\r\ndotnet_diagnostic.{DiagnosticIDs.MigrateHighDpiSettings}.severity = none") }
            }
        }.RunAsync();
    }

    [Fact]
    public async Task AppManifestAnalyzer_can_suppressed_if_manifest_file_has_dpi_info_VB()
    {
        const string manifestFilePath = @"C:\temp\app.manifest";

        SourceText manifestFile = SourceText.From(
            File.ReadAllText(@"System\Windows\Forms\Analyzers\MockData\dpi.manifest"));

        await new VisualBasicAnalyzerTest<AppManifestAnalyzer, DefaultVerifier>()
        {
            TestCode = VbCode,
            TestState =
            {
                AdditionalFiles = { (manifestFilePath, manifestFile) },
                AnalyzerConfigFiles = { ("/.globalconfig", $"is_global = true\r\ndotnet_diagnostic.{DiagnosticIDs.MigrateHighDpiSettings}.severity = none") }
            }
        }.RunAsync();
    }
}
