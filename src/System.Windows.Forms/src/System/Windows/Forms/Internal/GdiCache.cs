// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Cache of GDI objects to reuse commonly created items.
    /// </summary>
    internal static partial class GdiCache
    {
        private static readonly ScreenDcCache s_dcCache = new ScreenDcCache();
        private static readonly FontCache s_fontCache = new FontCache();

        /// <summary>
        ///  Gets an <see cref="Gdi32.HDC"/> based off of the primary display.
        /// </summary>
        /// <remarks>
        ///  Use in a using statement for proper cleanup.
        ///
        ///  When disposed the <see cref="Gdi32.HDC"/> will be returned to the cache. Do NOT change the state of the
        ///  DC (clipping, selecting objects into it) without restoring the state. If you must pass the scope to
        ///  another method it must be passed by reference or you risk accidentally returning extra copies to the
        ///  cache.
        /// </remarks>
        public static ScreenDcCache.ScreenDcScope GetScreenHdc() => s_dcCache.Acquire();

        /// <summary>
        ///  Gets an <see cref="Graphics"/> based off of the primary display.
        /// </summary>
        /// <remarks>
        ///  Use in a using statement for proper cleanup.
        ///
        ///  When disposed the <see cref="Graphics"/> object will be disposed and the underlying <see cref="Gdi32.HDC"/>
        ///  will be returned to the cache. Do NOT change the state of the underlying DC (clipping, selecting objects
        ///  into it) without restoring the state. If you must pass the scope to another method it must be passed by
        ///  reference or you risk double disposal and accidentally returning extra copies to the cache.
        /// </remarks>
        public static ScreenGraphicsScope GetScreenDCGraphics()
        {
            ScreenDcCache.ScreenDcScope scope = GetScreenHdc();
            return new ScreenGraphicsScope(ref scope);
        }

        /// <summary>
        ///  Gets a cached <see cref="Gdi32.HFONT"/> based off of the given <paramref name="font"/> and
        ///  <paramref name="quality"/>.
        /// </summary>
        /// <remarks>
        ///  Use in a using statement for proper cleanup.
        ///
        ///  When disposed the <see cref="Gdi32.HFONT"/> will be returned to the cache.  If you must pass the scope to
        ///  another method it must be passed by reference or you risk double disposal and accidentally returning extra
        ///  copies to the cache.
        /// </remarks>
        public static FontCache.FontScope GetHFONT(Font? font, Gdi32.QUALITY quality = Gdi32.QUALITY.DEFAULT)
            => font is null ? default : s_fontCache.GetHFONT(font, quality);
    }
}
