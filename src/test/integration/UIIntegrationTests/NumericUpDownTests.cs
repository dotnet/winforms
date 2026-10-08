// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.UITests;

public class NumericUpDownTests : ControlTestBase
{
    public NumericUpDownTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [ActiveIssue("https://github.com/dotnet/winforms/issues/11329")]
    [WinFormsFact]
    [SkipOnArchitecture(TestArchitectures.X64,
        "Flaky tests, see: https://github.com/dotnet/winforms/issues/11329")]
    public async Task NumericUpDownAccessibleObject_Focused_ReturnsCorrectValueAsync()
    {
        await RunSingleControlTestAsync<NumericUpDown>(async (form, control) =>
        {
            var accessibleObject = control.AccessibilityObject;
            await MoveMouseToControlAsync(control);
            form.Activate();
            control.Focus();

            var focused = accessibleObject.GetFocused();
            Assert.NotNull(focused);
        });
    }

    [WinFormsFact]
    public async Task NumericUpDown_ModernChrome_UsesInsetEditAndSideBySideButtonsAsync()
    {
        await RunSingleControlTestAsync<NumericUpDown>(async (form, control) =>
        {
            control.VisualStylesMode = VisualStylesMode.Net11;
            control.AutoSize = true;
            form.PerformLayout();

            if (!control.UseSideBySideButtons)
            {
                return;
            }

            int expectedInset = control.LogicalToDeviceUnits(
                ModernControlVisualStyles.Fixed3DBorderPadding
                    + ModernControlVisualStyles.InternalChromeInset);

            Assert.True(control.Height >= control.PreferredHeight);
            Assert.Equal(expectedInset, control.TextBox.Left);
            Assert.Equal(expectedInset, control.UpDownButtonsInternal.Top);
            Assert.True(control.UpDownButtonsInternal.Bounds.Left >= control.TextBox.Bounds.Right);
        });
    }
}
