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
        public static extern BOOL GetThemeSysBool(IntPtr hTheme, TMT iBoolId);

        public static BOOL GetThemeSysBool(IHandle hTheme, TMT iBoolId)
        {
            BOOL result = GetThemeSysBool(hTheme.Handle, iBoolId);
            GC.KeepAlive(hTheme);
            return result;
        }
    }
}
