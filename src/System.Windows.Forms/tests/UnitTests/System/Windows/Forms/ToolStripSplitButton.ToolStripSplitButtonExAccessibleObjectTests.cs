// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ToolStripSplitButton;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripSplitButton_ToolStripSplitButtonExAccessibleObjectTests
    {
        [WinFormsFact]
        public void ToolStripSplitButtonExAccessibleObject_Ctor_OwnerToolStripSplitButtonCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ToolStripSplitButtonExAccessibleObject(null));
        }

        [WinFormsFact]
        public void ToolStripSplitButtonExAccessibleObject_ControlType_ReturnsExpected()
        {
            using ToolStripSplitButton toolStripSplitButton = new();

            ToolStripSplitButtonExAccessibleObject accessibleObject = new(toolStripSplitButton);

            Assert.Equal(UiaCore.UIA.ButtonControlTypeId, accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
        }

        [WinFormsFact]
        public void ToolStripSplitButtonExAccessibleObject_IsIAccessibleExSupported_ReturnsExpected()
        {
            using ToolStripSplitButton toolStripSplitButton = new();

            ToolStripSplitButtonExAccessibleObject accessibleObject = new(toolStripSplitButton);

            Assert.True(accessibleObject.IsIAccessibleExSupported());
        }

        [WinFormsFact]
        public void ToolStripSplitButtonExAccessibleObject_DropDownItemsCount_ReturnsExpected_IfDropDownCollapsed()
        {
            using ToolStripSplitButton toolStripSplitButton = new();

            ToolStripSplitButtonExAccessibleObject accessibleObject = new(toolStripSplitButton);

            Assert.Equal(UiaCore.ExpandCollapseState.Collapsed, accessibleObject.ExpandCollapseState);
            Assert.Equal(0, accessibleObject.TestAccessor().Dynamic.DropDownItemsCount);
        }

        [WinFormsFact]
        public void ToolStripSplitButtonExAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
        {
            using ToolStripSplitButton toolStripSplitButton = new();

            using ToolStrip toolStrip = new();
            toolStrip.Items.Add(toolStripSplitButton);
            toolStrip.PerformLayout();
            toolStrip.CreateControl();

            ToolStripSplitButtonExAccessibleObject accessibleObject = new(toolStripSplitButton);
            AccessibleObject expected = toolStrip.AccessibilityObject;

            Assert.Equal(expected, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
        }

        [WinFormsFact]
        public void ToolStripSplitButtonExAccessibleObject_FragmentNavigate_Parent_ReturnsNull_IfHandleNotCreated()
        {
            using ToolStripSplitButton toolStripSplitButton = new();

            using ToolStrip toolStrip = new();
            toolStrip.Items.Add(toolStripSplitButton);

            ToolStripSplitButtonExAccessibleObject accessibleObject = new(toolStripSplitButton);

            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.False(toolStrip.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripSplitButtonExAccessibleObject_FragmentNavigate_Child_ReturnsExpected()
        {
            using ToolStripSplitButton toolStripSplitButton = new();

            ToolStripItem item1 = toolStripSplitButton.DropDownItems.Add(string.Empty);
            ToolStripItem item2 = toolStripSplitButton.DropDownItems.Add(string.Empty);

            ToolStripSplitButtonExAccessibleObject accessibleObject = new(toolStripSplitButton);

            toolStripSplitButton.DropDown.Show();

            Assert.Equal(item1.AccessibilityObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Equal(item2.AccessibilityObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
        }

        [WinFormsFact]
        public void ToolStripSplitButtonExAccessibleObject_FragmentNavigate_Child_ReturnsNull_IfDropDownNotOpened()
        {
            using ToolStripSplitButton toolStripSplitButton = new();

            toolStripSplitButton.DropDownItems.Add(string.Empty);

            ToolStripSplitButtonExAccessibleObject accessibleObject = new(toolStripSplitButton);

            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
        }

        [WinFormsFact]
        public void ToolStripSplitButtonExAccessibleObject_FragmentNavigate_Child_ReturnsNull_IfNoDropDownItems()
        {
            using ToolStripSplitButton toolStripSplitButton = new();

            ToolStripSplitButtonExAccessibleObject accessibleObject = new(toolStripSplitButton);

            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
        }

        [WinFormsFact]
        public void ToolStripSplitButtonExAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfItemsAligned()
        {
            using ToolStripSplitButton toolStripSplitButton = new();

            ToolStripItem item1 = toolStripSplitButton.DropDownItems.Add(string.Empty);
            ToolStripItem item2 = toolStripSplitButton.DropDownItems.Add(string.Empty);

            item1.Alignment = ToolStripItemAlignment.Right;

            ToolStripSplitButtonExAccessibleObject accessibleObject = new(toolStripSplitButton);

            toolStripSplitButton.DropDown.Show();

            Assert.Equal(item1.AccessibilityObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Equal(item2.AccessibilityObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
        }
    }
}
