// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class NtDll
    {
        /// <summary>
        ///  Version info structure for <see cref="RtlGetVersion(out RTL_OSVERSIONINFOEX)" />
        /// </summary>
        /// <remarks>
        ///  Note that this structure is the exact same defintion as OSVERSIONINFOEX.
        /// </remarks>
        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
        internal unsafe struct RTL_OSVERSIONINFOEX
        {
            internal uint dwOSVersionInfoSize;
            internal uint dwMajorVersion;
            internal uint dwMinorVersion;
            internal uint dwBuildNumber;
            internal uint dwPlatformId;
            internal fixed char szCSDVersion[128];
            internal ushort wServicePackMajor;
            internal ushort wServicePackMinor;
            internal ushort wSuiteMask;
            internal byte wProductType;
            internal byte wReserved;
        }
    }
}
