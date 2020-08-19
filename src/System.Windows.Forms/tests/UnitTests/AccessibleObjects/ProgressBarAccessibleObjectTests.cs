// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Accessibility;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ProgressBarAccessibleObject : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ProgressBarAccessibilityObject_Properties_ReturnsExpected_IfHandleIsCreated()
        {
            using var ownerControl = new ProgressBar
            {
                Value = 5,
            };

            ownerControl.CreateControl();

            Control.ControlAccessibleObject accessibilityObject = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(ownerControl.AccessibilityObject);

            Assert.Equal(ownerControl.ClientSize, accessibilityObject.Bounds.Size);
            Assert.Null(accessibilityObject.DefaultAction);
            Assert.Null(accessibilityObject.Description);
            Assert.Null(accessibilityObject.Help);
            Assert.Null(accessibilityObject.KeyboardShortcut);
            Assert.Null(accessibilityObject.Name);
            Assert.Equal(AccessibleRole.ProgressBar, accessibilityObject.Role);
            Assert.Same(ownerControl, accessibilityObject.Owner);
            Assert.NotNull(accessibilityObject.Parent);
            Assert.Equal(AccessibleStates.ReadOnly | AccessibleStates.Focusable, accessibilityObject.State);
            Assert.Equal("5%", accessibilityObject.Value);
            Assert.True(ownerControl.IsHandleCreated);
            Assert.Equal(ownerControl.Handle, accessibilityObject.Handle);
        }

        [WinFormsFact]
        public void ProgressBarAccessibilityObject_Properties_ReturnsExpected_IfHandleIsNotCreated()
        {
            using var ownerControl = new ProgressBar
            {
                Value = 5,
            };

            Control.ControlAccessibleObject accessibilityObject = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(ownerControl.AccessibilityObject);

            Assert.Equal(Rectangle.Empty.Size, accessibilityObject.Bounds.Size);
            Assert.Null(accessibilityObject.DefaultAction);
            Assert.Null(accessibilityObject.Description);
            Assert.Null(accessibilityObject.Help);
            Assert.Null(accessibilityObject.KeyboardShortcut);
            Assert.Null(accessibilityObject.Name);
            Assert.Equal(AccessibleRole.None, accessibilityObject.Role);
            Assert.Same(ownerControl, accessibilityObject.Owner);
            Assert.Null(accessibilityObject.Parent);
            Assert.Equal(AccessibleStates.None, accessibilityObject.State);
            Assert.Equal(string.Empty, accessibilityObject.Value);
            Assert.False(ownerControl.IsHandleCreated);
            Assert.Equal(ownerControl.Handle, accessibilityObject.Handle);
        }

        [WinFormsTheory]
        [InlineData("100%", true, "0%")]
        [InlineData("0%", true, "0%")]
        [InlineData(null, true, "0%")]
        [InlineData("", true, "0%")]
        [InlineData("INVALID", true, "0%")]
        [InlineData("100%", false, "")]
        [InlineData("0%", false, "")]
        [InlineData(null, false, "")]
        [InlineData("", false, "")]
        [InlineData("INVALID", false, "")]
        public void ProgressBarAccessibilityObject_Value_Set_GetReturnsExpected(string value, bool createControl, string expectedValue)
        {
            using var ownerControl = new ProgressBar();
            if (createControl)
            {
                ownerControl.CreateControl();
            }

            Assert.Equal(createControl, ownerControl.IsHandleCreated);

            Control.ControlAccessibleObject accessibilityObject = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(ownerControl.AccessibilityObject);
            Assert.Equal(createControl, ownerControl.IsHandleCreated);
            accessibilityObject.Value = value;
            Assert.Equal(expectedValue, accessibilityObject.Value);
            Assert.Equal(0, ownerControl.Value);

            // Set same.
            accessibilityObject.Value = value;
            Assert.Equal(expectedValue, accessibilityObject.Value);
            Assert.Equal(0, ownerControl.Value);
        }

        [WinFormsFact]
        public void ProgressBarAccessibilityObject_GetChildCount_ReturnsExpected()
        {
            using var ownerControl = new ProgressBar
            {
                Value = 5
            };
            Control.ControlAccessibleObject accessibilityObject = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(ownerControl.AccessibilityObject);
            IAccessible iAccessible = accessibilityObject;
            Assert.Equal(0, iAccessible.accChildCount);
            Assert.Equal(-1, accessibilityObject.GetChildCount());
        }
    }
}
