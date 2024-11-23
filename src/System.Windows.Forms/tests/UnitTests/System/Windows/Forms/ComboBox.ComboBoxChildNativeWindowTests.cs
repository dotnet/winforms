// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
            childNativeWindow.TestAccessor().Dynamic._childWindowType = childWindowType;
            Assert.True(childNativeWindow.TestAccessor().Dynamic.GetChildAccessibleObject() is ComboBox.ChildAccessibleObject);
        }
    }
}
