// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.Diagnostics;
using System.Windows.Forms.CSharp.Analyzers.AvoidPassingTaskWithoutCancellationToken;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace System.Windows.Forms.Analyzers.Test;

public class AvoidPassingTaskWithoutCancellationTokenTests
{
    // Currently, we do not have Control.InvokeAsync in the .NET 9.0 Windows reference assemblies.
    // That's why we need to add this Async Control. Once it's there, this test will fail.
    // We can then remove the AsyncControl and the test will pass, replace AsyncControl with
    // Control, and the test will pass.
    private const string AsyncControl = """
        
        """;

    private const string TestCode = """
                
        """;

    public static IEnumerable<object[]> GetReferenceAssemblies()
    {
        yield return [ReferenceAssemblies.Net.Net90Windows];
    }

    [Theory]
    [MemberData(nameof(GetReferenceAssemblies))]
    public async Task AvoidPassingTaskWithoutCancellationAnalyzer(ReferenceAssemblies referenceAssemblies)
    {
        // If the API does not exist, we need to add it to the test.
        string customControlSource = AsyncControl;
        string diagnosticId = DiagnosticIDs.AvoidPassingFuncReturningTaskWithoutCancellationToken;

        var context = new CSharpAnalyzerTest
            <AvoidPassingTaskWithoutCancellationTokenAnalyzer,
             DefaultVerifier>
        {
            TestCode = TestCode,
            TestState =
                {
                    OutputKind = OutputKind.WindowsApplication,
                    Sources = { customControlSource },
                    ExpectedDiagnostics =
                    {
                        DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(41, 21, 41, 97),
                        DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(44, 21, 44, 97),
                        DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(47, 21, 47, 98),
                    },
                },
            ReferenceAssemblies = referenceAssemblies
        };

        await context.RunAsync();
    }
}
