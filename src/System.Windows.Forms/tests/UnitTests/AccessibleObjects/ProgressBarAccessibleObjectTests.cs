// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Accessibility;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ProgressBarAccessibleObject : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ProgressBarAccessibilityObject_Properties_ReturnsExpected()
        {
            using var ownerControl = new ProgressBar
            {
                Value = 5,
            };
            Control.ControlAccessibleObject accessibilityObject = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(ownerControl.AccessibilityObject);
            Assert.Equal(ownerControl.ClientSize, accessibilityObject.Bounds.Size);
            Assert.Null(accessibilityObject.DefaultAction);
            Assert.Null(accessibilityObject.Description);
            Assert.Equal(ownerControl.Handle, accessibilityObject.Handle);
            Assert.Null(accessibilityObject.Help);
            Assert.Null(accessibilityObject.KeyboardShortcut);
            Assert.Null(accessibilityObject.Name);
            Assert.Equal(AccessibleRole.ProgressBar, accessibilityObject.Role);
            Assert.Same(ownerControl, accessibilityObject.Owner);
            Assert.NotNull(accessibilityObject.Parent);
            Assert.Equal(AccessibleStates.ReadOnly | AccessibleStates.Focusable, accessibilityObject.State);
            Assert.Equal("5%", accessibilityObject.Value);
        }

        [WinFormsTheory]
        [InlineData("100%")]
        [InlineData("0%")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("INVALID")]
        public void ProgressBarAccessibilityObject_Value_Set_GetReturnsExpected(string value)
        {
            using var ownerControl = new ProgressBar();
            Control.ControlAccessibleObject accessibilityObject = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(ownerControl.AccessibilityObject);
            accessibilityObject.Value = value;
            Assert.Equal("0%", accessibilityObject.Value);
            Assert.Equal(0, ownerControl.Value);

            // Set same.
            accessibilityObject.Value = value;
            Assert.Equal("0%", accessibilityObject.Value);
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
