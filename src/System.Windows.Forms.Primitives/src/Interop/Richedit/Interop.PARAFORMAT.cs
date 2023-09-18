// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.UI.Controls.RichEdit;
internal partial class Interop
{
    internal static partial class Richedit
    {
        [StructLayout(LayoutKind.Sequential, Pack = RichEditPack)]
        public unsafe struct PARAFORMAT
        {
            public uint cbSize;
            public PFM dwMask;
            public PARAFORMAT_NUMBERING wNumbering;
            public ushort wReserved;
            public int dxStartIndent;
            public int dxRightIndent;
            public int dxOffset;
            public PFA wAlignment;
            public short cTabCount;
            public fixed int rgxTabs[(int)PInvoke.MAX_TAB_STOPS];
        }
    }
}
