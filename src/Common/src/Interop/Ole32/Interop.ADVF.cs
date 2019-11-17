// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [Flags]
        public enum ADVF : uint
        {
            NODATA = 0x01,
            PRIMEFIRST = 0x02,
            ONLYONCE = 0x04,
            CACHE_NOHANDLER = 0x08,
            CACHE_FORCEBUILTIN = 0x10,
            CACHE_ONSAVE = 0x20,
            DATAONSTOP = 0x40,
        }
    }
}
