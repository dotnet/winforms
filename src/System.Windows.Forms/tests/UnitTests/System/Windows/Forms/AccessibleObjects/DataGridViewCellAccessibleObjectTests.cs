// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Moq;
using Moq.Protected;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

// NB: doesn't require thread affinity
public class DataGridViewCellAccessibleObjectTests : DataGridViewCell
{
    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Ctor_Default()
    {
        DataGridViewCellAccessibleObject accessibleObject = new();

        Assert.Null(accessibleObject.Owner);
        Assert.Equal(AccessibleRole.Cell, accessibleObject.Role);
    }

    public static IEnumerable<object[]> Ctor_DataGridViewCell_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new SubDataGridViewCell() };
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.No)]
    [InlineData(RightToLeft.Yes)]
    public void DataGridViewCellsAccessibleObject_Ctor_Default_IfHandleIsCreated(RightToLeft rightToLeft)
    {
        using DataGridView dataGridView = new()
        {
            RightToLeft = rightToLeft,
            ColumnCount = 4,
            Width = 85
        };

        dataGridView.CreateControl();
        dataGridView.Columns[0].Width = 40;
        dataGridView.Columns[1].Width = 40;
        dataGridView.Columns[2].Width = 40;
        dataGridView.Columns[3].Width = 40;

        AccessibleObject rr = dataGridView.AccessibilityObject; // it is necessary to be in time to initialize elements

        int accCellWidthSum = 0;
        for (int i = 0; i < 4; i++)
        {
            accCellWidthSum += dataGridView.Rows[0].Cells[i].AccessibilityObject.BoundingRectangle.Width;
        }

        int accRowWidth = dataGridView.Rows[0].AccessibilityObject.BoundingRectangle.Width;

        Assert.Equal(accCellWidthSum, accRowWidth - dataGridView.RowHeadersWidth);
        Assert.True(dataGridView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.No)]
    [InlineData(RightToLeft.Yes)]
    public void DataGridViewCellsAccessibleObject_Ctor_Default_IfHandleIsNotCreated(RightToLeft rightToLeft)
    {
        using DataGridView dataGridView = new()
        {
            RightToLeft = rightToLeft,
            ColumnCount = 4,
            Width = 85
        };

        dataGridView.Columns[0].Width = 40;
        dataGridView.Columns[1].Width = 40;
        dataGridView.Columns[2].Width = 40;
        dataGridView.Columns[3].Width = 40;

        AccessibleObject rr = dataGridView.AccessibilityObject; // it is necessary to be in time to initialize elements

        int accCellWidthSum = 0;
        for (int i = 0; i < 4; i++)
        {
            accCellWidthSum += dataGridView.Rows[0].Cells[i].AccessibilityObject.BoundingRectangle.Width;
        }

        int accRowWidth = dataGridView.Rows[0].AccessibilityObject.BoundingRectangle.Width;

        Assert.Equal(0, accCellWidthSum);
        Assert.Equal(0, accRowWidth);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_DataGridViewCell_TestData))]
    public void DataGridViewCellAccessibleObject_Ctor_DataGridViewCell(DataGridViewCell owner)
    {
        DataGridViewCellAccessibleObject accessibleObject = new(owner);

        Assert.Equal(owner, accessibleObject.Owner);
        Assert.Equal(AccessibleRole.Cell, accessibleObject.Role);
    }

    public static IEnumerable<object[]> Bounds_TestData()
    {
        DataGridViewRow row = new();
        SubDataGridViewCell cell = new();
        row.Cells.Add(cell);
        yield return new object[] { new DataGridViewCellAccessibleObject(cell), Rectangle.Empty };
    }

    [WinFormsTheory]
    [MemberData(nameof(Bounds_TestData))]
    public void DataGridViewCellAccessibleObject_Bounds_Get_ReturnsExpected(AccessibleObject accessibleObject, Rectangle expected)
    {
        Assert.Equal(expected, accessibleObject.Bounds);
    }

    public static IEnumerable<object[]> NoOwner_TestData()
    {
        yield return new object[] { new DataGridViewCellAccessibleObject() };
        yield return new object[] { new DataGridViewCellAccessibleObject(null) };
    }

    [WinFormsTheory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewCellAccessibleObject_Bounds_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(() => accessibleObject.Bounds);
    }

    public static IEnumerable<object[]> DefaultAction_TestData()
    {
        yield return new object[] { new DataGridViewCellAccessibleObject(new SubDataGridViewCell()), "Edit" };

        DataGridViewRow row = new();
        SubDataGridViewCell cell = new();
        row.Cells.Add(cell);
        cell.ReadOnly = true;
        yield return new object[] { new DataGridViewCellAccessibleObject(cell), string.Empty };
    }

    [WinFormsTheory]
    [MemberData(nameof(DefaultAction_TestData))]
    public void DataGridViewCellAccessibleObject_DefaultAction_Get_ReturnsExpected(AccessibleObject accessibleObject, string expected)
    {
        Assert.Equal(expected, accessibleObject.DefaultAction);
    }

    [WinFormsTheory]
    [MemberData(nameof(DefaultAction_TestData))]
    public void DataGridViewCellAccessibleObject_GetPropertyValue_LegacyIAccessibleDefaultActionPropertyId_ReturnsExpected(AccessibleObject accessibleObject, string expected)
    {
        if (!string.IsNullOrEmpty(expected))
        {
            Assert.Equal(expected, ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleDefaultActionPropertyId)).ToStringAndFree());
        }
        else
        {
            Assert.Equal(VARIANT.Empty, accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleDefaultActionPropertyId));
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewCellAccessibleObject_DefaultAction_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(() => accessibleObject.DefaultAction);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Help_Get_ReturnsNull()
    {
        DataGridViewCellAccessibleObject accessibleObject = new();

        Assert.Null(accessibleObject.Help);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Name_ReturnStringEmpty_IfOwningRowAndColumnNotExist()
    {
        SubDataGridViewCell cell = new();
        AccessibleObject accessibleObject = new DataGridViewCellAccessibleObject(cell);
        Assert.Equal(string.Empty, accessibleObject.Name);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Name_ReturnStringEmpty_IfOwningRowNotExist()
    {
        SubDataGridViewCell cell = new()
        {
            OwningColumn = new DataGridViewTextBoxColumn()
        };
        AccessibleObject accessibleObject = new DataGridViewCellAccessibleObject(cell);

        Assert.Equal(string.Empty, accessibleObject.Name);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Name_ReturnStringEmpty_IfOwningColumnNotExist()
    {
        SubDataGridViewCell cell = new()
        {
            OwningRow = new DataGridViewRow()
        };
        AccessibleObject accessibleObject = new DataGridViewCellAccessibleObject(cell);
        Assert.Equal(string.Empty, accessibleObject.Name);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Name_ReturnExpected_IfDataGridViewNotExist()
    {
        SubDataGridViewCell cell = new()
        {
            OwningRow = new DataGridViewRow(),
            OwningColumn = new DataGridViewTextBoxColumn() { HeaderText = "Test", SortMode = DataGridViewColumnSortMode.NotSortable }
        };
        AccessibleObject accessibleObject = new DataGridViewCellAccessibleObject(cell);
        string expected = string.Format(SR.DataGridView_AccDataGridViewCellName, cell.OwningColumn.HeaderText, -1);

        Assert.Equal(expected, accessibleObject.Name);
    }

    // Whether UIA row indexing is 1-based or 0-based, is controlled by the DataGridViewUIAStartRowCountAtZero switch
    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Name_ReturnExpected()
    {
        using DataGridView dataGridView = new();
        DataGridViewTextBoxColumn column = new() { HeaderText = "Test", SortMode = DataGridViewColumnSortMode.NotSortable };
        dataGridView.Columns.Add(column);
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");

        AccessibleObject accessibleObject = dataGridView.Rows[2].Cells[0].AccessibilityObject;
        string expected = string.Format(SR.DataGridView_AccDataGridViewCellName, column.HeaderText, 3);

        Assert.Equal(expected, accessibleObject.Name);
        Assert.False(dataGridView.IsHandleCreated);
    }

    // Whether UIA row indexing is 1-based or 0-based, is controlled by the DataGridViewUIAStartRowCountAtZero switch
    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Name_ReturnExpected_IfOneRowHidden()
    {
        using DataGridView dataGridView = new();
        DataGridViewTextBoxColumn column = new() { HeaderText = "Test", SortMode = DataGridViewColumnSortMode.NotSortable };
        dataGridView.Columns.Add(column);
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[0].Visible = false;

        AccessibleObject accessibleObject = dataGridView.Rows[2].Cells[0].AccessibilityObject;
        string expected = string.Format(SR.DataGridView_AccDataGridViewCellName, column.HeaderText, 2);

        Assert.Equal(expected, accessibleObject.Name);
        Assert.False(dataGridView.IsHandleCreated);
    }

    // Whether UIA row indexing is 1-based or 0-based, is controlled by the DataGridViewUIAStartRowCountAtZero switch
    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Name_ReturnExpected_IfTwoRowsHidden()
    {
        using DataGridView dataGridView = new();
        DataGridViewTextBoxColumn column = new() { HeaderText = "Test", SortMode = DataGridViewColumnSortMode.NotSortable };
        dataGridView.Columns.Add(column);
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[0].Visible = false;
        dataGridView.Rows[1].Visible = false;

        AccessibleObject accessibleObject = dataGridView.Rows[2].Cells[0].AccessibilityObject;
        string expected = string.Format(SR.DataGridView_AccDataGridViewCellName, column.HeaderText, 1);

        Assert.Equal(expected, accessibleObject.Name);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewCellAccessibleObject_Name_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(() => accessibleObject.Name);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Owner_Set_GetReturnsExpected()
    {
        using SubDataGridViewCell owner = new();
        DataGridViewCellAccessibleObject accessibleObject = new()
        {
            Owner = owner
        };

        Assert.Same(owner, accessibleObject.Owner);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Owner_SetAlreadyWithOwner_ThrowsInvalidOperationException()
    {
        using SubDataGridViewCell owner = new();
        DataGridViewCellAccessibleObject accessibleObject = new(owner);

        Assert.Throws<InvalidOperationException>(() => accessibleObject.Owner = owner);
    }

    public static IEnumerable<object[]> Parent_TestData()
    {
        yield return new object[] { new DataGridViewCellAccessibleObject(new SubDataGridViewCell()), null };

        DataGridViewRow row = new();
        SubDataGridViewCell cell = new();
        row.Cells.Add(cell);
        yield return new object[] { new DataGridViewCellAccessibleObject(cell), row.AccessibilityObject };
    }

    [WinFormsTheory]
    [MemberData(nameof(Parent_TestData))]
    public void DataGridViewCellAccessibleObject_Parent_Get_ReturnsExpected(AccessibleObject accessibleObject, AccessibleObject expected)
    {
        Assert.Equal(expected, accessibleObject.Parent);
    }

    [WinFormsTheory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewCellAccessibleObject_Parent_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(() => accessibleObject.Parent);
    }

    [WinFormsTheory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewCellAccessibleObject_State_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(() => accessibleObject.State);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_State_NoDataGridView_ReturnsExpected()
    {
        using SubDataGridViewCell owner = new();
        DataGridViewCellAccessibleObject accessibleObject = new(owner);

        Assert.Equal(AccessibleStates.Focusable | AccessibleStates.Selectable, accessibleObject.State);
    }

    public static IEnumerable<object[]> Value_TestData()
    {
        yield return new object[] { new DataGridViewCellAccessibleObject(new SubDataGridViewCell()), "(null)" };
    }

    [WinFormsTheory]
    [MemberData(nameof(Value_TestData))]
    public void DataGridViewCellAccessibleObject_Value_Get_ReturnsExpected(AccessibleObject accessibleObject, string expected)
    {
        Assert.Equal(expected, accessibleObject.Value);
    }

    [WinFormsTheory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewCellAccessibleObject_Value_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(() => accessibleObject.Value);
    }

    [WinFormsTheory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewCellAccessibleObject_DoDefaultAction_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(accessibleObject.DoDefaultAction);
    }

    [WinFormsTheory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewCellAccessibleObject_GetChild_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(() => accessibleObject.GetChild(-1));
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_GetChild_NoDataGridView_ReturnsNull()
    {
        using SubDataGridViewCell owner = new();
        DataGridViewCellAccessibleObject accessibleObject = new(owner);

        Assert.Null(accessibleObject.GetChild(-1));
    }

    [WinFormsTheory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewCellAccessibleObject_GetChildCount_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(() => accessibleObject.GetChildCount());
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_GetChildCount_NoDataGridView_ReturnsZero()
    {
        using SubDataGridViewCell owner = new();
        DataGridViewCellAccessibleObject accessibleObject = new(owner);

        Assert.Equal(0, accessibleObject.GetChildCount());
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_GetFocused_Invoke_ReturnsNull()
    {
        DataGridViewCellAccessibleObject accessibleObject = new();

        Assert.Null(accessibleObject.GetFocused());
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_GetSelected_Invoke_ReturnsNull()
    {
        DataGridViewCellAccessibleObject accessibleObject = new();

        Assert.Null(accessibleObject.GetSelected());
    }

    public static IEnumerable<object[]> Navigate_TestData()
    {
        yield return new object[] { new DataGridViewCellAccessibleObject(new SubDataGridViewCell()), AccessibleNavigation.Right, null };

        DataGridViewRow row = new();
        SubDataGridViewCell cell = new();
        row.Cells.Add(cell);
        yield return new object[] { new DataGridViewCellAccessibleObject(cell), AccessibleNavigation.Right, null };
    }

    [WinFormsTheory]
    [MemberData(nameof(Navigate_TestData))]
    public void DataGridViewCellAccessibleObject_Navigate_Invoke_ReturnsExpected(AccessibleObject accessibleObject, AccessibleNavigation navigationDirection, AccessibleObject expected)
    {
        Assert.Equal(expected, accessibleObject.Navigate(navigationDirection));
    }

    [WinFormsTheory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewCellAccessibleObject_Navigate_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(() => accessibleObject.Navigate(AccessibleNavigation.Right));
    }

    [WinFormsTheory]
    [InlineData(AccessibleSelection.None)]
    [InlineData(AccessibleSelection.RemoveSelection)]
    public void DataGridViewCellAccessibleObject_Select_NothingToDo_Nop(AccessibleSelection flags)
    {
        using SubDataGridViewCell owner = new();
        DataGridViewCellAccessibleObject accessibleObject = new(owner);
        accessibleObject.Select(flags);
    }

    [WinFormsTheory]
    [MemberData(nameof(NoOwner_TestData))]
    public void DataGridViewCellAccessibleObject_Select_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
    {
        Assert.Throws<InvalidOperationException>(() => accessibleObject.Select(AccessibleSelection.None));
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Select_HasSelectionFlagsWithoutValidDataGridView_DoesNothing()
    {
        Mock<DataGridViewCell> mockCell = new(MockBehavior.Strict);
        mockCell
            .SetupSet(s => s.State = DataGridViewElementStates.None)
            .Verifiable();
        mockCell
            .SetupSet(s => s.State = DataGridViewElementStates.Visible)
            .Verifiable();
        mockCell
            .Protected()
            .Setup("Dispose", ItExpr.IsAny<bool>());

        var mockObj = mockCell.Object;
        DataGridViewCellAccessibleObject accessibleObject = new(mockObj);
        accessibleObject.Select(AccessibleSelection.None);
        // NB: asserts are implicit - check that nothing was called on the mock that we didn't anticipate

        using DataGridView dataGridView = new();
        mockCell
            .Protected()
            .Setup("OnDataGridViewChanged");
        mockObj.DataGridView = dataGridView;

        accessibleObject.Select(AccessibleSelection.None);
        // NB: asserts are implicit - check that nothing was called on the mock that we didn't anticipate
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Select_TakeFocus()
    {
        using SubDataGridView dataGridView = new();
        dataGridView.CreateControl();
        Assert.True(dataGridView.IsHandleCreated);

        Mock<DataGridViewCell> mockCell = new(MockBehavior.Strict);
        mockCell
            .SetupSet(s => s.State = DataGridViewElementStates.None)
            .Verifiable();
        mockCell
            .SetupSet(s => s.State = DataGridViewElementStates.Visible)
            .Verifiable();
        mockCell
            .Protected()
            .Setup("Dispose", ItExpr.IsAny<bool>());
        mockCell
            .Protected()
            .Setup("OnDataGridViewChanged");

        var mockObj = mockCell.Object;
        mockObj.DataGridView = dataGridView;

        DataGridViewCellAccessibleObject accessibleObject = new(mockObj);
        // verify that we check for a flag, not direct comparison. 128 is an arbitrary large flag.
        accessibleObject.Select((AccessibleSelection)128 | AccessibleSelection.TakeFocus);

        // NB: some asserts are implicit - check that nothing was called on the mock that we didn't anticipate
        Assert.True(dataGridView.FocusCalled);
        Assert.True(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Select_TakeSelection()
    {
        using DataGridView dataGridView = new();
        dataGridView.CreateControl();
        Assert.True(dataGridView.IsHandleCreated);

        Mock<DataGridViewCell> mockCell = new(MockBehavior.Strict);
        mockCell
            .SetupSet(s => s.State = DataGridViewElementStates.None)
            .Verifiable();
        mockCell
            .SetupSet(s => s.State = DataGridViewElementStates.Visible)
            .Verifiable();
        mockCell
            .SetupSet(s => s.Selected = true)
            .Verifiable();
        mockCell
            .Protected()
            .Setup("Dispose", ItExpr.IsAny<bool>());
        mockCell
            .Protected()
            .Setup("OnDataGridViewChanged");

        var mockObj = mockCell.Object;
        mockObj.DataGridView = dataGridView;

        DataGridViewCellAccessibleObject accessibleObject = new(mockObj);
        // verify that we check for a flag, not direct comparison. 128 is an arbitrary large flag.
        accessibleObject.Select((AccessibleSelection)128 | AccessibleSelection.TakeSelection);

        // NB: some asserts are implicit - check that nothing was called on the mock that we didn't anticipate

        // Can't test whether CurrentCell was set unless we add the whole layer of Rows, Columns, etc.
        // Assert.Equal(mockObj, dataGridView.CurrentCell);

        Assert.True(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Select_AddSelection()
    {
        using DataGridView dataGridView = new();
        dataGridView.CreateControl();
        Assert.True(dataGridView.IsHandleCreated);

        Mock<DataGridViewCell> mockCell = new(MockBehavior.Strict);
        mockCell
            .SetupSet(s => s.State = DataGridViewElementStates.None)
            .Verifiable();
        mockCell
            .SetupSet(s => s.State = DataGridViewElementStates.Visible)
            .Verifiable();
        mockCell
            .SetupSet(s => s.Selected = true)
            .Verifiable();
        mockCell
            .Protected()
            .Setup("Dispose", ItExpr.IsAny<bool>());
        mockCell
            .Protected()
            .Setup("OnDataGridViewChanged");

        mockCell.Object.DataGridView = dataGridView;

        DataGridViewCellAccessibleObject accessibleObject = new(mockCell.Object);
        // verify that we check for a flag, not direct comparison. 128 is an arbitrary large flag.
        accessibleObject.Select((AccessibleSelection)128 | AccessibleSelection.AddSelection);
        Assert.True(dataGridView.IsHandleCreated);

        // NB: asserts are implicit - check that nothing was called on the mock that we didn't anticipate
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Select_RemoveSelection()
    {
        using DataGridView dataGridView = new();
        dataGridView.CreateControl();
        Assert.True(dataGridView.IsHandleCreated);

        Mock<DataGridViewCell> mockCell = new(MockBehavior.Strict);
        mockCell
            .SetupSet(s => s.State = DataGridViewElementStates.Visible)
            .Verifiable();
        mockCell
            .SetupSet(s => s.State = DataGridViewElementStates.None)
            .Verifiable();
        mockCell
            .SetupSet(s => s.Selected = It.IsAny<bool>())
            .Verifiable();
        mockCell
            .Protected()
            .Setup("Dispose", ItExpr.IsAny<bool>());
        mockCell
            .Protected()
            .Setup("OnDataGridViewChanged");

        mockCell.Object.DataGridView = dataGridView;

        DataGridViewCellAccessibleObject accessibleObject = new(mockCell.Object);

        // set selection, RemoveSelection is ignored
        accessibleObject.Select(AccessibleSelection.AddSelection | AccessibleSelection.RemoveSelection);
        // set selection, RemoveSelection is ignored
        accessibleObject.Select(AccessibleSelection.TakeSelection | AccessibleSelection.RemoveSelection);
        // now remove the selection
        accessibleObject.Select(AccessibleSelection.RemoveSelection);

        // NB: asserts are implicit - check that nothing was called on the mock that we didn't anticipate
        mockCell.VerifySet(s => s.Selected = true, Times.Exactly(2));
        mockCell.VerifySet(s => s.Selected = false, Times.Once());
    }

    [WinFormsFact]
    public void DataGridViewCellsAccessibleObject_IsReadOnly_property()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add(new DataGridViewRow());
        dataGridView.Rows.Add(new DataGridViewRow());

        dataGridView.Rows[0].Cells[0].ReadOnly = true;
        dataGridView.Rows[1].ReadOnly = true;
        dataGridView.Rows[2].ReadOnly = false;

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            foreach (DataGridViewCell cell in row.Cells)
            {
                bool value = cell.AccessibilityObject.IsReadOnly;

                Assert.Equal(cell.ReadOnly, value);
            }
        }

        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public unsafe void DataGridViewCellAccessibleObject_ControlType_IsDefined()
    {
        IRawElementProviderSimple.Interface provider = new DataGridViewCellAccessibleObject();
        using VARIANT actual = default;
        Assert.True(provider.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId, &actual).Succeeded);
        Assert.Equal((int)UIA_CONTROLTYPE_ID.UIA_DataItemControlTypeId, actual.ToObject());
    }

    [WinFormsFact]
    public void DataGridViewCellsAccessibleObject_GetPropertyValue_Name_ReturnsExpected()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add(new DataGridViewRow());

        DataGridViewCellAccessibleObject accessibleObject = new(dataGridView.Rows[0].Cells[0]);

        // DataGridViewCellAccessibleObject name couldn't be set, it's gathered dynamically in the Name property accessor
        Assert.Equal(accessibleObject.Name, ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId)).ToStringAndFree());
        Assert.Equal(accessibleObject.Name, ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleNamePropertyId)).ToStringAndFree());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewCellsAccessibleObject_GetPropertyValue_HelpText_ReturnsExpected()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add(new DataGridViewRow());

        DataGridViewCellAccessibleObject accessibleObject = new(dataGridView.Rows[0].Cells[0]);

        VARIANT actual = accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HelpTextPropertyId);

        Assert.Equal(accessibleObject.Help ?? string.Empty, ((BSTR)actual).ToStringAndFree());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewCellsAccessibleObject_GetPropertyValue_IsOffscreen_ReturnsFalse()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add(new DataGridViewRow());

        DataGridViewCellAccessibleObject accessibleObject = new(dataGridView.Rows[0].Cells[0]);

        bool actual = (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId);

        Assert.False(actual);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsExpandCollapsePatternAvailablePropertyId))]
    [InlineData(true, ((int)UIA_PROPERTY_ID.UIA_IsGridItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsGridPatternAvailablePropertyId))]
    [InlineData(true, ((int)UIA_PROPERTY_ID.UIA_IsLegacyIAccessiblePatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsMultipleViewPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsScrollItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsScrollPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsSelectionItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsSelectionPatternAvailablePropertyId))]
    [InlineData(true, ((int)UIA_PROPERTY_ID.UIA_IsTableItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTablePatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTextPattern2AvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTextPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTogglePatternAvailablePropertyId))]
    [InlineData(true, ((int)UIA_PROPERTY_ID.UIA_IsValuePatternAvailablePropertyId))]
    public void DataGridViewCellAccessibleObject_GetPropertyValue_Pattern_ReturnsExpected(bool expected, int propertyId)
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add(new DataGridViewRow());
        DataGridViewCellAccessibleObject accessibleObject = new(dataGridView.Rows[0].Cells[0]);
        var result = accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyId);

        Assert.Equal(expected, !result.IsEmpty && (bool)result);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Row_ReturnExpected_IfOwningRowNotExist()
    {
        AccessibleObject accessibleObject = new DataGridViewCellAccessibleObject(new SubDataGridViewCell());

        Assert.Equal(-1, accessibleObject.Row);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Row_ReturnExpected_IfDataGridViewNotExist()
    {
        SubDataGridViewCell cell = new()
        {
            OwningRow = new DataGridViewRow()
        };

        Assert.Equal(-1, cell.AccessibilityObject.Row);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Row_ReturnExpected()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].Cells[0].AccessibilityObject;

        Assert.Equal(0, accessibleObject1.Row);
        Assert.Equal(1, accessibleObject2.Row);
        Assert.Equal(2, accessibleObject3.Row);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Row_ReturnExpected_IfFirstRowHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[0].Visible = false;

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].Cells[0].AccessibilityObject;

        Assert.Equal(-1, accessibleObject1.Row);
        Assert.Equal(0, accessibleObject2.Row);
        Assert.Equal(1, accessibleObject3.Row);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Row_ReturnExpected_IfSecondRowHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[1].Visible = false;

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].Cells[0].AccessibilityObject;

        Assert.Equal(0, accessibleObject1.Row);
        Assert.Equal(-1, accessibleObject2.Row);
        Assert.Equal(1, accessibleObject3.Row);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Row_ReturnExpected_IfLastRowHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Rows.Add("1");
        dataGridView.Rows.Add("2");
        dataGridView.Rows.Add("3");
        dataGridView.Rows[2].Visible = false;

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[1].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[2].Cells[0].AccessibilityObject;

        Assert.Equal(0, accessibleObject1.Row);
        Assert.Equal(1, accessibleObject2.Row);
        Assert.Equal(-1, accessibleObject3.Row);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Column_ReturnExpected_IfOwningColumnNotExist()
    {
        AccessibleObject accessibleObject = new DataGridViewCellAccessibleObject(new SubDataGridViewCell());

        Assert.Equal(-1, accessibleObject.Column);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Column_ReturnExpected_IfDataGridViewNotExist()
    {
        SubDataGridViewCell cell = new()
        {
            OwningColumn = new DataGridViewTextBoxColumn()
        };

        Assert.Equal(-1, cell.AccessibilityObject.Column);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Column_ReturnExpected()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Rows.Add("1");

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(0, accessibleObject1.Column);
        Assert.Equal(1, accessibleObject2.Column);
        Assert.Equal(2, accessibleObject3.Column);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Column_ReturnExpected_IfFirstColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].Visible = false;
        dataGridView.Rows.Add("1");

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(-1, accessibleObject1.Column);
        Assert.Equal(0, accessibleObject2.Column);
        Assert.Equal(1, accessibleObject3.Column);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Column_ReturnExpected_IfSecondColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[1].Visible = false;
        dataGridView.Rows.Add("1");

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(0, accessibleObject1.Column);
        Assert.Equal(-1, accessibleObject2.Column);
        Assert.Equal(1, accessibleObject3.Column);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Column_ReturnExpected_IfLastColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[2].Visible = false;
        dataGridView.Rows.Add("1");

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(0, accessibleObject1.Column);
        Assert.Equal(1, accessibleObject2.Column);
        Assert.Equal(-1, accessibleObject3.Column);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Column_ReturnExpected_IfCustomOrder()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Rows.Add("1");

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(2, accessibleObject1.Column);
        Assert.Equal(1, accessibleObject2.Column);
        Assert.Equal(0, accessibleObject3.Column);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Column_ReturnExpected_IfCustomOrderAndFirstColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[0].Visible = false;
        dataGridView.Rows.Add("1");

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(-1, accessibleObject1.Column);
        Assert.Equal(1, accessibleObject2.Column);
        Assert.Equal(0, accessibleObject3.Column);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Column_ReturnExpected_IfCustomOrderAndSecondColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[1].Visible = false;
        dataGridView.Rows.Add("1");

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(1, accessibleObject1.Column);
        Assert.Equal(-1, accessibleObject2.Column);
        Assert.Equal(0, accessibleObject3.Column);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_Column_ReturnExpected_IfCustomOrderAndLastColumnHidden()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("Column 1", "Column 1");
        dataGridView.Columns.Add("Column 2", "Column 2");
        dataGridView.Columns.Add("Column 3", "Column 3");
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[2].Visible = false;
        dataGridView.Rows.Add("1");

        AccessibleObject accessibleObject1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(1, accessibleObject1.Column);
        Assert.Equal(0, accessibleObject2.Column);
        Assert.Equal(-1, accessibleObject3.Column);
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_GetPropertyValue_AutomationId_ReturnsExpected()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add(new DataGridViewRow());

        DataGridViewCellAccessibleObject dataGridViewCellAccessibleObject = new(dataGridView.Rows[0].Cells[0]);
        string expected = string.Concat(dataGridViewCellAccessibleObject.RuntimeId);

        Assert.Equal(expected, ((BSTR)dataGridViewCellAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_AutomationIdPropertyId)).ToStringAndFree());
        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewCellAccessibleObject_FragmentNavigate_Parent_ReturnsExpected(bool createControl)
    {
        using DataGridView dataGridView = CreateDataGridView(columnCount: 1, createControl);

        DataGridViewRow row = dataGridView.Rows[0];
        AccessibleObject accessibleObject = row.Cells[0].AccessibilityObject;

        if (createControl)
        {
            Assert.Equal(row.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        }
        else
        {
            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        }

        Assert.Equal(createControl, dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_FragmentNavigate_Parent_ReturnsNull_IfOwningRowIsNull()
    {
        using SubDataGridViewCell owner = new();
        DataGridViewCellAccessibleObject accessibleObject = new(owner);

        Assert.Null(accessibleObject.Owner.OwningRow);
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected()
    {
        using DataGridView dataGridView = CreateDataGridView(columnCount: 3);

        AccessibleObject rowHeader = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject cell1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject cell2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject cell3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(cell2, cell1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(cell3, cell2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(cell3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(cell2, cell3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(cell1, cell2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(rowHeader, cell1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_FragmentNavigate_Sibling_ReturnsNull_IfHandleIsNotCreated()
    {
        using DataGridView dataGridView = CreateDataGridView(columnCount: 2, createControl: false);

        DataGridViewRow row = dataGridView.Rows[0];
        AccessibleObject cell1 = row.Cells[0].AccessibilityObject;
        AccessibleObject cell2 = row.Cells[1].AccessibilityObject;

        Assert.Null(cell1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(cell2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Null(cell1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(cell2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.False(dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfRowHeadersHidden()
    {
        using DataGridView dataGridView = CreateDataGridView(columnCount: 3);
        dataGridView.RowHeadersVisible = false;

        AccessibleObject cell1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject cell2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject cell3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(cell2, cell1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(cell3, cell2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(cell3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(cell2, cell3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(cell1, cell2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(cell1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfFirstColumnHidden()
    {
        using DataGridView dataGridView = CreateDataGridView(columnCount: 3);
        dataGridView.Columns[0].Visible = false;

        AccessibleObject rowHeader = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject cell2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject cell3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(cell2, rowHeader.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(cell3, cell2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(cell3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(cell2, cell3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(rowHeader, cell2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(rowHeader.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfSecondColumnHidden()
    {
        using DataGridView dataGridView = CreateDataGridView(columnCount: 3);
        dataGridView.Columns[1].Visible = false;

        AccessibleObject rowHeader = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject cell1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject cell3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(cell1, rowHeader.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(cell3, cell1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(cell3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(cell1, cell3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(rowHeader, cell1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(rowHeader.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfLastColumnHidden()
    {
        using DataGridView dataGridView = CreateDataGridView(columnCount: 3);
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowHeader = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject cell1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject cell2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;

        Assert.Equal(cell1, rowHeader.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(cell2, cell1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(cell2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(cell1, cell2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(rowHeader, cell1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(rowHeader.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfRowHeadersAndFirstColumnHidden()
    {
        using DataGridView dataGridView = CreateDataGridView(columnCount: 4);
        dataGridView.RowHeadersVisible = false;
        dataGridView.Columns[0].Visible = false;

        AccessibleObject cell2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject cell3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;
        AccessibleObject cell4 = dataGridView.Rows[0].Cells[3].AccessibilityObject;

        Assert.Equal(cell3, cell2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(cell4, cell3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(cell4.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(cell3, cell4.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(cell2, cell3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(cell2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfRowHeadersAndSecondColumnHidden()
    {
        using DataGridView dataGridView = CreateDataGridView(columnCount: 4);
        dataGridView.RowHeadersVisible = false;
        dataGridView.Columns[1].Visible = false;

        AccessibleObject cell1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject cell3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;
        AccessibleObject cell4 = dataGridView.Rows[0].Cells[3].AccessibilityObject;

        Assert.Equal(cell3, cell1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(cell4, cell3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(cell4.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(cell3, cell4.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(cell1, cell3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(cell1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfRowHeadersAndLastColumnHidden()
    {
        using DataGridView dataGridView = CreateDataGridView(columnCount: 4);
        dataGridView.RowHeadersVisible = false;
        dataGridView.Columns[3].Visible = false;

        AccessibleObject cell1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject cell2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject cell3 = dataGridView.Rows[0].Cells[2].AccessibilityObject;

        Assert.Equal(cell2, cell1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(cell3, cell2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(cell3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(cell2, cell3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(cell1, cell2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(cell1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfCustomOrder()
    {
        using DataGridView dataGridView = CreateDataGridView(columnCount: 3);
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;

        AccessibleObject rowHeader = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject cell1 = dataGridView.Rows[0].Cells[2].AccessibilityObject;
        AccessibleObject cell2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject cell3 = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(cell1, rowHeader.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(cell2, cell1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(cell3, cell2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(cell3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(cell2, cell3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(cell1, cell2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(rowHeader, cell1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(rowHeader.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfCustomOrderAndFirstColumnHidden()
    {
        using DataGridView dataGridView = CreateDataGridView(columnCount: 3);
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[0].Visible = false;

        AccessibleObject rowHeader = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject cell1 = dataGridView.Rows[0].Cells[2].AccessibilityObject;
        AccessibleObject cell2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;

        Assert.Equal(cell1, rowHeader.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(cell2, cell1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(cell2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(cell1, cell2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(rowHeader, cell1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(rowHeader.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfCustomOrderAndSecondColumnHidden()
    {
        using DataGridView dataGridView = CreateDataGridView(columnCount: 3);
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[1].Visible = false;

        AccessibleObject rowHeader = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject cell1 = dataGridView.Rows[0].Cells[2].AccessibilityObject;
        AccessibleObject cell3 = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(cell1, rowHeader.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(cell3, cell1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(cell3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(cell1, cell3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(rowHeader, cell1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(rowHeader.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfCustomOrderAndLastColumnHidden()
    {
        using DataGridView dataGridView = CreateDataGridView(columnCount: 3);
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject rowHeader = dataGridView.Rows[0].HeaderCell.AccessibilityObject;
        AccessibleObject cell2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject cell3 = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(cell2, rowHeader.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(cell3, cell2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(cell3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(cell2, cell3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(rowHeader, cell2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(rowHeader.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfCustomOrderAndRowHeadersHidden()
    {
        using DataGridView dataGridView = CreateDataGridView(columnCount: 3);
        dataGridView.RowHeadersVisible = false;
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;

        AccessibleObject cell1 = dataGridView.Rows[0].Cells[2].AccessibilityObject;
        AccessibleObject cell2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject cell3 = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(cell2, cell1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(cell3, cell2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(cell3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(cell2, cell3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(cell1, cell2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(cell1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfCustomOrderAndFirstColumnAndRowHeadersHidden()
    {
        using DataGridView dataGridView = CreateDataGridView(columnCount: 3);
        dataGridView.RowHeadersVisible = false;
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[0].Visible = false;

        AccessibleObject cell1 = dataGridView.Rows[0].Cells[2].AccessibilityObject;
        AccessibleObject cell2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;

        Assert.Equal(cell2, cell1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(cell2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(cell1, cell2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(cell1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfCustomOrderAndSecondColumnAndRowHeadersHidden()
    {
        using DataGridView dataGridView = CreateDataGridView(columnCount: 3);
        dataGridView.RowHeadersVisible = false;
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[1].Visible = false;

        AccessibleObject cell1 = dataGridView.Rows[0].Cells[2].AccessibilityObject;
        AccessibleObject cell3 = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(cell3, cell1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(cell3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(cell1, cell3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(cell1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfCustomOrderAndLastColumnAndRowHeadersHidden()
    {
        using DataGridView dataGridView = CreateDataGridView(columnCount: 3);
        dataGridView.RowHeadersVisible = false;
        dataGridView.Columns[0].DisplayIndex = 2;
        dataGridView.Columns[1].DisplayIndex = 1;
        dataGridView.Columns[2].DisplayIndex = 0;
        dataGridView.Columns[2].Visible = false;

        AccessibleObject cell2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;
        AccessibleObject cell3 = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Equal(cell3, cell2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(cell3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(cell2, cell3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(cell2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DataGridViewCellAccessibleObject_FragmentNavigate_Child_ReturnsNull(bool createControl)
    {
        using DataGridView dataGridView = CreateDataGridView(columnCount: 2, createControl);

        AccessibleObject cell1 = dataGridView.Rows[0].Cells[0].AccessibilityObject;
        AccessibleObject cell2 = dataGridView.Rows[0].Cells[1].AccessibilityObject;

        Assert.Null(cell1.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(cell1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Null(cell2.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(cell2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Equal(createControl, dataGridView.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewCellAccessibleObject_FragmentNavigate_Child_ReturnsNull_IfRowHeadersHidden()
    {
        using DataGridView dataGridView = CreateDataGridView(columnCount: 1);
        dataGridView.RowHeadersVisible = false;

        AccessibleObject cell = dataGridView.Rows[0].Cells[0].AccessibilityObject;

        Assert.Null(cell.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(cell.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
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

    // Unit test for https://github.com/dotnet/winforms/issues/7154
    [WinFormsTheory]
    [InlineData([false, 1])]
    [InlineData([true, 0])]
    public void DataGridView_SwitchConfigured_AdjustsCellRowStartIndices(bool switchValue, int expectedIndex)
    {
        using DataGridViewUIAStartRowCountAtZeroScope scope = new(switchValue);
        using DataGridView dataGridView = new DataGridView();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add(new DataGridViewRow());
        Assert.Equal($"{string.Format(SR.DataGridView_AccRowName, expectedIndex)}, Not sorted.", dataGridView.Rows[0].Cells[0].AccessibilityObject.Name);
    }

    private class SubDataGridViewCell : DataGridViewCell
    {
    }

    private class SubDataGridView : DataGridView
    {
        public bool FocusCalled { get; private set; }

        public new DataGridViewCell CurrentCell { get; set; }

        private protected override bool FocusInternal()
        {
            FocusCalled = true;
            return false;
        }
    }
}
