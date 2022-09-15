// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests
{
    public class TaskDialogTests : ControlTestBase
    {
        public TaskDialogTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [WinFormsFact]
        public void TaskDialog_ShowDialog_SetProperty_SameThread_Success()
        {
            // Control.CheckForIllegalCrossThreadCalls is process-wide and may affect other unit tests that run in parallel. This test
            // method has been moved to UITests so that it can be executed serially, and without the risk of affecting other tests.
            Control.CheckForIllegalCrossThreadCalls = true;

            TaskDialogPage page = new TaskDialogPage();
            page.Created += (_, __) =>
            {
                // Set the property in the same thread.
                page.Text = "X";
                page.BoundDialog?.Close();
            };

            TaskDialog.ShowDialog(page);
        }

        [WinFormsFact]
        public void TaskDialog_ShowDialog_SetProperty_DifferentThread_ThrowsInvalidOperationException()
        {
            // Control.CheckForIllegalCrossThreadCalls is process-wide and may affect other unit tests that run in parallel. This test
            // method has been moved to UITests so that it can be executed serially, and without the risk of affecting other tests.
            Control.CheckForIllegalCrossThreadCalls = true;

            TaskDialogPage page = new TaskDialogPage();
            page.Created += (_, __) =>
            {
                // Set the property in a different thread.
                var separateTask = Task.Run(() => page.Text = "X");
                Assert.Throws<InvalidOperationException>(separateTask.GetAwaiter().GetResult);

                page.BoundDialog?.Close();
            };

            TaskDialog.ShowDialog(page);
        }
    }
}
