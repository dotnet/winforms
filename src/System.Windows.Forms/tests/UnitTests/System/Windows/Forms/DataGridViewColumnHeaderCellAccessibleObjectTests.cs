// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewColumnHeaderCellAccessibleObjectTests : DataGridViewColumnHeaderCell
    {
        [WinFormsFact]
        public void DataGridViewColumnHeaderCellAccessibleObject_Ctor_Default()
        {
            var accessibleObject = new DataGridViewColumnHeaderCellAccessibleObject(null);
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

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.InvokePatternId)]
        [InlineData((int)UiaCore.UIA.LegacyIAccessiblePatternId)]
        public void DataGridViewColumnHeaderCellAccessibleObject_IsPatternSupported_ReturnsExpected(int patternId)
        {
            var accessibleObject = new DataGridViewColumnHeaderCellAccessibleObject(null);
            Assert.True((bool)accessibleObject.IsPatternSupported((UiaCore.UIA)patternId));
        }

        [WinFormsFact]
        public void DataGridViewColumnHeaderCellAccessibleObject_ControlType_ReturnsExpected()
        {
            var accessibleObject = new DataGridViewColumnHeaderCellAccessibleObject(null);
            Assert.Equal(UiaCore.UIA.HeaderControlTypeId, accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
        }

        [WinFormsFact]
        public void DataGridViewColumnHeaderCellAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
        {
            using DataGridView control = new();
            control.Columns.Add("Column 1", "Header text 1");
            DataGridViewColumn column = control.Columns[0];

            var accessibleObject = (DataGridViewColumnHeaderCellAccessibleObject)column.HeaderCell.AccessibilityObject;

            Assert.Equal(control.AccessibilityObject.GetChild(0), accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.False(control.IsHandleCreated);
        }
    }
}
