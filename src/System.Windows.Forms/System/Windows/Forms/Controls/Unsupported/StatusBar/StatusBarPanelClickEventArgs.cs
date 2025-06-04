// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable

/// <summary>
///  This type is provided for binary compatibility with .NET Framework and is not intended to be used directly from your code.
/// </summary>
[Obsolete(
    Obsoletions.StatusBarMessage,
    error: false,
    DiagnosticId = Obsoletions.UnsupportedControlsDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
[Browsable(false)]
public class StatusBarPanelClickEventArgs : MouseEventArgs
{
    public StatusBarPanelClickEventArgs(
        StatusBarPanel statusBarPanel,
        MouseButtons button,
        int clicks,
        int x,
        int y) : base(button: button, clicks: clicks, x: x, y: y, delta: 0) => throw new PlatformNotSupportedException();

    public StatusBarPanel StatusBarPanel => throw null;
}
