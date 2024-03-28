// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class DataGridViewTopRowAccessibleObjectTests
{
    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_FragmentNavigate_ReturnsExpected_AllowUserToAddRowsEnabled()
    {
        using DataGridView dataGridView = new();
        DataGridViewTextBoxColumn dataGridViewColumn = new() { Name = "Col 1", HeaderText = "Col 1" };

        dataGridView.Columns.Add(dataGridViewColumn);

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject expectedNextSibling = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject expectedFirstChild = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject expectedLastChild = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(dataGridView.AccessibilityObject, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(expectedNextSibling, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(expectedFirstChild, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(expectedLastChild, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_FragmentNavigate_ReturnsExpected_AllowUserToAddRowsDisabled()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };
        DataGridViewTextBoxColumn dataGridViewColumn = new() { Name = "Col 1", HeaderText = "Col 1" };

        dataGridView.Columns.Add(dataGridViewColumn);

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject expectedFirstChild = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject expectedLastChild = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(dataGridView.AccessibilityObject, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(expectedFirstChild, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(expectedLastChild, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_FragmentNavigate_ReturnsExpected_WithoutColumns()
    {
        using DataGridView dataGridView = new();

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;

        Assert.Equal(dataGridView.AccessibilityObject, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfRowHeadersAndFirstColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfRowHeadersAndSecondColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[1].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfRowHeadersAndLastColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfRowHeadersAndAllColumnsHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        Assert.Null(topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfFirstColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfSecondColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[1].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfLastColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfAllColumnsHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfCustomOrderAndRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfCustomOrderAndFirstColumnAndRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[0].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfCustomOrderAndSecondColumnAndRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[1].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfCustomOrderAndLastColumnAndRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfCustomOrder()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfCustomOrderAndFirstColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[0].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfCustomOrderAndSecondColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[1].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfCustomOrderAndLastColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_Navigate_ReturnsExpected_IfRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_Navigate_ReturnsExpected_IfRowHeadersAndFirstColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_Navigate_ReturnsExpected_IfRowHeadersAndSecondColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[1].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_Navigate_ReturnsExpected_IfRowHeadersAndLastColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_Navigate_ReturnsExpected_IfRowHeadersAndAllColumnsHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        Assert.Null(topRowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Null(topRowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_Navigate_ReturnsExpected_IfFirstColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject, topRowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_Navigate_ReturnsExpected_IfSecondColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[1].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject, topRowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_Navigate_ReturnsExpected_IfLastColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject, topRowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_Navigate_ReturnsExpected_IfAllColumnsHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_Navigate_ReturnsExpected_IfCustomOrderAndRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_Navigate_ReturnsExpected_IfCustomOrderAndFirstColumnAndRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[0].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_Navigate_ReturnsExpected_IfCustomOrderAndSecondColumnAndRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[1].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_Navigate_ReturnsExpected_IfCustomOrderAndLastColumnAndRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_Navigate_ReturnsExpected_IfCustomOrder()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject, topRowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_Navigate_ReturnsExpected_IfCustomOrderAndFirstColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[0].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject, topRowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_Navigate_ReturnsExpected_IfCustomOrderAndSecondColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[1].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject, topRowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_Navigate_ReturnsExpected_IfCustomOrderAndLastColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject, topRowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChild_ReturnsExpected_IfRowHeadersAndColumnsHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        Assert.Null(topRowAccessibleObject.GetChild(0));
        Assert.Null(topRowAccessibleObject.GetChild(1));
        Assert.Null(topRowAccessibleObject.GetChild(2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChild_ReturnsExpected_IfRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.GetChild(1));
        Assert.Equal(accessibleObject3, topRowAccessibleObject.GetChild(2));
        Assert.Null(topRowAccessibleObject.GetChild(3));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChild_ReturnsExpected_IfRowHeadersAndFirstColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject2, topRowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject3, topRowAccessibleObject.GetChild(1));
        Assert.Null(topRowAccessibleObject.GetChild(2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChild_ReturnsExpected_IfRowHeadersAndSecondColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[1].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject3, topRowAccessibleObject.GetChild(1));
        Assert.Null(topRowAccessibleObject.GetChild(2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChild_ReturnsExpected_IfRowHeadersAndLastColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.GetChild(1));
        Assert.Null(topRowAccessibleObject.GetChild(2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChild_ReturnsExpected()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject1, topRowAccessibleObject.GetChild(1));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.GetChild(2));
        Assert.Equal(accessibleObject3, topRowAccessibleObject.GetChild(3));
        Assert.Null(topRowAccessibleObject.GetChild(4));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChild_ReturnsExpected_IfAllColumnsHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.GetChild(0));
        Assert.Null(topRowAccessibleObject.GetChild(1));
        Assert.Null(topRowAccessibleObject.GetChild(2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChild_ReturnsExpected_IfFirstColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.GetChild(1));
        Assert.Equal(accessibleObject3, topRowAccessibleObject.GetChild(2));
        Assert.Null(topRowAccessibleObject.GetChild(3));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChild_ReturnsExpected_IfSecondColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[1].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject1, topRowAccessibleObject.GetChild(1));
        Assert.Equal(accessibleObject3, topRowAccessibleObject.GetChild(2));
        Assert.Null(topRowAccessibleObject.GetChild(3));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChild_ReturnsExpected_IfThirdColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject1, topRowAccessibleObject.GetChild(1));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.GetChild(2));
        Assert.Null(topRowAccessibleObject.GetChild(3));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChild_ReturnsExpected_IfCustomOrderAndRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.GetChild(1));
        Assert.Equal(accessibleObject3, topRowAccessibleObject.GetChild(2));
        Assert.Null(topRowAccessibleObject.GetChild(3));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChild_ReturnsExpected_IfCustomOrderAndRowHeadersAndFirstColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[0].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.GetChild(1));
        Assert.Null(topRowAccessibleObject.GetChild(2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChild_ReturnsExpected_IfCustomOrderAndRowHeadersAndSecondColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[1].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject3, topRowAccessibleObject.GetChild(1));
        Assert.Null(topRowAccessibleObject.GetChild(2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChild_ReturnsExpected_IfCustomOrderAndRowHeadersAndLastColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject2, topRowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject3, topRowAccessibleObject.GetChild(1));
        Assert.Null(topRowAccessibleObject.GetChild(2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChild_ReturnsExpected_IfCustomOrder()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject1, topRowAccessibleObject.GetChild(1));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.GetChild(2));
        Assert.Equal(accessibleObject3, topRowAccessibleObject.GetChild(3));
        Assert.Null(topRowAccessibleObject.GetChild(4));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChild_ReturnsExpected_IfCustomOrderAndFirstColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[0].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject1, topRowAccessibleObject.GetChild(1));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.GetChild(2));
        Assert.Null(topRowAccessibleObject.GetChild(3));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChild_ReturnsExpected_IfCustomOrderAndSecondColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[1].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject1, topRowAccessibleObject.GetChild(1));
        Assert.Equal(accessibleObject3, topRowAccessibleObject.GetChild(2));
        Assert.Null(topRowAccessibleObject.GetChild(3));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChild_ReturnsExpected_IfCustomOrderAndLastColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject2, topRowAccessibleObject.GetChild(1));
        Assert.Equal(accessibleObject3, topRowAccessibleObject.GetChild(2));
        Assert.Null(topRowAccessibleObject.GetChild(3));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChildCount_ReturnsFour()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        Assert.Equal(4, topRowAccessibleObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChildCount_ReturnsThree_IfOneColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        Assert.Equal(3, topRowAccessibleObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChildCount_ReturnsTwo_IfTwoColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        Assert.Equal(2, topRowAccessibleObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChildCount_ReturnsOne_IfAllColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        Assert.Equal(1, topRowAccessibleObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChildCount_ReturnsThreeIfRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        Assert.Equal(3, topRowAccessibleObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChildCount_ReturnsTwo_IfRowHeadersAndOneColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        Assert.Equal(2, topRowAccessibleObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChildCount_ReturnsOne_IfRowHeadersAndTwoColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        Assert.Equal(1, topRowAccessibleObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetChildCount_ReturnsZero_IfRowHeadersAndAllColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        Assert.Equal(0, topRowAccessibleObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewTopRowAccessibleObject_GetPropertyValue_ReturnsExpected()
    {
        using DataGridView dataGridView = new();
        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        Assert.True((bool)topRowAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsLegacyIAccessiblePatternAvailablePropertyId));
        Assert.Equal(string.Empty, ((BSTR)topRowAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HelpTextPropertyId)).ToStringAndFree());
        Assert.Equal(SR.DataGridView_AccTopRow, ((BSTR)topRowAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId)).ToStringAndFree());
        Assert.Equal(SR.DataGridView_AccTopRow, ((BSTR)topRowAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleNamePropertyId)).ToStringAndFree());
        Assert.Equal("Top Row", ((BSTR)topRowAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId)).ToStringAndFree());
        Assert.False(dataGridView.IsHandleCreated);
    }
}
