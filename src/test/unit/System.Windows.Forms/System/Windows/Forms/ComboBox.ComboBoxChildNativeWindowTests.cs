// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

namespace System.Windows.Forms.Tests;

public class ComboBox_ComboBoxChildNativeWindowTests
{
    [WinFormsFact]
    public void ComboBoxChildNativeWindow_GetChildAccessibleObject()
    {
        using ComboBox comboBox = new() { DropDownStyle = ComboBoxStyle.DropDown };
        comboBox.CreateControl();

        var childNativeWindow = comboBox.GetListNativeWindow();
        Type childWindowTypeEnum = typeof(ComboBox).GetNestedType("ChildWindowType", Reflection.BindingFlags.NonPublic);

        foreach (object childWindowType in Enum.GetValues(childWindowTypeEnum))
        {
            childNativeWindow.TestAccessor.Dynamic._childWindowType = childWindowType;
            Assert.True(childNativeWindow.TestAccessor.Dynamic.GetChildAccessibleObject() is ComboBox.ChildAccessibleObject);
        }
    }

    [WinFormsFact]
    public void ComboBox_DropDownHeight_UpdatesAfterItemsClear()
    {
        using ComboBox comboBox = new() { DropDownStyle = ComboBoxStyle.DropDown, ItemHeight = 20 };
        comboBox.Items.Add("Item 1");
        comboBox.Items.Add("Item 2");
        comboBox.Items.Add("Item 3");
        comboBox.CreateControl();
        comboBox.DroppedDown = true;

        var childNativeWindow = comboBox.GetListNativeWindow();
        Assert.NotNull(childNativeWindow);
        PInvokeCore.GetWindowRect(childNativeWindow.HWND, out RECT rect);
        int heightBeforeClear = rect.Height;
        comboBox.DroppedDown = false;

        comboBox.Items.Clear();
        comboBox.DroppedDown = true;
        PInvokeCore.GetWindowRect(childNativeWindow.HWND, out RECT rect1);
        int heightAfterClear = rect1.Height;
        comboBox.DroppedDown = false;

        bool heightReduced = heightBeforeClear > heightAfterClear;
        Assert.True(heightReduced);
        Assert.NotEqual(heightBeforeClear, heightAfterClear);

        comboBox.Items.Add("Item 1");
        comboBox.Items.Add("Item 2");
        comboBox.DroppedDown = true;
        PInvokeCore.GetWindowRect(childNativeWindow.HWND, out RECT rect2);
        int heightAfterAdd = rect2.Height;

        bool heightIncreased = heightAfterAdd > heightAfterClear;
        Assert.True(heightIncreased);
        Assert.NotEqual(heightAfterAdd, heightAfterClear);
    }

    [WinFormsFact]
    public void ComboBox_DropDownHeight_UpdatesAfterRemovingAllItems()
    {
        using ComboBox comboBox = new() { DropDownStyle = ComboBoxStyle.DropDown, ItemHeight = 20 };
        comboBox.Items.Add("Item 1");
        comboBox.Items.Add("Item 2");
        comboBox.Items.Add("Item 3");
        comboBox.CreateControl();
        comboBox.DroppedDown = true;

        var childNativeWindow = comboBox.GetListNativeWindow();
        Assert.NotNull(childNativeWindow);
        PInvokeCore.GetWindowRect(childNativeWindow.HWND, out RECT rect);
        int heightBeforeClear = rect.Height;
        comboBox.DroppedDown = false;

        for (int i = 0; i < 3; i++)
        {
            comboBox.Items.RemoveAt(0);
        }

        comboBox.DroppedDown = true;
        PInvokeCore.GetWindowRect(childNativeWindow.HWND, out RECT rect1);
        int heightAfterClear = rect1.Height;
        comboBox.DroppedDown = false;

        bool heightReduced = heightBeforeClear > heightAfterClear;
        Assert.True(heightReduced);
        Assert.NotEqual(heightBeforeClear, heightAfterClear);
    }
}
