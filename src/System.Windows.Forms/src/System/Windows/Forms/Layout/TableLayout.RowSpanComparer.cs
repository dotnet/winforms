// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Layout
{
    internal partial class TableLayout
    {
        private class RowSpanComparer : SpanComparer
        {
            private static readonly RowSpanComparer instance = new RowSpanComparer();

            public override int GetSpan(LayoutInfo layoutInfo)
            {
                return layoutInfo.RowSpan;
            }

            public static RowSpanComparer GetInstance
            {
                get { return instance; }
            }
        }
    }
}
