// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

public partial class DataGrid
{
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
    [Flags]
    public enum HitTestType
    {
        None = 0x00000000,
        Cell = 0x00000001,
        ColumnHeader = 0x00000002,
        RowHeader = 0x00000004,
        ColumnResize = 0x00000008,
        RowResize = 0x00000010,
        Caption = 0x00000020,
        ParentRows = 0x00000040
    }
}
