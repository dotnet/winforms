// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [Flags]
        public enum PARAMFLAG : ushort
        {
            NONE = 0x00,
            FIN = 0x01,
            FOUT = 0x02,
            FLCID = 0x04,
            FRETVAL = 0x08,
            FOPT = 0x10,
            FHASDEFAULT = 0x20,
            FHASCUSTDATA = 0x40,
        }
    }
}
