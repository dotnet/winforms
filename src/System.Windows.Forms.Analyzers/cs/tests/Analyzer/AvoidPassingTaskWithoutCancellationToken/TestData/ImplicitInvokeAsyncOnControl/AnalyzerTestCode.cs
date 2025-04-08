// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace TestNamespace;

public class MyForm : Form
{
    internal async Task DoWorkWithoutThis()
    {
        // Make sure, both get flagged, because they would 
        // not be awaited internally and became a fire-and-forget.
        await InvokeAsync(async () => await Task.Delay(100));
        await this.InvokeAsync(async () => await DoWorkInNestedContext());
    }

    private async Task DoWorkInNestedContext()
    {
        await LocalFunction();
        bool test = await InvokeAsync(
            DoSomethingWithTokenAsync,
            CancellationToken.None);

        async Task LocalFunction()
        {
            // Make sure we detect this inside of a nested local function.
            await InvokeAsync(async () => await Task.Delay(100));
        }
    }

    // Helper methods for the test cases
    private async Task<bool> DoSomethingAsync(CancellationToken token)
    {
        await Task.Delay(42 + 73, token);
        return true;
    }

    private async ValueTask<bool> DoSomethingWithTokenAsync(CancellationToken token)
    {
        bool flag = await DoSomethingAsync(token);
        var meaningOfLife = 21 + 21;

        return (meaningOfLife == await GetMeaningOfLifeAsync(token)) && flag;
    }

    private async Task<int> GetMeaningOfLifeAsync(CancellationToken token)
    {
        DerivedForm derivedForm = new();
        await derivedForm.DoWorkInDerivedClassAsync();

        await Task.Delay(100, token);
        return 42;
    }
}

// Testing in a derived class to ensure the analyzer works with inheritance
public class DerivedForm : Form
{
    internal async Task DoWorkInDerivedClassAsync()
    {
        await InvokeAsync(async () => await Task.Delay(99));

        await InvokeAsync(ct => new ValueTask<string>(
            task: DoSomethingStringAsync(ct)),
            cancellationToken: CancellationToken.None);

        await this.InvokeAsync(async () => await Task.Delay(99));
    }

    private async Task<string> DoSomethingStringAsync(CancellationToken token)
    {
        await Task.Delay(100, token);
        return "test";
    }
}
