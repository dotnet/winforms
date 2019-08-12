// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    /// <summary>
    ///  Edit Control Messages.
    ///  Copied form winuser.h
    /// </summary>
    public static class EditMessages
    {
        public const int EM_GETSEL = 0x00B0;
        public const int EM_SETSEL = 0x00B1;
        public const int EM_GETRECT = 0x00B2;
        public const int EM_SETRECT = 0x00B3;
        public const int EM_SETRECTNP = 0x00B4;
        public const int EM_SCROLL = 0x00B5;
        public const int EM_LINESCROLL = 0x00B6;
        public const int EM_SCROLLCARET = 0x00B7;
        public const int EM_GETMODIFY = 0x00B8;
        public const int EM_SETMODIFY = 0x00B9;
        public const int EM_GETLINECOUNT = 0x00BA;
        public const int EM_LINEINDEX = 0x00BB;
        public const int EM_SETHANDLE = 0x00BC;
        public const int EM_GETHANDLE = 0x00BD;
        public const int EM_GETTHUMB = 0x00BE;
        public const int EM_LINELENGTH = 0x00C1;
        public const int EM_REPLACESEL = 0x00C2;
        public const int EM_GETLINE = 0x00C4;
        public const int EM_LIMITTEXT = 0x00C5;
        public const int EM_CANUNDO = 0x00C6;
        public const int EM_UNDO = 0x00C7;
        public const int EM_FMTLINES = 0x00C8;
        public const int EM_LINEFROMCHAR = 0x00C9;
        public const int EM_SETTABSTOPS = 0x00CB;
        public const int EM_SETPASSWORDCHAR = 0x00CC;
        public const int EM_EMPTYUNDOBUFFER = 0x00CD;
        public const int EM_GETFIRSTVISIBLELINE = 0x00CE;
        public const int EM_SETREADONLY = 0x00CF;
        public const int EM_SETWORDBREAKPROC = 0x00D0;
        public const int EM_GETWORDBREAKPROC = 0x00D1;
        public const int EM_GETPASSWORDCHAR = 0x00D2;
        public const int EM_SETMARGINS = 0x00D3;
        public const int EM_GETMARGINS = 0x00D4;
        public const int EM_GETLIMITTEXT = 0x00D5;
        public const int EM_POSFROMCHAR = 0x00D6;
        public const int EM_CHARFROMPOS = 0x00D7;
    }
}
