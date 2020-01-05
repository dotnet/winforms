// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [DllImport(Libraries.Ole32, ExactSpelling = true)]
        public unsafe static extern BOOL IsAccelerator(IntPtr hAccel, int cAccelEntries, ref User32.MSG lpMsg, ushort* lpwCmd);

        public unsafe static BOOL IsAccelerator(HandleRef hAccel, int cAccelEntries, ref User32.MSG lpMsg,  ushort* lpwCmd)
        {
            BOOL result = IsAccelerator(hAccel.Handle, cAccelEntries, ref lpMsg, lpwCmd);
            GC.KeepAlive(hAccel.Wrapper);
            return result;
        }
    }
}
