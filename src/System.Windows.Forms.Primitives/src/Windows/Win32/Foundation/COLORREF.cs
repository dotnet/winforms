// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32.Foundation;

internal readonly partial struct COLORREF
{
    public static implicit operator COLORREF(Color color) => new((uint)ColorTranslator.ToWin32(color));
    public static implicit operator Color(COLORREF color) => ColorTranslator.FromWin32((int)color.Value);
    public static implicit operator COLORREF(int color) => new((uint)color);
}
