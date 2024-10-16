// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.MonthCalendar;

namespace System.Windows.Forms.Tests.AccessibleObjects;

[UseDefaultXunitCulture]
public class MonthCalendar_CalendarCellAccessibleObjectTests
{
    [WinFormsFact]
    public void CalendarCellAccessibleObject_ctor_default()
    {
        using MonthCalendar control = new();
        CalendarCellAccessibleObject cellAccessibleObject = CreateCalendarCellAccessibleObject(control);

        int columnIndexResult = cellAccessibleObject.TestAccessor().Dynamic._columnIndex;
        columnIndexResult.Should().Be(0);

        int rowIndexResult = cellAccessibleObject.TestAccessor().Dynamic._rowIndex;
        rowIndexResult.Should().Be(0);

        int calendarIndexResult = cellAccessibleObject.TestAccessor().Dynamic._calendarIndex;
        calendarIndexResult.Should().Be(0);

        cellAccessibleObject.CanGetDescriptionInternal.Should().BeFalse();
        cellAccessibleObject.GetColumnHeaderItems().Should().BeNull();
        control.IsHandleCreated.Should().BeFalse();
    }

    public static IEnumerable<object[]> CalendarCellAccessibleObject_Bounds_ReturnsExpected_TestData()
    {
        yield return new object[] { 0, 0, new Rectangle(13, 81, 31, 15) };
        yield return new object[] { 0, 1, new Rectangle(44, 81, 31, 15) };
        yield return new object[] { 0, 2, new Rectangle(75, 81, 31, 15) };
        yield return new object[] { 1, 0, new Rectangle(13, 96, 31, 15) };
        yield return new object[] { 1, 1, new Rectangle(44, 96, 31, 15) };
        yield return new object[] { 1, 2, new Rectangle(75, 96, 31, 15) };
    }

    [WinFormsTheory]
    [MemberData(nameof(CalendarCellAccessibleObject_Bounds_ReturnsExpected_TestData))]
    public void CalendarCellAccessibleObject_Bounds_ReturnsExpected(int rowIndex, int columnIndex, Rectangle expected)
    {
        using MonthCalendar control = new();
        CalendarCellAccessibleObject cellAccessibleObject = CreateCalendarCellAccessibleObject(control, 0, rowIndex, columnIndex);

        control.CreateControl();
        Rectangle actual = cellAccessibleObject.Bounds;

        Assert.Equal(expected, actual);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void CalendarCellAccessibleObject_CalendarIndex_ReturnsExpected(int calendarIndex)
    {
        using MonthCalendar control = new();
        CalendarCellAccessibleObject cellAccessibleObject = CreateCalendarCellAccessibleObject(control, calendarIndex);

        Assert.Equal(calendarIndex, cellAccessibleObject.CalendarIndex);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void CalendarCellAccessibleObject_Column_ReturnsExpected(int columnIndex)
    {
        using MonthCalendar control = new();
        CalendarCellAccessibleObject cellAccessibleObject = CreateCalendarCellAccessibleObject(control, 0, 0, columnIndex);

        Assert.Equal(columnIndex, cellAccessibleObject.Column);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarCellAccessibleObject_ContainingGrid_ReturnsExpected()
    {
        using MonthCalendar control = new();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, 0, "Test name");
        CalendarBodyAccessibleObject bodyAccessibleObject = new(calendarAccessibleObject, controlAccessibleObject, 0);
        CalendarRowAccessibleObject rowAccessibleObject = new(bodyAccessibleObject, controlAccessibleObject, 0, 0);
        CalendarCellAccessibleObject cellAccessibleObject = new(rowAccessibleObject, bodyAccessibleObject, controlAccessibleObject, 0, 0, 0);

        Assert.Equal(bodyAccessibleObject, cellAccessibleObject.ContainingGrid);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void CalendarCellAccessibleObject_GetChildId_ReturnsExpected(int columnIndex)
    {
        using MonthCalendar control = new();
        CalendarCellAccessibleObject cellAccessibleObject = CreateCalendarCellAccessibleObject(control, 0, 0, columnIndex);

        int actual = cellAccessibleObject.GetChildId();

        Assert.Equal(columnIndex + 1, actual);
        Assert.False(control.IsHandleCreated);
    }

    private CalendarCellAccessibleObject CreateCalendarCellAccessibleObject(MonthCalendar control, int calendarIndex = 0, int rowIndex = 0, int columnIndex = 0)
    {
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, calendarIndex, "Test name");
        CalendarBodyAccessibleObject bodyAccessibleObject = new(calendarAccessibleObject, controlAccessibleObject, calendarIndex);
        CalendarRowAccessibleObject rowAccessibleObject = new(bodyAccessibleObject, controlAccessibleObject, calendarIndex, rowIndex);
        CalendarCellAccessibleObject cellAccessibleObject = new(rowAccessibleObject, bodyAccessibleObject, controlAccessibleObject, calendarIndex, rowIndex, columnIndex);

        return cellAccessibleObject;
    }

    [WinFormsFact]
    public void CalendarCellAccessibleObject_Name_IsEmptyString_IfControlIsNotCreated()
    {
        using MonthCalendar control = new();
        CalendarCellAccessibleObject cellAccessibleObject = CreateCalendarCellAccessibleObject(control, 0, 0, 0);

        Assert.Empty(cellAccessibleObject.Name);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> CalendarCellAccessibleObject_Name_ReturnsExpected_TestData()
    {
        yield return new object[] { MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH, "Wednesday, June 16, 2021" };
        yield return new object[] { MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_YEAR, "November 2021" };
        yield return new object[] { MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_DECADE, "2029" };
        yield return new object[] { MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_CENTURY, "2090 - 2099" };
    }

    [WinFormsTheory]
    [MemberData(nameof(CalendarCellAccessibleObject_Name_ReturnsExpected_TestData))]
    public void CalendarCellAccessibleObject_Name_ReturnsExpected(int view, string expected)
    {
        using MonthCalendar control = new();
        control.FirstDayOfWeek = Day.Monday;
        control.SelectionStart = new DateTime(2021, 6, 16); // Set a date to have a stable test case

        control.CreateControl();
        PInvokeCore.SendMessage(control, PInvoke.MCM_SETCURRENTVIEW, 0, view);

        CalendarCellAccessibleObject cellAccessibleObject = CreateCalendarCellAccessibleObject(control, 0, 2, 2);

        Assert.Equal(expected, cellAccessibleObject.Name);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarCellAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
    {
        using MonthCalendar control = new();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "");
        CalendarBodyAccessibleObject body = new(calendar, controlAccessibleObject, 0);
        CalendarRowAccessibleObject row = new(body, controlAccessibleObject, 0, 2);
        CalendarCellAccessibleObject cell = new(row, body, controlAccessibleObject, 0, 0, 0);

        Assert.Equal(row, cell.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected()
    {
        using MonthCalendar control = new()
        {
            SelectionStart = new DateTime(2022, 10, 1) // Set a date to have a stable test case
        };
        control.CreateControl();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;

        CalendarAccessibleObject calendar = controlAccessibleObject.CalendarsAccessibleObjects.First?.Value;
        Assert.NotNull(calendar);

        CalendarBodyAccessibleObject body = calendar.CalendarBodyAccessibleObject;
        Assert.NotNull(body);

        CalendarRowAccessibleObject secondWeek = body.RowsAccessibleObjects?.First?.Next?.Next?.Value;
        Assert.NotNull(secondWeek);

        CalendarCellAccessibleObject sunday = secondWeek.CellsAccessibleObjects?.First?.Value;
        CalendarCellAccessibleObject monday = secondWeek.CellsAccessibleObjects?.First?.Next?.Value;
        CalendarCellAccessibleObject tuesday = secondWeek.CellsAccessibleObjects?.First?.Next?.Next?.Value;
        CalendarCellAccessibleObject friday = secondWeek.CellsAccessibleObjects?.Last?.Previous?.Value;
        CalendarCellAccessibleObject saturday = secondWeek.CellsAccessibleObjects?.Last?.Value;

        Assert.NotNull(sunday);
        Assert.NotNull(monday);
        Assert.NotNull(tuesday);
        Assert.NotNull(friday);
        Assert.NotNull(saturday);

        Assert.Null(sunday.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(monday, sunday.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(sunday, monday.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(tuesday, monday.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(friday, saturday.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(saturday.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
    }

    [WinFormsFact]
    public void CalendarCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfWeekNumbersVisible()
    {
        using MonthCalendar control = new()
        {
            ShowWeekNumbers = true,
            SelectionStart = new DateTime(2022, 10, 1) // Set a date to have a stable test case
        };
        control.CreateControl();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;

        CalendarAccessibleObject calendar = controlAccessibleObject.CalendarsAccessibleObjects.First?.Value;
        Assert.NotNull(calendar);

        CalendarBodyAccessibleObject body = calendar.CalendarBodyAccessibleObject;
        Assert.NotNull(body);

        CalendarRowAccessibleObject secondWeek = body.RowsAccessibleObjects?.First?.Next?.Next?.Value;
        Assert.NotNull(secondWeek);

        CalendarWeekNumberCellAccessibleObject weekNumber = secondWeek.WeekNumberCellAccessibleObject;
        CalendarCellAccessibleObject sunday = secondWeek.CellsAccessibleObjects?.First?.Value;
        CalendarCellAccessibleObject monday = secondWeek.CellsAccessibleObjects?.First?.Next?.Value;

        Assert.NotNull(weekNumber);
        Assert.NotNull(sunday);
        Assert.NotNull(monday);

        Assert.Equal(weekNumber, sunday.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(monday, sunday.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
    }

    [WinFormsFact]
    public void CalendarCellAccessibleObject_FragmentNavigate_Child_ReturnsExpected()
    {
        using MonthCalendar control = new();
        CalendarCellAccessibleObject cell = CreateCalendarCellAccessibleObject(control, 0, 0, 0);

        Assert.Null(cell.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(cell.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 1, typeof(CalendarWeekNumberCellAccessibleObject))]
    [InlineData(false, 0, null)]
    public void CalendarCellAccessibleObject_GetRowHeaderItems_ReturnsCorrectly(bool showWeekNumbers, int expectedCount, Type expectedType)
    {
        using MonthCalendar control = new() { ShowWeekNumbers = showWeekNumbers };
        control.CreateControl();
        CalendarCellAccessibleObject cellAccessibleObject = CreateCalendarCellAccessibleObject(control, 0, 2, 2);

        var rowHeaderItems = cellAccessibleObject.GetRowHeaderItems();

        if (expectedCount == 0)
        {
            rowHeaderItems.Should().BeNull("no week number cells should be present as row headers when week numbers are not visible");
        }
        else
        {
            rowHeaderItems.Should().NotBeNull();
            rowHeaderItems.Should().HaveCount(expectedCount, $"expected {expectedCount} week number cell(s) as row header(s)");
            rowHeaderItems[0].Should().BeAssignableTo(expectedType, "the row header should match the expected type");
        }

        control.IsHandleCreated.Should().BeTrue();
    }
}
