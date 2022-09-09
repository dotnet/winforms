﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Imm32
    {
        [DllImport(Libraries.Imm32)]
        public static extern BOOL ImmGetConversionStatus(IntPtr hIMC, out IME_CMODE lpfdwConversion, out IME_SMODE lpfdwSentence);
    }
}
