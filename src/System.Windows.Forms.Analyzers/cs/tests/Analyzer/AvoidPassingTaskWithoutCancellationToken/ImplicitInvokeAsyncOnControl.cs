// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.Diagnostics;
using System.Windows.Forms.Analyzers.Tests.Microsoft.WinForms;
using System.Windows.Forms.CSharp.Analyzers.AvoidPassingTaskWithoutCancellationToken;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.WinForms.Test;
using Microsoft.WinForms.Utilities.Shared;

namespace System.Windows.Forms.Analyzers.CSharp.Tests.AnalyzerTests.AvoidPassingTaskWithoutCancellationToken;

/// <summary>
///  Tests for the AvoidPassingTaskWithoutCancellationTokenAnalyzer that verify it correctly
///  detects InvokeAsync calls without explicit 'this' keyword.
/// </summary>
public class ImplicitInvokeAsyncOnControl 
    : RoslynAnalyzerAndCodeFixTestBase<AvoidPassingTaskWithoutCancellationTokenAnalyzer, DefaultVerifier>
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="ImplicitInvokeAsyncOnControl"/> class.
    /// </summary>
    public ImplicitInvokeAsyncOnControl()
        : base(SourceLanguage.CSharp) { }

    /// <summary>
    ///  Retrieves reference assemblies for the latest target framework versions.
    /// </summary>
    public static IEnumerable<object[]> GetReferenceAssemblies()
    {
        NetVersion[] tfms =
        [
            NetVersion.Net9_0
        ];

        foreach (ReferenceAssemblies refAssembly in ReferenceAssemblyGenerator.GetForLatestTFMs(tfms))
        {
            yield return new object[] { refAssembly };
        }
    }

    /// <summary>
    ///  Tests that the analyzer detects InvokeAsync calls with Task return types
    ///  even when the 'this' keyword is omitted.
    /// </summary>
    [Theory]
    [CodeTestData(nameof(GetReferenceAssemblies))]
    public async Task DetectImplicitInvokeAsyncCalls(
        ReferenceAssemblies referenceAssemblies,
        TestDataFileSet fileSet)
    {
        // Make sure, we can resolve the assembly we're testing against:
        var referenceAssembly = await referenceAssemblies.ResolveAsync(
            language: string.Empty,
            cancellationToken: CancellationToken.None);

        string diagnosticId = DiagnosticIDs.AvoidPassingFuncReturningTaskWithoutCancellationToken;

        var context = GetAnalyzerTestContext(fileSet, referenceAssemblies);
        
        // Explicitly specify where diagnostics are expected
        context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(31, 13, 31, 89));
        context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(46, 13, 46, 89));
        context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(74, 13, 74, 89));

        await context.RunAsync();
    }
}
