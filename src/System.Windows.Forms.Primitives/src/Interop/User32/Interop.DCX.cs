// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum DCX : uint
        {
            WINDOW              = 0x00000001,
            CACHE               = 0x00000002,
            NORESETATTRS        = 0x00000004,
            CLIPCHILDREN        = 0x00000008,
            CLIPSIBLINGS        = 0x00000010,
            PARENTCLIP          = 0x00000020,
            EXCLUDERGN          = 0x00000040,
            INTERSECTRGN        = 0x00000080,
            EXCLUDEUPDATE       = 0x00000100,
            INTERSECTUPDATE     = 0x00000200,
            LOCKWINDOWUPDATE    = 0x00000400,
            USESTYLE            = 0x00010000,
            VALIDATE            = 0x00200000,
        }
    }
}
