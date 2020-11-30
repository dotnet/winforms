// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
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

        public static IEnumerable<object[]> TabControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(TabControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void TabControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using TabControl tabControl = new TabControl();
            tabControl.AccessibleRole = role;

            object actual = tabControl.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(tabControl.IsHandleCreated);
        }
    }
}
