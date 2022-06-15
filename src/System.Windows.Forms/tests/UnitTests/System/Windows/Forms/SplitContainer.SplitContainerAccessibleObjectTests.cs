// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class SplitContainer_SplitContainerAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void SplitContainerAccessibleObject_Ctor_Default()
        {
            using SplitContainer splitContainer = new();
            SplitContainer.SplitContainerAccessibleObject accessibleObject = new(splitContainer);

            Assert.Equal(splitContainer, accessibleObject.Owner);
            Assert.False(splitContainer.IsHandleCreated);
        }

        [WinFormsFact]
        public void SplitContainerAccessibleObject_ControlType_IsPane_IfAccessibleRoleIsDefault()
        {
            using SplitContainer splitContainer = new();
            splitContainer.CreateControl();
            // AccessibleRole is not set = Default

            object actual = splitContainer.AccessibilityObject.GetPropertyValue(UIA.ControlTypePropertyId);

            Assert.Equal(UIA.PaneControlTypeId, actual);
            Assert.True(splitContainer.IsHandleCreated);
        }

        [WinFormsFact]
        public void SplitContainerAccessibleObject_Role_IsClient_ByDefault()
        {
            using SplitContainer splitContainer = new();
            splitContainer.CreateControl();
            // AccessibleRole is not set = Default

            AccessibleRole actual = splitContainer.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.Client, actual);
            Assert.True(splitContainer.IsHandleCreated);
        }

        public static IEnumerable<object[]> SplitContainerAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(SplitContainerAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void SplitContainerAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using SplitContainer splitContainer = new();
            splitContainer.AccessibleRole = role;

            object actual = splitContainer.AccessibilityObject.GetPropertyValue(UIA.ControlTypePropertyId);
            UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(splitContainer.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UIA.NamePropertyId, "TestName")]
        [InlineData((int)UIA.AutomationIdPropertyId, "SplitContainer1")]
        public void SplitContainerAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
        {
            using SplitContainer control = new()
            {
                Name = "SplitContainer1",
                AccessibleName = "TestName"
            };

            var accessibleObject = new SplitContainer.SplitContainerAccessibleObject(control);
            object value = accessibleObject.GetPropertyValue((UIA)propertyID);

            Assert.Equal(expected, value);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void SplitContainerAccessibleObject_GetPropertyValue_HasKeyboardFocus_ReturnsTrue_IfControlHasFocus()
        {
            using SplitContainer control = new();

            var accessibleObject = new SplitContainer.SplitContainerAccessibleObject(control);
            Assert.False(control.IsHandleCreated);
            control.FocusActiveControlInternal();
            bool value = (bool)accessibleObject.GetPropertyValue(UIA.HasKeyboardFocusPropertyId);

            Assert.True(value);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void SplitContainerAccessibleObject_GetPropertyValue_HasKeyboardFocus_ReturnsFalse_IfControlHasNoFocus()
        {
            using SplitContainer control = new();

            var accessibleObject = new SplitContainer.SplitContainerAccessibleObject(control);
            bool value = (bool)accessibleObject.GetPropertyValue(UIA.HasKeyboardFocusPropertyId);

            Assert.False(value);
            Assert.False(control.IsHandleCreated);
        }
    }
}
