// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Internal
{
    [Flags]
    internal enum WindowsPenStyle
    {
        Solid = 0,
        Dash = 1,           /* -------  */
        Dot = 2,            /* .......  */
        DashDot = 3,        /* _._._._  */
        DashDotDot = 4,     /* _.._.._  */
        Null = 5,
        InsideFrame = 6,
        UserStyle = 7,
        Alternate = 8,

        // endcap style
        EndcapRound = 0x00000000,
        EndcapSquare = 0x00000100,
        EndcapFlat = 0x00000200,

        // join style
        JoinRound = 0x00000000,
        JoinBevel = 0x00001000,
        JoinMiter = 0x00002000,

        // pen type style
        Cosmetic = 0x00000000,
        Geometric = 0x00010000,

        Default = 0x00000000 // Solid | EndcapRound | JoinRound | Cosmetic

        // (From wingdi.h)
        /* Pen Styles */
        /* Pen Styles */
        //#define PS_SOLID            0
        //#define PS_DASH             1       /* -------  */
        //#define PS_DOT              2       /* .......  */
        //#define PS_DASHDOT          3       /* _._._._  */
        //#define PS_DASHDOTDOT       4       /* _.._.._  */
        //#define PS_NULL             5
        //#define PS_INSIDEFRAME      6
        //#define PS_USERSTYLE        7
        //#define PS_ALTERNATE        8
        //#define PS_STYLE_MASK       0x0000000F
        //
        //#define PS_ENDCAP_ROUND     0x00000000
        //#define PS_ENDCAP_SQUARE    0x00000100
        //#define PS_ENDCAP_FLAT      0x00000200
        //#define PS_ENDCAP_MASK      0x00000F00
        //
        //#define PS_JOIN_ROUND       0x00000000
        //#define PS_JOIN_BEVEL       0x00001000
        //#define PS_JOIN_MITER       0x00002000
        //#define PS_JOIN_MASK        0x0000F000
        //
        //#define PS_COSMETIC         0x00000000
        //#define PS_GEOMETRIC        0x00010000
        //#define PS_TYPE_MASK        0x000F0000
    }
}
