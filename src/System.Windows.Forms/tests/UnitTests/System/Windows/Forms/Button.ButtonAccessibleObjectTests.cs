﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class Button_ButtonAccessibleObjectTests
    {
        [WinFormsFact]
        public void ButtonAccessibleObject_Ctor_NullControl_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Button.ButtonAccessibleObject(null));
        }

        [WinFormsFact]
        public void ButtonAccessibleObject_Ctor_InitializesOwner()
        {
            using var button = new Button();
            Assert.False(button.IsHandleCreated);
            var buttonAccessibleObject = new Button.ButtonAccessibleObject(button);

            Assert.Same(button, buttonAccessibleObject.Owner);
            // TODO: ControlAccessibleObject should not force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(button.IsHandleCreated);
        }

        [WinFormsFact]
        public void ButtonAccessibleObject_AccessibleRole_Default_ReturnsPushButton()
        {
            using var button = new Button
            {
                AccessibleRole = AccessibleRole.Default
            };

            Assert.False(button.IsHandleCreated);
            var buttonAccessibleObject = new Button.ButtonAccessibleObject(button);

            Assert.Equal(AccessibleRole.PushButton, buttonAccessibleObject.Role);
            // TODO: ControlAccessibleObject should not force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(button.IsHandleCreated);
        }

        [WinFormsFact]
        public void ButtonAccessibleObject_AccessibleRole_Custom_ReturnsExpected()
        {
            using var button = new Button
            {
                AccessibleRole = AccessibleRole.Link
            };

            Assert.False(button.IsHandleCreated);
            var buttonAccessibleObject = new Button.ButtonAccessibleObject(button);

            Assert.Equal(AccessibleRole.Link, buttonAccessibleObject.Role);
            // TODO: ControlAccessibleObject should not force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(button.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UIA.NamePropertyId, "TestName")]
        [InlineData((int)UIA.ControlTypePropertyId, UIA.ButtonControlTypeId)]
        [InlineData((int)UIA.IsKeyboardFocusablePropertyId, true)]
        [InlineData((int)UIA.AutomationIdPropertyId, "Button1")]
        public void ButtonAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
        {
            using var button = new Button
            {
                Name = "Button1",
                AccessibleName = "TestName"
            };

            Assert.False(button.IsHandleCreated);
            var buttonAccessibleObject = new Button.ButtonAccessibleObject(button);
            object value = buttonAccessibleObject.GetPropertyValue((UIA)propertyID);

            Assert.Equal(expected, value);
            // TODO: ControlAccessibleObject should not force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(button.IsHandleCreated);
        }

        [WinFormsFact]
        public void ButtonAccessibleObject_IsPatternSupported_Invoke_ReturnsTrue_ForLegacyIAccessiblePatternId()
        {
            using var button = new Button();

            Assert.False(button.IsHandleCreated);
            var buttonAccessibleObject = new Button.ButtonAccessibleObject(button);

            Assert.True(buttonAccessibleObject.IsPatternSupported(UIA.LegacyIAccessiblePatternId));
            // TODO: ControlAccessibleObject should not force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(button.IsHandleCreated);
        }
    }
}
