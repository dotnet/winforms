// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;

    /// <include file='doc\DataGridViewCellToolTipTextNeededEventArgs.uex' path='docs/doc[@for="DataGridViewCellToolTipTextNeededEventArgs"]/*' />
    public class DataGridViewCellToolTipTextNeededEventArgs : DataGridViewCellEventArgs
    {
        private string toolTipText;

        internal DataGridViewCellToolTipTextNeededEventArgs(
            int columnIndex, 
            int rowIndex,
            string toolTipText) : base(columnIndex, rowIndex)
        {
            this.toolTipText = toolTipText;
        }

        /// <include file='doc\DataGridViewCellToolTipTextNeededEventArgs.uex' path='docs/doc[@for="DataGridViewCellToolTipTextNeededEventArgs.ToolTipText"]/*' />
        public string ToolTipText
        {
            get
            {
                return this.toolTipText;
            }
            set
            {
                this.toolTipText = value;
            }
        }
    }
}
