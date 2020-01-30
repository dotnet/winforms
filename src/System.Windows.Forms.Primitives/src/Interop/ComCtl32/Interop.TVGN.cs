// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum TVGN : uint
        {
            ROOT = 0x0000,
            NEXT = 0x0001,
            PREVIOUS = 0x0002,
            PARENT = 0x0003,
            CHILD = 0x0004,
            FIRSTVISIBLE = 0x0005,
            NEXTVISIBLE = 0x0006,
            PREVIOUSVISIBLE = 0x0007,
            DROPHILITE = 0x0008,
            CARET = 0x0009,
            LASTVISIBLE = 0x000A,
            NEXTSELECTED = 0x000B,
        }
    }
}
