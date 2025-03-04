// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.CSharp.Analyzers.MissingPropertySerializationConfiguration;
using System.Windows.Forms.CSharp.CodeFixes.AddDesignerSerializationVisibility;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.WinForms.Test;
using Microsoft.WinForms.Utilities.Shared;

namespace System.Windows.Forms.Analyzers.CSharp.Tests.AnalyzerTests.MissingPropertySerializationConfiguration;

public class CustomControlScenarios
    : RoslynAnalyzerAndCodeFixTestBase<MissingPropertySerializationConfigurationAnalyzer, DefaultVerifier>
{
    public CustomControlScenarios() : base(SourceLanguage.CSharp)
    { }

    // We are testing the analyzer with all versions of the .NET SDK from 6.0 on.
    public static IEnumerable<object[]> GetReferenceAssemblies()
    {
        // yield return [ReferenceAssemblies.Net.Net60Windows];
        // yield return [ReferenceAssemblies.Net.Net70Windows];
        // yield return [ReferenceAssemblies.Net.Net80Windows];
        yield return [ReferenceAssemblies.Net.Net90Windows];
    }

    [Theory]
    [MemberData(nameof(GetReferenceAssemblies))]
    public async Task TestDiagnostics(ReferenceAssemblies referenceAssemblies)
    {
        await EnumerateTestFilesAsync(TestMethod);

        async Task TestMethod(TestDataFileSet fileSet)
        {
            var context = GetAnalyzerTestContext(fileSet, referenceAssemblies);
            await context.RunAsync().ConfigureAwait(false);

            context = GetFixedTestContext(fileSet, referenceAssemblies);
            await context.RunAsync().ConfigureAwait(false);
        }
    }

    [Theory]
    [MemberData(nameof(GetReferenceAssemblies))]
    public async Task TestCodeFix(ReferenceAssemblies referenceAssemblies)
    {
        await EnumerateTestFilesAsync(TestMethod);

        async Task TestMethod(TestDataFileSet fileSet)
        {
            var context = GetCodeFixTestContext<AddDesignerSerializationVisibilityCodeFixProvider>(
                fileSet,
                referenceAssemblies,

                // Negative numbers states |max| positive number states absolute.
                numberOfFixAllIterations: -2);

            // In this (and probably the most cases), we do not want to
            // take track about the Fix-iterations on a Project-
            // let alone the Solution level.
            context.CodeFixTestBehaviors =
                CodeFixTestBehaviors.SkipFixAllInProjectCheck |
                CodeFixTestBehaviors.SkipFixAllInSolutionCheck;

            await context.RunAsync().ConfigureAwait(false);
        }
    }
}
