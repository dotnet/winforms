// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable

/// <summary>
///  This type is provided for binary compatibility with .NET Framework and is not intended to be used directly from your code.
/// </summary>
[Obsolete(
    Obsoletions.DataGridMessage,
    error: false,
    DiagnosticId = Obsoletions.UnsupportedControlsDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
[Browsable(false)]
public sealed class GridTablesFactory
{
    private GridTablesFactory() { }

    public static DataGridTableStyle[] CreateGridTables(
        DataGridTableStyle gridTable,
        object dataSource,
        string dataMember,
        BindingContext bindingManager) => throw new PlatformNotSupportedException();
}
