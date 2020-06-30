// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
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
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(radioButton.IsHandleCreated);
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
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(radioButton.IsHandleCreated);
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
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(radioButton.IsHandleCreated);
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
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(radioButton.IsHandleCreated);
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
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(radioButton.IsHandleCreated);
        }

        [WinFormsFact]
        public void RadioButtonAccessibleObject_Role_Default_ReturnsExpected()
        {
            using var radioButton = new RadioButton();
            Assert.False(radioButton.IsHandleCreated);
            var radioButtonAccessibleObject = new RadioButton.RadioButtonAccessibleObject(radioButton);

            Assert.Equal(AccessibleRole.RadioButton, radioButtonAccessibleObject.Role);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(radioButton.IsHandleCreated);
        }

        [WinFormsFact]
        public void RadioButtonAccessibleObject_State_ReturnsExpected()
        {
            using var radioButton = new RadioButton();
            Assert.False(radioButton.IsHandleCreated);
            var radioButtonAccessibleObject = new RadioButton.RadioButtonAccessibleObject(radioButton);
            Assert.Equal(AccessibleStates.Focusable, radioButtonAccessibleObject.State);

            radioButtonAccessibleObject.DoDefaultAction();

            Assert.Equal(AccessibleStates.Focusable | AccessibleStates.Checked, radioButtonAccessibleObject.State);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(radioButton.IsHandleCreated);
        }

        [WinFormsFact]
        public void RadioButtonAccessibleObject_IsItemSelected_ReturnsExpected()
        {
            using var radioButton = new RadioButton();
            Assert.False(radioButton.IsHandleCreated);
            var radioButtonAccessibleObject = new RadioButton.RadioButtonAccessibleObject(radioButton);
            Assert.False(radioButtonAccessibleObject.IsItemSelected);

            radioButtonAccessibleObject.DoDefaultAction();

            Assert.True(radioButtonAccessibleObject.IsItemSelected);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(radioButton.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UIA.NamePropertyId, "TestName")]
        [InlineData((int)UIA.ControlTypePropertyId, UIA.RadioButtonControlTypeId)]
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
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(radioButton.IsHandleCreated);
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
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(radioButton.IsHandleCreated);
        }
    }
}
