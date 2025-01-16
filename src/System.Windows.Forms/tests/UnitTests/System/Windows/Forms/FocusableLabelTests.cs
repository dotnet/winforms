// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms;

public class FocusableLabelTests
{
    [Fact]
    public void UnderlineWhenFocused_DefaultValue_ShouldBeTrue()
    {
        using FocusableLabel label = new();

        label.UnderlineWhenFocused.Should().BeTrue();
    }

    [Fact]
    public void FocusableLabel_ShouldBeSelectableAndTabStop()
    {
        using FocusableLabel label = new();

        label.TabStop.Should().BeTrue("Expected TabStop to be true, indicating the control is selectable.");
    }
}
