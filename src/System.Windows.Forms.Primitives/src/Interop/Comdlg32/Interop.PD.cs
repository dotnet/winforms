// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal partial class Comdlg32
    {
        [Flags]
        public enum PD : uint
        {
            ALLPAGES = 0x00000000,
            SELECTION = 0x00000001,
            PAGENUMS = 0x00000002,
            NOSELECTION = 0x00000004,
            NOPAGENUMS = 0x00000008,
            COLLATE = 0x00000010,
            PRINTTOFILE = 0x00000020,
            PRINTSETUP = 0x00000040,
            NOWARNING = 0x00000080,
            RETURNDC = 0x00000100,
            RETURNIC = 0x00000200,
            RETURNDEFAULT = 0x00000400,
            SHOWHELP = 0x00000800,
            ENABLEPRINTHOOK = 0x00001000,
            ENABLESETUPHOOK = 0x00002000,
            ENABLEPRINTTEMPLATE = 0x00004000,
            ENABLESETUPTEMPLATE = 0x00008000,
            ENABLEPRINTTEMPLATEHANDLE = 0x00010000,
            ENABLESETUPTEMPLATEHANDLE = 0x00020000,
            USEDEVMODECOPIES = 0x00040000,
            USEDEVMODECOPIESANDCOLLATE = 0x00040000,
            DISABLEPRINTTOFILE = 0x00080000,
            HIDEPRINTTOFILE = 0x00100000,
            NONETWORKBUTTON = 0x00200000,
            CURRENTPAGE = 0x00400000,
            NOCURRENTPAGE = 0x00800000,
            EXCLUSIONFLAGS = 0x01000000,
            USELARGETEMPLATE = 0x10000000
        }
    }
}
