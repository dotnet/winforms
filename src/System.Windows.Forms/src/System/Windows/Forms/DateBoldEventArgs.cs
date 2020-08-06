// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  The month calendar control fires this event to request information about how
    ///  the days within the visible months should be displayed.
    /// </summary>
    public class DateBoldEventArgs : EventArgs
    {
        internal DateBoldEventArgs(DateTime start, int size)
        {
            StartDate = start;
            Size = size;
        }

        public DateTime StartDate { get; }

        public int Size { get; }

        public int[]? DaysToBold { get; set; }
    }
}
