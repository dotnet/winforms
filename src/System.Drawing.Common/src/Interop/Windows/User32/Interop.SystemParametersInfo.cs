// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum SystemParametersAction : uint
        {
            SPI_GETICONTITLELOGFONT = 0x1F,
            SPI_GETNONCLIENTMETRICS = 0x29
        }

        [return: MarshalAs(UnmanagedType.Bool)]
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.User32)]
        public static unsafe partial bool SystemParametersInfoW(
#else
        [DllImport(Libraries.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern unsafe bool SystemParametersInfoW(
#endif
            SystemParametersAction uiAction,
            uint uiParam,
            void* pvParam,
            uint fWinIni);
    }
}
