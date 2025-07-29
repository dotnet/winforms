// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public static unsafe partial class ControlPaint
{
    [Flags]
    internal enum ModernControlButtonStyle
    {
        Empty = 0x0,
        Up = 0x1,
        Down = 0x2,
        UpDown = 0x3,
        Right = 0x4,
        Left = 0x8,
        RightLeft = 0xC,
        Ellipse = 0x10,
        OpenDropDown = 0x20,
        SingleBorder = 0x10000,
        RoundedBorder = 0x20000,
    }
}
