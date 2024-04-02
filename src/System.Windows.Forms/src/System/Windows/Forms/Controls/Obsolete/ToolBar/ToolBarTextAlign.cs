// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[EditorBrowsable(EditorBrowsableState.Never)]
[Obsolete(
    Obsoletions.ToolBarTextAlignMessage,
    error: false,
    DiagnosticId = Obsoletions.ToolBarTextAlignDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
#pragma warning disable RS0016 // Add public types and members to the declared API
public enum ToolBarTextAlign
{
    Underneath = 0,
    Right = 1,
}
