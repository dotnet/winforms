﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, SetLastError = true)]
        public static extern unsafe uint SendInput(uint cInputs, INPUT* pInputs, int cbSize);

        public static unsafe uint SendInput(uint cInputs, Span<INPUT> pInputs, int cbSize)
        {
            fixed (INPUT* input = &MemoryMarshal.GetReference(pInputs))
            {
                return SendInput(cInputs, input, cbSize);
            }
        }
    }
}
