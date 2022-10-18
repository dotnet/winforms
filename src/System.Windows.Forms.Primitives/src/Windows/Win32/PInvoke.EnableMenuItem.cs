// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static BOOL EnableMenuItem<T>(T hMenu, Interop.User32.SC uIDEnableItem, MENU_ITEM_FLAGS uEnable)
            where T : IHandle<HMENU>
        {
            BOOL result = EnableMenuItem(hMenu.Handle, (uint)uIDEnableItem, uEnable);
            GC.KeepAlive(hMenu.Wrapper);
            return result;
        }
    }
}
