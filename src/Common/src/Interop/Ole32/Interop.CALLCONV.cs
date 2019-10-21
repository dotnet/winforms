﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Ole32
    {
        public enum CALLCONV : int
        {
            FASTCALL = 0,
            CDECL = 1,
            MSCPASCAL = 2,
            PASCAL = MSCPASCAL,
            MACPASCAL = 3,
            STDCALL = 4,
            FPFASTCALL = 5,
            SYSCALL = 6,
            MPWCDECL = 7,
            MPWPASCAL = 8,
            MAX = 9
        }
    }
}
