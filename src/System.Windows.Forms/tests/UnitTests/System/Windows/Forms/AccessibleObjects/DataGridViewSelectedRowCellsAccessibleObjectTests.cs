// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class DataGridViewSelectedRowCellsAccessibleObjectTests
{
    [WinFormsFact]
    public void DataGridViewSelectedRowCellsAccessibleObject_Ctor_Default()
    {
        Type type = typeof(DataGridViewRow)
            .GetNestedType("DataGridViewSelectedRowCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, [null], null);

        Assert.Null(accessibleObject.TestAccessor().Dynamic._owningDataGridViewRow);
        Assert.Equal(AccessibleRole.Grouping, accessibleObject.Role);
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCellsAccessibleObject_Name_ReturnsExpected()
    {
        Type type = typeof(DataGridViewRow)
            .GetNestedType("DataGridViewSelectedRowCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, [null], null);

        Assert.Equal(SR.DataGridView_AccSelectedRowCellsName, accessibleObject.Name);
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCellsAccessibleObject_Value_ReturnsExpected()
    {
        Type type = typeof(DataGridViewRow)
            .GetNestedType("DataGridViewSelectedRowCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, [null], null);

        Assert.Equal(SR.DataGridView_AccSelectedRowCellsName, accessibleObject.Value);
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCellsAccessibleObject_State_ReturnsExpected()
    {
        Type type = typeof(DataGridViewRow)
            .GetNestedType("DataGridViewSelectedRowCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, [null], null);

        Assert.Equal(AccessibleStates.Selected | AccessibleStates.Selectable, accessibleObject.State);
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCellsAccessibleObject_Parent_ReturnsExpected()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");
        control.Rows.Add("Row1");
        DataGridViewRow row = control.Rows[0];
        Type type = typeof(DataGridViewRow)
            .GetNestedType("DataGridViewSelectedRowCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, [row], null);

        Assert.Equal(row.AccessibilityObject, accessibleObject.Parent);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCellsAccessibleObject_GetSelected_ReturnsExpected()
    {
        Type type = typeof(DataGridViewRow)
            .GetNestedType("DataGridViewSelectedRowCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, [null], null);

        Assert.Equal(accessibleObject, accessibleObject.GetSelected());
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCellsAccessibleObject_GetFocused_ReturnsExpected_IfCurrentCellIsSelected()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");
        control.Rows.Add("Row1");
        DataGridViewCell currentCell = control.Rows[0].Cells[0];
        control.CurrentCell = currentCell;
        currentCell.Selected = true;

        Type type = typeof(DataGridViewRow)
            .GetNestedType("DataGridViewSelectedRowCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, [control.Rows[0]], null);

        Assert.NotNull(currentCell);
        Assert.Equal(currentCell.AccessibilityObject, accessibleObject.GetFocused());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCellsAccessibleObject_GetFocused_ReturnsNull_IfCurrentCellIsNotSelected()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");
        DataGridViewCell currentCell = control.Rows[0].Cells[0];
        control.CurrentCell = currentCell;
        currentCell.Selected = false;

        Type type = typeof(DataGridViewRow)
            .GetNestedType("DataGridViewSelectedRowCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, [control.Rows[0]], null);

        Assert.NotNull(currentCell);
        Assert.Null(accessibleObject.GetFocused());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCellsAccessibleObject_GetChildCount_ReturnsExpected()
    {
        int selecetedCellsCount = 2;
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");
        control.Columns.Add("Column 2", "Header text 2");
        control.Columns.Add("Column 3", "Header text 3");
        control.Rows.Add("Row1");
        DataGridViewRow row = control.Rows[0];

        for (int i = 0; i < selecetedCellsCount; i++)
        {
            row.Cells[i].Selected = true;
        }

        Type type = typeof(DataGridViewRow)
            .GetNestedType("DataGridViewSelectedRowCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, [row], null);

        Assert.Equal(selecetedCellsCount, accessibleObject.GetChildCount());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCellsAccessibleObject_GetChild_ReturnsExpected()
    {
        using NoAssertContext context = new();
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");
        control.Columns.Add("Column 2", "Header text 2");
        control.Rows.Add("Row1");
        DataGridViewRow row = control.Rows[0];

        DataGridViewCell selecetedCell1 = row.Cells[0];
        selecetedCell1.Selected = true;

        Type type = typeof(DataGridViewRow)
            .GetNestedType("DataGridViewSelectedRowCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, [row], null);

        Assert.Null(accessibleObject.GetChild(-1));
        Assert.Equal(selecetedCell1.AccessibilityObject, accessibleObject.GetChild(0));
        Assert.Null(accessibleObject.GetChild(1));

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCellsAccessibleObject_Navigate_ReturnsExpected_NoChilds()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");

        Type type = typeof(DataGridViewRow)
            .GetNestedType("DataGridViewSelectedRowCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, [control.Rows[0]], null);

        Assert.Equal(0, accessibleObject.GetChildCount());
        Assert.Null(accessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Null(accessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCellsAccessibleObject_Navigate_ReturnsExpected()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");
        control.Columns.Add("Column 2", "Header text 2");
        control.Rows.Add("Row1");
        DataGridViewRow row = control.Rows[0];

        DataGridViewCell selecetedCell1 = row.Cells[0];
        selecetedCell1.Selected = true;

        Type type = typeof(DataGridViewRow)
            .GetNestedType("DataGridViewSelectedRowCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, [row], null);

        Assert.Equal(1, accessibleObject.GetChildCount());
        Assert.Equal(selecetedCell1.AccessibilityObject, accessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(selecetedCell1.AccessibilityObject, accessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCellsAccessibleObject_GetPropertyValue_ValueValuePropertyId_ReturnsExpected()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");
        control.Columns.Add("Column 2", "Header text 2");
        control.Rows.Add("Row1");
        DataGridViewRow row = control.Rows[0];
        DataGridViewCell selecetedCell1 = row.Cells[0];
        selecetedCell1.Selected = true;
        Type type = typeof(DataGridViewRow)
            .GetNestedType("DataGridViewSelectedRowCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, [row], null);

        Assert.Equal("Selected Row Cells", ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId)).ToStringAndFree());
        Assert.False(control.IsHandleCreated);
    }
}
