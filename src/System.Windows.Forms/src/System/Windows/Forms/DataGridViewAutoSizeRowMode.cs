// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Intentionally has no 0 value because values are combinations of DataGridViewAutoSizeRowCriteriaInternal.")]
    public enum DataGridViewAutoSizeRowMode
    {
        AllCells = DataGridViewAutoSizeRowCriteriaInternal.Header | DataGridViewAutoSizeRowCriteriaInternal.AllColumns,
        AllCellsExceptHeader = DataGridViewAutoSizeRowCriteriaInternal.AllColumns,
        RowHeader = DataGridViewAutoSizeRowCriteriaInternal.Header
    }
}
