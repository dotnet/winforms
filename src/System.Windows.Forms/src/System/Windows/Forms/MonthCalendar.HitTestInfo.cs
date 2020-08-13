// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        /// <summary>
        ///  HitTestInfo objects are returned by MonthCalendar in response to the hitTest method.
        ///  HitTestInfo is for informational purposes only; the user should not construct these objects, and
        ///  cannot modify any of the members.
        /// </summary>
        public sealed class HitTestInfo
        {
            internal HitTestInfo(Point pt, HitArea area, DateTime time)
            {
                Point = pt;
                HitArea = area;
                Time = time;
            }

            /// <summary>
            ///  This constructor is used when the DateTime member is invalid.
            /// </summary>
            internal HitTestInfo(Point pt, HitArea area)
            {
                Point = pt;
                HitArea = area;
            }

            /// <summary>
            ///  The point that was hit-tested
            /// </summary>
            public Point Point { get; }

            /// <summary>
            ///  Output member that receives an enumeration value from System.Windows.Forms.MonthCalendar.HitArea
            ///  representing the result of the hit-test operation.
            /// </summary>
            public HitArea HitArea { get; }

            /// <summary>
            ///  The time information specific to the location that was hit-tested. This value
            ///  will only be valid at certain values of hitArea.
            /// </summary>
            public DateTime Time { get; }

            /// <summary>
            ///  Determines whether a given HitArea should have a corresponding valid DateTime
            /// </summary>
            internal static bool HitAreaHasValidDateTime(HitArea hitArea)
            {
                // ComCtl does not provide a valid date for DayOfWeek.
                switch (hitArea)
                {
                    case HitArea.Date:
                    case HitArea.WeekNumbers:
                        return true;
                }

                return false;
            }
        }
    }
}
