// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{

    internal static partial class ComCtl32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class LVHITTESTINFO
        {
            public int pt_x;
            public int pt_y;
            public int flags = 0;
            public int iItem = 0;
            public int iSubItem = 0;
        }
    }
}
