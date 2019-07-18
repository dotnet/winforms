// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class DataGridViewAccessibleObjectTests
    {
        [Fact]
        public void PropertyGridAccessibleObject_Ctor_Default()
        {
            DataGridView dataGridView = new DataGridView();

            AccessibleObject accessibleObject = dataGridView.AccessibilityObject;
            Assert.NotNull(accessibleObject);
            Assert.Equal(AccessibleRole.Table, accessibleObject.Role);
        }

        [Fact]
        public void DataGridViewAccessibleObject_ItemStatus()
        {
            DataGridView dataGridView = new DataGridView();
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.SortMode = DataGridViewColumnSortMode.Programmatic;
            column.HeaderText = "Some column";

            dataGridView.Columns.Add(column);
            dataGridView.Sort(dataGridView.Columns[0], ListSortDirection.Ascending);

            string itemStatus = dataGridView.AccessibilityObject.GetPropertyValue(NativeMethods.UIA_ItemStatusPropertyId)?.ToString();
            string expectedStatus = "Sorted ascending by Some column.";
            Assert.Equal(expectedStatus, itemStatus);
        }
    }
}
