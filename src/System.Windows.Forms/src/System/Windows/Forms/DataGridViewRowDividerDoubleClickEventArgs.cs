// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public class DataGridViewRowDividerDoubleClickEventArgs : HandledMouseEventArgs
    {
        public DataGridViewRowDividerDoubleClickEventArgs(int rowIndex, HandledMouseEventArgs e) : base(e?.Button ?? MouseButtons.None, e?.Clicks ?? 0, e?.X ?? 0, e?.Y ?? 0, e?.Delta ?? 0, e?.Handled ?? false)
        {
            if (rowIndex < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            RowIndex = rowIndex;
        }

        public int RowIndex { get; }
    }
}
