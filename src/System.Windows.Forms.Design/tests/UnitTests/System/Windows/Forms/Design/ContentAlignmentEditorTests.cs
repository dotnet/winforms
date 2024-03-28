// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Design;
using System.Reflection;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Design.Tests;

public class ContentAlignmentEditorTests
{
    [Theory]
    [InlineData("_topLeft")]
    [InlineData("_topCenter")]
    [InlineData("_topRight")]
    [InlineData("_middleLeft")]
    [InlineData("_middleCenter")]
    [InlineData("_middleRight")]
    [InlineData("_bottomLeft")]
    [InlineData("_bottomCenter")]
    [InlineData("_bottomRight")]
    public void ContentAlignmentEditor_ContentAlignmentEditor_ContentUI_IsRadioButton(string fieldName)
    {
        ContentAlignmentEditor editor = new();
        Type type = editor.GetType()
            .GetNestedType("ContentUI", BindingFlags.NonPublic | BindingFlags.Instance);
        var contentUI = (Control)Activator.CreateInstance(type);
        var item = (Control)contentUI.GetType()
            .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(contentUI);

        var actual = (UIA_CONTROLTYPE_ID)(int)item.AccessibilityObject.TestAccessor().Dynamic
            .GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_RadioButtonControlTypeId, actual);
    }
}
