// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class DataGridViewTopLeftHeaderCellAccessibleObjectTests : DataGridViewTopLeftHeaderCell
{
    [WinFormsFact]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_Ctor_Default()
    {
        DataGridViewTopLeftHeaderCellAccessibleObject accessibleObject = new(null);

        Assert.Null(accessibleObject.Owner);
    }

    [WinFormsFact]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_Bounds_ThrowsException_IfOwnerIsNull()
    {
        Assert.Throws<InvalidOperationException>(() =>
            new DataGridViewTopLeftHeaderCellAccessibleObject(null).Bounds);
    }

    [WinFormsFact]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_Bounds_ReturnsEmptyRectangle_IfDataGridViewIsNull()
    {
        using DataGridViewTopLeftHeaderCell cell = new();

        Assert.Null(cell.DataGridView);
        Assert.Equal(Rectangle.Empty, cell.AccessibilityObject.Bounds);
    }

    [WinFormsFact]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_Bounds_ReturnsEmptyRectangle_IfDataGridViewIsNotCreated()
    {
        using DataGridView control = new();
        using DataGridViewTopLeftHeaderCell cell = new();

        control.TopLeftHeaderCell = cell;

        Assert.Equal(Rectangle.Empty, cell.AccessibilityObject.Bounds);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_Bounds_ReturnsExpected()
    {
        using DataGridView control = new();
        using DataGridViewTopLeftHeaderCell cell = new();

        control.TopLeftHeaderCell = cell;
        control.CreateControl();

        Assert.NotEqual(Rectangle.Empty, cell.AccessibilityObject.Bounds);
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> DataGridViewTopLeftHeaderCellAccessibleObject_DefaultAction_TestData()
    {
        yield return new object[] { true, SR.DataGridView_AccTopLeftColumnHeaderCellDefaultAction };
        yield return new object[] { false, string.Empty };
    }

    [WinFormsTheory]
    [MemberData(nameof(DataGridViewTopLeftHeaderCellAccessibleObject_DefaultAction_TestData))]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_DefaultAction_ReturnsExpected(bool isMultiSelect, string expected)
    {
        using DataGridView control = new();
        using DataGridViewTopLeftHeaderCell cell = new();

        control.TopLeftHeaderCell = cell;
        control.MultiSelect = isMultiSelect;

        Assert.Equal(expected, cell.AccessibilityObject.DefaultAction);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_Value_ReturnsExpected()
    {
        using DataGridViewTopLeftHeaderCell cell = new();

        Assert.Equal(string.Empty, cell.AccessibilityObject.Value);
    }

    [WinFormsFact]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_ControlType_ReturnsExpected()
    {
        DataGridViewTopLeftHeaderCellAccessibleObject accessibleObject = new(null);

        UIA_CONTROLTYPE_ID expected = UIA_CONTROLTYPE_ID.UIA_HeaderControlTypeId;

        Assert.Equal(expected, (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId));
    }

    [WinFormsTheory]
    [BoolData]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_IsEnabled_ReturnsExpected(bool isEnabled)
    {
        using DataGridView control = new();
        using DataGridViewTopLeftHeaderCell cell = new();

        control.TopLeftHeaderCell = cell;
        control.Enabled = isEnabled;

        Assert.Equal(isEnabled, (bool)cell.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId));
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> DataGridViewTopLeftHeaderCellAccessibleObject_Name_TestData()
    {
        yield return new object[] { RightToLeft.No, SR.DataGridView_AccTopLeftColumnHeaderCellName };
        yield return new object[] { RightToLeft.Yes, SR.DataGridView_AccTopLeftColumnHeaderCellNameRTL };
    }

    [WinFormsTheory]
    [MemberData(nameof(DataGridViewTopLeftHeaderCellAccessibleObject_Name_TestData))]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_Name_ReturnsExpected(RightToLeft value, string expected)
    {
        using DataGridView control = new();
        using DataGridViewTopLeftHeaderCell cell = new();

        control.TopLeftHeaderCell = cell;
        control.RightToLeft = value;

        Assert.Equal(expected, cell.AccessibilityObject.Name);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_Name_ReturnsEmptyString_IfOwnerValueIsNotEmptySring()
    {
        using DataGridView control = new();
        using DataGridViewTopLeftHeaderCell cell = new();

        cell.Value = "It is not empty string";
        control.TopLeftHeaderCell = cell;

        Assert.Equal(string.Empty, cell.AccessibilityObject.Name);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_FragmentNavigate_Parent_ReturnsExpected(bool createControl)
    {
        using DataGridView control = CreateDataGridView(columnCount: 0, createControl);
        using DataGridViewTopLeftHeaderCell cell = new();

        control.TopLeftHeaderCell = cell;
        AccessibleObject expected = control.AccessibilityObject.GetChild(0);

        Assert.Equal(expected, cell.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));

        Assert.Equal(createControl, control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected(bool createControl)
    {
        using DataGridView control = CreateDataGridView(columnCount: 0, createControl);
        using DataGridViewTopLeftHeaderCell cell = new();

        control.TopLeftHeaderCell = cell;

        Assert.Null(cell.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(createControl, control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected(bool createControl)
    {
        using DataGridView control = CreateDataGridView(columnCount: 0, createControl);
        using DataGridViewTopLeftHeaderCell cell = new();

        control.TopLeftHeaderCell = cell;
        AccessibleObject expected = control.AccessibilityObject.GetChild(0)?.GetChild(1);

        Assert.Equal(expected, cell.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(createControl, control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_FragmentNavigate_NextSibling_ReturnsNull_IfDataGridViewHasNoVisibleCollumns(bool createControl)
    {
        using DataGridView control = CreateDataGridView(columnCount: 0, createControl);
        using DataGridViewTopLeftHeaderCell cell = new();

        control.TopLeftHeaderCell = cell;

        Assert.Null(cell.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(createControl, control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected_IfFirstColumnHidden(bool createControl)
    {
        using DataGridView control = CreateDataGridView(columnCount: 2, createControl);
        control.Columns[0].Visible = false;

        using DataGridViewTopLeftHeaderCell cell = new();
        control.TopLeftHeaderCell = cell;

        AccessibleObject expected = control.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(expected, cell.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(createControl, control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected_IfCustomOrder(bool createControl)
    {
        using DataGridView control = CreateDataGridView(columnCount: 2, createControl);
        control.Columns[0].DisplayIndex = 1;
        control.Columns[1].DisplayIndex = 0;

        using DataGridViewTopLeftHeaderCell cell = new();
        control.TopLeftHeaderCell = cell;

        AccessibleObject expected = control.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(expected, cell.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(createControl, control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected_IfCustomOrderAndFirstDisplayedColumnHidden(bool createControl)
    {
        using DataGridView control = CreateDataGridView(columnCount: 2, createControl);
        control.Columns[0].DisplayIndex = 1;
        control.Columns[1].DisplayIndex = 0;
        control.Columns[1].Visible = false;

        using DataGridViewTopLeftHeaderCell cell = new();
        control.TopLeftHeaderCell = cell;

        AccessibleObject expected = control.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(expected, cell.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(createControl, control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_FragmentNavigate_Child_ReturnsNull(bool createControl)
    {
        using DataGridView control = CreateDataGridView(columnCount: 0, createControl);

        using DataGridViewTopLeftHeaderCell cell = new();
        control.TopLeftHeaderCell = cell;

        Assert.Null(cell.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(cell.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Equal(createControl, control.IsHandleCreated);
    }

    private DataGridView CreateDataGridView(int columnCount, bool createControl = true)
    {
        DataGridView dataGridView = new();

        for (int i = 0; i < columnCount; i++)
        {
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        }

        if (createControl)
        {
            dataGridView.CreateControl();
        }

        return dataGridView;
    }
}
