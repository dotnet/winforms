// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        /// <summary>
        ///  Scroll bar values (SB_) that indicates the user's scrolling request in a vertical scrollbar.
        ///  Used by WM_VSCROLL message.
        /// </summary>
        public enum SBV : int
        {
            LINEUP = 0,
            LINEDOWN = 1,
            PAGEUP = 2,
            PAGEDOWN = 3,
            THUMBPOSITION = 4,
            THUMBTRACK = 5,
            TOP = 6,
            BOTTOM = 7,
            ENDSCROLL = 8,
        }
    }
}
