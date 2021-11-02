// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.UI.IntegrationTests.Infra;
using Xunit;

namespace System.Windows.Forms.UI.IntegrationTests
{
    [ConfigureJoinableTaskFactory]
    public class LabelTests
    {
        [StaFact]
        public void AutoSize_Does_Not_Change_Size_When_False()
        {
            RunTest(label =>
            {
                label.AutoSize = false;

                label.Size = new Size(10, 10);
                label.Text = "Hello";
                Size oldSize = label.Size;
                label.Text = "Say Hello";
                Size newSize = label.Size;
                Assert.Equal(newSize, oldSize);
            });
        }

        [StaFact]
        public void AutoSize_Changes_Size_When_True()
        {
            RunTest(label =>
            {
                label.AutoSize = true;

                label.Size = new Size(10, 10);
                label.Text = "Hello";
                Size oldSize = label.Size;
                label.Text = "Say Hello";
                Size newSize = label.Size;
                Assert.NotEqual(newSize, oldSize);
            });
        }

        private void RunTest(Action<Label> runTest)
        {
            UITest.RunControl(
                createControl: form =>
                {
                    Label label = new()
                    {
                        Parent = form,
                        Text = "&Label"
                    };
                    return label;
                },
                runTestAsync: async label =>
                {
                    // Wait for pending operations so the Control is loaded completely before testing it
                    await AsyncTestHelper.JoinPendingOperationsAsync(AsyncTestHelper.UnexpectedTimeout);

                    runTest(label);
                });
        }
    }
}
