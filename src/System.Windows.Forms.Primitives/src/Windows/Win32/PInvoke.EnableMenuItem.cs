// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="EnableMenuItem(HMENU, uint, MENU_ITEM_FLAGS)"/>
    public static BOOL EnableMenuItem<T>(T hMenu, uint uIDEnableItem, MENU_ITEM_FLAGS uEnable)
        where T : IHandle<HMENU>
    {
        BOOL result = EnableMenuItem(hMenu.Handle, uIDEnableItem, uEnable);
        GC.KeepAlive(hMenu.Wrapper);
        return result;
    }
}
