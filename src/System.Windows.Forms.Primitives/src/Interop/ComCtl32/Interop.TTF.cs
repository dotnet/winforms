// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum TTF
        {
            IDISHWND = 0x0001,
            CENTERTIP = 0x0002,
            RTLREADING = 0x0004,
            SUBCLASS = 0x0010,
            TRACK = 0x0020,
            ABSOLUTE = 0x0080,
            TRANSPARENT = 0x0100,
            PARSELINKS = 0x1000,
            DI_SETITEM = 0x8000
        }
    }
}
