// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable

[Obsolete(
    Obsoletions.DataGridMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class GridTablesFactory
{
    private GridTablesFactory() { }

    public static DataGridTableStyle[] CreateGridTables(DataGridTableStyle gridTable,
        object dataSource,
        string dataMember,
        BindingContext bindingManager) => throw new PlatformNotSupportedException();
}
