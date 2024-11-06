// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

// NB: doesn't require thread affinity
public class DataGridViewRowAccessibleObjectTests : DataGridViewRow
{
    [Fact]
    public void DataGridViewRowAccessibleObject_Ctor_Default()
    {
        DataGridViewRowAccessibleObject accessibleObject = new();

        Assert.Null(accessibleObject.Owner);
        Assert.Equal(AccessibleRole.Row, accessibleObject.Role);
        Assert.Null(accessibleObject.DefaultAction);
        Assert.Null(accessibleObject.Help);
    }

    public static IEnumerable<object[]> Ctor_DataGridViewRow_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new DataGridViewRow() };
    }

    [Theory]
    [MemberData(nameof(Ctor_DataGridViewRow_TestData))]
    public void DataGridViewRowAccessibleObject_Ctor_DataGridViewRow(DataGridViewRow owner)
    {
        DataGridViewRowAccessibleObject accessibleObject = new(owner);

        Assert.Equal(owner, accessibleObject.Owner);
        Assert.Equal(AccessibleRole.Row, accessibleObject.Role);
        Assert.Null(accessibleObject.DefaultAction);
        Assert.Null(accessibleObject.Help);
    }

    public static IEnumerable<object[]> Bounds_TestData()
    {
        yield return new object[] { new DataGridViewRowAccessibleObject(new DataGridViewRow()), Rectangle.Empty };
    }

    [Theory]
    [MemberData(nameof(Bounds_TestData))]
    public void DataGridViewRowAccessibleObject_Bounds_Get_ReturnsExpected(AccessibleObject accessibleObject, Rectangle expected)
    {
        Assert.Equal(expected, accessibleObject.Bounds);
    }

    public static IEnumerable<object[]> NoOwner_TestData()
    {
        yield return new object[] { new DataGridViewRowAccessibleObject() };
        yield return new object[] { new DataGridViewRowAccessibleObject(null) };
    }

    [Theory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewRowAccessibleObject_Bounds_GetNoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(() => accessibleObject.Bounds);
    }

    [Fact]
    public void DataGridViewRowAccessibleObject_Name_Get_ReturnsExpected_IfDataGridViewNotExist()
    {
        AccessibleObject accessibilityObject = new DataGridViewRowAccessibleObject(new DataGridViewRow());

        Assert.Equal(string.Format(SR.DataGridView_AccRowName, -1), accessibilityObject.Name);
    }

    // Whether UIA row indexing is 1-based or 0-based, is controlled by the DataGridViewUIAStartRowCountAtZero switch
    [Fact]
    public void DataGridViewRowAccessibleObject_Name_Get_ReturnsExpected()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;

        Assert.Equal(string.Format(SR.DataGridView_AccRowName, 1), accessibleObject1.Name);
        Assert.Equal(string.Format(SR.DataGridView_AccRowName, 2), accessibleObject2.Name);
        Assert.Equal(string.Format(SR.DataGridView_AccRowName, 3), accessibleObject3.Name);
        Assert.False(dataGridView.IsHandleCreated);
    }

    // Whether UIA row indexing is 1-based or 0-based, is controlled by the DataGridViewUIAStartRowCountAtZero switch
    [Fact]
    public void DataGridViewRowAccessibleObject_Name_Get_ReturnsExpected_IfFirstRowHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[0].Visible = false;

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;

        Assert.Equal(string.Format(SR.DataGridView_AccRowName, -1), accessibleObject1.Name);
        Assert.Equal(string.Format(SR.DataGridView_AccRowName, 1), accessibleObject2.Name);
        Assert.Equal(string.Format(SR.DataGridView_AccRowName, 2), accessibleObject3.Name);
        Assert.False(dataGridView.IsHandleCreated);
    }

    // Whether UIA row indexing is 1-based or 0-based, is controlled by the DataGridViewUIAStartRowCountAtZero switch
    [Fact]
    public void DataGridViewRowAccessibleObject_Name_Get_ReturnsExpected_IfSecondRowHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[1].Visible = false;

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;

        Assert.Equal(string.Format(SR.DataGridView_AccRowName, 1), accessibleObject1.Name);
        Assert.Equal(string.Format(SR.DataGridView_AccRowName, -1), accessibleObject2.Name);
        Assert.Equal(string.Format(SR.DataGridView_AccRowName, 2), accessibleObject3.Name);
        Assert.False(dataGridView.IsHandleCreated);
    }

    // Whether UIA row indexing is 1-based or 0-based, is controlled by the DataGridViewUIAStartRowCountAtZero switch
    [Fact]
    public void DataGridViewRowAccessibleObject_Name_Get_ReturnsExpected_IfLastRowHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[2].Visible = false;

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;

        Assert.Equal(string.Format(SR.DataGridView_AccRowName, 1), accessibleObject1.Name);
        Assert.Equal(string.Format(SR.DataGridView_AccRowName, 2), accessibleObject2.Name);
        Assert.Equal(string.Format(SR.DataGridView_AccRowName, -1), accessibleObject3.Name);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [Theory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewRowAccessibleObject_Name_GetNoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(() => accessibleObject.Name);
    }

    [Fact]
    public void DataGridViewRowAccessibleObject_Owner_Set_GetReturnsExpected()
    {
        using DataGridViewRow owner = new();
        DataGridViewRowAccessibleObject accessibleObject = new()
        {
            Owner = owner
        };
        Assert.Same(owner, accessibleObject.Owner);
    }

    [Fact]
    public void DataGridViewRowAccessibleObject_Owner_SetAlreadyWithOwner_ThrowsInvalidOperationException()
    {
        using DataGridViewRow owner = new();
        DataGridViewRowAccessibleObject accessibleObject = new(owner);
        Assert.Throws<InvalidOperationException>(() => accessibleObject.Owner = owner);
    }

    public static IEnumerable<object[]> Parent_TestData()
    {
        yield return new object[] { new DataGridViewRowAccessibleObject(new DataGridViewRow()), null };
    }

    [Theory]
    [MemberData(nameof(Parent_TestData))]
    public void DataGridViewRowAccessibleObject_Parent_Get_ReturnsExpected(AccessibleObject accessibleObject, AccessibleObject expected)
    {
        Assert.Same(expected, accessibleObject.Parent);
    }

    [Theory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewRowAccessibleObject_Parent_GetNoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(() => accessibleObject.Parent);
    }

    public static IEnumerable<object[]> State_TestData()
    {
        yield return new object[] { new DataGridViewRowAccessibleObject(new DataGridViewRow()), AccessibleStates.Selected | AccessibleStates.Selectable };
    }

    [Theory]
    [MemberData(nameof(State_TestData))]
    public void DataGridViewRowAccessibleObject_State_Get_ReturnsExpected(AccessibleObject accessibleObject, AccessibleStates expected)
    {
        Assert.Equal(expected, accessibleObject.State);
    }

    [Theory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewRowAccessibleObject_State_GetNoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(() => accessibleObject.State);
    }

    public static IEnumerable<object[]> Value_TestData()
    {
        yield return new object[] { new DataGridViewRowAccessibleObject(new DataGridViewRow()), string.Empty };
    }

    [Theory]
    [MemberData(nameof(Value_TestData))]
    public void DataGridViewRowAccessibleObject_Value_Get_ReturnsExpected(AccessibleObject accessibleObject, string expected)
    {
        Assert.Equal(expected, accessibleObject.Value);
    }

    [Theory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewRowAccessibleObject_Value_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(() => accessibleObject.Value);
    }

    [Fact]
    public void DataGridViewRowAccessibleObject_DoDefaultAction_Invoke_Nop()
    {
        DataGridViewRowAccessibleObject accessibleObject = new();
        accessibleObject.DoDefaultAction();
    }

    [Theory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewRowAccessibleObject_GetChild_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(() => accessibleObject.GetChild(0));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void DataGridViewRowAccessibleObject_GetChild_NoDataGridView_ReturnsNull(int index)
    {
        using DataGridViewRow owner = new();
        DataGridViewRowAccessibleObject accessibleObject = new(owner);

        Assert.Null(accessibleObject.GetChild(index));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-2)]
    public void DataGridViewRowAccessibleObject_GetChild_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
    {
        DataGridViewRowAccessibleObject accessibleObject = new();
        Assert.Throws<ArgumentOutOfRangeException>("index", () => accessibleObject.GetChild(index));
    }

    [Theory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewRowAccessibleObject_GetChildCount_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(() => accessibleObject.GetChildCount());
    }

    [Fact]
    public void DataGridViewRowAccessibleObject_GetChildCount_NoDataGridView_ReturnsZero()
    {
        using DataGridViewRow owner = new();
        DataGridViewRowAccessibleObject accessibleObject = new(owner);

        Assert.Equal(0, accessibleObject.GetChildCount());
    }

    [Theory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewRowAccessibleObject_GetFocused_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(accessibleObject.GetFocused);
    }

    [Fact]
    public void DataGridViewRowAccessibleObject_GetFocused_NoDataGridView_ReturnsNull()
    {
        using DataGridViewRow owner = new();
        DataGridViewRowAccessibleObject accessibleObject = new(owner);
        Assert.Null(accessibleObject.GetFocused());
    }

    [Fact]
    public void DataGridViewRowAccessibleObject_GetSelected_Invoke_ReturnsSameInstance()
    {
        using DataGridViewRow owner = new();
        DataGridViewRowAccessibleObject accessibleObject = new(owner);
        Assert.Same(accessibleObject.GetSelected(), accessibleObject.GetSelected());

        AccessibleObject selectedAccessibleObject = accessibleObject.GetSelected();

        Assert.Equal("Selected Row Cells", selectedAccessibleObject.Name);
        Assert.Equal(owner.AccessibilityObject, selectedAccessibleObject.Parent);
        Assert.Equal(AccessibleRole.Grouping, selectedAccessibleObject.Role);
        Assert.Equal(AccessibleStates.Selected | AccessibleStates.Selectable, selectedAccessibleObject.State);
        Assert.Equal("Selected Row Cells", selectedAccessibleObject.Value);
    }

    [Theory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewRowAccessibleObject_GetSelected_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(accessibleObject.GetSelected);
    }

    public static IEnumerable<object[]> Navigate_TestData()
    {
        yield return new object[] { new DataGridViewRowAccessibleObject(new DataGridViewRow()), AccessibleNavigation.Up - 1, null };
        yield return new object[] { new DataGridViewRowAccessibleObject(new DataGridViewRow()), AccessibleNavigation.LastChild + 1, null };
        yield return new object[] { new DataGridViewRowAccessibleObject(new DataGridViewRow()), AccessibleNavigation.Left, null };
        yield return new object[] { new DataGridViewRowAccessibleObject(new DataGridViewRow()), AccessibleNavigation.Right, null };
    }

    [Theory]
    [MemberData(nameof(Navigate_TestData))]
    public void DataGridViewRowAccessibleObject_Navigate_Invoke_ReturnsExpected(AccessibleObject accessibleObject, AccessibleNavigation navigationDirection, AccessibleObject expected)
    {
        Assert.Equal(expected, accessibleObject.Navigate(navigationDirection));
    }

    [Theory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewRowAccessibleObject_Navigate_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(() => accessibleObject.Navigate(AccessibleNavigation.Right));
    }

    [Theory]
    [InlineData(AccessibleNavigation.Down)]
    [InlineData(AccessibleNavigation.FirstChild)]
    [InlineData(AccessibleNavigation.LastChild)]
    [InlineData(AccessibleNavigation.Next)]
    [InlineData(AccessibleNavigation.Previous)]
    [InlineData(AccessibleNavigation.Up)]
    public void DataGridViewRowAccessibleObject_Navigate_NoDataGridView_ThrowsNullReferenceException(AccessibleNavigation navigationDirection)
    {
        using DataGridViewRow owner = new();
        DataGridViewRowAccessibleObject accessibleObject = new(owner);

        Assert.Null(accessibleObject.Navigate(navigationDirection));
    }

    [Theory]
    [InlineData(AccessibleSelection.None)]
    [InlineData(AccessibleSelection.TakeSelection)]
    [InlineData(AccessibleSelection.AddSelection)]
    [InlineData(AccessibleSelection.AddSelection | AccessibleSelection.RemoveSelection)]
    [InlineData(AccessibleSelection.TakeSelection | AccessibleSelection.RemoveSelection)]
    [InlineData(AccessibleSelection.RemoveSelection)]
    [InlineData(AccessibleSelection.TakeFocus)]
    public void DataGridViewRowAccessibleObject_Select_NoDataGridView_Nop(AccessibleSelection flags)
    {
        using DataGridViewRow owner = new();
        DataGridViewRowAccessibleObject accessibleObject = new(owner);

        accessibleObject.Select(flags);
    }

    [Theory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewRowAccessibleObject_Select_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(() => accessibleObject.Select(AccessibleSelection.None));
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentRoot_ReturnsNull_WithoutDataGridView()
    {
        using DataGridViewRow dataGridViewRow = new();

        Assert.Null(dataGridViewRow.AccessibilityObject.FragmentRoot);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_IsEnabled_ReturnsFalse_WithoutDataGridView()
    {
        using DataGridViewRow dataGridViewRow = new();
        bool actualValue = (bool)dataGridViewRow.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId);

        Assert.False(actualValue);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Sibling_ReturnExpected()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = dataGridView.Rows[3].AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject4, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject3, accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(topRowAccessibleObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Sibling_ReturnExpected_IfFirstRowHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[0].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = dataGridView.Rows[3].AccessibilityObject;

        Assert.Equal(accessibleObject2, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject4, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject3, accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(topRowAccessibleObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Sibling_ReturnExpected_IfSecondRowHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[1].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = dataGridView.Rows[3].AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject3, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject4, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject3, accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject1, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(topRowAccessibleObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Sibling_ReturnExpected_IfLastRowHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject4 = dataGridView.Rows[3].AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject4, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject2, accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(topRowAccessibleObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Sibling_ReturnExpected_IfSpecialRowHidden()
    {
        using DataGridView dataGridView = new() { ColumnHeadersVisible = false, AllowUserToAddRows = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;

        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Sibling_ReturnExpected_IfFirstRowAndSpecialRowHidden()
    {
        using DataGridView dataGridView = new() { ColumnHeadersVisible = false, AllowUserToAddRows = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[0].Visible = false;

        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;

        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Sibling_ReturnExpected_IfSecondRowAndSpecialRowHidden()
    {
        using DataGridView dataGridView = new() { ColumnHeadersVisible = false, AllowUserToAddRows = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[1].Visible = false;

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;

        Assert.Equal(accessibleObject3, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject1, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Sibling_ReturnExpected_IfLastRowAndSpecialRowHidden()
    {
        using DataGridView dataGridView = new() { ColumnHeadersVisible = false, AllowUserToAddRows = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[2].Visible = false;

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;

        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Sibling_ReturnExpected_IfColumnHeadersHidden()
    {
        using DataGridView dataGridView = new() { ColumnHeadersVisible = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = dataGridView.Rows[3].AccessibilityObject;

        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject4, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject3, accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Sibling_ReturnExpected_IfFirstRowAndColumnHeadersHidden()
    {
        using DataGridView dataGridView = new() { ColumnHeadersVisible = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");

        dataGridView.Rows[0].Visible = false;

        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = dataGridView.Rows[3].AccessibilityObject;

        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject4, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject3, accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Sibling_ReturnExpected_IfSecondRowAndColumnHeadersHidden()
    {
        using DataGridView dataGridView = new() { ColumnHeadersVisible = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");

        dataGridView.Rows[1].Visible = false;

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = dataGridView.Rows[3].AccessibilityObject;

        Assert.Equal(accessibleObject3, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject4, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject3, accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject1, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Sibling_ReturnExpected_IfLastRowAndColumnHeadersHidden()
    {
        using DataGridView dataGridView = new() { ColumnHeadersVisible = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");

        dataGridView.Rows[2].Visible = false;

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject4 = dataGridView.Rows[3].AccessibilityObject;

        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject4, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject2, accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Sibling_ReturnExpected_IfUserRowHidden()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(topRowAccessibleObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Sibling_ReturnExpected_IfFirstRowAndUserRowHidden()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[0].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;

        Assert.Equal(accessibleObject2, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(topRowAccessibleObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Sibling_ReturnExpected_IfSecondRowAndUserRowHidden()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[1].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject3, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject1, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(topRowAccessibleObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Sibling_ReturnExpected_IfLastRowAndUserRowHidden()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(topRowAccessibleObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(topRowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Sibling_ReturnExpected()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = dataGridView.Rows[3].AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject2, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject3, accessibleObject2.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject4, accessibleObject3.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject4.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject3, accessibleObject4.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject2, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject1, accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(topRowAccessibleObject, accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.Null(topRowAccessibleObject.Navigate(AccessibleNavigation.Previous));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Sibling_ReturnExpected_IfFirstRowHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[0].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = dataGridView.Rows[3].AccessibilityObject;

        Assert.Equal(accessibleObject2, topRowAccessibleObject.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject3, accessibleObject2.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject4, accessibleObject3.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject4.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject3, accessibleObject4.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject2, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(topRowAccessibleObject, accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.Null(topRowAccessibleObject.Navigate(AccessibleNavigation.Previous));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Sibling_ReturnExpected_IfSecondRowHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[1].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = dataGridView.Rows[3].AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject3, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject4, accessibleObject3.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject4.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject3, accessibleObject4.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject1, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(topRowAccessibleObject, accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.Null(topRowAccessibleObject.Navigate(AccessibleNavigation.Previous));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Sibling_ReturnExpected_IfLastRowHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject4 = dataGridView.Rows[3].AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject2, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject4, accessibleObject2.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject4.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject2, accessibleObject4.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject1, accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(topRowAccessibleObject, accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.Null(topRowAccessibleObject.Navigate(AccessibleNavigation.Previous));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Sibling_ReturnExpected_IfSpecialRowHidden()
    {
        using DataGridView dataGridView = new() { ColumnHeadersVisible = false, AllowUserToAddRows = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;

        Assert.Equal(accessibleObject2, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject3, accessibleObject2.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject3.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject2, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject1, accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Sibling_ReturnExpected_IfFirstRowAndSpecialRowHidden()
    {
        using DataGridView dataGridView = new() { ColumnHeadersVisible = false, AllowUserToAddRows = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[0].Visible = false;

        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;

        Assert.Equal(accessibleObject3, accessibleObject2.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject3.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject2, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Sibling_ReturnExpected_IfSecondRowAndSpecialRowHidden()
    {
        using DataGridView dataGridView = new() { ColumnHeadersVisible = false, AllowUserToAddRows = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[1].Visible = false;

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;

        Assert.Equal(accessibleObject3, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject3.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject1, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Sibling_ReturnExpected_IfLastRowAndSpecialRowHidden()
    {
        using DataGridView dataGridView = new() { ColumnHeadersVisible = false, AllowUserToAddRows = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[2].Visible = false;

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;

        Assert.Equal(accessibleObject2, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject2.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject1, accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Sibling_ReturnExpected_IfColumnHeadersHidden()
    {
        using DataGridView dataGridView = new() { ColumnHeadersVisible = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = dataGridView.Rows[3].AccessibilityObject;

        Assert.Equal(accessibleObject2, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject3, accessibleObject2.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject4, accessibleObject3.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject4.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject3, accessibleObject4.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject2, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject1, accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Sibling_ReturnExpected_IfFirstRowAndColumnHeadersHidden()
    {
        using DataGridView dataGridView = new() { ColumnHeadersVisible = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");

        dataGridView.Rows[0].Visible = false;

        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = dataGridView.Rows[3].AccessibilityObject;

        Assert.Equal(accessibleObject3, accessibleObject2.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject4, accessibleObject3.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject4.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject3, accessibleObject4.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject2, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Sibling_ReturnExpected_IfSecondRowAndColumnHeadersHidden()
    {
        using DataGridView dataGridView = new() { ColumnHeadersVisible = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");

        dataGridView.Rows[1].Visible = false;

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = dataGridView.Rows[3].AccessibilityObject;

        Assert.Equal(accessibleObject3, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject4, accessibleObject3.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject4.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject3, accessibleObject4.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject1, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Sibling_ReturnExpected_IfLastRowAndColumnHeadersHidden()
    {
        using DataGridView dataGridView = new() { ColumnHeadersVisible = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");

        dataGridView.Rows[2].Visible = false;

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject4 = dataGridView.Rows[3].AccessibilityObject;

        Assert.Equal(accessibleObject2, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject4, accessibleObject2.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject4.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject2, accessibleObject4.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject1, accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.Null(accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Sibling_ReturnExpected_IfUserRowHidden()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject2, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject3, accessibleObject2.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject3.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject2, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(accessibleObject1, accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(topRowAccessibleObject, accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.Null(topRowAccessibleObject.Navigate(AccessibleNavigation.Previous));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Sibling_ReturnExpected_IfFirstRowAndUserRowHidden()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[0].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;

        Assert.Equal(accessibleObject2, topRowAccessibleObject.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject3, accessibleObject2.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject3.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject2, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(topRowAccessibleObject, accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.Null(topRowAccessibleObject.Navigate(AccessibleNavigation.Previous));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Sibling_ReturnExpected_IfSecondRowAndUserRowHidden()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[1].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject3, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject3.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject1, accessibleObject3.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(topRowAccessibleObject, accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.Null(topRowAccessibleObject.Navigate(AccessibleNavigation.Previous));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Sibling_ReturnExpected_IfLastRowAndUserRowHidden()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[2].Visible = false;

        AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].AccessibilityObject;

        Assert.Equal(accessibleObject1, topRowAccessibleObject.Navigate(AccessibleNavigation.Next));
        Assert.Equal(accessibleObject2, accessibleObject1.Navigate(AccessibleNavigation.Next));
        Assert.Null(accessibleObject2.Navigate(AccessibleNavigation.Next));

        Assert.Equal(accessibleObject1, accessibleObject2.Navigate(AccessibleNavigation.Previous));
        Assert.Equal(topRowAccessibleObject, accessibleObject1.Navigate(AccessibleNavigation.Previous));
        Assert.Null(topRowAccessibleObject.Navigate(AccessibleNavigation.Previous));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_ReturnsExpected_IfColumnsHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns[0].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Child_ReturnsNull_IfRowHeadersAndColumnsHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns[0].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;

        Assert.Null(rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfRowHeadersAndFirstColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(accessibleObject2, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject3, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfRowHeadersAndSecondColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[1].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(accessibleObject1, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject3, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfRowHeadersAndLastColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;

        Assert.Equal(accessibleObject1, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject2, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfRowHeadersAndAllColumnsHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;

        Assert.Null(rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfFirstColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfSecondColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[1].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfLastColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Rows[0].Cells[1].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfAllColumnsHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfCustomOrderAndRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[2].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(accessibleObject1, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject2, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfCustomOrderAndFirstColumnAndRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[0].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[2].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;

        Assert.Equal(accessibleObject1, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject2, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfCustomOrderAndSecondColumnAndRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[1].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[2].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(accessibleObject1, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject2, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfCustomOrderAndLastColumnAndRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(accessibleObject1, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject2, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfCustomOrder()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfCustomOrderAndFirstColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[0].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Rows[0].Cells[1].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfCustomOrderAndSecondColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[1].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfCustomOrderAndLastColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject, rowAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Child_ReturnsNull_IfRowHeadersAndColumnsHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns[0].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;

        Assert.Null(rowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Null(rowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Child_ReturnsExpected_IfRowHeadersAndFirstColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(accessibleObject2, rowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject3, rowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Child_ReturnsExpected_IfRowHeadersAndSecondColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[1].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(accessibleObject1, rowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject3, rowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Child_ReturnsExpected_IfRowHeadersAndLastColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;

        Assert.Equal(accessibleObject1, rowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject2, rowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Child_ReturnsExpected_IfRowHeadersAndAllColumnsHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;

        Assert.Null(rowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Null(rowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Child_ReturnsExpected_IfFirstColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject, rowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Child_ReturnsExpected_IfSecondColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[1].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject, rowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Child_ReturnsExpected_IfLastColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Rows[0].Cells[1].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject, rowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Child_ReturnsExpected_IfAllColumnsHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Child_ReturnsExpected_IfCustomOrderAndRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[2].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(accessibleObject1, rowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject2, rowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Child_ReturnsExpected_IfCustomOrderAndFirstColumnAndRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[0].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[2].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;

        Assert.Equal(accessibleObject1, rowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject2, rowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Child_ReturnsExpected_IfCustomOrderAndSecondColumnAndRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[1].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[2].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(accessibleObject1, rowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject2, rowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Child_ReturnsExpected_IfCustomOrderAndLastColumnAndRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(accessibleObject1, rowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject2, rowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Child_ReturnsExpected_IfCustomOrder()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject, rowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Child_ReturnsExpected_IfCustomOrderAndFirstColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[0].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Rows[0].Cells[1].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject, rowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Child_ReturnsExpected_IfCustomOrderAndSecondColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[1].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject, rowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_Navigate_Child_ReturnsExpected_IfCustomOrderAndLastColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(accessibleObject, rowAccessibleObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChild_ReturnsExpected_IfRowHeadersAndColumnsHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;

        Assert.Null(rowAccessibleObject.GetChild(0));
        Assert.Null(rowAccessibleObject.GetChild(1));
        Assert.Null(rowAccessibleObject.GetChild(2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChild_ReturnsExpected_IfRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(accessibleObject1, rowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject2, rowAccessibleObject.GetChild(1));
        Assert.Equal(accessibleObject3, rowAccessibleObject.GetChild(2));
        Assert.Null(rowAccessibleObject.GetChild(3));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChild_ReturnsExpected_IfRowHeadersAndFirstColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(accessibleObject2, rowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject3, rowAccessibleObject.GetChild(1));
        Assert.Null(rowAccessibleObject.GetChild(2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChild_ReturnsExpected_IfRowHeadersAndSecondColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[1].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(accessibleObject1, rowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject3, rowAccessibleObject.GetChild(1));
        Assert.Null(rowAccessibleObject.GetChild(2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChild_ReturnsExpected_IfRowHeadersAndLastColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;

        Assert.Equal(accessibleObject1, rowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject2, rowAccessibleObject.GetChild(1));
        Assert.Null(rowAccessibleObject.GetChild(2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChild_ReturnsExpected()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject1, rowAccessibleObject.GetChild(1));
        Assert.Equal(accessibleObject2, rowAccessibleObject.GetChild(2));
        Assert.Equal(accessibleObject3, rowAccessibleObject.GetChild(3));
        Assert.Null(rowAccessibleObject.GetChild(4));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChild_ReturnsExpected_IfAllColumnsHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.GetChild(0));
        Assert.Null(rowAccessibleObject.GetChild(1));
        Assert.Null(rowAccessibleObject.GetChild(2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChild_ReturnsExpected_IfFirstColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject2, rowAccessibleObject.GetChild(1));
        Assert.Equal(accessibleObject3, rowAccessibleObject.GetChild(2));
        Assert.Null(rowAccessibleObject.GetChild(3));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChild_ReturnsExpected_IfSecondColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[1].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject1, rowAccessibleObject.GetChild(1));
        Assert.Equal(accessibleObject3, rowAccessibleObject.GetChild(2));
        Assert.Null(rowAccessibleObject.GetChild(3));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChild_ReturnsExpected_IfThirdColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject1, rowAccessibleObject.GetChild(1));
        Assert.Equal(accessibleObject2, rowAccessibleObject.GetChild(2));
        Assert.Null(rowAccessibleObject.GetChild(3));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChild_ReturnsExpected_IfCustomOrderAndRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[2].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(accessibleObject1, rowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject2, rowAccessibleObject.GetChild(1));
        Assert.Equal(accessibleObject3, rowAccessibleObject.GetChild(2));
        Assert.Null(rowAccessibleObject.GetChild(3));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChild_ReturnsExpected_IfCustomOrderAndRowHeadersAndFirstColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[0].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[2].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;

        Assert.Equal(accessibleObject1, rowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject2, rowAccessibleObject.GetChild(1));
        Assert.Null(rowAccessibleObject.GetChild(2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChild_ReturnsExpected_IfCustomOrderAndRowHeadersAndSecondColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[1].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[2].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(accessibleObject1, rowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject3, rowAccessibleObject.GetChild(1));
        Assert.Null(rowAccessibleObject.GetChild(2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChild_ReturnsExpected_IfCustomOrderAndRowHeadersAndLastColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(accessibleObject2, rowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject3, rowAccessibleObject.GetChild(1));
        Assert.Null(rowAccessibleObject.GetChild(2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChild_ReturnsExpected_IfCustomOrder()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[2].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject1, rowAccessibleObject.GetChild(1));
        Assert.Equal(accessibleObject2, rowAccessibleObject.GetChild(2));
        Assert.Equal(accessibleObject3, rowAccessibleObject.GetChild(3));
        Assert.Null(rowAccessibleObject.GetChild(4));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChild_ReturnsExpected_IfCustomOrderAndFirstColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[0].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[2].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject1, rowAccessibleObject.GetChild(1));
        Assert.Equal(accessibleObject2, rowAccessibleObject.GetChild(2));
        Assert.Null(rowAccessibleObject.GetChild(3));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChild_ReturnsExpected_IfCustomOrderAndSecondColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[1].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[2].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject1, rowAccessibleObject.GetChild(1));
        Assert.Equal(accessibleObject3, rowAccessibleObject.GetChild(2));
        Assert.Null(rowAccessibleObject.GetChild(3));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChild_ReturnsExpected_IfCustomOrderAndLastColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;
        AccessibleObject topLeftAccessibilityObject = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(topLeftAccessibilityObject, rowAccessibleObject.GetChild(0));
        Assert.Equal(accessibleObject2, rowAccessibleObject.GetChild(1));
        Assert.Equal(accessibleObject3, rowAccessibleObject.GetChild(2));
        Assert.Null(rowAccessibleObject.GetChild(3));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChildCount_ReturnsFour()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;

        Assert.Equal(4, rowAccessibleObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChildCount_ReturnsThree_IfOneColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;

        Assert.Equal(3, rowAccessibleObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChildCount_ReturnsTwo_IfTwoColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;

        Assert.Equal(2, rowAccessibleObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChildCount_ReturnsOne_IfAllColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;

        Assert.Equal(1, rowAccessibleObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChildCount_ReturnsThreeIfRowHeadersHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;

        Assert.Equal(3, rowAccessibleObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChildCount_ReturnsTwo_IfRowHeadersAndOneColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;

        Assert.Equal(2, rowAccessibleObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChildCount_ReturnsOne_IfRowHeadersAndTwoColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;

        Assert.Equal(1, rowAccessibleObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetChildCount_ReturnsZero_IfRowHeadersAndAllColumnHidden()
    {
        using DataGridView dataGridView = new() { RowHeadersVisible = false };
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowAccessibleObject = dataGridView.Rows[0].AccessibilityObject;

        Assert.Equal(0, rowAccessibleObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewRowAccessibleObject_GetPropertyValue_ValueValuePropertyId_ReturnsExpected()
    {
        using DataGridView dataGridView = new();
        dataGridView.AutoGenerateColumns = false;
        DataGridViewTextBoxColumn column = new()
        {
            DataPropertyName = "col1"
        };
        dataGridView.Columns.Add(column);
        dataGridView.Rows.Add(new DataGridViewRow());
        dataGridView.Rows[0].Cells[0].Value = "test1";

        Assert.Equal("test1", ((BSTR)dataGridView.Rows[0].AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId)).ToStringAndFree());
        Assert.False(dataGridView.IsHandleCreated);
    }

    // Unit test for https://github.com/dotnet/winforms/issues/7154
    [WinFormsTheory]
    [InlineData([false, 1])]
    [InlineData([true, 0])]
    public void DataGridView_SwitchConfigured_AdjustsRowStartIndices(bool switchValue, int expectedIndex)
    {
        using DataGridViewUIAStartRowCountAtZeroScope scope = new(switchValue);
        using DataGridView dataGridView = new DataGridView();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add(new DataGridViewRow());

        Assert.Equal(string.Format(SR.DataGridView_AccRowName, expectedIndex), dataGridView.Rows[0].AccessibilityObject.Name);
    }

    private class SubDataGridViewCell : DataGridViewCell
    {
    }
}
