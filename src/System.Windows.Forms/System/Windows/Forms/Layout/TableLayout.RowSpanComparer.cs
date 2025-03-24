// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Layout;

internal partial class TableLayout
{
    private class RowSpanComparer : SpanComparer
    {
        public override int GetSpan(LayoutInfo layoutInfo) => layoutInfo.RowSpan;

        public static RowSpanComparer GetInstance { get; } = new();
    }
}
