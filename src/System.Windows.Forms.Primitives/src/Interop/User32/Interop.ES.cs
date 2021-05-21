// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum ES : uint
        {
            LEFT        = 0x0000,
            CENTER      = 0x0001,
            RIGHT       = 0x0002,
            MULTILINE   = 0x0004,
            UPPERCASE   = 0x0008,
            LOWERCASE   = 0x0010,
            PASSWORD    = 0x0020,
            AUTOVSCROLL = 0x0040,
            AUTOHSCROLL = 0x0080,
            NOHIDESEL   = 0x0100,
            OEMCONVERT  = 0x0400,
            READONLY    = 0x0800,
            WANTRETURN  = 0x1000,
            NUMBER      = 0x2000
        }
    }
}
