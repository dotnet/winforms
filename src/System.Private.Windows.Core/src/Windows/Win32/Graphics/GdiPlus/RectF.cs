// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.CompilerServices;

namespace Windows.Win32.Graphics.GdiPlus;

internal partial struct RectF
{
    public static implicit operator RectangleF(RectF rect) => Unsafe.As<RectF, RectangleF>(ref rect);
    public static implicit operator RectF(RectangleF rectangle) => Unsafe.As<RectangleF, RectF>(ref rectangle);
}
