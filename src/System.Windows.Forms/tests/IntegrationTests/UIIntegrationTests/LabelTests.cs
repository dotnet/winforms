// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests
{
    public class LabelTests : ControlTestBase
    {
        public LabelTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [WinFormsFact]
        public async Task Label_AutoSize_Does_Not_Change_Size_When_FalseAsync()
        {
            await RunTestAsync((form, label) =>
            {
                label.AutoSize = false;
                label.Size = new Size(10, 10);
                label.Text = "Hello";
                Size oldSize = label.Size;
                label.Text = "Say Hello";
                Size newSize = label.Size;

                Assert.Equal(newSize, oldSize);

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task Label_AutoSize_Changes_Size_When_TrueAsync()
        {
            await RunTestAsync((form, label) =>
            {
                label.AutoSize = true;
                label.Size = new Size(10, 10);
                label.Text = "Hello";
                Size oldSize = label.Size;
                label.Text = "Say Hello";
                Size newSize = label.Size;

                Assert.NotEqual(newSize, oldSize);

                return Task.CompletedTask;
            });
        }

        private async Task RunTestAsync(Func<Form, Label, Task> runTest)
        {
            await RunSingleControlTestAsync(
                testDriverAsync: runTest,
                createControl: () =>
                {
                    Label control = new()
                    {
                        Text = "Hello",
                    };

                    return control;
                },
                createForm: () =>
                {
                    return new()
                    {
                        Size = new(500, 300),
                    };
                });
        }
    }
}
