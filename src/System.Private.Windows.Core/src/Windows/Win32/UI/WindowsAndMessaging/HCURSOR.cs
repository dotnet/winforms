// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.WindowsAndMessaging;

internal unsafe partial struct HCURSOR : IHandle<HCURSOR>
{
    HCURSOR IHandle<HCURSOR>.Handle => this;
    object? IHandle<HCURSOR>.Wrapper => null;

    public static explicit operator HCURSOR(HANDLE handle) => new((nint)handle);
    public static implicit operator HANDLE(HCURSOR handle) => new((nint)handle);
}
