// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public class DataGridViewCellStyleContentChangedEventArgs : EventArgs
    {
        internal DataGridViewCellStyleContentChangedEventArgs(DataGridViewCellStyle dataGridViewCellStyle, bool changeAffectsPreferredSize)
        {
            CellStyle = dataGridViewCellStyle;
            ChangeAffectsPreferredSize = changeAffectsPreferredSize;
        }

        public DataGridViewCellStyle CellStyle { get; }

        public DataGridViewCellStyleScopes CellStyleScope => CellStyle.Scope;

        internal bool ChangeAffectsPreferredSize { get; }
    }
}
