// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewTopRowAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void DataGridViewTopRowAccessibleObject_FragmentNavigate_ReturnsExpected_AllowUserToAddRowsEnabled()
        {
            using DataGridView dataGridView = new();
            DataGridViewTextBoxColumn dataGridViewColumn = new() {Name = "Col 1", HeaderText = "Col 1" };

            dataGridView.Columns.Add(dataGridViewColumn);

            AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
            AccessibleObject expectedNextSibling = dataGridView.Rows[0].AccessibilityObject;
            AccessibleObject expectedFirstChild = dataGridView.TopLeftHeaderCell.AccessibilityObject;
            AccessibleObject expectedLastChild = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

            Assert.Equal(dataGridView.AccessibilityObject, topRowAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.Parent));
            Assert.Null(topRowAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(expectedNextSibling, topRowAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling));
            Assert.Equal(expectedFirstChild, topRowAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild));
            Assert.Equal(expectedLastChild, topRowAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.LastChild));
        }

        [WinFormsFact]
        public void DataGridViewTopRowAccessibleObject_FragmentNavigate_ReturnsExpected_AllowUserToAddRowsDisabled()
        {
            using DataGridView dataGridView = new() { AllowUserToAddRows = false };
            DataGridViewTextBoxColumn dataGridViewColumn = new() { Name = "Col 1", HeaderText = "Col 1" };

            dataGridView.Columns.Add(dataGridViewColumn);

            AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
            AccessibleObject expectedFirstChild = dataGridView.TopLeftHeaderCell.AccessibilityObject;
            AccessibleObject expectedLastChild = dataGridView.Columns[0].HeaderCell.AccessibilityObject;

            Assert.Equal(dataGridView.AccessibilityObject, topRowAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.Parent));
            Assert.Null(topRowAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(topRowAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling));
            Assert.Equal(expectedFirstChild, topRowAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild));
            Assert.Equal(expectedLastChild, topRowAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.LastChild));
        }

        [WinFormsFact]
        public void DataGridViewTopRowAccessibleObject_FragmentNavigate_ReturnsExpected_WithoutColumns()
        {
            using DataGridView dataGridView = new();

            AccessibleObject topRowAccessibleObject = dataGridView.AccessibilityObject.TestAccessor().Dynamic.TopRowAccessibilityObject;
            AccessibleObject topLeftAccessibilityObject = dataGridView.TopLeftHeaderCell.AccessibilityObject;

            Assert.Equal(dataGridView.AccessibilityObject, topRowAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.Parent));
            Assert.Null(topRowAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(topRowAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling));
            Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild));
            Assert.Equal(topLeftAccessibilityObject, topRowAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.LastChild));
        }
    }
}
