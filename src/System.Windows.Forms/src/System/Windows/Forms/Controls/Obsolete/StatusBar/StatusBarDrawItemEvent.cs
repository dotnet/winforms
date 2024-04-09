// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

#pragma warning disable RS0016 // Add public types and members to the declared API to simplify porting of applications from .NET Framework to .NET.
// These types will not work, but if they are not accessed, other features in the application will work.
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
        StatusBarPanel panel) : base(g, font, r, itemId, itemState) => throw new PlatformNotSupportedException();

    public StatusBarDrawItemEventArgs(Graphics g,
        Font font,
        Rectangle r,
        int itemId,
        DrawItemState itemState,
        StatusBarPanel panel,
        Color foreColor,
        Color backColor) : base(g, font, r, itemId, itemState, foreColor, backColor) => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public StatusBarPanel Panel => throw new PlatformNotSupportedException();
}
