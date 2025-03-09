// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.Diagnostics;
using System.Windows.Forms.Analyzers.Tests.Microsoft.WinForms;
using System.Windows.Forms.CSharp.Analyzers.AvoidPassingTaskWithoutCancellationToken;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.WinForms.Test;
using Microsoft.WinForms.Utilities.Shared;

namespace System.Windows.Forms.Analyzers.CSharp.Tests.AnalyzerTests.AvoidPassingTaskWithoutCancellationToken;

public class InvokeAsyncOnControl
    : RoslynAnalyzerAndCodeFixTestBase<AvoidPassingTaskWithoutCancellationTokenAnalyzer, DefaultVerifier>
{
    public InvokeAsyncOnControl()
        : base(SourceLanguage.CSharp) { }

    public static IEnumerable<object[]> GetReferenceAssemblies()
    {
        NetVersion[] tfms =
        [
            NetVersion.Net9_0,
            NetVersion.Net10_0_Preview1,
        ];

        foreach (ReferenceAssemblies refAssembly in ReferenceAssemblyGenerator.GetForLatestTFMs(tfms))
        {
            yield return new object[] { refAssembly };
        }
    }

    [Theory]
    [CodeTestData(nameof(GetReferenceAssemblies))]
    public async Task AvoidPassingTaskWithoutCancellationAnalyzer(
        ReferenceAssemblies referenceAssemblies,
        TestDataFileSet fileSet)
    {
        // Make sure, we can resolve the assembly we're testing against:
        // Always pass `string.empty` for the language here to keep it generic.
        var referenceAssembly = await referenceAssemblies.ResolveAsync(
            language: string.Empty,
            cancellationToken: CancellationToken.None);

        string diagnosticId = DiagnosticIDs.AvoidPassingFuncReturningTaskWithoutCancellationToken;

        var context = GetAnalyzerTestContext(fileSet, referenceAssemblies);
        context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(41, 21, 41, 97));
        context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(44, 21, 44, 97));
        context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(47, 21, 47, 98));

        await context.RunAsync();
    }
}
