// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, CharSet = CharSet.Unicode, ExactSpelling = true)]
        private unsafe static extern bool SystemParametersInfoW(SPI uiAction, uint uiParam, void* pvParam, uint fWinIni);

        public unsafe static bool SystemParametersInfoW<T>(SPI uiAction, ref T value) where T : unmanaged
        {
            fixed (void* p = &value)
            {
                return SystemParametersInfoW(uiAction, 0, p, 0);
            }
        }

        public unsafe static int SystemParametersInfoInt(SPI uiAction)
        {
            int value = 0;
            SystemParametersInfoW(uiAction, ref value);
            return value;
        }

        public unsafe static bool SystemParametersInfoW(SPI uiAction, ref bool value)
        {
            BOOL nativeBool = value ? BOOL.TRUE : BOOL.FALSE;
            bool result = SystemParametersInfoW(uiAction, 0, &nativeBool, 0);
            value = nativeBool.IsTrue();
            return result;
        }

        public unsafe static bool SystemParametersInfoBool(SPI uiAction)
        {
            bool value = false;
            SystemParametersInfoW(uiAction, ref value);
            return value;
        }

        public unsafe static bool SystemParametersInfoW(ref HIGHCONTRASTW highContrast)
        {
            fixed (void* p = &highContrast)
            {
                highContrast.cbSize = (uint)sizeof(HIGHCONTRASTW);
                return SystemParametersInfoW(
                    SPI.GETHIGHCONTRAST,
                    highContrast.cbSize,
                    p,
                    0); // This has no meaning when getting values
            }
        }

        public unsafe static bool SystemParametersInfoW(ref NONCLIENTMETRICSW metrics)
        {
            fixed (void* p = &metrics)
            {
                metrics.cbSize = (uint)sizeof(NONCLIENTMETRICSW);
                return SystemParametersInfoW(
                    SPI.GETNONCLIENTMETRICS,
                    metrics.cbSize,
                    p,
                    0); // This has no meaning when getting values
            }
        }

        // This API is available starting Windows 10 Anniversary Update (Redstone 1 / 1607 / 14393).
        // Unlike SystemParametersInfo, there is no "A/W" variance in this api.
        [DllImport(Libraries.User32, ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool SystemParametersInfoForDpi(
            SPI uiAction,
            uint uiParam,
            ref NONCLIENTMETRICSW pvParam,
            uint fWinIni,
            uint dpi);

        /// <summary>
        ///  Tries to get system parameter info for the dpi. dpi is ignored if "SystemParametersInfoForDpi()" API is not available on the OS that this application is running.
        /// </summary>
        public unsafe static bool TrySystemParametersInfoForDpi(ref NONCLIENTMETRICSW metrics, uint dpi)
        {
            if (OsVersion.IsWindows10_1607OrGreater)
            {
                metrics.cbSize = (uint)sizeof(NONCLIENTMETRICSW);
                return SystemParametersInfoForDpi(
                    SPI.GETNONCLIENTMETRICS,
                    metrics.cbSize,
                    ref metrics,
                    0,
                    dpi);
            }

            return SystemParametersInfoW(ref metrics);
        }
    }
}
