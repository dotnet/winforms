// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ScrollBar_ScrollBarAccessibleObjectTests
{
    [WinFormsFact]
    public void ScrollBarAccessibleObject_ctor_ThrowsException_IfScrollBarAccessibleObjectIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new ScrollBar.ScrollBarAccessibleObject(null));
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.ScrollBar)]
    [InlineData(false, AccessibleRole.None)]
    public void ScrollBarAccessibleObject_Ctor_Default(bool createControl, AccessibleRole accessibleRole)
    {
        using SubScrollBar scrollBar = new();

        if (createControl)
        {
            scrollBar.CreateControl();
        }

        AccessibleObject accessibleObject = scrollBar.AccessibilityObject;

        Assert.NotNull(accessibleObject);
        Assert.Equal(accessibleRole, accessibleObject.Role);
        Assert.Equal(createControl, scrollBar.IsHandleCreated);
    }

    [WinFormsFact]
    public void ScrollBarAccessibleObject_IsPatternSupported_Invoke_ReturnsExpected()
    {
        using SubScrollBar scrollBar = new();
        scrollBar.CreateControl();
        AccessibleObject accessibleObject = scrollBar.AccessibilityObject;

        Assert.True(accessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_ValuePatternId));
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_NamePropertyId, "TestName")]
    [InlineData((int)UIA_PROPERTY_ID.UIA_ControlTypePropertyId, (int)UIA_CONTROLTYPE_ID.UIA_ScrollBarControlTypeId)] // If AccessibleRole is Default
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId, true)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsValuePatternAvailablePropertyId, true)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_AutomationIdPropertyId, "AutomId")]
    [InlineData((int)UIA_PROPERTY_ID.UIA_RangeValueMaximumPropertyId, 100d)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_RangeValueMinimumPropertyId, 0d)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_RangeValueValuePropertyId, 0d)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_RangeValueLargeChangePropertyId, 10d)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_RangeValueSmallChangePropertyId, 1d)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_RangeValueIsReadOnlyPropertyId, false)]
    public void ScrollBarAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
    {
        using SubScrollBar scrollBar = new()
        {
            AccessibleName = "TestName",
            Name = "AutomId"
        };

        Assert.False(scrollBar.IsHandleCreated);
        var scrollBarAccessibleObject = new ScrollBar.ScrollBarAccessibleObject(scrollBar);
        using VARIANT value = scrollBarAccessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyID);

        Assert.Equal(expected, value.ToObject());
        Assert.False(scrollBar.IsHandleCreated);
    }

    public static IEnumerable<object[]> ScrollBarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(ScrollBarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void ScrollBarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using ScrollBar scrollBar = new SubScrollBar();
        scrollBar.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)scrollBar.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(scrollBar.IsHandleCreated);
    }

    [WinFormsFact]
    public void ScrollBarAccessibleObject_GetPropertyValue_RuntimeId_ReturnsExpected()
    {
        using SubScrollBar scrollBar = new();

        using VARIANT actual = scrollBar.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_RuntimeIdPropertyId);

        Assert.Equal(scrollBar.AccessibilityObject.RuntimeId, actual.ToObject());
        Assert.False(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ScrollBarAccessibleObject_GetPropertyValue_IsEnabled_ReturnsExpected(bool enabled)
    {
        using SubScrollBar scrollBar = new()
        {
            Enabled = enabled
        };

        bool actual = (bool)scrollBar.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId);

        Assert.Equal(scrollBar.Enabled, actual);
        Assert.False(scrollBar.IsHandleCreated);
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
    [InlineData(true, ((int)UIA_PROPERTY_ID.UIA_IsRangeValuePatternAvailablePropertyId))]
    public void ScrollBarAccessibleObject_GetPropertyValue_Pattern_ReturnsExpected(bool expected, int propertyId)
    {
        using SubScrollBar scrollBar = new() { Enabled = true };
        ScrollBar.ScrollBarAccessibleObject accessibleObject = (ScrollBar.ScrollBarAccessibleObject)scrollBar.AccessibilityObject;
        var result = accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyId);
        Assert.Equal(expected, !result.IsEmpty && (bool)result);
        Assert.False(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(100, 100d)]
    [InlineData(1, 1d)]
    [InlineData(0, 0d)]
    [InlineData(50d, 50d)]
    public void ScrollBarAccessibleObject_SetValue_Invoke_ReturnsExpected(int newValue, object expected)
    {
        using SubScrollBar scrollBar = new();
        scrollBar.CreateControl();
        AccessibleObject accessibleObject = scrollBar.AccessibilityObject;

        accessibleObject.SetValue(newValue);
        double actual = (double)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_RangeValueValuePropertyId);

        Assert.Equal(expected, actual);
        Assert.Equal(expected, (double)scrollBar.Value);
        Assert.True(scrollBar.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(101)]
    [InlineData(-1)]
    public void ScrollBarAccessibleObject_SetValue_OutOfRangeValue_ThrowExceptionExpected(int newValue)
    {
        using SubScrollBar scrollBar = new();
        scrollBar.CreateControl();
        AccessibleObject accessibleObject = scrollBar.AccessibilityObject;

        Assert.Throws<ArgumentOutOfRangeException>("value", () => accessibleObject.SetValue(newValue));
        Assert.True(scrollBar.IsHandleCreated);
    }

    private class SubScrollBar : ScrollBar
    {
        public SubScrollBar() : base()
        {
        }
    }
}
