// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewColumnHeadersHeightSizeMode.DataGridViewColumnHeadersHeightSizeMode"]/*' />
    public enum DataGridViewColumnHeadersHeightSizeMode
    {
        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewColumnHeadersHeightSizeMode.EnableResizing"]/*' />
        EnableResizing = 0,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewColumnHeadersHeightSizeMode.DisableResizing"]/*' />
        DisableResizing,
        
        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewColumnHeadersHeightSizeMode.AutoSize"]/*' />
        AutoSize
    }

    /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewRowHeadersWidthSizeMode.DataGridViewRowHeadersWidthSizeMode"]/*' />
    public enum DataGridViewRowHeadersWidthSizeMode
    {
        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewRowHeadersWidthSizeMode.EnableResizing"]/*' />
        EnableResizing = 0,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewRowHeadersWidthSizeMode.DisableResizing"]/*' />
        DisableResizing,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders"]/*' />
        AutoSizeToAllHeaders,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders"]/*' />
        AutoSizeToDisplayedHeaders,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader"]/*' />
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

    /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnsMode.DataGridViewAutoSizeColumnsMode"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue"), // Intentionally has no 0 value because NotSet is used in DataGridViewAutoSizeColumnMode.
        SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags") // values are not combinable
    ]
    public enum DataGridViewAutoSizeColumnsMode
    {
        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnsMode.AllCells"]/*' />
        AllCells = DataGridViewAutoSizeColumnCriteriaInternal.Header | DataGridViewAutoSizeColumnCriteriaInternal.AllRows,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader"]/*' />
        AllCellsExceptHeader = DataGridViewAutoSizeColumnCriteriaInternal.AllRows,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnsMode.DisplayedCells"]/*' />
        DisplayedCells = DataGridViewAutoSizeColumnCriteriaInternal.Header | DataGridViewAutoSizeColumnCriteriaInternal.DisplayedRows,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader"]/*' />
        DisplayedCellsExceptHeader = DataGridViewAutoSizeColumnCriteriaInternal.DisplayedRows,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnsMode.None"]/*' />
        None = DataGridViewAutoSizeColumnCriteriaInternal.None,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnsMode.ColumnHeader"]/*' />
        ColumnHeader = DataGridViewAutoSizeColumnCriteriaInternal.Header,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnsMode.Fill"]/*' />
        Fill = DataGridViewAutoSizeColumnCriteriaInternal.Fill
    }

    /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnMode.DataGridViewAutoSizeColumnMode"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags") // values are not combinable
    ]
    public enum DataGridViewAutoSizeColumnMode
    {
        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnMode.NotSet"]/*' />
        NotSet = DataGridViewAutoSizeColumnCriteriaInternal.NotSet,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnMode.None"]/*' />
        None = DataGridViewAutoSizeColumnCriteriaInternal.None,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnMode.AllCells"]/*' />
        AllCells = DataGridViewAutoSizeColumnCriteriaInternal.Header | DataGridViewAutoSizeColumnCriteriaInternal.AllRows,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnMode.AllCellsExceptHeader"]/*' />
        AllCellsExceptHeader = DataGridViewAutoSizeColumnCriteriaInternal.AllRows,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnMode.DisplayedCells"]/*' />
        DisplayedCells = DataGridViewAutoSizeColumnCriteriaInternal.Header | DataGridViewAutoSizeColumnCriteriaInternal.DisplayedRows,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader"]/*' />
        DisplayedCellsExceptHeader = DataGridViewAutoSizeColumnCriteriaInternal.DisplayedRows,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnMode.ColumnHeader"]/*' />
        ColumnHeader = DataGridViewAutoSizeColumnCriteriaInternal.Header,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnMode.Fill"]/*' />
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

    /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeRowsMode.DataGridViewAutoSizeRowsMode"]/*' />
    public enum DataGridViewAutoSizeRowsMode
    {
        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeRowsMode.AllCells"]/*' />
        AllCells = DataGridViewAutoSizeRowsModeInternal.Header | DataGridViewAutoSizeRowsModeInternal.AllColumns | DataGridViewAutoSizeRowsModeInternal.AllRows,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders"]/*' />
        AllCellsExceptHeaders = DataGridViewAutoSizeRowsModeInternal.AllColumns | DataGridViewAutoSizeRowsModeInternal.AllRows,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeRowsMode.AllHeaders"]/*' />
        AllHeaders = DataGridViewAutoSizeRowsModeInternal.Header | DataGridViewAutoSizeRowsModeInternal.AllRows,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeRowsMode.DisplayedCells"]/*' />
        DisplayedCells = DataGridViewAutoSizeRowsModeInternal.Header | DataGridViewAutoSizeRowsModeInternal.AllColumns | DataGridViewAutoSizeRowsModeInternal.DisplayedRows,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders"]/*' />
        DisplayedCellsExceptHeaders = DataGridViewAutoSizeRowsModeInternal.AllColumns | DataGridViewAutoSizeRowsModeInternal.DisplayedRows,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeRowsMode.DisplayedHeaders"]/*' />
        DisplayedHeaders = DataGridViewAutoSizeRowsModeInternal.Header | DataGridViewAutoSizeRowsModeInternal.DisplayedRows,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeRowsMode.None"]/*' />
        None = DataGridViewAutoSizeRowsModeInternal.None,
    }

    [Flags]
    internal enum DataGridViewAutoSizeRowCriteriaInternal
    {
        Header = 0x01,
        AllColumns = 0x02
    }

    /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeRowMode.DataGridViewAutoSizeRowMode"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")] // Intentionally has no 0 value because values are combinations of DataGridViewAutoSizeRowCriteriaInternal.
    public enum DataGridViewAutoSizeRowMode
    {
        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeRowMode.AllCells"]/*' />
        AllCells = DataGridViewAutoSizeRowCriteriaInternal.Header | DataGridViewAutoSizeRowCriteriaInternal.AllColumns,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeRowMode.AllCellsExceptHeader"]/*' />
        AllCellsExceptHeader = DataGridViewAutoSizeRowCriteriaInternal.AllColumns,

        /// <include file='doc\DataGridViewAutoSizeEnums.uex' path='docs/doc[@for="DataGridViewAutoSizeRowMode.RowHeader"]/*' />
        RowHeader = DataGridViewAutoSizeRowCriteriaInternal.Header
    }
}
