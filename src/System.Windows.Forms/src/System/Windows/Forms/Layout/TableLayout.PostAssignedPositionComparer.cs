// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;

namespace System.Windows.Forms.Layout
{
    internal partial class TableLayout
    {
        private class PostAssignedPositionComparer : IComparer
        {
            private static readonly PostAssignedPositionComparer instance = new PostAssignedPositionComparer();

            public static PostAssignedPositionComparer GetInstance
            {
                get { return instance; }
            }

            public int Compare(object x, object y)
            {
                LayoutInfo xInfo = (LayoutInfo)x;
                LayoutInfo yInfo = (LayoutInfo)y;
                if (xInfo.RowStart < yInfo.RowStart)
                {
                    return -1;
                }

                if (xInfo.RowStart > yInfo.RowStart)
                {
                    return 1;
                }

                if (xInfo.ColumnStart < yInfo.ColumnStart)
                {
                    return -1;
                }

                if (xInfo.ColumnStart > yInfo.ColumnStart)
                {
                    return 1;
                }

                return 0;
            }
        }
    }
}
