﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public struct NMLVGETINFOTIPW
        {
            public NMHDR nmhdr;
            public NMLVGETINFOTIP_FLAGS flags;
            public IntPtr lpszText;
            public int cchTextMax;
            public int item;
            public int subItem;
            public IntPtr lParam;
        }
    }
}
