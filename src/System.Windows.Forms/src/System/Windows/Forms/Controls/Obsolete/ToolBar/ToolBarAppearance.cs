// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[Obsolete(
    Obsoletions.ToolBarAppearanceMessage,
    error: false,
    DiagnosticId = Obsoletions.ToolBarAppearanceDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat),
    EditorBrowsable(EditorBrowsableState.Never)]
public enum ToolBarAppearance
{
    Normal = 0,
    Flat = 1,
}
