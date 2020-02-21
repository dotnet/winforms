// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum LVHT : uint
        {
            /// <summary>
            /// The position is inside the list-view control's client window, but it is not over a list item.
            /// </summary>
            NOWHERE = 0x0001,

            /// <summary>
            /// The position is over a list-view item's icon.
            /// </summary>
            ONITEMICON = 0x0002,

            /// <summary>
            /// The position is over a list-view item's text.
            /// </summary>
            ONITEMLABEL = 0x0004,

            /// <summary>
            /// The position is above the control's client area.
            /// </summary>
            ABOVE = 0x0008,

            /// <summary>
            /// The position is below the control's client area.
            /// </summary>
            BELOW = 0x0010,

            /// <summary>
            /// The position is to the right of the list-view control's client area.
            /// </summary>
            TORIGHT = 0x0020,

            /// <summary>
            /// The position is to the left of the list-view control's client area.
            /// </summary>
            TOLEFT = 0x0040,

            /// <summary>
            /// The position is over a list-view item.
            /// </summary>
            ONITEM = ONITEMICON | ONITEMLABEL | ABOVE,

            /// <summary>
            /// The position is over the state image of a list-view item.
            /// </summary>
            ONITEMSTATEICON = 0x0008,

            /// <summary>
            /// The point is within the group header.
            /// </summary>
            EX_GROUP_HEADER = 0x10000000,

            /// <summary>
            /// The point is within the group footer.
            /// </summary>
            EX_GROUP_FOOTER = 0x20000000,

            /// <summary>
            /// The point is within the collapse/expand button of the group.
            /// </summary>
            EX_GROUP_COLLAPSE = 0x40000000,

            /// <summary>
            /// The point is within the area of the group where items are displayed.
            /// </summary>
            EX_GROUP_BACKGROUND = 0x80000000,

            /// <summary>
            /// The point is within the state icon of the group.
            /// </summary>
            EX_GROUP_STATEICON = 0x01000000,

            /// <summary>
            /// The point is within the subset link of the group.
            /// </summary>
            EX_GROUP_SUBSETLINK = 0x02000000,

            /// <summary>
            /// The point is within the area of the group where items are displayed.
            /// </summary>
            EX_GROUP = EX_GROUP_BACKGROUND | EX_GROUP_COLLAPSE | EX_GROUP_FOOTER | EX_GROUP_HEADER | EX_GROUP_STATEICON | EX_GROUP_SUBSETLINK,

            /// <summary>
            /// The point is within the icon or text content of the item and not on the background.
            /// </summary>
            EX_ONCONTENTS = 0x04000000,

            /// <summary>
            /// The point is within the footer of the list-view control.
            /// </summary>
            EX_FOOTER = 0x08000000
        }
    }
}
