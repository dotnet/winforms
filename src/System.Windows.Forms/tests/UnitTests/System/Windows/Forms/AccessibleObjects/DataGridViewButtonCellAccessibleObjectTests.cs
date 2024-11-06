// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class DataGridViewButtonCellAccessibleObjectTests : DataGridViewButtonCell
{
    [WinFormsFact]
    public void DataGridViewButtonCellAccessibleObject_Ctor_Default()
    {
        DataGridViewButtonCellAccessibleObject accessibleObject = new(null);
        Assert.Null(accessibleObject.Owner);
        Assert.Equal(AccessibleRole.Cell, accessibleObject.Role);
    }

    [WinFormsFact]
    public void DataGridViewButtonCellAccessibleObject_DefaultAction_ReturnsExpected()
    {
        DataGridViewButtonCellAccessibleObject accessibleObject = new(null);
        Assert.Equal(SR.DataGridView_AccButtonCellDefaultAction, accessibleObject.DefaultAction);
    }

    [WinFormsFact]
    public void DataGridViewButtonCellAccessibleObject_GetPropertyValue_LegacyIAccessibleDefaultActionPropertyId_ReturnsExpected()
    {
        DataGridViewButtonCellAccessibleObject accessibleObject = new(null);
        Assert.Equal(SR.DataGridView_AccButtonCellDefaultAction, ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleDefaultActionPropertyId)).ToStringAndFree());
    }

    [WinFormsFact]
    public void DataGridViewButtonCellAccessibleObject_GetChildCount_ReturnsExpected()
    {
        DataGridViewButtonCellAccessibleObject accessibleObject = new(null);
        Assert.Equal(0, accessibleObject.GetChildCount());
    }

    [WinFormsFact]
    public void DataGridViewButtonCellAccessibleObject_IsIAccessibleExSupported_ReturnsExpected()
    {
        DataGridViewButtonCellAccessibleObject accessibleObject = new(null);
        Assert.True(accessibleObject.IsIAccessibleExSupported());
    }

    [WinFormsFact]
    public void DataGridViewButtonCellAccessibleObject_ControlType_ReturnsExpected()
    {
        DataGridViewButtonCellAccessibleObject accessibleObject = new(null);
        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId, (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId));
    }

    [WinFormsFact]
    public void DataGridViewButtonCellAccessibleObject_DoDefaultAction_ThrowsException_IfOwnerIsNull()
    {
        Assert.Throws<InvalidOperationException>(()
            => new DataGridViewButtonCellAccessibleObject(null).DoDefaultAction());
    }

    [WinFormsFact]
    public void DataGridViewButtonCellAccessibleObject_DoDefaultAction_ThrowsException_IfOwnerRowIndexIncorrect()
    {
        using DataGridViewCell cell = new DataGridViewButtonCell();
        AccessibleObject accessibleObject = cell.AccessibilityObject;

        Assert.Equal(-1, cell.RowIndex);
        Assert.Throws<InvalidOperationException>(accessibleObject.DoDefaultAction);
    }
}
