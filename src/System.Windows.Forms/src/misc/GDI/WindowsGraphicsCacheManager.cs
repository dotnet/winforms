// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms.Internal
{
    /// <summary>
    ///  Keeps a cache of some graphics primitives.
    ///  Created to improve performance of TextRenderer.MeasureText methods that don't receive a WindowsGraphics.
    ///  This class mantains a cache of MRU WindowsFont objects in the process.
    /// </summary>
    internal class WindowsGraphicsCacheManager
    {
        // From MSDN: Do not specify initial values for fields marked with ThreadStaticAttribute, because such initialization occurs only once,
        // when the class constructor executes, and therefore affects only one thread.

        // WindowsGraphics object used for measuring text based on the screen DC.  TLS to avoid synchronization issues.
        [ThreadStatic]
        private static WindowsGraphics? t_measurementGraphics;

        // Circular list implementing the WindowsFont per-process cache.
        private const int CacheSize = 10;
        [ThreadStatic]
        private static int t_currentIndex;
        [ThreadStatic]
        private static List<KeyValuePair<Font, WindowsFont>>? t_windowsFontCache;

        /// <summary>
        ///  Static constructor since this is a utility class.
        /// </summary>
        static WindowsGraphicsCacheManager()
        {
        }

        /// <summary>
        ///  Class is never instantiated, private constructor prevents the compiler from generating a default constructor.
        /// </summary>
        private WindowsGraphicsCacheManager()
        {
        }

        /// <summary>
        ///  Initializes the WindowsFontCache object.
        /// </summary>
        private static List<KeyValuePair<Font, WindowsFont>> WindowsFontCache
        {
            get
            {
                if (t_windowsFontCache == null)
                {
                    t_currentIndex = -1;
                    t_windowsFontCache = new List<KeyValuePair<Font, WindowsFont>>(CacheSize);
                }

                return t_windowsFontCache;
            }
        }

        /// <summary>
        ///  Get the cached screen (primary monitor) memory dc.
        ///  Users of this class should always use this property to get the WindowsGraphics and never cache it, it could be mistakenly
        ///  disposed and we would recreate it if needed.
        ///  Users should not dispose of the WindowsGraphics so it can be reused for the lifetime of the thread.
        /// </summary>
        public static WindowsGraphics MeasurementGraphics
        {
            get
            {
                if (t_measurementGraphics == null || t_measurementGraphics.DeviceContext == null /*object disposed*/)
                {
                    Debug.Assert(t_measurementGraphics == null || t_measurementGraphics.DeviceContext != null, "TLS MeasurementGraphics was disposed somewhere, enable TRACK_HDC macro to determine who did it, recreating it for now ...");
#if TRACK_HDC
                    Debug.WriteLine( DbgUtil.StackTraceToStr("Initializing MeasurementGraphics"));
#endif
                    t_measurementGraphics = WindowsGraphics.CreateMeasurementWindowsGraphics();
                }

                return t_measurementGraphics;
            }
        }
#if OPTIMIZED_MEASUREMENTDC
        // in some cases, we dont want to demand create MeasurementGraphics, as creating it has
        // re-entrant side effects.
        internal static WindowsGraphics? GetCurrentMeasurementGraphics()
        {
            return t_measurementGraphics;
        }
#endif

        /// <summary>
        ///  Get the cached WindowsFont associated with the specified font if one exists, otherwise create one and
        ///  add it to the cache.
        /// </summary>
        public static WindowsFont? GetWindowsFont(Font? font, Gdi32.QUALITY fontQuality = Gdi32.QUALITY.DEFAULT)
        {
            if (font == null)
            {
                return null;
            }

            // First check if font is in the cache.

            int count = 0;
            int index = t_currentIndex;

            // Search by index of most recently added object.
            while (count < WindowsFontCache.Count)
            {
                if (WindowsFontCache[index].Key.Equals(font))  // don't do shallow comparison, we could miss cloned fonts.
                {
                    // We got a Font in the cache, let's see if we have a WindowsFont with the same quality as required by the caller.

                    // WARNING: It is not expected that the WindowsFont is disposed externally since it is created by this class.
                    Debug.Assert(WindowsFontCache[index].Value.Hfont != IntPtr.Zero, "Cached WindowsFont was disposed, enable GDI_FINALIZATION_WATCH to track who did it!");

                    WindowsFont wf = WindowsFontCache[index].Value;
                    if (wf.Quality == fontQuality)
                    {
                        return wf;
                    }
                }

                index--;
                count++;

                if (index < 0)
                {
                    index = CacheSize - 1;
                }
            }

            // Font is not in the cache, let's add it.

            WindowsFont winFont = WindowsFont.FromFont(font, fontQuality);
            KeyValuePair<Font, WindowsFont> newEntry = new KeyValuePair<Font, WindowsFont>(font, winFont);

            t_currentIndex++;

            if (t_currentIndex == CacheSize)
            {
                t_currentIndex = 0;
            }

            if (WindowsFontCache.Count == CacheSize)  // No more room, update current index.
            {
                WindowsFont? wfont = null;

                // Go through the existing fonts in the cache, and see if any
                // are not in use by a DC.  If one isn't, replace that.  If
                // all are in use, new up a new font and do not cache it.

                bool finished = false;
                int startIndex = t_currentIndex;
                int loopIndex = startIndex + 1;
                while (!finished)
                {
                    if (loopIndex >= CacheSize)
                    {
                        loopIndex = 0;
                    }

                    if (loopIndex == startIndex)
                    {
                        finished = true;
                    }

                    wfont = WindowsFontCache[loopIndex].Value;
                    if (!DeviceContexts.IsFontInUse(wfont))
                    {
                        t_currentIndex = loopIndex;
                        finished = true;
                        break;
                    }
                    else
                    {
                        loopIndex++;
                        wfont = null;
                    }
                }

                if (wfont != null)
                {
                    WindowsFontCache[t_currentIndex] = newEntry;
                    winFont.OwnedByCacheManager = true;

                    wfont.OwnedByCacheManager = false;
                    wfont.Dispose();
                }
                else
                {
                    // do not cache font - caller is ALWAYS responsible for
                    // disposing now.  If it is owned  by the CM, it will not
                    // disposed.

                    winFont.OwnedByCacheManager = false;

                }
            }
            else
            {
                winFont.OwnedByCacheManager = true;
                WindowsFontCache.Add(newEntry);

            }
            return winFont;
        }
    }
}
