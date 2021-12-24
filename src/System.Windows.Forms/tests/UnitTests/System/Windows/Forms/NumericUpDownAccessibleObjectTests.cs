// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class NumericUpDownAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void NumericUpDownAccessibleObject_Ctor_Default()
        {
            using NumericUpDown numericUpDown = new NumericUpDown();
            AccessibleObject accessibleObject = numericUpDown.AccessibilityObject;
            Assert.NotNull(accessibleObject);
        }

        [WinFormsFact]
        public void NumericUpDownAccessibleObject_GetPropertyValue_IsKeyboardFocusable_ReturnsTrue()
        {
            using NumericUpDown numericUpDown = new NumericUpDown();
            AccessibleObject accessibleObject = numericUpDown.AccessibilityObject;

            bool isKeyboardFocusable = (bool)accessibleObject.GetPropertyValue(Interop.UiaCore.UIA.IsKeyboardFocusablePropertyId);
            Assert.True(isKeyboardFocusable);
        }

        [WinFormsFact]
        public void NumericUpDownAccessibleObject_GetPropertyValue_IsKeyboardFocusable_WhenDisabled_ReturnsFalse()
        {
            using NumericUpDown numericUpDown = new NumericUpDown();
            AccessibleObject accessibleObject = numericUpDown.AccessibilityObject;

            numericUpDown.Enabled = false;

            bool isKeyboardFocusable = (bool)accessibleObject.GetPropertyValue(Interop.UiaCore.UIA.IsKeyboardFocusablePropertyId);
            Assert.False(isKeyboardFocusable);
        }

        [WinFormsFact]
        public void NumericUpDownAccessibleObject_ControlType_IsSpinner_IfAccessibleRoleIsDefault()
        {
            using NumericUpDown numericUpDown = new NumericUpDown();
            // AccessibleRole is not set = Default

            object actual = numericUpDown.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.SpinnerControlTypeId, actual);
            Assert.False(numericUpDown.IsHandleCreated);
        }

        [WinFormsFact]
        public void NumericUpDownAccessibleObject_Role_IsSpinButton_ByDefault()
        {
            using NumericUpDown numericUpDown = new NumericUpDown();
            // AccessibleRole is not set = Default

            AccessibleRole actual = numericUpDown.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.SpinButton, actual);
            Assert.False(numericUpDown.IsHandleCreated);
        }

        public static IEnumerable<object[]> NumericUpDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(NumericUpDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void NumericUpDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using NumericUpDown numericUpDown = new NumericUpDown();
            numericUpDown.AccessibleRole = role;

            object actual = numericUpDown.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(numericUpDown.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.LegacyIAccessibleRolePropertyId, AccessibleRole.SpinButton)]
        [InlineData((int)UiaCore.UIA.LegacyIAccessibleStatePropertyId, AccessibleStates.None)]
        public void NumericUpDownAccessibleObject_GetPropertyValue_ReturnsExpected(int property, object expected)
        {
            using NumericUpDown numericUpDown = new NumericUpDown();
            AccessibleObject accessibleObject = numericUpDown.AccessibilityObject;
            object actual = accessibleObject.GetPropertyValue((UiaCore.UIA)property);

            Assert.Equal(expected, actual);
            Assert.False(numericUpDown.IsHandleCreated);
        }
    }
}
