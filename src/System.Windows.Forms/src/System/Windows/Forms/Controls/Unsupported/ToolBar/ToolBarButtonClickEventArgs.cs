﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable

[Obsolete(
    Obsoletions.ToolBarMessage,
    error: false,
    DiagnosticId = Obsoletions.UnsupportedControlsDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
[Browsable(false)]
public class ToolBarButtonClickEventArgs : EventArgs
{
    public ToolBarButtonClickEventArgs(ToolBarButton button) => throw new PlatformNotSupportedException();

    public ToolBarButton Button
    {
        get => throw null;
        set { }
    }
}
