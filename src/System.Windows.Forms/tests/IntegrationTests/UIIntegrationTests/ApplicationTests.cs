// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
            var child = new Form() { RightToLeft = RightToLeft.No };
            child.Show(form);

            var formCnt = 2;
            Assert.Equal(formCnt, Application.OpenForms.Count);
            child.RightToLeft = RightToLeft.Yes;
            Assert.Equal(formCnt, Application.OpenForms.Count);
            child.ShowInTaskbar = !child.ShowInTaskbar;
            Assert.Equal(formCnt, Application.OpenForms.Count);
            child.RecreateHandleCore();
            Assert.Equal(formCnt, Application.OpenForms.Count);

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
