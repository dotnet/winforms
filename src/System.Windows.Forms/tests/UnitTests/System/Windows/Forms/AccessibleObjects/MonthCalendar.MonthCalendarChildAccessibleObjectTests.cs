// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.MonthCalendar;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class MonthCalendar_MonthCalendarChildAccessibleObjectTests
{
    [WinFormsFact]
    public void MonthCalendarChildAccessibleObject_ctor_ThrowsException_IfMonthCalendarAccessibleObjectIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SubObject(null));
    }

    [WinFormsFact]
    public void MonthCalendarChildAccessibleObject_ctor_default()
    {
        using MonthCalendar control = new();

        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        MonthCalendarChildAccessibleObject accessibleObject = new SubObject(controlAccessibleObject);

        Assert.Equal(controlAccessibleObject, accessibleObject.FragmentRoot);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId, false)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsEnabledPropertyId, true)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId, false)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsLegacyIAccessiblePatternAvailablePropertyId, true)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_LegacyIAccessibleRolePropertyId, (int)AccessibleRole.None)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_LegacyIAccessibleStatePropertyId, (int)AccessibleStates.None)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_NamePropertyId, null)]
    public void MonthCalendarChildAccessibleObject_GetPropertyValue_ReturnsExpected(int property, object expected)
    {
        using MonthCalendar control = new();

        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        MonthCalendarChildAccessibleObject accessibleObject = new SubObject(controlAccessibleObject);
        VARIANT actual = accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)property);

        Assert.Equal(expected, actual.ToObject());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void MonthCalendarChildAccessibleObject_LegacyIAccessiblePattern_IsSupported()
    {
        using MonthCalendar control = new();

        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        MonthCalendarChildAccessibleObject accessibleObject = new SubObject(controlAccessibleObject);

        Assert.True(accessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void MonthCalendarChildAccessibleObject_FragmentRoot_IsControlAccessibleObject()
    {
        using MonthCalendar control = new();

        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        MonthCalendarChildAccessibleObject accessibleObject = new SubObject(controlAccessibleObject);

        Assert.Equal(controlAccessibleObject, accessibleObject.FragmentRoot);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)NavigateDirection.NavigateDirection_FirstChild)]
    [InlineData((int)NavigateDirection.NavigateDirection_LastChild)]
    [InlineData((int)NavigateDirection.NavigateDirection_NextSibling)]
    [InlineData((int)NavigateDirection.NavigateDirection_PreviousSibling)]
    public void MonthCalendarChildAccessibleObject_FragmentNavigate_DoesntHaveChildrenAndSiblings(int direction)
    {
        using MonthCalendar control = new();

        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        MonthCalendarChildAccessibleObject accessibleObject = new SubObject(controlAccessibleObject);

        Assert.Null(accessibleObject.FragmentNavigate((NavigateDirection)direction));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void MonthCalendarChildAccessibleObject_FragmentNavigate_Parent_IsNull()
    {
        using MonthCalendar control = new();

        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        MonthCalendarChildAccessibleObject accessibleObject = new SubObject(controlAccessibleObject);

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void MonthCalendarChildAccessibleObject_RuntimeId_HasThreeExpectedItems()
    {
        using MonthCalendar control = new();

        control.CreateControl();
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        MonthCalendarChildAccessibleObject accessibleObject = new SubObject(controlAccessibleObject);

        Assert.Equal(3, accessibleObject.RuntimeId.Length);
        Assert.Equal(AccessibleObject.RuntimeIDFirstItem, accessibleObject.RuntimeId[0]);
        Assert.Equal(PARAM.ToInt(control.Handle), accessibleObject.RuntimeId[1]);
        Assert.Equal(accessibleObject.GetChildId(), accessibleObject.RuntimeId[2]);
        Assert.True(control.IsHandleCreated);
    }

    private class SubObject : MonthCalendarChildAccessibleObject
    {
        public SubObject(MonthCalendarAccessibleObject calendarAccessibleObject)
            : base(calendarAccessibleObject)
        { }
    }
}
