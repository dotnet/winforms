﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class ListBoxAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ListBoxAccessibleObjectTests_Ctor_Default()
        {
            using ListBox listBox = new ListBox();
            listBox.Items.AddRange(new object[] {
                "a",
                "b",
                "c",
                "d",
                "e",
                "f",
                "g"
            });

            var childCount = listBox.AccessibilityObject.GetChildCount();

            for (int i = 0; i < childCount; i++)
            {
                var child = listBox.AccessibilityObject.GetChild(i);
                Assert.True(child.IsPatternSupported(UiaCore.UIA.ScrollItemPatternId));
            }
        }

        [WinFormsFact]
        public void ListBoxAccessibleObject_ControlType_IsList_IfAccessibleRoleIsDefault()
        {
            using ListBox listBox = new ListBox();
            // AccessibleRole is not set = Default

            object actual = listBox.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ListControlTypeId, actual);
            Assert.False(listBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.List)]
        [InlineData(false, AccessibleRole.None)]
        public void ListBoxAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
        {
            using ListBox listBox = new ListBox();
            // AccessibleRole is not set = Default

            if (createControl)
            {
                listBox.CreateControl();
            }

            AccessibleRole actual = listBox.AccessibilityObject.Role;

            Assert.Equal(expectedRole, actual);
            Assert.Equal(createControl, listBox.IsHandleCreated);
        }

        public static IEnumerable<object[]> ListBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(ListBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void ListBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using ListBox listBox = new ListBox();
            listBox.AccessibleRole = role;

            object actual = listBox.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(listBox.IsHandleCreated);
        }
    }
}
