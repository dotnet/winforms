// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
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

        public static IEnumerable<object[]> ToolStripItemObject_TestData()
        {
            var types = typeof(ToolStripItem).Assembly.GetTypes().Where(type => IsNotAbstractToolStripItem(type));
            foreach (var type in types)
            {
                yield return new object[] { type };
            }
        }

        [Theory]
        [MemberData(nameof(ToolStripItemObject_TestData))]
        public void ToolStripItemAccessibleObject_LegacyIAccessible_Custom_Role_ReturnsExpected(Type type)
        {
            using ToolStripItem control = GetToolStripItem(type);
            if (control == null)
            {
                return;
            }
            control.AccessibleRole = AccessibleRole.Link;
            AccessibleObject toolStripItemAccessibleObject = control.AccessibilityObject;

            var accessibleObjectRole = toolStripItemAccessibleObject.Role;

            Assert.Equal(AccessibleRole.Link, accessibleObjectRole);
        }

        [Theory]
        [MemberData(nameof(ToolStripItemObject_TestData))]
        public void ToolStripItemAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue(Type type)
        {
            using ToolStripItem control = GetToolStripItem(type);
            if (control == null)
            {
                return;
            }
            AccessibleObject toolStripItemAccessibleObject = control.AccessibilityObject;

            bool supportsLegacyIAccessiblePatternId = toolStripItemAccessibleObject.IsPatternSupported(NativeMethods.UIA_LegacyIAccessiblePatternId);

            Assert.True(supportsLegacyIAccessiblePatternId);
        }

        [Theory]
        [MemberData(nameof(ToolStripItemObject_TestData))]
        public void ToolStripItemAccessibleObject_LegacyIAccessible_Custom_Description_ReturnsExpected(Type type)
        {
            using ToolStripItem control = GetToolStripItem(type);
            if (control == null)
            {
                return;
            }

            control.AccessibleDescription = "Test Accessible Description";
            AccessibleObject toolStripItemAccessibleObject = control.AccessibilityObject;

            var accessibleObjectDescription = toolStripItemAccessibleObject.Description;

            Assert.Equal("Test Accessible Description", accessibleObjectDescription);
        }

        [Theory]
        [MemberData(nameof(ToolStripItemObject_TestData))]
        public void ToolStripItemAccessibleObject_GetPropertyValue_Custom_Name_ReturnsExpected(Type type)
        {
            using ToolStripItem control = GetToolStripItem(type);
            if (control == null)
            {
                return;
            }

            control.Name = "Name1";
            control.AccessibleName = "Test Name";

            AccessibleObject toolStripItemAccessibleObject = control.AccessibilityObject;
            var accessibleName = toolStripItemAccessibleObject.GetPropertyValue(NativeMethods.UIA_NamePropertyId);

            Assert.Equal("Test Name", accessibleName);
        }

        private static bool IsNotAbstractToolStripItem(Type type)
        {
            return !type.IsAbstract && typeof(ToolStripItem).IsAssignableFrom(type);
        }

        private ToolStripItem GetToolStripItem(Type type)
        {
            var ctor = type.GetConstructor(
                bindingAttr: BindingFlags.Public | BindingFlags.Instance,
                binder: null,
                types: Array.Empty<Type>(),
                modifiers: null);

            if (ctor == null)
            {
                return null;
            }

            return (ToolStripItem)ctor.Invoke(Array.Empty<object>());
        }

        private class SubToolStripItem : ToolStripItem
        {
            public SubToolStripItem() : base()
            {
            }
        }
    }
}
