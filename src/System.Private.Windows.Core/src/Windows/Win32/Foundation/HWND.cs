// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Foundation;

internal partial struct HWND : IHandle<HWND>
{
    HWND IHandle<HWND>.Handle => this;
    object? IHandle<HWND>.Wrapper => null;
}
