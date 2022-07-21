// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Win32.Foundation;
using Gdi = Windows.Win32.Graphics.Gdi;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        /// <summary>
        ///  Helper to scope lifetime of an HDC retrieved via <see cref="BeginPaint(HWND, out Gdi.PAINTSTRUCT)"/>
        /// </summary>
        /// <remarks>
        ///  <para>
        ///   Use in a <see langword="using" /> statement. If you must pass this around, always pass
        ///   by <see langword="ref" /> to avoid duplicating the handle and risking a double EndPaint.
        ///  </para>
        /// </remarks>
#if DEBUG
        internal class BeginPaintScope : DisposalTracking.Tracker, IDisposable
#else
        internal readonly ref struct BeginPaintScope
#endif
        {
            private readonly Gdi.PAINTSTRUCT _paintStruct;

            public Gdi.HDC HDC { get; }
            public HWND HWND { get; }

            public Gdi.PAINTSTRUCT PaintStruct => _paintStruct;

            public BeginPaintScope(HWND hwnd)
            {
                HDC = BeginPaint(hwnd, out _paintStruct);
                HWND = hwnd;
            }

            public static implicit operator Interop.Gdi32.HDC(in BeginPaintScope scope) => scope.HDC;
            public static implicit operator Gdi.HDC(in BeginPaintScope scope) => scope.HDC;

            public unsafe void Dispose()
            {
                if (!HDC.IsNull)
                {
                    EndPaint(HWND, _paintStruct);
                }

#if DEBUG
                GC.SuppressFinalize(this);
#endif
            }
        }
    }
}
