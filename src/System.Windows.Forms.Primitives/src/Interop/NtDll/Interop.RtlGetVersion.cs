// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class NtDll
    {
        [DllImport(Libraries.NtDll, EntryPoint="RtlGetVersion", ExactSpelling=true)]
        private static extern int RtlGetVersionInternal(ref RTL_OSVERSIONINFOEX lpVersionInformation);

        internal static unsafe int RtlGetVersion(out RTL_OSVERSIONINFOEX versionInfo)
        {
            versionInfo = new RTL_OSVERSIONINFOEX
            {
                dwOSVersionInfoSize = (uint)sizeof(RTL_OSVERSIONINFOEX)
            };
            return RtlGetVersionInternal(ref versionInfo);
        }
    }
}
