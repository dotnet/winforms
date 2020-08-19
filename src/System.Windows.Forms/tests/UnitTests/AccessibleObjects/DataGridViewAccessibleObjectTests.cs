// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class DataGridViewAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void DataGridViewAccessibleObject_Ctor_Default()
        {
            using DataGridView dataGridView = new DataGridView();

            AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
            Assert.NotNull(accessibleObject);
            Assert.Equal(AccessibleRole.Table, accessibleObject.Role);
        }

        [WinFormsFact]
        public void DataGridViewAccessibleObject_ItemStatus_ReturnsAsSorted()
        {
            using DataGridView dataGridView = new DataGridView();
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.SortMode = DataGridViewColumnSortMode.Programmatic;
            column.HeaderText = "Some column";

            dataGridView.Columns.Add(column);
            dataGridView.Sort(dataGridView.Columns[0], ListSortDirection.Ascending);

            string itemStatus = dataGridView.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ItemStatusPropertyId)?.ToString();
            string expectedStatus = "Sorted ascending by Some column.";
            Assert.Equal(expectedStatus, itemStatus);
        }

        [WinFormsFact]
        public void DataGridViewAccessibleObject_EmptyGrid_GetChildCount_ReturnsCorrectValue()
        {
            using DataGridView dataGridView = new DataGridView();
            AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
            Assert.Equal(0, accessibleObject.GetChildCount()); // dataGridView doesn't have items
        }

        [WinFormsFact]
        public void DataGridViewAccessibleObject_GridWithFirstRowOnly_GetChildCount_ReturnsCorrectValue()
        {
            using DataGridView dataGridView = new DataGridView();
            AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView.ColumnHeadersVisible = false;
            Assert.Equal(1, accessibleObject.GetChildCount()); // A first row only
        }

        [WinFormsFact]
        public void DataGridViewAccessibleObject_GridWithColumnHeadersAndFirstRow_GetChildCount_ReturnsCorrectValue()
        {
            using DataGridView dataGridView = new DataGridView();
            AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
            Assert.Equal(2, accessibleObject.GetChildCount()); // Column headers and a first Row
        }

        [WinFormsFact]
        public void DataGridViewAccessibleObject_EmptyGrid_GetChild_ReturnsCorrectValue()
        {
            using DataGridView dataGridView = new DataGridView();
            AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
            Assert.Equal(0, accessibleObject.GetChildCount()); // dataGridView doesn't have an item
            Assert.Null(accessibleObject.GetChild(0)); // GetChild method should not throw an exception
        }

        [WinFormsFact]
        public void DataGridViewAccessibleObject_GridWithFirstRowOnly_GetChild_ReturnsCorrectValue()
        {
            using DataGridView dataGridView = new DataGridView();
            AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView.ColumnHeadersVisible = false;
            Assert.NotNull(accessibleObject.GetChild(0)); // dataGridView a first empty row.
            Assert.Null(accessibleObject.GetChild(1)); // GetChild method should not throw an exception
        }

        [WinFormsFact]
        public void DataGridViewAccessibleObject_GridWithColumnHeadersAndFirstRow_GetChild_ReturnsCorrectValue()
        {
            using DataGridView dataGridView = new DataGridView();
            AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
            Assert.NotNull(accessibleObject.GetChild(0)); // dataGridView column headers
            Assert.NotNull(accessibleObject.GetChild(1)); // dataGridView a first empty row
        }

        [WinFormsFact]
        public void DataGridViewAccessibleObject_Bounds_ReturnsCorrectValue_IfHandleIsCreated()
        {
            using DataGridView dataGridView = new DataGridView();
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
            using DataGridView dataGridView = new DataGridView();
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
        public void DataGridViewAccessibleObject_ControlType_IsTable()
        {
            using DataGridView dataGridView = new DataGridView();
            AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = UiaCore.UIA.TableControlTypeId;
            Assert.Equal(expected, actual);
        }

        [WinFormsFact]
        public void DataGridViewAccessibleObject_GetFocused_ReturnsCorrectFocusedCell()
        {
            using DataGridView dataGridView = new DataGridView();
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
            User32.SetFocus(new HandleRef(dataGridView, dataGridView.Handle));

            DataGridViewCell cell = dataGridView.Rows[0].Cells[0];
            Assert.NotNull(cell);

            dataGridView.CurrentCell = cell;
            Assert.True(cell.Selected);

            AccessibleObject focusedCell = dataGridView.AccessibilityObject.GetFocused();
            Assert.NotNull(focusedCell);
            Assert.Equal(cell.AccessibilityObject, focusedCell);
        }

        [WinFormsFact]
        public void DataGridViewAccessibleObject_GetItem_ReturnsCorrectValue()
        {
            using DataGridView dataGridView = new DataGridView();
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
        }

        [WinFormsFact]
        public void DataGridViewAccessibleObject_ItemStatus_IsCorrectWhenSorted()
        {
            using DataGridView dataGridView = new DataGridView();
            using DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.SortMode = DataGridViewColumnSortMode.Programmatic;
            column.HeaderText = "Some column";

            dataGridView.Columns.Add(column);
            dataGridView.Sort(dataGridView.Columns[0], ListSortDirection.Ascending);

            string actualStatus = dataGridView.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ItemStatusPropertyId)?.ToString();
            string expectedStatus = "Sorted ascending by Some column.";
            Assert.Equal(expectedStatus, actualStatus);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleStates.Focusable)]
        [InlineData(false, AccessibleStates.None)]
        public void DataGridViewAccessibleObject_State_IsFocusable(bool createControl, AccessibleStates expectedAccessibleStates)
        {
            using DataGridView dataGridView = new DataGridView();
            if (createControl)
            {
                dataGridView.CreateControl();
            }

            Assert.Equal(createControl, dataGridView.IsHandleCreated);
            AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
            Assert.Equal(createControl, dataGridView.IsHandleCreated);
            Assert.Equal(expectedAccessibleStates, accessibleObject.State & AccessibleStates.Focusable);
        }

        [WinFormsFact]
        public void DataGridViewAccessibleObject_Ctor_NullOwnerParameter_ThrowsArgumentNullException()
        {
            using DataGridView dataGridView = new DataGridView();
            Type type = dataGridView.AccessibilityObject.GetType();
            ConstructorInfo ctor = type.GetConstructor(new Type[] { typeof(DataGridView) });
            Assert.NotNull(ctor);
            Assert.Throws<TargetInvocationException>(() => ctor.Invoke(new object[] { null }));
        }

        [WinFormsFact]
        public void DataGridViewAccessibleObject_FirstAndLastChildren_AreNotNull()
        {
            using DataGridView dataGridView = new DataGridView();
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
            AccessibleObject accessibleObject = dataGridView.AccessibilityObject;

            // ColumnHeaders
            UiaCore.IRawElementProviderFragment firstChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
            Assert.NotNull(firstChild);

            // New row
            UiaCore.IRawElementProviderFragment lastChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild);
            Assert.NotNull(lastChild);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.IsGridPatternAvailablePropertyId)]
        [InlineData((int)UiaCore.UIA.IsTablePatternAvailablePropertyId)]
        public void DataGridViewAccessibleObject_Pattern_IsAvailable(int propertyId)
        {
            using DataGridView dataGridView = new DataGridView();
            AccessibleObject accessibilityObject = dataGridView.AccessibilityObject;
            Assert.True((bool)accessibilityObject.GetPropertyValue((UiaCore.UIA)propertyId));
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void DataGridViewAccessibleObject_Cell_IsOffscreen_ReturnsCorrectValue(bool createControl)
        {
            using DataGridView dataGridView = new DataGridView();

            if (createControl)
            {
                dataGridView.CreateControl();
            }

            AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
            dataGridView.Size = new Size(200, 100);
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn());

            dataGridView.Rows.Add(); // 1
            object isOffscreen = dataGridView.Rows[1].Cells[0].AccessibilityObject.GetPropertyValue(UiaCore.UIA.IsOffscreenPropertyId);
            Assert.False((bool)isOffscreen); // Within the visible area

            dataGridView.Rows.Add(); // 2
            dataGridView.Rows.Add(); // 3
            dataGridView.Rows.Add(); // 4
            isOffscreen = dataGridView.Rows[4].Cells[0].AccessibilityObject.GetPropertyValue(UiaCore.UIA.IsOffscreenPropertyId);

            Assert.Equal(createControl, (bool)isOffscreen); // Out of the visible area
            Assert.Equal(createControl, dataGridView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.TablePatternId)]
        [InlineData((int)UiaCore.UIA.GridPatternId)]
        public void DataGridViewAccessibleObject_IsPatternSupported(int patternId)
        {
            using DataGridView dataGridView = new DataGridView();
            AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
            Assert.True(accessibleObject.IsPatternSupported((UiaCore.UIA)patternId));
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void DataGridViewAccessibleObject_Cell_IsReadOnly_ReturnsCorrectValue(bool isReadOnly)
        {
            using DataGridView dataGridView = new DataGridView();
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
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void DataGridViewAccessibleObject_Row_IsReadOnly_ReturnsCorrectValue(bool isReadOnly)
        {
            using DataGridView dataGridView = new DataGridView();
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
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void DataGridViewAccessibleObject_Grid_IsReadOnly_ReturnsCorrectValue(bool isReadOnly)
        {
            using DataGridView dataGridView = new DataGridView();
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
        }

        [WinFormsFact]
        public void DataGridViewAccessibleObject_Owner_IsNotNull()
        {
            using DataGridView dataGridView = new DataGridView();
            Control.ControlAccessibleObject accessibleObject = (Control.ControlAccessibleObject)dataGridView.AccessibilityObject;
            Assert.NotNull(accessibleObject.Owner);
        }

        [WinFormsFact]
        public void DataGridViewAccessibleObject_Parent_IsNotNull_IfHandleIsCreated()
        {
            using DataGridView dataGridView = new DataGridView();
            dataGridView.CreateControl();
            AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
            Assert.NotNull(accessibleObject.Parent);
            Assert.True(dataGridView.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewAccessibleObject_Parent_IsNotNull_IfHandleIsNotCreated()
        {
            using DataGridView dataGridView = new DataGridView();
            AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
            Assert.Null(accessibleObject.Parent);
            Assert.False(dataGridView.IsHandleCreated);
        }
    }
}
