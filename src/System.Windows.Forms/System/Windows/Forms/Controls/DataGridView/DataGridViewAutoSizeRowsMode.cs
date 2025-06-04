// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public enum DataGridViewAutoSizeRowsMode
{
    AllCells = DataGridViewAutoSizeRowsModeInternal.Header | DataGridViewAutoSizeRowsModeInternal.AllColumns | DataGridViewAutoSizeRowsModeInternal.AllRows,
    AllCellsExceptHeaders = DataGridViewAutoSizeRowsModeInternal.AllColumns | DataGridViewAutoSizeRowsModeInternal.AllRows,
    AllHeaders = DataGridViewAutoSizeRowsModeInternal.Header | DataGridViewAutoSizeRowsModeInternal.AllRows,
    DisplayedCells = DataGridViewAutoSizeRowsModeInternal.Header | DataGridViewAutoSizeRowsModeInternal.AllColumns | DataGridViewAutoSizeRowsModeInternal.DisplayedRows,
    DisplayedCellsExceptHeaders = DataGridViewAutoSizeRowsModeInternal.AllColumns | DataGridViewAutoSizeRowsModeInternal.DisplayedRows,
    DisplayedHeaders = DataGridViewAutoSizeRowsModeInternal.Header | DataGridViewAutoSizeRowsModeInternal.DisplayedRows,
    None = DataGridViewAutoSizeRowsModeInternal.None,
}
