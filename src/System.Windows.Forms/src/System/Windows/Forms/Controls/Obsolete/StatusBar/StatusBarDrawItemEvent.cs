// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[Obsolete(
    Obsoletions.StatusBarDrawItemEventArgsMessage,
    error: false,
    DiagnosticId = Obsoletions.StatusBarDrawItemEventArgsDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat),
    EditorBrowsable(EditorBrowsableState.Never)]
public class StatusBarDrawItemEventArgs : DrawItemEventArgs
{
    public StatusBarDrawItemEventArgs(Graphics g,
        Font font,
        Rectangle r,
        int itemId,
        DrawItemState itemState,
        StatusBarPanel panel) : base(graphics: g,
            font: font,
            rect: r,
            index: itemId,
            state: itemState) => throw new PlatformNotSupportedException();

    public StatusBarDrawItemEventArgs(Graphics g,
        Font font,
        Rectangle r,
        int itemId,
        DrawItemState itemState,
        StatusBarPanel panel,
        Color foreColor,
        Color backColor) : base(graphics: g,
            font: font,
            rect: r,
            index: itemId,
            state: itemState,
            foreColor: foreColor,
            backColor: backColor) => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public StatusBarPanel Panel => throw new PlatformNotSupportedException();
}
