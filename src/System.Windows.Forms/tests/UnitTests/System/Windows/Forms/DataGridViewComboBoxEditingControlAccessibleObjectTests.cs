// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewComboBoxEditingControlAccessibleObjectTests
    {
        [WinFormsFact]
        public void DataGridViewComboBoxEditingControlAccessibleObjectTests_Ctor_Default()
        {
            using DataGridViewComboBoxEditingControl control = new DataGridViewComboBoxEditingControl();

            DataGridViewComboBoxEditingControlAccessibleObject accessibleObject = new DataGridViewComboBoxEditingControlAccessibleObject(control);

            Assert.Equal(control, accessibleObject.Owner);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.ComboBox)]
        [InlineData(false, AccessibleRole.None)]
        public void DataGridViewComboBoxEditingControlAccessibleObjectTests_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
        {
            using DataGridViewComboBoxEditingControl control = new DataGridViewComboBoxEditingControl();
            // AccessibleRole is not set = Default

            if (createControl)
            {
                control.CreateControl();
            }

            object actual = control.AccessibilityObject.Role;

            Assert.Equal(expectedRole, actual);
            Assert.Equal(createControl, control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewComboBoxEditingControlAccessibleObjectTests_ControlType_IsComboBox_IfAccessibleRoleIsDefault()
        {
            using DataGridViewComboBoxEditingControl control = new DataGridViewComboBoxEditingControl();
            // AccessibleRole is not set = Default

            object actual = control.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ComboBoxControlTypeId, actual);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> DataGridViewComboBoxEditingControlAccessibleObjectTest_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(DataGridViewComboBoxEditingControlAccessibleObjectTest_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void DataGridViewComboBoxEditingControlAccessibleObjectTest_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using DataGridViewComboBoxEditingControl control = new DataGridViewComboBoxEditingControl();
            control.AccessibleRole = role;

            object actual = control.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(control.IsHandleCreated);
        }
    }
}
