// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#pragma warning disable RS0016
[Obsolete(
    Obsoletions.GridTablesFactoryMessage,
    error: false,
    DiagnosticId = Obsoletions.GridTablesFactoryDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public sealed class GridTablesFactory
{
    private GridTablesFactory()
    {
    }

    public static DataGridTableStyle[] CreateGridTables(DataGridTableStyle gridTable,
        object dataSource,
        string dataMember,
        BindingContext bindingManager)
        => throw new PlatformNotSupportedException();
}
