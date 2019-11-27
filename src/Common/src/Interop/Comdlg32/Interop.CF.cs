// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal partial class Comdlg32
    {
        [Flags]
        public enum CF : uint
        {
            SCREENFONTS = 0x00000001,
            PRINTERFONTS = 0x00000002,
            BOTH = SCREENFONTS | PRINTERFONTS,
            SHOWHELP = 0x00000004,
            ENABLEHOOK = 0x00000008,
            ENABLETEMPLATE = 0x00000010,
            ENABLETEMPLATEHANDLE = 0x00000020,
            INITTOLOGFONTSTRUCT = 0x00000040,
            USESTYLE = 0x00000080,
            EFFECTS = 0x00000100,
            APPLY = 0x00000200,
            ANSIONLY = 0x00000400,
            SCRIPTSONLY = ANSIONLY,
            NOVECTORFONTS = 0x00000800,
            NOOEMFONTS = NOVECTORFONTS,
            NOSIMULATIONS = 0x00001000,
            LIMITSIZE = 0x00002000,
            FIXEDPITCHONLY = 0x00004000,
            WYSIWYG = 0x00008000,
            FORCEFONTEXIST = 0x00010000,
            SCALABLEONLY = 0x00020000,
            TTONLY = 0x00040000,
            NOFACESEL = 0x00080000,
            NOSTYLESEL = 0x00100000,
            NOSIZESEL = 0x00200000,
            SELECTSCRIPT = 0x00400000,
            NOSCRIPTSEL = 0x00800000,
            NOVERTFONTS = 0x01000000,
            INACTIVEFONTS = 0x02000000,
        }
    }
}
