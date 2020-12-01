﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class MaskedTextBoxAccessibilityObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void MaskedTextBoxAccessibilityObject_Ctor_Default()
        {
            using MaskedTextBox maskedTextBox = new MaskedTextBox();
            maskedTextBox.CreateControl();

            Assert.NotNull(maskedTextBox.AccessibilityObject);
            Assert.True(maskedTextBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void MaskedTextBoxAccessibilityObject_ControlType_IsEdit_IfAccessibleRoleIsDefault()
        {
            using MaskedTextBox maskedTextBox = new MaskedTextBox();
            maskedTextBox.CreateControl();
            // AccessibleRole is not set = Default

            object actual = maskedTextBox.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.EditControlTypeId, actual);
            Assert.True(maskedTextBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void MaskedTextBoxAccessibilityObject_Role_IsText_ByDefault()
        {
            using MaskedTextBox maskedTextBox = new MaskedTextBox();
            maskedTextBox.CreateControl();
            // AccessibleRole is not set = Default

            AccessibleRole actual = maskedTextBox.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.Text, actual);
            Assert.True(maskedTextBox.IsHandleCreated);
        }

        public static IEnumerable<object[]> MaskedTextBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(MaskedTextBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void MaskedTextBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using MaskedTextBox maskedTextBox = new MaskedTextBox();
            maskedTextBox.AccessibleRole = role;

            object actual = maskedTextBox.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(maskedTextBox.IsHandleCreated);
        }
    }
}
