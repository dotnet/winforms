// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Imm32
    {
        [DllImport(Libraries.Imm32, ExactSpelling = true)]
        public static extern BOOL ImmNotifyIME(IntPtr hIMC, NI dwAction, CPS dwIndex, int dwValue);
    }
}
