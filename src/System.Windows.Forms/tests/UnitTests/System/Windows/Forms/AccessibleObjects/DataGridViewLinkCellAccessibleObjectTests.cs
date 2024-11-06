// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class DataGridViewLinkCellAccessibleObjectTests : DataGridViewLinkCell
{
    [WinFormsFact]
    public void DataGridViewLinkCellAccessibleObject_Ctor_Default()
    {
        DataGridViewLinkCellAccessibleObject accessibleObject = new(null);

        Assert.Null(accessibleObject.Owner);
        Assert.Equal(AccessibleRole.Cell, accessibleObject.Role);
    }

    [WinFormsFact]
    public void DataGridViewLinkCellAccessibleObject_DefaultAction_ReturnsExpected()
    {
        DataGridViewLinkCellAccessibleObject accessibleObject = new(null);

        Assert.Equal(SR.DataGridView_AccLinkCellDefaultAction, accessibleObject.DefaultAction);
    }

    [WinFormsFact]
    public void DataGridViewLinkCellAccessibleObject_GetChildCount_ReturnsExpected()
    {
        DataGridViewLinkCellAccessibleObject accessibleObject = new(null);

        Assert.Equal(0, accessibleObject.GetChildCount());
    }

    [WinFormsFact]
    public void DataGridViewLinkCellAccessibleObject_IsIAccessibleExSupported_ReturnsExpected()
    {
        DataGridViewLinkCellAccessibleObject accessibleObject = new(null);

        Assert.True(accessibleObject.IsIAccessibleExSupported());
    }

    [WinFormsFact]
    public void DataGridViewLinkCellAccessibleObject_ControlType_ReturnsExpected()
    {
        DataGridViewLinkCellAccessibleObject accessibleObject = new(null);

        UIA_CONTROLTYPE_ID expected = UIA_CONTROLTYPE_ID.UIA_HyperlinkControlTypeId;

        Assert.Equal(expected, (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId));
    }

    [WinFormsFact]
    public void DataGridViewLinkCellAccessibleObject_DoDefaultAction_ThrowsException_IfOwnerIsNull()
    {
        Assert.Throws<InvalidOperationException>(() =>
        new DataGridViewLinkCellAccessibleObject(null).DoDefaultAction());
    }

    [WinFormsFact]
    public void DataGridViewLinkCellAccessibleObject_DoDefaultAction_ThrowsException_IfRowIndexIsIncorrect()
    {
        using DataGridViewLinkCell cell = new();

        Assert.Equal(-1, cell.RowIndex);
        Assert.Throws<InvalidOperationException>(cell.AccessibilityObject.DoDefaultAction);
    }
}
