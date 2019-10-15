// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
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
            protected MonthCalendarAccessibleObject _calendarAccessibleObject;
            protected int _calendarIndex;
            protected CalendarChildType _itemType;

            public CalendarChildAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject, int calendarIndex, CalendarChildType itemType)
            {
                _calendarAccessibleObject = calendarAccessibleObject;
                _calendarIndex = calendarIndex;
                _itemType = itemType;
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot => _calendarAccessibleObject;

            public override AccessibleObject Parent => _calendarAccessibleObject;

            internal override Rectangle BoundingRectangle => CalculateBoundingRectangle();

            protected virtual RECT CalculateBoundingRectangle() => new RECT();

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction) =>
                direction switch
                {
                    UnsafeNativeMethods.NavigateDirection.Parent => Parent,
                    _ => base.FragmentNavigate(direction)
                };

            internal override object GetPropertyValue(int propertyID) =>
                propertyID switch
                {
                    NativeMethods.UIA_IsEnabledPropertyId => _calendarAccessibleObject.Enabled,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override int[] RuntimeId =>
                new int[]
                {
                    RuntimeIDFirstItem,
                    _calendarAccessibleObject.Owner.Handle.ToInt32(),
                    GetChildId()
                };

            public void RaiseMouseClick()
            {
                // Make sure that the control is enabled.
                if (!SafeNativeMethods.IsWindowEnabled(new HandleRef(null, _calendarAccessibleObject.Owner.Handle)))
                {
                    return;
                }

                var rectangle = CalculateBoundingRectangle();
                int x = rectangle.left + ((rectangle.right - rectangle.left) / 2);
                int y = rectangle.top + ((rectangle.bottom - rectangle.top) / 2);

                _calendarAccessibleObject.RaiseMouseClick(x, y);
            }
        }
    }
}
