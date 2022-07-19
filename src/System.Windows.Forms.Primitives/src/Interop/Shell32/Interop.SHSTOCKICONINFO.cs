// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct SHSTOCKICONINFO
        {
            public uint cbSize;
            public IntPtr hIcon;
            public int iSysImageIndex;
            public int iIcon;
            private fixed char _szPath[Kernel32.MAX_PATH];

            private ReadOnlySpan<char> szPath
            {
                get { fixed (char* c = _szPath) { return new ReadOnlySpan<char>(c, Kernel32.MAX_PATH); } }
            }

            public ReadOnlySpan<char> Path
            {
                get { return szPath.SliceAtFirstNull(); }
            }
        }
    }
}
