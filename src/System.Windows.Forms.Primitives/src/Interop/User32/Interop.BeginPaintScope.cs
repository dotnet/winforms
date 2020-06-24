﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        /// <summary>
        ///  Helper to scope lifetime of an HDC retrieved via <see cref="BeginPaint(IntPtr, ref PAINTSTRUCT)"/>
        /// </summary>
        /// <remarks>
        ///  Use in a <see langword="using" /> statement. If you must pass this around, always pass
        ///  by <see langword="ref" /> to avoid duplicating the handle and risking a double EndPaint.
        /// </remarks>
        public ref struct BeginPaintScope
        {
            public IntPtr HDC { get; }
            public IntPtr HWND { get; }
            private PAINTSTRUCT _paintStruct;

            public BeginPaintScope(IntPtr hwnd)
            {
                _paintStruct = default;
                HDC = BeginPaint(hwnd, ref _paintStruct);
                HWND = hwnd;
            }

            public static implicit operator IntPtr(BeginPaintScope paintScope) => paintScope.HDC;

            public void Dispose()
            {
                if (HDC != IntPtr.Zero)
                {
                    EndPaint(HWND, ref _paintStruct);
                }
            }
        }
    }
}
