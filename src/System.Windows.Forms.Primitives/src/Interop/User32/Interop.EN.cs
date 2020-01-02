// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum EN : uint
        {
            SETFOCUS = 0x0100,
            KILLFOCUS = 0x0200,
            CHANGE = 0x0300,
            UPDATE = 0x0400,
            ERRSPACE = 0x0500,
            MAXTEXT = 0x0501,
            HSCROLL = 0x0601,
            VSCROLL = 0x0602,
            ALIGN_LTR_EC = 0x0700,
            ALIGN_RTL_EC = 0x0701,
            BEFORE_PASTE = 0x0800,
            AFTER_PASTE = 0x0801
        }
    }
}
