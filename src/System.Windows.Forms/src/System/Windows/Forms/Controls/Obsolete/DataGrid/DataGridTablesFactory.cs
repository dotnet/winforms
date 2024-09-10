// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#pragma warning disable RS0016
// Add public types and members to the declared API to simplify porting of applications from .NET Framework to .NET.
// These types will not work, but if they are not accessed, other features in the application will work.
[Obsolete(
    Obsoletions.GridTablesFactoryMessage,
    error: false,
    DiagnosticId = Obsoletions.GridTablesFactoryDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public sealed class GridTablesFactory
{
    public static DataGridTableStyle[] CreateGridTables(DataGridTableStyle gridTable,
        object dataSource,
        string dataMember,
        BindingContext bindingManager) => throw new PlatformNotSupportedException();
}
