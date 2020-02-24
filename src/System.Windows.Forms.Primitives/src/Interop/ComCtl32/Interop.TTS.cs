// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum TTS : int
        {
            ALWAYSTIP = 0x01,
            NOPREFIX = 0x02,
            NOANIMATE = 0x10,
            NOFADE = 0x20,
            BALLOON = 0x40,
            CLOSE = 0x80,
            USEVISUALSTYLE = 0x100
        }
    }
}
