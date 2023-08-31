// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Shell32
    {
#if NET8_0_OR_GREATER
        [LibraryImport(Libraries.Shell32, StringMarshalling = StringMarshalling.Utf16, EntryPoint = "SHDefExtractIconW")]
        internal static unsafe partial HRESULT SHDefExtractIcon(
            char* pszIconFile,
            int iIndex,
            uint uFlags,
            nint* phiconLarge,
            nint* phiconSmall,
            uint nIconSize);
#endif
    }
}
