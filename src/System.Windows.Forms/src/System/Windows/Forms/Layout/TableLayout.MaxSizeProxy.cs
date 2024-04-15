// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Layout;

internal partial class TableLayout
{
    private class MaxSizeProxy : SizeProxy
    {
        public override int Size
        {
            get { return strip.MaxSize; }
            set { strip.MaxSize = value; }
        }

        public static MaxSizeProxy GetInstance { get; } = new();
    }
}
