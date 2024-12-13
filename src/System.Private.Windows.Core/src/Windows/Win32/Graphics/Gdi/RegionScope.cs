// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32.Graphics.Gdi;

/// <summary>
///  Helper to scope creating regions. Deletes the region when disposed.
/// </summary>
/// <remarks>
///  <para>
///   Use in a <see langword="using" /> statement. If you must pass this around, always pass
///   by <see langword="ref" /> to avoid duplicating the handle and risking a double deletion.
///  </para>
/// </remarks>
#if DEBUG
internal unsafe class RegionScope : DisposalTracking.Tracker, IDisposable
#else
internal unsafe ref struct RegionScope
#endif
{
    public HRGN Region { get; private set; }

    /// <summary>
    ///  Creates a region with the given rectangle via <see cref="PInvokeCore.CreateRectRgn(int, int, int, int)"/>.
    /// </summary>
    public RegionScope(Rectangle rectangle) =>
        Region = PInvokeCore.CreateRectRgn(rectangle.X, rectangle.Y, rectangle.Right, rectangle.Bottom);

    /// <summary>
    ///  Creates a region with the given rectangle via <see cref="PInvokeCore.CreateRectRgn(int, int, int, int)"/>.
    /// </summary>
    public RegionScope(int x1, int y1, int x2, int y2) =>
        Region = PInvokeCore.CreateRectRgn(x1, y1, x2, y2);

    /// <summary>
    ///  Creates a clipping region copy via <see cref="PInvokeCore.GetClipRgn(HDC, HRGN)"/> for the given device context.
    /// </summary>
    /// <param name="hdc">Handle to a device context to copy the clipping region from.</param>
    public RegionScope(HDC hdc)
    {
        HRGN region = PInvokeCore.CreateRectRgn(0, 0, 0, 0);
        int result = PInvokeCore.GetClipRgn(hdc, region);
        Debug.Assert(result != -1, "GetClipRgn failed");

        if (result == 1)
        {
            Region = region;
        }
        else
        {
            // No region, delete our temporary region
            PInvokeCore.DeleteObject(region);
            Region = default;
        }
    }

    /// <summary>
    ///  Creates a region scope with the given <see cref="HRGN"/>.
    /// </summary>
    public RegionScope(HRGN region) => Region = region;

    /// <summary>
    ///  Returns true if this represents a null HRGN.
    /// </summary>
#if DEBUG
    public bool IsNull => Region.IsNull;
#else
    public readonly bool IsNull => Region.IsNull;
#endif

    public static implicit operator HRGN(RegionScope regionScope) => regionScope.Region;

    /// <summary>
    ///  Clears the handle. Use this to hand over ownership to another entity.
    /// </summary>
    public void RelinquishOwnership() => Region = default;

#if DEBUG
    public void Dispose()
#else
    public readonly void Dispose()
#endif
    {
        if (!IsNull)
        {
            PInvokeCore.DeleteObject(Region);
        }

#if DEBUG
        GC.SuppressFinalize(this);
#endif
    }
}
