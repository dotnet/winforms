// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Layout;

internal partial class TableLayout
{
    internal struct Strip
    {
        public int MinSize { get; set; }

        public int MaxSize { get; set; }

        public bool IsStart { get; set; }
    }
}
