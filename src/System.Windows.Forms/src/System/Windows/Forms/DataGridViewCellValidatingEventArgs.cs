// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;

    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class DataGridViewCellValidatingEventArgs : CancelEventArgs
    {
        private int rowIndex, columnIndex;
        private object formattedValue;

        internal DataGridViewCellValidatingEventArgs(int columnIndex, int rowIndex, object formattedValue)
        {
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
            this.formattedValue = formattedValue;
        }

        public int ColumnIndex
        {
            get
            {
                return this.columnIndex;
            }
        }

        public object FormattedValue
        {
            get
            {
                return this.formattedValue;
            }
        }

        public int RowIndex
        {
            get
            {
                return this.rowIndex;
            }
        }
    }
}
