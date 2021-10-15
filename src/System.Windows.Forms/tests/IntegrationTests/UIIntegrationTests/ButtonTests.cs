// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.UI.IntegrationTests.Infra;
using Xunit;

namespace System.Windows.Forms.UI.IntegrationTests
{
    [ConfigureJoinableTaskFactory]
    public class ButtonTests
    {
        [StaFact]
        public void ButtonTests_Click_Fires_OnClick()
        {
            RunTest(button =>
            {
                bool wasClicked = false;
                button.Click += (x, y) => wasClicked = true;

                button.PerformClick();

                Assert.True(wasClicked);
            });
        }

        [StaFact]
        public void ButtonTests_Hotkey_Fires_OnClick()
        {
            RunTest(button =>
            {
                bool wasClicked = false;
                button.Click += (x, y) => wasClicked = true;

                SendKeys.SendWait("%C");

                Assert.True(wasClicked);
            });
        }

        [StaFact]
        public void ButtonTests_Hotkey_DoesNotFire_OnClick()
        {
            RunTest(button =>
            {
                bool wasClicked = false;
                button.Click += (x, y) => wasClicked = true;

                SendKeys.SendWait("%l");

                Assert.False(wasClicked);
            });
        }

        private void RunTest(Action<Button> runTest)
        {
            UITest.RunControl(
                createControl: form =>
                {
                    Button button = new()
                    {
                        Parent = form,
                        Text = "&Click"
                    };
                    return button;
                },
                runTestAsync: async button =>
                {
                    // Wait for pending operations so the Control is loaded completely before testing it
                    await AsyncTestHelper.JoinPendingOperationsAsync(AsyncTestHelper.UnexpectedTimeout);

                    runTest(button);
                });
        }
    }
}
