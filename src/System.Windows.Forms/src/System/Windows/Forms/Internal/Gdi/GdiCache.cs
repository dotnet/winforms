﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;

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
        ///  Gets an <see cref="HDC"/> based off of the primary display.
        /// </summary>
        /// <remarks>
        ///  Use in a using statement for proper cleanup.
        ///
        ///  When disposed the <see cref="HDC"/> will be returned to the cache. Do NOT change the state of the
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
        ///  When disposed the <see cref="Graphics"/> object will be disposed and the underlying <see cref="HDC"/>
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
                ArgumentValidation.ThrowIfNull(scope.HDC, "hdc");

                OBJ_TYPE type = (OBJ_TYPE)PInvoke.GetObjectType(scope.HDC);
                if (type == OBJ_TYPE.OBJ_DC
                    || type == OBJ_TYPE.OBJ_ENHMETADC
                    || type == OBJ_TYPE.OBJ_MEMDC
                    || type == OBJ_TYPE.OBJ_METADC)
                {
                    // Not sure what is wrong in this case, throw the original.
                    throw;
                }

                throw new InvalidOperationException(string.Format(SR.InvalidHdcType, type));
            }
        }

        /// <summary>
        ///  Gets a cached <see cref="HFONT"/> based off of the given <paramref name="font"/> and
        ///  <paramref name="quality"/>.
        /// </summary>
        /// <remarks>
        ///  Use in a using statement for proper cleanup.
        ///
        ///  When disposed the <see cref="HFONT"/> will be returned to the cache.  If you must pass the scope to
        ///  another method it must be passed by reference or you risk double disposal and accidentally returning extra
        ///  copies to the cache.
        /// </remarks>
        public static FontCache.Scope GetHFONT(Font? font, FONT_QUALITY quality = FONT_QUALITY.DEFAULT_QUALITY)
        {
            Debug.Assert(font is not null);
#if DEBUG
            return font is null ? new FontCache.Scope() : s_fontCache.GetEntry(font, quality);
#else
            return font is null ? default : s_fontCache.GetEntry(font, quality);
#endif
        }

        public static FontCache.Scope GetHFONT(Font? font, FONT_QUALITY quality, HDC hdc)
        {
            if (font is not null)
            {
                return GetHFONT(font, quality);
            }

            // Font is null, build off of the specified HDC's current font.
            HFONT hfont = (HFONT)PInvoke.GetCurrentObject(hdc, OBJ_TYPE.OBJ_FONT);
            return new FontCache.Scope(hfont);
        }
    }
}
