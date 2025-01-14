// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms;
public class FocusableLabelTests
{
    [WinFormsFact]
    public void UnderlineWhenFocused_DefaultValue_ShouldBeTrue()
    {
        // Arrange
        var label = new FocusableLabel();

        // Act
        var result = label.UnderlineWhenFocused;

        // Assert
        result.Should().BeTrue();
    }

    [WinFormsFact]
    public void FocusableLabel_ShouldBeSelectableAndTabStop()
    {
        // Arrange
        var label = new FocusableLabel();

        // Act
        var isTabStop = label.TabStop;

        // Assert
        isTabStop.Should().BeTrue("Expected TabStop to be true, indicating the control is selectable.");
    }
}
