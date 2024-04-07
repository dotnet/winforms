// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#pragma warning disable RS0016  // Add public types and members to the declared API to simplify porting of applications from .NET Framework to .NET.
// These types will not work, but if they are not accessed, other features in the application will work.
[Obsolete(
    Obsoletions.StatusBarPanelClickEventArgsMessage,
    error: false,
    DiagnosticId = Obsoletions.StatusBarPanelClickEventArgsDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public class StatusBarPanelClickEventArgs : MouseEventArgs
{
    public StatusBarPanelClickEventArgs(StatusBarPanel statusBarPanel,
        MouseButtons button,
        int clicks,
        int x,
        int y)
        : base(button, clicks, x, y, 0)
        => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public StatusBarPanel StatusBarPanel
        => throw new PlatformNotSupportedException();
}
