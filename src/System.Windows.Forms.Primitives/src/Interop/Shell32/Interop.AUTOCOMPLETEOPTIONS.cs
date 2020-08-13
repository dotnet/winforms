// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        [Flags]
        public enum AUTOCOMPLETEOPTIONS : uint
        {
            NONE = 0x00,
            AUTOSUGGEST = 0x01,
            AUTOAPPEND = 0x02,
            SEARCH = 0x04,
            FILTERPREFIXES = 0x08,
            USETAB = 0x10,
            UPDOWNKEYDROPSLIST = 0x20,
            RTLREADING = 0x40,
            WORD_FILTER = 0x80,
            NOPREFIXFILTERING = 0x100,
        }
    }
}
