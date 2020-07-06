﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class CheckBox_CheckBoxAccessibleObjectTests
    {
        [WinFormsFact]
        public void CheckBoxAccessibleObject_Ctor_NullControl_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new CheckBox.CheckBoxAccessibleObject(null));
        }

        [WinFormsFact]
        public void CheckBoxAccessibleObject_Ctor_InvalidTypeControl_ThrowsArgumentException()
        {
            using var textBox = new TextBox();
            Assert.Throws<ArgumentException>(() => new CheckBox.CheckBoxAccessibleObject(textBox));
        }

        [WinFormsFact]
        public void CheckBoxAccessibleObject_Ctor_Default()
        {
            using var checkBox = new CheckBox();
            Assert.False(checkBox.IsHandleCreated);
            var checkBoxAccessibleObject = new CheckBox.CheckBoxAccessibleObject(checkBox);

            Assert.NotNull(checkBoxAccessibleObject.Owner);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(checkBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void CheckBoxAccessibleObject_CustomDoDefaultAction_ReturnsExpected()
        {;
            using var checkBox = new CheckBox
            {
                Name = "CheckBox1",
                AccessibleDefaultActionDescription = "TestActionDescription"
            };

            Assert.False(checkBox.IsHandleCreated);
            var checkBoxAccessibleObject = new CheckBox.CheckBoxAccessibleObject(checkBox);

            Assert.Equal("TestActionDescription", checkBoxAccessibleObject.DefaultAction);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(checkBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void CheckBoxAccessibleObject_DefaultRole_ReturnsExpected()
        {
            using var checkBox = new CheckBox();
            Assert.False(checkBox.IsHandleCreated);
            var checkBoxAccessibleObject = new CheckBox.CheckBoxAccessibleObject(checkBox);

            Assert.Equal(AccessibleRole.CheckButton, checkBoxAccessibleObject.Role);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(checkBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void CheckBoxAccessibleObject_CustomRole_ReturnsExpected()
        {
            using var checkBox = new CheckBox
            {
                AccessibleRole = AccessibleRole.PushButton
            };

            Assert.False(checkBox.IsHandleCreated);
            var checkBoxAccessibleObject = new CheckBox.CheckBoxAccessibleObject(checkBox);

            Assert.Equal(AccessibleRole.PushButton, checkBoxAccessibleObject.Role);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(checkBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void CheckBoxAccessibleObject_State_ReturnsExpected()
        {
            using var checkBox = new CheckBox();
            Assert.False(checkBox.IsHandleCreated);
            var checkBoxAccessibleObject = new CheckBox.CheckBoxAccessibleObject(checkBox);

            Assert.Equal(AccessibleStates.Focusable, checkBoxAccessibleObject.State);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(checkBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void CheckBoxAccessibleObject_ToggleState_ReturnsExpected()
        {
            using var checkBox = new CheckBox();
            Assert.False(checkBox.IsHandleCreated);
            var checkBoxAccessibleObject = new CheckBox.CheckBoxAccessibleObject(checkBox);
            Assert.Equal(ToggleState.Off, checkBoxAccessibleObject.ToggleState);
            checkBoxAccessibleObject.DoDefaultAction();

            Assert.Equal(ToggleState.On, checkBoxAccessibleObject.ToggleState);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(checkBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void CheckBoxAccessibleObject_Description_ReturnsExpected()
        {
            using var checkBox = new CheckBox
            {
                AccessibleDescription = "TestDescription"
            };

            Assert.False(checkBox.IsHandleCreated);
            var checkBoxAccessibleObject = new CheckBox.CheckBoxAccessibleObject(checkBox);

            Assert.Equal("TestDescription", checkBoxAccessibleObject.Description);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(checkBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void CheckBoxAccessibleObject_Name_ReturnsExpected()
        {
            using var checkBox = new CheckBox
            {
                AccessibleName = "TestName"
            };

            Assert.False(checkBox.IsHandleCreated);
            var checkBoxAccessibleObject = new CheckBox.CheckBoxAccessibleObject(checkBox);

            Assert.Equal("TestName", checkBoxAccessibleObject.Name);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(checkBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UIA.NamePropertyId, "TestName")]
        [InlineData((int)UIA.ControlTypePropertyId, UIA.CheckBoxControlTypeId)]
        [InlineData((int)UIA.IsKeyboardFocusablePropertyId, true)]
        [InlineData((int)UIA.AutomationIdPropertyId, "CheckBox1")]
        public void CheckBoxAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
        {
            using var checkBox = new CheckBox
            {
                Name = "CheckBox1",
                AccessibleName = "TestName"
            };

            Assert.False(checkBox.IsHandleCreated);
            var checkBoxAccessibleObject = new CheckBox.CheckBoxAccessibleObject(checkBox);
            object value = checkBoxAccessibleObject.GetPropertyValue((UIA)propertyID);

            Assert.Equal(expected, value);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(checkBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UIA.TogglePatternId)]
        [InlineData((int)UIA.LegacyIAccessiblePatternId)]
        public void CheckBoxAccessibleObject_IsPatternSupported_Invoke_ReturnsExpected(int patternId)
        {
            using var checkBox = new CheckBox();
            Assert.False(checkBox.IsHandleCreated);
            var checkBoxAccessibleObject = new CheckBox.CheckBoxAccessibleObject(checkBox);

            Assert.True(checkBoxAccessibleObject.IsPatternSupported((UIA)patternId));
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(checkBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void CheckBoxAccessibleObject_Toggle_Invoke_Success()
        {
            using var checkBox = new CheckBox();
            Assert.False(checkBox.IsHandleCreated);
            var checkBoxAccessibleObject = new CheckBox.CheckBoxAccessibleObject(checkBox);
            Assert.False(checkBox.Checked);

            checkBoxAccessibleObject.Toggle();
            Assert.True(checkBox.Checked);

            // toggle again
            checkBoxAccessibleObject.Toggle();

            Assert.False(checkBox.Checked);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(checkBox.IsHandleCreated);
        }
    }
}
