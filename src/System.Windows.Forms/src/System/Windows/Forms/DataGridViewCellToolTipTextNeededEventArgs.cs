// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class DataGridViewCellToolTipTextNeededEventArgs : DataGridViewCellEventArgs
{
    internal DataGridViewCellToolTipTextNeededEventArgs(int columnIndex, int rowIndex, string? toolTipText)
        : base(columnIndex, rowIndex)
    {
        ToolTipText = toolTipText;
    }

    public string? ToolTipText { get; set; }
}
