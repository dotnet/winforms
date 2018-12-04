// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System.Windows.Forms;
    using System;

    /// <include file='doc\DataGridViewEditingControlShowingEventArgs.uex' path='docs/doc[@for="DataGridViewEditingControlShowingEventArgs"]/*' />
    public class DataGridViewEditingControlShowingEventArgs : EventArgs
    {
        Control control = null;
        DataGridViewCellStyle cellStyle;

        /// <include file='doc\DataGridViewEditingControlShowingEventArgs.uex' path='docs/doc[@for="DataGridViewEditingControlShowingEventArgs.DataGridViewEditingControlShowingEventArgs"]/*' />
        public DataGridViewEditingControlShowingEventArgs(Control control, DataGridViewCellStyle cellStyle)
        {
            this.control = control;
            this.cellStyle = cellStyle;
        }

        /// <include file='doc\DataGridViewEditingControlShowingEventArgs.uex' path='docs/doc[@for="DataGridViewEditingControlShowingEventArgs.CellStyle"]/*' />
        public DataGridViewCellStyle CellStyle
        {
            get
            {
                return this.cellStyle;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                this.cellStyle = value;
            }
        }

        /// <include file='doc\DataGridViewEditingControlShowingEventArgs.uex' path='docs/doc[@for="DataGridViewEditingControlShowingEventArgs.Control"]/*' />
        public Control Control
        {
            get
            {
                return this.control;
            }
        }
    }
}
