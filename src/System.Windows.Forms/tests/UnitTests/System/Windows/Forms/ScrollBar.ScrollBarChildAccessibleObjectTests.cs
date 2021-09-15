// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Xunit;
using static System.Windows.Forms.ScrollBar;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ScrollBar_ScrollBarChildAccessibleObjectTests
    {
        [WinFormsFact]
        public void ScrollBarChildAccessibleObject_Ctor_Default()
        {
            var accessibleObject = new ScrollBarChildAccessibleObject(null);
            Assert.Null(accessibleObject.OwningScrollBar);
        }

        [WinFormsFact]
        public void ScrollBarChildAccessibleObject_Bounds_IsEmptyRectangle_IfOwningControlIsNotCreated()
        {
            using SubScrollBar control = new();
            var accessibleObject = new ScrollBarChildAccessibleObject(control);

            Assert.Equal(Rectangle.Empty, accessibleObject.Bounds);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ScrollBarChildAccessibleObject_Bounds_IsEmptyRectangle_IfOwningControlIsNotVisible()
        {
            using SubScrollBar control = new();
            control.CreateControl();
            control.Visible = false;
            var accessibleObject = new ScrollBarChildAccessibleObject(control);

            Assert.Equal(Rectangle.Empty, accessibleObject.Bounds);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ScrollBarChildAccessibleObject_Bounds_IsNotEmptyRectangle()
        {
            using SubScrollBar control = new();
            control.CreateControl();
            var accessibleObject = new ScrollBarChildAccessibleObject(control);

            Assert.NotEqual(Rectangle.Empty, accessibleObject.Bounds);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ScrollBarChildAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
        {
            using SubScrollBar control = new();
            var accessibleObject = new ScrollBarChildAccessibleObject(control);

            Assert.Equal(control.AccessibilityObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ScrollBarChildAccessibleObject_FragmentRoot_ReturnsExpected()
        {
            using SubScrollBar control = new();
            var accessibleObject = new ScrollBarChildAccessibleObject(control);

            Assert.Equal(control.AccessibilityObject, accessibleObject.FragmentRoot);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollBarChildAccessibleObject_IsDisplayed_ReturnsExpected(bool isVisible)
        {
            using SubScrollBar control = new();
            control.Visible = isVisible;
            var accessibleObject = new ScrollBarChildAccessibleObject(control);

            Assert.Equal(isVisible, accessibleObject.IsDisplayed);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ScrollBarChildAccessibleObject_GetPropertyValue_ControlType_ReturnsExpected()
        {
            var accessibleObject = new ScrollBarChildAccessibleObject(null);
            Assert.Equal(UiaCore.UIA.ButtonControlTypeId, accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollBarChildAccessibleObject_GetPropertyValue_IsEnabled_ReturnsExpected(bool isEnabled)
        {
            using SubScrollBar control = new();
            control.Enabled = isEnabled;
            var accessibleObject = new ScrollBarChildAccessibleObject(control);

            Assert.Equal(isEnabled, accessibleObject.GetPropertyValue(UiaCore.UIA.IsEnabledPropertyId));
            Assert.False(control.IsHandleCreated);
        }

        private class SubScrollBar : ScrollBar
        { }
    }
}
