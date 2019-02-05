// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    public class ColumnWidthChangingEventArgs : CancelEventArgs
    {
        public ColumnWidthChangingEventArgs(int columnIndex, int newWidth) : this(columnIndex, newWidth, false)
        {
        }

        public ColumnWidthChangingEventArgs(int columnIndex, int newWidth, bool cancel) : base(cancel)
        {
            ColumnIndex = columnIndex;
            NewWidth = newWidth;
        }

        /// <devdoc>
        /// Returns the index of the column header whose width is changing
        /// </devdoc>
        public int ColumnIndex { get; }

        /// <devdoc>
        /// Returns the new width for the column header who is changing
        /// </devdoc>
        public int NewWidth { get; set; }
    }
}
