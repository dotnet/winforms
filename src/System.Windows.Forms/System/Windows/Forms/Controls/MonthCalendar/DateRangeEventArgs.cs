// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  The SelectEvent is fired when the user makes an explicit date selection within a
///  month calendar control.
/// </summary>
public class DateRangeEventArgs : EventArgs
{
    public DateRangeEventArgs(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    public DateTime Start { get; }

    public DateTime End { get; }
}
