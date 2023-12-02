// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public enum DataGridViewAutoSizeRowMode
{
    AllCells = DataGridViewAutoSizeRowCriteriaInternal.Header | DataGridViewAutoSizeRowCriteriaInternal.AllColumns,
    AllCellsExceptHeader = DataGridViewAutoSizeRowCriteriaInternal.AllColumns,
    RowHeader = DataGridViewAutoSizeRowCriteriaInternal.Header
}
