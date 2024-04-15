// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class DataGridViewColumnHeaderCellAccessibleObjectTests : DataGridViewColumnHeaderCell
{
    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Ctor_Default()
    {
        DataGridViewColumnHeaderCellAccessibleObject accessibleObject = new(null);
        Assert.Null(accessibleObject.Owner);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Parent_ThrowException_IfOwnerIsNull()
    {
        Assert.Throws<InvalidOperationException>(() => new DataGridViewColumnHeaderCellAccessibleObject(null).Parent);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Parent_ReturnsExpected()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");
        DataGridViewColumnHeaderCell cell = control.Columns[0].HeaderCell;

        var accessibleObject = (DataGridViewColumnHeaderCellAccessibleObject)cell.AccessibilityObject;

        Assert.Equal(control.AccessibilityObject.GetChild(0), accessibleObject.Parent);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Role_ReturnsExpected()
    {
        DataGridViewColumnHeaderCellAccessibleObject accessibleObject = new(null);
        Assert.Equal(AccessibleRole.ColumnHeader, accessibleObject.Role);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Name_ReturnsExpected()
    {
        string testHeaderName = "Test header name";
        using DataGridView control = new();
        control.Columns.Add("Column 1", testHeaderName);

        var accessibleObject = (DataGridViewColumnHeaderCellAccessibleObject)control.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(testHeaderName, accessibleObject.Name);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Value_ReturnsExpected()
    {
        string testHeaderName = "Test header name";
        using DataGridView control = new();
        control.Columns.Add("Column 1", testHeaderName);

        var accessibleObject = (DataGridViewColumnHeaderCellAccessibleObject)control.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(testHeaderName, accessibleObject.Value);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Bounds_IsNotEmptyRectangle_IfHandleIsCreated()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");
        control.CreateControl();

        var accessibleObject = (DataGridViewColumnHeaderCellAccessibleObject)control.Columns[0].HeaderCell.AccessibilityObject;

        Assert.NotEqual(Rectangle.Empty, accessibleObject.Bounds);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Bounds_IsEmptyRectangle_IfHandleIsNotCreated()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");

        var accessibleObject = (DataGridViewColumnHeaderCellAccessibleObject)control.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(Rectangle.Empty, accessibleObject.Bounds);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_DefaultAction_ReturnsExpected_IfSortModeIsAutomatic()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");
        DataGridViewColumn column = control.Columns[0];
        column.SortMode = DataGridViewColumnSortMode.Automatic;

        var accessibleObject = (DataGridViewColumnHeaderCellAccessibleObject)column.HeaderCell.AccessibilityObject;

        Assert.Equal(SR.DataGridView_AccColumnHeaderCellDefaultAction, accessibleObject.DefaultAction);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_GetPropertyValue_LegacyIAccessibleDefaultActionPropertyId_ReturnsExpected()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");
        DataGridViewColumn column = control.Columns[0];
        column.SortMode = DataGridViewColumnSortMode.Automatic;
        var accessibleObject = (DataGridViewColumnHeaderCellAccessibleObject)column.HeaderCell.AccessibilityObject;

        Assert.Equal(SR.DataGridView_AccColumnHeaderCellDefaultAction, ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleDefaultActionPropertyId)).ToStringAndFree());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_InvokePatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId)]
    public void DataGridViewColumnHeaderCellAccessibleObject_IsPatternSupported_ReturnsExpected(int patternId)
    {
        DataGridViewColumnHeaderCellAccessibleObject accessibleObject = new(null);

        Assert.True(accessibleObject.IsPatternSupported((UIA_PATTERN_ID)patternId));
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_ControlType_ReturnsExpected()
    {
        DataGridViewColumnHeaderCellAccessibleObject accessibleObject = new(null);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_HeaderControlTypeId, (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId));
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_ValueValuepropertyId_ReturnsExpected()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");
        DataGridViewColumn column = control.Columns[0];
        column.SortMode = DataGridViewColumnSortMode.Automatic;

        var accessibleObject = (DataGridViewColumnHeaderCellAccessibleObject)column.HeaderCell.AccessibilityObject;
        Assert.Equal("Header text 1", ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId)).ToStringAndFree());
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Header text 1");
        var accessibleObject = control.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject topRowAccessibleObject = control.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        Assert.Equal(topRowAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfRowHeadersHidden()
    {
        using DataGridView control = new() { RowHeadersVisible = false };
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");

        AccessibleObject accessibleObject1 = control.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfFirstColumnAndRowHeadersHidden()
    {
        using DataGridView control = new() { RowHeadersVisible = false };
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[0].Visible = false;

        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfSecondColumnAndRowHeadersHidden()
    {
        using DataGridView control = new() { RowHeadersVisible = false };
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[1].Visible = false;

        AccessibleObject accessibleObject1 = control.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject3, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject1, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfLastColumnAndRowHeadersHidden()
    {
        using DataGridView control = new() { RowHeadersVisible = false };
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[2].Visible = false;

        AccessibleObject accessibleObject1 = control.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");

        AccessibleObject accessibleObject0 = control.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = control.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, accessibleObject0.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject0, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject0.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfFirstColumnHidden()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[0].Visible = false;

        AccessibleObject accessibleObject0 = control.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject2, accessibleObject0.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject0, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject0.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfSecondColumnHidden()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[1].Visible = false;

        AccessibleObject accessibleObject0 = control.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = control.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, accessibleObject0.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject3, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject1, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject0, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject0.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfLastColumnHidden()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[2].Visible = false;

        AccessibleObject accessibleObject0 = control.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = control.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, accessibleObject0.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject0, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject0.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Navigate_Sibling_ReturnsExpected_IfRowHeadersHidden()
    {
        using DataGridView control = new() { RowHeadersVisible = false };
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");

        AccessibleObject accessibleObject1 = control.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject2, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject3, accessibleObject2.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject3.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject2, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject1, accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Navigate_Sibling_ReturnsExpected_IfFirstColumnAndRowHeadersHidden()
    {
        using DataGridView control = new() { RowHeadersVisible = false };
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[0].Visible = false;

        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject3, accessibleObject2.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject3.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject2, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Navigate_Sibling_ReturnsExpected_IfSecondColumnAndRowHeadersHidden()
    {
        using DataGridView control = new() { RowHeadersVisible = false };
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[1].Visible = false;

        AccessibleObject accessibleObject1 = control.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject3, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject3.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject1, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Navigate_Sibling_ReturnsExpected_IfLastColumnAndRowHeadersHidden()
    {
        using DataGridView control = new() { RowHeadersVisible = false };
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[2].Visible = false;

        AccessibleObject accessibleObject1 = control.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject2, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject2.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject1, accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Navigate_Sibling_ReturnsExpected()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");

        AccessibleObject accessibleObject0 = control.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = control.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, accessibleObject0.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject2, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject3, accessibleObject2.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject3.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject2, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject1, accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject0, accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject0.Navigate(AccessibleNavigation.Previous));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Navigate_Sibling_ReturnsExpected_IfFirstColumnHidden()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[0].Visible = false;

        AccessibleObject accessibleObject0 = control.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject2, accessibleObject0.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject3, accessibleObject2.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject3.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject2, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject0, accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject0.Navigate(AccessibleNavigation.Previous));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Navigate_Sibling_ReturnsExpected_IfSecondColumnHidden()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[1].Visible = false;

        AccessibleObject accessibleObject0 = control.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = control.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[2].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, accessibleObject0.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject3, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject3.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject1, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject0, accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject0.Navigate(AccessibleNavigation.Previous));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Navigate_Sibling_ReturnsExpected_IfLastColumnHidden()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[2].Visible = false;

        AccessibleObject accessibleObject0 = control.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = control.Columns[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, accessibleObject0.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject2, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject2.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject1, accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject0, accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject0.Navigate(AccessibleNavigation.Previous));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Navigate_Child_ReturnsNull_IfRowHeadersHidden()
    {
        using DataGridView control = new() { RowHeadersVisible = false };
        control.Columns.Add("Column 1", "Column 1");
        AccessibleObject accessibleObject = control.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Null(accessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Null(accessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Navigate_Child_ReturnsNull()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Column 1");
        AccessibleObject accessibleObject0 = control.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = control.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Null(accessibleObject0.Navigate(AccessibleNavigation.FirstChild));
        Assert.Null(accessibleObject0.Navigate(AccessibleNavigation.LastChild));
        Assert.Null(accessibleObject1.Navigate(AccessibleNavigation.FirstChild));
        Assert.Null(accessibleObject1.Navigate(AccessibleNavigation.LastChild));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_FragmentNavigate_Child_ReturnsNull_IfRowHeadersHidden()
    {
        using DataGridView control = new() { RowHeadersVisible = false };
        control.Columns.Add("Column 1", "Column 1");
        AccessibleObject accessibleObject = control.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_FragmentNavigate_Child_ReturnsNull()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Column 1");
        AccessibleObject accessibleObject0 = control.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = control.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Null(accessibleObject0.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject0.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfCustomOrder()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[0].DisplayIndex = 2;
        control.Columns[1].DisplayIndex = 1;
        control.Columns[2].DisplayIndex = 0;

        AccessibleObject accessibleObject0 = control.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = control.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, accessibleObject0.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject0, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject0.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfCustomOrderAndFirstColumnHidden()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[0].DisplayIndex = 2;
        control.Columns[0].Visible = false;
        control.Columns[1].DisplayIndex = 1;
        control.Columns[2].DisplayIndex = 0;

        AccessibleObject accessibleObject0 = control.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = control.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, accessibleObject0.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject0, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject0.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfCustomOrderAndSecondColumnHidden()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[0].DisplayIndex = 2;
        control.Columns[1].DisplayIndex = 1;
        control.Columns[1].Visible = false;
        control.Columns[2].DisplayIndex = 0;

        AccessibleObject accessibleObject0 = control.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = control.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, accessibleObject0.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject3, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject1, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject0, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject0.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfCustomOrderAndLastColumnHidden()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[0].DisplayIndex = 2;
        control.Columns[1].DisplayIndex = 1;
        control.Columns[2].DisplayIndex = 0;
        control.Columns[2].Visible = false;

        AccessibleObject accessibleObject0 = control.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject2, accessibleObject0.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject0, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject0.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfCustomOrderAndRowHeadersHidden()
    {
        using DataGridView control = new() { RowHeadersVisible = false };
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[0].DisplayIndex = 2;
        control.Columns[1].DisplayIndex = 1;
        control.Columns[2].DisplayIndex = 0;

        AccessibleObject accessibleObject1 = control.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfCustomOrderAndFirstColumnAndRowHeadersHidden()
    {
        using DataGridView control = new() { RowHeadersVisible = false };
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[0].DisplayIndex = 2;
        control.Columns[0].Visible = false;
        control.Columns[1].DisplayIndex = 1;
        control.Columns[2].DisplayIndex = 0;

        AccessibleObject accessibleObject1 = control.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfCustomOrderAndSecondColumnAndRowHeadersHidden()
    {
        using DataGridView control = new() { RowHeadersVisible = false };
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[0].DisplayIndex = 2;
        control.Columns[1].DisplayIndex = 1;
        control.Columns[1].Visible = false;
        control.Columns[2].DisplayIndex = 0;

        AccessibleObject accessibleObject1 = control.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject3, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject1, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfCustomOrderAndLastColumnAndRowHeadersHidden()
    {
        using DataGridView control = new() { RowHeadersVisible = false };
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[0].DisplayIndex = 2;
        control.Columns[1].DisplayIndex = 1;
        control.Columns[2].DisplayIndex = 0;
        control.Columns[2].Visible = false;

        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Navigate_Sibling_ReturnsExpected_IfCustomOrder()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[0].DisplayIndex = 2;
        control.Columns[1].DisplayIndex = 1;
        control.Columns[2].DisplayIndex = 0;

        AccessibleObject accessibleObject0 = control.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = control.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, accessibleObject0.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject2, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject3, accessibleObject2.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject3.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject2, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject1, accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject0, accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject0.Navigate(AccessibleNavigation.Previous));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Navigate_Sibling_ReturnsExpected_IfCustomOrderAndFirstColumnHidden()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[0].DisplayIndex = 2;
        control.Columns[0].Visible = false;
        control.Columns[1].DisplayIndex = 1;
        control.Columns[2].DisplayIndex = 0;

        AccessibleObject accessibleObject0 = control.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = control.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, accessibleObject0.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject2, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject2.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject1, accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject0, accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject0.Navigate(AccessibleNavigation.Previous));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Navigate_Sibling_ReturnsExpected_IfCustomOrderAndSecondColumnHidden()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[0].DisplayIndex = 2;
        control.Columns[1].DisplayIndex = 1;
        control.Columns[1].Visible = false;
        control.Columns[2].DisplayIndex = 0;

        AccessibleObject accessibleObject0 = control.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = control.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject1, accessibleObject0.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject3, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject3.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject1, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject0, accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject0.Navigate(AccessibleNavigation.Previous));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Navigate_Sibling_ReturnsExpected_IfCustomOrderAndLastColumnHidden()
    {
        using DataGridView control = new();
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[0].DisplayIndex = 2;
        control.Columns[1].DisplayIndex = 1;
        control.Columns[2].DisplayIndex = 0;
        control.Columns[2].Visible = false;

        AccessibleObject accessibleObject0 = control.TopLeftHeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject2, accessibleObject0.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject3, accessibleObject2.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject3.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject2, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject0, accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject0.Navigate(AccessibleNavigation.Previous));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Navigate_Sibling_ReturnsExpected_IfCustomOrderAndRowHeadersHidden()
    {
        using DataGridView control = new() { RowHeadersVisible = false };
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[0].DisplayIndex = 2;
        control.Columns[1].DisplayIndex = 1;
        control.Columns[2].DisplayIndex = 0;

        AccessibleObject accessibleObject1 = control.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject2, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject3, accessibleObject2.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject3.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject2, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject1, accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Navigate_Sibling_ReturnsExpected_IfCustomOrderAndFirstColumnAndRowHeadersHidden()
    {
        using DataGridView control = new() { RowHeadersVisible = false };
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[0].DisplayIndex = 2;
        control.Columns[0].Visible = false;
        control.Columns[1].DisplayIndex = 1;
        control.Columns[2].DisplayIndex = 0;

        AccessibleObject accessibleObject1 = control.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject2, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject2.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject1, accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Navigate_Sibling_ReturnsExpected_IfCustomOrderAndSecondColumnAndRowHeadersHidden()
    {
        using DataGridView control = new() { RowHeadersVisible = false };
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[0].DisplayIndex = 2;
        control.Columns[1].DisplayIndex = 1;
        control.Columns[1].Visible = false;
        control.Columns[2].DisplayIndex = 0;

        AccessibleObject accessibleObject1 = control.Columns[2].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject3, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject3.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject1, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumnHeaderCellAccessibleObject_Navigate_Sibling_ReturnsExpected_IfCustomOrderAndLastColumnAndRowHeadersHidden()
    {
        using DataGridView control = new() { RowHeadersVisible = false };
        control.Columns.Add("Column 1", "Column 1");
        control.Columns.Add("Column 2", "Column 2");
        control.Columns.Add("Column 3", "Column 3");
        control.Columns[0].DisplayIndex = 2;
        control.Columns[1].DisplayIndex = 1;
        control.Columns[2].DisplayIndex = 0;
        control.Columns[2].Visible = false;

        AccessibleObject accessibleObject2 = control.Columns[1].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Columns[0].HeaderCell.AccessibilityObject;

        Assert.Equal(accessibleObject3, accessibleObject2.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject3.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject2, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.False(control.IsHandleCreated);
    }
}
