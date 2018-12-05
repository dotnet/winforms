// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System.Windows.Forms;
    using System;

    public class DataGridViewEditingControlShowingEventArgs : EventArgs
    {
        Control control = null;
        DataGridViewCellStyle cellStyle;

        public DataGridViewEditingControlShowingEventArgs(Control control, DataGridViewCellStyle cellStyle)
        {
            this.control = control;
            this.cellStyle = cellStyle;
        }

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

        public Control Control
        {
            get
            {
                return this.control;
            }
        }
    }
}
