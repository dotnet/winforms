// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        /// <summary>
        /// Represents the calendar child accessible object.
        /// </summary>
        internal abstract class CalendarChildAccessibleObject : AccessibleObject
        {
            protected readonly MonthCalendarAccessibleObject _calendarAccessibleObject;
            protected int _calendarIndex;
            protected CalendarChildType _itemType;

            public CalendarChildAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject, int calendarIndex, CalendarChildType itemType)
            {
                _calendarAccessibleObject = calendarAccessibleObject ?? throw new ArgumentNullException(nameof(calendarAccessibleObject));
                _calendarIndex = calendarIndex;
                _itemType = itemType;
            }

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => _calendarAccessibleObject;

            public override AccessibleObject Parent => _calendarAccessibleObject;

            internal override Rectangle BoundingRectangle => CalculateBoundingRectangle();

            protected virtual RECT CalculateBoundingRectangle() => new RECT();

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction) =>
                direction switch
                {
                    UiaCore.NavigateDirection.Parent => Parent,
                    _ => base.FragmentNavigate(direction)
                };

            internal override object GetPropertyValue(UiaCore.UIA propertyID) =>
                propertyID switch
                {
                    UiaCore.UIA.IsEnabledPropertyId => _calendarAccessibleObject.Enabled,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override int[] RuntimeId =>
                new int[]
                {
                    RuntimeIDFirstItem,
                    (int)(long)_calendarAccessibleObject.Owner.InternalHandle,
                    GetChildId()
                };

            public void RaiseMouseClick()
            {
                // Make sure that the control is enabled.
                if (!_calendarAccessibleObject.Owner.IsHandleCreated || User32.IsWindowEnabled(_calendarAccessibleObject.Owner.Handle).IsFalse())
                {
                    return;
                }

                RECT rectangle = CalculateBoundingRectangle();
                int x = rectangle.left + ((rectangle.right - rectangle.left) / 2);
                int y = rectangle.top + ((rectangle.bottom - rectangle.top) / 2);

                _calendarAccessibleObject.RaiseMouseClick(x, y);
            }
        }
    }
}
