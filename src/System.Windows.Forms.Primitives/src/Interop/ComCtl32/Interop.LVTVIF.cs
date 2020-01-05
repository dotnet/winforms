// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum LVTVIF : uint
        {
            /// <summary>
            /// Size the tiles automatically.
            /// </summary>
            AUTOSIZE = 0x00,

            /// <summary>
            /// Apply a fixed width to the tiles.
            /// </summary>
            FIXEDWIDTH = 0x01,

            /// <summary>
            /// Apply a fixed height to the tiles.
            /// </summary>
            FIXEDHEIGHT = 0x02,

            /// <summary>
            /// Apply a fixed height and width to the tiles.
            /// </summary>
            FIXEDSIZE = 0x03,

            /// <summary>
            /// This flag is not supported and should not be used.
            /// </summary>
            EXTENDED = 0x04
        }
    }
}
