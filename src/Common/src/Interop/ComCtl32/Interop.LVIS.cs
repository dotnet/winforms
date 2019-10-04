// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum LVIS : uint
        {
            FOCUSED = 0x0001,
            SELECTED = 0x0002,
            CUT = 0x0004,
            DROPHILITED = 0x0008,
            OVERLAYMASK = 0x0F00,
            STATEIMAGEMASK = 0xF000,
        }
    }
}
