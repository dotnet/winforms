// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public class DataGridViewCellStyleContentChangedEventArgs : EventArgs
    {
        private DataGridViewCellStyle dataGridViewCellStyle;
        private bool changeAffectsPreferredSize;

        internal DataGridViewCellStyleContentChangedEventArgs(DataGridViewCellStyle dataGridViewCellStyle, bool changeAffectsPreferredSize)
        {
            this.dataGridViewCellStyle = dataGridViewCellStyle;
            this.changeAffectsPreferredSize = changeAffectsPreferredSize;
        }

        public DataGridViewCellStyle CellStyle
        {
            get
            {
                return this.dataGridViewCellStyle;
            }
        }

        public DataGridViewCellStyleScopes CellStyleScope
        {
            get
            {
                return this.dataGridViewCellStyle.Scope;
            }
        }

        internal bool ChangeAffectsPreferredSize
        {
            get
            {
                return this.changeAffectsPreferredSize;
            }
        }
    }
}
