// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class TrackBar_TrackBarThumbAccessibleObjectTests
{
    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_WithoutCreateControl_TestData))]
    public void TrackBarThumbAccessibleObject_Bounds_ReturnsEmptyRectangle_IfHandleIsNotCreated(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: false, value, minimum, maximum);
        TrackBar.TrackBarThumbAccessibleObject accessibleObject = GetTrackBarThumb(trackBar);

        Assert.Equal(Rectangle.Empty, accessibleObject.Bounds);
        Assert.False(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_WithoutCreateControl_TestData))]
    public void TrackBarThumbAccessibleObject_Bounds_ReturnsRectangle_IfHandleIsCreated(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
        TrackBar.TrackBarThumbAccessibleObject accessibleObject = GetTrackBarThumb(trackBar);

        Assert.NotEqual(Rectangle.Empty, accessibleObject.Bounds);
        Assert.True(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarThumbAccessibleObject_DefaultAction_ReturnActionPress(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarThumbAccessibleObject accessibleObject = GetTrackBarThumb(trackBar);

        Assert.Null(accessibleObject.DefaultAction);
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarThumbAccessibleObject_Help_ReturnExpected(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarThumbAccessibleObject accessibleObject = GetTrackBarThumb(trackBar);

        Assert.Null(accessibleObject.Help);
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarThumbAccessibleObject_Name_ReturnExpected(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarThumbAccessibleObject accessibleObject = GetTrackBarThumb(trackBar);

        Assert.Equal(SR.TrackBarPositionButtonName, accessibleObject.Name);
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarThumbAccessibleObject_Role_ReturnExpected(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarThumbAccessibleObject accessibleObject = GetTrackBarThumb(trackBar);
        AccessibleRole accessibleRole = createControl ? AccessibleRole.Indicator : AccessibleRole.None;

        Assert.Equal(accessibleRole, accessibleObject.Role);
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarThumbAccessibleObject_State_ReturnExpected(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarThumbAccessibleObject accessibleObject = GetTrackBarThumb(trackBar);
        Assert.Equal(AccessibleStates.None, accessibleObject.State);
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_ButtonsAreDisplayed_TestData))]
    public void TrackBarThumbAccessibleObject_FragmentNavigate_ReturnExpected_IfButtonsIsDisplayed(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
        var trackBarAccessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;
        TrackBar.TrackBarThumbAccessibleObject accessibleObject = trackBarAccessibleObject.ThumbAccessibleObject;

        Assert.Equal(trackBarAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(trackBarAccessibleObject.LastButtonAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(trackBarAccessibleObject.FirstButtonAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.True(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_FirstButtonIsHidden_TestData))]
    public void TrackBarThumbAccessibleObject_FragmentNavigate_ReturnExpected_IfFirstButtonIsDisplayed(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
        var trackBarAccessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;
        TrackBar.TrackBarThumbAccessibleObject accessibleObject = trackBarAccessibleObject.ThumbAccessibleObject;

        Assert.Equal(trackBarAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(trackBarAccessibleObject.LastButtonAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.True(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_LastButtonIsHidden_TestData))]
    public void TrackBarThumbAccessibleObject_FragmentNavigate_ReturnExpected_IfLastButtonIsDisplayed(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
        var trackBarAccessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;
        TrackBar.TrackBarThumbAccessibleObject accessibleObject = trackBarAccessibleObject.ThumbAccessibleObject;

        Assert.Equal(trackBarAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(trackBarAccessibleObject.FirstButtonAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.True(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_WithoutCreateControl_TestData))]
    public void TrackBarThumbAccessibleObject_FragmentNavigate_ReturnNull_IfHandleIsNotCreated(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: false, value, minimum, maximum);
        var trackBarAccessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;
        TrackBar.TrackBarThumbAccessibleObject accessibleObject = trackBarAccessibleObject.ThumbAccessibleObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarThumbAccessibleObject_GetChildCount_ReturnExpected(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarThumbAccessibleObject accessibleObject = GetTrackBarThumb(trackBar);

        Assert.Equal(-1, accessibleObject.GetChildCount());
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarThumbAccessibleObject_GetChild_ReturnExpected(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarThumbAccessibleObject accessibleObject = GetTrackBarThumb(trackBar);

        Assert.Null(accessibleObject.GetChild(-1));
        Assert.Null(accessibleObject.GetChild(0));
        Assert.Null(accessibleObject.GetChild(1));
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarThumbAccessibleObject_InvokePattern_DoesNotSupport(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarThumbAccessibleObject accessibleObject = GetTrackBarThumb(trackBar);

        Assert.False(accessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_InvokePatternId));
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarThumbAccessibleObject_GetPropertyValue_IsEnabledProperty_ReturnsTrue_OwnerEnabled(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        trackBar.Enabled = true;
        TrackBar.TrackBarThumbAccessibleObject accessibleObject = GetTrackBarThumb(trackBar);

        Assert.True((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId));
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarThumbAccessibleObject_GetPropertyValue_IsEnabledProperty_ReturnsFalse_OwnerDisabled(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        trackBar.Enabled = false;
        TrackBar.TrackBarThumbAccessibleObject accessibleObject = GetTrackBarThumb(trackBar);

        Assert.False((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId));
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarThumbAccessibleObject_IsDisplayed_ReturnsTrue_OwnerVisible(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        trackBar.Visible = true;
        TrackBar.TrackBarThumbAccessibleObject accessibleObject = GetTrackBarThumb(trackBar);

        Assert.True(accessibleObject.IsDisplayed);
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarThumbAccessibleObject_IsDisplayed_ReturnsFalse_OwnerInvisible(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        trackBar.Visible = false;
        TrackBar.TrackBarThumbAccessibleObject accessibleObject = GetTrackBarThumb(trackBar);

        Assert.False(accessibleObject.IsDisplayed);
        Assert.Equal(createControl, trackBar.IsHandleCreated);
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

    private TrackBar.TrackBarThumbAccessibleObject GetTrackBarThumb(TrackBar trackBar)
    {
        TrackBar.TrackBarAccessibleObject trackBarAccessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;

        return trackBarAccessibleObject.ThumbAccessibleObject;
    }
}
