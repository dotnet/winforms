// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Drawing;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        /// <summary>
        ///  Helper to scope creating regions. Deletes the region when disposed.
        /// </summary>
        /// <remarks>
        ///  Use in a <see langword="using" /> statement. If you must pass this around, always pass
        ///  by <see langword="ref" /> to avoid duplicating the handle and risking a double deletion.
        /// </remarks>
#if DEBUG
        internal class RegionScope : DisposalTracking.Tracker, IDisposable
#else
        internal ref struct RegionScope
#endif
        {
            public HRGN Region { get; private set; }

            /// <summary>
            ///  Creates a region with the given rectangle via <see cref="CreateRectRgn(int, int, int, int)"/>.
            /// </summary>
            public RegionScope(Rectangle rectangle)
            {
                Region = CreateRectRgn(rectangle.X, rectangle.Y, rectangle.Right, rectangle.Bottom);
            }

            /// <summary>
            ///  Creates a region with the given rectangle via <see cref="CreateRectRgn(int, int, int, int)"/>.
            /// </summary>
            public RegionScope(int x1, int y1, int x2, int y2)
            {
                Region = CreateRectRgn(x1, y1, x2, y2);
            }

            /// <summary>
            ///  Creates a clipping region copy via <see cref="GetClipRgn(HDC, HRGN)"/> for the given device context.
            /// </summary>
            /// <param name="hdc">Handle to a device context to copy the clipping region from.</param>
            public RegionScope(HDC hdc)
            {
                HRGN region = CreateRectRgn(0, 0, 0, 0);
                int result = GetClipRgn(hdc, region);
                Debug.Assert(result != -1, "GetClipRgn failed");

                if (result == 1)
                {
                    Region = region;
                }
                else
                {
                    // No region, delete our temporary region
                    DeleteObject(region);
                    Region = default;
                }
            }

            /// <summary>
            ///  Creates a native region from a GDI+ <see cref="Region"/>.
            /// </summary>
            public RegionScope(Region region, Graphics graphics)
            {
                if (region.IsInfinite(graphics))
                {
                    // An infinite region would cover the entire device region which is the same as
                    // not having a clipping region. Observe that this is not the same as having an
                    // empty region, which when clipping to it has the effect of excluding the entire
                    // device region.
                    //
                    // To remove the clip region from a dc the SelectClipRgn() function needs to be
                    // called with a null region ptr - that's why we use the empty constructor here.
                    // GDI+ will return IntPtr.Zero for Region.GetHrgn(Graphics) when the region is
                    // Infinite.

                    Region = default;
                    return;
                }

                Region = new HRGN(region.GetHrgn(graphics));
            }

            public RegionScope(Region region, IntPtr hwnd)
            {
                using var graphics = Graphics.FromHwndInternal(hwnd);
                Region = new HRGN(region.GetHrgn(graphics));
            }

            /// <summary>
            ///  Returns true if this represents a null HRGN.
            /// </summary>
            public bool IsNull => Region.Handle == IntPtr.Zero;

            public static implicit operator HRGN(RegionScope regionScope) => regionScope.Region;

            /// <summary>
            ///  Creates a GDI+ region for this region.
            /// </summary>
            /// <returns>The GDI+ region. Must be disposed.</returns>
            public Region CreateGdiPlusRegion() => System.Drawing.Region.FromHrgn(Region.Handle);

            /// <summary>
            ///  Clears the handle. Use this to hand over ownership to another entity.
            /// </summary>
            public void RelinquishOwnership() => Region = default;

            public void Dispose()
            {
                if (Region.Handle != IntPtr.Zero)
                {
                    DeleteObject(Region);
                }

                DisposalTracking.SuppressFinalize(this!);
            }
        }
    }
}
