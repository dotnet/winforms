// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Layout
{
    internal partial class TableLayout
    {
        private class PreAssignedPositionComparer : IComparer<LayoutInfo>
        {
            private static readonly PreAssignedPositionComparer instance = new PreAssignedPositionComparer();

            public static PreAssignedPositionComparer GetInstance
            {
                get { return instance; }
            }

            public int Compare(LayoutInfo? x, LayoutInfo? y)
            {
                if (x is null && y is null)
                {
                    return 0;
                }

                if (x is null)
                {
                    return -1;
                }

                if (y is null)
                {
                    return 1;
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
}
