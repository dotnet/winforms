// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [LibraryImport(Libraries.User32)]
        public static partial BOOL GetMenuItemInfoW(IntPtr hmenu, int item, BOOL fByPosition, ref MENUITEMINFOW lpmii);

        public static BOOL GetMenuItemInfoW(HandleRef hmenu, int item, BOOL fByPosition, ref MENUITEMINFOW lpmii)
        {
            BOOL result = GetMenuItemInfoW(hmenu.Handle, item, fByPosition, ref lpmii);
            GC.KeepAlive(hmenu.Wrapper);
            return result;
        }
    }
}
