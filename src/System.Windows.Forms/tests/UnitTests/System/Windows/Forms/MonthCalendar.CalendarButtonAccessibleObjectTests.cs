// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.MonthCalendar;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class MonthCalendar_CalendarButtonAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
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

            Assert.Equal(SR.AccessibleActionClick, buttonAccessibleObject.GetPropertyValue(UiaCore.UIA.LegacyIAccessibleDefaultActionPropertyId));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarButtonAccessibleObject_ControlType_IsButton()
        {
            using MonthCalendar control = new();
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarButtonAccessibleObject buttonAccessibleObject = new SubCalendarButtonAccessibleObject(controlAccessibleObject);

            UiaCore.UIA actual = (UiaCore.UIA)buttonAccessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ButtonControlTypeId, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarButtonAccessibleObject_Supports_InvokePattern()
        {
            using MonthCalendar control = new();
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarButtonAccessibleObject buttonAccessibleObject = new SubCalendarButtonAccessibleObject(controlAccessibleObject);

            bool actual = buttonAccessibleObject.IsPatternSupported(UiaCore.UIA.InvokePatternId);

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
}
