﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Richedit
    {
        [StructLayout(LayoutKind.Sequential, Pack = RichEditPack)]
        public struct TEXTRANGE
        {
            public Richedit.CHARRANGE chrg;
            public IntPtr lpstrText; // allocated by caller, zero terminated by RichEdit
        }
    }
}
