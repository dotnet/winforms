// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        /// <summary>
        ///  Helper to scope lifetime of an HDC retrieved via GetDC/Ex. Releases the HDC (if any) when disposed.
        /// </summary>
        /// <remarks>
        ///  Use in a <see langword="using" /> statement. If you must pass this around, always pass
        ///  by <see langword="ref" /> to avoid duplicating the handle and risking a double release.
        /// </remarks>
        internal ref struct GetDcScope
        {
            public IntPtr HDC { get; }
            public IntPtr HWND { get; }

            public GetDcScope(IntPtr hwnd)
            {
                HWND = hwnd;
                HDC = GetDC(hwnd);
            }

            public GetDcScope(IntPtr hwnd, IntPtr hrgnClip, DCX flags)
            {
                HWND = hwnd;
                HDC = GetDCEx(hwnd, hrgnClip, flags);
            }

            /// <summary>
            ///  Creates a DC scope for the primary monitor (not the entire desktop).
            /// </summary>
            /// <remarks>
            ///   <see cref="Gdi32.CreateDC(string, string, string, IntPtr)" /> is
            ///   the API to get the DC for the entire desktop.
            /// </remarks>
            public static GetDcScope ScreenDC
                => new GetDcScope(IntPtr.Zero);

            public static implicit operator IntPtr(GetDcScope dcScope) => dcScope.HDC;

            public void Dispose()
            {
                if (HDC is not null)
                {
                    ReleaseDC(HWND, HDC);
                }
            }
        }
    }
}
