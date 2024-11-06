// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class DataGridViewAccessibleObjectTests
{
    [WinFormsFact]
    public void DataGridViewAccessibleObject_Ctor_Default()
    {
        using DataGridView dataGridView = new();

        AccessibleObject accessibleObject = dataGridView.AccessibilityObject;

        Assert.NotNull(accessibleObject);
        Assert.Equal(AccessibleRole.Table, accessibleObject.Role);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_ItemStatus_ReturnsAsSorted()
    {
        using DataGridView dataGridView = new();
        DataGridViewTextBoxColumn column = new()
        {
            SortMode = DataGridViewColumnSortMode.Programmatic,
            HeaderText = "Some column"
        };

        dataGridView.Columns.Add(column);
        dataGridView.Sort(dataGridView.Columns[0], ListSortDirection.Ascending);

        string itemStatus = ((BSTR)dataGridView.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ItemStatusPropertyId)).ToStringAndFree();
        string expectedStatus = "Sorted ascending by Some column.";

        Assert.Equal(expectedStatus, itemStatus);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_ItemStatus_ReturnsNotSorted_IfSortedColumnInvisible()
    {
        using DataGridView dataGridView = new();
        DataGridViewTextBoxColumn column1 = new()
        {
            SortMode = DataGridViewColumnSortMode.Programmatic,
            HeaderText = "Sortable"
        };

        DataGridViewTextBoxColumn column2 = new()
        {
            SortMode = DataGridViewColumnSortMode.NotSortable,
            HeaderText = "Not Sortable"
        };

        dataGridView.Columns.Add(column1);
        dataGridView.Columns.Add(column2);
        dataGridView.Sort(column1, ListSortDirection.Ascending);
        column1.Visible = false;

        string itemStatus = ((BSTR)dataGridView.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ItemStatusPropertyId)).ToStringAndFree();

        Assert.Equal(SR.NotSortedAccessibleStatus, itemStatus);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_EmptyGrid_GetChildCount_ReturnsCorrectValue()
    {
        using DataGridView dataGridView = new();
        AccessibleObject accessibleObject = dataGridView.AccessibilityObject;

        Assert.Equal(0, accessibleObject.GetChildCount()); // dataGridView doesn't have items
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GridWithFirstRowOnly_GetChildCount_ReturnsCorrectValue()
    {
        using DataGridView dataGridView = new();
        AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.ColumnHeadersVisible = false;

        Assert.Equal(1, accessibleObject.GetChildCount()); // A first row only
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GridWithColumnHeadersAndFirstRow_GetChildCount_ReturnsCorrectValue()
    {
        using DataGridView dataGridView = new();
        AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());

        Assert.Equal(2, accessibleObject.GetChildCount()); // Column headers and a first Row
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_EmptyGrid_GetChild_ReturnsCorrectValue()
    {
        using DataGridView dataGridView = new();
        AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
        Assert.Equal(0, accessibleObject.GetChildCount()); // dataGridView doesn't have an item

        Assert.Null(accessibleObject.GetChild(0)); // GetChild method should not throw an exception
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GridWithFirstRowOnly_GetChild_ReturnsCorrectValue()
    {
        using DataGridView dataGridView = new();
        AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.ColumnHeadersVisible = false;

        Assert.NotNull(accessibleObject.GetChild(0)); // dataGridView a first empty row.
        Assert.Null(accessibleObject.GetChild(1)); // GetChild method should not throw an exception
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GridWithColumnHeadersAndFirstRow_GetChild_ReturnsCorrectValue()
    {
        using DataGridView dataGridView = new();
        AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());

        Assert.NotNull(accessibleObject.GetChild(0)); // dataGridView column headers
        Assert.NotNull(accessibleObject.GetChild(1)); // dataGridView a first empty row
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_Bounds_ReturnsCorrectValue_IfHandleIsCreated()
    {
        using DataGridView dataGridView = new();
        dataGridView.CreateControl();
        dataGridView.Size = new Size(500, 300);
        AccessibleObject accessibleObject = dataGridView.AccessibilityObject;

        int actualWidth = accessibleObject.Bounds.Width;
        int expectedWidth = dataGridView.Size.Width;
        Assert.Equal(expectedWidth, actualWidth);

        int actualHeight = accessibleObject.Bounds.Height;
        int expectedHeight = dataGridView.Size.Height;
        Assert.Equal(expectedHeight, actualHeight);

        Rectangle actualBounds = accessibleObject.Bounds;
        actualBounds.Location = new Point(0, 0);
        Rectangle expectedBounds = dataGridView.Bounds;

        Assert.Equal(expectedBounds, actualBounds);
        Assert.True(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_Bounds_ReturnsCorrectValue_IfHandleIsNotCreated()
    {
        using DataGridView dataGridView = new();
        dataGridView.Size = new Size(500, 300);
        AccessibleObject accessibleObject = dataGridView.AccessibilityObject;

        int actualWidth = accessibleObject.Bounds.Width;
        Assert.Equal(0, actualWidth);

        int actualHeight = accessibleObject.Bounds.Height;
        Assert.Equal(0, actualHeight);

        Rectangle actualBounds = accessibleObject.Bounds;
        actualBounds.Location = new Point(0, 0);
        Rectangle expectedBounds = dataGridView.Bounds;

        Assert.Equal(Rectangle.Empty, actualBounds);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_ControlType_IsDataGrid_IfAccessibleRoleIsDefault()
    {
        using DataGridView dataGridView = new();
        AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
        VARIANT actual = accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = UIA_CONTROLTYPE_ID.UIA_DataGridControlTypeId;

        Assert.False(dataGridView.IsHandleCreated);
        Assert.Equal(expected, (UIA_CONTROLTYPE_ID)(int)actual);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetFocused_ReturnsCorrectFocusedCell()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        PInvoke.SetFocus(dataGridView);

        DataGridViewCell cell = dataGridView.Rows[0].Cells[0];
        Assert.NotNull(cell);

        dataGridView.CurrentCell = cell;
        Assert.True(cell.Selected);

        AccessibleObject focusedCell = dataGridView.AccessibilityObject.GetFocused();

        Assert.NotNull(focusedCell);
        Assert.Equal(cell.AccessibilityObject, focusedCell);
        Assert.True(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetItem_ReturnsCorrectValue()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add();

        for (int i = 0; i < dataGridView.Rows.Count; i++)
        {
            for (int j = 0; j < dataGridView.Rows[i].Cells.Count; j++)
            {
                AccessibleObject expected = dataGridView.Rows[i].Cells[j].AccessibilityObject;
                AccessibleObject actual = (AccessibleObject)dataGridView.AccessibilityObject.GetItem(i, j);

                Assert.Equal(expected, actual);
            }
        }

        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleStates.Focusable)]
    [InlineData(false, AccessibleStates.None)]
    public void DataGridViewAccessibleObject_State_IsFocusable(bool createControl, AccessibleStates expectedAccessibleStates)
    {
        using DataGridView dataGridView = new();
        if (createControl)
        {
            dataGridView.CreateControl();
        }

        AccessibleObject accessibleObject = dataGridView.AccessibilityObject;

        Assert.Equal(createControl, dataGridView.IsHandleCreated);
        Assert.Equal(expectedAccessibleStates, accessibleObject.State & AccessibleStates.Focusable);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_Ctor_NullOwnerParameter_ThrowsArgumentNullException()
    {
        using DataGridView dataGridView = new();
        Type type = dataGridView.AccessibilityObject.GetType();

        ConstructorInfo ctor = type.GetConstructor([typeof(DataGridView)]);

        Assert.NotNull(ctor);
        Assert.Throws<TargetInvocationException>(() => ctor.Invoke([null]));
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_FirstAndLastChildren_AreNotNull()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        AccessibleObject accessibleObject = dataGridView.AccessibilityObject;

        // ColumnHeaders
        IRawElementProviderFragment.Interface firstChild = accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild);
        Assert.NotNull(firstChild);

        // New row
        IRawElementProviderFragment.Interface lastChild = accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);
        Assert.NotNull(lastChild);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsGridPatternAvailablePropertyId)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsTablePatternAvailablePropertyId)]
    public void DataGridViewAccessibleObject_Pattern_IsAvailable_IfDGVIsNotEmpty(int propertyId)
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        AccessibleObject accessibilityObject = dataGridView.AccessibilityObject;

        Assert.True((bool)accessibilityObject.GetPropertyValue((UIA_PROPERTY_ID)propertyId));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_TablePattern_IsNotAvailable_IfDGVIsEmpty()
    {
        using DataGridView dataGridView = new();
        AccessibleObject accessibilityObject = dataGridView.AccessibilityObject;

        Assert.False((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsTablePatternAvailablePropertyId));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewAccessibleObject_Cell_IsOffscreen_ReturnsCorrectValue(bool createControl)
    {
        using DataGridView dataGridView = new();

        if (createControl)
        {
            dataGridView.CreateControl();
        }

        AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
        dataGridView.Size = new Size(200, 100);
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());

        dataGridView.Rows.Add(); // 1
        VARIANT isOffscreen = dataGridView.Rows[1].Cells[0].AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId);
        Assert.False((bool)isOffscreen); // Within the visible area

        dataGridView.Rows.Add(); // 2
        dataGridView.Rows.Add(); // 3
        dataGridView.Rows.Add(); // 4
        isOffscreen = dataGridView.Rows[4].Cells[0].AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId);

        Assert.Equal(createControl, (bool)isOffscreen); // Out of the visible area
        Assert.Equal(createControl, dataGridView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_TablePatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_GridPatternId)]
    public void DataGridViewAccessibleObject_IsPatternSupported_IfDGVIsNotEmpty(int patternId)
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        AccessibleObject accessibleObject = dataGridView.AccessibilityObject;

        Assert.True(accessibleObject.IsPatternSupported((UIA_PATTERN_ID)patternId));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_TablePattern_IsNotSupported_IfDGVIsEmpty()
    {
        using DataGridView dataGridView = new();
        AccessibleObject accessibilityObject = dataGridView.AccessibilityObject;

        Assert.False(accessibilityObject.IsPatternSupported((UIA_PATTERN_ID)UIA_PROPERTY_ID.UIA_IsTablePatternAvailablePropertyId));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_TablePattern_IsNotSupported_IfColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns[0].Visible = false;
        AccessibleObject accessibilityObject = dataGridView.AccessibilityObject;

        Assert.False(accessibilityObject.IsPatternSupported((UIA_PATTERN_ID)UIA_PROPERTY_ID.UIA_IsTablePatternAvailablePropertyId));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewAccessibleObject_Cell_IsReadOnly_ReturnsCorrectValue(bool isReadOnly)
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());

        dataGridView.Rows[0].Cells[0].ReadOnly = isReadOnly;

        Assert.Equal(dataGridView.ReadOnly, dataGridView.AccessibilityObject.IsReadOnly);

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            Assert.Equal(row.ReadOnly, row.AccessibilityObject.IsReadOnly);

            foreach (DataGridViewCell cell in row.Cells)
            {
                Assert.Equal(cell.ReadOnly, cell.AccessibilityObject.IsReadOnly);
            }
        }

        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewAccessibleObject_Row_IsReadOnly_ReturnsCorrectValue(bool isReadOnly)
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());

        dataGridView.Rows[0].ReadOnly = isReadOnly;

        Assert.Equal(dataGridView.ReadOnly, dataGridView.AccessibilityObject.IsReadOnly);

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            Assert.Equal(row.ReadOnly, row.AccessibilityObject.IsReadOnly);

            foreach (DataGridViewCell cell in row.Cells)
            {
                Assert.Equal(cell.ReadOnly, cell.AccessibilityObject.IsReadOnly);
            }
        }

        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewAccessibleObject_Grid_IsReadOnly_ReturnsCorrectValue(bool isReadOnly)
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());

        dataGridView.ReadOnly = isReadOnly;

        Assert.Equal(dataGridView.ReadOnly, dataGridView.AccessibilityObject.IsReadOnly);

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            Assert.Equal(row.ReadOnly, row.AccessibilityObject.IsReadOnly);

            foreach (DataGridViewCell cell in row.Cells)
            {
                Assert.Equal(cell.ReadOnly, cell.AccessibilityObject.IsReadOnly);
            }
        }

        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_Owner_IsNotNull()
    {
        using DataGridView dataGridView = new();
        Control.ControlAccessibleObject accessibleObject = (Control.ControlAccessibleObject)dataGridView.AccessibilityObject;

        Assert.NotNull(accessibleObject.Owner);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_Parent_IsNotNull_IfHandleIsCreated()
    {
        using DataGridView dataGridView = new();
        dataGridView.CreateControl();
        AccessibleObject accessibleObject = dataGridView.AccessibilityObject;

        Assert.NotNull(accessibleObject.Parent);
        Assert.True(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_Parent_IsNotNull_IfHandleIsNotCreated()
    {
        using DataGridView dataGridView = new();
        AccessibleObject accessibleObject = dataGridView.AccessibilityObject;

        Assert.Null(accessibleObject.Parent);
        Assert.False(dataGridView.IsHandleCreated);
    }

    public static IEnumerable<object[]> DataGridViewAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(DataGridViewAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void DataGridViewAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using DataGridView dataGridView = new();
        dataGridView.AccessibleRole = role;

        VARIANT actual = dataGridView.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, (UIA_CONTROLTYPE_ID)(int)actual);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewAccessibleObject_GetPropertyValue_HasKeyboardFocus_IsExpected_ForEmptyDGV(bool focused)
    {
        using DataGridView dataGridView = new FakeFocusDataGridView(focused);

        bool actual = (bool)dataGridView.AccessibilityObject
            .GetPropertyValue(UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId);

        Assert.Equal(focused, dataGridView.Focused);
        Assert.Equal(0, dataGridView.RowCount); // DGV is empty
        Assert.Equal(focused, actual);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewAccessibleObject_GetPropertyValue_HasKeyboardFocus_False_ForNotEmptyDGV(bool focused)
    {
        using DataGridView dataGridView = new FakeFocusDataGridView(focused);
        dataGridView.Columns.Add(new DataGridViewButtonColumn());

        bool actual = (bool)dataGridView.AccessibilityObject
            .GetPropertyValue(UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId);

        Assert.Equal(focused, dataGridView.Focused);
        Assert.Equal(1, dataGridView.RowCount); // One new row for editing, it will be in focus instead of whole DGV
        Assert.False(actual);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewAccessibleObject_GetPropertyValue_HasKeyboardFocus_AsExpected_DependsOnForm(bool modal)
    {
        using Form form = new();
        using DataGridView dataGridView = new FakeFocusDataGridView(true);
        dataGridView.Columns.Add(new DataGridViewButtonColumn());
        dataGridView.Parent = form;

        bool? actualValue = null;
        form.Load += (_, _) =>
        {
            actualValue = (bool)dataGridView.AccessibilityObject
                .GetPropertyValue(UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId);
            form.Close();
        };
        if (modal)
        {
            form.ShowDialog();
        }
        else
        {
            form.Show();
        }

        Assert.Equal(modal, actualValue);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewAccessibleObject_GetPropertyValue_HasKeyboardFocus_IsExpected_ForGridWithHiddenRow(bool focused)
    {
        using DataGridView dataGridView = new FakeFocusDataGridView(focused) { AllowUserToAddRows = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test");
        dataGridView.Rows[0].Visible = false;

        bool actual = (bool)dataGridView.AccessibilityObject
            .GetPropertyValue(UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId);

        Assert.Equal(focused, dataGridView.Focused);
        Assert.Equal(focused, actual);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewAccessibleObject_GetPropertyValue_HasKeyboardFocus_IsExpected_ForNonEditableDGV(bool focused)
    {
        using DataGridView dataGridView = new FakeFocusDataGridView(focused)
        {
            ReadOnly = true,
            AllowUserToAddRows = false
        };

        bool actual = (bool)dataGridView.AccessibilityObject
            .GetPropertyValue(UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId);

        Assert.Equal(focused, dataGridView.Focused);
        Assert.Equal(focused, actual);
        Assert.False(dataGridView.IsHandleCreated);
    }

    private class FakeFocusDataGridView : DataGridView
    {
        private readonly bool _focused;

        public FakeFocusDataGridView(bool focused)
        {
            _focused = focused;
        }

        // Emulate the focus state to avoid creation of a form and the control's Handle
        public override bool Focused => _focused;
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetChildCount_ReturnOne_IfFirstRowAndColumnHeadersInvisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
            ColumnHeadersVisible = false
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");

        dataGridView.Rows[0].Visible = false;

        Assert.Equal(1, dataGridView.AccessibilityObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetChildCount_ReturnOne_IfSecondRowAndColumnHeadersInVisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
            ColumnHeadersVisible = false
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");

        dataGridView.Rows[1].Visible = false;

        Assert.Equal(1, dataGridView.AccessibilityObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetChildCount_ReturnZero_IfRowsAndColumnHeadersInVisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
            ColumnHeadersVisible = false
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");

        dataGridView.Rows[0].Visible = false;
        dataGridView.Rows[1].Visible = false;

        Assert.Equal(0, dataGridView.AccessibilityObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetChildCount_ReturnTwo_IfFirstRowInvisibleAndColumnHeadersVisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");

        dataGridView.Rows[0].Visible = false;

        Assert.Equal(2, dataGridView.AccessibilityObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetChildCount_ReturnTwo_IfSecondRowInvisibleAndColumnHeadersVisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");

        dataGridView.Rows[1].Visible = false;

        Assert.Equal(2, dataGridView.AccessibilityObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetChildCount_ReturnOne_IfRowsInvisibleAndColumnHeadersVisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");

        dataGridView.Rows[0].Visible = false;
        dataGridView.Rows[1].Visible = false;

        Assert.Equal(1, dataGridView.AccessibilityObject.GetChildCount());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetChild_ReturnExpected_IfFirstRowAndColumnHeadersInvisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
            ColumnHeadersVisible = false
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");

        dataGridView.Rows[0].Visible = false;

        Assert.Null(dataGridView.AccessibilityObject.GetChild(-1));
        Assert.Equal(dataGridView.Rows[1].AccessibilityObject, dataGridView.AccessibilityObject.GetChild(0));
        Assert.Null(dataGridView.AccessibilityObject.GetChild(1));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetChild_ReturnExpected_IfSecondRowAndColumnHeadersInvisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
            ColumnHeadersVisible = false
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");

        dataGridView.Rows[1].Visible = false;

        Assert.Null(dataGridView.AccessibilityObject.GetChild(-1));
        Assert.Equal(dataGridView.Rows[0].AccessibilityObject, dataGridView.AccessibilityObject.GetChild(0));
        Assert.Null(dataGridView.AccessibilityObject.GetChild(1));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetChild_ReturnNull_IfRowsAndColumnHeadersInVisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
            ColumnHeadersVisible = false
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");

        dataGridView.Rows[0].Visible = false;
        dataGridView.Rows[1].Visible = false;

        Assert.Null(dataGridView.AccessibilityObject.GetChild(-1));
        Assert.Null(dataGridView.AccessibilityObject.GetChild(0));
        Assert.Null(dataGridView.AccessibilityObject.GetChild(1));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetChild_ReturnExpected_IfFirstRowInvisibleAndColumnHeadersVisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");

        dataGridView.Rows[0].Visible = false;

        AccessibleObject topRowAccessibilityObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        Assert.Null(dataGridView.AccessibilityObject.GetChild(-1));
        Assert.Equal(topRowAccessibilityObject, dataGridView.AccessibilityObject.GetChild(0));
        Assert.Equal(dataGridView.Rows[1].AccessibilityObject, dataGridView.AccessibilityObject.GetChild(1));
        Assert.Null(dataGridView.AccessibilityObject.GetChild(2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetChild_ReturnExpected_IfSecondRowInvisibleAndColumnHeadersVisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");

        dataGridView.Rows[1].Visible = false;

        AccessibleObject topRowAccessibilityObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        Assert.Null(dataGridView.AccessibilityObject.GetChild(-1));
        Assert.Equal(topRowAccessibilityObject, dataGridView.AccessibilityObject.GetChild(0));
        Assert.Equal(dataGridView.Rows[0].AccessibilityObject, dataGridView.AccessibilityObject.GetChild(1));
        Assert.Null(dataGridView.AccessibilityObject.GetChild(2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetChild_ReturnExpected_IfRowsInvisibleAndColumnHeadersVisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");

        dataGridView.Rows[0].Visible = false;
        dataGridView.Rows[1].Visible = false;

        AccessibleObject topRowAccessibilityObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        Assert.Null(dataGridView.AccessibilityObject.GetChild(-1));
        Assert.Equal(topRowAccessibilityObject, dataGridView.AccessibilityObject.GetChild(0));
        Assert.Null(dataGridView.AccessibilityObject.GetChild(1));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_FragmentNavigate_Child_RetrunExpected_IfFirstRowAndColumnHeadersInvisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
            ColumnHeadersVisible = false
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");

        dataGridView.Rows[0].Visible = false;

        Assert.Equal(dataGridView.Rows[1].AccessibilityObject, dataGridView.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(dataGridView.Rows[1].AccessibilityObject, dataGridView.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_FragmentNavigate_Child_RetrunExpected_IfSecondRowAndColumnHeadersInvisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
            ColumnHeadersVisible = false
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");

        dataGridView.Rows[1].Visible = false;

        Assert.Equal(dataGridView.Rows[0].AccessibilityObject, dataGridView.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(dataGridView.Rows[0].AccessibilityObject, dataGridView.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_FragmentNavigate_Child_RetrunNull_IfRowsAndColumnHeadersInvisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
            ColumnHeadersVisible = false
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");

        dataGridView.Rows[0].Visible = false;
        dataGridView.Rows[1].Visible = false;

        Assert.Null(dataGridView.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(dataGridView.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_FragmentNavigate_Child_RetrunExpected_IfFirstRowInvisibleAndColumnHeadersVisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");
        AccessibleObject topRowAccessibilityObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        dataGridView.Rows[0].Visible = false;

        Assert.Equal(topRowAccessibilityObject, dataGridView.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(dataGridView.Rows[1].AccessibilityObject, dataGridView.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_FragmentNavigate_Child_RetrunExpected_IfSecondRowInvisibleAndColumnHeadersVisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");
        AccessibleObject topRowAccessibilityObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        dataGridView.Rows[1].Visible = false;

        Assert.Equal(topRowAccessibilityObject, dataGridView.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(dataGridView.Rows[0].AccessibilityObject, dataGridView.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_FragmentNavigate_Child_RetrunExpected_IfRowsInvisibleAndColumnHeadersVisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");
        AccessibleObject topRowAccessibilityObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        dataGridView.Rows[0].Visible = false;
        dataGridView.Rows[1].Visible = false;

        Assert.Equal(topRowAccessibilityObject, dataGridView.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(topRowAccessibilityObject, dataGridView.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_Navigate_Child_RetrunExpected_IfFirstRowAndColumnHeadersInvisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
            ColumnHeadersVisible = false
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");

        dataGridView.Rows[0].Visible = false;

        Assert.Equal(dataGridView.Rows[1].AccessibilityObject, dataGridView.AccessibilityObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(dataGridView.Rows[1].AccessibilityObject, dataGridView.AccessibilityObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_Navigate_Child_RetrunExpected_IfSecondRowAndColumnHeadersInvisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
            ColumnHeadersVisible = false
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");

        dataGridView.Rows[1].Visible = false;

        Assert.Equal(dataGridView.Rows[0].AccessibilityObject, dataGridView.AccessibilityObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(dataGridView.Rows[0].AccessibilityObject, dataGridView.AccessibilityObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_Navigate_Child_RetrunNull_IfRowsAndColumnHeadersInvisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
            ColumnHeadersVisible = false
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");

        dataGridView.Rows[0].Visible = false;
        dataGridView.Rows[1].Visible = false;

        Assert.Null(dataGridView.AccessibilityObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Null(dataGridView.AccessibilityObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_Navigate_Child_RetrunExpected_IfFirstRowInvisibleAndColumnHeadersVisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");
        AccessibleObject topRowAccessibilityObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        dataGridView.Rows[0].Visible = false;

        Assert.Equal(topRowAccessibilityObject, dataGridView.AccessibilityObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(dataGridView.Rows[1].AccessibilityObject, dataGridView.AccessibilityObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_Navigate_Child_RetrunExpected_IfSecondRowInvisibleAndColumnHeadersVisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");
        AccessibleObject topRowAccessibilityObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        dataGridView.Rows[1].Visible = false;

        Assert.Equal(topRowAccessibilityObject, dataGridView.AccessibilityObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(dataGridView.Rows[0].AccessibilityObject, dataGridView.AccessibilityObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_Navigate_Child_RetrunExpected_IfRowsInvisibleAndColumnHeadersVisible()
    {
        using DataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
        };

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");
        AccessibleObject topRowAccessibilityObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;

        dataGridView.Rows[0].Visible = false;
        dataGridView.Rows[1].Visible = false;

        Assert.Equal(topRowAccessibilityObject, dataGridView.AccessibilityObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(topRowAccessibilityObject, dataGridView.AccessibilityObject.Navigate(AccessibleNavigation.LastChild));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetColumnHeaders_ReturnExpected()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Test 1", "Test 1");
        dataGridView.Columns.Add("Test 2", "Test 2");
        dataGridView.Columns.Add("Test 3", "Test 3");

        var columnHeaders = dataGridView.AccessibilityObject.GetColumnHeaders();

        Assert.NotNull(columnHeaders);
        Assert.Equal(3, columnHeaders.Length);
        Assert.Equal(dataGridView.Columns[0].HeaderCell.AccessibilityObject, columnHeaders[0]);
        Assert.Equal(dataGridView.Columns[1].HeaderCell.AccessibilityObject, columnHeaders[1]);
        Assert.Equal(dataGridView.Columns[2].HeaderCell.AccessibilityObject, columnHeaders[2]);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetColumnHeaders_ReturnExpected_IfFirstColumnInvisible()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Test 1", "Test 1");
        dataGridView.Columns.Add("Test 2", "Test 2");
        dataGridView.Columns.Add("Test 3", "Test 3");
        dataGridView.Columns[0].Visible = false;

        var columnHeaders = dataGridView.AccessibilityObject.GetColumnHeaders();

        Assert.NotNull(columnHeaders);
        Assert.Equal(2, columnHeaders.Length);
        Assert.Equal(dataGridView.Columns[1].HeaderCell.AccessibilityObject, columnHeaders[0]);
        Assert.Equal(dataGridView.Columns[2].HeaderCell.AccessibilityObject, columnHeaders[1]);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetColumnHeaders_ReturnExpected_IfSecondColumnInvisible()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Test 1", "Test 1");
        dataGridView.Columns.Add("Test 2", "Test 2");
        dataGridView.Columns.Add("Test 3", "Test 3");
        dataGridView.Columns[1].Visible = false;

        var columnHeaders = dataGridView.AccessibilityObject.GetColumnHeaders();

        Assert.NotNull(columnHeaders);
        Assert.Equal(2, columnHeaders.Length);
        Assert.Equal(dataGridView.Columns[0].HeaderCell.AccessibilityObject, columnHeaders[0]);
        Assert.Equal(dataGridView.Columns[2].HeaderCell.AccessibilityObject, columnHeaders[1]);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetColumnHeaders_ReturnExpected_IfThirdColumnInvisible()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Test 1", "Test 1");
        dataGridView.Columns.Add("Test 2", "Test 2");
        dataGridView.Columns.Add("Test 3", "Test 3");
        dataGridView.Columns[2].Visible = false;

        var columnHeaders = dataGridView.AccessibilityObject.GetColumnHeaders();

        Assert.NotNull(columnHeaders);
        Assert.Equal(2, columnHeaders.Length);
        Assert.Equal(dataGridView.Columns[0].HeaderCell.AccessibilityObject, columnHeaders[0]);
        Assert.Equal(dataGridView.Columns[1].HeaderCell.AccessibilityObject, columnHeaders[1]);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetColumnHeaders_ReturnEmptyArray_IfColumnsInvisible()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Test 1", "Test 1");
        dataGridView.Columns.Add("Test 2", "Test 2");
        dataGridView.Columns.Add("Test 3", "Test 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;
        dataGridView.Columns[2].Visible = false;

        var columnHeaders = dataGridView.AccessibilityObject.GetColumnHeaders();

        Assert.NotNull(columnHeaders);
        Assert.Empty(columnHeaders);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetRowHeaders_ReturnExpected()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };
        dataGridView.Columns.Add("Test", "Test");
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");
        dataGridView.Rows.Add("Test 3");

        var rowHeaders = dataGridView.AccessibilityObject.GetRowHeaders();

        Assert.NotNull(rowHeaders);
        Assert.Equal(3, rowHeaders.Length);
        Assert.Equal(dataGridView.Rows[0].HeaderCell.AccessibilityObject, rowHeaders[0]);
        Assert.Equal(dataGridView.Rows[1].HeaderCell.AccessibilityObject, rowHeaders[1]);
        Assert.Equal(dataGridView.Rows[2].HeaderCell.AccessibilityObject, rowHeaders[2]);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetRowHeaders_ReturnExpected_IfFirstRowHidden()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };
        dataGridView.Columns.Add("Test", "Test");
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");
        dataGridView.Rows.Add("Test 3");
        dataGridView.Rows[0].Visible = false;

        var rowHeaders = dataGridView.AccessibilityObject.GetRowHeaders();

        Assert.NotNull(rowHeaders);
        Assert.Equal(2, rowHeaders.Length);
        Assert.Equal(dataGridView.Rows[1].HeaderCell.AccessibilityObject, rowHeaders[0]);
        Assert.Equal(dataGridView.Rows[2].HeaderCell.AccessibilityObject, rowHeaders[1]);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetRowHeaders_ReturnExpected_IfSecondRowHidden()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };
        dataGridView.Columns.Add("Test", "Test");
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");
        dataGridView.Rows.Add("Test 3");
        dataGridView.Rows[1].Visible = false;

        var rowHeaders = dataGridView.AccessibilityObject.GetRowHeaders();

        Assert.NotNull(rowHeaders);
        Assert.Equal(2, rowHeaders.Length);
        Assert.Equal(dataGridView.Rows[0].HeaderCell.AccessibilityObject, rowHeaders[0]);
        Assert.Equal(dataGridView.Rows[2].HeaderCell.AccessibilityObject, rowHeaders[1]);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetRowHeaders_ReturnExpected_IfThirdRowHidden()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };
        dataGridView.Columns.Add("Test", "Test");
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");
        dataGridView.Rows.Add("Test 3");
        dataGridView.Rows[2].Visible = false;

        var rowHeaders = dataGridView.AccessibilityObject.GetRowHeaders();

        Assert.NotNull(rowHeaders);
        Assert.Equal(2, rowHeaders.Length);
        Assert.Equal(dataGridView.Rows[0].HeaderCell.AccessibilityObject, rowHeaders[0]);
        Assert.Equal(dataGridView.Rows[1].HeaderCell.AccessibilityObject, rowHeaders[1]);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetRowHeaders_ReturnExpected_IfRowsHidden()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };
        dataGridView.Columns.Add("Test", "Test");
        dataGridView.Rows.Add("Test 1");
        dataGridView.Rows.Add("Test 2");
        dataGridView.Rows.Add("Test 3");
        dataGridView.Rows[0].Visible = false;
        dataGridView.Rows[1].Visible = false;
        dataGridView.Rows[2].Visible = false;

        var rowHeaders = dataGridView.AccessibilityObject.GetRowHeaders();

        Assert.NotNull(rowHeaders);
        Assert.Empty(rowHeaders);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetItem_ReturnsExpected_IfFirstColumnAndRowHidden()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add();
        dataGridView.Rows.Add();
        dataGridView.Rows.Add();
        dataGridView.Columns[0].Visible = false;
        dataGridView.Rows[0].Visible = false;

        int rowCount = dataGridView.Rows.Count;
        int columnCount = dataGridView.Columns.Count;

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                AccessibleObject expected = (i >= rowCount - 1 || j >= columnCount - 1)
                    ? null
                    : dataGridView.Rows[i + 1].Cells[j + 1].AccessibilityObject;

                Assert.Equal(expected, dataGridView.AccessibilityObject.GetItem(i, j));
            }
        }

        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetItem_ReturnsExpected_IfSecondColumnAndRowHidden()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add();
        dataGridView.Rows.Add();
        dataGridView.Rows.Add();
        dataGridView.Columns[1].Visible = false;
        dataGridView.Rows[1].Visible = false;

        int rowCount = dataGridView.Rows.Count;
        int columnCount = dataGridView.Columns.Count;

        AccessibleObject accessibleObject = dataGridView.AccessibilityObject;

        AccessibleObject expected = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        var actual = accessibleObject.GetItem(0, 0);
        Assert.Equal(expected, actual);

        expected = dataGridView.Rows[0].Cells[2].AccessibilityObject;
        actual = accessibleObject.GetItem(0, 1);
        Assert.Equal(expected, actual);

        Assert.Null(accessibleObject.GetItem(0, 2));

        expected = dataGridView.Rows[2].Cells[0].AccessibilityObject;
        actual = accessibleObject.GetItem(1, 0);
        Assert.Equal(expected, actual);

        expected = dataGridView.Rows[2].Cells[2].AccessibilityObject;
        actual = accessibleObject.GetItem(1, 1);
        Assert.Equal(expected, actual);

        Assert.Null(accessibleObject.GetItem(1, 2));
        Assert.Null(accessibleObject.GetItem(2, 0));
        Assert.Null(accessibleObject.GetItem(2, 1));
        Assert.Null(accessibleObject.GetItem(2, 2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetItem_ReturnsExpected_IfLastColumnAndRowHidden()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add();
        dataGridView.Rows.Add();
        dataGridView.Rows.Add();
        dataGridView.Columns[2].Visible = false;
        dataGridView.Rows[2].Visible = false;

        int rowCount = dataGridView.Rows.Count;
        int columnCount = dataGridView.Columns.Count;

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                AccessibleObject expected = (i >= rowCount - 1 || j >= columnCount - 1)
                    ? null
                    : dataGridView.Rows[i].Cells[j].AccessibilityObject;

                Assert.Equal(expected, dataGridView.AccessibilityObject.GetItem(i, j));
            }
        }

        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetItem_ReturnsExpected_IfCustomOrder()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;

        AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
        AccessibleObject accessibleObjectCell1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObjectCell2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObjectCell3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(accessibleObjectCell3, dataGridView.AccessibilityObject.GetItem(0, 0));
        Assert.Equal(accessibleObjectCell2, dataGridView.AccessibilityObject.GetItem(0, 1));
        Assert.Equal(accessibleObjectCell1, dataGridView.AccessibilityObject.GetItem(0, 2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetItem_ReturnsExpected_IfCustomOrderAndFirstColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[0].Visible = false;

        AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
        AccessibleObject accessibleObjectCell2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObjectCell3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(accessibleObjectCell3, dataGridView.AccessibilityObject.GetItem(0, 0));
        Assert.Equal(accessibleObjectCell2, dataGridView.AccessibilityObject.GetItem(0, 1));
        Assert.Null(dataGridView.AccessibilityObject.GetItem(0, 2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetItem_ReturnsExpected_IfCustomOrderAndSecondColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[1].Visible = false;

        AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
        AccessibleObject accessibleObjectCell1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObjectCell3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(accessibleObjectCell3, dataGridView.AccessibilityObject.GetItem(0, 0));
        Assert.Equal(accessibleObjectCell1, dataGridView.AccessibilityObject.GetItem(0, 1));
        Assert.Null(dataGridView.AccessibilityObject.GetItem(0, 2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_GetItem_ReturnsExpected_IfCustomOrderAndLastColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
        AccessibleObject accessibleObjectCell1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObjectCell2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;

        Assert.Equal(accessibleObjectCell2, dataGridView.AccessibilityObject.GetItem(0, 0));
        Assert.Equal(accessibleObjectCell1, dataGridView.AccessibilityObject.GetItem(0, 1));
        Assert.Null(dataGridView.AccessibilityObject.GetItem(0, 2));
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_ColumnCount_ReturnsExpected()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Test 1", "Test 1");
        dataGridView.Columns.Add("Test 2", "Test 2");
        dataGridView.Columns.Add("Test 3", "Test 3");

        Assert.Equal(3, dataGridView.AccessibilityObject.ColumnCount);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_ColumnCount_ReturnsExpected_IfOneColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Test 1", "Test 1");
        dataGridView.Columns.Add("Test 2", "Test 2");
        dataGridView.Columns.Add("Test 3", "Test 3");
        dataGridView.Columns[0].Visible = false;

        Assert.Equal(2, dataGridView.AccessibilityObject.ColumnCount);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_ColumnCount_ReturnsExpected_IfTwoColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Test 1", "Test 1");
        dataGridView.Columns.Add("Test 2", "Test 2");
        dataGridView.Columns.Add("Test 3", "Test 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;

        Assert.Equal(1, dataGridView.AccessibilityObject.ColumnCount);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_ColumnCount_ReturnsExpected_IfThreeColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Test 1", "Test 1");
        dataGridView.Columns.Add("Test 2", "Test 2");
        dataGridView.Columns.Add("Test 3", "Test 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Columns[1].Visible = false;
        dataGridView.Columns[2].Visible = false;

        Assert.Equal(0, dataGridView.AccessibilityObject.ColumnCount);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_RowCount_ReturnsExpected()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Test 1", "Test 1");
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");

        Assert.Equal(4, dataGridView.AccessibilityObject.RowCount);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_RowCount_ReturnsExpected_IfOneColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Test 1", "Test 1");
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[0].Visible = false;

        Assert.Equal(3, dataGridView.AccessibilityObject.RowCount);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_RowCount_ReturnsExpected_IfTwoColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Test 1", "Test 1");
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[0].Visible = false;
        dataGridView.Rows[1].Visible = false;

        Assert.Equal(2, dataGridView.AccessibilityObject.RowCount);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_RowCount_ReturnsExpected_IfThreeColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Test 1", "Test 1");
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[0].Visible = false;
        dataGridView.Rows[1].Visible = false;
        dataGridView.Rows[2].Visible = false;

        Assert.Equal(1, dataGridView.AccessibilityObject.RowCount);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_RowCount_ReturnsExpected_IfUserRowHidden()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };
        dataGridView.Columns.Add("Test 1", "Test 1");
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");

        Assert.Equal(3, dataGridView.AccessibilityObject.RowCount);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_RowCount_ReturnsExpected_IfOneColumnHidden_IfUserRowHidden()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };
        dataGridView.Columns.Add("Test 1", "Test 1");
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[0].Visible = false;

        Assert.Equal(2, dataGridView.AccessibilityObject.RowCount);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_RowCount_ReturnsExpected_IfTwoColumnHidden_IfUserRowHidden()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };
        dataGridView.Columns.Add("Test 1", "Test 1");
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[0].Visible = false;
        dataGridView.Rows[1].Visible = false;

        Assert.Equal(1, dataGridView.AccessibilityObject.RowCount);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_RowCount_ReturnsExpected_IfThreeColumnHidden_IfUserRowHidden()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };
        dataGridView.Columns.Add("Test 1", "Test 1");
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[0].Visible = false;
        dataGridView.Rows[1].Visible = false;
        dataGridView.Rows[2].Visible = false;

        Assert.Equal(0, dataGridView.AccessibilityObject.RowCount);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_ItemStatusProperty_IsExpected_ForNonSortedDGV()
    {
        using DataGridView dataGridView = new();
        using DataGridViewTextBoxColumn column = new();
        dataGridView.Columns.Add(column);

        VARIANT actual = dataGridView.AccessibilityObject
            .GetPropertyValue(UIA_PROPERTY_ID.UIA_ItemStatusPropertyId);

        Assert.Equal(1, dataGridView.RowCount);
        Assert.Equal(1, dataGridView.ColumnCount);
        Assert.Equal(SR.NotSortedAccessibleStatus, ((BSTR)actual).ToStringAndFree());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewAccessibleObject_ItemStatusProperty_IsNull_ForEmptyDGV()
    {
        using DataGridView dataGridView = new() { AllowUserToAddRows = false };

        VARIANT actual = dataGridView.AccessibilityObject
            .GetPropertyValue(UIA_PROPERTY_ID.UIA_ItemStatusPropertyId);

        Assert.Equal(0, dataGridView.RowCount);
        Assert.Equal(0, dataGridView.ColumnCount);
        Assert.Equal(VARIANT.Empty, actual);
        Assert.False(dataGridView.IsHandleCreated);
    }
}
