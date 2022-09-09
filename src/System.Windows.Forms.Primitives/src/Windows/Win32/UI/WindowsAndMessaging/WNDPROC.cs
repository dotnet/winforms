// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32.UI.WindowsAndMessaging
{
    internal delegate LRESULT WNDPROC(HWND hWnd, Interop.User32.WM msg, WPARAM wParam, LPARAM lParam);
}
