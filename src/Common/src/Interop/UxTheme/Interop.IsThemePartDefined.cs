// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    public static partial class UxTheme
    {
        [DllImport(Libraries.UxTheme, ExactSpelling = true)]
        public static extern BOOL IsThemePartDefined(IntPtr hTheme, int iPartId, int iStateId);

        public static BOOL IsThemePartDefined(IHandle hTheme, int iPartId, int iStateId)
        {
            BOOL result = IsThemePartDefined(hTheme.Handle, iPartId, iStateId);
            GC.KeepAlive(hTheme);
            return result;
        }
    }
}
