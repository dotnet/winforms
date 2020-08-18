// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.User32;

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum EM : uint
        {
            GETSEL = 0x00B0,
            SETSEL = 0x00B1,
            GETRECT = 0x00B2,
            SETRECT = 0x00B3,
            SETRECTNP = 0x00B4,
            SCROLL = 0x00B5,
            LINESCROLL = 0x00B6,
            SCROLLCARET = 0x00B7,
            GETMODIFY = 0x00B8,
            SETMODIFY = 0x00B9,
            GETLINECOUNT = 0x00BA,
            LINEINDEX = 0x00BB,
            SETHANDLE = 0x00BC,
            GETHANDLE = 0x00BD,
            GETTHUMB = 0x00BE,
            LINELENGTH = 0x00C1,
            REPLACESEL = 0x00C2,
            GETLINE = 0x00C4,
            LIMITTEXT = 0x00C5,
            CANUNDO = 0x00C6,
            UNDO = 0x00C7,
            FMTLINES = 0x00C8,
            LINEFROMCHAR = 0x00C9,
            SETTABSTOPS = 0x00CB,
            SETPASSWORDCHAR = 0x00CC,
            EMPTYUNDOBUFFER = 0x00CD,
            GETFIRSTVISIBLELINE = 0x00CE,
            SETREADONLY = 0x00CF,
            SETWORDBREAKPROC = 0x00D0,
            GETWORDBREAKPROC = 0x00D1,
            GETPASSWORDCHAR = 0x00D2,
            SETMARGINS = 0x00D3,
            GETMARGINS = 0x00D4,
            SETLIMITTEXT = LIMITTEXT,
            GETLIMITTEXT = 0x00D5,
            POSFROMCHAR = 0x00D6,
            CHARFROMPOS = 0x00D7,
            SETIMESTATUS = 0x00D8,
            GETIMESTATUS = 0x00D9,
            ENABLEFEATURE = 0x00DA,
            GETTEXTEX = WM.USER + 94,
            GETTEXTLENGTHEX = WM.USER + 95,
        }
    }
}
