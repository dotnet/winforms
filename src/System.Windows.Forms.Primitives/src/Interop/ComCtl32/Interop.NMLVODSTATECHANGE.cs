﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public struct NMLVODSTATECHANGE
        {
            public NMHDR hdr;
            public int iFrom;
            public int iTo;
            public LIST_VIEW_ITEM_STATE_FLAGS uNewState;
            public LIST_VIEW_ITEM_STATE_FLAGS uOldState;
        }
    }
}
