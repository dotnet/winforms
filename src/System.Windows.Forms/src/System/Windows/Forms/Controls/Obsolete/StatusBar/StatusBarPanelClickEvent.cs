// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;
#pragma warning disable RS0016 // Add public types and members to the declared API
[Obsolete("StatusBarPanelClickEventArgs has been deprecated.")]
public class StatusBarPanelClickEventArgs : MouseEventArgs
{
    public StatusBarPanelClickEventArgs(StatusBarPanel statusBarPanel, MouseButtons button, int clicks, int x, int y)
        : base(button, clicks, x, y, 0)
    {
        throw new PlatformNotSupportedException();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public StatusBarPanel StatusBarPanel
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
    }
}
