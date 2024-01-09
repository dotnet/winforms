// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.Gdi32, EntryPoint = "AddFontResourceExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        internal static partial int AddFontResourceEx(
#else
        [DllImport(Libraries.Gdi32, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int AddFontResourceEx(
#endif
            string lpszFilename,
            int fl,
            IntPtr pdv);

        internal static int AddFontFile(string fileName)
        {
            return AddFontResourceEx(fileName, /*FR_PRIVATE*/ 0x10, IntPtr.Zero);
        }
    }
}
