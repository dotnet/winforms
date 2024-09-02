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

        buttonAccessibleObject.Parent.Should().Be(controlAccessibleObject);

        bool canGetDefaultActionInternal = buttonAccessibleObject.TestAccessor().Dynamic.CanGetDefaultActionInternal;
        canGetDefaultActionInternal.Should().BeFalse();
        control.IsHandleCreated.Should().BeFalse();
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

    [WinFormsFact]
    public void CalendarButtonAccessibleObject_InvokeAndDoDefaultAction_DoesNotThrow()
    {
        using MonthCalendar control = new();
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarButtonAccessibleObject buttonAccessibleObject = new SubCalendarButtonAccessibleObject(controlAccessibleObject);

        Action combinedAction = () =>
        {
            buttonAccessibleObject.Invoke();
            buttonAccessibleObject.DoDefaultAction();
        };

        combinedAction.Should().NotThrow();
        control.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsFact]
    public void CalendarButtonAccessibleObject_RaiseMouseClick_DoesNotThrow_WhenControlIsEnabledAndHasHandle()
    {
        using MonthCalendar control = new();
        control.CreateControl();
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        SubCalendarButtonAccessibleObject buttonAccessibleObject = new SubCalendarButtonAccessibleObject(controlAccessibleObject);

        Action action = () => buttonAccessibleObject.TestAccessor().Dynamic.RaiseMouseClick();

        action.Should().NotThrow();
        control.IsHandleCreated.Should().BeTrue();
    }

    private class SubCalendarButtonAccessibleObject : CalendarButtonAccessibleObject
    {
        public SubCalendarButtonAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject)
            : base(calendarAccessibleObject)
        { }
    }
}
