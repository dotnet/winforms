// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class DataGridViewRowHeaderCellAccessibleObjectTests : DataGridViewRowHeaderCell
{
    [WinFormsFact]
    public void DataGridViewRowHeaderCellAccessibleObject_Ctor_Default()
    {
        DataGridViewRowHeaderCellAccessibleObject accessibleObject = new(null);
        Assert.Null(accessibleObject.Owner);
    }

    [WinFormsFact]
    public void DataGridViewRowHeaderCellAccessibleObject_Parent_ThrowException_IfOwnerIsNull()
    {
        Assert.Throws<InvalidOperationException>(() => new DataGridViewRowHeaderCellAccessibleObject(null).Parent);
    }

    [WinFormsFact]
    public void DataGridViewRowHeaderCellAccessibleObject_Parent_ReturnsExpected()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");
        DataGridViewRowHeaderCell cell = control.Rows[0].HeaderCell;

        var accessibleObject = (DataGridViewRowHeaderCellAccessibleObject)cell.AccessibilityObject;

        Assert.Equal(cell.OwningRow.AccessibilityObject, accessibleObject.Parent);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowHeaderCellAccessibleObject_Value_ReturnsExpected()
    {
        DataGridViewRowHeaderCellAccessibleObject accessibleObject = new(null);
        Assert.Equal(string.Empty, accessibleObject.Value);
    }

    [WinFormsFact]
    public void DataGridViewRowHeaderCellAccessibleObject_Role_ReturnsExpected()
    {
        DataGridViewRowHeaderCellAccessibleObject accessibleObject = new(null);
        Assert.Equal(AccessibleRole.RowHeader, accessibleObject.Role);
    }

    [WinFormsFact]
    public void DataGridViewRowHeaderCellAccessibleObject_Name_ReturnsExpected()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");
        DataGridViewRow row = control.Rows[0];

        Assert.Equal(row.AccessibilityObject.Name, row.HeaderCell.AccessibilityObject.Name);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> DataGridViewRowHeaderCellAccessibleObject_DefaultAction_TestData()
    {
        yield return new object[] { DataGridViewSelectionMode.FullRowSelect, SR.DataGridView_RowHeaderCellAccDefaultAction };
        yield return new object[] { DataGridViewSelectionMode.RowHeaderSelect, SR.DataGridView_RowHeaderCellAccDefaultAction };
        yield return new object[] { DataGridViewSelectionMode.CellSelect, string.Empty };
    }

    [WinFormsTheory]
    [MemberData(nameof(DataGridViewRowHeaderCellAccessibleObject_DefaultAction_TestData))]
    public void DataGridViewRowHeaderCellAccessibleObject_DefaultAction_ReturnsExpected(DataGridViewSelectionMode mode, string expected)
    {
        using DataGridView control = new();
        control.SelectionMode = mode;
        control.Columns.Add("Column 1", "Header text 1");

        var accessibleObject = (DataGridViewRowHeaderCellAccessibleObject)control.Rows[0].HeaderCell.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.DefaultAction);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(DataGridViewSelectionMode.FullRowSelect, true)]
    [InlineData(DataGridViewSelectionMode.RowHeaderSelect, true)]
    [InlineData(DataGridViewSelectionMode.CellSelect, false)]
    public void DataGridViewRowHeaderCellAccessibleObject_DoDefaultAction_WorksExpected(DataGridViewSelectionMode mode, bool expected)
    {
        using DataGridView control = new();
        control.SelectionMode = mode;
        control.Columns.Add("Column 1", "Header text 1");
        DataGridViewRowHeaderCell cell = control.Rows[0].HeaderCell;
        control.CreateControl();

        Assert.False(cell.OwningRow.Selected);

        cell.AccessibilityObject.DoDefaultAction();

        Assert.Equal(expected, cell.OwningRow.Selected);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(DataGridViewSelectionMode.FullRowSelect)]
    [InlineData(DataGridViewSelectionMode.RowHeaderSelect)]
    [InlineData(DataGridViewSelectionMode.CellSelect)]
    public void DataGridViewRowHeaderCellAccessibleObject_DoDefaultAction_DoesNothing_IfHandleIsNotCreated(DataGridViewSelectionMode mode)
    {
        using DataGridView control = new();
        control.SelectionMode = mode;
        control.Columns.Add("Column 1", "Header text 1");
        DataGridViewRowHeaderCell cell = control.Rows[0].HeaderCell;

        Assert.False(cell.OwningRow.Selected);

        cell.AccessibilityObject.DoDefaultAction();

        Assert.False(cell.OwningRow.Selected);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowHeaderCellAccessibleObject_Bounds_IsNotEmptyRectangle()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");

        var accessibleObject = (DataGridViewRowHeaderCellAccessibleObject)control.Rows[0].HeaderCell.AccessibilityObject;

        Assert.NotEqual(Rectangle.Empty, accessibleObject.Bounds);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewRowHeaderCellAccessibleObject_FragmentNavigate_Parent_ReturnsExpected(bool createControl)
    {
        using DataGridView control = CreateDataGridView(columnCount: 1, createControl);
        DataGridViewRow row = control.Rows[0];

        var accessibleObject = (DataGridViewRowHeaderCellAccessibleObject)row.HeaderCell.AccessibilityObject;
        AccessibleObject expected = row.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));

        Assert.Equal(createControl, control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowHeaderCellAccessibleObject_FragmentNavigate_Parent_ReturnsNull_IfOwningRowIsNull()
    {
        DataGridViewRowHeaderCellAccessibleObject accessibleObject = new(new());

        Assert.Null(accessibleObject.Owner.OwningRow);
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewRowHeaderCellAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected(bool createControl)
    {
        using DataGridView control = CreateDataGridView(columnCount: 1, createControl);
        DataGridViewRow row = control.Rows[0];

        AccessibleObject accessibleObject = (DataGridViewRowHeaderCellAccessibleObject)row.HeaderCell.AccessibilityObject;
        AccessibleObject expected = row.Cells[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(createControl, control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewRowHeaderCellAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected_IfFirstColumnHidden(bool createControl)
    {
        using DataGridView control = CreateDataGridView(columnCount: 2, createControl);
        control.Columns[0].Visible = false;
        DataGridViewRow row = control.Rows[0];

        AccessibleObject accessibleObject = (DataGridViewRowHeaderCellAccessibleObject)row.HeaderCell.AccessibilityObject;
        AccessibleObject expected = row.Cells[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(createControl, control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewRowHeaderCellAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected_IfCustomOrder(bool createControl)
    {
        using DataGridView control = CreateDataGridView(columnCount: 2, createControl);
        control.Columns[0].DisplayIndex = 1;
        control.Columns[1].DisplayIndex = 0;
        DataGridViewRow row = control.Rows[0];

        AccessibleObject accessibleObject = (DataGridViewRowHeaderCellAccessibleObject)row.HeaderCell.AccessibilityObject;
        AccessibleObject expected = row.Cells[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(createControl, control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewRowHeaderCellAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected_IfCustomOrderAndFirstDisplayedColumnHidden(bool createControl)
    {
        using DataGridView control = CreateDataGridView(columnCount: 2, createControl);
        control.Columns[0].DisplayIndex = 1;
        control.Columns[1].DisplayIndex = 0;
        control.Columns[1].Visible = false;
        DataGridViewRow row = control.Rows[0];

        AccessibleObject accessibleObject = (DataGridViewRowHeaderCellAccessibleObject)row.HeaderCell.AccessibilityObject;
        AccessibleObject expected = row.Cells[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(createControl, control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewRowHeaderCellAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected(bool createControl)
    {
        using DataGridView control = CreateDataGridView(columnCount: 1, createControl);
        DataGridViewRow row = control.Rows[0];

        var accessibleObject = (DataGridViewRowHeaderCellAccessibleObject)row.HeaderCell.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(createControl, control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewRowHeaderCellAccessibleObject_FragmentNavigate_Child_ReturnsNull(bool createControl)
    {
        using DataGridView control = CreateDataGridView(columnCount: 1, createControl);
        DataGridViewRow row = control.Rows[0];

        var accessibleObject = (DataGridViewRowHeaderCellAccessibleObject)row.HeaderCell.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Equal(createControl, control.IsHandleCreated);
    }

    private DataGridView CreateDataGridView(int columnCount, bool createControl)
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
