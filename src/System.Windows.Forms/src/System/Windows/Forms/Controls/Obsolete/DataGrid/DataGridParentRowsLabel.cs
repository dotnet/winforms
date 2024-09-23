// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[Obsolete(
    Obsoletions.DataGridParentRowsLabelStyleMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridParentRowsLabelStyleDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat),
    EditorBrowsable(EditorBrowsableState.Never)]
public enum DataGridParentRowsLabelStyle
{
    None = 0,
    TableName = 1,
    ColumnName = 2,
    Both = 3,
}
