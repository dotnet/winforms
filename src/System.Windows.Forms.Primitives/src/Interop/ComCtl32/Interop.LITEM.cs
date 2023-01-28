// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct LITEM
        {
            private const int MAX_LINKID_TEXT = 48;
            private const int L_MAX_URL_LENGTH = 2048 + 32 + 3;

            public LIST_ITEM_FLAGS mask;
            public int iLink;
            public LIST_ITEM_STATE_FLAGS state;
            public LIST_ITEM_STATE_FLAGS stateMask;
            public fixed char szID[MAX_LINKID_TEXT];
            public fixed char szUrl[L_MAX_URL_LENGTH];
        }
    }
}
