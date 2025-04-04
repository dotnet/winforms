// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestNamespace;

public class MyForm : Form
{
    private async Task DoWorkWithoutThis()
    {
        // Case 1: Using InvokeAsync without 'this' in a synchronous context
        // This should be detected by the analyzer
        await InvokeAsync(ct => new ValueTask<bool>(DoSomethingAsync(ct)), CancellationToken.None);

        // Case 2: Using a variable instead of 'this', but not triggering the analyzer
        CancellationToken token = new CancellationToken();
        await this.InvokeAsync(ct => DoSomethingWithToken(ct), token);
    }

    private async Task DoWorkInNestedContext()
    {
        async Task LocalFunction()
        {
            // Case 3: Using InvokeAsync without 'this' in a nested function
            // This should be detected by the analyzer
            await InvokeAsync(
                ct => new ValueTask<int>(DoSomethingIntAsync(ct)),
                CancellationToken.None);
        }

        await LocalFunction();
    }

    // Helper methods for the test cases
    private async Task<bool> DoSomethingAsync(CancellationToken token)
    {
        await Task.Delay(100, token);
        return true;
    }

    private ValueTask DoSomethingWithToken(CancellationToken token)
    {
        return new ValueTask(Task.CompletedTask);
    }

    private async Task<int> DoSomethingIntAsync(CancellationToken token)
    {
        await Task.Delay(100, token);
        return 42;
    }
}

// Testing in a derived class to ensure the analyzer works with inheritance
public class DerivedForm : Form
{
    private async Task DoWorkInDerivedClass()
    {
        // Case 4: Using InvokeAsync without 'this' in a derived class
        // This should be detected by the analyzer
        await InvokeAsync(ct => new ValueTask<string>(DoSomethingStringAsync(ct)), CancellationToken.None);
    }

    private async Task<string> DoSomethingStringAsync(CancellationToken token)
    {
        await Task.Delay(100, token);
        return "test";
    }
}
