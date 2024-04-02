// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

#pragma warning disable RS0016 // Add public types and members to the declared API
[Obsolete(
    Obsoletions.StatusBarDrawItemEventArgsMessage,
    error: false,
    DiagnosticId = Obsoletions.StatusBarDrawItemEventArgsDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public class StatusBarDrawItemEventArgs : DrawItemEventArgs
{
    public StatusBarDrawItemEventArgs(Graphics g,
        Font font,
        Rectangle r,
        int itemId,
        DrawItemState itemState,
        StatusBarPanel panel) : base(g, font, r, itemId, itemState)
            => throw new PlatformNotSupportedException();

    public StatusBarDrawItemEventArgs(Graphics g,
        Font font,
        Rectangle r,
        int itemId,
        DrawItemState itemState,
        StatusBarPanel panel,
        Color foreColor,
        Color backColor) : base(g, font, r, itemId, itemState, foreColor, backColor)
            => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public StatusBarPanel Panel
    {
        get => throw new PlatformNotSupportedException();
    }
}
