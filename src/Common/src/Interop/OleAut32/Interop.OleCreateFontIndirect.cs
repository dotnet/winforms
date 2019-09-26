// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Oleaut32
    {
        [DllImport(Libraries.Oleaut32, ExactSpelling = true, PreserveSig = false)]
        public static extern Ole32.IFont OleCreateFontIndirect(ref FONTDESC lpFontDesc, ref Guid riid);

        [DllImport(Libraries.Oleaut32, ExactSpelling = true, EntryPoint = "OleCreateFontIndirect", PreserveSig = false)]
        public static extern Ole32.IFontDisp OleCreateIFontDispIndirect(ref FONTDESC lpFontDesc, ref Guid riid);
    }
}
