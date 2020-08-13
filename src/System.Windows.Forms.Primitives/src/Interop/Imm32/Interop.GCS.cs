// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal partial class Imm32
    {
        [Flags]
        public enum GCS : int
        {
            COMPREADSTR = 0x0001,
            COMPREADATTR = 0x0002,
            COMPREADCLAUSE = 0x0004,
            COMPSTR = 0x0008,
            COMPATTR = 0x0010,
            COMPCLAUSE = 0x0020,
            CURSORPOS = 0x0080,
            DELTASTART = 0x0100,
            RESULTREADSTR = 0x0200,
            RESULTREADCLAUSE = 0x0400,
            RESULTSTR = 0x0800,
            RESULTCLAUSE = 0x1000
        }
    }
}
