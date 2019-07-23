// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.VisualStyles
{
    [Flags]
    public enum Edges
    {
        Left = 0x0001,
        Top = 0x0002,
        Right = 0x0004,
        Bottom = 0x0008,
        Diagonal = 0x0010,

        //  #define BF_LEFT         0x0001
        //  #define BF_TOP          0x0002
        //  #define BF_RIGHT        0x0004
        //  #define BF_BOTTOM       0x0008
        //
        //  #define BF_TOPLEFT      (BF_TOP | BF_LEFT)
        //  #define BF_TOPRIGHT     (BF_TOP | BF_RIGHT)
        //  #define BF_BOTTOMLEFT   (BF_BOTTOM | BF_LEFT)
        //  #define BF_BOTTOMRIGHT  (BF_BOTTOM | BF_RIGHT)
        //  #define BF_RECT         (BF_LEFT | BF_TOP | BF_RIGHT | BF_BOTTOM)
        //
        //  #define BF_DIAGONAL     0x0010

        //  // For diagonal lines, the BF_RECT flags specify the end point of the
        //  // vector bounded by the rectangle parameter.
        //  #define BF_DIAGONAL_ENDTOPRIGHT     (BF_DIAGONAL | BF_TOP | BF_RIGHT)
        //  #define BF_DIAGONAL_ENDTOPLEFT      (BF_DIAGONAL | BF_TOP | BF_LEFT)
        //  #define BF_DIAGONAL_ENDBOTTOMLEFT   (BF_DIAGONAL | BF_BOTTOM | BF_LEFT)
        //  #define BF_DIAGONAL_ENDBOTTOMRIGHT  (BF_DIAGONAL | BF_BOTTOM | BF_RIGHT)
    }
}
