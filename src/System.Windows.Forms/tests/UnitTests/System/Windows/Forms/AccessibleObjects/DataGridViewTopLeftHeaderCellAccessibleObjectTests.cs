// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class DataGridViewTopLeftHeaderCellAccessibleObjectTests : DataGridViewTopLeftHeaderCell
{
    [WinFormsFact]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_Ctor_Default()
    {
        DataGridViewTopLeftHeaderCellAccessibleObject accessibleObject = new(null!);

        Assert.Null(accessibleObject.Owner);
    }

    [WinFormsFact]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_Bounds_ThrowsException_IfOwnerIsNull()
    {
        Assert.Throws<InvalidOperationException>(() =>
            new DataGridViewTopLeftHeaderCellAccessibleObject(null!).Bounds);
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

    public static TheoryData<bool, string> DataGridViewTopLeftHeaderCellAccessibleObject_DefaultAction_TestData =>
        new TheoryData<bool, string>
        {
            { true, SR.DataGridView_AccTopLeftColumnHeaderCellDefaultAction },
            { false, string.Empty }
        };

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
    public void DataGridViewTopLeftHeaderCellAccessibleObject_DefaultAction_ThrowsInvalidOperationException_WhenOwnerIsNull()
    {
        DataGridViewTopLeftHeaderCellAccessibleObject accessibleObject = new(null!);

        Action action = () => _ = accessibleObject.DefaultAction;
        action.Should().Throw<InvalidOperationException>().WithMessage(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
    }

    [WinFormsFact]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_State_ThrowsInvalidOperationException_IfOwnerIsNull()
    {
        DataGridViewTopLeftHeaderCellAccessibleObject accessibleObject = new(null!);

        Action action = () => _ = accessibleObject.State;
        action.Should().Throw<InvalidOperationException>().WithMessage(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
    }

    [WinFormsFact]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_State_ReturnsSelectable()
    {
        using DataGridView control = new();
        using DataGridViewTopLeftHeaderCell cell = new();
        control.TopLeftHeaderCell = cell;

        AccessibleStates state = cell.AccessibilityObject.State;

        state.Should().HaveFlag(AccessibleStates.Selectable);
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
        DataGridViewTopLeftHeaderCellAccessibleObject accessibleObject = new(null!);

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

    public static TheoryData<RightToLeft, object?, string> DataGridViewTopLeftHeaderCellAccessibleObject_Name_TestData =>
        new TheoryData<RightToLeft, object?, string>
        {
            { RightToLeft.No, null, SR.DataGridView_AccTopLeftColumnHeaderCellName },
            { RightToLeft.Yes, null, SR.DataGridView_AccTopLeftColumnHeaderCellNameRTL },
            { RightToLeft.No, "It is not empty string", string.Empty },
            { RightToLeft.Yes, "It is not empty string", string.Empty }
        };

    [WinFormsTheory]
    [MemberData(nameof(DataGridViewTopLeftHeaderCellAccessibleObject_Name_TestData))]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_Name_ReturnsExpected(RightToLeft rightToLeft, object? value, string expected)
    {
        using DataGridView control = new();
        using DataGridViewTopLeftHeaderCell cell = new();

        control.TopLeftHeaderCell = cell;
        control.RightToLeft = rightToLeft;
        cell.Value = value;

        cell.AccessibilityObject.Name.Should().Be(expected);
        control.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsFact]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_Name_ThrowsInvalidOperationException_WhenOwnerIsNull()
    {
        DataGridViewTopLeftHeaderCellAccessibleObject accessibleObject = new(null!);

        Action action = () => _ = accessibleObject.Name;
        action.Should().Throw<InvalidOperationException>().WithMessage(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
    }

    [WinFormsFact]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_DoDefaultAction_SelectsAllCells()
    {
        using DataGridView control = new();
        using DataGridViewTopLeftHeaderCell cell = new();
        control.TopLeftHeaderCell = cell;
        control.CreateControl();

        cell.AccessibilityObject.DoDefaultAction();

        control.AreAllCellsSelected(false).Should().BeTrue();
    }

    [WinFormsFact]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_Navigate_ThrowsInvalidOperationException_IfOwnerIsNull()
    {
        DataGridViewTopLeftHeaderCellAccessibleObject accessibleObject = new(null!);

        Action action = () => accessibleObject.Navigate(AccessibleNavigation.Next);
        action.Should().Throw<InvalidOperationException>().WithMessage(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
    }

    [WinFormsFact]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_Navigate_ReturnsNull_IfDataGridViewIsNull()
    {
        using DataGridViewTopLeftHeaderCell cell = new();
        AccessibleObject accessibleObject = cell.AccessibilityObject;

        accessibleObject.Navigate(AccessibleNavigation.Next).Should().BeNull();
    }

    [WinFormsTheory]
    [InlineData(AccessibleNavigation.Previous)]
    [InlineData(AccessibleNavigation.Left)]
    [InlineData(AccessibleNavigation.Right)]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_Navigate_ReturnsNull_IfNoNavigationPossible(AccessibleNavigation direction)
    {
        using DataGridView control = new();
        using DataGridViewTopLeftHeaderCell cell = new();
        control.TopLeftHeaderCell = cell;
        control.CreateControl();

        AccessibleObject accessibleObject = cell.AccessibilityObject;

        accessibleObject.Navigate(direction).Should().BeNull();
    }

    [WinFormsFact]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_Navigate_ReturnsNextSibling()
    {
        using DataGridView control = new();
        using DataGridViewTopLeftHeaderCell cell = new();
        control.TopLeftHeaderCell = cell;
        control.Columns.Add(new DataGridViewTextBoxColumn());
        control.CreateControl();

        AccessibleObject accessibleObject = cell.AccessibilityObject;
        AccessibleObject expected = control.Columns[0].HeaderCell.AccessibilityObject;

        accessibleObject.Navigate(AccessibleNavigation.Next).Should().Be(expected);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewTopLeftHeaderCellAccessibleObject_FragmentNavigate_Parent_ReturnsExpected(bool createControl)
    {
        using DataGridView control = CreateDataGridView(columnCount: 0, createControl);
        using DataGridViewTopLeftHeaderCell cell = new();

        control.TopLeftHeaderCell = cell;
        AccessibleObject? expected = control.AccessibilityObject.GetChild(0);

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
        AccessibleObject? expected = control.AccessibilityObject.GetChild(0)?.GetChild(1);

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
