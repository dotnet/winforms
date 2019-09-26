// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [Flags]
        public enum GC_WCH : uint
        {
            SIBLING = 0x00000001,
            CONTAINER = 0x00000002,
            CONTAINED = 0x00000003,
            ALL = 0x00000004,
            FREVERSEDIR = 0x08000000,
            FONLYNEXT = 0x10000000,
            FONLYPREV = 0x20000000,
            FSELECTED = 0x40000000,
        }
    }
}
