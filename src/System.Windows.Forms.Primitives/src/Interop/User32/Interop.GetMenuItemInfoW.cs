// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern BOOL GetMenuItemInfoW(IntPtr hmenu, int item, BOOL fByPosition, ref MENUITEMINFOW lpmii);

        public static BOOL GetMenuItemInfoW(HandleRef hmenu, int item, BOOL fByPosition, ref MENUITEMINFOW lpmii)
        {
            BOOL result = GetMenuItemInfoW(hmenu.Handle, item, fByPosition, ref lpmii);
            GC.KeepAlive(hmenu.Wrapper);
            return result;
        }
    }
}
