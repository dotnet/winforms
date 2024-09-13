// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests;

public class ApplicationTests : ControlTestBase
{
    public ApplicationTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public async Task Application_OpenForms_RecreateHandle()
    {
        await RunTestAsync(form =>
        {
            form.Show();
            using Form child = new() { RightToLeft = RightToLeft.No };
#pragma warning disable VSTHRD103 // Call async methods when in an async method
            child.Show(form);
#pragma warning restore VSTHRD103

            const int expectedFormCount = 2;
            Assert.Equal(expectedFormCount, Application.OpenForms.Count);
            child.RightToLeft = RightToLeft.Yes;
            Assert.Equal(expectedFormCount, Application.OpenForms.Count);
            child.ShowInTaskbar = !child.ShowInTaskbar;
            Assert.Equal(expectedFormCount, Application.OpenForms.Count);
            child.RecreateHandleCore();
            Assert.Equal(expectedFormCount, Application.OpenForms.Count);

            return Task.CompletedTask;
        });
    }

    private async Task RunTestAsync(Func<Form, Task> runTest)
    {
        await RunFormWithoutControlAsync(
            testDriverAsync: runTest,
            createForm: () =>
            {
                return new()
                {
                    ClientSize = new Size(640, 480)
                };
            });
    }
}
