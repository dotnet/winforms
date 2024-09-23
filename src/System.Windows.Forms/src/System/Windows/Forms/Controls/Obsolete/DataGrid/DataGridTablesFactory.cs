// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[Obsolete(
    Obsoletions.GridTablesFactoryMessage,
    error: false,
    DiagnosticId = Obsoletions.GridTablesFactoryDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat),
    EditorBrowsable(EditorBrowsableState.Never)]
public sealed class GridTablesFactory
{
    public static DataGridTableStyle[] CreateGridTables(DataGridTableStyle gridTable,
        object dataSource,
        string dataMember,
        BindingContext bindingManager) => throw new PlatformNotSupportedException();
}
