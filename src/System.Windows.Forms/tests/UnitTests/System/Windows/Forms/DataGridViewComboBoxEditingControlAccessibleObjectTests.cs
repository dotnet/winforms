// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    }
}
