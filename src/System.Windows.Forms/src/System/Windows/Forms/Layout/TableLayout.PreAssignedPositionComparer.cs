// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;

namespace System.Windows.Forms.Layout
{
    internal partial class TableLayout
    {
        private class PreAssignedPositionComparer : IComparer
        {
            private static readonly PreAssignedPositionComparer instance = new PreAssignedPositionComparer();

            public static PreAssignedPositionComparer GetInstance
            {
                get { return instance; }
            }

            public int Compare(object x, object y)
            {
                LayoutInfo xInfo = (LayoutInfo)x;
                LayoutInfo yInfo = (LayoutInfo)y;
                if (xInfo.RowPosition < yInfo.RowPosition)
                {
                    return -1;
                }

                if (xInfo.RowPosition > yInfo.RowPosition)
                {
                    return 1;
                }

                if (xInfo.ColumnPosition < yInfo.ColumnPosition)
                {
                    return -1;
                }

                if (xInfo.ColumnPosition > yInfo.ColumnPosition)
                {
                    return 1;
                }

                return 0;
            }
        }
    }
}
