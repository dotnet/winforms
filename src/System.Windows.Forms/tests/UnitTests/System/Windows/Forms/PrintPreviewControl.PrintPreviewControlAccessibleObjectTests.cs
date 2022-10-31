// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class PrintPreviewControl_PrintPreviewControlAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void PrintPreviewControlAccessibleObject_Ctor_Default()
        {
            using PrintPreviewControl control = new();
            PrintPreviewControl.PrintPreviewControlAccessibleObject accessibleObject = new(control);

            Assert.Equal(control, accessibleObject.Owner);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UIA.NamePropertyId, "TestName")]
        [InlineData((int)UIA.AutomationIdPropertyId, "PrintPreviewControl1")]
        public void PrintPreviewControlAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
        {
            using PrintPreviewControl control = new()
            {
                Name = "PrintPreviewControl1",
                AccessibleName = "TestName"
            };

            Assert.False(control.IsHandleCreated);
            var accessibleObject = new PrintPreviewControl.PrintPreviewControlAccessibleObject(control);
            object value = accessibleObject.GetPropertyValue((UIA)propertyID);

            Assert.Equal(expected, value);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void PrintPreviewControlAccessibleObject_GetPropertyValue_Focused_ReturnsExpected()
        {
            using PrintPreviewControl control = new();

            var accessibleObject = new PrintPreviewControl.PrintPreviewControlAccessibleObject(control);
            control.CreateControl();
            PInvoke.SetFocus(control);
            bool value = (bool)accessibleObject.GetPropertyValue(UIA.HasKeyboardFocusPropertyId);

            Assert.True(value);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void PrintPreviewControlAccessibleObject_GetPropertyValue_Unfocused_ReturnsExpected()
        {
            using PrintPreviewControl control = new();

            Assert.False(control.IsHandleCreated);
            var accessibleObject = new PrintPreviewControl.PrintPreviewControlAccessibleObject(control);
            bool value = (bool)accessibleObject.GetPropertyValue(UIA.HasKeyboardFocusPropertyId);

            Assert.False(value);
            Assert.False(control.IsHandleCreated);
        }
    }
}
