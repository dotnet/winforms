// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewCellAccessibleObjectTests : DataGridViewCell
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
            var owner = new SubDataGridViewCell();
            var accessibleObject = new DataGridViewCellAccessibleObject
            {
                Owner = owner
            };
            Assert.Same(owner, accessibleObject.Owner);
        }

        [Fact]
        public void DataGridViewCellAccessibleObject_Owner_SetAlreadyWithOwner_ThrowsInvalidOperationException()
        {
            var owner = new SubDataGridViewCell();
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
            var owner = new SubDataGridViewCell();
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

        [Fact]
        public void DataGridViewCellAccessibleObject_DoDefaultAction_NoDataGridView_ThrowsInvalidOperationException()
        {
            var owner = new SubDataGridViewCell();
            var accessibleObject = new DataGridViewCellAccessibleObject(owner);
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
            var owner = new SubDataGridViewCell();
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
            var owner = new SubDataGridViewCell();
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
            var owner = new SubDataGridViewCell();
            var accessibleObject = new DataGridViewCellAccessibleObject(owner);
            accessibleObject.Select(flags);
        }

        [Theory]
        [MemberData(nameof(NoOwner_TestData))]
        public void DataGridViewCellAccessibleObject_Select_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
        {
            Assert.Throws<InvalidOperationException>(() => accessibleObject.Select(AccessibleSelection.None));
        }

        [Theory]
        [InlineData(AccessibleSelection.TakeSelection)]
        [InlineData(AccessibleSelection.AddSelection)]
        [InlineData(AccessibleSelection.AddSelection | AccessibleSelection.RemoveSelection)]
        [InlineData(AccessibleSelection.TakeSelection | AccessibleSelection.RemoveSelection)]
        public void DataGridViewCellAccessibleObject_Select_HasSelectionFlagsWithoutDataGridView_ThrowsInvalidOperationException(AccessibleSelection flags)
        {
            var owner = new SubDataGridViewCell();
            var accessibleObject = new DataGridViewCellAccessibleObject(owner);
            Assert.Throws<InvalidOperationException>(() => accessibleObject.Select(flags));
        }

        [Theory]
        [InlineData(AccessibleSelection.TakeFocus)]
        public void DataGridViewCellAccessibleObject_Select_HasFocusFlagsWithoutDataGridView_ThrowsInvalidOperationException(AccessibleSelection flags)
        {
            var owner = new SubDataGridViewCell();
            var accessibleObject = new DataGridViewCellAccessibleObject(owner);
            accessibleObject.Select(flags);
        }

        private class SubDataGridViewCell : DataGridViewCell
        {
        }
    }
}
