// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Reflection;

namespace System.Windows.Forms;

public class FocusableLabelTests
{
    [Fact]
    public void UnderlineWhenFocused_ShouldBeTrue()
    {
        using FocusableLabel label = new();

        label.UnderlineWhenFocused.Should().BeTrue();
    }

    [Fact]
    public void UnderlineWhenFocused_SetToFalse_ShouldBeFalse()
    {
        using FocusableLabel label = new();

        label.UnderlineWhenFocused = false;

        label.UnderlineWhenFocused.Should().BeFalse();
    }

    [Fact]
    public void CreateParams_ClassName_ShouldBeNull()
    {
        using FocusableLabel label = new();

        var createParams = GetCreateParams(label);

        createParams.Should().NotBeNull();

        var className = createParams?.GetType().GetProperty("ClassName")?.GetValue(createParams);

        className.Should().BeNull();
    }

    private static object? GetCreateParams(Control control)
    {
        return typeof(Control)
        .GetProperty("CreateParams", BindingFlags.NonPublic | BindingFlags.Instance)?
        .GetValue(control);
    }

    [Fact]
    public void TabStop_ShouldBeTrue()
    {
        using FocusableLabel label = new();

        label.TabStop.Should().BeTrue("Expected TabStop to be true, indicating the control is selectable.");
    }
}
