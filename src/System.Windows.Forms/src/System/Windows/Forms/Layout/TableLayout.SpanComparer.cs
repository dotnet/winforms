// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Layout;

internal partial class TableLayout
{
    private abstract class SpanComparer : IComparer<LayoutInfo>
    {
        public abstract int GetSpan(LayoutInfo layoutInfo);

        public int Compare(LayoutInfo? x, LayoutInfo? y)
        {
            if (IComparerHelpers.CompareReturnIfNull(x, y, out int? returnValue))
            {
                return (int)returnValue;
            }

            return GetSpan(x) - GetSpan(y);
        }
    }
}
