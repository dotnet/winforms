// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Xunit;
using static System.Windows.Forms.TrackBar;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class TrackBar_TrackBarChildAccessibleObjectTests
    {
        [WinFormsFact]
        public void TrackBarChildAccessibleObject_Ctor_OwnerTrackBarCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SubTrackBarChildAccessibleObject(null));
        }

        [WinFormsFact]
        public void TrackBarChildAccessibleObject_Bounds_IsEmptyRectangle_IfOwningControlIsNotCreated()
        {
            using TrackBar control = new();
            var accessibleObject = new SubTrackBarChildAccessibleObject(control);

            Assert.Equal(Rectangle.Empty, accessibleObject.Bounds);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TrackBarChildAccessibleObject_Bounds_IsEmptyRectangle_IfOwningControlIsNotVisible()
        {
            using TrackBar control = new();
            control.CreateControl();
            control.Visible = false;
            var accessibleObject = new SubTrackBarChildAccessibleObject(control);

            Assert.Equal(Rectangle.Empty, accessibleObject.Bounds);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TrackBarChildAccessibleObject_Bounds_IsNotEmptyRectangle()
        {
            using TrackBar control = new();
            control.CreateControl();
            var accessibleObject = new SubTrackBarChildAccessibleObject(control);

            Assert.NotEqual(Rectangle.Empty, accessibleObject.Bounds);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TrackBarChildAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
        {
            using TrackBar control = new();
            var accessibleObject = new SubTrackBarChildAccessibleObject(control);

            Assert.Equal(control.AccessibilityObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TrackBarChildAccessibleObject_FragmentRoot_ReturnsExpected()
        {
            using TrackBar control = new();
            var accessibleObject = new SubTrackBarChildAccessibleObject(control);

            Assert.Equal(control.AccessibilityObject, accessibleObject.FragmentRoot);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TrackBarChildAccessibleObject_IsDisplayed_ReturnsExpected(bool isVisible)
        {
            using TrackBar control = new();
            control.Visible = isVisible;
            var accessibleObject = new SubTrackBarChildAccessibleObject(control);

            Assert.Equal(isVisible, accessibleObject.IsDisplayed);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TrackBarChildAccessibleObject_GetPropertyValue_ControlType_ReturnsExpected()
        {
            using TrackBar control = new();
            var accessibleObject = new SubTrackBarChildAccessibleObject(control);

            Assert.Equal(UiaCore.UIA.ButtonControlTypeId, accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TrackBarChildAccessibleObject_GetPropertyValue_IsEnabled_ReturnsExpected(bool isEnabled)
        {
            using TrackBar control = new();
            control.Enabled = isEnabled;
            var accessibleObject = new SubTrackBarChildAccessibleObject(control);

            Assert.Equal(isEnabled, accessibleObject.GetPropertyValue(UiaCore.UIA.IsEnabledPropertyId));
            Assert.False(control.IsHandleCreated);
        }

        private class SubTrackBarChildAccessibleObject : TrackBarChildAccessibleObject
        {
            public SubTrackBarChildAccessibleObject(TrackBar owningTrackBar) : base(owningTrackBar)
            { }
        }
    }
}
