﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;
using static System.Windows.Forms.ListViewItem;

namespace System.Windows.Forms.Tests;

public class ListViewItem_ListViewItemImageAccessibleObjectTests
{
    [WinFormsFact]
    public void ListViewItemImageAccessibleObject_GetChild_ReturnCorrectValue()
    {
        using ImageList imageCollection = new();
        imageCollection.Images.Add(Form.DefaultIcon);

        ListViewItem listViewItem = new("Test", 0);
        using ListView list = new()
        {
            View = View.Details,
            SmallImageList = imageCollection
        };
        using ColumnHeader column = new();
        list.Columns.Add(column);
        list.Items.Add(listViewItem);

        list.CreateControl();

        AccessibleObject imageAccessibleObject = listViewItem.AccessibilityObject.GetChild(0);

        Assert.NotNull(imageAccessibleObject);
        Assert.IsType<ListViewItemImageAccessibleObject>(imageAccessibleObject);
        Assert.True(list.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewItemImageAccessibleObject_GetPropertyValue_ReturnsExpected()
    {
        using ImageList imageCollection = new();
        imageCollection.Images.Add(Form.DefaultIcon);

        ListViewItem listViewItem = new("Test", 0);

        using ListView list = new()
        {
            View = View.Details,
            SmallImageList = imageCollection
        };
        using ColumnHeader column = new();
        list.Columns.Add(column);
        list.Items.Add(listViewItem);

        list.CreateControl();

        AccessibleObject imageAccessibleObject = listViewItem.AccessibilityObject.GetChild(0);

        Assert.Equal(UiaCore.UIA.ImageControlTypeId, (UiaCore.UIA)imageAccessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
        Assert.False((bool)imageAccessibleObject.GetPropertyValue(UiaCore.UIA.HasKeyboardFocusPropertyId));
        Assert.False((bool)imageAccessibleObject.GetPropertyValue(UiaCore.UIA.IsKeyboardFocusablePropertyId));
    }
}
