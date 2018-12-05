// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    public enum DataGridViewColumnHeadersHeightSizeMode
    {
        EnableResizing = 0,

        DisableResizing,
        
        AutoSize
    }

    public enum DataGridViewRowHeadersWidthSizeMode
    {
        EnableResizing = 0,

        DisableResizing,

        AutoSizeToAllHeaders,

        AutoSizeToDisplayedHeaders,

        AutoSizeToFirstHeader
    }

    [Flags]
    internal enum DataGridViewAutoSizeColumnCriteriaInternal
    {
        NotSet = 0x00,
        None = 0x01,
        Header = 0x02,
        AllRows = 0x04,
        DisplayedRows = 0x08,
        Fill = 0x10
    }

    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue"), // Intentionally has no 0 value because NotSet is used in DataGridViewAutoSizeColumnMode.
        SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags") // values are not combinable
    ]
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

    [
        SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags") // values are not combinable
    ]
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


    [Flags]
    internal enum DataGridViewAutoSizeRowsModeInternal
    {
        None = 0x00,
        Header = 0x01,
        AllColumns = 0x02,
        AllRows = 0x04,
        DisplayedRows = 0x08
    }

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

    [Flags]
    internal enum DataGridViewAutoSizeRowCriteriaInternal
    {
        Header = 0x01,
        AllColumns = 0x02
    }

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")] // Intentionally has no 0 value because values are combinations of DataGridViewAutoSizeRowCriteriaInternal.
    public enum DataGridViewAutoSizeRowMode
    {
        AllCells = DataGridViewAutoSizeRowCriteriaInternal.Header | DataGridViewAutoSizeRowCriteriaInternal.AllColumns,

        AllCellsExceptHeader = DataGridViewAutoSizeRowCriteriaInternal.AllColumns,

        RowHeader = DataGridViewAutoSizeRowCriteriaInternal.Header
    }
}
