// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class DataGridViewEditingControlShowingEventArgs : EventArgs
{
    private DataGridViewCellStyle _cellStyle;

    public DataGridViewEditingControlShowingEventArgs(Control control, DataGridViewCellStyle cellStyle)
    {
        Control = control.OrThrowIfNull();
        _cellStyle = cellStyle.OrThrowIfNull();
    }

    public Control Control { get; }

    public DataGridViewCellStyle CellStyle
    {
        get => _cellStyle;
        set => _cellStyle = value.OrThrowIfNull();
    }
}
