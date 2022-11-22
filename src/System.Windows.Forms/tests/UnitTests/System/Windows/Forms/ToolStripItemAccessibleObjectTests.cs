// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
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

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemAccessibleObject_TestData))]
        public void ToolStripHostedControlAccessibleObject_GetPropertyValue_IsOffscreenPropertyId_ReturnExpected(Type type)
        {
            using var toolStrip = new ToolStrip();
            toolStrip.CreateControl();
            using ToolStripItem item = ReflectionHelper.InvokePublicConstructor<ToolStripItem>(type);
            item.Size = new Size(0, 0);
            toolStrip.Items.Add(item);

            AccessibleObject toolStripItemAccessibleObject = item.AccessibilityObject;

            Assert.True((bool)toolStripItemAccessibleObject.GetPropertyValue(UIA.IsOffscreenPropertyId) ||
                (toolStripItemAccessibleObject.Bounds.Width > 0 && toolStripItemAccessibleObject.Bounds.Height > 0));
        }

        [WinFormsFact]
        public void ToolStripItemAccessibleObject_ControlType_IsButton_IfAccessibleRoleIsDefault()
        {
            // Test the Default role case separately because ToolStripItemAccessibleObject
            // has default Role property value as "PushButton"

            using ToolStripItem toolStripItem = new SubToolStripItem();
            // AccessibleRole is not set = Default

            UIA actual = (UIA)toolStripItem.AccessibilityObject.GetPropertyValue(UIA.ControlTypePropertyId);

            Assert.Equal(UIA.ButtonControlTypeId, actual);
        }

        [WinFormsFact]
        public static IEnumerable<object[]> ToolStripItemAccessibleObject_GetPropertyValue_ControlTypeProperty_ReturnsCorrectValue_TestData()
        {
            Array roles = Enum.GetValues(typeof(AccessibleRole));

            foreach (AccessibleRole role in roles)
            {
                if (role == AccessibleRole.Default)
                {
                    continue; // The test checks custom roles
                }

                yield return new object[] { role };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemAccessibleObject_GetPropertyValue_ControlTypeProperty_ReturnsCorrectValue_TestData))]
        public void ToolStripItemAccessibleObject_GetPropertyValue_ControlTypeProperty_ReturnsCorrectValue(AccessibleRole role)
        {
            using ToolStripItem toolStripItem = new SubToolStripItem();
            toolStripItem.AccessibleRole = role;

            UIA actual = (UIA)toolStripItem.AccessibilityObject.GetPropertyValue(UIA.ControlTypePropertyId);
            UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            // Check if the method returns an exist UIA_ControlTypeId
            Assert.True(actual >= UIA.ButtonControlTypeId && actual <= UIA.AppBarControlTypeId);
        }

        [WinFormsFact]
        public void ToolStripItemAccessibleObject_GetPropertyValue_ReturnsExpected()
        {
            using ToolStripItem toolStripItem = new SubToolStripItem();

            Assert.False((bool)toolStripItem.AccessibilityObject.GetPropertyValue(UIA.IsExpandCollapsePatternAvailablePropertyId));
            Assert.Null(toolStripItem.AccessibilityObject.GetPropertyValue(UIA.ValueValuePropertyId));
        }

        [WinFormsFact]
        public void ToolStripItemAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
        {
            using ToolStrip toolStrip = new();
            toolStrip.Items.Add("");
            toolStrip.PerformLayout();
            toolStrip.CreateControl();

            AccessibleObject accessibleObject = toolStrip.Items[0].AccessibilityObject;
            AccessibleObject expected = toolStrip.AccessibilityObject;

            Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.Parent));
        }

        [WinFormsFact]
        public void ToolStripItemAccessibleObject_FragmentNavigate_Parent_ReturnsNull_IfHandleNotCreated()
        {
            using ToolStrip toolStrip = new();
            toolStrip.Items.Add("");

            AccessibleObject accessibleObject = toolStrip.Items[0].AccessibilityObject;

            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.Parent));
            Assert.False(toolStrip.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripItemAccessibleObject_FragmentNavigate_Child_ReturnExpected()
        {
            using ToolStrip toolStrip = new();
            toolStrip.Items.Add("");

            AccessibleObject accessibleObject = toolStrip.Items[0].AccessibilityObject;

            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.LastChild));
            Assert.False(toolStrip.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripItemAccessibleObject_FragmentNavigate_Sibling_ReturnExpected()
        {
            using ToolStrip toolStrip = new() { AutoSize = false, Size = new(300, 30) };

            toolStrip.Items.Add("");
            toolStrip.Items.Add("");

            toolStrip.PerformLayout();

            AccessibleObject grip = toolStrip.Grip.AccessibilityObject;
            AccessibleObject item1 = toolStrip.Items[0].AccessibilityObject;
            AccessibleObject item2 = toolStrip.Items[1].AccessibilityObject;

            Assert.Equal(item1, grip.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Equal(item2, item1.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Null(item2.FragmentNavigate(NavigateDirection.NextSibling));

            Assert.Equal(item1, item2.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Equal(grip, item1.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Null(grip.FragmentNavigate(NavigateDirection.PreviousSibling));

            Assert.False(toolStrip.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripItemAccessibleObject_FragmentNavigate_PreviousSibling_ReturnExpected_IfGripHidden()
        {
            using ToolStrip toolStrip = new() { GripStyle = ToolStripGripStyle.Hidden };

            toolStrip.Items.Add("");

            toolStrip.PerformLayout();

            AccessibleObject accessibleObject = toolStrip.Items[0].AccessibilityObject;

            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.PreviousSibling));

            Assert.False(toolStrip.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripItemAccessibleObject_FragmentNavigate_Sibling_ReturnExpected_IfItemsAligned()
        {
            using ToolStrip toolStrip = new() { AutoSize = false, Size = new(300, 30) };

            toolStrip.Items.Add("");
            toolStrip.Items.Add("");

            toolStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

            toolStrip.PerformLayout();

            AccessibleObject grip = toolStrip.Grip.AccessibilityObject;
            AccessibleObject item1 = toolStrip.Items[0].AccessibilityObject;
            AccessibleObject item2 = toolStrip.Items[1].AccessibilityObject;

            Assert.Equal(item2, grip.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Equal(item1, item2.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Null(item1.FragmentNavigate(NavigateDirection.NextSibling));

            Assert.Equal(item2, item1.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Equal(grip, item2.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Null(grip.FragmentNavigate(NavigateDirection.PreviousSibling));

            Assert.False(toolStrip.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripItemAccessibleObject_FragmentNavigate_Sibling_ReturnExpected_IfItemsSkipped()
        {
            using ToolStrip toolStrip = new() { AutoSize = false, Size = new(300, 30) };

            ToolStripItem CreateSkippedItem()
            {
                return new ToolStripControlHost(new Label());
            }

            toolStrip.Items.Add(CreateSkippedItem());
            toolStrip.Items.Add("");
            toolStrip.Items.Add(CreateSkippedItem());
            toolStrip.Items.Add("");
            toolStrip.Items.Add(CreateSkippedItem());

            toolStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

            toolStrip.PerformLayout();

            AccessibleObject grip = toolStrip.Grip.AccessibilityObject;
            AccessibleObject item2 = toolStrip.Items[1].AccessibilityObject;
            AccessibleObject item4 = toolStrip.Items[3].AccessibilityObject;

            Assert.Equal(item2, grip.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Equal(item4, item2.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Null(item4.FragmentNavigate(NavigateDirection.NextSibling));

            Assert.Equal(item2, item4.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Equal(grip, item2.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Null(grip.FragmentNavigate(NavigateDirection.PreviousSibling));

            Assert.False(toolStrip.IsHandleCreated);
        }

        private class SubToolStripItem : ToolStripItem
        {
            public SubToolStripItem() : base()
            {
            }
        }
    }
}
