// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[Obsolete(
    Obsoletions.ToolBarTextAlignMessage,
    error: false,
    DiagnosticId = Obsoletions.ToolBarTextAlignDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat),
    EditorBrowsable(EditorBrowsableState.Never)]
public enum ToolBarTextAlign
{
    Underneath = 0,
    Right = 1,
}
