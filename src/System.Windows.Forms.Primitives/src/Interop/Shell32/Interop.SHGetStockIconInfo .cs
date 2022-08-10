// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        [LibraryImport(Libraries.Shell32)]
        public static unsafe partial HRESULT SHGetStockIconInfo(SHSTOCKICONID siid, SHGSI uFlags, SHSTOCKICONINFO* psii);
    }
}
