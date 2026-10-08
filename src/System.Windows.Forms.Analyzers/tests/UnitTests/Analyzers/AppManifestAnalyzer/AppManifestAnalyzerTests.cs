// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
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
        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<AppManifestAnalyzer>(
            CreateTestCase(CSharpCode))
            .RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task AppManifestAnalyzer_no_op_if_manifest_file_has_no_dpi_info()
    {
        string input = await TestFileLoader.GetAnalyzerTestCodeAsync("nodpi.manifest");
        SourceText manifestFile = SourceText.From(input);

        AnalyzerTestCase testCase = CreateTestCase(CSharpCode);
        testCase.AdditionalFiles.Add((@"C:\temp\app.manifest", manifestFile));

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<AppManifestAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task AppManifestAnalyzer_noop_if_manifest_file_corrupt()
    {
        string input = await TestFileLoader.GetAnalyzerTestCodeAsync("invalid.manifest");
        SourceText manifestFile = SourceText.From(input);

        AnalyzerTestCase testCase = CreateTestCase(CSharpCode);
        testCase.AdditionalFiles.Add((@"C:\temp\app.manifest", manifestFile));

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<AppManifestAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task AppManifestAnalyzer_warn_if_manifest_file_has_dpi_info_CSharp()
    {
        const string manifestFilePath = @"C:\temp\app.manifest";

        string input = await TestFileLoader.GetAnalyzerTestCodeAsync("dpi.manifest");
        SourceText manifestFile = SourceText.From(input);

        AnalyzerTestCase testCase = CreateTestCase(CSharpCode);
        testCase.AdditionalFiles.Add((manifestFilePath, manifestFile));
        testCase.ExpectedDiagnostics.Add(
            new DiagnosticResult(SharedDiagnosticDescriptors.s_cSharpMigrateHighDpiSettings)
                .WithArguments(manifestFilePath, ApplicationConfig.PropertyNameCSharp.HighDpiMode));

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<AppManifestAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task AppManifestAnalyzer_warn_if_manifest_file_has_dpi_info_VB()
    {
        const string manifestFilePath = @"C:\temp\app.manifest";

        string input = await TestFileLoader.GetAnalyzerTestCodeAsync("dpi.manifest");
        SourceText manifestFile = SourceText.From(input);

        AnalyzerTestCase testCase = CreateTestCase(VbCode);
        testCase.AdditionalFiles.Add((manifestFilePath, manifestFile));
        testCase.ExpectedDiagnostics.Add(
            new DiagnosticResult(SharedDiagnosticDescriptors.s_visualBasicMigrateHighDpiSettings)
                .WithArguments(manifestFilePath, ApplicationConfig.PropertyNameVisualBasic.HighDpiMode));

        await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest<AppManifestAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task AppManifestAnalyzer_can_suppressed_if_manifest_file_has_dpi_info_CSharp()
    {
        const string manifestFilePath = @"C:\temp\app.manifest";

        string input = await TestFileLoader.GetAnalyzerTestCodeAsync("dpi.manifest");
        SourceText manifestFile = SourceText.From(input);

        AnalyzerTestCase testCase = CreateTestCase(CSharpCode);
        testCase.AdditionalFiles.Add((manifestFilePath, manifestFile));
        AddSuppression(testCase);

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<AppManifestAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task AppManifestAnalyzer_can_suppressed_if_manifest_file_has_dpi_info_VB()
    {
        const string manifestFilePath = @"C:\temp\app.manifest";

        string input = await TestFileLoader.GetAnalyzerTestCodeAsync("dpi.manifest");
        SourceText manifestFile = SourceText.From(input);

        AnalyzerTestCase testCase = CreateTestCase(VbCode);
        testCase.AdditionalFiles.Add((manifestFilePath, manifestFile));
        AddSuppression(testCase);

        await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest<AppManifestAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    private static AnalyzerTestCase CreateTestCase(string source)
        => new(
            ReferenceAssemblies.Net.Net90Windows,
            new AnalyzerTestSource("Test0", source));

    private static void AddSuppression(AnalyzerTestCase testCase)
        => testCase.AnalyzerConfigFiles.Add(
            ("/.globalconfig", $"is_global = true\r\ndotnet_diagnostic.{DiagnosticIDs.MigrateHighDpiSettings}.severity = none"));
}
