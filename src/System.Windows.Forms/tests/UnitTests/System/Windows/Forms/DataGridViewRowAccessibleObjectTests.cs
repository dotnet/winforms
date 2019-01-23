﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewRowAccessibleObjectTests : DataGridViewRow
    {
        [Fact]
        public void DataGridViewRowAccessibleObject_Ctor_Default()
        {
            var accessibleObject = new DataGridViewRowAccessibleObject();
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
            var accessibleObject = new DataGridViewRowAccessibleObject(owner);
            Assert.Equal(owner, accessibleObject.Owner);
            Assert.Equal(AccessibleRole.Row, accessibleObject.Role);
            Assert.Null(accessibleObject.DefaultAction);
            Assert.Null(accessibleObject.Help);
        }

        public static IEnumerable<object[]> NoOwner_TestData()
        {
            yield return new object[] { new DataGridViewRowAccessibleObject() };
            yield return new object[] { new DataGridViewRowAccessibleObject(null) };
        }

        [Theory]
        [MemberData(nameof(NoOwner_TestData))]
        public void DataGridViewRowAccessibleObject_Bounds_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
        {
            Assert.Throws<InvalidOperationException>(() => accessibleObject.Bounds);
        }

        [Fact]
        public void DataGridViewRowAccessibleObject_Bounds_NoDataGridView_ThrowsNullReferenceException()
        {
            var row = new DataGridViewRow();
            var accessibleObject = new DataGridViewRowAccessibleObject(row);
            Assert.Throws<NullReferenceException>(() => accessibleObject.Bounds);
        }

        public static IEnumerable<object[]> Name_TestData()
        {
            yield return new object[] { new DataGridViewRowAccessibleObject(new DataGridViewRow()), "Row -1" };
        }

        [Theory]
        [MemberData(nameof(Name_TestData))]
        public void DataGridViewRowAccessibleObject_Name_Get_ReturnsExpected(AccessibleObject accessibleObject, string expected)
        {
            Assert.Equal(expected, accessibleObject.Name);
        }

        [Theory]
        [MemberData(nameof(NoOwner_TestData))]
        public void DataGridViewRowAccessibleObject_Name_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
        {
            Assert.Throws<InvalidOperationException>(() => accessibleObject.Name);
        }

        [Fact]
        public void DataGridViewRowAccessibleObject_Owner_Set_GetReturnsExpected()
        {
            var owner = new DataGridViewRow();
            var accessibleObject = new DataGridViewRowAccessibleObject
            {
                Owner = owner
            };
            Assert.Same(owner, accessibleObject.Owner);
        }

        [Fact]
        public void DataGridViewRowAccessibleObject_Owner_SetAlreadyWithOwner_ThrowsInvalidOperationException()
        {
            var owner = new DataGridViewRow();
            var accessibleObject = new DataGridViewRowAccessibleObject(owner);
            Assert.Throws<InvalidOperationException>(() => accessibleObject.Owner = owner);
        }

        [Theory]
        [MemberData(nameof(NoOwner_TestData))]
        public void DataGridViewRowAccessibleObject_Parent_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
        {
            Assert.Throws<InvalidOperationException>(() => accessibleObject.Parent);
        }

        [Fact]
        public void DataGridViewRowAccessibleObject_Parent_NoDataGridView_ThrowsNullReferenceException()
        {
            var owner = new DataGridViewRow();
            var accessibleObject = new DataGridViewRowAccessibleObject(owner);
            Assert.Throws<NullReferenceException>(() => accessibleObject.Parent);
        }

        [Theory]
        [MemberData(nameof(NoOwner_TestData))]
        public void DataGridViewRowAccessibleObject_State_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
        {
            Assert.Throws<InvalidOperationException>(() => accessibleObject.State);
        }

        [Fact]
        public void DataGridViewRowAccessibleObject_State_NoDataGridView_ThrowsNullReferenceException()
        {
            var owner = new DataGridViewRow();
            var accessibleObject = new DataGridViewRowAccessibleObject(owner);
            Assert.Throws<NullReferenceException>(() => accessibleObject.State);
        }

        [Theory]
        [MemberData(nameof(NoOwner_TestData))]
        public void DataGridViewRowAccessibleObject_Value_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
        {
            Assert.Throws<InvalidOperationException>(() => accessibleObject.Value);
        }

        [Fact]
        public void DataGridViewRowAccessibleObject_Value_NoDataGridView_ThrowsNullReferenceException()
        {
            var owner = new DataGridViewRow();
            var accessibleObject = new DataGridViewRowAccessibleObject(owner);
            Assert.Throws<NullReferenceException>(() => accessibleObject.Value);
        }

        [Fact]
        public void DataGridViewRowAccessibleObject_DoDefaultAction_Invoke_Nop()
        {
            var accessibleObject = new DataGridViewRowAccessibleObject();
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
        public void DataGridViewRowAccessibleObject_GetChild_NoDataGridView_ThrowsNullReferenceException(int index)
        {
            var owner = new DataGridViewRow();
            var accessibleObject = new DataGridViewRowAccessibleObject(owner);
            Assert.Throws<NullReferenceException>(() => accessibleObject.GetChild(index));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        public void DataGridViewRowAccessibleObject_GetChild_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            var accessibleObject = new DataGridViewRowAccessibleObject();
            Assert.Throws<ArgumentOutOfRangeException>("index", () => accessibleObject.GetChild(index));
        }

        [Theory]
        [MemberData(nameof(NoOwner_TestData))]
        public void DataGridViewRowAccessibleObject_GetChildCount_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
        {
            Assert.Throws<InvalidOperationException>(() => accessibleObject.GetChildCount());
        }

        [Fact]
        public void DataGridViewRowAccessibleObject_GetChildCount_NoDataGridView_ThrowsNullReferenceException()
        {
            var owner = new DataGridViewRow();
            var accessibleObject = new DataGridViewRowAccessibleObject(owner);
            Assert.Throws<NullReferenceException>(() => accessibleObject.GetChildCount());
        }

        [Theory]
        [MemberData(nameof(NoOwner_TestData))]
        public void DataGridViewRowAccessibleObject_GetFocused_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
        {
            Assert.Throws<InvalidOperationException>(() => accessibleObject.GetFocused());
        }

        [Fact]
        public void DataGridViewRowAccessibleObject_GetFocused_NoDataGridView_ThrowsNullReferenceException()
        {
            var owner = new DataGridViewRow();
            var accessibleObject = new DataGridViewRowAccessibleObject(owner);
            Assert.Throws<NullReferenceException>(() => accessibleObject.GetFocused());
        }

        [Fact]
        public void DataGridViewRowAccessibleObject_GetSelected_Invoke_ReturnsSameInstance()
        {
            var owner = new DataGridViewRow();
            var accessibleObject = new DataGridViewRowAccessibleObject(owner);
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
            Assert.Throws<InvalidOperationException>(() => accessibleObject.GetSelected());
        }

        public static IEnumerable<object[]> Navigate_TestData()
        {
            yield return new object[] { new DataGridViewRowAccessibleObject(new DataGridViewRow()), (AccessibleNavigation)(AccessibleNavigation.Up - 1), null };
            yield return new object[] { new DataGridViewRowAccessibleObject(new DataGridViewRow()), (AccessibleNavigation)(AccessibleNavigation.LastChild + 1), null };
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
            var owner = new DataGridViewRow();
            var accessibleObject = new DataGridViewRowAccessibleObject(owner);
            Assert.Throws<NullReferenceException>(() => accessibleObject.Navigate(navigationDirection));
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
            var owner = new DataGridViewRow();
            var accessibleObject = new DataGridViewRowAccessibleObject(owner);
            accessibleObject.Select(flags);
        }

        [Theory]
        [MemberData(nameof(NoOwner_TestData))]
        public void DataGridViewRowAccessibleObject_Select_NoOwner_ThrowsInvalidOperationException(AccessibleObject accessibleObject)
        {
            Assert.Throws<InvalidOperationException>(() => accessibleObject.Select(AccessibleSelection.None));
        }

        private class SubDataGridViewCell : DataGridViewCell
        {
        }
    }
}
