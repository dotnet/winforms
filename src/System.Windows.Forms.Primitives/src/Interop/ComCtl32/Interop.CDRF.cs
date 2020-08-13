// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum CDRF : uint
        {
            DODEFAULT = 0x00000000,
            NEWFONT = 0x00000002,
            SKIPDEFAULT = 0x00000004,
            DOERASE = 0x00000008,
            SKIPPOSTPAINT = 0x00000100,
            NOTIFYPOSTPAINT = 0x00000010,
            NOTIFYITEMDRAW = 0x00000020,
            NOTIFYSUBITEMDRAW = 0x00000020,
            NOTIFYPOSTERASE = 0x00000040,
        }
    }
}
