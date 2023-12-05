// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

public class ToolStripDropDownClosingEventArgs : CancelEventArgs
{
    public ToolStripDropDownClosingEventArgs(ToolStripDropDownCloseReason reason)
    {
        CloseReason = reason;
    }

    public ToolStripDropDownCloseReason CloseReason { get; }
}
