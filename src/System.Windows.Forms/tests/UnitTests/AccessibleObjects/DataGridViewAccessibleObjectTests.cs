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
        public void DataGridViewAccessibleObject_Ctor_Default()
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

        [WinFormsFact]
        public void DataGridViewAccessibleObject_GetPropertyValue_Name_ReturnsExpected()
        {
            using var dataGridView = new DataGridView()
            {
                Name = "Name1",
                AccessibleName = "Test Name"
            };

            AccessibleObject dataGridViewAccessibleObject = dataGridView.AccessibilityObject;
            var accessibleName = dataGridViewAccessibleObject.GetPropertyValue(NativeMethods.UIA_NamePropertyId);

            Assert.Equal("Test Name", accessibleName);
        }

        [WinFormsFact]
        public void DataGridViewAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue()
        {
            using var dataGridView = new DataGridView();
            AccessibleObject dataGridViewAccessibleObject = dataGridView.AccessibilityObject;

            bool supportsLegacyIAccessiblePatternId = dataGridViewAccessibleObject.IsPatternSupported(NativeMethods.UIA_LegacyIAccessiblePatternId);

            Assert.True(supportsLegacyIAccessiblePatternId);
        }

        [WinFormsFact]
        public void DataGridViewAccessibleObject_LegacyIAccessible_Role_ReturnsExpected()
        {
            using var dataGridView = new DataGridView()
            {
                AccessibleRole = AccessibleRole.Link
            };

            AccessibleObject dataGridViewAccessibleObject = dataGridView.AccessibilityObject;
            var accessibleObjectRole = dataGridViewAccessibleObject.Role;

            Assert.Equal(AccessibleRole.Link, accessibleObjectRole);
        }

        [WinFormsFact]
        public void DataGridViewAccessibleObject_LegacyIAccessible_Description_ReturnsExpected()
        {
            using var dataGridView = new DataGridView()
            {
                AccessibleDescription = "Test Description"
            };

            AccessibleObject dataGridViewAccessibleObject = dataGridView.AccessibilityObject;
            var accessibleObjectDescription = dataGridViewAccessibleObject.Description;

            Assert.Equal("Test Description", accessibleObjectDescription);
        }
    }
}
