// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Xunit;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class ToolStripItemAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripItemAccessibleObject_Ctor_ToolStripItem()
        {
            using var item = new SubToolStripItem
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

        [WinFormsFact]
        public void ToolStripItemAccessibleObject_Ctor_NullOwnerItem_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("ownerItem", () => new ToolStripItem.ToolStripItemAccessibleObject(null));
        }

        public static IEnumerable<object[]> ToolStripItemAccessibleObject_TestData()
        {
            return ReflectionHelper.GetPublicNotAbstractClasses<ToolStripItem>().Select(type => new object[] { type });
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemAccessibleObject_TestData))]
        public void ToolStripItemAccessibleObject_LegacyIAccessible_Custom_Role_ReturnsExpected(Type type)
        {
            using (new NoAssertContext())
            {
                using ToolStripItem item = ReflectionHelper.InvokePublicConstructor<ToolStripItem>(type);
                item.AccessibleRole = AccessibleRole.Link;
                AccessibleObject toolStripItemAccessibleObject = item.AccessibilityObject;

                var accessibleObjectRole = toolStripItemAccessibleObject.Role;

                Assert.Equal(AccessibleRole.Link, accessibleObjectRole);
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemAccessibleObject_TestData))]
        public void ToolStripItemAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue(Type type)
        {
            using ToolStripItem item = ReflectionHelper.InvokePublicConstructor<ToolStripItem>(type);
            AccessibleObject toolStripItemAccessibleObject = item.AccessibilityObject;

            bool supportsLegacyIAccessiblePatternId = toolStripItemAccessibleObject.IsPatternSupported(UIA.LegacyIAccessiblePatternId);

            Assert.True(supportsLegacyIAccessiblePatternId);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemAccessibleObject_TestData))]
        public void ToolStripItemAccessibleObject_LegacyIAccessible_Custom_Description_ReturnsExpected(Type type)
        {
            using ToolStripItem item = ReflectionHelper.InvokePublicConstructor<ToolStripItem>(type);
            item.AccessibleDescription = "Test Accessible Description";
            AccessibleObject toolStripItemAccessibleObject = item.AccessibilityObject;

            var accessibleObjectDescription = toolStripItemAccessibleObject.Description;

            Assert.Equal("Test Accessible Description", accessibleObjectDescription);
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemAccessibleObject_TestData))]
        public void ToolStripItemAccessibleObject_GetPropertyValue_Custom_Name_ReturnsExpected(Type type)
        {
            using ToolStripItem item = ReflectionHelper.InvokePublicConstructor<ToolStripItem>(type);
            AccessibleObject toolStripItemAccessibleObject = item.AccessibilityObject;

            // By default Name has string.Empty value, because if AccessibleName is not defined
            // then control uses the value of "Text" property from owner Item (by default an empty string)
            Assert.Equal(string.Empty, toolStripItemAccessibleObject.GetPropertyValue(UIA.NamePropertyId));

            item.Name = "Name1";
            item.AccessibleName = "Test Name";

            var accessibleName = toolStripItemAccessibleObject.GetPropertyValue(UIA.NamePropertyId);

            Assert.Equal("Test Name", accessibleName);
        }

        private class SubToolStripItem : ToolStripItem
        {
            public SubToolStripItem() : base()
            {
            }
        }
    }
}
