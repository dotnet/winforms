// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[Obsolete(
    Obsoletions.DataGridLineStyleMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridLineStyleDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat),
    EditorBrowsable(EditorBrowsableState.Never)]
public enum DataGridLineStyle
{
    None,
    Solid
}
