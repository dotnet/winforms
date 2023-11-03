// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.MonthCalendar;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class MonthCalendar_CalendarButtonAccessibleObjectTests
{
    [WinFormsFact]
    public void CalendarButtonAccessibleObject_ctor_default()
    {
        using MonthCalendar control = new();
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarButtonAccessibleObject buttonAccessibleObject = new SubCalendarButtonAccessibleObject(controlAccessibleObject);

        Assert.Equal(controlAccessibleObject, buttonAccessibleObject.Parent);
        Assert.Equal(controlAccessibleObject, buttonAccessibleObject.TestAccessor().Dynamic._monthCalendarAccessibleObject);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarButtonAccessibleObject_DefaultAction_ReturnsExpected()
    {
        using MonthCalendar control = new();
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarButtonAccessibleObject buttonAccessibleObject = new SubCalendarButtonAccessibleObject(controlAccessibleObject);

        Assert.Equal(SR.AccessibleActionClick, buttonAccessibleObject.DefaultAction);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarButtonAccessibleObject_GetPropertyValue_LegacyIAccessibleDefaultActionPropertyId_ReturnsExpected()
    {
        using MonthCalendar control = new();
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarButtonAccessibleObject buttonAccessibleObject = new SubCalendarButtonAccessibleObject(controlAccessibleObject);

        Assert.Equal(SR.AccessibleActionClick, ((BSTR)buttonAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleDefaultActionPropertyId)).ToStringAndFree());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarButtonAccessibleObject_ControlType_IsButton()
    {
        using MonthCalendar control = new();
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarButtonAccessibleObject buttonAccessibleObject = new SubCalendarButtonAccessibleObject(controlAccessibleObject);

        UIA_CONTROLTYPE_ID actual = (UIA_CONTROLTYPE_ID)(int)buttonAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId, actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarButtonAccessibleObject_Supports_InvokePattern()
    {
        using MonthCalendar control = new();
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarButtonAccessibleObject buttonAccessibleObject = new SubCalendarButtonAccessibleObject(controlAccessibleObject);

        bool actual = buttonAccessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_InvokePatternId);

        Assert.True(actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarButtonAccessibleObject_Parent_IsCalendarAccessibleObject()
    {
        using MonthCalendar control = new();
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarButtonAccessibleObject buttonAccessibleObject = new SubCalendarButtonAccessibleObject(controlAccessibleObject);

        AccessibleObject actual = buttonAccessibleObject.Parent;

        Assert.Equal(controlAccessibleObject, actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarButtonAccessibleObject_Role_IsPushButton()
    {
        using MonthCalendar control = new();
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarButtonAccessibleObject buttonAccessibleObject = new SubCalendarButtonAccessibleObject(controlAccessibleObject);

        AccessibleRole actual = buttonAccessibleObject.Role;

        Assert.Equal(AccessibleRole.PushButton, actual);
        Assert.False(control.IsHandleCreated);
    }

    private class SubCalendarButtonAccessibleObject : CalendarButtonAccessibleObject
    {
        public SubCalendarButtonAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject)
            : base(calendarAccessibleObject)
        { }
    }
}
