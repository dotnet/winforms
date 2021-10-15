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
    }
}
