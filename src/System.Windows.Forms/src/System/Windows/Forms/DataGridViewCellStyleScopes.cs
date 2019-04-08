// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    [Flags]
    public enum DataGridViewCellStyleScopes
    {
        None = 0x00,
        Cell = 0x01,
        Column = 0x02,
        Row = 0x04,
        DataGridView = 0x08,
        ColumnHeaders = 0x10,
        RowHeaders = 0x20,
        Rows = 0x40,
        AlternatingRows = 0x80
    }
}
