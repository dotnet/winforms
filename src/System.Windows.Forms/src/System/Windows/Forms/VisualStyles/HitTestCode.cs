// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.User32;

namespace System.Windows.Forms.VisualStyles
{
    public enum HitTestCode
    {
        Nowhere = HT.NOWHERE,
        Client = HT.CLIENT,
        Left = HT.LEFT,
        Right = HT.RIGHT,
        Top = HT.TOP,
        Bottom = HT.BOTTOM,
        TopLeft = HT.TOPLEFT,
        TopRight = HT.TOPRIGHT,
        BottomLeft = HT.BOTTOMLEFT,
        BottomRight = HT.BOTTOMRIGHT
    }
}
