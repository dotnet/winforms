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
    }
}
