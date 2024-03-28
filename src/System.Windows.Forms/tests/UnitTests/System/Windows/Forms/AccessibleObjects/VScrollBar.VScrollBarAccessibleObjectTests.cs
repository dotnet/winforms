// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ScrollBar;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class VScrollBar_VScrollBarAccessibleObjectTests
{
    [WinFormsFact]
    public void VScrollBarAccessibleObject_ctor_ThrowsException_IfVScrollBarAccessibleObjectIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new ScrollBarAccessibleObject(null));
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.ScrollBar)]
    [InlineData(false, AccessibleRole.None)]
    public void VScrollBarAccessibleObject_Ctor_Default(bool createControl, AccessibleRole accessibleRole)
    {
        using VScrollBar scrollBar = new();

        if (createControl)
        {
            scrollBar.CreateControl();
        }

        AccessibleObject accessibleObject = scrollBar.AccessibilityObject;

        Assert.NotNull(accessibleObject);
        Assert.Equal(accessibleRole, accessibleObject.Role);
        Assert.Equal(createControl, scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Test")]
    public void VScrollBarAccessibleObject_Description_Get_ReturnsExpected(string accessibleDescription)
    {
        using VScrollBar scrollBar = new();
        scrollBar.AccessibleDescription = accessibleDescription;
        ScrollBarAccessibleObject accessibleObject =
            Assert.IsType<ScrollBarAccessibleObject>(scrollBar.AccessibilityObject);

        Assert.Equal(accessibleDescription, accessibleObject.Description);
        Assert.False(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Test")]
    public void VScrollBarAccessibleObject_Name_Get_ReturnsExpected(string accessibleName)
    {
        using VScrollBar scrollBar = new();
        scrollBar.AccessibleName = accessibleName;
        ScrollBarAccessibleObject accessibleObject =
            Assert.IsType<ScrollBarAccessibleObject>(scrollBar.AccessibilityObject);

        Assert.Equal(accessibleName, accessibleObject.Name);
        Assert.False(scrollBar.IsHandleCreated);
    }

    [WinFormsFact]
    public void VScrollBarAccessibleObject_ControlType_IsScrollBar_IfAccessibleRoleIsDefault()
    {
        using VScrollBar scrollBar = new();
        // AccessibleRole is not set = Default

        var actual = (UIA_CONTROLTYPE_ID)(int)scrollBar.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ScrollBarControlTypeId, actual);
        Assert.False(scrollBar.IsHandleCreated);
    }

    public static IEnumerable<object[]> VScrollBarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(VScrollBarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void VScrollBarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using VScrollBar scrollBar = new();
        scrollBar.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)scrollBar.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_TestData))]
    public void VScrollBarAccessibleObject_FragmentNavigate_Child_ReturnExpected(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using VScrollBar scrollBar = GetVScrollBar(createControl, rightToLeft, minimum, maximum, value);
        var accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;

        Assert.Equal(accessibleObject.FirstLineButtonAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject.LastLineButtonAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Equal(createControl, scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_BothButtonAreDisplayed_TestData))]
    public void VScrollBarAccessibleObject_GetChildCount_ReturnsFive_AllButtonsAreDisplayed(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        ScrollBarAccessibleObject accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;

        Assert.Equal(5, accessibleObject.GetChildCount());
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.VScrollBarAccessibleObject_FirstPageButtonIsHidden_TestData))]
    public void VScrollBarAccessibleObject_GetChildCount_ReturnsFour_FirstPageButtonIsHidden(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        ScrollBarAccessibleObject accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;

        Assert.Equal(4, accessibleObject.GetChildCount());
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.VScrollBarAccessibleObject_LastPageButtonIsHidden_TestData))]
    public void VScrollBarAccessibleObject_GetChildCount_ReturnsFour_LastPageButtonIsHidden(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        ScrollBarAccessibleObject accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;

        Assert.Equal(4, accessibleObject.GetChildCount());
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_MinimumEqualsMaximum_TestData))]
    public void VScrollBarAccessibleObject_GetChildCount_ReturnsThree_MinimumEqualsMaximum(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        ScrollBarAccessibleObject accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;

        Assert.Equal(3, accessibleObject.GetChildCount());
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_BothButtonAreDisplayed_TestData))]
    public void VScrollBarAccessibleObject_GetChild_ReturnExpected_AllButtonsAreDisplayed(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        ScrollBarAccessibleObject accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
        Assert.Null(accessibleObject.GetChild(-1));
        Assert.Equal(accessibleObject.FirstLineButtonAccessibleObject, accessibleObject.GetChild(0));
        Assert.Equal(accessibleObject.FirstPageButtonAccessibleObject, accessibleObject.GetChild(1));
        Assert.Equal(accessibleObject.ThumbAccessibleObject, accessibleObject.GetChild(2));
        Assert.Equal(accessibleObject.LastPageButtonAccessibleObject, accessibleObject.GetChild(3));
        Assert.Equal(accessibleObject.LastLineButtonAccessibleObject, accessibleObject.GetChild(4));
        Assert.Null(accessibleObject.GetChild(5));
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.VScrollBarAccessibleObject_FirstPageButtonIsHidden_TestData))]
    public void VScrollBarAccessibleObject_GetChild_ReturnExpected_FirstButtonIsHidden(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        ScrollBarAccessibleObject accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
        Assert.Null(accessibleObject.GetChild(-1));
        Assert.Equal(accessibleObject.FirstLineButtonAccessibleObject, accessibleObject.GetChild(0));
        Assert.Equal(accessibleObject.ThumbAccessibleObject, accessibleObject.GetChild(1));
        Assert.Equal(accessibleObject.LastPageButtonAccessibleObject, accessibleObject.GetChild(2));
        Assert.Equal(accessibleObject.LastLineButtonAccessibleObject, accessibleObject.GetChild(3));
        Assert.Null(accessibleObject.GetChild(4));
        Assert.Null(accessibleObject.GetChild(5));
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.VScrollBarAccessibleObject_LastPageButtonIsHidden_TestData))]
    public void VScrollBarAccessibleObject_GetChild_ReturnExpected_LastButtonIsHidden(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        ScrollBarAccessibleObject accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
        Assert.Null(accessibleObject.GetChild(-1));
        Assert.Equal(accessibleObject.FirstLineButtonAccessibleObject, accessibleObject.GetChild(0));
        Assert.Equal(accessibleObject.FirstPageButtonAccessibleObject, accessibleObject.GetChild(1));
        Assert.Equal(accessibleObject.ThumbAccessibleObject, accessibleObject.GetChild(2));
        Assert.Equal(accessibleObject.LastLineButtonAccessibleObject, accessibleObject.GetChild(3));
        Assert.Null(accessibleObject.GetChild(4));
        Assert.Null(accessibleObject.GetChild(5));
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_MinimumEqualsMaximum_TestData))]
    public void VScrollBarAccessibleObject_GetChild_ReturnExpected_MinimumEqualsMaximum(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using VScrollBar scrollBar = GetVScrollBar(createControl: true, rightToLeft, minimum, maximum, value);
        ScrollBarAccessibleObject accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
        Assert.Null(accessibleObject.GetChild(-1));
        Assert.Equal(accessibleObject.FirstLineButtonAccessibleObject, accessibleObject.GetChild(0));
        Assert.Equal(accessibleObject.ThumbAccessibleObject, accessibleObject.GetChild(1));
        Assert.Equal(accessibleObject.LastLineButtonAccessibleObject, accessibleObject.GetChild(2));
        Assert.Null(accessibleObject.GetChild(3));
        Assert.Null(accessibleObject.GetChild(4));
        Assert.Null(accessibleObject.GetChild(5));
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ScrollBarTestHelper), nameof(ScrollBarTestHelper.ScrollBarAccessibleObject_WithoutCreateControl_TestData))]
    public void VScrollBarAccessibleObject_GetChild_ReturnExpected_HandleIsNotCreated(RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        using VScrollBar scrollBar = GetVScrollBar(createControl: false, rightToLeft, minimum, maximum, value);
        ScrollBarAccessibleObject accessibleObject = (ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
        Assert.Null(accessibleObject.GetChild(-1));
        Assert.Null(accessibleObject.GetChild(0));
        Assert.Null(accessibleObject.GetChild(1));
        Assert.Null(accessibleObject.GetChild(2));
        Assert.Null(accessibleObject.GetChild(3));
        Assert.Null(accessibleObject.GetChild(4));
        Assert.Null(accessibleObject.GetChild(5));
        Assert.False(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void VScrollBarAccessibilityObject_GetPropertyValue_IsEnabledProperty_ReturnsExpected(bool enabled)
    {
        using VScrollBar scrollBar = new()
        {
            Enabled = enabled
        };

        Assert.Equal(enabled, (bool)scrollBar.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId));
        Assert.False(scrollBar.IsHandleCreated);
    }

    private VScrollBar GetVScrollBar(bool createControl, RightToLeft rightToLeft, int minimum, int maximum, int value)
    {
        VScrollBar vScrollBar = new()
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
            vScrollBar.CreateControl();
        }

        return vScrollBar;
    }
}
