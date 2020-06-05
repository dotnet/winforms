// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class RadioButtonAccessibleObjectTests
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
            var radioButtonAccessibleObject = new RadioButton.RadioButtonAccessibleObject(radioButton);

            Assert.NotNull(radioButtonAccessibleObject.Owner);
            Assert.Equal(AccessibleRole.RadioButton, radioButtonAccessibleObject.Role);
        }

        [WinFormsFact]
        public void RadioButtonAccessibleObject_Property_returns_correct_value()
        {
            using var radioButton = new RadioButton
            {
                Name = "RadioButton1",
                AccessibleName = "TestName",
                AccessibleDescription = "TestDescription",
                AccessibleRole = AccessibleRole.PushButton
            };

            var radioButtonAccessibleObject = new RadioButton.RadioButtonAccessibleObject(radioButton);
            Assert.Equal("TestName", radioButtonAccessibleObject.Name);
            Assert.Equal("TestDescription", radioButtonAccessibleObject.Description);
            Assert.Equal(AccessibleRole.PushButton, radioButtonAccessibleObject.Role);
        }

        public static IEnumerable<object[]> GetPropertyValue_TestData()
        {
            foreach (bool isHandleCreated in new bool[] { true, false })
            {
                yield return new object[] { UIA.NamePropertyId, isHandleCreated, "TestName" };
                yield return new object[] { UIA.ControlTypePropertyId, isHandleCreated, UIA.RadioButtonControlTypeId };
                yield return new object[] { UIA.IsKeyboardFocusablePropertyId, isHandleCreated, true };
            }

            yield return new object[] { UIA.AutomationIdPropertyId, true, "RadioButton1" };
            yield return new object[] { UIA.AutomationIdPropertyId, false, "RadioButton1" };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetPropertyValue_TestData))]
        internal void RadioButtonAccessibleObject_GetPropertyValue_returns_correct_values(UIA propertyID, bool isHandleCreated, object expected)
        {
            using var radioButton = new RadioButton
            {
                Name = "RadioButton1",
                AccessibleName = "TestName"
            };

            var radioButtonAccessibleObject = new RadioButton.RadioButtonAccessibleObject(radioButton);

            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(radioButton.IsHandleCreated);

            if (isHandleCreated)
            {
                Assert.NotEqual(IntPtr.Zero, radioButton.Handle);
            }

            object value = radioButtonAccessibleObject.GetPropertyValue(propertyID);
            Assert.Equal(expected, value);

            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            // Assert.Equal(isHandleCreated, radioButton.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(UIA.LegacyIAccessiblePatternId)]
        [InlineData(UIA.SelectionItemPatternId)]
        internal void RadioButtonAccessibleObject_IsPatternSupported_returns_correct_value(UIA patternId)
        {
            using var radioButton = new RadioButton
            {
                Name = "RadioButton1"
            };

            var radioButtonAccessibleObject = new RadioButton.RadioButtonAccessibleObject(radioButton);
            Assert.True(radioButtonAccessibleObject.IsPatternSupported(patternId));
        }
    }
}
