// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum TBSTATE : byte
        {
            CHECKED = 0x01,
            PRESSED = 0x02,
            ENABLED = 0x04,
            HIDDEN = 0x08,
            INDETERMINATE = 0x10,
            WRAP = 0x20,
            ELLIPSES = 0x40,
            MARKED = 0x80,
        }
    }
}
