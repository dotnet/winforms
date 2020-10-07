// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class TabPage_TabPageAccessibilityObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void TabPageAccessibilityObject_Ctor_Default()
        {
            using TabPage tabPage = new TabPage();
            tabPage.CreateControl();

            Assert.NotNull(tabPage.AccessibilityObject);
            Assert.True(tabPage.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabPageAccessibilityObject_ControlType_IsPane_IfAccessibleRoleIsDefault()
        {
            using TabPage tabPage = new TabPage();
            tabPage.CreateControl();
            // AccessibleRole is not set = Default

            object actual = tabPage.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.PaneControlTypeId, actual);
            Assert.True(tabPage.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabPageAccessibilityObject_Role_IsClient_ByDefault()
        {
            using TabPage tabPage = new TabPage();
            tabPage.CreateControl();
            // AccessibleRole is not set = Default

            AccessibleRole actual = tabPage.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.Client, actual);
            Assert.True(tabPage.IsHandleCreated);
        }
    }
}
