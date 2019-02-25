// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public class DataGridViewRowsAddedEventArgs : EventArgs
    {
        public DataGridViewRowsAddedEventArgs(int rowIndex, int rowCount)
        {
            RowIndex = rowIndex;
            RowCount = rowCount;
        }

        public int RowIndex { get; }

        public int RowCount { get; }
    }
}
