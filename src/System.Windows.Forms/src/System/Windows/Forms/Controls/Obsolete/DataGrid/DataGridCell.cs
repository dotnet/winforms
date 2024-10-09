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
public struct DataGridCell
{
    public DataGridCell(int r, int c) => throw new PlatformNotSupportedException();

    public int ColumnNumber
    {
        readonly get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public int RowNumber
    {
        readonly get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }
}
