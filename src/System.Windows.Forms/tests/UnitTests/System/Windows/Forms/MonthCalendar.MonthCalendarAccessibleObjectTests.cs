﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.MonthCalendar;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class MonthCalendar_MonthCalendarAccessibleObjectTests
    {
        [WinFormsFact]
        public void MonthCalendarAccessibleObject_ctor_ThrowsException_IfOwnerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new MonthCalendarAccessibleObject(null));
        }

        [WinFormsTheory]
        [InlineData("Test name", (int)UiaCore.UIA.CalendarControlTypeId)]
        [InlineData(null, (int)UiaCore.UIA.CalendarControlTypeId)]
        public void MonthCalendarAccessibleObject_ControlType_IsExpected_IfAccessibleRoleIsDefault(string name, int expected)
        {
            // UIA is less accessible than the test
            // so we have to use "int" type here for "expected" argument
            using MonthCalendar monthCalendar = new MonthCalendar()
            {
                AccessibleName = name
            };
            // AccessibleRole is not set = Default

            object actual = monthCalendar.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal((UiaCore.UIA)expected, actual);
            Assert.False(monthCalendar.IsHandleCreated);
        }

        [WinFormsFact]
        public void MonthCalendarAccessibleObject_Role_IsExpected_ByDefault()
        {
            using MonthCalendar monthCalendar = new MonthCalendar();
            // AccessibleRole is not set = Default

            AccessibleRole actual = monthCalendar.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.Table, actual);
            Assert.False(monthCalendar.IsHandleCreated);
        }

        public static IEnumerable<object[]> MonthCalendarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(MonthCalendarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void MonthCalendarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using MonthCalendar monthCalendar = new MonthCalendar();
            monthCalendar.AccessibleRole = role;

            object actual = monthCalendar.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(monthCalendar.IsHandleCreated);
        }

        [WinFormsFact]
        public void MonthCalendarAccessibleObject_GetPropertyValue_ReturnsExpected()
        {
            using MonthCalendar monthCalendar = new MonthCalendar();
            DateTime dt = new DateTime(2000, 1, 1);
            monthCalendar.SetDate(dt);

            Assert.Equal(dt.ToLongDateString(), monthCalendar.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ValueValuePropertyId));
            Assert.Equal(AccessibleStates.None, monthCalendar.AccessibilityObject.GetPropertyValue(UiaCore.UIA.LegacyIAccessibleStatePropertyId));
            Assert.False(monthCalendar.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void MonthCalendarAccessibleObject_ShowToday_IsExpected(bool showToday)
        {
            using MonthCalendar monthCalendar = new();
            monthCalendar.ShowToday = showToday;
            var accessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;

            Assert.Equal(showToday, accessibleObject.ShowToday);
            Assert.False(monthCalendar.IsHandleCreated);
        }

        [WinFormsFact]
        public void MonthCalendarAccessibleObject_TodayDate_IsToday()
        {
            using MonthCalendar monthCalendar = new();
            var accessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;

            Assert.Equal(DateTime.Today, accessibleObject.TodayDate);
            Assert.False(monthCalendar.IsHandleCreated);
        }

        [WinFormsFact]
        public void MonthCalendarAccessibleObject_ColumnCount_IsMinusOne_IfHandleIsNotCreated()
        {
            using MonthCalendar monthCalendar = new();
            var accessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;

            Assert.False(monthCalendar.IsHandleCreated);
            Assert.Equal(-1, accessibleObject.ColumnCount);
        }

        [WinFormsFact]
        public void MonthCalendarAccessibleObject_RowCount_IsZero_IfHandleIsNotCreated()
        {
            using MonthCalendar monthCalendar = new();
            var accessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;

            Assert.False(monthCalendar.IsHandleCreated);
            Assert.Equal(0, accessibleObject.RowCount);
        }

        [WinFormsFact]
        public void MonthCalendarAccessibleObject_CalendarsAccessibleObjects_IsNull_IfHandleIsNotCreated()
        {
            using MonthCalendar monthCalendar = new();
            var accessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;

            Assert.False(monthCalendar.IsHandleCreated);
            Assert.Null(accessibleObject.CalendarsAccessibleObjects);
        }

        public static IEnumerable<object[]> MonthCalendarAccessibleObject_Date_ReturnsExpected_TestData()
        {
            yield return new object[] { new DateTime(2000, 1, 1) };
            yield return new object[] { DateTime.Today };
        }

        [WinFormsTheory]
        [MemberData(nameof(MonthCalendarAccessibleObject_Date_ReturnsExpected_TestData))]
        public void MonthCalendarAccessibleObject_MinDate_IsExpected(DateTime minDate)
        {
            using MonthCalendar monthCalendar = new();
            monthCalendar.MinDate = minDate;
            var accessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;

            Assert.Equal(DateTimePicker.EffectiveMinDate(minDate), accessibleObject.MinDate);
            Assert.False(monthCalendar.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(MonthCalendarAccessibleObject_Date_ReturnsExpected_TestData))]
        public void MonthCalendarAccessibleObject_MaxDate_IsExpected(DateTime maxDate)
        {
            using MonthCalendar monthCalendar = new();
            monthCalendar.MaxDate = maxDate;
            var accessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;

            Assert.Equal(DateTimePicker.EffectiveMaxDate(maxDate), accessibleObject.MaxDate);
            Assert.False(monthCalendar.IsHandleCreated);
        }

        public static IEnumerable<object[]> MonthCalendarAccessibleObject_CastDayToDayOfWeek_ReturnsExpected_TestData()
        {
            yield return new object[] { Day.Monday, DayOfWeek.Monday };
            yield return new object[] { Day.Tuesday, DayOfWeek.Tuesday };
            yield return new object[] { Day.Wednesday, DayOfWeek.Wednesday };
            yield return new object[] { Day.Thursday, DayOfWeek.Thursday };
            yield return new object[] { Day.Friday, DayOfWeek.Friday };
            yield return new object[] { Day.Saturday, DayOfWeek.Saturday };
            yield return new object[] { Day.Sunday, DayOfWeek.Sunday };
            yield return new object[] { Day.Default, DayOfWeek.Sunday };
        }

        [WinFormsTheory]
        [MemberData(nameof(MonthCalendarAccessibleObject_CastDayToDayOfWeek_ReturnsExpected_TestData))]
        public void MonthCalendarAccessibleObject_CastDayToDayOfWeek_IsExpected(Day day, DayOfWeek expected)
        {
            using MonthCalendar monthCalendar = new();
            var accessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;

            DayOfWeek actual = accessibleObject.TestAccessor().Dynamic.CastDayToDayOfWeek(day);

            Assert.Equal(expected, actual);
            Assert.False(monthCalendar.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void MonthCalendarAccessibleObject_GetDisplayRange_IsNull_IfHandleIsNotCreated(bool visible)
        {
            using MonthCalendar monthCalendar = new();
            var accessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;

            Assert.False(monthCalendar.IsHandleCreated);
            Assert.Null(accessibleObject.GetDisplayRange(visible));
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void MonthCalendarAccessibleObject_GetDisplayRange_IsExpected(bool visible)
        {
            using MonthCalendar monthCalendar = new();
            monthCalendar.CreateControl();
            var accessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;

            SelectionRange expected = monthCalendar.GetDisplayRange(visible);
            SelectionRange actual = accessibleObject.GetDisplayRange(visible);

            Assert.NotNull(actual);
            Assert.Equal(expected.Start, actual.Start);
            Assert.Equal(expected.End, actual.End);
            Assert.True(monthCalendar.IsHandleCreated);
        }
    }
}
