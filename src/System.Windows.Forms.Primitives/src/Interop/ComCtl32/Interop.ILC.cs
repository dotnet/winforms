// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum ILC : uint
        {
            MASK = 0x00000001,
            COLOR = 0x00000000,
            COLOR4 = 0x00000004,
            COLOR8 = 0x00000008,
            COLOR16 = 0x00000010,
            COLOR24 = 0x00000018,
            COLOR32 = 0x00000020,
            MIRROR = 0x00002000,
        }
    }
}
