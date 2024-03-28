// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.DataGridViewTextBoxEditingControl;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class DataGridViewTextBoxEditingControl_DataGridViewTextBoxEditingControlAccessibleObjectTests
{
    [WinFormsFact]
    public void DataGridViewTextBoxEditingControlAccessibleObject_Ctor_Default()
    {
        using DataGridViewTextBoxEditingControl textCellControl = new();
        DataGridViewTextBoxEditingControlAccessibleObject accessibleObject = new(textCellControl);
        Assert.Equal(textCellControl, accessibleObject.Owner);
    }

    [WinFormsFact]
    public void DataGridViewTextBoxEditingControlAccessibleObject_Ctor_ThrowsException_IfOwnerIsNull()
    {
        using DataGridViewTextBoxEditingControl textCellControl = new();
        Assert.Throws<ArgumentNullException>(() => new DataGridViewTextBoxEditingControlAccessibleObject(null));
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewTextBoxEditingControlAccessibleObject_IsReadOnly_IsExpected(bool readOnly)
    {
        using DataGridViewTextBoxEditingControl textCellControl = new();
        textCellControl.ReadOnly = readOnly;
        AccessibleObject accessibleObject = textCellControl.AccessibilityObject;
        Assert.Equal(readOnly, accessibleObject.IsReadOnly);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsTextPatternAvailablePropertyId)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsTextPattern2AvailablePropertyId)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsValuePatternAvailablePropertyId)]
    public void DataGridViewTextBoxEditingControlAccessibleObject_GetPropertyValue_PatternsSuported(int propertyID)
    {
        using DataGridViewTextBoxEditingControl textCellControl = new();
        AccessibleObject accessibleObject = textCellControl.AccessibilityObject;
        Assert.True((bool)accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyID));
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_ValuePatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_TextPatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_TextPattern2Id)]
    public void DataGridViewTextBoxEditingControlAccessibleObject_IsPatternSupported_PatternsSuported(int patternId)
    {
        using DataGridViewTextBoxEditingControl textCellControl = new();
        AccessibleObject accessibleObject = textCellControl.AccessibilityObject;
        Assert.True(accessibleObject.IsPatternSupported((UIA_PATTERN_ID)patternId));
    }

    [WinFormsFact]
    public void DataGridViewTextBoxEditingControlAccessibleObject_ControlType_IsEdit_IfAccessibleRoleIsDefault()
    {
        using DataGridViewTextBoxEditingControl textCellControl = new();
        // AccessibleRole is not set = Default

        var actual = (UIA_CONTROLTYPE_ID)(int)textCellControl.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_EditControlTypeId, actual);
        Assert.False(textCellControl.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.Text)]
    [InlineData(false, AccessibleRole.None)]
    public void DataGridViewTextBoxEditingControlAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
    {
        using DataGridViewTextBoxEditingControl textCellControl = new();
        // AccessibleRole is not set = Default

        if (createControl)
        {
            textCellControl.CreateControl();
        }

        object actual = textCellControl.AccessibilityObject.Role;

        Assert.Equal(expectedRole, actual);
        Assert.Equal(createControl, textCellControl.IsHandleCreated);
    }

    public static IEnumerable<object[]> DataGridViewTextBoxEditingControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
    {
        Array roles = Enum.GetValues(typeof(AccessibleRole));

        foreach (AccessibleRole role in roles)
        {
            if (role == AccessibleRole.Default)
            {
                continue; // The test checks custom roles
            }

            yield return new object[] { role };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DataGridViewTextBoxEditingControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void DataGridViewTextBoxEditingControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using DataGridViewTextBoxEditingControl textCellControl = new();
        textCellControl.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)textCellControl.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(textCellControl.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)NavigateDirection.NavigateDirection_NextSibling)]
    [InlineData((int)NavigateDirection.NavigateDirection_PreviousSibling)]
    [InlineData((int)NavigateDirection.NavigateDirection_FirstChild)]
    [InlineData((int)NavigateDirection.NavigateDirection_LastChild)]
    public void DataGridViewTextBoxEditingControlAccessibleObject_FragmentNavigate_SiblingsAndChildrenAreNull(int direction)
    {
        using DataGridViewTextBoxEditingControl control = new();

        object actual = control.AccessibilityObject.FragmentNavigate((NavigateDirection)direction);

        Assert.Null(actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTextBoxEditingControlAccessibleObject_FragmentNavigate_ParentIsCell()
    {
        using DataGridView control = new();

        Assert.True(control.AccessibilityObject is Control.ControlAccessibleObject);

        control.Columns.Add(new DataGridViewTextBoxColumn());
        control.Rows.Add();

        control.CreateControl();
        control.CurrentCell = control.Rows[0].Cells[0];
        control.BeginEdit(false);

        object actual = control.EditingControlAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent);

        control.EndEdit();

        Assert.Null(control.EditingControl);
        Assert.Equal(control.CurrentCell.AccessibilityObject, actual);
        Assert.True(control.IsHandleCreated);
    }
}
