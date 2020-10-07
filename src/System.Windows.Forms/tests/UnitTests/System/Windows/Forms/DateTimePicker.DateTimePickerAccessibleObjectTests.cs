// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.DateTimePicker;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class DateTimePicker_DateTimePickerAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void DateTimePickerAccessibleObject_Ctor_Default()
        {
            using DateTimePicker dateTimePicker = new DateTimePicker();

            DateTimePickerAccessibleObject accessibleObject = new DateTimePickerAccessibleObject(dateTimePicker);

            Assert.Equal(dateTimePicker, accessibleObject.Owner);
            Assert.False(dateTimePicker.IsHandleCreated);
        }

        [WinFormsFact]
        public void DateTimePickerAccessibleObject_ControlType_IsComboBox_IfAccessibleRoleIsDefault()
        {
            using DateTimePicker dateTimePicker = new DateTimePicker();
            // AccessibleRole is not set = Default

            object actual = dateTimePicker.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            Assert.Null(actual);
            Assert.False(dateTimePicker.IsHandleCreated);
        }

        [WinFormsFact]
        public void DateTimePickerAccessibleObject_Role_IsComboBox_ByDefault()
        {
            using DateTimePicker dateTimePicker = new DateTimePicker();
            // AccessibleRole is not set = Default

            AccessibleRole actual = dateTimePicker.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.ComboBox, actual);
            Assert.False(dateTimePicker.IsHandleCreated);
        }

        [WinFormsFact]
        public void DateTimePickerAccessibleObject_LocalizedControlType_ReturnsExpected_IfAccessibleRoleIsDefault()
        {
            using DateTimePicker dateTimePicker = new DateTimePicker();
            // AccessibleRole is not set = Default

            object actual = dateTimePicker.AccessibilityObject.GetPropertyValue(UiaCore.UIA.LocalizedControlTypePropertyId);
            string expected = SR.DateTimePickerLocalizedControlType;

            Assert.Equal(expected, actual);
            Assert.False(dateTimePicker.IsHandleCreated);
        }
    }
}
