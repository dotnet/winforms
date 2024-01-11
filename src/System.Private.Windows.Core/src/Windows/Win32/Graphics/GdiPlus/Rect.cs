// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.CompilerServices;

namespace Windows.Win32.Graphics.GdiPlus;

internal partial struct Rect
{
    public static implicit operator Rectangle(Rect rect) => Unsafe.As<Rect, Rectangle>(ref rect);
    public static implicit operator Rect(Rectangle rectangle) => Unsafe.As<Rectangle, Rect>(ref rectangle);
    public static explicit operator RectF(Rect rect) =>
        new() { X = rect.X, Y = rect.Y, Width = rect.Width, Height = rect.Height };
}
