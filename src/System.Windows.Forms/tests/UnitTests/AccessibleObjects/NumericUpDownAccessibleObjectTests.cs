// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests.AccessibleObjects
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
    }
}
