﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public enum DataGridViewAutoSizeColumnsMode
{
    AllCells = DataGridViewAutoSizeColumnCriteriaInternal.Header | DataGridViewAutoSizeColumnCriteriaInternal.AllRows,
    AllCellsExceptHeader = DataGridViewAutoSizeColumnCriteriaInternal.AllRows,
    DisplayedCells = DataGridViewAutoSizeColumnCriteriaInternal.Header | DataGridViewAutoSizeColumnCriteriaInternal.DisplayedRows,
    DisplayedCellsExceptHeader = DataGridViewAutoSizeColumnCriteriaInternal.DisplayedRows,
    None = DataGridViewAutoSizeColumnCriteriaInternal.None,
    ColumnHeader = DataGridViewAutoSizeColumnCriteriaInternal.Header,
    Fill = DataGridViewAutoSizeColumnCriteriaInternal.Fill
}
