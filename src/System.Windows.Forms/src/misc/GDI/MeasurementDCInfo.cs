// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

        [ThreadStatic]
        private static CachedInfo? t_cachedMeasurementDCInfo;

        ///  IsMeasurementDC
        ///  Returns whether the IDeviceContext passed in is our static MeasurementDC.
        ///  If it is, we know a bit more information about it.
        internal static bool IsMeasurementDC(DeviceContext dc)
        {
            WindowsGraphics? sharedGraphics = WindowsGraphicsCacheManager.GetCurrentMeasurementGraphics();
            return sharedGraphics != null && sharedGraphics.DeviceContext != null && sharedGraphics.DeviceContext.Hdc == dc.Hdc;
        }

        /// <summary>
        ///  Returns the font we think was last selected into the MeasurementGraphics.
        /// </summary>
        internal static WindowsFont? LastUsedFont
        {
            get => t_cachedMeasurementDCInfo?.LastUsedFont;
            set
            {
                t_cachedMeasurementDCInfo ??= new CachedInfo();
                t_cachedMeasurementDCInfo.UpdateFont(value);
            }
        }

        /// <summary>
        ///  Checks to see if we have cached information about the current font,
        ///  returns info about it.
        ///  An MRU of Font margins was considered, but seems like overhead.
        /// </summary>
        internal static User32.DRAWTEXTPARAMS GetTextMargins(WindowsGraphics wg, WindowsFont? font)
        {
            // PERF: operate on a local reference rather than party directly on the thread static one.
            CachedInfo? currentCachedInfo = t_cachedMeasurementDCInfo;

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
                t_cachedMeasurementDCInfo = currentCachedInfo;
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
            WindowsGraphics? sharedGraphics = WindowsGraphicsCacheManager.GetCurrentMeasurementGraphics();
            if (sharedGraphics != null && sharedGraphics.DeviceContext != null && sharedGraphics.DeviceContext.Hdc == hdc)
            {
                CachedInfo? currentCachedInfo = t_cachedMeasurementDCInfo;
                currentCachedInfo?.UpdateFont(null);
            }
        }

        /// <summary>
        ///  Clear the current cached information about the measurement dc.
        /// </summary>
        internal static void Reset()
        {
            CachedInfo? currentCachedInfo = t_cachedMeasurementDCInfo;
            currentCachedInfo?.UpdateFont(null);
        }

        /// <summary>
        ///  Store all the thread statics together so we dont have to fetch individual fields out of TLS
        /// </summary>
        private sealed class CachedInfo
        {
            public WindowsFont? LastUsedFont;
            public int LeftTextMargin;
            public int RightTextMargin;

            internal void UpdateFont(WindowsFont? font)
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
