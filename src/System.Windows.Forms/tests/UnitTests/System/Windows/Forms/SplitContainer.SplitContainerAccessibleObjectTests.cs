// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class SplitContainer_SplitContainerAccessibilityObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void SplitContainerAccessibilityObject_Ctor_Default()
        {
            using SplitContainer splitContainer = new SplitContainer();
            splitContainer.CreateControl();

            Assert.NotNull(splitContainer.AccessibilityObject);
            Assert.True(splitContainer.IsHandleCreated);
        }

        [WinFormsFact]
        public void SplitContainerAccessibilityObject_ControlType_IsPane_IfAccessibleRoleIsDefault()
        {
            using SplitContainer splitContainer = new SplitContainer();
            splitContainer.CreateControl();
            // AccessibleRole is not set = Default

            object actual = splitContainer.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.PaneControlTypeId, actual);
            Assert.True(splitContainer.IsHandleCreated);
        }

        [WinFormsFact]
        public void SplitContainerAccessibilityObject_Role_IsClient_ByDefault()
        {
            using SplitContainer splitContainer = new SplitContainer();
            splitContainer.CreateControl();
            // AccessibleRole is not set = Default

            AccessibleRole actual = splitContainer.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.Client, actual);
            Assert.True(splitContainer.IsHandleCreated);
        }

        public static IEnumerable<object[]> SplitContainerAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(SplitContainerAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void SplitContainerAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using SplitContainer splitContainer = new SplitContainer();
            splitContainer.AccessibleRole = role;

            object actual = splitContainer.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(splitContainer.IsHandleCreated);
        }
    }
}
