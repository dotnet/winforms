// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit.Abstractions;

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
}
