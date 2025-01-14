// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms;

public class FocusableLabelTests
{
    [WinFormsFact]
    public void UnderlineWhenFocused_DefaultValue_ShouldBeTrue()
    {
        FocusableLabel label = new();

        var result = label.UnderlineWhenFocused;

        result.Should().BeTrue();
    }

    [WinFormsFact]
    public void FocusableLabel_ShouldBeSelectableAndTabStop()
    {
        FocusableLabel label = new();

        var isTabStop = label.TabStop;

        isTabStop.Should().BeTrue("Expected TabStop to be true, indicating the control is selectable.");
    }
}
