﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using InternalUtilitiesForTests.src;
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
            return ReflectionHelper.GetDerivedPublicNotAbstractClasses<ToolStripItem>();
        }

        [Theory]
        [MemberData(nameof(ToolStripItemObject_TestData))]
        public void ToolStripItemAccessibleObject_LegacyIAccessible_Custom_Role_ReturnsExpected(Type type)
        {
            using ToolStripItem item = ReflectionHelper.InvokePublicConstructor<ToolStripItem>(type);
            if (item == null)
            {
                return;
            }
            item.AccessibleRole = AccessibleRole.Link;
            AccessibleObject toolStripItemAccessibleObject = item.AccessibilityObject;

            var accessibleObjectRole = toolStripItemAccessibleObject.Role;

            Assert.Equal(AccessibleRole.Link, accessibleObjectRole);
        }

        [Theory]
        [MemberData(nameof(ToolStripItemObject_TestData))]
        public void ToolStripItemAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue(Type type)
        {
            using ToolStripItem item = ReflectionHelper.InvokePublicConstructor<ToolStripItem>(type);
            if (item == null)
            {
                return;
            }
            AccessibleObject toolStripItemAccessibleObject = item.AccessibilityObject;

            bool supportsLegacyIAccessiblePatternId = toolStripItemAccessibleObject.IsPatternSupported(NativeMethods.UIA_LegacyIAccessiblePatternId);

            Assert.True(supportsLegacyIAccessiblePatternId);
        }

        [Theory]
        [MemberData(nameof(ToolStripItemObject_TestData))]
        public void ToolStripItemAccessibleObject_LegacyIAccessible_Custom_Description_ReturnsExpected(Type type)
        {
            using ToolStripItem item = ReflectionHelper.InvokePublicConstructor<ToolStripItem>(type);
            if (item == null)
            {
                return;
            }

            item.AccessibleDescription = "Test Accessible Description";
            AccessibleObject toolStripItemAccessibleObject = item.AccessibilityObject;

            var accessibleObjectDescription = toolStripItemAccessibleObject.Description;

            Assert.Equal("Test Accessible Description", accessibleObjectDescription);
        }

        [Theory]
        [MemberData(nameof(ToolStripItemObject_TestData))]
        public void ToolStripItemAccessibleObject_GetPropertyValue_Custom_Name_ReturnsExpected(Type type)
        {
            using ToolStripItem item = ReflectionHelper.InvokePublicConstructor<ToolStripItem>(type);
            if (item == null)
            {
                return;
            }

            item.Name = "Name1";
            item.AccessibleName = "Test Name";

            AccessibleObject toolStripItemAccessibleObject = item.AccessibilityObject;
            var accessibleName = toolStripItemAccessibleObject.GetPropertyValue(NativeMethods.UIA_NamePropertyId);

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
