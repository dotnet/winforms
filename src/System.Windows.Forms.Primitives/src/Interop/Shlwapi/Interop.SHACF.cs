// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Shlwapi
    {
        [Flags]
        public enum SHACF : uint
        {
            DEFAULT = 0x00000000,
            FILESYSTEM = 0x00000001,
            URLHISTORY = 0x00000002,
            URLMRU = 0x00000004,
            URLALL = URLHISTORY | URLMRU,
            USETAB = 0x00000008,
            FILESYS_ONLY = 0x00000010,
            FILESYS_DIRS = 0x00000020,
            AUTOSUGGEST_FORCE_ON = 0x10000000,
            AUTOSUGGEST_FORCE_OFF = 0x20000000,
            AUTOAPPEND_FORCE_ON = 0x40000000,
            AUTOAPPEND_FORCE_OFF = 0x80000000,
        }
    }
}
