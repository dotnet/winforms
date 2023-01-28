// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ColumnHeader;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class ColumnHeader_ListViewColumnHeaderAccessibleObjectTests
    {
        [WinFormsFact]
        public void ListViewColumnHeaderAccessibleObject_Ctor_OwnerColumnHeaderCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ListViewColumnHeaderAccessibleObject(null));
        }

        [WinFormsFact]
        public void ListViewColumnHeaderAccessibleObject_GetPropertyValue_ControlType_ReturnsExpected()
        {
            using ColumnHeader columnHeader = new();

            ListViewColumnHeaderAccessibleObject accessibleObject = new(columnHeader);

            Assert.Equal(UIA.HeaderItemControlTypeId, accessibleObject.GetPropertyValue(UIA.ControlTypePropertyId));
        }

        [WinFormsFact]
        public void ListViewColumnHeaderAccessibleObject_GetPropertyValue_Name_ReturnsExpected()
        {
            string testText = "This is a simple text for testing.";
            using ColumnHeader columnHeader = new() { Text = testText };

            ListViewColumnHeaderAccessibleObject accessibleObject = new(columnHeader);

            Assert.Equal(testText, accessibleObject.GetPropertyValue(UIA.NamePropertyId));
            Assert.Equal(testText, accessibleObject.GetPropertyValue(UIA.LegacyIAccessibleNamePropertyId));
            Assert.Null(accessibleObject.GetPropertyValue(UIA.LegacyIAccessibleDefaultActionPropertyId));
        }

        [WinFormsFact]
        public void ListViewColumnHeaderAccessibleObject_IsDisconnected_WhenListViewReleasesUiaProvider()
        {
            using ListView listView = new();
            using ColumnHeader columnHeader = new();
            listView.Columns.Add(columnHeader);
            EnforceAccessibleObjectCreation(columnHeader);
            _ = listView.AccessibilityObject;

            listView.ReleaseUiaProvider(listView.HWND);

            Assert.Null(columnHeader.TestAccessor().Dynamic._accessibilityObject);
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewColumnHeaderAccessibleObject_IsDisconnected_WhenListViewIsCleared()
        {
            using ListView listView = new();
            using ColumnHeader columnHeader = new();
            listView.Columns.Add(columnHeader);
            EnforceAccessibleObjectCreation(columnHeader);

            listView.Clear();

            Assert.Null(columnHeader.TestAccessor().Dynamic._accessibilityObject);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewColumnHeaderAccessibleObject_IsDisconnected_WhenColumnsAreCleared()
        {
            using ListView listView = new();
            using ColumnHeader columnHeader = new();
            listView.Columns.Add(columnHeader);
            EnforceAccessibleObjectCreation(columnHeader);

            listView.Columns.Clear();

            Assert.Null(columnHeader.TestAccessor().Dynamic._accessibilityObject);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewColumnHeaderAccessibleObject_IsDisconnected_WhenColumnIsRemoved()
        {
            using ListView listView = new();
            using ColumnHeader columnHeader = new();
            listView.Columns.Add(columnHeader);
            EnforceAccessibleObjectCreation(columnHeader);

            listView.Columns.Remove(columnHeader);

            Assert.Null(columnHeader.TestAccessor().Dynamic._accessibilityObject);
            Assert.False(listView.IsHandleCreated);
        }

        private static void EnforceAccessibleObjectCreation(ColumnHeader columnHeader)
        {
            _ = columnHeader.AccessibilityObject;
            Assert.NotNull(columnHeader.TestAccessor().Dynamic._accessibilityObject);
        }
    }
}
