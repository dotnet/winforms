// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum SIF : uint
        {
            RANGE = 0x0001,
            PAGE = 0x0002,
            POS = 0x0004,
            DISABLENOSCROLL = 0x0008,
            TRACKPOS = 0x0010,
            ALL = RANGE | PAGE | POS | TRACKPOS,
        }
    }
}
