// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

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

            internal override UiaCore.IRawElementProviderSimple ContainingGrid => _calendarAccessibleObject;

            internal override int[] RuntimeId =>
                new int[5]
                {
                    RuntimeIDFirstItem,
                    (int)(long)_calendarAccessibleObject.Owner.InternalHandle,
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

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction) =>
                direction switch
                {
                    UiaCore.NavigateDirection.Parent => _parentAccessibleObject,
                    UiaCore.NavigateDirection.NextSibling =>
                        _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarCell, _parentAccessibleObject, _columnIndex + 1),
                    UiaCore.NavigateDirection.PreviousSibling =>
                        _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarCell, _parentAccessibleObject, _columnIndex - 1),
                    _ => base.FragmentNavigate(direction)
                };

            internal override object GetPropertyValue(UiaCore.UIA propertyID) =>
                propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => (_rowIndex == -1) ? UiaCore.UIA.HeaderControlTypeId : UiaCore.UIA.DataItemControlTypeId,
                    UiaCore.UIA.NamePropertyId => Name,
                    var p when
                        p == UiaCore.UIA.HasKeyboardFocusPropertyId ||
                        p == UiaCore.UIA.IsGridItemPatternAvailablePropertyId ||
                        p == UiaCore.UIA.IsTableItemPatternAvailablePropertyId => true,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(UiaCore.UIA patternId) =>
                patternId switch
                {
                    var p when
                        p == UiaCore.UIA.GridItemPatternId ||
                        p == UiaCore.UIA.InvokePatternId ||
                        p == UiaCore.UIA.TableItemPatternId => true,
                    _ => base.IsPatternSupported(patternId)
                };

            internal override void Invoke()
            {
                if (_calendarAccessibleObject.Owner.IsHandleCreated)
                {
                    RaiseMouseClick();
                }
            }

            internal override UiaCore.IRawElementProviderSimple[] GetRowHeaderItems() => null;

            internal override UiaCore.IRawElementProviderSimple[] GetColumnHeaderItems()
            {
                if (!_calendarAccessibleObject.Owner.IsHandleCreated || !_calendarAccessibleObject.HasHeaderRow)
                {
                    return null;
                }

                AccessibleObject headerRowAccessibleObject =
                    _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarRow, _parentAccessibleObject.Parent, -1);
                AccessibleObject headerCellAccessibleObject =
                    _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarCell, headerRowAccessibleObject, _columnIndex);

                return new UiaCore.IRawElementProviderSimple[1] { headerCellAccessibleObject };
            }
        }
    }
}
