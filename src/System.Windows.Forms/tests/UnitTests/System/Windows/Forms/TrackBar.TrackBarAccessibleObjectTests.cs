// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.TestUtilities;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
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
            Assert.Equal(AccessibleRole.Slider, accessibilityObject.Role);
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
        [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_ButtonsAreDisplayed_TestData))]
        public void TrackBarAccessibilityObject_GetChildCount_ReturnsThree_IfAllButtonsAreDisplayed(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
        {
            using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
            TrackBar.TrackBarAccessibleObject accessibilityObject = Assert.IsAssignableFrom<TrackBar.TrackBarAccessibleObject>(trackBar.AccessibilityObject);

            // All control elements (thumb and left/right buttons) are displayed
            Assert.Equal(3, accessibilityObject.GetChildCount());
            Assert.True(trackBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_FirstButtonIsHidden_TestData))]
        public void TrackBarAccessibilityObject_GetChildCount_ReturnsTwo_IfFirstButtonIsHidden(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
        {
            using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
            TrackBar.TrackBarAccessibleObject accessibilityObject = Assert.IsAssignableFrom<TrackBar.TrackBarAccessibleObject>(trackBar.AccessibilityObject);

            // Only thumb and left/right button are displayed
            Assert.Equal(2, accessibilityObject.GetChildCount());
            Assert.True(trackBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_LastButtonIsHidden_TestData))]
        public void TrackBarAccessibilityObject_GetChildCount_ReturnsTwo_IfLastButtonIsHidden(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
        {
            using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
            TrackBar.TrackBarAccessibleObject accessibilityObject = Assert.IsAssignableFrom<TrackBar.TrackBarAccessibleObject>(trackBar.AccessibilityObject);

            // Only thumb and left/right button are displayed
            Assert.Equal(2, accessibilityObject.GetChildCount());
            Assert.True(trackBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_MinimumEqualsMaximum_TestData))]
        public void TrackBarAccessibilityObject_GetChildCount_ReturnsTwo_MinimumEqualsMaximum(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
        {
            using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
            TrackBar.TrackBarAccessibleObject accessibilityObject = Assert.IsAssignableFrom<TrackBar.TrackBarAccessibleObject>(trackBar.AccessibilityObject);

            // Only thumb and left/right button are displayed.
            // The left/right button is displayed even if the minimum is equal to the maximum
            Assert.Equal(2, accessibilityObject.GetChildCount());
            Assert.True(trackBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_WithoutCreateControl_TestData))]
        public void TrackBarAccessibilityObject_GetChildCount_ReturnsMinusOne_IfHandleIsNotCreated(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
        {
            using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: false, value, minimum, maximum);
            TrackBar.TrackBarAccessibleObject accessibilityObject = Assert.IsAssignableFrom<TrackBar.TrackBarAccessibleObject>(trackBar.AccessibilityObject);

            Assert.Equal(-1, accessibilityObject.GetChildCount());
            Assert.False(trackBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_ButtonsAreDisplayed_TestData))]
        public void TrackBarAccessibilityObject_GetChild_ReturnsExpected_ButtonsAreDisplayed(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
        {
            using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
            TrackBar.TrackBarAccessibleObject accessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;

            Assert.Null(accessibleObject.GetChild(-1));
            Assert.Equal(accessibleObject.FirstButtonAccessibleObject, accessibleObject.GetChild(0));
            Assert.Equal(accessibleObject.ThumbAccessibleObject, accessibleObject.GetChild(1));
            Assert.Equal(accessibleObject.LastButtonAccessibleObject, accessibleObject.GetChild(2));
            Assert.Null(accessibleObject.GetChild(3));
            Assert.True(trackBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_FirstButtonIsHidden_TestData))]
        public void TrackBarAccessibilityObject_GetChild_ReturnsExpected_FirstButtonIsHidden(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
        {
            using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
            TrackBar.TrackBarAccessibleObject accessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;

            Assert.Null(accessibleObject.GetChild(-1));
            Assert.Equal(accessibleObject.ThumbAccessibleObject, accessibleObject.GetChild(0));
            Assert.Equal(accessibleObject.LastButtonAccessibleObject, accessibleObject.GetChild(1));
            Assert.Null(accessibleObject.GetChild(2));
            Assert.True(trackBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_LastButtonIsHidden_TestData))]
        public void TrackBarAccessibilityObject_GetChild_ReturnsExpected_LastButtonIsHidden(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
        {
            using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
            TrackBar.TrackBarAccessibleObject accessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;

            Assert.Null(accessibleObject.GetChild(-1));
            Assert.Equal(accessibleObject.FirstButtonAccessibleObject, accessibleObject.GetChild(0));
            Assert.Equal(accessibleObject.ThumbAccessibleObject, accessibleObject.GetChild(1));
            Assert.Null(accessibleObject.GetChild(2));
            Assert.Null(accessibleObject.GetChild(3));
            Assert.True(trackBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_WithoutCreateControl_TestData))]
        public void TrackBarAccessibilityObject_GetChild_ReturnsNull_IfHandleNotCreated(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
        {
            using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: false, value, minimum, maximum);
            TrackBar.TrackBarAccessibleObject accessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;

            Assert.Null(accessibleObject.GetChild(-1));
            Assert.Null(accessibleObject.GetChild(0));
            Assert.Null(accessibleObject.GetChild(1));
            Assert.Null(accessibleObject.GetChild(2));
            Assert.Null(accessibleObject.GetChild(3));
            Assert.False(trackBar.IsHandleCreated);
        }

        [WinFormsFact]
        public void TrackBarAccessibilityObject_ControlType_IsSlider_IfAccessibleRoleIsDefault()
        {
            using TrackBar trackBar = new();

            // AccessibleRole is not set = Default
            object actual = trackBar.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.SliderControlTypeId, actual);
            Assert.False(trackBar.IsHandleCreated);
        }

        [WinFormsFact]
        public void TrackBarAccessibilityObject_Role_IsStatusBar_ByDefault()
        {
            using TrackBar trackBar = new();

            // AccessibleRole is not set = Default
            Assert.Equal(AccessibleRole.Slider, trackBar.AccessibilityObject.Role);
            Assert.False(trackBar.IsHandleCreated);
        }

        public static IEnumerable<object[]> TrackBarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
        {
            Array roles = Enum.GetValues(typeof(AccessibleRole));

            foreach (AccessibleRole role in roles)
            {
                if (role == AccessibleRole.Default)
                {
                    continue; // The test checks custom roles
                }

                yield return new object[] { role };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(TrackBarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void TrackBarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using TrackBar trackBar = new();
            trackBar.AccessibleRole = role;

            object actual = trackBar.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(trackBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData("Test Description")]
        [InlineData(null)]
        public void TrackBarAccessibilityObject_DefaultAction_ReturnExpected(string defaultAction)
        {
            using TrackBar trackBar = new();
            trackBar.AccessibleDefaultActionDescription = defaultAction;

            Assert.Equal(defaultAction, trackBar.AccessibilityObject.DefaultAction);
            Assert.False(trackBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData("Test Description")]
        [InlineData(null)]
        public void TrackBarAccessibilityObject_GetProperyValue_LegacyIAccessibleDefaultActionPropertyId_ReturnExpected(string defaultAction)
        {
            using TrackBar trackBar = new();
            trackBar.AccessibleDefaultActionDescription = defaultAction;

            Assert.Equal(defaultAction, trackBar.AccessibilityObject.GetPropertyValue(UiaCore.UIA.LegacyIAccessibleDefaultActionPropertyId));
            Assert.False(trackBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_ButtonsAreDisplayed_TestData))]
        public void TrackBarAccessibilityObject_FragmentNavigate_Child_ReturnsExpected_ButtonsAreDisplayed(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
        {
            using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
            TrackBar.TrackBarAccessibleObject accessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;

            Assert.Equal(accessibleObject.FirstButtonAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Equal(accessibleObject.LastButtonAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.True(trackBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_FirstButtonIsHidden_TestData))]
        public void TrackBarAccessibilityObject_FragmentNavigate_Child_ReturnsExpected_IfFirstButtonIsHidden(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
        {
            using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
            TrackBar.TrackBarAccessibleObject accessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;

            Assert.Equal(accessibleObject.ThumbAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Equal(accessibleObject.LastButtonAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.True(trackBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_LastButtonIsHidden_TestData))]
        public void TrackBarAccessibilityObject_FragmentNavigate_Child_ReturnsExpected_IfLastButtonIsHidden(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
        {
            using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
            TrackBar.TrackBarAccessibleObject accessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;

            Assert.Equal(accessibleObject.FirstButtonAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Equal(accessibleObject.ThumbAccessibleObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.True(trackBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_WithoutCreateControl_TestData))]
        public void TrackBarAccessibilityObject_FragmentNavigate_Child_ReturnsNull_IfHandleNotCreated(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
        {
            using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: false, value, minimum, maximum);
            TrackBar.TrackBarAccessibleObject accessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;

            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.False(trackBar.IsHandleCreated);
        }

        [WinFormsFact]
        public void TrackBarAccessibilityObject_IsPatternSupported_ReturnsExpected()
        {
            using TrackBar trackBar = new();

            Assert.True(trackBar.AccessibilityObject.IsPatternSupported(UiaCore.UIA.ValuePatternId));
            Assert.True(trackBar.AccessibilityObject.IsPatternSupported(UiaCore.UIA.LegacyIAccessiblePatternId));
            Assert.False(trackBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void TrackBarAccessibilityObject_GetPropertyValue_IsEnabledProperty_ReturnsExpected(bool enabled)
        {
            using TrackBar trackBar = new()
            {
                Enabled = enabled
            };

            Assert.Equal(enabled, trackBar.AccessibilityObject.GetPropertyValue(UiaCore.UIA.IsEnabledPropertyId));
            Assert.False(trackBar.IsHandleCreated);
        }

        [WinFormsFact]
        public void TrackBarAccessibilityObject_GetPropertyValue_RuntimeId_ReturnsExpected()
        {
            using TrackBar trackBar = new();
            object actual = trackBar.AccessibilityObject.GetPropertyValue(UiaCore.UIA.RuntimeIdPropertyId);

            Assert.Equal(trackBar.AccessibilityObject.RuntimeId, actual);
            Assert.False(trackBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.NamePropertyId, "TestAccessibleName")]
        [InlineData((int)UiaCore.UIA.AutomationIdPropertyId, "TestControlName")]
        public void TrackBarAccessibilityObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
        {
            using TrackBar trackBar = new()
            {
                Name = expected.ToString(),
                AccessibleName = expected.ToString()
            };

            TrackBar.TrackBarAccessibleObject accessibilityObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;

            object value = accessibilityObject.GetPropertyValue((UiaCore.UIA)propertyID);
            Assert.Equal(expected, value);
            Assert.False(trackBar.IsHandleCreated);
        }

        [WinFormsFact]
        public void TrackBarAccessibilityObject_GetPropertyValue_NativeWindowHandle_ReturnsExpected()
        {
            using TrackBar trackBar = new();
            trackBar.CreateControl(false);
            object actual = trackBar.AccessibilityObject.GetPropertyValue(UiaCore.UIA.NativeWindowHandlePropertyId);

            Assert.Equal(trackBar.InternalHandle, actual);
        }

        [WinFormsTheory]
        [InlineData(false, ((int)UiaCore.UIA.IsExpandCollapsePatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsGridItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsGridPatternAvailablePropertyId))]
        [InlineData(true, ((int)UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsMultipleViewPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsScrollItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsScrollPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsSelectionItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsSelectionPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTableItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTablePatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTextPattern2AvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTextPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTogglePatternAvailablePropertyId))]
        [InlineData(true, ((int)UiaCore.UIA.IsValuePatternAvailablePropertyId))]
        public void TrackBarAccessibleObject_GetPropertyValue_Pattern_ReturnsExpected(bool expected, int propertyId)
        {
            using TrackBar trackBar = new();
            TrackBar.TrackBarAccessibleObject accessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;

            Assert.Equal(expected, accessibleObject.GetPropertyValue((UiaCore.UIA)propertyId) ?? false);
            Assert.False(trackBar.IsHandleCreated);
        }

        private TrackBar GetTrackBar(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int value, int minimum, int maximum)
        {
            TrackBar trackBar = new()
            {
                Value = value,
                Maximum = maximum,
                Minimum = minimum,
                LargeChange = 3,
                Orientation = orientation,
                RightToLeft = rightToLeft,
                RightToLeftLayout = rightToLeftLayout
            };

            if (createControl)
            {
                trackBar.CreateControl();
            }

            return trackBar;
        }
    }
}
