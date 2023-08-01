// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public struct LVHITTESTINFO
        {
            public Point pt;
            public LVHITTESTINFO_FLAGS flags;
            public int iItem;
            public int iSubItem;
            public int iGroup;
        }
    }
}
