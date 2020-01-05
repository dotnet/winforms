// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [Flags]
        public enum IMPLTYPEFLAG : int
        {
            FDEFAULT = 0x1,
            FSOURCE = 0x2,
            FRESTRICTED = 0x4,
            FDEFAULTVTABLE = 0x8
        }
    }
}
