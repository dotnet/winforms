// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using Accessibility;
using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class TrackBarAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void TrackBarAccessibilityObject_Properties_ReturnsExpected_IfHandleIsCreated()
        {
            using var ownerControl = new TrackBar
            {
                Value = 5,
            };

            ownerControl.CreateControl();
            Control.ControlAccessibleObject accessibilityObject = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(ownerControl.AccessibilityObject);
            Assert.Equal(ownerControl.Size, accessibilityObject.Bounds.Size);
            Assert.Null(accessibilityObject.DefaultAction);
            Assert.Null(accessibilityObject.Description);
            Assert.True(ownerControl.IsHandleCreated);
            Assert.Equal(ownerControl.Handle, accessibilityObject.Handle);
            Assert.Null(accessibilityObject.Help);
            Assert.Null(accessibilityObject.KeyboardShortcut);
            Assert.Null(accessibilityObject.Name);
            Assert.Equal(AccessibleRole.Slider, accessibilityObject.Role);
            Assert.Same(ownerControl, accessibilityObject.Owner);
            Assert.NotNull(accessibilityObject.Parent);
            Assert.Equal(AccessibleStates.Focusable, accessibilityObject.State);
            Assert.Equal("50", accessibilityObject.Value);
        }

        [WinFormsFact]
        public void TrackBarAccessibilityObject_Properties_ReturnsExpected_IfHandleIsNotCreated()
        {
            using var ownerControl = new TrackBar
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
        [InlineData("100", 10, "100", true)]
        [InlineData("50", 5, "50", true)]
        [InlineData("54", 5, "50", true)]
        [InlineData("56", 5, "50", true)]
        [InlineData("0", 0, "0", true)]
        [InlineData("100", 0, "", false)]
        [InlineData("50", 0, "", false)]
        [InlineData("54", 0, "", false)]
        [InlineData("56", 0, "", false)]
        [InlineData("0", 0, "", false)]
        public void TrackBarAccessibilityObject_Value_Set_GetReturnsExpected(string value, int expected, string expectedValueString, bool createControl)
        {
            using var ownerControl = new TrackBar();
            if (createControl)
            {
                ownerControl.CreateControl();
            }

            Assert.Equal(createControl, ownerControl.IsHandleCreated);
            Control.ControlAccessibleObject accessibilityObject = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(ownerControl.AccessibilityObject);
            Assert.Equal(createControl, ownerControl.IsHandleCreated);
            accessibilityObject.Value = value;
            Assert.Equal(expectedValueString, accessibilityObject.Value);
            Assert.Equal(expected, ownerControl.Value);

            // Set same.
            accessibilityObject.Value = value;
            Assert.Equal(expectedValueString, accessibilityObject.Value);
            Assert.Equal(expected, ownerControl.Value);
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("NotAnInt")]
        public void TrackBarAccessibilityObject_Value_SetInvalid_ThrowsCOMException_IfHandleIsCreated(string value)
        {
            using var ownerControl = new TrackBar
            {
                Value = 5
            };

            ownerControl.CreateControl();
            Control.ControlAccessibleObject accessibilityObject = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(ownerControl.AccessibilityObject);
            Assert.Throws<COMException>(() => accessibilityObject.Value = value);
            Assert.Equal("50", accessibilityObject.Value);
            Assert.Equal(5, ownerControl.Value);
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("NotAnInt")]
        public void TrackBarAccessibilityObject_Value_SetInvalid_ThrowsCOMException_IfHandleIsNotCreated(string value)
        {
            using var ownerControl = new TrackBar
            {
                Value = 5
            };

            Control.ControlAccessibleObject accessibilityObject = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(ownerControl.AccessibilityObject);
            accessibilityObject.Value = value;
            Assert.Equal(string.Empty, accessibilityObject.Value);
            Assert.Equal(5, ownerControl.Value);
        }

        [WinFormsTheory]
        [InlineData(true, 3)]
        [InlineData(false, 0)]
        public void TrackBarAccessibilityObject_GetChildCount_ReturnsExpected(bool createControl, int expectedChildACount)
        {
            using var ownerControl = new TrackBar
            {
                Value = 5
            };

            if (createControl)
            {
                ownerControl.CreateControl();
            }

            Control.ControlAccessibleObject accessibilityObject = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(ownerControl.AccessibilityObject);
            IAccessible iAccessible = accessibilityObject;
            Assert.Equal(expectedChildACount, iAccessible.accChildCount);
            Assert.Equal(-1, accessibilityObject.GetChildCount());
        }
    }
}
