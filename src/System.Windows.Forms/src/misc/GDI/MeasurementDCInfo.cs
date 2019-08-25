// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if OPTIMIZED_MEASUREMENTDC
#if WGCM_TEST_SUITE // Enable tracking when built for the test suites.
#define TRACK_HDC
#define GDI_FONT_CACHE_TRACK
#endif

using static Interop;

namespace System.Windows.Forms.Internal
{
    internal static class MeasurementDCInfo
    {
        ///  MeasurementDCInfo
        ///  This class optimizes the MeasurmentGraphics as it caches in the last used font and TextMargins used.
        ///  This prevents unnecessary p/invoke calls to GetCurrentObject, etc
        ///  It has been found to give close to 2x performance when drawing lots of text in rapid succession
        ///  DataGridView with lots of text, etc.
        ///  To turn it on for your DLL, use the OPTIMIZED_MEASUREMENTDC compiler switch and add this class to the sources.

        [ThreadStatic]
        private static CachedInfo cachedMeasurementDCInfo;

        ///  IsMeasurementDC
        ///  Returns whether the IDeviceContext passed in is our static MeasurementDC.
        ///  If it is, we know a bit more information about it.
        internal static bool IsMeasurementDC(DeviceContext dc)
        {
            WindowsGraphics sharedGraphics = WindowsGraphicsCacheManager.GetCurrentMeasurementGraphics();
            return sharedGraphics != null && sharedGraphics.DeviceContext != null && sharedGraphics.DeviceContext.Hdc == dc.Hdc;
        }

        ///  LastUsedFont -
        ///  Returns the font we think was last selected into the MeasurementGraphics.
        ///
        internal static WindowsFont LastUsedFont
        {
            get
            {
                return cachedMeasurementDCInfo?.LastUsedFont;
            }
            set
            {
                if (cachedMeasurementDCInfo == null)
                {
                    cachedMeasurementDCInfo = new CachedInfo();
                }
                cachedMeasurementDCInfo.UpdateFont(value);
            }
        }

        ///  GetTextMargins - checks to see if we have cached information about the current font,
        ///  returns info about it.
        ///  An MRU of Font margins was considered, but seems like overhead.
        internal static User32.DRAWTEXTPARAMS GetTextMargins(WindowsGraphics wg, WindowsFont font)
        {
            // PERF: operate on a local reference rather than party directly on the thread static one.
            CachedInfo currentCachedInfo = cachedMeasurementDCInfo;

            if (currentCachedInfo != null && currentCachedInfo.LeftTextMargin > 0 && currentCachedInfo.RightTextMargin > 0 && font == currentCachedInfo.LastUsedFont)
            {
                // we have to return clones as DrawTextEx will modify this struct
                return new User32.DRAWTEXTPARAMS
                {
                    iLeftMargin = currentCachedInfo.LeftTextMargin,
                    iRightMargin = currentCachedInfo.RightTextMargin
                };
            }
            else if (currentCachedInfo == null)
            {
                currentCachedInfo = new CachedInfo();
                cachedMeasurementDCInfo = currentCachedInfo;
            }
            User32.DRAWTEXTPARAMS drawTextParams = wg.GetTextMargins(font);
            currentCachedInfo.LeftTextMargin = drawTextParams.iLeftMargin;
            currentCachedInfo.RightTextMargin = drawTextParams.iRightMargin;

            // returning a copy here to be consistent with the return value from the cache.
            return new User32.DRAWTEXTPARAMS
            {
                iLeftMargin = currentCachedInfo.LeftTextMargin,
                iRightMargin = currentCachedInfo.RightTextMargin
            };
        }

        internal static void ResetIfIsMeasurementDC(IntPtr hdc)
        {
            WindowsGraphics sharedGraphics = WindowsGraphicsCacheManager.GetCurrentMeasurementGraphics();
            if (sharedGraphics != null && sharedGraphics.DeviceContext != null && sharedGraphics.DeviceContext.Hdc == hdc)
            {
                CachedInfo currentCachedInfo = cachedMeasurementDCInfo;
                if (currentCachedInfo != null)
                {
                    currentCachedInfo.UpdateFont(null);
                }
            }
        }
        ///  Reset
        ///  clear the current cached information about the measurement dc.
        internal static void Reset()
        {
            CachedInfo currentCachedInfo = cachedMeasurementDCInfo;
            if (currentCachedInfo != null)
            {
                currentCachedInfo.UpdateFont(null);
            }
        }
        ///  CachedInfo
        ///  store all the thread statics together so we dont have to fetch individual fields out of TLS
        private sealed class CachedInfo
        {
            public WindowsFont LastUsedFont;
            public int LeftTextMargin;
            public int RightTextMargin;

            internal void UpdateFont(WindowsFont font)
            {
                if (LastUsedFont != font)
                {
                    LastUsedFont = font;
                    LeftTextMargin = -1;
                    RightTextMargin = -1;
                }
            }
        }
    }
}
#endif // OPTIMIZED_MEASUREMENTDC
