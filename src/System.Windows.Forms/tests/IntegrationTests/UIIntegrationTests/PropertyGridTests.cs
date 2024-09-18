// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;
using FluentAssertions;

namespace System.Windows.Forms.UITests;
public class PropertyGridTests : IDisposable
{
    private readonly PropertyGrid _propertyGrid;
    private readonly Form _form;

    public PropertyGridTests()
    {
        _propertyGrid = new();
        _form = new() { Controls = { _propertyGrid } };
        _propertyGrid.CreateControl();
        _propertyGrid.SelectedObject = _form;
        _form.Show();
    }

    public void Dispose()
    {
        _form.Dispose();
        _propertyGrid.Dispose();
    }

    [WinFormsFact]
    public void PropertyGrid_Focused_Get_ShouldBeFalseByDefault()
    {
        _propertyGrid.Focused.Should().BeFalse();
    }

    [WinFormsFact]
    public void PropertyGrid_ToolbarVisible_GetSet_ReturnsExpected()
    {
        _propertyGrid.ToolbarVisible.Should().BeTrue();

        _propertyGrid.ToolbarVisible = false;
        _propertyGrid.ToolbarVisible.Should().BeFalse();
    }

    [WinFormsFact]
    public void PropertyGrid_HelpVisible_GetSet_ReturnsExpected()
    {
        _propertyGrid.HelpVisible.Should().BeTrue();

        _propertyGrid.HelpVisible = false;
        _propertyGrid.HelpVisible.Should().BeFalse();
    }

    [WinFormsFact]
    public void PropertyGrid_CommandsVisibleIfAvailable_GetSet_ReturnsExpected()
    {
        _propertyGrid.CommandsVisibleIfAvailable.Should().BeTrue();

        _propertyGrid.CommandsVisibleIfAvailable = false;
        _propertyGrid.CommandsVisibleIfAvailable.Should().BeFalse();
    }

    [WinFormsFact]
    public void PropertyGrid_CommandsVisible_Get_ShouldBeFalseByDefault()
    {
        _propertyGrid.CommandsVisible.Should().BeFalse();
    }

    [WinFormsFact]
    public void PropertyGrid_CanShowCommands_Get_ShouldBeFalseByDefault()
    {
        _propertyGrid.CanShowCommands.Should().BeFalse();
    }

    [WinFormsFact]
    public void PropertyGrid_CanShowVisualStyleGlyphs_GetSet_ReturnsExpected()
    {
        _propertyGrid.CanShowVisualStyleGlyphs.Should().BeTrue();

        _propertyGrid.CanShowVisualStyleGlyphs = false;
        _propertyGrid.CanShowVisualStyleGlyphs.Should().BeFalse();
    }

    [WinFormsFact]
    public void PropertyGrid_AutoScroll_GetSet_ReturnsExpected()
    {
        _propertyGrid.AutoScroll.Should().BeFalse();

        _propertyGrid.AutoScroll = true;
        _propertyGrid.AutoScroll.Should().BeTrue();
    }

    [WinFormsFact]
    public void PropertyGrid_RefreshEvent_Raised_ShouldNotThrowException()
    {
        Action act = () => _propertyGrid.Refresh();
        act.Should().NotThrow();
    }

    [WinFormsFact]
    public void PropertyGrid_CommandsBorderColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.CommandsBorderColor.Should().Be(SystemColors.ControlDark);

        _propertyGrid.CommandsBorderColor = Color.Red;
        _propertyGrid.CommandsBorderColor.Should().Be(Color.Red);
    }

    [WinFormsFact]
    public void PropertyGrid_HelpBorderColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.HelpBorderColor.Should().Be(SystemColors.ControlDark);

        _propertyGrid.HelpBorderColor = Color.Blue;
        _propertyGrid.HelpBorderColor.Should().Be(Color.Blue);
    }

    [WinFormsFact]
    public void PropertyGrid_SelectedItemWithFocusBackColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.SelectedItemWithFocusBackColor.Should().Be(SystemColors.Highlight);

        _propertyGrid.SelectedItemWithFocusBackColor = Color.Green;
        _propertyGrid.SelectedItemWithFocusBackColor.Should().Be(Color.Green);
    }

    [WinFormsFact]
    public void PropertyGrid_SelectedItemWithFocusForeColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.SelectedItemWithFocusForeColor.Should().Be(SystemColors.HighlightText);

        _propertyGrid.SelectedItemWithFocusForeColor = Color.Red;
        _propertyGrid.SelectedItemWithFocusForeColor.Should().Be(Color.Red);
    }

    [WinFormsFact]
    public void PropertyGrid_DisabledItemForeColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.DisabledItemForeColor.Should().Be(SystemColors.GrayText);

        _propertyGrid.DisabledItemForeColor = Color.Blue;
        _propertyGrid.DisabledItemForeColor.Should().Be(Color.Blue);
    }

    [WinFormsFact]
    public void PropertyGrid_CategorySplitterColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.CategorySplitterColor.Should().Be(SystemColors.Control);

        _propertyGrid.CategorySplitterColor = Color.Green;
        _propertyGrid.CategorySplitterColor.Should().Be(Color.Green);
    }

    [WinFormsFact]
    public void PropertyGrid_ForeColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.ForeColor.Should().Be(SystemColors.ControlText);

        _propertyGrid.ForeColor = Color.Red;
        _propertyGrid.ForeColor.Should().Be(Color.Red);
    }

    [WinFormsFact]
    public void PropertyGrid_BackgroundImageLayout_GetSet_ReturnsExpected()
    {
        _propertyGrid.BackgroundImageLayout.Should().Be(ImageLayout.Tile);

        _propertyGrid.BackgroundImageLayout = ImageLayout.Center;
        _propertyGrid.BackgroundImageLayout.Should().Be(ImageLayout.Center);
    }

    [WinFormsFact]
    public void PropertyGrid_BackgroundImage_GetSet_ReturnsExpected()
    {
        _propertyGrid.BackgroundImage.Should().BeNull();

        using Image newImage = new Bitmap(100, 100);
        _propertyGrid.BackgroundImage = newImage;
        _propertyGrid.BackgroundImage.Should().Be(newImage);
    }

    [WinFormsFact]
    public void PropertyGrid_BackColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.BackColor.Should().Be(SystemColors.Control);

        _propertyGrid.BackColor = Color.Blue;
        _propertyGrid.BackColor.Should().Be(Color.Blue);
    }
}
