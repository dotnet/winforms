// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class MonthCalendar
{
    /// <summary>
    ///  Represents an accessible object for a calendar day of week cell in <see cref="MonthCalendar"/> control.
    /// </summary>
    internal sealed class CalendarDayOfWeekCellAccessibleObject : CalendarCellAccessibleObject
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

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_NextSibling => _calendarRowAccessibleObject.CellsAccessibleObjects?.Find(this)?.Next?.Value,
                NavigateDirection.NavigateDirection_PreviousSibling => _calendarRowAccessibleObject.CellsAccessibleObjects?.Find(this)?.Previous?.Value,
                _ => base.FragmentNavigate(direction)
            };

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_HeaderControlTypeId,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => VARIANT.False,
                _ => base.GetPropertyValue(propertyID)
            };

        private protected override bool HasKeyboardFocus => false;

        internal override void Invoke()
        { }

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                // Only date cells support Invoke, TableItem and GridItem patterns.
                // Header cells of a table are not its items.
                UIA_PATTERN_ID.UIA_GridItemPatternId => false,
                UIA_PATTERN_ID.UIA_TableItemPatternId => false,
                UIA_PATTERN_ID.UIA_InvokePatternId => false,
                _ => base.IsPatternSupported(patternId)
            };

        public override string Name => _initName;

        public override AccessibleRole Role => AccessibleRole.ColumnHeader;

        public override AccessibleStates State => AccessibleStates.None;
    }
}
