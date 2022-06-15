// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;

namespace System.Windows.Forms.Layout
{
    internal partial class TableLayout
    {
        private abstract class SpanComparer : IComparer
        {
            public abstract int GetSpan(LayoutInfo layoutInfo);

            public int Compare(object x, object y)
            {
                LayoutInfo xInfo = (LayoutInfo)x;
                LayoutInfo yInfo = (LayoutInfo)y;
                return GetSpan(xInfo) - GetSpan(yInfo);
            }
        }
    }
}
