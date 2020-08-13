// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;

internal static partial class Interop
{
    internal static partial class User32
    {
        /// <summary>
        ///  Helper to scope lifetime of an <see cref="Gdi32.HDC"/> retrieved via <see cref="GetDC(IntPtr)"/> and
        ///  <see cref="GetDCEx(IntPtr, IntPtr, DCX)"/>. Releases the <see cref="Gdi32.HDC"/> (if any) when disposed.
        /// </summary>
        /// <remarks>
        ///  Use in a <see langword="using" /> statement. If you must pass this around, always pass by <see langword="ref" />
        ///  to avoid duplicating the handle and risking a double release.
        /// </remarks>
        public readonly ref struct GetDcScope
        {
            public Gdi32.HDC HDC { get; }
            public IntPtr HWND { get; }

            public GetDcScope(IntPtr hwnd)
            {
                HWND = hwnd;
                HDC = GetDC(hwnd);
            }

            /// <summary>
            ///  Creates a <see cref="Gdi32.HDC"/> using <see cref="GetDCEx(IntPtr, IntPtr, DCX)"/>.
            /// </summary>
            /// <remarks>
            ///  GetWindowDC calls GetDCEx(hwnd, null, DCX_WINDOW | DCX_USESTYLE).
            ///
            ///  GetDC calls GetDCEx(hwnd, null, DCX_USESTYLE) when given a handle. (When given null it has additional
            ///  logic, and can't be replaced directly by GetDCEx.
            /// </remarks>
            public GetDcScope(IntPtr hwnd, IntPtr hrgnClip, DCX flags)
            {
                HWND = hwnd;
                HDC = GetDCEx(hwnd, hrgnClip, flags);
            }

            /// <summary>
            ///  Creates a DC scope for the primary monitor (not the entire desktop).
            /// </summary>
            /// <remarks>
            ///   <see cref="Gdi32.CreateDC(string, string, string, IntPtr)" /> is the API to get the DC for the
            ///   entire desktop.
            /// </remarks>
            public static GetDcScope ScreenDC => new GetDcScope(IntPtr.Zero);

            public bool IsNull => HDC.IsNull;

            public static implicit operator IntPtr(in GetDcScope dcScope) => dcScope.HDC.Handle;
            public static implicit operator Gdi32.HDC(in GetDcScope dcScope) => dcScope.HDC;

            public void Dispose()
            {
                if (!HDC.IsNull)
                {
                    ReleaseDC(HWND, HDC);
                }
            }
        }
    }
}
