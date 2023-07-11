// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.VisualStyles;

public enum HitTestCode
{
    Nowhere = (int)PInvoke.HTNOWHERE,
    Client = (int)PInvoke.HTCLIENT,
    Left = (int)PInvoke.HTLEFT,
    Right = (int)PInvoke.HTRIGHT,
    Top = (int)PInvoke.HTTOP,
    Bottom = (int)PInvoke.HTBOTTOM,
    TopLeft = (int)PInvoke.HTTOPLEFT,
    TopRight = (int)PInvoke.HTTOPRIGHT,
    BottomLeft = (int)PInvoke.HTBOTTOMLEFT,
    BottomRight = (int)PInvoke.HTBOTTOMRIGHT
}
