// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading;

#if NET8_0_OR_GREATER

namespace System.Drawing.Imaging;

/// <summary>
///  A device dependent copy of a <see cref="Bitmap"/> matching a specified <see cref="Graphics"/> object's current
///  device (display) settings. Avoids reformatting step when rendering, which can significantly improve performance.
/// </summary>
/// <remarks>
///  <para>
///   <see cref="CachedBitmap"/> matches the current bit depth of the <see cref="Graphics"/>'s device. If the device bit
///   depth changes, the <see cref="CachedBitmap"/> will no longer be usable and a new instance will need to be created
///   that matches. If the <see cref="CachedBitmap"/> was created against <see cref="PixelFormat.Format32bppRgb"/> it
///   will always work.
///  </para>
///  <para>
///   <see cref="CachedBitmap"/> will not work with any transformations other than translation.
///  </para>
///  <para>
///   <see cref="CachedBitmap"/> cannot be used to draw to a printer or metafile.
///  </para>
/// </remarks>
public sealed unsafe class CachedBitmap : IDisposable
{
    private nint _handle;

    /// <summary>
    ///  Create a device dependent copy of the given <paramref name="bitmap"/> for the device settings of the given
    ///  <paramref name="graphics"/>
    /// </summary>
    /// <param name="bitmap">The <see cref="Bitmap"/> to convert.</param>
    /// <param name="graphics">
    ///  The <see cref="Graphics"/> object to use to format the cached copy of the <paramref name="bitmap"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///  <paramref name="bitmap"/> or <paramref name="graphics"/> is <see langword="null"/>.
    /// </exception>
    public CachedBitmap(Bitmap bitmap, Graphics graphics)
    {
        ArgumentNullException.ThrowIfNull(bitmap);
        ArgumentNullException.ThrowIfNull(graphics);

        GpCachedBitmap* cachedBitmap;
        PInvokeGdiPlus.GdipCreateCachedBitmap(
            bitmap.Pointer(),
            graphics.Pointer(),
            &cachedBitmap);
        _handle = (nint)cachedBitmap;
    }

    internal nint Handle => _handle;

    private void Dispose(bool disposing)
    {
        nint handle = Interlocked.Exchange(ref _handle, 0);
        if (handle == 0)
        {
            return;
        }

        Status status = PInvokeGdiPlus.GdipDeleteCachedBitmap((GpCachedBitmap*)handle);
        if (disposing)
        {
            // Don't want to throw on the finalizer thread.
            Gdip.CheckStatus(status);
        }
    }

    ~CachedBitmap() => Dispose(disposing: false);

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
#endif
