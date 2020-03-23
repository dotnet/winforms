// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum TVE : uint
        {
            COLLAPSE = 0x0001,
            EXPAND = 0x0002,
            TOGGLE = 0x0003,
            EXPANDPARTIAL = 0x4000,
            COLLAPSERESET = 0x8000,
        }
    }
}
