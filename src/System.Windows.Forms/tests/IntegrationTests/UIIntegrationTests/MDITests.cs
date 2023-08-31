// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests;

public class MDITests : ControlTestBase
{
    public MDITests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public async Task MDIForm_ResizeWhenMdiChildrenMinimizedAnchorBottom_DefaultAsync()
    {
        await RunTestAsync(form =>
        {
            using Form childForm = new()
            {
                MdiParent = form,
                WindowState = FormWindowState.Minimized
            };

            childForm.Show();

            int childFormMinimizedYPositionFromBottom = form.ClientSize.Height - childForm.Top;
            form.Height += 100;

            Assert.Equal(childFormMinimizedYPositionFromBottom, form.ClientSize.Height - childForm.Top);

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task MDIForm_ResizeWhenMdiChildrenMinimizedAnchorBottom_FalseAsync()
    {
        await RunTestAsync(form =>
        {
            using Form childForm = new()
            {
                MdiParent = form,
                WindowState = FormWindowState.Minimized
            };
            form.MdiChildrenMinimizedAnchorBottom = false;

            childForm.Show();

            int childFormMinimizedTop = childForm.Top;
            form.Height += 100;

            Assert.Equal(childFormMinimizedTop, childForm.Top);

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
                    IsMdiContainer = true,
                    ClientSize = new Size(640, 480),
                };
            });
    }
}
