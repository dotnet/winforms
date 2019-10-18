// Licensed to the .NET Foundation under one or more agreements.
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
            MSCPASCAL,
            PASCAL = MSCPASCAL,
            MACPASCAL,
            STDCALL,
            FPFASTCALL,
            SYSCALL,
            MPWCDECL,
            MPWPASCAL,
            MAX
        }
    }
}
