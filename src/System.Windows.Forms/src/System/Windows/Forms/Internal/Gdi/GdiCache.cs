// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Cache of GDI objects to reuse commonly created items.
    /// </summary>
    internal static partial class GdiCache
    {
        [ThreadStatic]
        private static ScreenDcCache? s_dcCache;

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
        public static ScreenDcCache.ScreenDcScope GetScreenHdc() => (s_dcCache ??= new ScreenDcCache()).Acquire();

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

            try
            {
                return new ScreenGraphicsScope(ref scope);
            }
            catch (OutOfMemoryException)
            {
                // GDI+ throws OOM if it can't confirm a valid HDC. We'll throw a more meaningful error here
                // for easier diagnosis.
                if (scope.HDC.IsNull)
                {
                    throw new ArgumentNullException("hdc");
                }

                Gdi32.OBJ type = Gdi32.GetObjectType(scope.HDC);
                if (type == Gdi32.OBJ.DC
                    || type == Gdi32.OBJ.ENHMETADC
                    || type == Gdi32.OBJ.MEMDC
                    || type == Gdi32.OBJ.METADC)
                {
                    // Not sure what is wrong in this case, throw the original.
                    throw;
                }

                throw new InvalidOperationException(string.Format(SR.InvalidHdcType, type));
            }
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
        public static FontCache.Scope GetHFONT(Font? font, Gdi32.QUALITY quality = Gdi32.QUALITY.DEFAULT)
        {
            Debug.Assert(font != null);
            return font is null ? new FontCache.Scope() : s_fontCache.GetEntry(font, quality);
        }

        public static FontCache.Scope GetHFONT(Font? font, Gdi32.QUALITY quality, Gdi32.HDC hdc)
        {
            if (font != null)
            {
                return GetHFONT(font, quality);
            }

            // Font is null, build off of the specified HDC's current font.
            Gdi32.HFONT hfont = (Gdi32.HFONT)Gdi32.GetCurrentObject(hdc, Gdi32.OBJ.FONT);
            return new FontCache.Scope(hfont); ;
        }
    }
}
