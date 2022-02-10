// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class RadioButton_RadioButtonAccessibleObjectTests
    {
        [WinFormsFact]
        public void RadioButtonAccessibleObject_Ctor_NullControl_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new RadioButton.RadioButtonAccessibleObject(null));
        }

        [WinFormsFact]
        public void RadioButtonAccessibleObject_Ctor_Default()
        {
            using var radioButton = new RadioButton();
            Assert.False(radioButton.IsHandleCreated);
            var radioButtonAccessibleObject = new RadioButton.RadioButtonAccessibleObject(radioButton);

            Assert.NotNull(radioButtonAccessibleObject.Owner);
            Assert.False(radioButton.IsHandleCreated);
        }

        [WinFormsFact]
        public void RadioButtonAccessibleObject_DefaultAction_ReturnsExpected()
        {
            using var radioButton = new RadioButton
            {
                AccessibleDefaultActionDescription = "TestActionDescription"
            };

            Assert.False(radioButton.IsHandleCreated);
            var radioButtonAccessibleObject = new RadioButton.RadioButtonAccessibleObject(radioButton);

            Assert.Equal("TestActionDescription", radioButtonAccessibleObject.DefaultAction);
            Assert.False(radioButton.IsHandleCreated);
        }

        [WinFormsFact]
        public void RadioButtonAccessibleObject_GetProperyValue_LegacyIAccessibleDefaultActionPropertyId_ReturnsExpected()
        {
            using var radioButton = new RadioButton
            {
                AccessibleDefaultActionDescription = "TestActionDescription"
            };

            var radioButtonAccessibleObject = new RadioButton.RadioButtonAccessibleObject(radioButton);

            Assert.Equal("TestActionDescription", radioButtonAccessibleObject.GetPropertyValue(UIA.LegacyIAccessibleDefaultActionPropertyId));
            Assert.False(radioButton.IsHandleCreated);
        }

        [WinFormsFact]
        public void RadioButtonAccessibleObject_Description_ReturnsExpected()
        {
            using var radioButton = new RadioButton
            {
                AccessibleDescription = "TestDescription"
            };

            Assert.False(radioButton.IsHandleCreated);
            var radioButtonAccessibleObject = new RadioButton.RadioButtonAccessibleObject(radioButton);

            Assert.Equal("TestDescription", radioButtonAccessibleObject.Description);
            Assert.False(radioButton.IsHandleCreated);
        }

        [WinFormsFact]
        public void RadioButtonAccessibleObject_Name_ReturnsExpected()
        {
            using var radioButton = new RadioButton
            {
                AccessibleName = "TestName"
            };

            Assert.False(radioButton.IsHandleCreated);
            var radioButtonAccessibleObject = new RadioButton.RadioButtonAccessibleObject(radioButton);

            Assert.Equal("TestName", radioButtonAccessibleObject.Name);
            Assert.False(radioButton.IsHandleCreated);
        }

        [WinFormsFact]
        public void RadioButtonAccessibleObject_Role_Custom_ReturnsExpected()
        {
            using var radioButton = new RadioButton
            {
                AccessibleRole = AccessibleRole.PushButton
            };

            Assert.False(radioButton.IsHandleCreated);
            var radioButtonAccessibleObject = new RadioButton.RadioButtonAccessibleObject(radioButton);

            Assert.Equal(AccessibleRole.PushButton, radioButtonAccessibleObject.Role);
            Assert.False(radioButton.IsHandleCreated);
        }

        [WinFormsFact]
        public void RadioButtonAccessibleObject_Role_Default_ReturnsExpected()
        {
            using var radioButton = new RadioButton();
            Assert.False(radioButton.IsHandleCreated);
            var radioButtonAccessibleObject = new RadioButton.RadioButtonAccessibleObject(radioButton);

            Assert.Equal(AccessibleRole.RadioButton, radioButtonAccessibleObject.Role);
            Assert.False(radioButton.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleStates.Focusable, AccessibleStates.Focusable | AccessibleStates.Checked)]
        [InlineData(false, AccessibleStates.None, AccessibleStates.None)]
        public void RadioButtonAccessibleObject_State_ReturnsExpected(bool createControl, AccessibleStates accessibleStatesFirstStage, AccessibleStates accessibleStatesSecondStage)
        {
            using var radioButton = new RadioButton();

            if (createControl)
            {
                radioButton.CreateControl();
            }

            var radioButtonAccessibleObject = new RadioButton.RadioButtonAccessibleObject(radioButton);
            Assert.Equal(accessibleStatesFirstStage, radioButtonAccessibleObject.State);

            radioButtonAccessibleObject.DoDefaultAction();

            Assert.Equal(accessibleStatesSecondStage, radioButtonAccessibleObject.State);
            Assert.Equal(createControl, radioButton.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void RadioButtonAccessibleObject_IsItemSelected_ReturnsExpected(bool createControl)
        {
            using var radioButton = new RadioButton();

            if (createControl)
            {
                radioButton.CreateControl();
            }

            var radioButtonAccessibleObject = new RadioButton.RadioButtonAccessibleObject(radioButton);
            Assert.False(radioButtonAccessibleObject.IsItemSelected);

            radioButtonAccessibleObject.DoDefaultAction();

            Assert.Equal(createControl, radioButtonAccessibleObject.IsItemSelected);
            Assert.Equal(createControl, radioButton.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UIA.NamePropertyId, "TestName")]
        [InlineData((int)UIA.ControlTypePropertyId, UIA.RadioButtonControlTypeId)] // If AccessibleRole is Default
        [InlineData((int)UIA.IsKeyboardFocusablePropertyId, true)]
        [InlineData((int)UIA.AutomationIdPropertyId, "RadioButton1")]
        public void RadioButtonAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
        {
            using var radioButton = new RadioButton
            {
                Name = "RadioButton1",
                AccessibleName = "TestName"
            };

            Assert.False(radioButton.IsHandleCreated);
            var radioButtonAccessibleObject = new RadioButton.RadioButtonAccessibleObject(radioButton);
            object value = radioButtonAccessibleObject.GetPropertyValue((UIA)propertyID);

            Assert.Equal(expected, value);
            Assert.False(radioButton.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UIA.LegacyIAccessiblePatternId)]
        [InlineData((int)UIA.SelectionItemPatternId)]
        public void RadioButtonAccessibleObject_IsPatternSupported_Invoke_ReturnsExpected(int patternId)
        {
            using var radioButton = new RadioButton
            {
                Name = "RadioButton1"
            };

            Assert.False(radioButton.IsHandleCreated);
            var radioButtonAccessibleObject = new RadioButton.RadioButtonAccessibleObject(radioButton);

            Assert.True(radioButtonAccessibleObject.IsPatternSupported((UIA)patternId));
            Assert.False(radioButton.IsHandleCreated);
        }

        public static IEnumerable<object[]> RadioButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(RadioButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void RadioButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using RadioButton radioButton = new RadioButton();
            radioButton.AccessibleRole = role;

            object actual = radioButton.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(radioButton.IsHandleCreated);
        }
    }
}
