// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.Windows.Forms
{
    public class DataGridViewRowsRemovedEventArgs : EventArgs
    {
        public DataGridViewRowsRemovedEventArgs(int rowIndex, int rowCount)
        {
            if (rowIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex), string.Format(SR.InvalidLowBoundArgumentEx, nameof(rowIndex), rowIndex.ToString(CultureInfo.CurrentCulture), 0));
            }
            if (rowCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowCount), string.Format(SR.InvalidLowBoundArgumentEx, nameof(rowCount), rowCount.ToString(CultureInfo.CurrentCulture), 1));
            }

            RowIndex = rowIndex;
            RowCount = rowCount;
        }

        public int RowIndex { get; }

        public int RowCount { get; }
    }
}
