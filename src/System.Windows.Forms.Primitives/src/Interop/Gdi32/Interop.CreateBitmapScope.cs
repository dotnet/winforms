﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        /// <summary>
        ///  Helper to scope lifetime of a <see cref="Windows.Win32.Graphics.Gdi.HBITMAP"/> created via <see cref="PInvoke.CreateBitmap"/>
        ///  Deletes the <see cref="Windows.Win32.Graphics.Gdi.HBITMAP"/> (if any) when disposed.
        /// </summary>
        /// <remarks>
        ///  Use in a <see langword="using" /> statement. If you must pass this around, always pass
        ///  by <see langword="ref" /> to avoid duplicating the handle and risking a double delete.
        /// </remarks>
#if DEBUG
        internal class CreateBitmapScope : DisposalTracking.Tracker, IDisposable
#else
        internal readonly ref struct CreateBitmapScope
#endif
        {
            public HBITMAP HBITMAP { get; }

            /// <summary>
            ///  Creates a bitmap using <see cref="PInvoke.CreateBitmap"/>
            /// </summary>
            public unsafe CreateBitmapScope(int nWidth, int nHeight, uint nPlanes, uint nBitCount, void* lpvBits)
            {
                HBITMAP = PInvoke.CreateBitmap(nWidth, nHeight, nPlanes, nBitCount, lpvBits);
            }

            /// <summary>
            ///  Creates a bitmap compatible with the given <see cref="HDC"/> via <see cref="PInvoke.CreateCompatibleBitmap(Windows.Win32.Graphics.Gdi.HDC, int, int)"/>
            /// </summary>
            public CreateBitmapScope(HDC hdc, int cx, int cy)
            {
                HBITMAP = PInvoke.CreateCompatibleBitmap(hdc, cx, cy);
            }

            public static implicit operator HBITMAP(in CreateBitmapScope scope) => scope.HBITMAP;
            public static implicit operator HGDIOBJ(in CreateBitmapScope scope) => scope.HBITMAP;
            public static explicit operator nint(in CreateBitmapScope scope) => scope.HBITMAP;

            public bool IsNull => HBITMAP.IsNull;

            public void Dispose()
            {
                if (!HBITMAP.IsNull)
                {
                    PInvoke.DeleteObject(HBITMAP);
                }

#if DEBUG
                GC.SuppressFinalize(this);
#endif
            }
        }
    }
}
