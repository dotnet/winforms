// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class TreeViewAccessibilityObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void TreeViewAccessibilityObject_Ctor_Default()
        {
            using TreeView treeView = new TreeView();
            treeView.CreateControl();

            Assert.NotNull(treeView.AccessibilityObject);
            Assert.True(treeView.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeViewAccessibilityObject_ControlType_IsTree_IfAccessibleRoleIsDefault()
        {
            using TreeView treeView = new TreeView();
            treeView.CreateControl();
            // AccessibleRole is not set = Default

            object actual = treeView.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.TreeControlTypeId, actual);
            Assert.True(treeView.IsHandleCreated);
        }

        [WinFormsFact]
        public void TreeViewAccessibilityObject_Role_IsOutline_ByDefault()
        {
            using TreeView treeView = new TreeView();
            treeView.CreateControl();
            // AccessibleRole is not set = Default

            AccessibleRole actual = treeView.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.Outline, actual);
            Assert.True(treeView.IsHandleCreated);
        }
    }
}
