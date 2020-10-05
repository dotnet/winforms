// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
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

        public static IEnumerable<object[]> TabPageAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(TabPageAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void TabPageAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using TabPage tabPage = new TabPage();
            tabPage.AccessibleRole = role;

            object actual = tabPage.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(tabPage.IsHandleCreated);
        }
    }
}
