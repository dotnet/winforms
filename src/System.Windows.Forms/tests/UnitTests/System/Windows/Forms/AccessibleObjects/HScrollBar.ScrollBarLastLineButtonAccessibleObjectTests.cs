// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ScrollBar;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class HScrollBar_ScrollBarLastLineButtonAccessibleObjectTests
{
    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
    public void HScrollBarLastLineButtonAccessibleObject_GetChild_ReturnNull(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.Null(accessibleObject.GetChild(-1));
        Assert.Null(accessibleObject.GetChild(0));
        Assert.Null(accessibleObject.GetChild(1));
        Assert.Equal(createControl, scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
    public void HScrollBarLastLineButtonAccessibleObject_GetChildCount_ReturnMinusOne(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.Equal(-1, accessibleObject.GetChildCount());
        Assert.Equal(createControl, scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
    public void HScrollBarLastLineButtonAccessibleObject_DefaultAction_ReturnNotNull_HandleIsCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.NotEmpty(accessibleObject.DefaultAction);
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
    public void HScrollBarLastLineButtonAccessibleObject_DefaultAction_ReturnNull_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.Null(accessibleObject.DefaultAction);
        Assert.False(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
    public void HScrollBarLastLineButtonAccessibleObject_Description_ReturnNotNull_HandleIsCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.NotEmpty(accessibleObject.Description);
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
    public void HScrollBarLastLineButtonAccessibleObject_Description_ReturnNull_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.Null(accessibleObject.Description);
        Assert.False(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
    public void HScrollBarLastLineButtonAccessibleObject_Name_ReturnNotNull_HandleIsCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.NotEmpty(accessibleObject.Name);
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
    public void HScrollBarLastLineButtonAccessibleObject_Name_ReturnNull_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.Null(accessibleObject.Name);
        Assert.False(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
    public void HScrollBarLastLineButtonAccessibleObject_State_ReturnExpected(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.Equal(AccessibleStates.None, accessibleObject.State);
        Assert.Equal(createControl, scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
    public void HScrollBarLastLineButtonAccessibleObject_Role_ReturnPushButton_HandleIsCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.Equal(AccessibleRole.PushButton, accessibleObject.Role);
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
    public void HScrollBarLastLineButtonAccessibleObject_Role_ReturnNone_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.Equal(AccessibleRole.None, accessibleObject.Role);
        Assert.False(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
    public void HScrollBarLastLineButtonAccessibleObject_InvokePattern_Supported(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.Equal(createControl, accessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_InvokePatternId));
        Assert.Equal(createControl, scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.HScrollBarAccessibleObject_LastPageButtonIsDisplayed_TestData))]
    public void HScrollBarLastLineButtonAccessibleObject_FragmentNavigate_ReturnsExpected_LastPageButtonIsDisplayed(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        var scrollBarAccessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.Equal(scrollBarAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(scrollBarAccessibleObject.LastPageButtonAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.HScrollBarAccessibleObject_LastPageButtonIsHidden_TestData))]
    public void HScrollBarLastLineButtonAccessibleObject_FragmentNavigate_ReturnsExpected_LastPageButtonIsHidden(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        var scrollBarAccessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.Equal(scrollBarAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(scrollBarAccessibleObject.ThumbAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_MinimumEqualsMaximum_TestData))]
    public void HScrollBarLastLineButtonAccessibleObject_FragmentNavigate_ReturnsExpected_MinimumEqualsMaximum(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        var scrollBarAccessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.Equal(scrollBarAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(scrollBarAccessibleObject.ThumbAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.HScrollBarAccessibleObject_LastPageButtonIsHidden_TestData))]
    public void HScrollBarLastLineButtonAccessibleObject_FragmentNavigate_ReturnsExpected_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
        var scrollBarAccessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(scrollBar.IsHandleCreated);
    }

    [WinFormsFact]
    public void HScrollBarLastLineButtonAccessibleObject_Invoke_DecreaseValue()
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: true, RightToLeft.Yes, minimum: 0, maximum: 100, value: 50);
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.Equal(50, scrollBar.Value);

        accessibleObject.Invoke();

        Assert.Equal(49, scrollBar.Value);

        accessibleObject.Invoke();

        Assert.Equal(48, scrollBar.Value);
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsFact]
    public void HScrollBarLastLineButtonAccessibleObject_Invoke_IncreaseValue()
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: true, RightToLeft.No, minimum: 0, maximum: 100, value: 50);
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.Equal(50, scrollBar.Value);

        accessibleObject.Invoke();

        Assert.Equal(51, scrollBar.Value);

        accessibleObject.Invoke();

        Assert.Equal(52, scrollBar.Value);
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.No, 99, 100)]
    [InlineData(RightToLeft.No, 0, 0)]
    [InlineData(RightToLeft.Yes, 0, 100)]
    [InlineData(RightToLeft.Yes, 0, 0)]
    public void HScrollBarLastLineButtonAccessibleObject_Invoke_DoesNotWork_OutOfRange(RightToLeft rightToLeft, int value, int maximum)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum: 0, maximum, value);
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.Equal(value, scrollBar.Value);

        accessibleObject.Invoke();

        Assert.Equal(value, scrollBar.Value);
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
    public void HScrollBarLastLineButtonAccessibleObject_Invoke_DoesNotWork_IfHandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.Equal(value, scrollBar.Value);

        accessibleObject.Invoke();

        Assert.Equal(value, scrollBar.Value);
        Assert.False(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_MinimumEqualsMaximum_TestData))]
    public void HScrollBarLinePageButtonAccessibleObject_Invoke_DoesNotWork_MinimumEqualsMaximum(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.Equal(value, scrollBar.Value);

        accessibleObject.Invoke();

        Assert.Equal(value, scrollBar.Value);
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
    public void HScrollBarLastLineButtonAccessibleObject_GetPropertyValue_IsEnabledProperty_ReturnsTrue_OwnerEnabled(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
        scrollBar.Enabled = true;
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.True((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId));
        Assert.Equal(createControl, scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
    public void HScrollBarLastLineButtonAccessibleObject_GetPropertyValue_IsEnabledProperty_ReturnsFalse_OwnerDisabled(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
        scrollBar.Enabled = false;
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.False((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId));
        Assert.Equal(createControl, scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
    public void HScrollBarLastLineButtonAccessibleObject_IsDisplayed_ReturnsTrue_OwnerVisible(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
        scrollBar.Visible = true;
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.True(accessibleObject.IsDisplayed);
        Assert.Equal(createControl, scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
    public void HScrollBarLastLineButtonAccessibleObject_IsDisplayed_ReturnsFalse_OwnerInvisible(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
        scrollBar.Visible = false;
        ScrollBarLastLineButtonAccessibleObject accessibleObject = GetLastLineButton(scrollBar);

        Assert.False(accessibleObject.IsDisplayed);
        Assert.Equal(createControl, scrollBar.IsHandleCreated);
    }

    private HScrollBar GetHScrollBar(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        HScrollBar hScrollBar = new()
        {
            RightToLeft = rightToLeft,
            Value = value,
            Minimum = minimum,
            Maximum = maximum,
            Size = new Size(500, 50),
            SmallChange = 1,
            LargeChange = 2
        };

        if (createControl)
        {
            hScrollBar.CreateControl();
        }

        return hScrollBar;
    }

    private ScrollBarLastLineButtonAccessibleObject GetLastLineButton(ScrollBar scrollBar)
    {
        var accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
        return accessibleObject.LastLineButtonAccessibleObject;
    }
}
