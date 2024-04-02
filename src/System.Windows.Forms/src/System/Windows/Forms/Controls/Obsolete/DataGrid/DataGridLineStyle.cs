// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#pragma warning disable RS0016
[Obsolete(
    Obsoletions.DataGridLineStyleMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridLineStyleDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public enum DataGridLineStyle
{
    None,
    Solid
}
