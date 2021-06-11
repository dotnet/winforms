# MonthCalendarAccessibleObject expected properties and behavior

This document describes the expected behavior of `MonthCalendarAccessibleObject` 
when using accessibility tools such as Inspect, Narrator or Accessibility Insights, and 
expected accessibility properties of all visible items 
of the [MonthCalendar control](https://docs.microsoft.com/dotnet/api/system.windows.forms.monthcalendar).


- [Overview](#Overview)
- [Accessibility Tools](#Accessibility-Tools)
    - [Inspect](#Inspect)
        - [Accessibility tree](##Accessibility-tree)
        - [Accessibility properties](##Accessibility-properties)
        - [Accessibility actions](##Accessibility-actions)
        - [ElementProviderFromPoint (get an item when hovering by mouse) behavior](##ElementProviderFromPoint-(get-an-item-when-hovering-by-mouse)-behavior)   
    - [Narrator](#Narrator)
    - [AccessibilityInsights](#AccessibilityInsights)
- [Behavior test cases](#Behavior-test-cases)
- [Additional dev points that don't affect users](#Additional-dev-points-that-don't-affect-users)
    

# Overview

[MonthCalendar](https://docs.microsoft.com/dotnet/api/system.windows.forms.monthcalendar) is a native Windows control, wrapped by Windows Forms SDK. Windows Forms provide a [UIA support](https://docs.microsoft.com/dotnet/framework/ui-automation/ui-automation-overview) for all
MonthCalendar accessible objects and their children making the MonthCalendar control accessible for Windows Forms end users.

Expected properties and actions of a MonthCalendar and its parts in different accessibility tools are listed below.
:point_up: A checked item means that the current implementation provides the full support, and the unchecked item means the expected implementation is currently unavailable.

# Accessibility Tools

## Inspect

### Accessibility tree

MonthCalendar accessibility tree must contain all visible items based on its view.

<details>
<summary>1. Month view</summary>

![monthcalendar-inspect-month-view-tree][monthcalendar-inspect-month-view-tree]

</details>
</br>

<details>
<summary>2. Year view</summary>

![monthcalendar-inspect-year-view-tree][monthcalendar-inspect-year-view-tree]

</details>
</br>

<details>
<summary>3. Decade view</summary>

![monthcalendar-inspect-decade-view-tree][monthcalendar-inspect-decade-view-tree]

</details>
</br>

<details>
<summary>4. Century view</summary>

![monthcalendar-inspect-century-view-tree][monthcalendar-inspect-century-view-tree]

</details>
</br>

### Accessibility properties


<details>
<summary>1. Month view</summary>
</br>

MonthCalendar:
- [x] `ControlType` = "calendar" always
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `HasKeyboardFocus` = `true`, if the control is in focus
- [x] `IsKeyboardFocusable` = `true`, if the calendar is enabled
- [x] `HelpText` = "MonthCalendar(Control)"
- [x] Correct grid Column and Row count
- [x] `Name` is empty, if it is not set
- [x] `Role` = "table"
- [x] `Value` = selected dates (e.g. "Saturday, April 10, 2021 - Wednesday, April 14, 2021")
- [x] Column and row headers = `null`
- [x] `State` = "focusable" + "focused", if the control is in focus
- [x] Supports Grid, LegacyIAccessible, Table, Value patterns

Previous/Next buttons:
- [x] `Name` = "Previous" or "Next"
- [x] `ControlType` = "button"
- [x] `IsKeyboardFocusable` = `false`
- [x] `IsEnabled` = `true`, if the control is enabled and there are next/previous calendars
- [x] `HasKeyboardFocus` = `false`
- [x] Has a default action and description
- [x] `Role` = "push button"
- [x] `State` = "normal"
- [x] Supports Invoke and LegacyIAccessible

Today button:
- [x] `Name` = a button text (e.g. "Today: 3/20/2021")
- [x] `ControlType` = "button"
- [x] `IsKeyboardFocusable` = `false`
- [x] `HasKeyboardFocus` = `false`
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] Has a default action and description
- [x] `Role` = "push button"
- [x] `State` = "normal"
- [x] Supports Invoke and LegacyIAccessible

Calendar:
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `ControlType` = "pane"
- [x] `HasKeyboardFocus` = `true`, if the control is in focus and the calendar contains the focused cell
- [x] `IsKeyboardFocusable` = `true`, if the calendar is enabled
- [x] Has correct GridItem properties
- [x] `Role` == "client"
- [x] `State` = "focusable, selectable" + has "focused", "selected", if the calendar contains the focused cell
- [x] Doesn't have TableItems columns and rows
- [x] Supports GridItem, LegacyIAccessible, TableItem patterns

Calendar header button:
- [x] `Name` = the button text (e.g. "March 2021")
- [x] `HasKeyboardFocus` = `false`
- [x] `IsKeyboardFocusable` = `false`
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `DefaultAction` = "Click"
- [x] `Role` = "push button"
- [x] `State` = "normal"
- [x] Supports Invoke and LegacyIAccessible

Calendar body:
- [x] `Name` = the header text (e.g. March 2021)
- [x] `HasKeyboardFocus` = `true`, if the control is in focus and the calendar contains the focused cell
- [x] `IsKeyboardFocusable` = `true`, if the calendar is enabled
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `ControlType` = "table"
- [x] Correct grid Column and Row count (headers are not included)
- [x] `Role` = "table"
- [x] `State` = "default"
- [x] Supports Grid, LegacyIAccessible, Table patterns

Calendar row:
- [x] `Name` is empty
- [x] `HasKeyboardFocus` = `true`, if the control is in focus and the row contains the focused cell
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `IsKeyboardFocusable` = `true`, if the calendar is enabled
- [x] `ControlType` = "pane"
- [x] `Role` = "row"
- [x] `State` = "normal"
- [x] `Description` = "Week {number}" for date rows. `Description` is empty for a header row
- [x] Supports LegacyIAccessible pattern

Cell of the header row (day of week):
- [x] `Name` = the cell text (e.g. "Mon" or "Fri")
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `ControlType` = "header"
- [x] `HasKeyboardFocus` = always `false`
- [x] `IsKeyboardFocusable` = `false`
- [x] `Role` = "column header"
- [x] `State` = "normal"
- [x] Doesn't have a `Description`
- [x] Doesn't have a `DefaultAction`
- [x] Supports LegacyIAccessible pattern

The first cell of date rows (week number):
- [x] `Name` = "Week {the cell text}" (e.g. "Week 12" or "Week 36" - a week number)
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `ControlType` = "header"
- [x] `HasKeyboardFocus` = always `false`
- [x] `IsKeyboardFocusable` = `false`
- [x] `Role` = "row header"
- [x] `State` = "normal"
- [x] Doesn't have a `Description`
- [x] Doesn't have a `DefaultAction`
- [x] Supports LegacyIAccessible pattern

Date cell:
- [x] `Name` = the day long name (e.g. "Wednesday, July 14, 2021")
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `ControlType` = "DataItem" ("item" in the accessibility tree)
- [x] `HasKeyboardFocus` = `true`, if the cell is focused and the control in focus
- [x] `IsKeyboardFocusable` = `true`, if the control is enabled
- [x] Correct GridItem pattern properties
- [x] `Description` = "Week {number}, {day of week}" (e.g. "Week 10, Friday")
- [x] `DefaultAction` = "Click"
- [x] `Role` = "cell"
- [x] `State` = "focusable, selectable", if the control is enabled (the order of the states doesn't matter), <br/>
              "selected, focusable, selectable", if the cell is selected, <br/>
              "focused, selected, focusable, selectable", if the cell is selected and focused. <br/>
              :warning: Important point: if a user selects several cells, all of them should have "selected" state, but only one of them should have "focused" state.
- [x] Correct TableItem column and row headers items
- [x] Supports Invoke, GridItem, LegacyIAccessible, TableItem patterns

</details>
</br>

<details>
<summary>2. Year view</summary>
</br>

MonthCalendar:
- [x] `ControlType` = "calendar" always
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `HasKeyboardFocus` = `true`, if the control is in focus
- [x] `IsKeyboardFocusable` = `true`, if the calendar is enabled
- [x] `HelpText` = "MonthCalendar(Control)"
- [x] Correct grid Column and Row count
- [x] `Name` is empty, if it is not set
- [x] `Role` = "table"
- [x] `Value` = a selected month (e.g. "September 2022")
- [x] Column and row headers = null
- [x] `State` = "focusable" + "focused" if the control is in focus
- [x] Supports Grid, LegacyIAccessible, Table, Value patterns

Previous/Next buttons:
- [x] `Name` = "Previous" or "Next"
- [x] `ControlType` = "button"
- [x] `IsKeyboardFocusable` = `false`
- [x] `IsEnabled` = `true`, if the control is enabled and there are next/previous calendars
- [x] `HasKeyboardFocus` = `false`
- [x] Has a default action and description
- [x] `Role` = "push button"
- [x] `State` = "normal"
- [x] Supports Invoke and LegacyIAccessible

Today button:
- [x] `Name` = a button text (e.g. "Today: 3/20/2021")
- [x] `ControlType` = "button"
- [x] `IsKeyboardFocusable` = `false`
- [x] `HasKeyboardFocus` = `false`
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] Has a default action and description
- [x] `Role` = "push button"
- [x] `State` = "normal"
- [x] Supports Invoke and LegacyIAccessible

Calendar:
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `ControlType` = "pane"
- [x] `HasKeyboardFocus` = `true`, if the control is in focus and the calendar contains the focused cell
- [x] `IsKeyboardFocusable` = `true`, if the calendar is enabled
- [x] Has correct GridItem properties
- [x] `Role` == "client"
- [x] `State` = "focusable, selectable" + has "focused", "selected", if the calendar contains the focused cell
- [x] Doesn't have TableItems columns and rows
- [x] Supports GridItem, LegacyIAccessible, TableItem patterns

Calendar header button:
- [x] `Name` = the button text (e.g. "2021")
- [x] `HasKeyboardFocus` = `false`
- [x] `IsKeyboardFocusable` = `false`
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] Has a default action
- [x] `Role` = "push button"
- [x] `State` = "normal"
- [x] Supports Invoke and LegacyIAccessible

Calendar body:
- [x] `Name` = the header text (e.g. "2021")
- [x] `HasKeyboardFocus` = `true`, if the control is in focus and the calendar contains the focused cell
- [x] `IsKeyboardFocusable` = `true`, if the calendar is enabled
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `ControlType` = "table"
- [x] Correct grid Column and Row count (headers are not included)
- [x] `Role` = "table"
- [x] `State` = "default"
- [x] Supports Grid, LegacyIAccessible, Table patterns

Calendar row:
- [x] `Name` is empty
- [x] `HasKeyboardFocus` = `true`, if the control is in focus and the row contains the focused cell
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `IsKeyboardFocusable` = `true`, if the calendar is enabled
- [x] `ControlType` = "pane"
- [x] `Role` = "row"
- [x] `State` = "normal"
- [x] `Description` is empty
- [x] Supports LegacyIAccessible pattern

Month cell:
- [x] `Name` = the cell text (e.g. "May")
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `ControlType` = "DataItem" ("item" in the accessibility tree)
- [x] `HasKeyboardFocus` = `true`, if the cell is focused
- [x] `IsKeyboardFocusable` = `true`, if the control is enabled
- [x] Correct GridItem pattern properties
- [x] `Description` is empty
- [x] `Role` = "cell"
- [x] `State` = "focusable, selectable" if the control is enabled. (the order of the states doesn't matter) <br/>
              "focused, selected, focusable, selectable" if the cell is selected and focused <br/>
	          :warning: Important point: if a user can't select several cells in this view, so only one cell should have "selected" state, and this cell should have "focused" state.
- [x] Doesn't have TableItem column and row headers items 
- [x] Supports GridItem, LegacyIAccessible, TableItem patterns

</details>
</br>

<details>
<summary>3. Decade view</summary>
</br>

MonthCalendar:
- [x] `ControlType` = "calendar" always
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `HasKeyboardFocus` = `true`, if the control is in focus
- [x] `IsKeyboardFocusable` = `true`, if the calendar is enabled
- [x] `HelpText` = "MonthCalendar(Control)"
- [x] Correct grid Column and Row count
- [x] `Name` is empty, if it is not set
- [x] `Role` = "table"
- [x] `Value` = a selected year (e.g. "2022")
- [x] Column and row headers = null
- [x] `State` = "focusable" + "focused" if the control is in focus
- [x] Supports Grid, LegacyIAccessible, Table, Value patterns

Previous/Next buttons:
- [x] `Name` = "Previous" or "Next"
- [x] `ControlType` = "button"
- [x] `IsKeyboardFocusable` = `false`
- [x] `IsEnabled` = `true`, if the control is enabled and there are next/previous calendars
- [x] `HasKeyboardFocus` = `false`
- [x] Has a default action and description
- [x] `Role` = "push button"
- [x] `State` = "normal"
- [x] Supports Invoke and LegacyIAccessible

Today button:
- [x] `Name` = a button text (e.g. "Today: 3/20/2021")
- [x] `ControlType` = "button"
- [x] `IsKeyboardFocusable` = `false`
- [x] `HasKeyboardFocus` = `false`
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] Has a default action and description
- [x] `Role` = "push button"
- [x] `State` = "normal"
- [x] Supports Invoke and LegacyIAccessible

Calendar:
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `ControlType` = "pane"
- [x] `HasKeyboardFocus` = `true`, if the control is in focus and the calendar contains the focused cell
- [x] `IsKeyboardFocusable` = `true`, if the calendar is enabled
- [x] Has correct GridItem properties
- [x] `Role` == "client"
- [x] `State` = "focusable, selectable" + has "focused", "selected", if the calendar contains the focused cell
- [x] Doesn't have TableItems columns and rows
- [x] Supports GridItem, LegacyIAccessible, TableItem patterns

Calendar header button:
- [x] `Name` = the button text (e.g. "2020-2029")
- [x] `HasKeyboardFocus` = `false`
- [x] `IsKeyboardFocusable` = `false`
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] Has a default action
- [x] `Role` = "push button"
- [x] `State` = "normal"
- [x] Supports Invoke and LegacyIAccessible

Calendar body:
- [x] `Name` = the header text (e.g. "2020-2029")
- [x] `HasKeyboardFocus` = `true`, if the control is in focus and the calendar contains the focused cell
- [x] `IsKeyboardFocusable` = `true`, if the calendar is enabled
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `ControlType` = "table"
- [x] Correct grid Column and Row count (headers are not included)
- [x] `Role` = "table"
- [x] `State` = "default"
- [x] Supports Grid, LegacyIAccessible, Table patterns

Calendar row:
- [x] `Name` is empty
- [x] `HasKeyboardFocus` = `true`, if the control is in focus and the row contains the focused cell
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `IsKeyboardFocusable` = `true`, if the calendar is enabled
- [x] `ControlType` = "pane"
- [x] `Role` = "row"
- [x] `State` = "normal"
- [x] `Description` is empty
- [x] Supports LegacyIAccessible pattern

Year cell:
- [x] `Name` = the cell text (e.g. "2020")
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `ControlType` = "DataItem" ("item" in the accessibility tree)
- [x] `HasKeyboardFocus ` = `true`, if the cell is focused
- [x] `IsKeyboardFocusable` = `true`, if the control is enabled
- [x] Correct GridItem pattern properties
- [x] `Description` is empty
- [x] `Role` = "cell"
- [x] `State` = "focusable, selectable" if the control is enabled. (the order of the states doesn't matter) <br/>
              "focused, selected, focusable, selectable" if the cell is selected and focused <br/>
	          :warning: Important point: if a user can't select several cells in this view, so only one cell should have "selected" state, and this cell should have "focused" state.
- [x] Doesn't have TableItem column and row headers items 
- [x] Supports GridItem, LegacyIAccessible, TableItem patterns

</details>
</br>

<details>
<summary>4. Century view</summary>
</br>

MonthCalendar:
- [x] `ControlType` = "calendar" always
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `HasKeyboardFocus` = `true`, if the control is in focus
- [x] `IsKeyboardFocusable` = `true`, if the calendar is enabled
- [x] `HelpText` = "MonthCalendar(Control)"
- [x] Correct grid Column and Row count
- [x] `Name` is empty, if it is not set
- [x] `Role` = "table"
- [x] `Value` = a selected decade (e.g. "2020-2029")
- [x] Column and row headers = null
- [x] `State` = "focusable" + "focused" if the control is in focus
- [x] Supports Grid, LegacyIAccessible, Table, Value patterns

Previous/Next buttons:
- [x] `Name` = "Previous" or "Next"
- [x] `ControlType` = "button"
- [x] `IsKeyboardFocusable` = `false`
- [x] `IsEnabled` = `true`, if the control is enabled and there are next/previous calendars
- [x] `HasKeyboardFocus` = `false`
- [x] Has a default action and description
- [x] `Role` = "push button"
- [x] `State` = "normal"
- [x] Supports Invoke and LegacyIAccessible

Today button:
- [x] `Name` = a button text (e.g. "Today: 3/20/2021")
- [x] `ControlType` = "button"
- [x] `IsKeyboardFocusable` = `false`
- [x] `HasKeyboardFocus` = `false`
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] Has a default action and description
- [x] `Role` = "push button"
- [x] `State` = "normal"
- [x] Supports Invoke and LegacyIAccessible

Calendar:
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `ControlType` = "pane"
- [x] `HasKeyboardFocus` = `true`, if the control is in focus and the calendar contains the focused cell
- [x] `IsKeyboardFocusable` = `true`, if the calendar is enabled
- [x] Has correct GridItem properties
- [x] `Role` == "client"
- [x] `State` = "focusable, selectable" + has "focused", "selected", if the calendar contains the focused cell
- [x] Doesn't have TableItems columns and rows
- [x] Supports GridItem, LegacyIAccessible, TableItem patterns

Calendar header button:
- [x] `Name` = the button text (e.g. "2000-2099")
- [x] `HasKeyboardFocus` = `false`
- [x] `IsKeyboardFocusable` = `false`
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] Has a default action
- [x] `Role` = "push button"
- [x] `State` = "normal"
- [x] Supports Invoke and LegacyIAccessible

Calendar body:
- [x] `Name` = the header text (e.g. "2000-2099")
- [x] `HasKeyboardFocus` = `true`, if the control is in focus and the calendar contains the focused cell
- [x] `IsKeyboardFocusable` = `true`, if the calendar is enabled
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `ControlType` = "table"
- [x] Correct grid Column and Row count (headers are not included)
- [x] `Role` = "table"
- [x] `State` = "default"
- [x] Supports Grid, LegacyIAccessible, Table patterns

Calendar row:
- [x] `Name` is empty
- [x] `HasKeyboardFocus` = `true`, if the control is in focus and the row contains the focused cell
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `IsKeyboardFocusable` = `true`, if the calendar is enabled
- [x] `ControlType` = "pane"
- [x] `Role` = "row"
- [x] `State` = "normal"
- [x] `Description` is empty
- [x] Supports LegacyIAccessible pattern

Decade cell:
- [x] `Name` = the cell text (e.g. "2020-2029")
- [x] `IsEnabled` = `true`, if the control is enabled
- [x] `ControlType` = "DataItem" ("item" in the accessibility tree)
- [x] `HasKeyboardFocus` = `true`, if the cell is focused
- [x] `IsKeyboardFocusable` = `true`, if the control is enabled
- [x] Correct GridItem pattern properties
- [x] `Description` is empty
- [x] `Role` = "cell"
- [x] `State` = "focusable, selectable" if the control is enabled. (the order of the states doesn't matter) <br/>
          "focused, selected, focusable, selectable" if the cell is selected and focused <br/>
	      :warning: Important point: if a user can't select several cells in this view, so only one cell should have "selected" state, and this cell should have "focused" state.
- [x] Doesn't have TableItem column and row headers items 
- [x] Supports GridItem, LegacyIAccessible, TableItem patterns

</details>
</br>

### Accessibility actions

Here are described accessibility actions of supported patterns.
:point_up: The "Focus" action has unexpected behavior, because when you call it from Inspect,
the testing form loses focus. Then the form gets focus, in this case,
MonthCalendar.OnGotFocus handler works, that raises accessibility focus event for the focused cell accessible object.

<details>
<summary>1. Month view</summary>
</br>

MonthCalendar:
- [x] Focus - focuses on the focused cell
- [ ] Grid.GetItem- returns OK for the correct row and column, returns FAIL for incorrect arguments (doesn't work, it's Inspect Issue)
- [x] Value.SetValue - does nothing
- [x] LegacyIAccessible.Select - does nothing, because the MonthCalendar is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Previous/Next buttons:
- [ ] Focus - the button is not keyboard focusable, so does nothing
- [x] Invoke.Invoke - clicks the button (moves to the previous/next month)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [ ] LegacyIAccessible.DoDefaultAction - clicks the button (works in the debug mode only, it's Inspect Issue)
- [x] LegacyIAccessible.SetValue - does nothing

Today button:
- [ ] Focus - the button is not keyboard focusable, so does nothing
- [x] Invoke.Invoke - clicks the button (moves to the today cell)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [ ] LegacyIAccessible.DoDefaultAction - clicks the button (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.SetValue - does nothing

Calendar:
- [ ] Focus - focuses on the focused cell, if the calendar contains it. And does nothing, if the calendar doesn't contain the focused cell 
- [x] LegacyIAccessible.Select - does nothing, because the calendar is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Calendar header button:
- [ ] Focus - the button is not keyboard focusable, so does nothing
- [x] Invoke.Invoke - clicks the button (changes the calendar view)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [ ] LegacyIAccessible.DoDefaultAction - clicks the button (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.SetValue - does nothing

Calendar body:
- [ ] Focus - focuses on the focused cell, if the calendar contains it. And does nothing, if the calendar doesn't contain the focused cell 
- [ ] Grid.GetItem - returns OK for the correct row and column, returns FAIL for incorrect arguments (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.Select - does nothing, because the body is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Calendar row:
- [ ] Focus - focuses on the focused cell, if the row contains it. And does nothing, if the row doesn't contain the focused cell 
- [x] LegacyIAccessible.Select - does nothing, because the row is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Cell of the header row (day of week):
- [ ] Focus - does nothing
- [x] LegacyIAccessible.Select - does nothing, because the header cell is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

The first cell of date rows (week numbers):
- [ ] Focus - does nothing
- [x] LegacyIAccessible.Select - does nothing, because the header cell is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Date cell:
- [x] Focus - focuses on the focused cell
- [x] Invoke.Invoke - clicks the cell (select it)
- [x] LegacyIAccessible.Select - selects the cell
- [x] LegacyIAccessible.DoDefaultAction - selects the cell
- [x] LegacyIAccessible.SetValue - does nothing

</details>
</br>

<details>
<summary>2. Year view</summary>
</br>

MonthCalendar:
- [x] Focus - focuses on the focused cell
- [ ] Grid.GetItem- returns OK for the correct row and column, returns FAIL for incorrect arguments (doesn't work, it's Inspect Issue)
- [x] Value.SetValue - does nothing
- [x] LegacyIAccessible.Select - does nothing, because the MonthCalendar is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Previous/Next buttons:
- [ ] Focus - the button is not keyboard focusable, so does nothing
- [x] Invoke.Invoke - clicks the button (moves to the previous/next month)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [ ] LegacyIAccessible.DoDefaultAction - clicks the button (works in the debug mode only, it's Inspect Issue)
- [x] LegacyIAccessible.SetValue - does nothing

Today button:
- [ ] Focus - the button is not keyboard focusable, so does nothing
- [x] Invoke.Invoke - clicks the button (moves to the today cell)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [ ] LegacyIAccessible.DoDefaultAction - clicks the button (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.SetValue - does nothing

Calendar:
- [ ] Focus - focuses on the focused cell, if the calendar contains it. And does nothing, if the calendar doesn't contain the focused cell 
- [x] LegacyIAccessible.Select - does nothing, because the calendar is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Calendar header button:
- [ ] Focus - the button is not keyboard focusable, so does nothing
- [x] Invoke.Invoke - clicks the button (changes the calendar view)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [ ] LegacyIAccessible.DoDefaultAction - clicks the button (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.SetValue - does nothing

Calendar body:
- [ ] Focus - focuses on the focused cell, if the calendar contains it. And does nothing, if the calendar doesn't contain the focused cell 
- [ ] Grid.GetItem - returns OK for the correct row and column, returns FAIL for incorrect arguments (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.Select - does nothing, because the body is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Calendar row:
- [ ] Focus - focuses on the focused cell, if the row contains it. And does nothing, if the row doesn't contain the focused cell 
- [x] LegacyIAccessible.Select - does nothing, because the row is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Month cell:
- [x] Focus - focuses on the focused cell
- [x] Invoke.Invoke - clicks the cell (changes the view)
- [x] LegacyIAccessible.Select - selects the cell
- [x] LegacyIAccessible.DoDefaultAction - click the cell
- [x] LegacyIAccessible.SetValue - does nothing


</details>
</br>

<details>
<summary>3. Decade view</summary>
</br>

MonthCalendar:
- [x] Focus - focuses on the focused cell
- [ ] Grid.GetItem- returns OK for the correct row and column, returns FAIL for incorrect arguments (doesn't work, it's Inspect Issue)
- [x] Value.SetValue - does nothing
- [x] LegacyIAccessible.Select - does nothing, because the MonthCalendar is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Previous/Next buttons:
- [ ] Focus - the button is not keyboard focusable, so does nothing
- [x] Invoke.Invoke - clicks the button (moves to the previous/next month)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [ ] LegacyIAccessible.DoDefaultAction - clicks the button (works in the debug mode only, it's Inspect Issue)
- [x] LegacyIAccessible.SetValue - does nothing

Today button:
- [ ] Focus - the button is not keyboard focusable, so does nothing
- [x] Invoke.Invoke - clicks the button (moves to the today cell)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [ ] LegacyIAccessible.DoDefaultAction - clicks the button (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.SetValue - does nothing

Calendar:
- [ ] Focus - focuses on the focused cell, if the calendar contains it. And does nothing, if the calendar doesn't contain the focused cell 
- [x] LegacyIAccessible.Select - does nothing, because the calendar is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Calendar header button:
- [ ] Focus - the button is not keyboard focusable, so does nothing
- [x] Invoke.Invoke - clicks the button (changes the calendar view)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [ ] LegacyIAccessible.DoDefaultAction - clicks the button (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.SetValue - does nothing

Calendar body:
- [ ] Focus - focuses on the focused cell, if the calendar contains it. And does nothing, if the calendar doesn't contain the focused cell 
- [ ] Grid.GetItem - returns OK for the correct row and column, returns FAIL for incorrect arguments (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.Select - does nothing, because the body is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Calendar row:
- [ ] Focus - focuses on the focused cell, if the row contains it. And does nothing, if the row doesn't contain the focused cell 
- [x] LegacyIAccessible.Select - does nothing, because the row is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Year cell:
- [x] Focus - focuses on the focused cell
- [x] Invoke.Invoke - clicks the cell (changes the view)
- [x] LegacyIAccessible.Select - selects the cell
- [x] LegacyIAccessible.DoDefaultAction - click the cell
- [x] LegacyIAccessible.SetValue - does nothing

</details>
</br>

<details>
<summary>4. Century view</summary>
</br>

MonthCalendar:
- [x] Focus - focuses on the focused cell
- [ ] Grid.GetItem- returns OK for the correct row and column, returns FAIL for incorrect arguments (doesn't work, it's Inspect Issue)
- [x] Value.SetValue - does nothing
- [x] LegacyIAccessible.Select - does nothing, because the MonthCalendar is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Previous/Next buttons:
- [ ] Focus - the button is not keyboard focusable, so does nothing
- [x] Invoke.Invoke - clicks the button (moves to the previous/next month)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [ ] LegacyIAccessible.DoDefaultAction - clicks the button (works in the debug mode only, it's Inspect Issue)
- [x] LegacyIAccessible.SetValue - does nothing

Today button:
- [ ] Focus - the button is not keyboard focusable, so does nothing
- [x] Invoke.Invoke - clicks the button (moves to the today cell)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [ ] LegacyIAccessible.DoDefaultAction - clicks the button (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.SetValue - does nothing

Calendar:
- [ ] Focus - focuses on the focused cell, if the calendar contains it. And does nothing, if the calendar doesn't contain the focused cell 
- [x] LegacyIAccessible.Select - does nothing, because the calendar is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Calendar header button:
- [ ] Focus - the button is not keyboard focusable, so does nothing
- [x] Invoke.Invoke - clicks the button (changes the calendar view)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [ ] LegacyIAccessible.DoDefaultAction - clicks the button (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.SetValue - does nothing

Calendar body:
- [ ] Focus - focuses on the focused cell, if the calendar contains it. And does nothing, if the calendar doesn't contain the focused cell 
- [ ] Grid.GetItem - returns OK for the correct row and column, returns FAIL for incorrect arguments (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.Select - does nothing, because the body is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Calendar row:
- [ ] Focus - focuses on the focused cell, if the row contains it. And does nothing, if the row doesn't contain the focused cell 
- [x] LegacyIAccessible.Select - does nothing, because the row is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Decade cell:
- [x] Focus - focuses on the focused cell
- [x] Invoke.Invoke - clicks the cell (changes the view)
- [x] LegacyIAccessible.Select - selects the cell
- [x] LegacyIAccessible.DoDefaultAction - click the cell
- [x] LegacyIAccessible.SetValue - does nothing

</details>
</br>

### ElementProviderFromPoint (get an item when hovering by mouse) behavior

- [x] Returns a MonthCalendar accessible object, if mouse isn't hovering any element
- [x] Returns a "Previous" button accessible object, if mouse is hovering it
- [x] Returns a "Next" button accessible object, if mouse is hovering it
- [x] Returns a "Today" button accessible object, if mouse is hovering the "Today" text, or to the right of the text
- [x] Returns a calendar header button accessible object (e.g. "March 2021"), if mouse is hovering it
- [x] Returns a day of week cell accessible object (e.g. "Sat"), if its MonthCalendar is in the Month view and mouse is hovering it
- [x] Returns a week number cell accessible object (e.g. "18"), if 
    - `ShowWeekNumber` = `true`, 
    - its MonthCalendar is in the Month view
    - mouse is hovering this cell
- [x] Returns a day/month/year/years cell accessible object, if this cell is visible, and mouse is hovering it
- [x] Returns an accessible object of a gray cell of the next/previous calendar, if it is, and if mouse is hovering it <details><summary>Screenshot</summary>![monthcalendar-gray-dates-accessible-from-point][monthcalendar-gray-dates-accessible-from-point]</details>
- [x] Returns a calendar accessible object, if its Bounds contains mouse coordinates, and if there is no any cell in this point <details><summary>Screenshot</summary>![monthcalendar-calendar-accessible-from-point][monthcalendar-calendar-accessible-from-point]</details>
- [x] Returns a MonthCalendar accessible object, if `MaxDate` and `MinDate` are set, and several months are missing there (they are invisible) and if mouse is hovering some invisible calendar <details><summary>Screenshot</summary>![monthcalendar-control-accessible-from-point][monthcalendar-control-accessible-from-point]</details>
- [x] Returns an accessible object of the first week number of a calendar, if mouse is hovering it, and if the calendar first row is partial (a specific case - there is a workaround that fixes a bug of Win API) <details><summary>Screenshot</summary>![monthcalendar-first-weeknumber-accessible-from-point][monthcalendar-first-weeknumber-accessible-from-point]</details>
- [x] Doesn't return week number cells accessible objects of the last calendar, if `MaxDate` is set, and mouse is hovering them <details><summary>Screenshot</summary>![monthcalendar-last-weeknumbers-accessible-from-point][monthcalendar-last-weeknumbers-accessible-from-point]</details>
- [x] Returns a correct accessible object, that is visible, if `MaxDate`, `MinDate`, `FirstDayOfWeek` are changed or the display range is changed via the "Next"/"Previous" buttons

## Narrator

<details>
<summary>1. Month view</summary>
</br>

- [x] Announces dates when moving through them
- [x] Moves through all the accessibility tree nodes in the "Scan" mode
- [ ] Moves through all the accessibility tree nodes in the "Scan" mode after the display range is changed
- [x] Focuses on the focused cell when the control gets focus
- [ ] Focuses on the focused cell, if `MaxDate`, `MinDate`, `FirstDayOfWeek` are changed or the display range is changed via the "Next"/"Previous" buttons

</details>
</br>

<details>
<summary>2. Year view</summary>
</br>

- [x] Announces dates when moving through them
- [x] Moves through all the accessibility tree nodes in the "Scan" mode
- [ ] Moves through all the accessibility tree nodes in the "Scan" mode after the display range is changed
- [x] Focuses on the focused cell when the control gets focus
- [ ] Focuses on the focused cell, if `MaxDate`, `MinDate`, `FirstDayOfWeek` are changed or the display range is changed via the "Next"/"Previous" buttons

</details>
</br>

<details>
<summary>3. Decade view</summary>
</br>

- [x] Announces dates when moving through them
- [x] Moves through all the accessibility tree nodes in the "Scan" mode
- [ ] Moves through all the accessibility tree nodes in the "Scan" mode after the display range is changed
- [x] Focuses on the focused cell when the control gets focus
- [ ] Focuses on the focused cell, if `MaxDate`, `MinDate`, `FirstDayOfWeek` are changed or the display range is changed via the "Next"/"Previous" buttons

</details>
</br>

<details>
<summary>4. Century view</summary>
</br>

- [x] Announces dates when moving through them
- [x] Moves through all the accessibility tree nodes in the "Scan" mode
- [ ] Moves through all the accessibility tree nodes in the "Scan" mode after the display range is changed
- [x] Focuses on the focused cell when the control gets focus
- [ ] Focuses on the focused cell, if `MaxDate`, `MinDate`, `FirstDayOfWeek` are changed or the display range is changed via the "Next"/"Previous" buttons

</details>
</br>

## AccessibilityInsights

<details>
<summary>1. Month view</summary>
</br>

- [x] There are no any AI errors
- [x] The accessibility tree is correct
- [x] AI gets a correct visible accessible object when hovering the mouse (an element from the point)
- [x] AI sees correct item patterns and does supported pattern Actions correctly

MonthCalendar:
- [ ] Grid.GetItem- returns OK for the correct row and column, returns FAIL for incorrect arguments (doesn't work, it's Inspect Issue)
- [x] Value.SetValue - does nothing
- [x] LegacyIAccessible.Select - does nothing, because the MonthCalendar is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Previous/Next buttons:
- [ ] Invoke.Invoke - clicks the button (moves to the previous/next month)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [ ] LegacyIAccessible.DoDefaultAction - clicks the button
- [x] LegacyIAccessible.SetValue - does nothing

Today button:
- [x] Invoke.Invoke - clicks the button (moves to the today cell)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [x] LegacyIAccessible.DoDefaultAction - clicks the button (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.SetValue - does nothing

Calendar:
- [x] LegacyIAccessible.Select - does nothing, because the calendar is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Calendar header button:
- [x] Invoke.Invoke - clicks the button (changes the calendar view)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [x] LegacyIAccessible.DoDefaultAction - clicks the button (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.SetValue - does nothing

Calendar body:
- [ ] Grid.GetItem - returns OK for the correct row and column, returns FAIL for incorrect arguments (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.Select - does nothing, because the body is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Calendar row:
- [x] LegacyIAccessible.Select - does nothing, because the row is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Cell of the header row (day of week):
- [x] LegacyIAccessible.Select - does nothing, because the header cell is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

The first cell of date rows (week numbers):
- [x] LegacyIAccessible.Select - does nothing, because the header cell is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Date cell:
- [x] Invoke.Invoke - clicks the cell (select it)
- [x] LegacyIAccessible.Select - selects the cell
- [ ] LegacyIAccessible.DoDefaultAction - selects the cell (AI issue)
- [x] LegacyIAccessible.SetValue - does nothing

</details>
</br>

<details>
<summary>2. Year view</summary>
</br>

- [x] There are no any AI errors
- [x] The accessibility tree is correct.
- [x] AI gets a correct visible accessible object when hovering the mouse (an element from the point).
- [x] AI sees correct items patterns and does supported pattern Actions correctly:

MonthCalendar:
- [ ] Grid.GetItem- returns OK for the correct row and column, returns FAIL for incorrect arguments (doesn't work, it's Inspect Issue)
- [x] Value.SetValue - does nothing
- [x] LegacyIAccessible.Select - does nothing, because the MonthCalendar is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Previous/Next buttons:
- [ ] Invoke.Invoke - clicks the button (moves to the previous/next month) (AI issue)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [ ] LegacyIAccessible.DoDefaultAction - clicks the button (AI issue)
- [x] LegacyIAccessible.SetValue - does nothing

Today button:
- [x] Invoke.Invoke - clicks the button (moves to the today cell)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [x] LegacyIAccessible.DoDefaultAction - clicks the button (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.SetValue - does nothing

Calendar:
- [x] LegacyIAccessible.Select - does nothing, because the calendar is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Calendar header button:
- [x] Invoke.Invoke - clicks the button (changes the calendar view)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [x] LegacyIAccessible.DoDefaultAction - clicks the button (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.SetValue - does nothing

Calendar body:
- [ ] Grid.GetItem - returns OK for the correct row and column, returns FAIL for incorrect arguments (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.Select - does nothing, because the body is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Calendar row:
- [x] LegacyIAccessible.Select - does nothing, because the row is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Month cell:
- [x] Invoke.Invoke - clicks the cell (select it)
- [x] LegacyIAccessible.Select - selects the cell
- [ ] LegacyIAccessible.DoDefaultAction - selects the cell (AI issue)
- [x] LegacyIAccessible.SetValue - does nothing

</details>
</br>

<details>
<summary>3. Decade view</summary>
</br>

- [x] There are no any AI errors
- [x] The accessibility tree is correct.
- [x] AI gets a correct visible accessible object when hovering the mouse (an element from the point).
- [x] AI sees correct items patterns and does supported pattern Actions correctly:

MonthCalendar:
- [ ] Grid.GetItem- returns OK for the correct row and column, returns FAIL for incorrect arguments (doesn't work, it's Inspect Issue)
- [x] Value.SetValue - does nothing
- [x] LegacyIAccessible.Select - does nothing, because the MonthCalendar is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Previous/Next buttons:
- [ ] Invoke.Invoke - clicks the button (moves to the previous/next month) (AI issue)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [ ] LegacyIAccessible.DoDefaultAction - clicks the button (AI issue)
- [x] LegacyIAccessible.SetValue - does nothing

Today button:
- [x] Invoke.Invoke - clicks the button (moves to the today cell)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [x] LegacyIAccessible.DoDefaultAction - clicks the button (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.SetValue - does nothing

Calendar:
- [x] LegacyIAccessible.Select - does nothing, because the calendar is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Calendar header button:
- [x] Invoke.Invoke - clicks the button (changes the calendar view)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [x] LegacyIAccessible.DoDefaultAction - clicks the button (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.SetValue - does nothing

Calendar body:
- [ ] Grid.GetItem - returns OK for the correct row and column, returns FAIL for incorrect arguments (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.Select - does nothing, because the body is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Calendar row:
- [x] LegacyIAccessible.Select - does nothing, because the row is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Year cell:
- [x] Invoke.Invoke - clicks the cell (select it)
- [x] LegacyIAccessible.Select - selects the cell
- [ ] LegacyIAccessible.DoDefaultAction - selects the cell (AI issue)
- [x] LegacyIAccessible.SetValue - does nothing

</details>
</br>

<details>
<summary>4. Century view</summary>
</br>

- [x] There are no any AI errors
- [x] The accessibility tree is correct.
- [x] AI gets a correct visible accessible object when hovering the mouse (an element from the point).
- [x] AI sees correct items patterns and does supported pattern Actions correctly:

MonthCalendar:
- [ ] Grid.GetItem- returns OK for the correct row and column, returns FAIL for incorrect arguments (doesn't work, it's Inspect Issue)
- [x] Value.SetValue - does nothing
- [x] LegacyIAccessible.Select - does nothing, because the MonthCalendar is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Previous/Next buttons:
- [ ] Invoke.Invoke - clicks the button (moves to the previous/next month) (AI issue)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [ ] LegacyIAccessible.DoDefaultAction - clicks the button (AI issue)
- [x] LegacyIAccessible.SetValue - does nothing

Today button:
- [x] Invoke.Invoke - clicks the button (moves to the today cell)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [x] LegacyIAccessible.DoDefaultAction - clicks the button (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.SetValue - does nothing

Calendar:
- [x] LegacyIAccessible.Select - does nothing, because the calendar is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Calendar header button:
- [x] Invoke.Invoke - clicks the button (changes the calendar view)
- [x] LegacyIAccessible.Select - does nothing, because the button is not selectable
- [x] LegacyIAccessible.DoDefaultAction - clicks the button (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.SetValue - does nothing

Calendar body:
- [ ] Grid.GetItem - returns OK for the correct row and column, returns FAIL for incorrect arguments (doesn't work, it's Inspect Issue)
- [x] LegacyIAccessible.Select - does nothing, because the body is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Calendar row:
- [x] LegacyIAccessible.Select - does nothing, because the row is not selectable
- [x] LegacyIAccessible.DoDefaultAction - does nothing
- [x] LegacyIAccessible.SetValue - does nothing

Decade cell:
- [x] Invoke.Invoke - clicks the cell (select it)
- [x] LegacyIAccessible.Select - selects the cell
- [ ] LegacyIAccessible.DoDefaultAction - selects the cell (AI issue)
- [x] LegacyIAccessible.SetValue - does nothing

</details>
</br>

# Behavior test cases

<details>
<summary>1. Month view</summary>
</br>

- [x] **Case:** Change the Today date (set `TodayDate` of a MonthCalendar)
</br>**Expected:** Nothing happens
- [x] **Case:** Click on a gray date cell (of the next or previous calendars)
</br>**Expected:** The monthCalendar changes the display range. Its accessibility tree rebuilds.
- [x] **Case:** Size of the control is changed that the control changes calendars count
</br>**Expected:** The accessibility tree is rebuilt. ElementProviderFromPoint returns visible items correctly
- [x] **Case:** A calendar of a MonthCalendar has non-full rows
</br>**Expected:** Inspect sees only visible items in that row
- [x] **Case:** A calendar of a MonthCalendar has some empty rows
</br>**Expected:** These rows are not in the accessibility tree
- [x] **Case:** The first week number cell in the first calendar in a MonthCalendar is in a non-full row
</br>**Expected:** Inspect sees that cell correctly with the correct name
- [x] **Case:** The last week number cells of the last non-full calendar have the same values for empty rows
</br>**Expected:** They are not in the accessibility tree
- [x] **Case:** Select some dates (e.g. 10-15th of September), move to right, thereby the focused cell 
will be in right (e.g. 15th of September). Set `MinDate` of the calendar less then the selected range (e.g. 1st of September).
</br>**Expected:** The selected range doesn't change. The focused cell doesn't change. 
The focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select some dates (e.g. 10-15th of September), move to left, thereby the focused cell 
will be in left (e.g. 10th of September). Set `MinDate` of the calendar less then the selected range (e.g. 1st of September).
</br>**Expected:** The selected range doesn't change. The focused cell doesn't change. 
The focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select some dates (e.g. 10-15th of September), move to right, thereby the focused cell 
will be in right (e.g. 15th of September). Set `MaxDate` of the calendar more then the selected range (e.g. 20th of September).
</br>**Expected:** The selected range doesn't change. The focused cell doesn't change. 
The focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select some dates (e.g. 10-15th of September), move to left, thereby the focused cell 
will be in left (e.g. 10th of September). Set `MaxDate` of the calendar more then the selected range (e.g. 20th of September).
</br>**Expected:** The selected range doesn't change. The focused cell doesn't change. 
The focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select some dates (e.g. 10-15th of September), move to right, thereby the focused cell 
will be in right (e.g. 15th of September). Set `MinDate` of the calendar more then the start of the selected range, 
but less then the end of the selected range (e.g. 13th of September).
</br>**Expected:** The selected range changes. The focused cell doesn't change. 
The focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select some dates (e.g. 10-15th of September), move to left, thereby the focused cell 
will be in left (e.g. 10th of September). Set `MinDate` of the calendar more then the start of the selected range, 
but less then the end of the selected range (e.g. 13th of September).
</br>**Expected:** The selected range changes. The focused cell changes (13th of September). 
The new focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select some dates (e.g. 10-15th of September), move to right, thereby the focused cell 
will be in right (e.g. 15th of September). Set `MaxDate` of the calendar more then the start of the selected range, 
but less then the end of the selected range (e.g. 13th of September).
</br>**Expected:** The selected range changes. The focused cell changes (13th of September). 
The new focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select some dates (e.g. 10-15th of September), move to left, thereby the focused cell 
will be in left (e.g. 10th of September). Set `MaxDate` of the calendar more then the start of the selected range, 
but less then the end of the selected range (e.g. 13th of September).
</br>**Expected:** The selected range changes. The focused cell cell doesn't change. 
The focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select some dates (e.g. 10-15th of September), move to right, thereby the focused cell 
will be in right (e.g. 15th of September). Set new `FirstDayOfWeek` (e.g. Friday).
</br>**Expected:** The selected range doesn't change. The focused cell doesn't change. 
The focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select some dates (e.g. 10-15th of September), move to left, thereby the focused cell 
will be in left (e.g. 10th of September). Set new `FirstDayOfWeek` (e.g. Friday).
</br>**Expected:** The selected range doesn't change. The focused cell doesn't change. 
The focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** `MinDate` is more then the selected range. 
</br>**Expected:** The focused cell changes. The new focused cell has "focused" accessibility state.
- [x] **Case:** `MaxDate` is less then the selected range. 
</br>**Expected:** The focused cell changes. The new focused cell has "focused" accessibility state.
- [x] **Case:** A MonthCalendar has 1 calendar.
</br>**Expected:** Accessibility tree has 1 calendar.
- [x] **Case:** A MonthCalendar has several calendars. 
</br>**Expected:** Accessibility tree has the same count of calendars.
- [x] **Case:** `MinDate` is set for a MonthCalendar. 
</br>**Expected:** Dates before `MinDate` are invisible and are not accessible.
- [x] **Case:** `MaxDate` is set for a MonthCalendar. 
</br>**Expected:** Dates after `MaxDate` are invisible and are not accessible.
- [x] **Case:** `MaxDate` and `MinDate` are set for a MonthCalendar. 
They are has a more date range then the display range of the MonthCalendar.
</br>**Expected:** Accessibility tree has all visible calendars. All dates are accessible.
- [x] **Case:** `MaxDate` and `MinDate` are set for a MonthCalendar. 
They are has a less date range then the display range of the MonthCalendar. 
Thereby the MonthCalendar has several partially visible calendars 
(e.g. the MonthCalendar can contain 6 calendars, but 3 of them are visible due `MinDate` and `MinDate`). 
</br>**Expected:** Accessibility tree has the count of visible calendars only (e.g. 3).
Invisible calendars are not accessible. Invisible dates of partial calendars are not accessible.

</details>
</br>

<details>
<summary>2. Year view</summary>
</br>

- [x] **Case:** Change the Today date (set `TodayDate` of a MonthCalendar)
</br>**Expected:** Nothing happens
- [x] **Case:** Click on a gray month cell (of the next or previous calendars)
</br>**Expected:** The monthCalendar changes the display range. It accessibility tree rebuilds.
- [x] **Case:** Size of the control is changed that the control changes calendars count
</br>**Expected:** The accessibility tree is rebuilt. ElementProviderFromPoint returns visible items correctly
- [x] **Case:** A calendar of a MonthCalendar has non-full rows
</br>**Expected:** Inspect sees only visible items in that row
- [x] **Case:** A calendar of a MonthCalendar has some empty rows
</br>**Expected:** These rows are not in the accessibility tree
- [x] **Case:** There are no week number and day of week cells in calendars
</br>**Expected:** There are no any invisible items (week number and day of week cells) in the accessibility tree
- [x] **Case:** Select one month cell (e.g. September), user can't select several cell in this view, 
so the selected cell is focused. Set `MinDate` of the calendar less then the selected cell (e.g. 1st of June).
</br>**Expected:** The focused cell doesn't change. The focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select one month cell (e.g. September), user can't select several cell in this view, 
so the selected cell is focused. Set `MinDate` of the calendar more then the selected cell (e.g. 1st of December).
</br>**Expected:** The focused cell changes (e.g. December). The new focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select one month cell (e.g. September), user can't select several cell in this view, 
so the selected cell is focused. Set `MaxDate` of the calendar less then the selected cell (e.g. 1st of June).
</br>**Expected:** The focused cell changes (e.g. June). The new focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select one month cell (e.g. September), user can't select several cell in this view, 
so the selected cell is focused. Set `MaxDate` of the calendar more then the selected cell (e.g. 1st of December).
</br>**Expected:** The focused cell doesn't change. The focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select one month cell (e.g. September), user can't select several cell in this view, 
so the selected cell is focused. Set `MinDate` of the calendar with the same month (e.g. 30th of September).
</br>**Expected:** The focused cell doesn't change. The focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select one month cell (e.g. September), user can't select several cell in this view, 
so the selected cell is focused. Set `MaxDate` of the calendar with the same month (e.g. 1st of September).
</br>**Expected:** The focused cell doesn't change. The focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** `MinDate` is more then the selected range. 
</br>**Expected:** The focused cell changes. The new focused cell has "focused" accessibility state.
- [x] **Case:** `MaxDate` is less then the selected range. 
</br>**Expected:** The focused cell changes. The new focused cell has "focused" accessibility state.
- [x] **Case:** A MonthCalendar has 1 calendar.
</br>**Expected:** Accessibility tree has 1 calendar.
- [x] **Case:** A MonthCalendar has several calendars. 
</br>**Expected:** Accessibility tree has the same count of calendars.
- [x]  **Case:** `MinDate` is set for a MonthCalendar. 
</br>**Expected:** Dates before `MinDate` are invisible and are not accessible.
- [x] **Case:** `MaxDate` is set for a MonthCalendar. 
</br>**Expected:** Dates after `MaxDate` are invisible and are not accessible.
- [x] **Case:** `MaxDate` and `MinDate` are set for a MonthCalendar. 
They are has a more date range then the display range of the MonthCalendar.
</br>**Expected:** Accessibility tree has all visible calendars. All dates are accessible.
- [x] **Case:** `MaxDate` and `MinDate` are set for a MonthCalendar. 
They are has a less date range then the display range of the MonthCalendar. 
Thereby the MonthCalendar has several partially visible calendars 
(e.g. the MonthCalendar can contain 6 calendars, but 3 of them are visible due `MinDate` and `MinDate`). 
</br>**Expected:** Accessibility tree has the count of visible calendars only (e.g. 3).
Invisible calendars are not accessible. Invisible dates of partial calendars are not accessible.

</details>
</br>

<details>
<summary>3. Decade view</summary>
</br>

- [x] **Case:** Change the Today date (set `TodayDate` of a MonthCalendar)
</br>**Expected:** Nothing happens
- [x] **Case:** Click on a gray year cell (of the next or previous calendars)
</br>**Expected:** The monthCalendar changes the display range. It accessibility tree rebuilds.
- [x] **Case:** Size of the control is changed that the control changes calendars count
</br>**Expected:** The accessibility tree is rebuilt. ElementProviderFromPoint returns visible items correctly
- [x] **Case:** A calendar of a MonthCalendar has non-full rows
</br>**Expected:** Inspect sees only visible items in that row
- [x] **Case:** A calendar of a MonthCalendar has some empty rows
</br>**Expected:** These rows are not in the accessibility tree
- [x] **Case:** There are no week number and day of week cells in calendars
</br>**Expected:** There are no any invisible items (week number and day of week cells) in the accessibility tree
- [x] **Case:** Select one month cell (e.g. 2020), user can't select several cell in this view, 
so the selected cell is focused. Set `MinDate` of the calendar less then the selected cell (e.g. 1st of June 2019).
</br>**Expected:** The focused cell doesn't change. The focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select one month cell (e.g. 2020), user can't select several cell in this view, 
so the selected cell is focused. Set `MinDate` of the calendar more then the selected cell (e.g. 1st of December 2021).
</br>**Expected:** The focused cell changes (e.g. 2021). The new focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select one month cell (e.g. 2020), user can't select several cell in this view, 
so the selected cell is focused. Set `MaxDate` of the calendar less then the selected cell (e.g. 1st of June 2019).
</br>**Expected:** The focused cell changes (e.g. 2019). The new focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select one month cell (e.g. 2020), user can't select several cell in this view, 
so the selected cell is focused. Set `MaxDate` of the calendar more then the selected cell (e.g. 1st of December 2021).
</br>**Expected:** The focused cell doesn't change. The focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select one month cell (e.g. 2020), user can't select several cell in this view, 
so the selected cell is focused. Set `MinDate` of the calendar with the same year (e.g. 31th of December 2020).
</br>**Expected:** The focused cell doesn't change. The focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select one month cell (e.g. 2020), user can't select several cell in this view, 
so the selected cell is focused. Set `MaxDate` of the calendar with the same year (e.g. 1st of January 2020).
</br>**Expected:** The focused cell doesn't change. The focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** `MinDate` is more then the selected range. 
</br>**Expected:** The focused cell changes. The new focused cell has "focused" accessibility state.
- [x] **Case:** `MaxDate` is less then the selected range. 
</br>**Expected:** The focused cell changes. The new focused cell has "focused" accessibility state.
- [x] **Case:** A MonthCalendar has 1 calendar.
</br>**Expected:** Accessibility tree has 1 calendar.
- [x] **Case:** A MonthCalendar has several calendars. 
</br>**Expected:** Accessibility tree has the same count of calendars.
- [x] **Case:** `MinDate` is set for a MonthCalendar. 
</br>**Expected:** Dates before `MinDate` are invisible and are not accessible.
- [x] **Case:** `MaxDate` is set for a MonthCalendar. 
</br>**Expected:** Dates after `MaxDate` are invisible and are not accessible.
- [x] **Case:** `MaxDate` and `MinDate` are set for a MonthCalendar. 
They are has a more date range then the display range of the MonthCalendar.
</br>**Expected:** Accessibility tree has all visible calendars. All dates are accessible.
- [x] **Case:** `MaxDate` and `MinDate` are set for a MonthCalendar. 
They are has a less date range then the display range of the MonthCalendar. 
Thereby the MonthCalendar has several partially visible calendars 
(e.g. the MonthCalendar can contain 6 calendars, but 3 of them are visible due `MinDate` and `MinDate`). 
</br>**Expected:** Accessibility tree has the count of visible calendars only (e.g. 3).
Invisible calendars are not accessible. Invisible dates of partial calendars are not accessible.

</details>
</br>

<details>
<summary>4. Century view</summary>
</br>

- [x] **Case:** Change the Today date (set `TodayDate` of a MonthCalendar)
</br>**Expected:** Nothing happens
- [x] **Case:** Click on a gray decade cell (of the next or previous calendars)
</br>**Expected:** The monthCalendar changes the display range. It accessibility tree rebuilds.
- [x] **Case:** Size of the control is changed that the control changes calendars count
</br>**Expected:** The accessibility tree is rebuilt. ElementProviderFromPoint returns visible items correctly
- [x] **Case:** A calendar of a MonthCalendar has non-full rows
</br>**Expected:** Inspect sees only visible items in that row
- [x] **Case:** A calendar of a MonthCalendar has some empty rows
</br>**Expected:** These rows are not in the accessibility tree
- [x] **Case:** There are no week number and day of week cells in calendars
</br>**Expected:** There are no any invisible items (week number and day of week cells) in the accessibility tree
- [x] **Case:** Select one month cell (e.g. 2020), user can't select several cell in this view, 
so the selected cell is focused. Set `MinDate` of the calendar less then the selected cell (e.g. 1st of June 2019).
</br>**Expected:** The focused cell doesn't change. The focused cell has "focused" accessibility state (check Inspect).
- **Case:** Select one month cell (e.g. 2020), user can't select several cell in this view, 
so the selected cell is focused. Set `MinDate` of the calendar more then the selected cell (e.g. 1st of December 2021).
</br>**Expected:** The focused cell changes (e.g. 2021). The new focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select one month cell (e.g. 2020), user can't select several cell in this view, 
so the selected cell is focused. Set `MaxDate` of the calendar less then the selected cell (e.g. 1st of June 2019).
</br>**Expected:** The focused cell changes (e.g. 2019). The new focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select one month cell (e.g. 2020), user can't select several cell in this view, 
so the selected cell is focused. Set `MaxDate` of the calendar more then the selected cell (e.g. 1st of December 2021).
</br>**Expected:** The focused cell doesn't change. The focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select one month cell (e.g. 2020), user can't select several cell in this view, 
so the selected cell is focused. Set `MinDate` of the calendar with the same year (e.g. 31th of December 2020).
</br>**Expected:** The focused cell doesn't change. The focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** Select one month cell (e.g. 2020), user can't select several cell in this view, 
so the selected cell is focused. Set `MaxDate` of the calendar with the same year (e.g. 1st of January 2020).
</br>**Expected:** The focused cell doesn't change. The focused cell has "focused" accessibility state (check Inspect).
- [x] **Case:** `MinDate` is more then the selected range. 
</br>**Expected:** The focused cell changes. The new focused cell has "focused" accessibility state.
- [x] **Case:** `MaxDate` is less then the selected range. 
</br>**Expected:** The focused cell changes. The new focused cell has "focused" accessibility state.
- [x] **Case:** A MonthCalendar has 1 calendar.
</br>**Expected:** Accessibility tree has 1 calendar.
- [x] **Case:** A MonthCalendar has several calendars. 
</br>**Expected:** Accessibility tree has the same count of calendars.
- [x] **Case:** `MinDate` is set for a MonthCalendar. 
</br>**Expected:** Dates before `MinDate` are invisible and are not accessible.
- [x] **Case:** `MaxDate` is set for a MonthCalendar. 
</br>**Expected:** Dates after `MaxDate` are invisible and are not accessible.
- [x] **Case:** `MaxDate` and `MinDate` are set for a MonthCalendar. 
They are has a more date range then the display range of the MonthCalendar.
</br>**Expected:** Accessibility tree has all visible calendars. All dates are accessible.
- [x] **Case:** `MaxDate` and `MinDate` are set for a MonthCalendar. 
They are has a less date range then the display range of the MonthCalendar. 
Thereby the MonthCalendar has several partially visible calendars 
(e.g. the MonthCalendar can contain 6 calendars, but 3 of them are visible due `MinDate` and `MinDate`). 
</br>**Expected:** Accessibility tree has the count of visible calendars only (e.g. 3).
Invisible calendars are not accessible. Invisible dates of partial calendars are not accessible.

</details>
</br>

# Additional dev points that don't affect users

- Rebuild the accessibility tree, if the calendar `View`, `MaxDate`, `MinDate`, `TodayDate`, `Size` are changed
or Next or Previous buttons are clicked (i.e. the display range is changed).
- Invoke method of button accessible objects of MonthCalendar simulates mouse moving, clicks and then returns the mouse position back.
Windows doesn't provide API to simulate  MonthCalendar buttons clicks via messages.

[monthcalendar-inspect-month-view-tree]: ../images/monthcalendar-inspect-month-view-tree.png
[monthcalendar-inspect-year-view-tree]: ../images/monthcalendar-inspect-year-view-tree.png
[monthcalendar-inspect-decade-view-tree]: ../images/monthcalendar-inspect-decade-view-tree.png
[monthcalendar-inspect-century-view-tree]: ../images/monthcalendar-inspect-century-view-tree.png
[monthcalendar-gray-dates-accessible-from-point]: ../images/monthcalendar-gray-dates-accessible-from-point.png
[monthcalendar-calendar-accessible-from-point]: ../images/monthcalendar-calendar-accessible-from-point.png
[monthcalendar-control-accessible-from-point]: ../images/monthcalendar-control-accessible-from-point.png
[monthcalendar-first-weeknumber-accessible-from-point]: ../images/monthcalendar-first-weeknumber-accessible-from-point.png
[monthcalendar-last-weeknumbers-accessible-from-point]: ../images/monthcalendar-last-weeknumbers-accessible-from-point.png