// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct MENUITEMINFOW
        {
            public uint cbSize;
            public MIIM fMask;
            public MFT fType;
            public MFS fState;
            public int wID;
            public IntPtr hSubMenu;
            public IntPtr hbmpChecked;
            public IntPtr hbmpUnchecked;
            public IntPtr dwItemData;
            public char* dwTypeData;
            public int cch;
            public IntPtr hbmpItem;
        }
    }
}
