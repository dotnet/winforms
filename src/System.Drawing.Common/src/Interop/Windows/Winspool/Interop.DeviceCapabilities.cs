// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Winspool
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.Winspool, EntryPoint = "DeviceCapabilitiesW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        internal static partial int DeviceCapabilities(
#else
        [DllImport(Libraries.Winspool, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int DeviceCapabilities(
#endif
            string pDevice,
            string pPort,
            short fwCapabilities,
            IntPtr pOutput,
            IntPtr /*DEVMODE*/ pDevMode);
    }
}
