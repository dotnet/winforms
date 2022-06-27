// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [LibraryImport(Libraries.Gdi32)]
        public unsafe static partial BOOL EnumEnhMetaFile(
            HDC hdc,
            HENHMETAFILE hmf,
            Enhmfenumproc proc,
            IntPtr param,
            RECT* lpRect);
    }
}
