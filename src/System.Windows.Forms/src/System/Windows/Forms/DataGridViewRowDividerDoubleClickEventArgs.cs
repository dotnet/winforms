// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <include file='doc\DataGridViewRowDividerDoubleClickEventArgs.uex' path='docs/doc[@for="DataGridViewRowDividerDoubleClickEventArgs"]/*' />
    public class DataGridViewRowDividerDoubleClickEventArgs : HandledMouseEventArgs
    {
        private int rowIndex;

        /// <include file='doc\DataGridViewRowDividerDoubleClickEventArgs.uex' path='docs/doc[@for="DataGridViewRowDividerDoubleClickEventArgs.DataGridViewRowDividerDoubleClickEventArgs"]/*' />
        public DataGridViewRowDividerDoubleClickEventArgs(int rowIndex, HandledMouseEventArgs e) : base(e.Button, e.Clicks, e.X, e.Y, e.Delta, e.Handled)
        {
            if (rowIndex < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }
            this.rowIndex = rowIndex;
        }

        /// <include file='doc\DataGridViewRowDividerDoubleClickEventArgs.uex' path='docs/doc[@for="DataGridViewRowDividerDoubleClickEventArgs.RowIndex"]/*' />
        public int RowIndex
        {
            get
            {
                return this.rowIndex;
            }
        }
    }
}
