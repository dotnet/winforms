// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class DataGridViewSelectedCellsAccessibleObjectTests
{
    [WinFormsFact]
    public void DataGridViewSelectedCellsAccessibleObject_Ctor_Default()
    {
        Type type = typeof(DataGridView)
            .GetNestedType("DataGridViewSelectedCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, [null]);

        Assert.Null(accessibleObject.TestAccessor().Dynamic._parentAccessibleObject);
        Assert.Equal(AccessibleRole.Grouping, accessibleObject.Role);
    }

    [WinFormsFact]
    public void DataGridViewSelectedCellsAccessibleObject_Name_ReturnsExpected()
    {
        Type type = typeof(DataGridView)
            .GetNestedType("DataGridViewSelectedCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, [null]);

        Assert.Equal(SR.DataGridView_AccSelectedCellsName, accessibleObject.Name);
    }

    [WinFormsFact]
    public void DataGridViewSelectedCellsAccessibleObject_Value_ReturnsExpected()
    {
        Type type = typeof(DataGridView)
            .GetNestedType("DataGridViewSelectedCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, [null]);

        Assert.Equal(SR.DataGridView_AccSelectedCellsName, accessibleObject.Value);
    }

    [WinFormsFact]
    public void DataGridViewSelectedCellsAccessibleObject_State_ReturnsExpected()
    {
        Type type = typeof(DataGridView)
            .GetNestedType("DataGridViewSelectedCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, [null]);

        Assert.Equal(AccessibleStates.Selected | AccessibleStates.Selectable, accessibleObject.State);
    }

    [WinFormsFact]
    public void DataGridViewSelectedCellsAccessibleObject_Parent_ReturnsExpected()
    {
        using DataGridView control = new();
        Type type = typeof(DataGridView)
            .GetNestedType("DataGridViewSelectedCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, [control.AccessibilityObject]);

        Assert.Equal(control.AccessibilityObject, accessibleObject.Parent);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewSelectedCellsAccessibleObject_GetSelected_ReturnsExpected()
    {
        Type type = typeof(DataGridView)
            .GetNestedType("DataGridViewSelectedCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, [null]);

        Assert.Equal(accessibleObject, accessibleObject.GetSelected());
    }

    [WinFormsFact]
    public void DataGridViewSelectedCellsAccessibleObject_GetFocused_ReturnsExpected_IfCurrentCellIsSelected()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");
        control.Rows.Add("Row1");
        DataGridViewCell currentCell = control.Rows[0].Cells[0];
        control.CurrentCell = currentCell;
        currentCell.Selected = true;

        Type type = typeof(DataGridView)
            .GetNestedType("DataGridViewSelectedCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, [control.AccessibilityObject]);

        Assert.NotNull(currentCell);
        Assert.Equal(currentCell.AccessibilityObject, accessibleObject.GetFocused());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewSelectedCellsAccessibleObject_GetFocused_ReturnsNull_IfCurrentCellIsNotSelected()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");
        DataGridViewCell currentCell = control.Rows[0].Cells[0];
        control.CurrentCell = currentCell;
        currentCell.Selected = false;

        Type type = typeof(DataGridView)
            .GetNestedType("DataGridViewSelectedCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, [control.AccessibilityObject]);

        Assert.NotNull(currentCell);
        Assert.Null(accessibleObject.GetFocused());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewSelectedCellsAccessibleObject_GetChildCount_ReturnsExpected()
    {
        int selecetedCellsCount = 2;
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");
        control.Columns.Add("Column 2", "Header text 2");
        control.Columns.Add("Column 3", "Header text 3");
        control.Rows.Add("Row1");

        for (int i = 0; i < selecetedCellsCount; i++)
        {
            control.Rows[0].Cells[i].Selected = true;
        }

        Type type = typeof(DataGridView)
            .GetNestedType("DataGridViewSelectedCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, [control.AccessibilityObject]);

        Assert.Equal(selecetedCellsCount, accessibleObject.GetChildCount());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewSelectedCellsAccessibleObject_GetChild_ReturnsExpected()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");
        control.Columns.Add("Column 2", "Header text 2");
        control.Rows.Add("Row1");

        DataGridViewCell selecetedCell1 = control.Rows[0].Cells[0];
        selecetedCell1.Selected = true;

        Type type = typeof(DataGridView)
            .GetNestedType("DataGridViewSelectedCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, [control.AccessibilityObject]);

        Assert.Null(accessibleObject.GetChild(-1));
        Assert.Equal(selecetedCell1.AccessibilityObject, accessibleObject.GetChild(0));
        Assert.Null(accessibleObject.GetChild(1));

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewSelectedCellsAccessibleObject_Navigate_ReturnsExpected_NoChilds()
    {
        using DataGridView control = new();

        Type type = typeof(DataGridView)
            .GetNestedType("DataGridViewSelectedCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, [control.AccessibilityObject]);

        Assert.Equal(0, accessibleObject.GetChildCount());
        Assert.Null(accessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Null(accessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewSelectedCellsAccessibleObject_Navigate_ReturnsExpected()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");
        control.Columns.Add("Column 2", "Header text 2");
        control.Rows.Add("Row1");

        DataGridViewCell selecetedCell1 = control.Rows[0].Cells[0];
        selecetedCell1.Selected = true;

        Type type = typeof(DataGridView)
            .GetNestedType("DataGridViewSelectedCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, [control.AccessibilityObject]);

        Assert.Equal(1, accessibleObject.GetChildCount());
        Assert.Equal(selecetedCell1.AccessibilityObject, accessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(selecetedCell1.AccessibilityObject, accessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewSelectedCellsAccessibleObject_GetPropertyValue_ValueValuePropertyId_ReturnsExpected()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");
        control.Columns.Add("Column 2", "Header text 2");
        control.Rows.Add("Row1");
        DataGridViewCell selecetedCell1 = control.Rows[0].Cells[0];
        selecetedCell1.Selected = true;
        Type type = typeof(DataGridView)
            .GetNestedType("DataGridViewSelectedCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, [control.AccessibilityObject]);

        Assert.Equal("Selected Cells", ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId)).ToStringAndFree());
        Assert.False(control.IsHandleCreated);
    }
}
