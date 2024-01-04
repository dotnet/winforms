// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.Gdi;

internal readonly partial struct HDC : IHandle<HDC>
{
    HDC IHandle<HDC>.Handle => this;
    object? IHandle<HDC>.Wrapper => null;

    public static HDC Null => default;
    public bool IsNull => Value == 0;
}
