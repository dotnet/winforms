// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using static System.Windows.Forms.PropertyGridInternal.PropertyGridView;

namespace System.Windows.Forms.PropertyGridInternal.Tests;

public class PropertyGridView_DropDownHolderTests
{
    [WinFormsFact]
    public void DropDownHolder_AccessibilityObject_Constructor_initializes_correctly()
    {
        using PropertyGridView propertyGridView = new(null, null);
        propertyGridView.BackColor = Color.Green;
        using DropDownHolder dropDownHolder = new(propertyGridView);

        Assert.Equal(Color.Green, dropDownHolder.BackColor);
    }

    [WinFormsFact]
    public void DropDownHolder_SupportsUiaProviders_returns_true()
    {
        using PropertyGridView propertyGridView = new(null, null);
        using DropDownHolder dropDownHolder = new(propertyGridView);
        Assert.True(dropDownHolder.SupportsUiaProviders);
    }

    [WinFormsFact]
    public void DropDownHolder_CreateAccessibilityObject_creates_DropDownHolderAccessibleObject()
    {
        using PropertyGrid propertyGrid = new();
        PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;
        using DropDownHolder dropDownHolder = new(propertyGridView);

        AccessibleObject accessibleObject = dropDownHolder.AccessibilityObject;
        Assert.Equal("DropDownHolderAccessibleObject", accessibleObject.GetType().Name);
    }

    [WinFormsFact]
    public void DropDownHolder_SetDropDownControl_control_notnull()
    {
        using PropertyGrid propertyGrid = new();
        PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;
        using DropDownHolder dropDownHolder = new(propertyGridView);
        dropDownHolder.Visible = true;

        using GridViewListBox listBox = new(propertyGridView);
        dropDownHolder.SetDropDownControl(listBox, resizable: false);

        // Verify the control's Dock style, visibility, and enabled state.
        Assert.Equal(DockStyle.Fill, listBox.Dock);
        Assert.True(listBox.Visible);
        Assert.True(dropDownHolder.Enabled);
    }

    [WinFormsFact]
    public void DropDownHolder_SetDropDownControl_control_null()
    {
        using PropertyGrid propertyGrid = new();
        PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;
        using DropDownHolder dropDownHolder = new(propertyGridView);

        dropDownHolder.SetDropDownControl(null, resizable: false);
        Assert.False(dropDownHolder.Enabled);
    }

    [WinFormsFact]
    public void DropDownHolder_SetDropDownControl_Control_Height_verify()
    {
        using PropertyGrid propertyGrid = new();
        PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;
        using DropDownHolder dropDownHolder = new(propertyGridView);
        using GridViewListBox listBox = new(propertyGridView);

        // Compare the height of the item with the default height of the control,
        // The control display height is displayed according to the higher one.
        listBox.Height = 100;
        listBox.ItemHeight = 16;
        dropDownHolder.SetDropDownControl(listBox, resizable: false);
        Assert.Equal(100, listBox.Height);

        listBox.ItemHeight = 200;
        dropDownHolder.SetDropDownControl(listBox, resizable: false);
        Assert.Equal(listBox.ItemHeight, listBox.Height);
    }

    [WinFormsFact]
    public void DropDownHolder_SetDropDownControl_resizable_true()
    {
        using PropertyGrid propertyGrid = new();
        PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;
        using DropDownHolder dropDownHolder = new(propertyGridView);
        using GridViewListBox listBox = new(propertyGridView);

        listBox.Height = 100;
        int DropDownHolderBorder = 1;
        int resizeBarSize = SystemInformation.HorizontalScrollBarHeight + 1;

        // Resize grip appear at the top left.
        dropDownHolder.ResizeUp = true;
        dropDownHolder.SetDropDownControl(listBox, resizable: true);

        Assert.Equal(resizeBarSize, dropDownHolder.DockPadding.Top);
        Assert.Equal(0, dropDownHolder.DockPadding.Bottom);

        // Resize grip appear at the bottom left.
        dropDownHolder.ResizeUp = false;
        dropDownHolder.SetDropDownControl(listBox, resizable: true);

        Assert.Equal(resizeBarSize, dropDownHolder.DockPadding.Bottom);
        Assert.Equal(0, dropDownHolder.DockPadding.Top);
        Assert.Equal(listBox.Height + 2 * DropDownHolderBorder + resizeBarSize, dropDownHolder.Size.Height);
    }
}
