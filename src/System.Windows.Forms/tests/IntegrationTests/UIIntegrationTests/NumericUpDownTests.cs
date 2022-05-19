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
                control.Focus();

                var isFocused = control.Focused;
                AccessibleObject accessibleObject = control.AccessibilityObject;
                var focused = accessibleObject.GetFocused();
                Assert.NotNull(focused);

                return Task.CompletedTask;
            });
        }
    }
}
