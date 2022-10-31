// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public unsafe struct TCITEMW
        {
            public TCITEMHEADERA_MASK mask;
            public TAB_CONTROL_ITEM_STATE dwState;
            public TAB_CONTROL_ITEM_STATE dwStateMask;
            public char* pszText;
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
        }
    }
}
