﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.TestUtilities;
using static Interop;

namespace System.Windows.Forms.Tests;

public class TrackBar_TrackBarFirstButtonAccessibleObjectTests
{
    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_FirstButtonIsDisplayed_TestData))]
    public void TrackBarFirstButtonAccessibleObject_Bounds_ReturnsNotEmptyRectangle_IfButtonIsDisplayed(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = GetTrackBarFirstButton(trackBar);

        Assert.NotEqual(Rectangle.Empty, accessibleObject.Bounds);
        Assert.True(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_FirstButtonIsHidden_TestData))]
    public void TrackBarFirstButtonAccessibleObject_Bounds_ReturnsEmptyRectangle_IfButtonIsHidden(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = GetTrackBarFirstButton(trackBar);

        Assert.Equal(Rectangle.Empty, accessibleObject.Bounds);
        Assert.True(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_WithoutCreateControl_TestData))]
    public void TrackBarFirstButtonAccessibleObject_Bounds_ReturnsEmptyRectangle_IfHandleIsNotCreated(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: false, value, minimum, maximum);
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = GetTrackBarFirstButton(trackBar);

        Assert.Equal(Rectangle.Empty, accessibleObject.Bounds);
        Assert.False(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarFirstButtonAccessibleObject_DefaultAction_ReturnActionPress(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = GetTrackBarFirstButton(trackBar);

        Assert.Equal(SR.AccessibleActionPress, accessibleObject.DefaultAction);
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarFirstButtonAccessibleObject_Help_ReturnExpected(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = GetTrackBarFirstButton(trackBar);

        Assert.Null(accessibleObject.Help);
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    public static IEnumerable<object[]> TrackBarAccessibleObject_DecreaseButtonName_TestData()
    {
        foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
        {
            foreach (bool rightToLeftLayout in new[] { true, false })
            {
                if (rightToLeft == RightToLeft.Yes && rightToLeftLayout == false)
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
    [MemberData(nameof(TrackBarAccessibleObject_DecreaseButtonName_TestData))]
    public void TrackBarFirstButtonAccessibleObject_Name_ReturnDescreaseName(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = GetTrackBarFirstButton(trackBar);

        Assert.Equal(SR.TrackBarLargeDecreaseButtonName, accessibleObject.Name);
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    public static IEnumerable<object[]> TrackBarAccessibleObject_IncreaseButtonName_TestData()
    {
        foreach (Orientation orientation in Enum.GetValues(typeof(Orientation)))
        {
            foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
            {
                foreach (bool rightToLeftLayout in new[] { true, false })
                {
                    if (orientation == Orientation.Vertical || (rightToLeft == RightToLeft.Yes && rightToLeftLayout == false))
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
    [MemberData(nameof(TrackBarAccessibleObject_IncreaseButtonName_TestData))]
    public void TrackBarFirstButtonAccessibleObject_Name_ReturnIncreaseName(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = GetTrackBarFirstButton(trackBar);

        Assert.Equal(SR.TrackBarLargeIncreaseButtonName, accessibleObject.Name);
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarFirstButtonAccessibleObject_Role_ReturnExpected(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = GetTrackBarFirstButton(trackBar);
        AccessibleRole accessibleRole = createControl ? AccessibleRole.PushButton : AccessibleRole.None;

        Assert.Equal(accessibleRole, accessibleObject.Role);
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarFirstButtonAccessibleObject_State_ReturnExpected(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = GetTrackBarFirstButton(trackBar);
        AccessibleStates expectedState = accessibleObject.IsDisplayed || !trackBar.IsHandleCreated
                                         ? AccessibleStates.None
                                         : AccessibleStates.Invisible;

        Assert.Equal(expectedState, accessibleObject.State);
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_FirstButtonIsHidden_TestData))]
    public void TrackBarFirstButtonAccessibleObject_FragmentNavigate_ReturnsNull_IfFirstButtonIsHidden(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
        var trackBarAccessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = trackBarAccessibleObject.FirstButtonAccessibleObject;

        Assert.Equal(trackBarAccessibleObject, accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.Parent));
        Assert.Null(accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.LastChild));
        Assert.True(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_MinimumEqualsMaximum_TestData))]
    public void TrackBarFirstButtonAccessibleObject_FragmentNavigate_ReturnsNull_MinimumEqualsMaximum(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
        var trackBarAccessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = trackBarAccessibleObject.FirstButtonAccessibleObject;

        Assert.Equal(trackBarAccessibleObject, accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.Parent));
        Assert.Null(accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.LastChild));
        Assert.True(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_FirstButtonIsDisplayed_TestData))]
    public void TrackBarFirstButtonAccessibleObject_FragmentNavigate_ReturnsExpected_IfFirstButtonIsDisplayed(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
        var trackBarAccessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = trackBarAccessibleObject.FirstButtonAccessibleObject;

        Assert.Equal(trackBarAccessibleObject, accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.Parent));
        Assert.Equal(trackBarAccessibleObject.ThumbAccessibleObject, accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.LastChild));
        Assert.True(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_WithoutCreateControl_TestData))]
    public void TrackBarFirstButtonAccessibleObject_FragmentNavigate_ReturnsNull_IfHandleIsNotCreated(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: false, value, minimum, maximum);
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = GetTrackBarFirstButton(trackBar);

        Assert.Null(accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.Parent));
        Assert.Null(accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.LastChild));
        Assert.False(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarFirstButtonAccessibleObject_GetChildCount_ReturnExpected(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = GetTrackBarFirstButton(trackBar);

        Assert.Equal(-1, accessibleObject.GetChildCount());
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarFirstButtonAccessibleObject_GetChild_ReturnExpected(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = GetTrackBarFirstButton(trackBar);

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
    public void TrackBarFirstButtonAccessibleObject_Invoke_Increase_WorkCorrectly(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value: 5, 0, 10);
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = GetTrackBarFirstButton(trackBar);

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
    [InlineData(Orientation.Horizontal, RightToLeft.No, true)]
    [InlineData(Orientation.Horizontal, RightToLeft.No, false)]
    [InlineData(Orientation.Horizontal, RightToLeft.Yes, true)]
    public void TrackBarFirstButtonAccessibleObject_Invoke_Decrease_WorkCorrectly(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value: 5, 0, 10);
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = GetTrackBarFirstButton(trackBar);

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
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_WithoutCreateControl_TestData))]
    public void TrackBarFirstButtonAccessibleObject_Invoke_DoesNotWork_IfHandleIsNotCreated(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: false, value, minimum, maximum);
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = GetTrackBarFirstButton(trackBar);

        Assert.Equal(value, trackBar.Value);

        accessibleObject.Invoke();

        Assert.Equal(value, trackBar.Value);
        Assert.False(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarFirstButtonAccessibleObject_InvokePattern_DoesNotSupport(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = GetTrackBarFirstButton(trackBar);

        Assert.True(accessibleObject.IsPatternSupported(Interop.UiaCore.UIA.InvokePatternId));
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarFirstButtonAccessibleObject_GetPropertyValue_IsEnabledProperty_ReturnsTrue_OwnerEnabled(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        trackBar.Enabled = true;
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = GetTrackBarFirstButton(trackBar);

        Assert.True((bool)accessibleObject.GetPropertyValue(UiaCore.UIA.IsEnabledPropertyId));
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_TestData))]
    public void TrackBarFirstButtonAccessibleObject_GetPropertyValue_IsEnabledProperty_ReturnsFalse_OwnerDisabled(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, bool createControl, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl, value, minimum, maximum);
        trackBar.Enabled = false;
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = GetTrackBarFirstButton(trackBar);

        Assert.False((bool)accessibleObject.GetPropertyValue(UiaCore.UIA.IsEnabledPropertyId));
        Assert.Equal(createControl, trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_FirstButtonIsDisplayed_TestData))]
    public void TrackBarFirstButtonAccessibleObject_IsDisplayed_ReturnsTrue_OwnerVisible(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
        trackBar.Visible = true;
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = GetTrackBarFirstButton(trackBar);

        Assert.True(accessibleObject.IsDisplayed);
        Assert.True(trackBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(TrackBarTestHelper), nameof(TrackBarTestHelper.TrackBarAccessibleObject_FirstButtonIsDisplayed_TestData))]
    public void TrackBarFirstButtonAccessibleObject_IsDisplayed_ReturnsFalse_OwnerInvisible(Orientation orientation, RightToLeft rightToLeft, bool rightToLeftLayout, int minimum, int maximum, int value)
    {
        using TrackBar trackBar = GetTrackBar(orientation, rightToLeft, rightToLeftLayout, createControl: true, value, minimum, maximum);
        trackBar.Visible = false;
        TrackBar.TrackBarFirstButtonAccessibleObject accessibleObject = GetTrackBarFirstButton(trackBar);

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

    private TrackBar.TrackBarFirstButtonAccessibleObject GetTrackBarFirstButton(TrackBar trackBar)
    {
        TrackBar.TrackBarAccessibleObject trackBarAccessibleObject = (TrackBar.TrackBarAccessibleObject)trackBar.AccessibilityObject;

        return trackBarAccessibleObject.FirstButtonAccessibleObject;
    }
}
