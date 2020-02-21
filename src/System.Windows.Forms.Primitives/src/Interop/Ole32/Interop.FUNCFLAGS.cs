// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [Flags]
        public enum FUNCFLAGS : ushort
        {
            FRESTRICTED = 0x0001,
            FSOURCE = 0x0002,
            FBINDABLE = 0x0004,
            FREQUESTEDIT = 0x0008,
            FDISPLAYBIND = 0x0010,
            FDEFAULTBIND = 0x0020,
            FHIDDEN = 0x0040,
            FUSESGETLASTERROR = 0x0080,
            FDEFAULTCOLLELEM = 0x0100,
            FUIDEFAULT = 0x0200,
            FNONBROWSABLE = 0x0400,
            FREPLACEABLE = 0x0800,
            FIMMEDIATEBIND = 0x1000,
        }
    }
}
