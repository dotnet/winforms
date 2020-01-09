// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This is a class that represents the date selection range of a MonthCalendar control.
    /// </summary>
    [
    TypeConverter(typeof(SelectionRangeConverter))
    ]
    public sealed class SelectionRange
    {
        /// <summary>
        ///  The lower limit of the selection range.
        /// </summary>
        private DateTime start = DateTime.MinValue.Date;

        /// <summary>
        ///  The upper limit of the selection range.
        /// </summary>
        private DateTime end = DateTime.MaxValue.Date;

        /// <summary>
        ///  Create a new SelectionRange object with the range [null, null].
        /// </summary>
        public SelectionRange()
        {
        }

        /// <summary>
        ///  Create a new SelectionRange object with the given range.
        /// </summary>
        public SelectionRange(DateTime lower, DateTime upper)
        {
            //NOTE: simcooke: we explicitly DO NOT want to throw an exception here - just silently
            //                swap them around. This is because the win32 control can return non-
            //                normalized ranges.

            // We use lower.Date and upper.Date to remove any time component
            //
            if (lower < upper)
            {
                start = lower.Date;
                end = upper.Date;
            }
            else
            {
                start = upper.Date;
                end = lower.Date;
            }
        }

        /// <summary>
        ///  Create a new SelectionRange object given an existing SelectionRange object.
        /// </summary>
        public SelectionRange(SelectionRange range)
        {
            start = range.start;
            end = range.end;
        }

        /// <summary>
        ///  Returns the ending time of this range.
        /// </summary>
        public DateTime End
        {
            get
            {
                return end;
            }
            set
            {
                end = value.Date;
            }
        }

        /// <summary>
        ///  Starting time of this range
        /// </summary>
        public DateTime Start
        {
            get
            {
                return start;
            }
            set
            {
                start = value.Date;
            }
        }

        /// <summary>
        ///  Returns a string representation for this control.
        /// </summary>
        public override string ToString()
        {
            return "SelectionRange: Start: " + start.ToString() + ", End: " + end.ToString();
        }
    }
}
