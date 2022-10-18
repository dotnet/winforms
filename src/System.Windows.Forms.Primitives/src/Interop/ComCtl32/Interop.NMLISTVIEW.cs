// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public struct NMLISTVIEW
        {
            public NMHDR hdr;
            public int iItem;
            public int iSubItem;
            public LIST_VIEW_ITEM_STATE_FLAGS uNewState;
            public LIST_VIEW_ITEM_STATE_FLAGS uOldState;
            public LIST_VIEW_ITEM_FLAGS uChanged;
            public Point ptAction;
            public IntPtr lParam;
        }
    }
}
