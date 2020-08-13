// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public struct NMLISTVIEW
        {
            public User32.NMHDR hdr;
            public int iItem;
            public int iSubItem;
            public LVIS uNewState;
            public LVIS uOldState;
            public LVIF uChanged;
            public Point ptAction;
            public IntPtr lParam;
        }
    }
}
