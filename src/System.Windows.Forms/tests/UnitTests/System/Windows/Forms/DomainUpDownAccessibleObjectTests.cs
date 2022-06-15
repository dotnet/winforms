// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class DomainUpDownAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void DomainUpDownAccessibleObject_Ctor_Default()
        {
            using DomainUpDown domainUpDown = new DomainUpDown();
            AccessibleObject accessibleObject = domainUpDown.AccessibilityObject;
            Assert.NotNull(accessibleObject);
            Assert.False(domainUpDown.IsHandleCreated);
        }

        [WinFormsFact]
        public void DomainUpDownAccessibleObject_ControlType_IsSpinner_IfAccessibleRoleIsDefault()
        {
            using DomainUpDown domainUpDown = new DomainUpDown();
            // AccessibleRole is not set = Default

            object actual = domainUpDown.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.SpinnerControlTypeId, actual);
            Assert.False(domainUpDown.IsHandleCreated);
        }

        [WinFormsFact]
        public void DomainUpDownAccessibleObject_Role_IsSpinButton_ByDefault()
        {
            using DomainUpDown domainUpDown = new DomainUpDown();
            // AccessibleRole is not set = Default

            AccessibleRole actual = domainUpDown.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.SpinButton, actual);
            Assert.False(domainUpDown.IsHandleCreated);
        }

        [WinFormsFact]
        public void DomainUpDownAccessibleObject_GetPropertyValue_IsKeyboardFocusable_ReturnsTrue()
        {
            using DomainUpDown domainUpDown = new DomainUpDown();
            AccessibleObject accessibleObject = domainUpDown.AccessibilityObject;

            bool isKeyboardFocusable = (bool)accessibleObject.GetPropertyValue(Interop.UiaCore.UIA.IsKeyboardFocusablePropertyId);
            Assert.True(isKeyboardFocusable);
        }

        [WinFormsFact]
        public void DomainUpDownAccessibleObject_GetPropertyValue_IsKeyboardFocusable_WhenDisabled_ReturnsFalse()
        {
            using DomainUpDown domainUpDown = new DomainUpDown();
            AccessibleObject accessibleObject = domainUpDown.AccessibilityObject;

            domainUpDown.Enabled = false;

            bool isKeyboardFocusable = (bool)accessibleObject.GetPropertyValue(Interop.UiaCore.UIA.IsKeyboardFocusablePropertyId);
            Assert.False(isKeyboardFocusable);
        }

        public static IEnumerable<object[]> DomainUpDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(DomainUpDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void DomainUpDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using DomainUpDown domainUpDown = new DomainUpDown();
            domainUpDown.AccessibleRole = role;

            object actual = domainUpDown.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(domainUpDown.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.LegacyIAccessibleRolePropertyId, AccessibleRole.SpinButton)]
        [InlineData((int)UiaCore.UIA.LegacyIAccessibleStatePropertyId, AccessibleStates.None)]
        [InlineData((int)UiaCore.UIA.ValueValuePropertyId, null)]
        public void DomainUpDownAccessibleObject_GetPropertyValue_ReturnsExpected(int property, object expected)
        {
            using DomainUpDown domainUpDown = new DomainUpDown();
            AccessibleObject accessibleObject = domainUpDown.AccessibilityObject;
            object actual = accessibleObject.GetPropertyValue((UiaCore.UIA)property);

            Assert.Equal(expected, actual);
            Assert.False(domainUpDown.IsHandleCreated);
        }
    }
}
