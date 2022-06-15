// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        /// <summary>
        ///  Represents an accessible object for a calendar day of week cell in <see cref="MonthCalendar"/> control.
        /// </summary>
        internal class CalendarDayOfWeekCellAccessibleObject : CalendarCellAccessibleObject
        {
            private readonly CalendarRowAccessibleObject _calendarRowAccessibleObject;
            private readonly string _initName;

            public CalendarDayOfWeekCellAccessibleObject(CalendarRowAccessibleObject calendarRowAccessibleObject,
                CalendarBodyAccessibleObject calendarBodyAccessibleObject,
                MonthCalendarAccessibleObject monthCalendarAccessibleObject,
                int calendarIndex, int rowIndex, int columnIndex, string initName)
                : base(calendarRowAccessibleObject, calendarBodyAccessibleObject, monthCalendarAccessibleObject,
                      calendarIndex, rowIndex, columnIndex)
            {
                _calendarRowAccessibleObject = calendarRowAccessibleObject;
                // Name don't change if the calendar date range is not changed,
                // otherwise the calendar accessibility tree will be rebuilt.
                // So save this value one time to avoid sending messages to Windows every time.
                _initName = initName;
            }

            internal override SelectionRange? DateRange => null;

            public override string DefaultAction => string.Empty;

            public override string? Description => null;

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction) =>
                direction switch
                {
                    UiaCore.NavigateDirection.NextSibling => _calendarRowAccessibleObject.CellsAccessibleObjects?.Find(this)?.Next?.Value,
                    UiaCore.NavigateDirection.PreviousSibling => _calendarRowAccessibleObject.CellsAccessibleObjects?.Find(this)?.Previous?.Value,
                    _ => base.FragmentNavigate(direction)
                };

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.HeaderControlTypeId,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => false,
                    _ => base.GetPropertyValue(propertyID)
                };

            private protected override bool HasKeyboardFocus => false;

            internal override void Invoke()
            { }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                    // Only date cells support Invoke, TableItem and GridItem patterns.
                    // Header cells of a table are not its items.
                    UiaCore.UIA.GridItemPatternId => false,
                    UiaCore.UIA.TableItemPatternId => false,
                    UiaCore.UIA.InvokePatternId => false,
                    _ => base.IsPatternSupported(patternId)
                };

            public override string Name => _initName;

            public override AccessibleRole Role => AccessibleRole.ColumnHeader;

            public override AccessibleStates State => AccessibleStates.None;
        }
    }
}
