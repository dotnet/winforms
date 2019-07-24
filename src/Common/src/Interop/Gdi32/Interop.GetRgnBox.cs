
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Gdi32
    {
        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern Interop.Gdi32.RegionType GetRgnBox(IntPtr hRegion, ref RECT clipRect);
    }
}
