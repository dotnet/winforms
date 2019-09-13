// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true, EntryPoint = "GetMonitorInfoW", CharSet = CharSet.Unicode)]
        private static extern bool GetMonitorInfoWInternal(IntPtr hmonitor, ref MONITORINFOEXW info);

        public static bool GetMonitorInfoW(IntPtr hmonitor, out MONITORINFOEXW info)
        {
            info = new MONITORINFOEXW
            {
                cbSize = (uint)Marshal.SizeOf<MONITORINFOEXW>()
            };
            return GetMonitorInfoWInternal(hmonitor, ref info);
        }
    }
}
