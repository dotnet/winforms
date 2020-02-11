// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Comdlg32
    {
        [Flags]
        public enum FR : uint
        {
            DOWN = 0x00000001,
            WHOLEWORD = 0x00000002,
            MATCHCASE = 0x00000004,
            FINDNEXT = 0x00000008,
            REPLACE = 0x00000010,
            REPLACEALL = 0x00000020,
            DIALOGTERM = 0x00000040,
            SHOWHELP = 0x00000080,
            ENABLEHOOK = 0x00000100,
            ENABLETEMPLATE = 0x00000200,
            NOUPDOWN = 0x00000400,
            NOMATCHCASE = 0x00000800,
            NOWHOLEWORD = 0x00001000,
            ENABLETEMPLATEHANDLE = 0x00002000,
            HIDEUPDOWN = 0x00004000,
            HIDEMATCHCASE = 0x00008000,
            HIDEWHOLEWORD = 0x00010000,
            RAW = 0x00020000,
            MATCHDIAC = 0x20000000,
            MATCHKASHIDA = 0x40000000,
            MATCHALEFHAMZA = 0x80000000,
        }
    }
}
