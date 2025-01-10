// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[Obsolete(
    Obsoletions.ToolBarMessage,
    error: false,
    DiagnosticId = Obsoletions.UnsupportedControlsDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
[Browsable(false)]
public enum ToolBarTextAlign
{
    Underneath = 0,
    Right = 1,
}
