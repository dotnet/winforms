// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.DataGridViewComboBoxEditingControl;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewComboBoxEditingControlAccessibleObjectTests
    {
        [WinFormsFact]
        public void DataGridViewComboBoxEditingControlAccessibleObject_Ctor_Default()
        {
            using DataGridViewComboBoxEditingControl control = new DataGridViewComboBoxEditingControl();

            DataGridViewComboBoxEditingControlAccessibleObject accessibleObject =
                new DataGridViewComboBoxEditingControlAccessibleObject(control);

            Assert.Equal(control, accessibleObject.Owner);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.ComboBox)]
        [InlineData(false, AccessibleRole.None)]
        public void DataGridViewComboBoxEditingControlAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
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
        public void DataGridViewComboBoxEditingControlAccessibleObject_ControlType_IsComboBox_IfAccessibleRoleIsDefault()
        {
            using DataGridViewComboBoxEditingControl control = new DataGridViewComboBoxEditingControl();
            // AccessibleRole is not set = Default

            object actual = control.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ComboBoxControlTypeId, actual);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> DataGridViewComboBoxEditingControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(DataGridViewComboBoxEditingControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void DataGridViewComboBoxEditingControlAccessibleObjectTest_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using DataGridViewComboBoxEditingControl control = new DataGridViewComboBoxEditingControl();
            control.AccessibleRole = role;

            object actual = control.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.NavigateDirection.NextSibling)]
        [InlineData((int)UiaCore.NavigateDirection.PreviousSibling)]
        public void DataGridViewComboBoxEditingControlAccessibleObject_FragmentNavigate_SiblingsAreNull(int direction)
        {
            using DataGridViewComboBoxEditingControl control = new();

            object actual = control.AccessibilityObject.FragmentNavigate((UiaCore.NavigateDirection)direction);

            Assert.Null(actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewComboBoxEditingControlAccessibleObject_FragmentNavigate_ChildrenAreExpected()
        {
            using DataGridViewComboBoxEditingControl control = new();
            control.CreateControl();

            object firstChild = control.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
            object lastChild = control.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild);

            Assert.Equal(control.ChildEditAccessibleObject, firstChild);
            Assert.Equal(((DataGridViewComboBoxEditingControlAccessibleObject)control.AccessibilityObject).DropDownButtonUiaProvider, lastChild);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewComboBoxEditingControlAccessibleObject_FragmentNavigate_ParentIsCell()
        {
            using DataGridView control = new();

            Assert.True(control.AccessibilityObject is Control.ControlAccessibleObject);

            control.Columns.Add(new DataGridViewComboBoxColumn());
            control.Rows.Add();

            control.CreateControl();
            control.CurrentCell = control.Rows[0].Cells[0];
            control.BeginEdit(false);

            object actual = control.EditingControlAccessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent);

            control.EndEdit();

            Assert.Null(control.EditingControl);
            Assert.Equal(control.CurrentCell.AccessibilityObject, actual);
            Assert.True(control.IsHandleCreated);
        }
    }
}
