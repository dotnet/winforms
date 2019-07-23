// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.VisualStyles
{
    public enum HitTestCode
    {
        Nowhere = 0,
        Client = 1,
        Left = 10,
        Right = 11,
        Top = 12,
        Bottom = 15,
        TopLeft = 13,
        TopRight = 14,
        BottomLeft = 16,
        BottomRight = 17
        //		#define HTNOWHERE           0
        //		#define HTCLIENT            1
        //		#define HTLEFT              10
        //		#define HTRIGHT             11
        //		#define HTTOP               12
        //		#define HTTOPLEFT           13
        //		#define HTTOPRIGHT          14
        //		#define HTBOTTOM            15
        //		#define HTBOTTOMLEFT        16
        //		#define HTBOTTOMRIGHT       17
    }
}
