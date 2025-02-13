// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis.Testing;

namespace System.Windows.Forms.Analyzers.CSharp.Tests.AnalyzerTests.AvoidPassingTaskWithoutCancellationToken;

public class InvokeAsyncOnControl
{
    public static IEnumerable<object[]> GetReferenceAssemblies()
    {
        yield return [ReferenceAssemblies.Net.Net90Windows];
    }

    //[Theory]
    //[MemberData(nameof(GetReferenceAssemblies))]
    //public async Task AvoidPassingTaskWithoutCancellationAnalyzer(ReferenceAssemblies referenceAssemblies)
    //{
    //    string diagnosticId = DiagnosticIDs.AvoidPassingFuncReturningTaskWithoutCancellationToken;

    //    var context = new CSharpAnalyzerTest
    //        <AvoidPassingTaskWithoutCancellationTokenAnalyzer,
    //         DefaultVerifier>
    //    {
    //        TestCode = TestCode,
    //        TestState =
    //            {
    //                OutputKind = OutputKind.WindowsApplication,
    //                Sources = { customControlSource },
    //                ExpectedDiagnostics =
    //                {
    //                    DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(41, 21, 41, 97),
    //                    DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(44, 21, 44, 97),
    //                    DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(47, 21, 47, 98),
    //                },
    //            },
    //        ReferenceAssemblies = referenceAssemblies
    //    };

    //    await context.RunAsync();
    // }
}
