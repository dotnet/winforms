// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public struct TBBUTTONINFOW
        {
            public uint cbSize;
            public TBIF dwMask;
            public int idCommand;
            public int iImage;
            public TBSTATE fsState;
            public byte fsStyle;
            public ushort cx;
            public IntPtr lParam;
            public IntPtr pszText;
            public int cchTest;
        }
    }
}
