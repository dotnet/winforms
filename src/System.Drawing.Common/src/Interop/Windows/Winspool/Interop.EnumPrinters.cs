// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Winspool
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.Winspool, EntryPoint = "EnumPrintersW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        internal static partial int EnumPrinters(
#else
        [DllImport(Libraries.Winspool, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int EnumPrinters(
#endif
            int flags,
            string? name,
            int level,
            IntPtr pPrinterEnum/*buffer*/,
            int cbBuf,
            out int pcbNeeded,
            out int pcReturned);
    }
}
