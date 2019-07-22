// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    internal static class OsVersion
    {
        private static Interop.NtDll.RTL_OSVERSIONINFOEX _versionInfo = InitVersion;

        private static Interop.NtDll.RTL_OSVERSIONINFOEX InitVersion
        {
            get
            {
                Interop.NtDll.RtlGetVersion(out Interop.NtDll.RTL_OSVERSIONINFOEX info);
                return info;
            }
        }

        /// <summary>
        /// Is Windows 10 Anniversary Update or later. (Redstone 1, build 14393, version 1607)
        /// </summary>
        public static bool IsWindows10_1607OrGreater
            => _versionInfo.dwMajorVersion >= 10 && _versionInfo.dwBuildNumber >= 14393;

        /// <summary>
        /// Is Windows 10 Creators Update or later. (Redstone 2, build 15063, version 1703)
        /// </summary>
        public static bool IsWindows10_1703OrGreater
            => _versionInfo.dwMajorVersion >= 10 && _versionInfo.dwBuildNumber >= 15063;

        /// <summary>
        /// Is Windows 8.1 or later.
        /// </summary>
        public static bool IsWindows8_1OrGreater
            => _versionInfo.dwMajorVersion >= 10
                || _versionInfo.dwMajorVersion == 6 && _versionInfo.dwMinorVersion == 3;
    }
}
