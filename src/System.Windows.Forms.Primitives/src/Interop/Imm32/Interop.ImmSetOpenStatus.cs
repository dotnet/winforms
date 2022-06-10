﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Imm32
    {
        [LibraryImport(Libraries.Imm32)]
        public static partial BOOL ImmSetOpenStatus(IntPtr hIMC, BOOL open);
    }
}
