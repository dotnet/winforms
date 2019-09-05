// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum AcceleratorFlags : byte
        {
            FVIRTKEY = 0x01,
            FNOINVERT = 0x02,
            FSHIFT = 0x04,
            FCONTROL = 0x08,
            FALT = 0x10,
        }
    }
}
