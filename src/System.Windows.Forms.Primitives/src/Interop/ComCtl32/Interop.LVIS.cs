// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum LVIS : uint
        {
            /// <summary>
            /// The item has the focus, so it is surrounded by a standard focus rectangle.
            /// Although more than one item may be selected, only one item can have the focus.
            /// </summary>
            FOCUSED = 0x0001,

            /// <summary>
            /// The item is selected. The appearance of a selected item depends on whether
            /// it has the focus and also on the system colors used for selection.
            /// </summary>
            SELECTED = 0x0002,

            /// <summary>
            /// The item is marked for a cut-and-paste operation.
            /// </summary>
            CUT = 0x0004,

            /// <summary>
            /// The item is highlighted as a drag-and-drop target.
            /// </summary>
            DROPHILITED = 0x0008,

            /// <summary>
            /// The item's overlay image index is retrieved by a mask.
            /// </summary>
            OVERLAYMASK = 0x0F00,

            /// <summary>
            /// The item's state image index is retrieved by a mask.
            /// </summary>
            STATEIMAGEMASK = 0xF000,
        }
    }
}
