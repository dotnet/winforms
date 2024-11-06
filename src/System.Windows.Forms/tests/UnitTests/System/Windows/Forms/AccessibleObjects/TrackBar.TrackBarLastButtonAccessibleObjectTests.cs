// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class TrackBar_TrackBarLastButtonAccessibleObjectTests
{
    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_LastButtonIsDisplayed_TestData))]
    public void TrackBarLastButtonAccessibleObject_Bounds_ReturnsNotEmptyRectangle_IfButtonIsDisplayed(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = GetTrackBarLastButton(trackBar);

        Assert.NotEqual(Rectangle.Empty, accessibleObject.Bounds);
        Assert.True(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_LastButtonIsHidden_TestData))]
    public void TrackBarLastButtonAccessibleObject_Bounds_ReturnsEmptyRectangle_IfButtonIsHidden(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = GetTrackBarLastButton(trackBar);

        Assert.Equal(Rectangle.Empty, accessibleObject.Bounds);
        Assert.True(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_WithoutCreateControl_TestData))]
    public void TrackBarLastButtonAccessibleObject_Bounds_ReturnsEmptyRectangle_IfHandleIsNotCreated(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: false, value, minimum, maximum);
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = GetTrackBarLastButton(trackBar);

        Assert.Equal(Rectangle.Empty, accessibleObject.Bounds);
        Assert.False(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarLastButtonAccessibleObject_DefaultAction_ReturnActionPress(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = GetTrackBarLastButton(trackBar);

        Assert.Equal(SR.AccessibleActionPress, accessibleObject.DefaultAction);
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarLastButtonAccessibleObject_Help_ReturnExpected(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = GetTrackBarLastButton(trackBar);

        Assert.Null(accessibleObject.Help);
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    public static IEnumerable<object[]> TrackBarAccessibleObject_IncreaseButtonName_TestData()
    {
        foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
        {
            foreach (bool rightToLeftLayout in new[] { true, false })
            {
                if (rightToLeft == RightToLeft.Yes && !rightToLeftLayout)
                {
                    continue;
                }

                foreach (bool createControl in new[] { true, false })
                {
                    yield return new object[] { Orientation.Horizontal, rightToLeft, rightToLeftLayout, createControl, /*minumim*/ 0, /*maximum*/ 10, /*value*/ 5 };
                    yield return new object[] { Orientation.Horizontal, rightToLeft, rightToLeftLayout, createControl, /*minumim*/ 0, /*maximum*/ 10, /*value*/ 0 };
                    yield return new object[] { Orientation.Horizontal, rightToLeft, rightToLeftLayout, createControl, /*minumim*/ 0, /*maximum*/ 10, /*value*/ 10 };
                    yield return new object[] { Orientation.Horizontal, rightToLeft, rightToLeftLayout, createControl, /*minumim*/ 0, /*maximum*/ 0, /*value*/ 0 };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(TrackBarAccessibleObject_IncreaseButtonName_TestData))]
    public void TrackBarFirstButtonAccessibleObject_Name_ReturnIncreaseName(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = GetTrackBarLastButton(trackBar);

        Assert.Equal(SR.TrackBarLargeIncreaseButtonName, accessibleObject.Name);
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    public static IEnumerable<object[]> TrackBarAccessibleObject_DecreaseButtonName_TestData()
    {
        foreach (Orientation orientation in Enum.GetValues(typeof(Orientation)))
        {
            foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
            {
                foreach (bool rightToLeftLayout in new[] { true, false })
                {
                    if (orientation == Orientation.Vertical || (rightToLeft == RightToLeft.Yes && !rightToLeftLayout))
                    {
                        foreach (bool createControl in new[] { true, false })
                        {
                            yield return new object[] { orientation, rightToLeft, rightToLeftLayout, createControl, /*minumim*/ 0, /*maximum*/ 10, /*value*/ 5 };
                            yield return new object[] { orientation, rightToLeft, rightToLeftLayout, createControl, /*minumim*/ 0, /*maximum*/ 10, /*value*/ 0 };
                            yield return new object[] { orientation, rightToLeft, rightToLeftLayout, createControl, /*minumim*/ 0, /*maximum*/ 10, /*value*/ 10 };
                            yield return new object[] { orientation, rightToLeft, rightToLeftLayout, createControl, /*minumim*/ 0, /*maximum*/ 0, /*value*/ 0 };
                        }
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(TrackBarAccessibleObject_DecreaseButtonName_TestData))]
    public void TrackBarLLastButtonAccessibleObject_Name_ReturnDecreaseName(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = GetTrackBarLastButton(trackBar);

        Assert.Equal(SR.TrackBarLargeDecreaseButtonName, accessibleObject.Name);
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarLastButtonAccessibleObject_Role_ReturnExpected(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = GetTrackBarLastButton(trackBar);
        AccessibleRole accessibleRole = createControl ? AccessibleRole.PushButton : AccessibleRole.None;

        Assert.Equal(accessibleRole, accessibleObject.Role);
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarLastButtonAccessibleObject_State_ReturnExpected(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = GetTrackBarLastButton(trackBar);
        var bounds = accessibleObject.Bounds;
        AccessibleStates expectedState = accessibleObject.IsDisplayed || !trackBar.IsHandleCreated
                                            ? AccessibleStates.None
                                            : AccessibleStates.Invisible;

        Assert.Equal(expectedState, accessibleObject.State);
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_LastButtonIsHidden_TestData))]
    public void TrackBarLastButtonAccessibleObject_FragmentNavigate_ReturnsNull_IfButtonIsHidden(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
        var trackBarAccessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = trackBarAccessibleObject.LastButtonAccessibleObject;

        Assert.Equal(trackBarAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.True(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_MinimumEqualsMaximum_TestData))]
    public void TrackBarLastButtonAccessibleObject_FragmentNavigate_ReturnsNull_MinimumEqualsMaximum(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
        var trackBarAccessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = trackBarAccessibleObject.LastButtonAccessibleObject;

        Assert.Equal(trackBarAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(trackBarAccessibleObject.ThumbAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.True(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_WithoutCreateControl_TestData))]
    public void TrackBarLastButtonAccessibleObject_FragmentNavigate_ReturnsNull_IfHandleIsNotCreated(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: false, value, minimum, maximum);
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = GetTrackBarLastButton(trackBar);

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_LastButtonIsDisplayed_TestData))]
    public void TrackBarLastButtonAccessibleObject_FragmentNavigate_ReturnsExpected_IfButtonIsDisplayed(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
        var trackBarAccessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = trackBarAccessibleObject.LastButtonAccessibleObject;

        Assert.Equal(trackBarAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(trackBarAccessibleObject.ThumbAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.True(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarLastButtonAccessibleObject_GetChildCount_ReturnExpected(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = GetTrackBarLastButton(trackBar);

        Assert.Equal(-1, accessibleObject.GetChildCount());
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarLastButtonAccessibleObject_GetChild_ReturnExpected(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = GetTrackBarLastButton(trackBar);

        Assert.Null(accessibleObject.GetChild(-1));
        Assert.Null(accessibleObject.GetChild(0));
        Assert.Null(accessibleObject.GetChild(1));
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(Orientation.Vertical, RightToLeft.Yes, true)]
    [InlineData(Orientation.Vertical, RightToLeft.Yes, false)]
    [InlineData(Orientation.Vertical, RightToLeft.No, true)]
    [InlineData(Orientation.Vertical, RightToLeft.No, false)]
    [InlineData(Orientation.Horizontal, RightToLeft.Yes, false)]
    public void TrackBarLastButtonAccessibleObject_Invoke_Decrease_WorkCorrectly(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value: 5, 0, 10);
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = GetTrackBarLastButton(trackBar);

        Assert.Equal(5, trackBar.Value);

        accessibleObject.Invoke();

        Assert.Equal(2, trackBar.Value);

        accessibleObject.Invoke();

        Assert.Equal(0, trackBar.Value);

        accessibleObject.Invoke();

        Assert.Equal(0, trackBar.Value);
        Assert.True(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(Orientation.Horizontal, RightToLeft.No, true)]
    [InlineData(Orientation.Horizontal, RightToLeft.No, false)]
    [InlineData(Orientation.Horizontal, RightToLeft.Yes, true)]
    public void TrackBarLastButtonAccessibleObject_Invoke_Increase_WorkCorrectly(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value: 5, 0, 10);
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = GetTrackBarLastButton(trackBar);

        Assert.Equal(5, trackBar.Value);

        accessibleObject.Invoke();

        Assert.Equal(8, trackBar.Value);

        accessibleObject.Invoke();

        Assert.Equal(10, trackBar.Value);

        accessibleObject.Invoke();

        Assert.Equal(10, trackBar.Value);
        Assert.True(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_WithoutCreateControl_TestData))]
    public void TrackBarLastButtonAccessibleObject_Invoke_DoesNotWork_IfHandleIsNotCreated(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: false, value, minimum, maximum);
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = GetTrackBarLastButton(trackBar);

        Assert.Equal(value, trackBar.Value);

        accessibleObject.Invoke();

        Assert.Equal(value, trackBar.Value);
        Assert.False(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarLastButtonAccessibleObject_InvokePattern_DoesNotSupport(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = GetTrackBarLastButton(trackBar);

        Assert.True(accessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_InvokePatternId));
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarLastButtonAccessibleObject_GetPropertyValue_IsEnabledProperty_ReturnsTrue_OwnerEnabled(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        trackBar.Enabled = true;
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = GetTrackBarLastButton(trackBar);

        Assert.True((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId));
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarLastButtonAccessibleObject_GetPropertyValue_IsEnabledProperty_ReturnsFalse_OwnerDisabled(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        trackBar.Enabled = false;
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = GetTrackBarLastButton(trackBar);

        Assert.False((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId));
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_LastButtonIsDisplayed_TestData))]
    public void TrackBarLastButtonAccessibleObject_IsDisplayed_ReturnsTrue_OwnerVisible(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
        trackBar.Visible = true;
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = GetTrackBarLastButton(trackBar);

        Assert.True(accessibleObject.IsDisplayed);
        Assert.True(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_LastButtonIsDisplayed_TestData))]
    public void TrackBarLastButtonAccessibleObject_IsDisplayed_ReturnsFalse_OwnerInvisible(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
        trackBar.Visible = false;
        TrackBar.TrackBarLastButtonAccessibleObject accessibleObject = GetTrackBarLastButton(trackBar);

        Assert.False(accessibleObject.IsDisplayed);
        Assert.True(trackBar.IsHandleCreated);
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

    private TrackBar.TrackBarLastButtonAccessibleObject GetTrackBarLastButton(TrackBar trackBar)
    {
        TrackBar.TrackBarAccessibleObject trackBarAccessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;

        return trackBarAccessibleObject.LastButtonAccessibleObject;
    }
}
