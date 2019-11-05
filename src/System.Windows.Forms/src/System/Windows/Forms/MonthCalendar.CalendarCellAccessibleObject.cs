// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        /// <summary>
        /// Represents the calendar cell accessible object.
        /// </summary>
        internal class CalendarCellAccessibleObject : CalendarGridChildAccessibleObject
        {
            private int _rowIndex;
            private int _columnIndex;
            private string _name;

            public CalendarCellAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject, int calendarIndex, AccessibleObject parentAccessibleObject, int rowIndex, int columnIndex, string name)
                : base(calendarAccessibleObject, calendarIndex, CalendarChildType.CalendarCell, parentAccessibleObject, rowIndex * columnIndex)
            {
                _rowIndex = rowIndex;
                _columnIndex = columnIndex;
                _name = name;
            }

            public override string Name => _name;

            internal override int Row => _rowIndex;

            internal override int Column => _columnIndex;

            internal override UnsafeNativeMethods.IRawElementProviderSimple ContainingGrid => _calendarAccessibleObject;

            internal override int[] RuntimeId =>
                new int[5]
                {
                    RuntimeIDFirstItem,
                    _calendarAccessibleObject.Owner.Handle.ToInt32(),
                    Parent.Parent.GetChildId(),
                    Parent.GetChildId(),
                    GetChildId()
                };

            protected override RECT CalculateBoundingRectangle()
            {
                _calendarAccessibleObject.GetCalendarPartRectangle(_calendarIndex, ComCtl32.MCGIP.CALENDARCELL, _rowIndex, _columnIndex, out RECT rectangle);
                return rectangle;
            }

            internal override int GetChildId() => _columnIndex + 1;

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction) =>
                direction switch
                {
                    UnsafeNativeMethods.NavigateDirection.Parent => _parentAccessibleObject,
                    UnsafeNativeMethods.NavigateDirection.NextSibling =>
                        _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarCell, _parentAccessibleObject, _columnIndex + 1),
                    UnsafeNativeMethods.NavigateDirection.PreviousSibling =>
                        _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarCell, _parentAccessibleObject, _columnIndex - 1),
                    _ => base.FragmentNavigate(direction)
                };

            internal override object GetPropertyValue(int propertyID) =>
                propertyID switch
                {
                    NativeMethods.UIA_ControlTypePropertyId =>
                        (_rowIndex == -1) ? NativeMethods.UIA_HeaderControlTypeId : NativeMethods.UIA_DataItemControlTypeId,
                    NativeMethods.UIA_NamePropertyId => Name,
                    var p when
                        p == NativeMethods.UIA_HasKeyboardFocusPropertyId ||
                        p == NativeMethods.UIA_IsGridItemPatternAvailablePropertyId ||
                        p == NativeMethods.UIA_IsTableItemPatternAvailablePropertyId => true,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(int patternId) =>
                patternId switch
                {
                    var p when
                        p == NativeMethods.UIA_GridItemPatternId ||
                        p == NativeMethods.UIA_InvokePatternId ||
                        p == NativeMethods.UIA_TableItemPatternId => true,
                    _ => base.IsPatternSupported(patternId)
                };

            internal override void Invoke()
            {
                RaiseMouseClick();
            }

            internal override UnsafeNativeMethods.IRawElementProviderSimple[] GetRowHeaderItems() => null;

            internal override UnsafeNativeMethods.IRawElementProviderSimple[] GetColumnHeaderItems()
            {
                if (!_calendarAccessibleObject.HasHeaderRow)
                {
                    return null;
                }

                AccessibleObject headerRowAccessibleObject =
                    _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarRow, _parentAccessibleObject.Parent, -1);
                AccessibleObject headerCellAccessibleObject =
                    _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarCell, headerRowAccessibleObject, _columnIndex);

                return new UnsafeNativeMethods.IRawElementProviderSimple[1] { headerCellAccessibleObject };
            }
        }
    }
}
