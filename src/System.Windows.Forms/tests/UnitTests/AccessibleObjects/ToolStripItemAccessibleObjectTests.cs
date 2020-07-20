// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolStripItemAccessibleObjectTests
    {
        [Fact]
        public void ToolStripItemAccessibleObject_Ctor_ToolStripItem()
        {
            var item = new SubToolStripItem
            {
                AccessibleDefaultActionDescription = "DefaultActionDescription",
                AccessibleDescription = "Description",
                AccessibleName = "Name",
                AccessibleRole = AccessibleRole.MenuBar
            };
            var accessibleObject = new ToolStripItem.ToolStripItemAccessibleObject(item);
            Assert.Equal(Rectangle.Empty, accessibleObject.Bounds);
            Assert.Equal("DefaultActionDescription", accessibleObject.DefaultAction);
            Assert.Equal("Description", accessibleObject.Description);
            Assert.Null(accessibleObject.Help);
            Assert.Empty(accessibleObject.KeyboardShortcut);
            Assert.Equal("Name", accessibleObject.Name);
            Assert.Null(accessibleObject.Parent);
            Assert.Equal(AccessibleRole.MenuBar, accessibleObject.Role);
            Assert.Equal(AccessibleStates.Focusable, accessibleObject.State);
        }

        [Fact]
        public void ToolStripItemAccessibleObject_Ctor_NullOwnerItem_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("ownerItem", () => new ToolStripItem.ToolStripItemAccessibleObject(null));
        }

        [WinFormsFact]
        public void ToolStripItemAccessibleObject_GetPropertyValue_Custom_Name_ReturnsExpected()
        {
            using var toolStripItem = new SubToolStripItem()
            {
                Name = "Name1",
                AccessibleName = "Test Name"
            };

            AccessibleObject toolStripItemAccessibleObject = toolStripItem.AccessibilityObject;
            var accessibleName = toolStripItemAccessibleObject.GetPropertyValue(NativeMethods.UIA_NamePropertyId);

            Assert.Equal("Test Name", accessibleName);
        }

        [WinFormsFact]
        public void ToolStripItemAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue()
        {
            using var toolStripItem = new SubToolStripItem();
            AccessibleObject toolStripItemAccessibleObject = toolStripItem.AccessibilityObject;

            bool supportsLegacyIAccessiblePatternId = toolStripItemAccessibleObject.IsPatternSupported(NativeMethods.UIA_LegacyIAccessiblePatternId);

            Assert.True(supportsLegacyIAccessiblePatternId);
        }

        [WinFormsFact]
        public void ToolStripItemAccessibleObject_LegacyIAccessible_Custom_Role_ReturnsExpected()
        {
            using var toolStripItem = new SubToolStripItem()
            {
                AccessibleRole = AccessibleRole.Link
            };

            AccessibleObject toolStripItemAccessibleObject = toolStripItem.AccessibilityObject;
            var accessibleObjectRole = toolStripItemAccessibleObject.Role;

            Assert.Equal(AccessibleRole.Link, accessibleObjectRole);
        }

        [WinFormsFact]
        public void ToolStripItemAccessibleObject_LegacyIAccessible_Custom_Description_ReturnsExpected()
        {
            using var toolStripItem = new SubToolStripItem()
            {
                AccessibleDescription = "Test Description"
            };

            AccessibleObject toolStripItemAccessibleObject = toolStripItem.AccessibilityObject;
            var accessibleObjectDescription = toolStripItemAccessibleObject.Description;

            Assert.Equal("Test Description", accessibleObjectDescription);
        }

        private class SubToolStripItem : ToolStripItem
        {
            public SubToolStripItem() : base()
            {
            }
        }
    }
}
