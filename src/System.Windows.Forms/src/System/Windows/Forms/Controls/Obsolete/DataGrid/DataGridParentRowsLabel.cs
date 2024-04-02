// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#pragma warning disable RS0016
[Obsolete(
    Obsoletions.DataGridParentRowsLabelStyleMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridParentRowsLabelStyleDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public enum DataGridParentRowsLabelStyle
{
    None = 0,
    TableName = 1,
    ColumnName = 2,
    Both = 3,
}
