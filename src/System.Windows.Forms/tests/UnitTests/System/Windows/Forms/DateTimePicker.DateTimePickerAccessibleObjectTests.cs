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
            using DateTimePicker dateTimePicker = new();

            DateTimePickerAccessibleObject accessibleObject = new DateTimePickerAccessibleObject(dateTimePicker);

            Assert.Equal(dateTimePicker, accessibleObject.Owner);
            Assert.False(dateTimePicker.IsHandleCreated);
        }

        [WinFormsFact]
        public void DateTimePickerAccessibleObject_ControlType_IsComboBox_IfAccessibleRoleIsDefault()
        {
            using DateTimePicker dateTimePicker = new();
            // AccessibleRole is not set = Default

            object actual = dateTimePicker.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ComboBoxControlTypeId, actual);
            Assert.False(dateTimePicker.IsHandleCreated);
        }

        [WinFormsFact]
        public void DateTimePickerAccessibleObject_Role_IsComboBox_ByDefault()
        {
            using DateTimePicker dateTimePicker = new();
            // AccessibleRole is not set = Default

            AccessibleRole actual = dateTimePicker.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.ComboBox, actual);
            Assert.False(dateTimePicker.IsHandleCreated);
        }

        [WinFormsFact]
        public void DateTimePickerAccessibleObject_LocalizedControlType_ReturnsExpected_IfAccessibleRoleIsDefault()
        {
            using DateTimePicker dateTimePicker = new();
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
            using DateTimePicker dateTimePicker = new();
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
            using DateTimePicker dateTimePicker = new();
            dateTimePicker.AccessibleRole = role;

            object actual = dateTimePicker.AccessibilityObject.GetPropertyValue(UiaCore.UIA.LocalizedControlTypePropertyId);

            Assert.Null(actual);
            Assert.False(dateTimePicker.IsHandleCreated);
        }

        [WinFormsFact]
        public void DateTimePickerAccessibleObject_GetPropertyValue_ReturnsExpected()
        {
            using DateTimePicker dateTimePicker = new();
            DateTime dt = new DateTime(2000, 1, 1);
            dateTimePicker.TestAccessor().Dynamic._text = dt.ToLongDateString();
            AccessibleObject accessibleObject = dateTimePicker.AccessibilityObject;

            Assert.Equal(dt.ToLongDateString(), accessibleObject.GetPropertyValue(UiaCore.UIA.ValueValuePropertyId));
            Assert.True((bool)accessibleObject.GetPropertyValue(UiaCore.UIA.IsExpandCollapsePatternAvailablePropertyId));
            Assert.False(dateTimePicker.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.ExpandCollapseState.Expanded)]
        [InlineData((int)UiaCore.ExpandCollapseState.Collapsed)]
        public void DateTimePickerAccessibleObject_ExpandCollapseState_ReturnsExpected(int expandCollapseState)
        {
            using DateTimePicker dateTimePicker = new();

            var expected = (UiaCore.ExpandCollapseState)expandCollapseState;
            var accessibleObject = (DateTimePickerAccessibleObject)dateTimePicker.AccessibilityObject;
            dateTimePicker.TestAccessor().Dynamic._expandCollapseState = expected;

            UiaCore.ExpandCollapseState actual = accessibleObject.ExpandCollapseState;

            Assert.Equal(expected, actual);
            Assert.False(dateTimePicker.IsHandleCreated);
        }

        [WinFormsFact]
        public void DateTimePickerAccessibleObject_Expand_IfHandleIsNotCreated_NothingChanges()
        {
            using DateTimePicker dateTimePicker = new();

            var accessibleObject = (DateTimePickerAccessibleObject)dateTimePicker.AccessibilityObject;

            // ExpandCollapseState is Collapsed before some actions
            Assert.Equal(UiaCore.ExpandCollapseState.Collapsed, accessibleObject.ExpandCollapseState);

            accessibleObject.Expand();

            Assert.Equal(UiaCore.ExpandCollapseState.Collapsed, accessibleObject.ExpandCollapseState);
            Assert.False(dateTimePicker.IsHandleCreated);
        }

        [WinFormsFact]
        public void DateTimePickerAccessibleObject_Collapse_IfHandleIsNotCreated_NothingChanges()
        {
            using DateTimePicker dateTimePicker = new();

            var accessibleObject = (DateTimePickerAccessibleObject)dateTimePicker.AccessibilityObject;

            // ExpandCollapseState is Collapsed before some actions
            Assert.Equal(UiaCore.ExpandCollapseState.Collapsed, accessibleObject.ExpandCollapseState);

            dateTimePicker.TestAccessor().Dynamic._expandCollapseState = UiaCore.ExpandCollapseState.Expanded;

            Assert.Equal(UiaCore.ExpandCollapseState.Expanded, accessibleObject.ExpandCollapseState);

            accessibleObject.Collapse();

            Assert.Equal(UiaCore.ExpandCollapseState.Expanded, accessibleObject.ExpandCollapseState);
            Assert.False(dateTimePicker.IsHandleCreated);
        }

        [WinFormsFact]
        public void DateTimePickerAccessibleObject_Expand_IfControlAlreadyIsExpanded_NothingChanges()
        {
            using DateTimePicker dateTimePicker = new();

            dateTimePicker.CreateControl();
            var accessibleObject = (DateTimePickerAccessibleObject)dateTimePicker.AccessibilityObject;

            // ExpandCollapseState is Collapsed before some actions
            Assert.Equal(UiaCore.ExpandCollapseState.Collapsed, accessibleObject.ExpandCollapseState);

            dateTimePicker.TestAccessor().Dynamic._expandCollapseState = UiaCore.ExpandCollapseState.Expanded;

            Assert.Equal(UiaCore.ExpandCollapseState.Expanded, accessibleObject.ExpandCollapseState);

            accessibleObject.Expand();

            Assert.Equal(UiaCore.ExpandCollapseState.Expanded, accessibleObject.ExpandCollapseState);
            Assert.True(dateTimePicker.IsHandleCreated);
        }

        [WinFormsFact]
        public void DateTimePickerAccessibleObject_Collapse_IfControlAlreadyIsCollapsed_NothingChanges()
        {
            using DateTimePicker dateTimePicker = new();

            dateTimePicker.CreateControl();
            var accessibleObject = (DateTimePickerAccessibleObject)dateTimePicker.AccessibilityObject;

            // ExpandCollapseState is Collapsed before some actions
            Assert.Equal(UiaCore.ExpandCollapseState.Collapsed, accessibleObject.ExpandCollapseState);

            accessibleObject.Collapse();

            Assert.Equal(UiaCore.ExpandCollapseState.Collapsed, accessibleObject.ExpandCollapseState);
            Assert.True(dateTimePicker.IsHandleCreated);
        }

        [WinFormsFact]
        public void DateTimePickerAccessibleObject_Expand_IfHandleIsCreated_ReturnsExpected()
        {
            using DateTimePicker dateTimePicker = new();

            dateTimePicker.CreateControl();
            var accessibleObject = (DateTimePickerAccessibleObject)dateTimePicker.AccessibilityObject;

            // ExpandCollapseState is Collapsed before some actions
            Assert.Equal(UiaCore.ExpandCollapseState.Collapsed, accessibleObject.ExpandCollapseState);

            accessibleObject.Expand();

            Assert.Equal(UiaCore.ExpandCollapseState.Expanded, accessibleObject.ExpandCollapseState);
            Assert.True(dateTimePicker.IsHandleCreated);
        }

        [WinFormsFact]
        public void DateTimePickerAccessibleObject_Collapse_IfHandleIsCreated_ReturnsExpected()
        {
            using DateTimePicker dateTimePicker = new();

            dateTimePicker.CreateControl();
            var accessibleObject = (DateTimePickerAccessibleObject)dateTimePicker.AccessibilityObject;

            Assert.Equal(UiaCore.ExpandCollapseState.Collapsed, accessibleObject.ExpandCollapseState);

            // If don't call Expand() on this control and just change state value instead
            // then call Collapse() does't work correctly due to the control is not expanded factually
            accessibleObject.Expand();

            Assert.Equal(UiaCore.ExpandCollapseState.Expanded, accessibleObject.ExpandCollapseState);

            accessibleObject.Collapse();

            Assert.Equal(UiaCore.ExpandCollapseState.Collapsed, accessibleObject.ExpandCollapseState);
            Assert.True(dateTimePicker.IsHandleCreated);
        }
    }
}
