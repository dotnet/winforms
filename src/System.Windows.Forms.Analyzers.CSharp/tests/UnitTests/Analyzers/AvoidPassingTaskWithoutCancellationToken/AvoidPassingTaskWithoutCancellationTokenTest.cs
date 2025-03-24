// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.Diagnostics;
using System.Windows.Forms.CSharp.Analyzers.AvoidPassingTaskWithoutCancellationToken;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace System.Windows.Forms.Analyzers.Tests;

public sealed class AvoidPassingTaskWithoutCancellationTokenTests
{
    private const string TestCode = """
        using System;
        using System.Threading;
        using System.Threading.Tasks;
        using System.Windows.Forms;
        
        namespace CSharpControls;
        
        public static class Program
        {
            public static void Main()
            {
                var control = new Control();
        
                // A sync Action delegate is always fine.
                var okAction = new Action(() => control.Text = "Hello, World!");

                // A sync Func delegate is also fine.
                var okFunc = new Func<int>(() => 42);
        
                // Just a Task we will get in trouble since it's handled as a fire and forget.
                var notOkAsyncFunc = new Func<Task>(() =>
                {
                    control.Text = "Hello, World!";
                    return Task.CompletedTask;
                });
        
                // A Task returning a value will also get us in trouble since it's handled as a fire and forget.
                var notOkAsyncFunc2 = new Func<Task<int>>(() =>
                {
                    control.Text = "Hello, World!";
                    return Task.FromResult(42);
                });
        
                // OK.
                var task1 = control.InvokeAsync(okAction);

                // Also OK.
                var task2 = control.InvokeAsync(okFunc);
        
                // Concerning. - Most likely fire and forget by accident. We should warn about this.
                var task3 = control.InvokeAsync(notOkAsyncFunc, System.Threading.CancellationToken.None);
        
                // Again: Concerning. - Most likely fire and forget by accident. We should warn about this.
                var task4 = control.InvokeAsync(notOkAsyncFunc, System.Threading.CancellationToken.None);
        
                // And again concerning. - We should warn about this, too.
                var task5 = control.InvokeAsync(notOkAsyncFunc2, System.Threading.CancellationToken.None);
        
                // This is OK, since we're passing a cancellation token.
                var okAsyncFunc = new Func<CancellationToken, ValueTask>((cancellation) =>
                {
                    control.Text = "Hello, World!";
                    return ValueTask.CompletedTask;
                });
        
                // This is also OK, again, because we're passing a cancellation token.
                var okAsyncFunc2 = new Func<CancellationToken, ValueTask<int>>((cancellation) =>
                {
                    control.Text = "Hello, World!";
                    return ValueTask.FromResult(42);
                });
        
                // And let's test that, too:
                var task6 = control.InvokeAsync(okAsyncFunc, System.Threading.CancellationToken.None);
        
                // And that, too:
                var task7 = control.InvokeAsync(okAsyncFunc2, System.Threading.CancellationToken.None);
            }
        }
                
        """;

    public static IEnumerable<object?[]> GetReferenceAssemblies()
    {
        yield return [ReferenceAssemblies.Net.Net90.AddPackages(
            [new PackageIdentity("Microsoft.WindowsDesktop.App.Ref", "9.0.0")]), ""];

        // The latest public API surface area build from this repository.
        yield return [CurrentReferences.NetCoreAppReferences, CurrentReferences.WinFormsRefPath];
    }

    [Theory]
    [MemberData(nameof(GetReferenceAssemblies))]
    public async Task CS_AvoidPassingTaskWithoutCancellationAnalyzer(ReferenceAssemblies? referenceAssemblies, string? pathToWinFormsAssembly)
    {
        Assert.NotNull(referenceAssemblies);
        Assert.NotNull(pathToWinFormsAssembly);
        Assert.True(pathToWinFormsAssembly == "" || File.Exists(pathToWinFormsAssembly));

        string diagnosticId = DiagnosticIDs.AvoidPassingFuncReturningTaskWithoutCancellationToken;

        var context = new CSharpAnalyzerTest
            <AvoidPassingTaskWithoutCancellationTokenAnalyzer,
             DefaultVerifier>
        {
            TestCode = TestCode,
            TestState =
                {
                    OutputKind = OutputKind.WindowsApplication,
                    ExpectedDiagnostics =
                    {
                        DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(41, 21, 41, 97),
                        DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(44, 21, 44, 97),
                        DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(47, 21, 47, 98),
                    }
                },
            ReferenceAssemblies = referenceAssemblies
        };

        if (pathToWinFormsAssembly != "")
        {
            context.TestState.AdditionalReferences.Add(pathToWinFormsAssembly);
        }

        await context.RunAsync();
    }
}
