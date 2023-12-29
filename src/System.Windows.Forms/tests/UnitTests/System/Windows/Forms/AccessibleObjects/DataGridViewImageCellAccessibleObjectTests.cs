// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class DataGridViewImageCellAccessibleObjectTests : DataGridViewImageCell
{
    [WinFormsFact]
    public void DataGridViewImageCellAccessibleObject_Ctor_Default()
    {
        DataGridViewImageCellAccessibleObject accessibleObject = new(null);

        Assert.Null(accessibleObject.Owner);
        Assert.Equal(AccessibleRole.Cell, accessibleObject.Role);
    }

    [WinFormsFact]
    public void DataGridViewImageCellAccessibleObject_DefaultAction_ReturnsExpected()
    {
        DataGridViewImageCellAccessibleObject accessibleObject = new(null);

        Assert.Equal(string.Empty, accessibleObject.DefaultAction);
    }

    [WinFormsFact]
    public void DataGridViewImageCellAccessibleObject_GetChildCount_Default()
    {
        DataGridViewImageCellAccessibleObject accessibleObject = new(null);

        Assert.Equal(0, accessibleObject.GetChildCount());
    }

    [WinFormsFact]
    public void DataGridViewImageCellAccessibleObject_IsIAccessibleExSupported_ReturnsExpected()
    {
        DataGridViewImageCellAccessibleObject accessibleObject = new(null);

        Assert.True(accessibleObject.IsIAccessibleExSupported());
    }

    [WinFormsFact]
    public void DataGridViewImageCellAccessibleObject_ControlType_ReturnsExpected()
    {
        DataGridViewImageCellAccessibleObject accessibleObject = new(null);

        UIA_CONTROLTYPE_ID expected = UIA_CONTROLTYPE_ID.UIA_ImageControlTypeId;
        Assert.Equal(expected, (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId));
    }

    [WinFormsFact]
    public void DataGridViewImageCellAccessibleObject_IsInvokePatternAvailable_ReturnsExpected()
    {
        DataGridViewImageCellAccessibleObject accessibleObject = new(null);

        Assert.True((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsInvokePatternAvailablePropertyId));
    }

    [WinFormsTheory]
    [InlineData(((int)UIA_PATTERN_ID.UIA_InvokePatternId))]
    public void DataGridViewImageCellAccessibleObject_IsPatternSupported_ReturnsExpected(int patternId)
    {
        DataGridViewImageCellAccessibleObject accessibleObject = new(null);

        Assert.True(accessibleObject.IsPatternSupported((UIA_PATTERN_ID)patternId));
    }

    [WinFormsFact]
    public void DataGridViewImageCellAccessibleObject_Description_IsNull_IfOwnerIsNotImageCell()
    {
        DataGridViewImageCellAccessibleObject accessibleObject = new(null);

        Assert.Null(accessibleObject.Description);
    }

    [WinFormsFact]
    public void DataGridViewImageCellAccessibleObject_Description_ReturnsExpected()
    {
        string testDescription = "This is a test description string";
        using DataGridViewImageCell cell = new();

        cell.Description = testDescription;

        Assert.Equal(testDescription, cell.AccessibilityObject.Description);
    }

    [WinFormsFact]
    public void DataGridViewImageCellAccessibleObject_DoDefaultAction_ThrowsException_IfOwnerIsNull()
    {
        Assert.Throws<InvalidOperationException>(() =>
        new DataGridViewImageCellAccessibleObject(null).DoDefaultAction());
    }
}
