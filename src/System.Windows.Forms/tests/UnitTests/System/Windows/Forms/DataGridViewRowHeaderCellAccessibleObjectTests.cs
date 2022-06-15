// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewRowHeaderCellAccessibleObjectTests : DataGridViewRowHeaderCell
    {
        [WinFormsFact]
        public void DataGridViewRowHeaderCellAccessibleObject_Ctor_Default()
        {
            DataGridViewRowHeaderCellAccessibleObject accessibleObject = new(null);
            Assert.Null(accessibleObject.Owner);
        }

        [WinFormsFact]
        public void DataGridViewRowHeaderCellAccessibleObject_Parent_ThrowException_IfOwnerIsNull()
        {
            Assert.Throws<InvalidOperationException>(() => new DataGridViewRowHeaderCellAccessibleObject(null).Parent);
        }

        [WinFormsFact]
        public void DataGridViewRowHeaderCellAccessibleObject_Parent_ReturnsExpected()
        {
            using DataGridView control = new();
            control.Columns.Add("Column 1", "Header text 1");
            DataGridViewRowHeaderCell cell = control.Rows[0].HeaderCell;

            var accessibleObject = (DataGridViewRowHeaderCellAccessibleObject)cell.AccessibilityObject;

            Assert.Equal(cell.OwningRow.AccessibilityObject, accessibleObject.Parent);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewRowHeaderCellAccessibleObject_Value_ReturnsExpected()
        {
            DataGridViewRowHeaderCellAccessibleObject accessibleObject = new(null);
            Assert.Equal(string.Empty, accessibleObject.Value);
        }

        [WinFormsFact]
        public void DataGridViewRowHeaderCellAccessibleObject_Role_ReturnsExpected()
        {
            DataGridViewRowHeaderCellAccessibleObject accessibleObject = new(null);
            Assert.Equal(AccessibleRole.RowHeader, accessibleObject.Role);
        }

        [WinFormsFact]
        public void DataGridViewRowHeaderCellAccessibleObject_Name_ReturnsExpected()
        {
            using DataGridView control = new();
            control.Columns.Add("Column 1", "Header text 1");
            DataGridViewRow row = control.Rows[0];

            Assert.Equal(row.AccessibilityObject.Name, row.HeaderCell.AccessibilityObject.Name);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> DataGridViewRowHeaderCellAccessibleObject_DefaultAction_TestData()
        {
            yield return new object[] { DataGridViewSelectionMode.FullRowSelect, SR.DataGridView_RowHeaderCellAccDefaultAction };
            yield return new object[] { DataGridViewSelectionMode.RowHeaderSelect, SR.DataGridView_RowHeaderCellAccDefaultAction };
            yield return new object[] { DataGridViewSelectionMode.CellSelect, string.Empty };
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewRowHeaderCellAccessibleObject_DefaultAction_TestData))]
        public void DataGridViewRowHeaderCellAccessibleObject_DefaultAction_ReturnsExpected(DataGridViewSelectionMode mode, string expected)
        {
            using DataGridView control = new();
            control.SelectionMode = mode;
            control.Columns.Add("Column 1", "Header text 1");

            var accessibleObject = (DataGridViewRowHeaderCellAccessibleObject)control.Rows[0].HeaderCell.AccessibilityObject;

            Assert.Equal(expected, accessibleObject.DefaultAction);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewSelectionMode.FullRowSelect, true)]
        [InlineData(DataGridViewSelectionMode.RowHeaderSelect, true)]
        [InlineData(DataGridViewSelectionMode.CellSelect, false)]
        public void DataGridViewRowHeaderCellAccessibleObject_DoDefaultAction_WorksExpected(DataGridViewSelectionMode mode, bool expected)
        {
            using DataGridView control = new();
            control.SelectionMode = mode;
            control.Columns.Add("Column 1", "Header text 1");
            DataGridViewRowHeaderCell cell = control.Rows[0].HeaderCell;
            control.CreateControl();

            Assert.False(cell.OwningRow.Selected);

            cell.AccessibilityObject.DoDefaultAction();

            Assert.Equal(expected, cell.OwningRow.Selected);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewSelectionMode.FullRowSelect)]
        [InlineData(DataGridViewSelectionMode.RowHeaderSelect)]
        [InlineData(DataGridViewSelectionMode.CellSelect)]
        public void DataGridViewRowHeaderCellAccessibleObject_DoDefaultAction_DoesNothing_IfHandleIsNotCreated(DataGridViewSelectionMode mode)
        {
            using DataGridView control = new();
            control.SelectionMode = mode;
            control.Columns.Add("Column 1", "Header text 1");
            DataGridViewRowHeaderCell cell = control.Rows[0].HeaderCell;

            Assert.False(cell.OwningRow.Selected);

            cell.AccessibilityObject.DoDefaultAction();

            Assert.False(cell.OwningRow.Selected);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewRowHeaderCellAccessibleObject_Bounds_IsNotEmptyRectangle()
        {
            using DataGridView control = new();
            control.Columns.Add("Column 1", "Header text 1");

            var accessibleObject = (DataGridViewRowHeaderCellAccessibleObject)control.Rows[0].HeaderCell.AccessibilityObject;

            Assert.NotEqual(Rectangle.Empty, accessibleObject.Bounds);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewRowHeaderCellAccessibleObject_FragmentNavigate_IsNull_IfOwningRowIsNull()
        {
            DataGridViewRowHeaderCellAccessibleObject accessibleObject = new(new());

            Assert.Null(accessibleObject.Owner.OwningRow);
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
        }

        [WinFormsFact]
        public void DataGridViewRowHeaderCellAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
        {
            using DataGridView control = new();
            control.Columns.Add("Column 1", "Header text 1");
            DataGridViewRow row = control.Rows[0];

            var accessibleObject = (DataGridViewRowHeaderCellAccessibleObject)row.HeaderCell.AccessibilityObject;

            Assert.Equal(row.AccessibilityObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewRowHeaderCellAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected()
        {
            using DataGridView control = new();
            control.Columns.Add("Column 1", "Header text 1");
            DataGridViewRow row = control.Rows[0];

            var accessibleObject = (DataGridViewRowHeaderCellAccessibleObject)row.HeaderCell.AccessibilityObject;

            Assert.Equal(row.Cells[0].AccessibilityObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.False(control.IsHandleCreated);
        }
    }
}
