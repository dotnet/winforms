// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum ScrollSW : uint
        {
            SCROLLCHILDREN = 0x0001,
            INVALIDATE = 0x0002,
            ERASE = 0x0004,
            SMOOTHSCROLL = 0x0010,
        }
    }
}
