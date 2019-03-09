// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Justification = "Values are not combinable")]
    public enum DataGridViewAutoSizeColumnMode
    {
        NotSet = DataGridViewAutoSizeColumnCriteriaInternal.NotSet,
        None = DataGridViewAutoSizeColumnCriteriaInternal.None,
        AllCells = DataGridViewAutoSizeColumnCriteriaInternal.Header | DataGridViewAutoSizeColumnCriteriaInternal.AllRows,
        AllCellsExceptHeader = DataGridViewAutoSizeColumnCriteriaInternal.AllRows,
        DisplayedCells = DataGridViewAutoSizeColumnCriteriaInternal.Header | DataGridViewAutoSizeColumnCriteriaInternal.DisplayedRows,
        DisplayedCellsExceptHeader = DataGridViewAutoSizeColumnCriteriaInternal.DisplayedRows,
        ColumnHeader = DataGridViewAutoSizeColumnCriteriaInternal.Header,
        Fill = DataGridViewAutoSizeColumnCriteriaInternal.Fill
    }
}
