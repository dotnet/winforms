// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal partial class Imm32
    {
        [Flags]
        public enum IME_SMODE : uint
        {
            NONE = 0x0000,
            PLAURALCLAUSE = 0x0001,
            SINGLECONVERT = 0x0002,
            AUTOMATIC = 0x0004,
            PHRASEPREDICT = 0x0008,
            CONVERSATION = 0x0010,
            RESERVED = 0x0000F000,
        }
    }
}
