
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Gdi32
    {
        public const int BITSPIXEL = 12;
        public const int PLANES = 14;
        public const int LOGPIXELSX = 88;
        public const int LOGPIXELSY = 90;

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);
    }
}
