// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewTopLeftHeaderCellAccessibleObjectTests : DataGridViewTopLeftHeaderCell
    {
        [WinFormsFact]
        public void DataGridViewTopLeftHeaderCellAccessibleObject_Ctor_Default()
        {
            var accessibleObject = new DataGridViewTopLeftHeaderCellAccessibleObject(null);

            Assert.Null(accessibleObject.Owner);
        }

        [WinFormsFact]
        public void DataGridViewTopLeftHeaderCellAccessibleObject_Bounds_ThrowsException_IfOwnerIsNull()
        {
            Assert.Throws<InvalidOperationException>(() =>
                new DataGridViewTopLeftHeaderCellAccessibleObject(null).Bounds);
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

        public static IEnumerable<object[]> DataGridViewTopLeftHeaderCellAccessibleObject_DefaultAction_TestData()
        {
            yield return new object[] { true, SR.DataGridView_AccTopLeftColumnHeaderCellDefaultAction };
            yield return new object[] { false, string.Empty };
        }

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
        public void DataGridViewTopLeftHeaderCellAccessibleObject_Value_ReturnsExpected()
        {
            using DataGridViewTopLeftHeaderCell cell = new();

            Assert.Equal(string.Empty, cell.AccessibilityObject.Value);
        }

        [WinFormsFact]
        public void DataGridViewTopLeftHeaderCellAccessibleObject_ControlType_ReturnsExpected()
        {
            var accessibleObject = new DataGridViewTopLeftHeaderCellAccessibleObject(null);

            UiaCore.UIA expected = UiaCore.UIA.HeaderControlTypeId;

            Assert.Equal(expected, accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewTopLeftHeaderCellAccessibleObject_IsEnabled_ReturnsExpected(bool isEnabled)
        {
            using DataGridView control = new();
            using DataGridViewTopLeftHeaderCell cell = new();

            control.TopLeftHeaderCell = cell;
            control.Enabled = isEnabled;

            Assert.Equal(isEnabled, cell.AccessibilityObject.GetPropertyValue(UiaCore.UIA.IsEnabledPropertyId));
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> DataGridViewTopLeftHeaderCellAccessibleObject_Name_TestData()
        {
            yield return new object[] { RightToLeft.No, SR.DataGridView_AccTopLeftColumnHeaderCellName };
            yield return new object[] { RightToLeft.Yes, SR.DataGridView_AccTopLeftColumnHeaderCellNameRTL };
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewTopLeftHeaderCellAccessibleObject_Name_TestData))]
        public void DataGridViewTopLeftHeaderCellAccessibleObject_Name_ReturnsExpected(RightToLeft value, string expected)
        {
            using DataGridView control = new();
            using DataGridViewTopLeftHeaderCell cell = new();

            control.TopLeftHeaderCell = cell;
            control.RightToLeft = value;

            Assert.Equal(expected, cell.AccessibilityObject.Name);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewTopLeftHeaderCellAccessibleObject_Name_ReturnsEmptyString_IfOwnerValueIsNotEmptySring()
        {
            using DataGridView control = new();
            using DataGridViewTopLeftHeaderCell cell = new();

            cell.Value = "It is not empty string";
            control.TopLeftHeaderCell = cell;

            Assert.Equal(string.Empty, cell.AccessibilityObject.Name);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewTopLeftHeaderCellAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
        {
            using DataGridView control = new();
            using DataGridViewTopLeftHeaderCell cell = new();

            control.TopLeftHeaderCell = cell;
            AccessibleObject expected = control.AccessibilityObject.GetChild(0);

            Assert.Equal(expected, cell.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewTopLeftHeaderCellAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected()
        {
            using DataGridView control = new();
            using DataGridViewTopLeftHeaderCell cell = new();

            control.TopLeftHeaderCell = cell;

            Assert.Null(cell.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewTopLeftHeaderCellAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected()
        {
            using DataGridView control = new();
            using DataGridViewTopLeftHeaderCell cell = new();

            control.TopLeftHeaderCell = cell;
            AccessibleObject expected = control.AccessibilityObject.GetChild(0)?.GetChild(1);

            Assert.Equal(expected, cell.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewTopLeftHeaderCellAccessibleObject_FragmentNavigate_NextSibling_ReturnsNull_IfDataGridViewHasNoVisibleCollumns()
        {
            using DataGridView control = new();
            using DataGridViewTopLeftHeaderCell cell = new();

            control.TopLeftHeaderCell = cell;

            Assert.Null(cell.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewTopLeftHeaderCellAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected_IfFirstColumnHidden()
        {
            using DataGridView control = new();
            control.Columns.AddRange(new DataGridViewTextBoxColumn(), new DataGridViewTextBoxColumn());
            control.Columns[0].Visible = false;

            using DataGridViewTopLeftHeaderCell cell = new();
            control.TopLeftHeaderCell = cell;

            AccessibleObject expected = control.Columns[1].HeaderCell.AccessibilityObject;

            Assert.Equal(expected, cell.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewTopLeftHeaderCellAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected_IfCustomOrder()
        {
            using DataGridView control = new();
            control.Columns.AddRange(new DataGridViewTextBoxColumn(), new DataGridViewTextBoxColumn());
            control.Columns[0].DisplayIndex = 1;
            control.Columns[1].DisplayIndex = 0;

            using DataGridViewTopLeftHeaderCell cell = new();
            control.TopLeftHeaderCell = cell;

            AccessibleObject expected = control.Columns[1].HeaderCell.AccessibilityObject;

            Assert.Equal(expected, cell.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewTopLeftHeaderCellAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected_IfCustomOrderAndFirstDisplayedColumnHidden()
        {
            using DataGridView control = new();
            control.Columns.AddRange(new DataGridViewTextBoxColumn(), new DataGridViewTextBoxColumn());
            control.Columns[0].DisplayIndex = 1;
            control.Columns[1].DisplayIndex = 0;
            control.Columns[1].Visible = false;

            using DataGridViewTopLeftHeaderCell cell = new();
            control.TopLeftHeaderCell = cell;

            AccessibleObject expected = control.Columns[0].HeaderCell.AccessibilityObject;

            Assert.Equal(expected, cell.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewTopLeftHeaderCellAccessibleObject_FragmentNavigate_Child_ReturnsNull()
        {
            using DataGridView control = new();

            using DataGridViewTopLeftHeaderCell cell = new();
            control.TopLeftHeaderCell = cell;

            Assert.Null(cell.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(cell.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));

            Assert.False(control.IsHandleCreated);
        }
    }
}
