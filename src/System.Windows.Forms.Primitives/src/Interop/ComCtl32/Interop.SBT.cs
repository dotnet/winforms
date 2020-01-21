// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum SBT : uint
        {
            NOBORDERS = 0x0100,
            POPOUT = 0x0200,
            RTLREADING = 0x0400,
            NOTABPARSING = 0x0800,
            OWNERDRAW = 0x1000
        }
    }
}
