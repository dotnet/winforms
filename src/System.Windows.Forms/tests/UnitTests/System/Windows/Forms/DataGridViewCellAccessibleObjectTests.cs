// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Moq;
using Moq.Protected;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class DataGridViewCellAccessibleObjectTests : DataGridViewCell, IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void DataGridViewCellAccessibleObject_Ctor_Default()
        {
            var accessibleObject = new DataGridViewCellAccessibleObject();
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
            using var dataGridView = new DataGridView
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

            AccessibleObject rr = dataGridView.AccessibilityObject; //it is necessary to be in time to initialize elements

            var accCellWidthSum = 0;
            for (int i = 0; i < 4; i++)
            {
                accCellWidthSum += dataGridView.Rows[0].Cells[i].AccessibilityObject.BoundingRectangle.Width;
            }

            var accRowWidth = dataGridView.Rows[0].AccessibilityObject.BoundingRectangle.Width;

            Assert.Equal(accCellWidthSum, accRowWidth - dataGridView.RowHeadersWidth);
        }

        [WinFormsTheory]
        [InlineData(RightToLeft.No)]
        [InlineData(RightToLeft.Yes)]
        public void DataGridViewCellsAccessibleObject_Ctor_Default_IfHandleIsNotCreated(RightToLeft rightToLeft)
        {
            using var dataGridView = new DataGridView
            {
                RightToLeft = rightToLeft,
                ColumnCount = 4,
                Width = 85
            };

            dataGridView.Columns[0].Width = 40;
            dataGridView.Columns[1].Width = 40;
            dataGridView.Columns[2].Width = 40;
            dataGridView.Columns[3].Width = 40;

            AccessibleObject rr = dataGridView.AccessibilityObject; //it is necessary to be in time to initialize elements

            var accCellWidthSum = 0;
            for (int i = 0; i < 4; i++)
            {
                accCellWidthSum += dataGridView.Rows[0].Cells[i].AccessibilityObject.BoundingRectangle.Width;
            }

            var accRowWidth = dataGridView.Rows[0].AccessibilityObject.BoundingRectangle.Width;

            Assert.Equal(0, accCellWidthSum);
            Assert.Equal(0, accRowWidth);
        }

        [Theory]
        [MemberData(nameof(Ctor_DataGridViewCell_TestData))]
        public void DataGridViewCellAccessibleObject_Ctor_DataGridViewCell(DataGridViewCell owner)
        {
            var accessibleObject = new DataGridViewCellAccessibleObject(owner);
            Assert.Equal(owner, accessibleObject.Owner);
            Assert.Equal(AccessibleRole.Cell, accessibleObject.Role);
        }

        public static IEnumerable<object[]> Bounds_TestData()
        {
            var row = new DataGridViewRow();
            var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            yield return new object[] { new DataGridViewCellAccessibleObject(cell), Rectangle.Empty };
        }

        [Theory]
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

        [Theory]
        [MemberData(nameof(NoOwner_TestData))]
        public void DataGridViewCellAccessibleObject_Bounds_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
        {
            Assert.Throws<InvalidOperationException>(() => accessibleObject.Bounds);
        }

        public static IEnumerable<object[]> DefaultAction_TestData()
        {
            yield return new object[] { new DataGridViewCellAccessibleObject(new SubDataGridViewCell()), "Edit" };

            var row = new DataGridViewRow();
            var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            cell.ReadOnly = true;
            yield return new object[] { new DataGridViewCellAccessibleObject(cell), string.Empty };
        }

        [Theory]
        [MemberData(nameof(DefaultAction_TestData))]
        public void DataGridViewCellAccessibleObject_DefaultAction_Get_ReturnsExpected(AccessibleObject accessibleObject, string expected)
        {
            Assert.Equal(expected, accessibleObject.DefaultAction);
        }

        [Theory]
        [MemberData(nameof(NoOwner_TestData))]
        public void DataGridViewCellAccessibleObject_DefaultAction_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
        {
            Assert.Throws<InvalidOperationException>(() => accessibleObject.DefaultAction);
        }

        [Fact]
        public void DataGridViewCellAccessibleObject_Help_Get_ReturnsNull()
        {
            var accessibleObject = new DataGridViewCellAccessibleObject();
            Assert.Null(accessibleObject.Help);
        }

        public static IEnumerable<object[]> Name_TestData()
        {
            yield return new object[] { new DataGridViewCellAccessibleObject(new SubDataGridViewCell()), string.Empty };

            var row = new DataGridViewRow();
            var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            yield return new object[] { new DataGridViewCellAccessibleObject(cell), string.Empty };
        }

        [Theory]
        [MemberData(nameof(Name_TestData))]
        public void DataGridViewCellAccessibleObject_Name_Get_ReturnsExpected(AccessibleObject accessibleObject, string expected)
        {
            Assert.Equal(expected, accessibleObject.Name);
        }

        [Theory]
        [MemberData(nameof(NoOwner_TestData))]
        public void DataGridViewCellAccessibleObject_Name_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
        {
            Assert.Throws<InvalidOperationException>(() => accessibleObject.Name);
        }

        [Fact]
        public void DataGridViewCellAccessibleObject_Owner_Set_GetReturnsExpected()
        {
            using var owner = new SubDataGridViewCell();
            var accessibleObject = new DataGridViewCellAccessibleObject
            {
                Owner = owner
            };
            Assert.Same(owner, accessibleObject.Owner);
        }

        [Fact]
        public void DataGridViewCellAccessibleObject_Owner_SetAlreadyWithOwner_ThrowsInvalidOperationException()
        {
            using var owner = new SubDataGridViewCell();
            var accessibleObject = new DataGridViewCellAccessibleObject(owner);
            Assert.Throws<InvalidOperationException>(() => accessibleObject.Owner = owner);
        }

        public static IEnumerable<object[]> Parent_TestData()
        {
            yield return new object[] { new DataGridViewCellAccessibleObject(new SubDataGridViewCell()), null };

            var row = new DataGridViewRow();
            var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            yield return new object[] { new DataGridViewCellAccessibleObject(cell), row.AccessibilityObject };
        }

        [Theory]
        [MemberData(nameof(Parent_TestData))]
        public void DataGridViewCellAccessibleObject_Parent_Get_ReturnsExpected(AccessibleObject accessibleObject, AccessibleObject expected)
        {
            Assert.Equal(expected, accessibleObject.Parent);
        }

        [Theory]
        [MemberData(nameof(NoOwner_TestData))]
        public void DataGridViewCellAccessibleObject_Parent_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
        {
            Assert.Throws<InvalidOperationException>(() => accessibleObject.Parent);
        }

        [Theory]
        [MemberData(nameof(NoOwner_TestData))]
        public void DataGridViewCellAccessibleObject_State_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
        {
            Assert.Throws<InvalidOperationException>(() => accessibleObject.State);
        }

        [Fact]
        public void DataGridViewCellAccessibleObject_State_NoDataGridView_ReturnsExpected()
        {
            using var owner = new SubDataGridViewCell();
            var accessibleObject = new DataGridViewCellAccessibleObject(owner);
            Assert.Equal(AccessibleStates.Focusable | AccessibleStates.Selectable, accessibleObject.State);
        }

        public static IEnumerable<object[]> Value_TestData()
        {
            yield return new object[] { new DataGridViewCellAccessibleObject(new SubDataGridViewCell()), "(null)" };
        }

        [Theory]
        [MemberData(nameof(Value_TestData))]
        public void DataGridViewCellAccessibleObject_Value_Get_ReturnsExpected(AccessibleObject accessibleObject, string expected)
        {
            Assert.Equal(expected, accessibleObject.Value);
        }

        [Theory]
        [MemberData(nameof(NoOwner_TestData))]
        public void DataGridViewCellAccessibleObject_Value_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
        {
            Assert.Throws<InvalidOperationException>(() => accessibleObject.Value);
        }

        [Theory]
        [MemberData(nameof(NoOwner_TestData))]
        public void DataGridViewCellAccessibleObject_DoDefaultAction_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
        {
            Assert.Throws<InvalidOperationException>(() => accessibleObject.DoDefaultAction());
        }

        [Theory]
        [MemberData(nameof(NoOwner_TestData))]
        public void DataGridViewCellAccessibleObject_GetChild_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
        {
            Assert.Throws<InvalidOperationException>(() => accessibleObject.GetChild(-1));
        }

        [Fact]
        public void DataGridViewCellAccessibleObject_GetChild_NoDataGridView_ReturnsNull()
        {
            using var owner = new SubDataGridViewCell();
            var accessibleObject = new DataGridViewCellAccessibleObject(owner);
            Assert.Null(accessibleObject.GetChild(-1));
        }

        [Theory]
        [MemberData(nameof(NoOwner_TestData))]
        public void DataGridViewCellAccessibleObject_GetChildCount_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
        {
            Assert.Throws<InvalidOperationException>(() => accessibleObject.GetChildCount());
        }

        [Fact]
        public void DataGridViewCellAccessibleObject_GetChildCount_NoDataGridView_ReturnsZero()
        {
            using var owner = new SubDataGridViewCell();
            var accessibleObject = new DataGridViewCellAccessibleObject(owner);
            Assert.Equal(0, accessibleObject.GetChildCount());
        }

        [Fact]
        public void DataGridViewCellAccessibleObject_GetFocused_Invoke_ReturnsNull()
        {
            var accessibleObject = new DataGridViewCellAccessibleObject();
            Assert.Null(accessibleObject.GetFocused());
        }

        [Fact]
        public void DataGridViewCellAccessibleObject_GetSelected_Invoke_ReturnsNull()
        {
            var accessibleObject = new DataGridViewCellAccessibleObject();
            Assert.Null(accessibleObject.GetSelected());
        }

        public static IEnumerable<object[]> Navigate_TestData()
        {
            yield return new object[] { new DataGridViewCellAccessibleObject(new SubDataGridViewCell()), AccessibleNavigation.Right, null };

            var row = new DataGridViewRow();
            var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            yield return new object[] { new DataGridViewCellAccessibleObject(cell), AccessibleNavigation.Right, null };
        }

        [Theory]
        [MemberData(nameof(Navigate_TestData))]
        public void DataGridViewCellAccessibleObject_Navigate_Invoke_ReturnsExpected(AccessibleObject accessibleObject, AccessibleNavigation navigationDirection, AccessibleObject expected)
        {
            Assert.Equal(expected, accessibleObject.Navigate(navigationDirection));
        }

        [Theory]
        [MemberData(nameof(NoOwner_TestData))]
        public void DataGridViewCellAccessibleObject_Navigate_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
        {
            Assert.Throws<InvalidOperationException>(() => accessibleObject.Navigate(AccessibleNavigation.Right));
        }

        [Theory]
        [InlineData(AccessibleSelection.None)]
        [InlineData(AccessibleSelection.RemoveSelection)]
        public void DataGridViewCellAccessibleObject_Select_NothingToDo_Nop(AccessibleSelection flags)
        {
            using var owner = new SubDataGridViewCell();
            var accessibleObject = new DataGridViewCellAccessibleObject(owner);
            accessibleObject.Select(flags);
        }

        [Theory]
        [MemberData(nameof(NoOwner_TestData))]
        public void DataGridViewCellAccessibleObject_Select_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
        {
            Assert.Throws<InvalidOperationException>(() => accessibleObject.Select(AccessibleSelection.None));
        }

        [WinFormsFact]
        public void DataGridViewCellAccessibleObject_Select_HasSelectionFlagsWithoutValidDataGridView_DoesNothing()
        {
            var mockCell = new Mock<DataGridViewCell>(MockBehavior.Strict);
            mockCell
                .SetupSet(s => s.State = DataGridViewElementStates.Visible)
                .Verifiable();
            mockCell
                .Protected()
                .Setup("Dispose", ItExpr.IsAny<bool>());

            var mockObj = mockCell.Object;
            var accessibleObject = new DataGridViewCellAccessibleObject(mockObj);
            accessibleObject.Select(AccessibleSelection.None);
            // NB: asserts are implicit - check that nothing was called on the mock that we didn't anticipate

            using DataGridView dataGridView = new DataGridView();
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
            using var dataGridView = new SubDataGridView();
            dataGridView.CreateControl();
            Assert.True(dataGridView.IsHandleCreated);

            var mockCell = new Mock<DataGridViewCell>(MockBehavior.Strict);
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

            var accessibleObject = new DataGridViewCellAccessibleObject(mockObj);
            // verify that we check for a flag, not direct comparison. 128 is an arbitrary large flag.
            accessibleObject.Select((AccessibleSelection)128 | AccessibleSelection.TakeFocus);

            // NB: some asserts are implicit - check that nothing was called on the mock that we didn't anticipate
            Assert.True(dataGridView.FocusCalled);
        }

        [WinFormsFact]
        public void DataGridViewCellAccessibleObject_Select_TakeSelection()
        {
            using var dataGridView = new DataGridView();
            dataGridView.CreateControl();
            Assert.True(dataGridView.IsHandleCreated);

            var mockCell = new Mock<DataGridViewCell>(MockBehavior.Strict);
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

            var accessibleObject = new DataGridViewCellAccessibleObject(mockObj);
            // verify that we check for a flag, not direct comparison. 128 is an arbitrary large flag.
            accessibleObject.Select((AccessibleSelection)128 | AccessibleSelection.TakeSelection);

            // NB: some asserts are implicit - check that nothing was called on the mock that we didn't anticipate

            // Can't test whether CurrentCell was set unless we add the whole layer of Rows, Columns, etc.
            // Assert.Equal(mockObj, dataGridView.CurrentCell);
        }

        [WinFormsFact]
        public void DataGridViewCellAccessibleObject_Select_AddSelection()
        {
            using var dataGridView = new DataGridView();
            dataGridView.CreateControl();
            Assert.True(dataGridView.IsHandleCreated);

            var mockCell = new Mock<DataGridViewCell>(MockBehavior.Strict);
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

            var accessibleObject = new DataGridViewCellAccessibleObject(mockCell.Object);
            // verify that we check for a flag, not direct comparison. 128 is an arbitrary large flag.
            accessibleObject.Select((AccessibleSelection)128 | AccessibleSelection.AddSelection);

            // NB: asserts are implicit - check that nothing was called on the mock that we didn't anticipate
        }

        [WinFormsFact]
        public void DataGridViewCellAccessibleObject_Select_RemoveSelection()
        {
            using var dataGridView = new DataGridView();
            dataGridView.CreateControl();
            Assert.True(dataGridView.IsHandleCreated);

            var mockCell = new Mock<DataGridViewCell>(MockBehavior.Strict);
            mockCell
                .SetupSet(s => s.State = DataGridViewElementStates.Visible)
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

            var accessibleObject = new DataGridViewCellAccessibleObject(mockCell.Object);

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
            using var dataGridView = new DataGridView();
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
        }

        [Fact]
        public void DataGridViewCellAccessibleObject_ControlType_IsDefined()
        {
            UiaCore.IRawElementProviderSimple provider = new DataGridViewCellAccessibleObject();

            Assert.Equal(UiaCore.UIA.DataItemControlTypeId, provider.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
        }

        [WinFormsFact]
        public void DataGridViewCellsAccessibleObject_GetPropertyValue_Name_ReturnsExpected()
        {
            using DataGridView dataGridView = new();
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView.Rows.Add(new DataGridViewRow());

            DataGridViewCellAccessibleObject accessibleObject = new(dataGridView.Rows[0].Cells[0]);

            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.NamePropertyId);

            //DataGridViewCellAccessibleObject name couldn't be set, it's gathered dynamically in the Name property accessor
            Assert.Equal(accessibleObject.Name, actual);
            Assert.False(dataGridView.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewCellsAccessibleObject_GetPropertyValue_HelpText_ReturnsExpected()
        {
            using DataGridView dataGridView = new();
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView.Rows.Add(new DataGridViewRow());

            DataGridViewCellAccessibleObject accessibleObject = new(dataGridView.Rows[0].Cells[0]);

            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.HelpTextPropertyId);

            Assert.Equal(accessibleObject.Help ?? string.Empty, actual);
            Assert.False(dataGridView.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewCellsAccessibleObject_GetPropertyValue_IsOffscreen_ReturnsFalse()
        {
            using DataGridView dataGridView = new();
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView.Rows.Add(new DataGridViewRow());

            DataGridViewCellAccessibleObject accessibleObject = new(dataGridView.Rows[0].Cells[0]);

            bool actual = (bool)accessibleObject.GetPropertyValue(UiaCore.UIA.IsOffscreenPropertyId);

            Assert.False(actual);
            Assert.False(dataGridView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(false, ((int)UiaCore.UIA.IsExpandCollapsePatternAvailablePropertyId))]
        [InlineData(true, ((int)UiaCore.UIA.IsGridItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsGridPatternAvailablePropertyId))]
        [InlineData(true, ((int)UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsMultipleViewPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsScrollItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsScrollPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsSelectionItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsSelectionPatternAvailablePropertyId))]
        [InlineData(true, ((int)UiaCore.UIA.IsTableItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTablePatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTextPattern2AvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTextPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTogglePatternAvailablePropertyId))]
        [InlineData(true, ((int)UiaCore.UIA.IsValuePatternAvailablePropertyId))]
        public void DataGridViewCellAccessibleObject_GetPropertyValue_Pattern_ReturnsExpected(bool expected, int propertyId)
        {
            using DataGridView dataGridView = new();
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewCellAccessibleObject accessibleObject = new(dataGridView.Rows[0].Cells[0]);

            Assert.Equal(expected, accessibleObject.GetPropertyValue((UiaCore.UIA)propertyId) ?? false);
            Assert.False(dataGridView.IsHandleCreated);
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
}
