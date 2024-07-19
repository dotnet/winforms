// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.TestUtilities;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class TrackBarAccessibleObjectTests
{
    [WinFormsFact]
    public void TrackBarAccessibilityObject_Properties_ReturnsExpected_IfHandleIsCreated()
    {
        using TrackBar ownerControl = new()
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
        using TrackBar ownerControl = new()
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
        using TrackBar ownerControl = new();
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
        using TrackBar ownerControl = new()
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
        using TrackBar ownerControl = new()
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
        var actual = (UIA_CONTROLTYPE_ID)(int)trackBar.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_SliderControlTypeId, actual);
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

        var actual = (UIA_CONTROLTYPE_ID)(int)trackBar.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

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
        VARIANT actual = trackBar.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleDefaultActionPropertyId);
        if (defaultAction is null)
        {
            Assert.Equal(VARIANT.Empty, actual);
        }
        else
        {
            Assert.Equal(defaultAction, ((BSTR)actual).ToStringAndFree());
        }

        Assert.False(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_ButtonsAreDisplayed_TestData))]
    public void TrackBarAccessibilityObject_FragmentNavigate_Child_ReturnsExpected_ButtonsAreDisplayed(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
        TrackBar.TrackBarAccessibleObject accessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;

        Assert.Equal(accessibleObject.FirstButtonAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject.LastButtonAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.True(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_FirstButtonIsHidden_TestData))]
    public void TrackBarAccessibilityObject_FragmentNavigate_Child_ReturnsExpected_IfFirstButtonIsHidden(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
        TrackBar.TrackBarAccessibleObject accessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;

        Assert.Equal(accessibleObject.ThumbAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject.LastButtonAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.True(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_LastButtonIsHidden_TestData))]
    public void TrackBarAccessibilityObject_FragmentNavigate_Child_ReturnsExpected_IfLastButtonIsHidden(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
        TrackBar.TrackBarAccessibleObject accessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;

        Assert.Equal(accessibleObject.FirstButtonAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject.ThumbAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.True(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_WithoutCreateControl_TestData))]
    public void TrackBarAccessibilityObject_FragmentNavigate_Child_ReturnsNull_IfHandleNotCreated(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: false, value, minimum, maximum);
        TrackBar.TrackBarAccessibleObject accessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(trackBar.IsHandleCreated);
    }

    [WinFormsFact]
    public void TrackBarAccessibilityObject_IsPatternSupported_ReturnsExpected()
    {
        using TrackBar trackBar = new();

        Assert.True(trackBar.AccessibilityObject.IsPatternSupported(UIA_PATTERN_ID.UIA_ValuePatternId));
        Assert.True(trackBar.AccessibilityObject.IsPatternSupported(UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId));
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

        Assert.Equal(enabled, (bool)trackBar.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId));
        Assert.False(trackBar.IsHandleCreated);
    }

    [WinFormsFact]
    public void TrackBarAccessibilityObject_GetPropertyValue_RuntimeId_ReturnsExpected()
    {
        using TrackBar trackBar = new();
        using VARIANT actual = trackBar.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_RuntimeIdPropertyId);

        Assert.Equal(trackBar.AccessibilityObject.RuntimeId, actual.ToObject());
        Assert.False(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_NamePropertyId, "TestAccessibleName")]
    [InlineData((int)UIA_PROPERTY_ID.UIA_AutomationIdPropertyId, "TestControlName")]
    public void TrackBarAccessibilityObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, string expected)
    {
        using TrackBar trackBar = new()
        {
            Name = expected.ToString(),
            AccessibleName = expected.ToString()
        };

        TrackBar.TrackBarAccessibleObject accessibilityObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;

        string value = ((BSTR)accessibilityObject.GetPropertyValue((UIA_PROPERTY_ID)propertyID)).ToStringAndFree();
        Assert.Equal(expected, value);
        Assert.False(trackBar.IsHandleCreated);
    }

    [WinFormsFact]
    public void TrackBarAccessibilityObject_GetPropertyValue_NativeWindowHandle_ReturnsExpected()
    {
        using TrackBar trackBar = new();
        trackBar.CreateControl(false);
        int actual = (int)trackBar.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NativeWindowHandlePropertyId);

        Assert.Equal((int)(nint)trackBar.InternalHandle, actual);
    }

    [WinFormsTheory]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsExpandCollapsePatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsGridItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsGridPatternAvailablePropertyId))]
    [InlineData(true, ((int)UIA_PROPERTY_ID.UIA_IsLegacyIAccessiblePatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsMultipleViewPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsScrollItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsScrollPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsSelectionItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsSelectionPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTableItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTablePatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTextPattern2AvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTextPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTogglePatternAvailablePropertyId))]
    [InlineData(true, ((int)UIA_PROPERTY_ID.UIA_IsValuePatternAvailablePropertyId))]
    public void TrackBarAccessibleObject_GetPropertyValue_Pattern_ReturnsExpected(bool expected, int propertyId)
    {
        using TrackBar trackBar = new();
        TrackBar.TrackBarAccessibleObject accessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;
        var result = accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyId);
        Assert.Equal(expected, !result.IsEmpty && (bool)result);
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

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("Test Default Action")]
    public void TrackBarAccessibleObject_DefaultAction_ReturnsExpected(string accessibleDefaultActionDescription)
    {
        using TrackBar trackBar = new();
        trackBar.CreateControl();
        TrackBar.TrackBarAccessibleObject accessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;

        trackBar.AccessibleDefaultActionDescription = accessibleDefaultActionDescription;

        accessibleObject.DefaultAction.Should().Be(accessibleDefaultActionDescription);
    }

    [WinFormsFact]
    public void TrackBarAccessibleObject_DefaultAction_ThrowsArgumentNullException_IfOwnerNotSet()
    {
        Assert.Throws<ArgumentNullException>(() => new TrackBar.TrackBarAccessibleObject(null));
    }

    [WinFormsFact]
    public void TrackBarAccessibleObject_HitTest_ReturnsExpected()
    {
        using TrackBar trackBar = new();
        trackBar.CreateControl();
        TrackBar.TrackBarAccessibleObject accessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;

        Point pointInThumb = new Point(accessibleObject.ThumbAccessibleObject.Bounds.X + 1, accessibleObject.ThumbAccessibleObject.Bounds.Y + 1);
        accessibleObject.HitTest(pointInThumb.X, pointInThumb.Y).Should().Be(accessibleObject.ThumbAccessibleObject);

        if (accessibleObject.FirstButtonAccessibleObject?.IsDisplayed ?? false)
        {
            Point pointInFirstButton = new Point(accessibleObject.FirstButtonAccessibleObject.Bounds.X + 1, accessibleObject.FirstButtonAccessibleObject.Bounds.Y + 1);
            accessibleObject.HitTest(pointInFirstButton.X, pointInFirstButton.Y).Should().Be(accessibleObject.FirstButtonAccessibleObject);
        }

        if (accessibleObject.LastButtonAccessibleObject?.IsDisplayed ?? false)
        {
            Point pointInLastButton = new Point(accessibleObject.LastButtonAccessibleObject.Bounds.X + 1, accessibleObject.LastButtonAccessibleObject.Bounds.Y + 1);
            accessibleObject.HitTest(pointInLastButton.X, pointInLastButton.Y).Should().Be(accessibleObject.LastButtonAccessibleObject);
        }

        accessibleObject.HitTest(-1, -1).Should().BeNull();
    }
}
