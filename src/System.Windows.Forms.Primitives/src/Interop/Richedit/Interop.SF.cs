// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Richedit
    {
        [Flags]
        public enum SF : uint
        {
            TEXT = 0x0001,
            RTF = 0x0002,
            RTFNOOBJS = 0x0003,
            TEXTIZED = 0x0004,
            UNICODE = 0x0010,
            USECODEPAGE = 0x0020,
            NCRFORNONASCII = 0x0040,
            RTFVAL = 0x0700,
            F_PWD = 0x0800,
            F_KEEPDOCINFO = 0x1000,
            F_PERSISTVIEWSCALE = 0x2000,
            F_PLAINRTF = 0x4000,
            F_SELECTION = 0x8000,
        }
    }
}
