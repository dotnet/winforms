// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.Diagnostics;
using System.Windows.Forms.CSharp.Analyzers.AvoidPassingTaskWithoutCancellationToken;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace System.Windows.Forms.Analyzers.Test;

public class AvoidPassingFuncReturningTaskWithoutCancellationTokenTest
{
    // Currently, we do not have Control.InvokeAsync in the .NET 9.0 Windows reference assemblies.
    // That's why we need to add this Async Control. Once it's there, this test will fail.
    // We can then remove the AsyncControl and the test will pass, replace AsyncControl with
    // Control, and the test will pass.
    private const string AsyncControl = """
        using System;
        using System.Threading;
        using System.Threading.Tasks;
        using System.Windows.Forms;

        namespace System.Windows.Forms
        {
            public class AsyncControl : Control
            {
                // BEGIN ASYNC API
                public Task InvokeAsync(
                    Action callback,
                    CancellationToken cancellationToken = default)
                {
                    var tcs = new TaskCompletionSource();

                    // Note: Code is INCORRECT, it's just here to satisfy the compiler!
                    using (cancellationToken.Register(() => tcs.TrySetCanceled()))
                    {
                        base.BeginInvoke(callback);
                    }

                    return tcs.Task;
                }

                public Task InvokeAsync<T>(
                    Func<T> callback,
                    CancellationToken cancellationToken = default)
                {
                    var tcs = new TaskCompletionSource<T>();

                    // Note: Code is INCORRECT, it's just here to satisfy the compiler!
                    using (cancellationToken.Register(() => tcs.TrySetCanceled()))
                    {
                        base.BeginInvoke(callback);
                    }

                    return tcs.Task;
                }

                public Task InvokeAsync(
                    Func<CancellationToken, ValueTask> callback,
                    CancellationToken cancellationToken = default)
                {
                    var tcs = new TaskCompletionSource();

                    // Note: Code is INCORRECT, it's just here to satisfy the compiler!
                    using (cancellationToken.Register(() => tcs.TrySetCanceled()))
                    {
                        base.BeginInvoke(callback);
                    }

                    return tcs.Task;
                }

                public Task<T> InvokeAsync<T>(
                    Func<CancellationToken, ValueTask<T>> callback,
                    CancellationToken cancellationToken = default)
                {
                    var tcs = new TaskCompletionSource<T>();
                    // Note: Code is INCORRECT, it's just here to satisfy the compiler!
                    using (cancellationToken.Register(() => tcs.TrySetCanceled()))
                    {
                        base.BeginInvoke(callback);
                    }
                    return tcs.Task;
                }
                // END ASYNC API
            }
        }
        
        """;

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
                var control = new AsyncControl();
        
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

    public static IEnumerable<object[]> GetReferenceAssemblies()
    {
        yield return [ReferenceAssemblies.Net.Net90Windows];
    }

    [Theory]
    [MemberData(nameof(GetReferenceAssemblies))]
    public async Task CS_AvoidPassingFuncReturningTaskWithoutCancellationAnalyzer(ReferenceAssemblies referenceAssemblies)
    {
        // If the API does not exist, we need to add it to the test.
        string customControlSource = AsyncControl;
        string diagnosticId = DiagnosticIDs.AvoidPassingFuncReturningTaskWithoutCancellationToken;

        var context = new CSharpAnalyzerTest
            <AvoidPassingFuncReturningTaskWithoutCancellationTokenAnalyzer,
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
