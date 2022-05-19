// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests
{
    public class NumericUpDownTests : ControlTestBase
    {
        public NumericUpDownTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [WinFormsFact]
        public async Task NumericUpDownAccessibleObject_Focused_ReturnsCorrectValueAsync()
        {
            await RunSingleControlTestAsync<NumericUpDown>((form, control) =>
            {
                var accessibleObject = control.AccessibilityObject;
                form.BringToFront();
                control.Focus();

                Assert.True(control.Focused, "NumericUpDown should be focused");
                this.TestOutputHelper.WriteLine($"Textbox state: {accessibleObject.GetChild(0)!.State}");
                Assert.True(
                    AccessibleStates.Focused == (accessibleObject.GetChild(0)!.State & AccessibleStates.Focused),
                    "NumericUpDown's text box accessbile object state should be focused");
                var focused = accessibleObject.GetFocused();
                Assert.NotNull(focused);

                return Task.CompletedTask;
            });
        }
    }
}
