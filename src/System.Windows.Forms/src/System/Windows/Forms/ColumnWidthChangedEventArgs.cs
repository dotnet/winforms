// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class ColumnWidthChangedEventArgs : EventArgs
{
    public ColumnWidthChangedEventArgs(int columnIndex)
    {
        ColumnIndex = columnIndex;
    }

    public int ColumnIndex { get; }
}
