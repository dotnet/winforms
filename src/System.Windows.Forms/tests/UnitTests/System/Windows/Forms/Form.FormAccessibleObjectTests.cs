// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using static System.Windows.Forms.Form;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class Form_FormAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void FormAccessibleObject_Ctor_Default()
        {
            using Form form = new Form();
            FormAccessibleObject accessibleObject = new FormAccessibleObject(form);

            Assert.Equal(form, accessibleObject.Owner);
            Assert.False(form.IsHandleCreated);
        }

        [WinFormsFact]
        public void FormAccessibleObject_ControlType_IsWindow_IfAccessibleRoleIsDefault()
        {
            using Form form = new Form();
            // AccessibleRole is not set = Default

            AccessibleObject accessibleObject = form.AccessibilityObject;
            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(AccessibleRole.Client, accessibleObject.Role);
            Assert.Equal(UiaCore.UIA.WindowControlTypeId, actual);
            Assert.False(form.IsHandleCreated);
        }

        public static IEnumerable<object[]> FormAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
        {
            Array roles = Enum.GetValues(typeof(AccessibleRole));

            foreach (AccessibleRole role in roles)
            {
                if (role == AccessibleRole.Default || role == AccessibleRole.Client)
                {
                    continue; // The test checks custom roles. "Client" is the default role and it has special handling.
                }

                yield return new object[] { role };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(FormAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void FormAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using Form form = new Form();
            form.AccessibleRole = role;

            object actual = form.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(form.IsHandleCreated);
        }

        [WinFormsFact]
        public void FormAccessibleObject_Role_IsClient_ByDefault()
        {
            using Form form = new Form();
            // AccessibleRole is not set = Default

            AccessibleRole actual = form.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.Client, actual);
            Assert.False(form.IsHandleCreated);
        }
    }
}
