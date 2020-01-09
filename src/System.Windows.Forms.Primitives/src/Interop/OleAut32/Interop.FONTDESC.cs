// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Oleaut32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct FONTDESC
        {
            public uint cbSizeOfStruct;
            public string? lpstrName;
            public long cySize;
            public short sWeight;
            public short sCharset;
            public BOOL fItalic;
            public BOOL fUnderline;
            public BOOL fStrikethrough;
        }
    }
}
