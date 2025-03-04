// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class FocusableLabelTests : IDisposable
{
    private readonly FocusableLabel _focusableLabel = new();

    public void Dispose() => _focusableLabel.Dispose();

    [Fact]
    public void UnderlineWhenFocused_ShouldBeTrue() => _focusableLabel.UnderlineWhenFocused.Should().BeTrue();

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
        using SubFocusableLabel subLabel = new();

        subLabel.CreateParams?.ClassName.Should().BeNull();
    }

    private class SubFocusableLabel : FocusableLabel
    {
        public new CreateParams? CreateParams => base.CreateParams;
    }

    [Fact]
    public void TabStop_ShouldBeTrue()
    {
        using FocusableLabel label = new();

        label.TabStop.Should().BeTrue();
    }
}
