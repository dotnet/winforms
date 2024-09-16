// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.MonthCalendar;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class MonthCalendar_MonthCalendarAccessibleObjectTests
{
    [WinFormsFact]
    public void MonthCalendarAccessibleObject_ctor_ThrowsException_IfOwnerIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new MonthCalendarAccessibleObject(null));
    }

    [WinFormsTheory]
    [InlineData("Test name", (int)UIA_CONTROLTYPE_ID.UIA_CalendarControlTypeId)]
    [InlineData(null, (int)UIA_CONTROLTYPE_ID.UIA_CalendarControlTypeId)]
    public void MonthCalendarAccessibleObject_ControlType_IsExpected_IfAccessibleRoleIsDefault(string name, int expected)
    {
        // UIA is less accessible than the test
        // so we have to use "int" type here for "expected" argument
        using MonthCalendar monthCalendar = new()
        {
            AccessibleName = name
        };
        // AccessibleRole is not set = Default

        int actual = (int)monthCalendar.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(expected, actual);
        Assert.False(monthCalendar.IsHandleCreated);
    }

    [WinFormsFact]
    public void MonthCalendarAccessibleObject_Role_IsExpected_ByDefault()
    {
        using MonthCalendar monthCalendar = new();
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
        using MonthCalendar monthCalendar = new();
        monthCalendar.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)monthCalendar.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(monthCalendar.IsHandleCreated);
    }

    [WinFormsFact]
    public void MonthCalendarAccessibleObject_GetPropertyValue_ReturnsExpected()
    {
        using MonthCalendar monthCalendar = new();
        DateTime dt = new(2000, 1, 1);
        monthCalendar.SetDate(dt);

        Assert.Equal(dt.ToLongDateString(), ((BSTR)monthCalendar.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId)).ToStringAndFree());
        Assert.Equal(AccessibleStates.None, (AccessibleStates)(int)monthCalendar.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleStatePropertyId));
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

    [WinFormsFact]
    public void MonthCalendarAccessibleObject_FragmentNavigate_Parent_Returns_Expected()
    {
        using MonthCalendar monthCalendar = new();
        MonthCalendarAccessibleObject accessibleObject = new(monthCalendar);

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
    }

    [WinFormsFact]
    public void MonthCalendarAccessibleObject_FragmentNavigate_Sibling_Returns_Expected()
    {
        using MonthCalendar monthCalendar = new();
        MonthCalendarAccessibleObject accessibleObject = new(monthCalendar);

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
    }

    [WinFormsFact]
    public void MonthCalendarAccessibleObject_FragmentNavigate_FirstChild_Returns_Expected()
    {
        using MonthCalendar monthCalendar = new();
        MonthCalendarAccessibleObject accessibleObject = new(monthCalendar);

        AccessibleObject previousButton = accessibleObject.PreviousButtonAccessibleObject;

        Assert.Equal(previousButton, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsFact]
    public void MonthCalendarAccessibleObject_FragmentNavigate_LastChild_Returns_Expected()
    {
        using MonthCalendar monthCalendar = new();
        MonthCalendarAccessibleObject accessibleObject = new(monthCalendar);

        AccessibleObject todayLink = accessibleObject.TodayLinkAccessibleObject;

        Assert.Equal(todayLink, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsFact]
    public void MonthCalendarAccessibleObject_FragmentNavigate_LastChild_Returns_Expected_IfTodayLinkHidden()
    {
        using MonthCalendar monthCalendar = new() { ShowToday = false };
        MonthCalendarAccessibleObject accessibleObject = new(monthCalendar);

        AccessibleObject lastCalendar = accessibleObject.CalendarsAccessibleObjects?.Last?.Value;

        Assert.Equal(lastCalendar, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsFact]
    public void MonthCalendarAccessibleObject_TodayDate_ReturnsOwnerTodayDate_IfOwnerIsNotNull()
    {
        using MonthCalendar monthCalendar = new();
        DateTime expectedTodayDate = new(2023, 1, 1);
        monthCalendar.TodayDate = expectedTodayDate;
        var accessibleObject = new MonthCalendarAccessibleObject(monthCalendar);

        accessibleObject.TodayDate.Should().Be(expectedTodayDate);
    }

    [WinFormsTheory]
    [BoolData]
    public void MonthCalendarAccessibleObject_ShowWeekNumbers_IsExpected(bool showWeekNumbers)
    {
        using MonthCalendar monthCalendar = new();
        monthCalendar.ShowWeekNumbers = showWeekNumbers;
        var accessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;

        accessibleObject.ShowWeekNumbers.Should().Be(showWeekNumbers);
    }

    [WinFormsFact]
    public void MonthCalendarAccessibleObject_FirstDayOfWeek_ReturnsExpected()
    {
        using MonthCalendar monthCalendar = new();
        monthCalendar.FirstDayOfWeek = Day.Monday;
        var accessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;

        accessibleObject.FirstDayOfWeek.Should().Be(DayOfWeek.Monday);
    }

    [WinFormsFact]
    public void MonthCalendarAccessibleObject_Constructor_InitializesPropertiesCorrectly()
    {
        using MonthCalendar monthCalendar = new();
        MonthCalendarAccessibleObject accessibleObject = new(monthCalendar);

        accessibleObject.GetColumnHeaders().Should().BeNull();
        accessibleObject.RowCount.Should().Be(0);
        accessibleObject.CalendarsAccessibleObjects.Should().BeNull();
        monthCalendar.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsFact]
    public void MonthCalendarAccessibleObject_GetFocus_ReturnsFocusedCell()
    {
        using MonthCalendar monthCalendar = new();
        monthCalendar.CreateControl();
        var accessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;

        DateTime focusedDate = new(2023, 10, 1);
        monthCalendar.SetDate(focusedDate);

        var focusedCell = accessibleObject.FocusedCell;
        var focus = accessibleObject.GetFocus();

        focus.Should().Be(focusedCell);
        monthCalendar.IsHandleCreated.Should().BeTrue();
    }

    [WinFormsFact]
    public void MonthCalendarAccessibleObject_GetFocus_ReturnsNull_IfNoFocusedCell()
    {
        using MonthCalendar monthCalendar = new();

        var accessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;
        var focus = accessibleObject.GetFocus();

        focus.Should().BeNull();
    }

    [WinFormsFact]
    public void CanGetHelpInternal_ShouldBeFalse()
    {
        using MonthCalendar monthCalendar = new();
        MonthCalendarAccessibleObject accessibleObject = new(monthCalendar);

        accessibleObject.CanGetHelpInternal.Should().BeFalse();
        monthCalendar.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsTheory]
    [BoolData]
    public void MonthCalendarAccessibleObject_IsEnabled_ReturnsExpected(bool enabled)
    {
        using MonthCalendar monthCalendar = new();
        monthCalendar.Enabled = enabled;
        var accessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;

        accessibleObject.IsEnabled.Should().Be(enabled);
    }

    [WinFormsTheory]
    [BoolData]
    public void MonthCalendarAccessibleObject_RowOrColumnMajor_ReturnsExpected(bool createControl)
    {
        using MonthCalendar monthCalendar = new();
        if (createControl)
        {
            monthCalendar.CreateControl();
        }

        var accessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;

        accessibleObject.RowOrColumnMajor.Should().Be(RowOrColumnMajor.RowOrColumnMajor_RowMajor);
        monthCalendar.IsHandleCreated.Should().Be(createControl);
    }

    [WinFormsFact]
    public void MonthCalendarAccessibleObject_SelectionRange_ReturnsExpected_IfHandleIsCreated()
    {
        using MonthCalendar monthCalendar = new();
        monthCalendar.CreateControl();
        var accessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;

        SelectionRange expected = monthCalendar.SelectionRange;
        SelectionRange actual = accessibleObject.SelectionRange;

        actual.Start.Should().Be(expected.Start);
        actual.End.Should().Be(expected.End);
        monthCalendar.IsHandleCreated.Should().BeTrue();
    }

    [WinFormsFact]
    public void MonthCalendarAccessibleObject_FocusedCell_Validation_WhenOwnerHasFocusedDate()
    {
        using MonthCalendar calendar = new();
        calendar.CreateControl();
        MonthCalendarAccessibleObject accessibleObject = new(calendar);

        calendar.SetDate(DateTime.Today);
        var focusedCellAfterSetDate = accessibleObject.FocusedCell;
        focusedCellAfterSetDate.Should().NotBeNull();
        focusedCellAfterSetDate.DateRange.Start.Date.Should().Be(DateTime.Today);

        var focusedCellAfterSecondCall = accessibleObject.FocusedCell;
        focusedCellAfterSetDate.Should().BeSameAs(focusedCellAfterSecondCall);
    }
}
