// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.UpDownBase;
using static System.Windows.Forms.UpDownBase.UpDownButtons;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class UpDownBase_UpDownButtons_UpDownButtonsAccessibleObject : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void UpDownButtonsAccessibleObject_Ctor_Default()
        {
            using UpDownBase upDownBase = new SubUpDownBase();
            using UpDownButtons upDownButtons = new UpDownButtons(upDownBase);
            UpDownButtonsAccessibleObject accessibleObject = new UpDownButtonsAccessibleObject(upDownButtons);

            Assert.Equal(upDownButtons, accessibleObject.Owner);
            Assert.False(upDownBase.IsHandleCreated);
            Assert.False(upDownButtons.IsHandleCreated);
        }

        [WinFormsFact]
        public void UpDownButtonsAccessibleObject_ControlType_IsSpinner_IfAccessibleRoleIsDefault()
        {
            using UpDownBase upDownBase = new SubUpDownBase();
            UpDownButtons upDownButtons = upDownBase.UpDownButtonsInternal;
            // AccessibleRole is not set = Default

            object actual = upDownButtons.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.SpinnerControlTypeId, actual);
            Assert.False(upDownBase.IsHandleCreated);
        }

        [WinFormsFact]
        public void UpDownButtonsAccessibleObject_Role_IsSpinButton_ByDefault()
        {
            using UpDownBase upDownBase = new SubUpDownBase();
            UpDownButtons upDownButtons = upDownBase.UpDownButtonsInternal;
            // AccessibleRole is not set = Default

            AccessibleRole actual = upDownButtons.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.SpinButton, actual);
            Assert.False(upDownBase.IsHandleCreated);
        }

        public static IEnumerable<object[]> UpDownButtonsAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(UpDownButtonsAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void UpDownButtonsAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using UpDownBase upDownBase = new SubUpDownBase();
            UpDownButtons upDownButtons = upDownBase.UpDownButtonsInternal;
            upDownButtons.AccessibleRole = role;

            object actual = upDownButtons.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(upDownBase.IsHandleCreated);
        }

        [WinFormsFact]
        public void UpDownButtonsAccessibleObject_GetPropertyValue_Name_ReturnsExpected()
        {
            const string name = "Test name";
            using SubUpDownBase upDownBase = new();
            UpDownButtons upDownButtons = upDownBase.UpDownButtonsInternal;
            upDownButtons.AccessibleName = name;

            object actual = upDownButtons.AccessibilityObject.GetPropertyValue(UiaCore.UIA.NamePropertyId);

            Assert.Equal(name, actual);
            Assert.False(upDownBase.IsHandleCreated);
        }

        [WinFormsFact]
        public void UpDownButtonsAccessibleObject_GetPropertyValue_RuntimeId_ReturnsExpected()
        {
            using SubUpDownBase upDownBase = new();
            UpDownButtons upDownButtons = upDownBase.UpDownButtonsInternal;

            object actual = upDownButtons.AccessibilityObject.GetPropertyValue(UiaCore.UIA.RuntimeIdPropertyId);

            Assert.Equal(upDownButtons.AccessibilityObject.RuntimeId, actual);
            Assert.False(upDownBase.IsHandleCreated);
        }

        [WinFormsFact]
        public void UpDownButtonsAccessibleObject_GetPropertyValue_BoundingRectangle_ReturnsExpected()
        {
            using SubUpDownBase upDownBase = new();
            UpDownButtons upDownButtons = upDownBase.UpDownButtonsInternal;
            object actual = upDownButtons.AccessibilityObject.GetPropertyValue(UiaCore.UIA.BoundingRectanglePropertyId);

            Assert.Equal(upDownButtons.AccessibilityObject.BoundingRectangle, actual);
            Assert.False(upDownBase.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(false, ((int)UiaCore.UIA.IsExpandCollapsePatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsGridItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsGridPatternAvailablePropertyId))]
        [InlineData(true, ((int)UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsMultipleViewPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsScrollItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsScrollPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsSelectionItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsSelectionPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTableItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTablePatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTextPattern2AvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTextPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTogglePatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsValuePatternAvailablePropertyId))]
        public void UpDownButtonsAccessibleObject_GetPropertyValue_Pattern_ReturnsExpected(bool expected, int propertyId)
        {
            using TrackBar trackBar = new();
            using SubUpDownBase upDownBase = new();
            UpDownButtons upDownButtons = upDownBase.UpDownButtonsInternal;
            UpDownButtonsAccessibleObject accessibleObject = (UpDownButtonsAccessibleObject)upDownButtons.AccessibilityObject;

            Assert.Equal(expected, accessibleObject.GetPropertyValue((UiaCore.UIA)propertyId) ?? false);
            Assert.False(upDownBase.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.LegacyIAccessibleRolePropertyId, AccessibleRole.SpinButton)]
        [InlineData((int)UiaCore.UIA.LegacyIAccessibleStatePropertyId, AccessibleStates.None)]
        [InlineData((int)UiaCore.UIA.ValueValuePropertyId, null)]
        public void UpDownButtonsAccessibleObject_GetPropertyValue_ReturnsExpected(int property, object expected)
        {
            using TrackBar trackBar = new();
            using SubUpDownBase upDownBase = new();
            UpDownButtons upDownButtons = upDownBase.UpDownButtonsInternal;
            UpDownButtonsAccessibleObject accessibleObject = (UpDownButtonsAccessibleObject)upDownButtons.AccessibilityObject;
            object actual = accessibleObject.GetPropertyValue((UiaCore.UIA)property);

            Assert.Equal(expected, actual);
            Assert.False(upDownBase.IsHandleCreated);
        }

        private class SubUpDownBase : UpDownBase
        {
            protected override void UpdateEditText() => throw new NotImplementedException();

            public override void UpButton() => throw new NotImplementedException();

            public override void DownButton() => throw new NotImplementedException();
        }
    }
}
