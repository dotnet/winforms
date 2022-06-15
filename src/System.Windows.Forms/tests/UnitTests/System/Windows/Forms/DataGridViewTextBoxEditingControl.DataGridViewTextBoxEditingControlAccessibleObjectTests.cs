// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.DataGridViewTextBoxEditingControl;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewTextBoxEditingControl_DataGridViewTextBoxEditingControlAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void DataGridViewTextBoxEditingControlAccessibleObject_Ctor_Default()
        {
            using DataGridViewTextBoxEditingControl textCellControl = new DataGridViewTextBoxEditingControl();
            DataGridViewTextBoxEditingControlAccessibleObject accessibleObject = new DataGridViewTextBoxEditingControlAccessibleObject(textCellControl);
            Assert.Equal(textCellControl, accessibleObject.Owner);
        }

        [WinFormsFact]
        public void DataGridViewTextBoxEditingControlAccessibleObject_Ctor_ThrowsException_IfOwnerIsNull()
        {
            using DataGridViewTextBoxEditingControl textCellControl = new DataGridViewTextBoxEditingControl();
            Assert.Throws<ArgumentNullException>(() => new DataGridViewTextBoxEditingControlAccessibleObject(null));
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void DataGridViewTextBoxEditingControlAccessibleObject_IsReadOnly_IsExpected(bool readOnly)
        {
            using DataGridViewTextBoxEditingControl textCellControl = new DataGridViewTextBoxEditingControl();
            textCellControl.ReadOnly = readOnly;
            AccessibleObject accessibleObject = textCellControl.AccessibilityObject;
            Assert.Equal(readOnly, accessibleObject.IsReadOnly);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.IsTextPatternAvailablePropertyId)]
        [InlineData((int)UiaCore.UIA.IsTextPattern2AvailablePropertyId)]
        [InlineData((int)UiaCore.UIA.IsValuePatternAvailablePropertyId)]
        public void DataGridViewTextBoxEditingControlAccessibleObject_GetPropertyValue_PatternsSuported(int propertyID)
        {
            using DataGridViewTextBoxEditingControl textCellControl = new DataGridViewTextBoxEditingControl();
            AccessibleObject accessibleObject = textCellControl.AccessibilityObject;
            Assert.True((bool)accessibleObject.GetPropertyValue((UiaCore.UIA)propertyID));
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.ValuePatternId)]
        [InlineData((int)UiaCore.UIA.TextPatternId)]
        [InlineData((int)UiaCore.UIA.TextPattern2Id)]
        public void DataGridViewTextBoxEditingControlAccessibleObject_IsPatternSupported_PatternsSuported(int patternId)
        {
            using DataGridViewTextBoxEditingControl textCellControl = new DataGridViewTextBoxEditingControl();
            AccessibleObject accessibleObject = textCellControl.AccessibilityObject;
            Assert.True(accessibleObject.IsPatternSupported((UiaCore.UIA)patternId));
        }

        [WinFormsFact]
        public void DataGridViewTextBoxEditingControlAccessibleObject_ControlType_IsEdit_IfAccessibleRoleIsDefault()
        {
            using DataGridViewTextBoxEditingControl textCellControl = new DataGridViewTextBoxEditingControl();
            // AccessibleRole is not set = Default

            object actual = textCellControl.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.EditControlTypeId, actual);
            Assert.False(textCellControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Text)]
        [InlineData(false, AccessibleRole.None)]
        public void DataGridViewTextBoxEditingControlAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
        {
            using DataGridViewTextBoxEditingControl textCellControl = new DataGridViewTextBoxEditingControl();
            // AccessibleRole is not set = Default

            if (createControl)
            {
                textCellControl.CreateControl();
            }

            object actual = textCellControl.AccessibilityObject.Role;

            Assert.Equal(expectedRole, actual);
            Assert.Equal(createControl, textCellControl.IsHandleCreated);
        }

        public static IEnumerable<object[]> DataGridViewTextBoxEditingControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(DataGridViewTextBoxEditingControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void DataGridViewTextBoxEditingControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using DataGridViewTextBoxEditingControl textCellControl = new DataGridViewTextBoxEditingControl();
            textCellControl.AccessibleRole = role;

            object actual = textCellControl.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(textCellControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.NavigateDirection.NextSibling)]
        [InlineData((int)UiaCore.NavigateDirection.PreviousSibling)]
        [InlineData((int)UiaCore.NavigateDirection.FirstChild)]
        [InlineData((int)UiaCore.NavigateDirection.LastChild)]
        public void DataGridViewTextBoxEditingControlAccessibleObject_FragmentNavigate_SiblingsAndChildrenAreNull(int direction)
        {
            using DataGridViewTextBoxEditingControl control = new();

            object actual = control.AccessibilityObject.FragmentNavigate((UiaCore.NavigateDirection)direction);

            Assert.Null(actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewTextBoxEditingControlAccessibleObject_FragmentNavigate_ParentIsCell()
        {
            using DataGridView control = new();

            Assert.True(control.AccessibilityObject is Control.ControlAccessibleObject);

            control.Columns.Add(new DataGridViewTextBoxColumn());
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
