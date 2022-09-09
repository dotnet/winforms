// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Windows.Win32;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true, SetLastError = true)]
        private static extern nint SetWinEventHook(uint eventMin, uint eventMax, nint hmodWinEventProc,
            WINEVENTPROC pfnWinEventProc, uint idProcess, uint idThread, WINEVENT dwFlags);

        public static nint SetWinEventHook(uint eventId, WINEVENTPROC pfnWinEventProc)
            => SetWinEventHook(eventId, eventId, PInvoke.GetModuleHandle(null), pfnWinEventProc,
                (uint)Environment.ProcessId, PInvoke.GetCurrentThreadId(), WINEVENT.INCONTEXT);
    }
}
