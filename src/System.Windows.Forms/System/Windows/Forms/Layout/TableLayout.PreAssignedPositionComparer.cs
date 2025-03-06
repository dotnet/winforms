// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Layout;

internal partial class TableLayout
{
    private class PreAssignedPositionComparer : IComparer<LayoutInfo>
    {
        public static PreAssignedPositionComparer GetInstance { get; } = new();

        public int Compare(LayoutInfo? x, LayoutInfo? y)
        {
            if (IComparerHelpers.CompareReturnIfNull(x, y, out int? returnValue))
            {
                return (int)returnValue;
            }

            if (x.RowPosition < y.RowPosition)
            {
                return -1;
            }

            if (x.RowPosition > y.RowPosition)
            {
                return 1;
            }

            if (x.ColumnPosition < y.ColumnPosition)
            {
                return -1;
            }

            if (x.ColumnPosition > y.ColumnPosition)
            {
                return 1;
            }

            return 0;
        }
    }
}
