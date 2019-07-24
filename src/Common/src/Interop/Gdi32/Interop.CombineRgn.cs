
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Gdi32
    {
        public const int RGN_AND = 1;
        public const int RGN_XOR = 3;
        public const int RGN_DIFF = 4;

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern RegionType CombineRgn(IntPtr hRgn, IntPtr hRgn1, IntPtr hRgn2, int nCombineMode);
    }
}
