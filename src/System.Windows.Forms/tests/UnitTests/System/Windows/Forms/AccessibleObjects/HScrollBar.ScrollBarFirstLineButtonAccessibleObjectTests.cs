// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ScrollBar;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class HScrollBar_ScrollBarFirstLineButtonAccessibleObjectTests
{
    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
    public void HScrollBarFirstLineButtonAccessibleObject_GetChild_ReturnNull(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

        Assert.Null(accessibleObject.GetChild(-1));
        Assert.Null(accessibleObject.GetChild(0));
        Assert.Null(accessibleObject.GetChild(1));
        Assert.Equal(createControl, scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
    public void HScrollBarFirstLineButtonAccessibleObject_GetChildCount_ReturnMinusOne(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

        Assert.Equal(-1, accessibleObject.GetChildCount());
        Assert.Equal(createControl, scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
    public void HScrollBarFirstLineButtonAccessibleObject_DefaultAction_ReturnNull_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

        Assert.Null(accessibleObject.DefaultAction);
        Assert.False(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
    public void HScrollBarFirstLineButtonAccessibleObject_DefaultAction_ReturnNull_HandleIsCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

        Assert.NotNull(accessibleObject.DefaultAction);
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
    public void HScrollBarFirstLineButtonAccessibleObject_Description_ReturnNull_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

        Assert.Null(accessibleObject.Description);
        Assert.False(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
    public void HScrollBarFirstLineButtonAccessibleObject_Description_ReturnNull_HandleIsCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

        Assert.NotNull(accessibleObject.Description);
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
    public void HScrollBarFirstLineButtonAccessibleObject_State_ReturnExpected(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

        Assert.Equal(AccessibleStates.None, accessibleObject.State);
        Assert.Equal(createControl, scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
    public void HScrollBarFirstLineButtonAccessibleObject_Name_ReturnNull_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

        Assert.Null(accessibleObject.Name);
        Assert.False(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
    public void HScrollBarFirstLineButtonAccessibleObject_Name_ReturnNull_HandleIsCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

        Assert.NotNull(accessibleObject.Name);
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
    public void HScrollBarFirstLineButtonAccessibleObject_Role_ReturnNotNull(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);
        AccessibleRole expectedRole = createControl ? AccessibleRole.PushButton : AccessibleRole.None;

        Assert.Equal(expectedRole, accessibleObject.Role);
        Assert.Equal(createControl, scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
    public void HScrollBarFirstLineButtonAccessibleObject_InvokePattern_Supported(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

        Assert.Equal(createControl, accessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_InvokePatternId));
        Assert.Equal(createControl, scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.HScrollBarAccessibleObject_FirstPageButtonIsDisplayed_TestData))]
    public void HScrollBarFirstLineButtonAccessibleObject_FragmentNavigate_ReturnExpected_FirstPageButtonIsDisplayed(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        var scrollBarAccessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

        Assert.Equal(scrollBarAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(scrollBarAccessibleObject.FirstPageButtonAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.HScrollBarAccessibleObject_FirstPageButtonIsHidden_TestData))]
    public void HScrollBarFirstLineButtonAccessibleObject_FragmentNavigate_ReturnExpected_FirstPageButtonIsHidden(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        var scrollBarAccessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

        Assert.Equal(scrollBarAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(scrollBarAccessibleObject.ThumbAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_MinimumEqualsMaximum_TestData))]
    public void HScrollBarFirstLineButtonAccessibleObject_FragmentNavigate_ReturnExpected_MinimumEqualsMaximum(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        var scrollBarAccessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

        Assert.Equal(scrollBarAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(scrollBarAccessibleObject.ThumbAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
    public void HScrollBarFirstLineButtonAccessibleObject_FragmentNavigate_ReturnNull_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
        var scrollBarAccessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(scrollBar.IsHandleCreated);
    }

    [WinFormsFact]
    public void HScrollBarFirstLineButtonAccessibleObject_Invoke_IncreaseValue()
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: true, RightToLeft.Yes, minimum: 0, maximum: 100, value: 50);
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

        Assert.Equal(50, scrollBar.Value);

        accessibleObject.Invoke();

        Assert.Equal(51, scrollBar.Value);

        accessibleObject.Invoke();

        Assert.Equal(52, scrollBar.Value);
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsFact]
    public void HScrollBarFirstLineButtonAccessibleObject_Invoke_DecreaseValue()
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: true, RightToLeft.No, minimum: 0, maximum: 100, value: 50);
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

        Assert.Equal(50, scrollBar.Value);

        accessibleObject.Invoke();

        Assert.Equal(49, scrollBar.Value);

        accessibleObject.Invoke();

        Assert.Equal(48, scrollBar.Value);
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.No, 0, 100)]
    [InlineData(RightToLeft.No, 0, 0)]
    [InlineData(RightToLeft.Yes, 99, 100)]
    [InlineData(RightToLeft.Yes, 0, 0)]
    public void HScrollBarFirstLineButtonAccessibleObject_Invoke_DoesNotWork_OutOfRange(RightToLeft rightToLeft, int value, int maximum)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: true, rightToLeft, minimum: 0, maximum, value);
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

        Assert.Equal(value, scrollBar.Value);

        accessibleObject.Invoke();

        Assert.Equal(value, scrollBar.Value);
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
    public void HScrollBarFirstLineButtonAccessibleObject_Invoke_DoesNotWork_IfHandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

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
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

        Assert.Equal(value, scrollBar.Value);

        accessibleObject.Invoke();

        Assert.Equal(value, scrollBar.Value);
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
    public void HScrollBarFirstLineButtonAccessibleObject_GetPropertyValue_IsEnabledProperty_ReturnsTrue_OwnerEnabled(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
        scrollBar.Enabled = true;
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

        Assert.True((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId));
        Assert.Equal(createControl, scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
    public void HScrollBarFirstLineButtonAccessibleObject_GetPropertyValue_IsEnabledProperty_ReturnsFalse_OwnerDisabled(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
        scrollBar.Enabled = false;
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

        Assert.False((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId));
        Assert.Equal(createControl, scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
    public void HScrollBarFirstLineButtonAccessibleObject_IsDisplayed_ReturnsTrue_OwnerVisible(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
        scrollBar.Visible = true;
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

        Assert.True(accessibleObject.IsDisplayed);
        Assert.Equal(createControl, scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
    public void HScrollBarFirstLineButtonAccessibleObject_IsDisplayed_ReturnsFalse_OwnerInvisible(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using HScrollBar scrollBar = GetHScrollBar(createControl, rightToLeft, minimum, maximum, value);
        scrollBar.Visible = false;
        ScrollBarFirstLineButtonAccessibleObject accessibleObject = GetFirstLineButton(scrollBar);

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
            SmallChange = 1,
            LargeChange = 2,
            Size = new Size(500, 50)
        };

        if (createControl)
        {
            hScrollBar.CreateControl();
        }

        return hScrollBar;
    }

    private ScrollBarFirstLineButtonAccessibleObject GetFirstLineButton(ScrollBar scrollBar)
    {
        var accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
        return accessibleObject.FirstLineButtonAccessibleObject;
    }
}
