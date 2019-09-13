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
        public delegate bool MonitorEnumProc(IntPtr monitor, IntPtr hdc, IntPtr lprcMonitor, IntPtr lParam);
        
        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static unsafe extern bool EnumDisplayMonitors(IntPtr hdc, RECT* rcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);
    }
}
