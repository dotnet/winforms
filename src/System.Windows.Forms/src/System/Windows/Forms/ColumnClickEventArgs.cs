// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Provides data for the <see cref="ListView.OnColumnClick"/> event.
/// </summary>
public class ColumnClickEventArgs : EventArgs
{
    public ColumnClickEventArgs(int column)
    {
        Column = column;
    }

    public int Column { get; }
}
