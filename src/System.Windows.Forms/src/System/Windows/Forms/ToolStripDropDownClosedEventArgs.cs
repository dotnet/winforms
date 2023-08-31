// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class ToolStripDropDownClosedEventArgs : EventArgs
{
    public ToolStripDropDownClosedEventArgs(ToolStripDropDownCloseReason reason)
    {
        CloseReason = reason;
    }

    public ToolStripDropDownCloseReason CloseReason { get; }
}
