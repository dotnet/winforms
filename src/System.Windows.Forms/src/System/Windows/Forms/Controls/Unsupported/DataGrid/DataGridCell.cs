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
[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "This type is provided for public surface compatibility with .NET Framework only. It can't run.")]
public struct DataGridCell
{
#pragma warning disable IDE0251 // Make member 'readonly' - applies to `set` methods.
    public DataGridCell(int r, int c) => throw new PlatformNotSupportedException();

    public int ColumnNumber
    {
        readonly get => throw null;
        set { }
    }

    public int RowNumber
    {
        readonly get => throw null;
        set { }
    }
}
