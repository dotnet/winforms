// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.CSharp.Analyzers.ConsiderNotPassingATaskWithoutCancellationToken;
using System.Windows.Forms.CSharp.CodeFixes.AddDesignerSerializationVisibility;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace System.Windows.Forms.Analyzers.Test;

public class ConsiderNotPassingFuncReturningTaskWithoutCancellationToken
{
    private const string ProblematicCode = """
        using System;
        using System.Threading.Tasks;
        using System.Windows.Forms;
        
        namespace CSharpControls;

        public static class Program
        {
            public static void Main()
            {
                var control = new Control();

                // An Action Delegate:
                var action = new Action(() => control.Text = "Hello, World!");

                // Just a Task:
                var asyncFunc = new Func<Task>(() =>
                {
                    control.Text = "Hello, World!";
                    return Task.CompletedTask;
                });

                // A Task returning a value:
                var asyncFunc2 = new Func<Task<int>>(() =>
                {
                    control.Text = "Hello, World!";
                    return Task.FromResult(42);
                });

                // OK.
                var task1 = control.InvokeAsync(action);

                // Concerning! - Most likely fire and forget by accident. We should warn about this!
                var task2 = control.InvokeAsync(asyncFunc);

                // This is fine. When we're passing a cancellation token, everything is handled internally correctly.
                var task3 = control.InvokeAsync(asyncFunc, System.Threading.CancellationToken.None);

                // This is fine. When we're passing a cancellation token, everything is handled internally correctly.
                var task4 = control.InvokeAsync(asyncFunc2, System.Threading.CancellationToken.None);
            }
        }

        """;

    private const string CorrectCode = """
        using System;
        using System.Threading.Tasks;
        using System.Windows.Forms;
        
        namespace CSharpControls;
        
        public static class Program
        {
            public static void Main()
            {
                var control = new Control();
        
                // An Action Delegate:
                var action = new Action(() => control.Text = "Hello, World!");
        
                // Just a Task:
                var asyncFunc = new Func<Task>(() =>
                {
                    control.Text = "Hello, World!";
                    return Task.CompletedTask;
                });
        
                // A Task returning a value:
                var asyncFunc2 = new Func<Task<int>>(() =>
                {
                    control.Text = "Hello, World!";
                    return Task.FromResult(42);
                });
        
                // OK.
                var task1 = control.InvokeAsync(action);
        
                // Concerning! - Most likely fire and forget by accident. We should warn about this!
                var task2 = control.InvokeAsync(asyncFunc);
        
                // This is fine. When we're passing a cancellation token, everything is handled internally correctly.
                var task3 = control.InvokeAsync(asyncFunc, System.Threading.CancellationToken.None);
        
                // This is fine. When we're passing a cancellation token, everything is handled internally correctly.
                var task4 = control.InvokeAsync(asyncFunc2, System.Threading.CancellationToken.None);
            }
        }
                
        """;

    private const string FixedCode = """
        using System.Windows.Forms;
        
        namespace CSharpControls;
        
        public static class Program
        {
            public static void Main()
            {
                var control = new Control();
        
                // An Action Delegate:
                var action = new Action(() => control.Text = ""Hello, World!"");
        
                // Just a Task:
                var asyncFunc = new Func<Task>(() =>
                {
                    control.Text = ""Hello, World!"";
                    return Task.CompletedTask;
                });
        
                // A Task returning a value:
                var asyncFunc2 = new Func<Task<int>>(() =>
                {
                    control.Text = ""Hello, World!"";
                    return 42;
                });
        
                // OK.
                var task1 = control.InvokeAsync(action);
        
                // Concerning! - Most likely fire and forget by accident. We should warn about this!
                var task2 = control.InvokeAsync(asyncFunc, System.Threading.CancellationToken.None);
        
                // This is fine. When we're passing a cancellation token, everything is handled internally correctly.
                var task3 = control.InvokeAsync(asyncFunc, System.Threading.CancellationToken.None);
        
                // This is fine. When we're passing a cancellation token, everything is handled internally correctly.
                var task4 = control.InvokeAsync(asyncFunc2, System.Threading.CancellationToken.None);
            }
        }

        """;

    // We are testing the analyzer with all versions of the .NET SDK from 9.0 on.
    public static IEnumerable<object[]> GetReferenceAssemblies()
    {
        yield return [ReferenceAssemblies.Net.Net90Windows];
    }

    [Theory]
    [MemberData(nameof(GetReferenceAssemblies))]
    public async Task CS_ConsiderNotPassingFuncReturningTaskWithoutCancellationAnalyzer(ReferenceAssemblies referenceAssemblies)
    {
        var context = new CSharpAnalyzerTest
            <ConsiderNotPassingFuncReturningTaskWithoutCancellationTokenAnalyzer,
             DefaultVerifier>
        {
            TestCode = CorrectCode,
            TestState =
                {
                    OutputKind = OutputKind.WindowsApplication,
                },
            ReferenceAssemblies = referenceAssemblies
        };

        await context.RunAsync();
    }

    [Theory]
    [MemberData(nameof(GetReferenceAssemblies))]
    public async Task CS_ConsiderNotPassingFuncReturningTaskWithoutCancellationCodeFix(ReferenceAssemblies referenceAssemblies)
    {
        var context = new CSharpCodeFixTest
            <ConsiderNotPassingFuncReturningTaskWithoutCancellationTokenAnalyzer,
             AddDesignerSerializationVisibilityCodeFixProvider,
             DefaultVerifier>
        {
            TestCode = ProblematicCode,
            FixedCode = FixedCode,
            TestState =
                {
                    OutputKind = OutputKind.WindowsApplication,
                },
            ReferenceAssemblies = referenceAssemblies,
            NumberOfFixAllIterations = 2
        };

        await context.RunAsync();
    }
}
