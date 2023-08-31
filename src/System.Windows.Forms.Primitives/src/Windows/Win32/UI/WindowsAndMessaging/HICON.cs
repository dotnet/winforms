// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.WindowsAndMessaging;

internal partial struct HICON : IHandle<HICON>
{
    HICON IHandle<HICON>.Handle => this;
    object? IHandle<HICON>.Wrapper => null;
}
