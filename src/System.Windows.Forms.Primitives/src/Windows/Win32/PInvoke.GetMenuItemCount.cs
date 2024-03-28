// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="GetMenuItemCount(HMENU)"/>
    public static int GetMenuItemCount<T>(T hMenu)
        where T : IHandle<HMENU>
    {
        int result = GetMenuItemCount(hMenu.Handle);
        GC.KeepAlive(hMenu.Wrapper);
        return result;
    }
}
