﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;
using Windows.Win32.UI.Accessibility;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static unsafe bool SystemParametersInfo<T>(SYSTEM_PARAMETERS_INFO_ACTION uiAction, ref T value)
            where T : unmanaged
        {
            fixed (void* p = &value)
            {
                return SystemParametersInfo(uiAction, 0, p, 0);
            }
        }

        public static unsafe int SystemParametersInfoInt(SYSTEM_PARAMETERS_INFO_ACTION uiAction)
        {
            int value = 0;
            SystemParametersInfo(uiAction, ref value);
            return value;
        }

        public static unsafe bool SystemParametersInfo(SYSTEM_PARAMETERS_INFO_ACTION uiAction, ref bool value, uint fWinIni = 0)
        {
            BOOL nativeBool = value;
            bool result = SystemParametersInfo(uiAction, 0, &nativeBool, (SYSTEM_PARAMETERS_INFO_UPDATE_FLAGS)fWinIni);
            value = nativeBool;
            return result;
        }

        public static unsafe bool SystemParametersInfoBool(SYSTEM_PARAMETERS_INFO_ACTION uiAction)
        {
            bool value = false;
            SystemParametersInfo(uiAction, ref value);
            return value;
        }

        public static unsafe bool SystemParametersInfo(ref HIGHCONTRASTW highContrast)
        {
            fixed (void* p = &highContrast)
            {
                highContrast.cbSize = (uint)sizeof(HIGHCONTRASTW);
                return SystemParametersInfo(
                    SYSTEM_PARAMETERS_INFO_ACTION.SPI_GETHIGHCONTRAST,
                    highContrast.cbSize,
                    p,
                    0); // This has no meaning when getting values
            }
        }

        public static unsafe bool SystemParametersInfo(ref NONCLIENTMETRICSW metrics)
        {
            fixed (void* p = &metrics)
            {
                metrics.cbSize = (uint)sizeof(NONCLIENTMETRICSW);
                return SystemParametersInfo(
                    SYSTEM_PARAMETERS_INFO_ACTION.SPI_GETNONCLIENTMETRICS,
                    metrics.cbSize,
                    p,
                    0); // This has no meaning when getting values
            }
        }

        /// <summary>
        ///  Tries to get system parameter info for the dpi. dpi is ignored if "SystemParametersInfoForDpi()" API
        ///  is not available on the OS that this application is running.
        /// </summary>
        public static unsafe bool TrySystemParametersInfoForDpi(ref NONCLIENTMETRICSW metrics, uint dpi)
        {
            if (OsVersion.IsWindows10_1607OrGreater())
            {
                fixed (void* p = &metrics)
                {
                    metrics.cbSize = (uint)sizeof(NONCLIENTMETRICSW);
                    return SystemParametersInfoForDpi(
                        (uint)SYSTEM_PARAMETERS_INFO_ACTION.SPI_GETNONCLIENTMETRICS,
                        metrics.cbSize,
                        p,
                        0, // This has no meaning when getting values
                        dpi);
                }
            }

            return SystemParametersInfo(ref metrics);
        }
    }
}
