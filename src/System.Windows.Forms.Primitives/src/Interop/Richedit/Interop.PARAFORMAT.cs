// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Richedit
    {
        [StructLayout(LayoutKind.Sequential, Pack = RichEditPack)]
        public unsafe struct PARAFORMAT
        {
            public uint cbSize;
            public PFM dwMask;
            public PFN wNumbering;
            public ushort wReserved;
            public int dxStartIndent;
            public int dxRightIndent;
            public int dxOffset;
            public PFA wAlignment;
            public short cTabCount;
            public fixed int rgxTabs[MAX_TAB_STOPS];
        }
    }
}
