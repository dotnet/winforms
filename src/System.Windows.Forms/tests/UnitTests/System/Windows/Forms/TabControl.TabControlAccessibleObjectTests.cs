// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class TabControl_TabControlAccessibilityObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void TabControlAccessibilityObject_Ctor_Default()
        {
            using TabControl tabControl = new TabControl();
            tabControl.CreateControl();

            Assert.NotNull(tabControl.AccessibilityObject);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibilityObject_ControlType_IsTabControl_IfAccessibleRoleIsDefault()
        {
            using TabControl tabControl = new TabControl();
            tabControl.CreateControl();
            // AccessibleRole is not set = Default

            object actual = tabControl.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.TabControlTypeId, actual);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibilityObject_Role_IsPageTabList_ByDefault()
        {
            using TabControl tabControl = new TabControl();
            tabControl.CreateControl();
            // AccessibleRole is not set = Default

            AccessibleRole actual = tabControl.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.PageTabList, actual);
            Assert.True(tabControl.IsHandleCreated);
        }
    }
}
