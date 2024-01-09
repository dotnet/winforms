// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="DispatchMessage(MSG*)"/>
    [DllImport(Libraries.User32, ExactSpelling = true)]
    public static extern unsafe LRESULT DispatchMessageA(MSG* msg);
}
