// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Richedit
    {
        [Flags]
        public enum ENM
        {
            NONE = 0x00000000,
            CHANGE = 0x00000001,
            UPDATE = 0x00000002,
            SCROLL = 0x00000004,
            SCROLLEVENTS = 0x00000008,
            DRAGDROPDONE = 0x00000010,
            PARAGRAPHEXPANDED = 0x00000020,
            PAGECHANGE = 0x00000040,
            CLIPFORMAT = 0x00000080,
            KEYEVENTS = 0x00010000,
            MOUSEEVENTS = 0x00020000,
            REQUESTRESIZE = 0x00040000,
            SELCHANGE = 0x00080000,
            DROPFILES = 0x00100000,
            PROTECTED = 0x00200000,
            CORRECTTEXT = 0x00400000,
            IMECHANGE = 0x00800000,
            LANGCHANGE = 0x01000000,
            OBJECTPOSITIONS = 0x02000000,
            LINK = 0x04000000,
            LOWFIRTF = 0x08000000,
            STARTCOMPOSITION = 0x10000000,
            ENDCOMPOSITION = 0x20000000,
            GROUPTYPINGCHANGE = 0x40000000,
            HIDELINKTOOLTIP = unchecked((int)0x80000000)
        }
    }
}
