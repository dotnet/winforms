﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public class DataGridViewColumnDividerDoubleClickEventArgs : HandledMouseEventArgs
    {
        public DataGridViewColumnDividerDoubleClickEventArgs(int columnIndex, HandledMouseEventArgs e) : base(e?.Button ?? MouseButtons.None, e?.Clicks ?? 0, e?.X ?? 0, e?.Y ?? 0, e?.Delta ?? 0, e?.Handled ?? false)
        {
            if (columnIndex < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            }
            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            ColumnIndex = columnIndex;
        }

        public int ColumnIndex { get; }
    }
}
