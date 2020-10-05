// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
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

            Assert.Equal(UiaCore.UIA.ComboBoxControlTypeId, actual);
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

        public static IEnumerable<object[]> DateTimePickerAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(DateTimePickerAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void DateTimePickerAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using DateTimePicker dateTimePicker = new DateTimePicker();
            dateTimePicker.AccessibleRole = role;

            object actual = dateTimePicker.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(dateTimePicker.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(DateTimePickerAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void DateTimePickerAccessibleObject_GetPropertyValue_LocalizedControlType_IsNull_ForCustomRole(AccessibleRole role)
        {
            using DateTimePicker dateTimePicker = new DateTimePicker();
            dateTimePicker.AccessibleRole = role;

            object actual = dateTimePicker.AccessibilityObject.GetPropertyValue(UiaCore.UIA.LocalizedControlTypePropertyId);

            Assert.Null(actual);
            Assert.False(dateTimePicker.IsHandleCreated);
        }
    }
}
