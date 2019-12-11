// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        public const int LB_ERR = -1;
        public const int LB_ERRSPACE = -2;

        public enum LB : uint
        {
            ADDSTRING = 0x0180,
            INSERTSTRING = 0x0181,
            DELETESTRING = 0x0182,
            SELITEMRANGEEX = 0x0183,
            RESETCONTENT = 0x0184,
            SETSEL = 0x0185,
            SETCURSEL = 0x0186,
            GETSEL = 0x0187,
            GETCURSEL = 0x0188,
            GETTEXT = 0x0189,
            GETTEXTLEN = 0x018A,
            GETCOUNT = 0x018B,
            SELECTSTRING = 0x018C,
            DIR = 0x018D,
            GETTOPINDEX = 0x018E,
            FINDSTRING = 0x018F,
            GETSELCOUNT = 0x0190,
            GETSELITEMS = 0x0191,
            SETTABSTOPS = 0x0192,
            GETHORIZONTALEXTENT = 0x0193,
            SETHORIZONTALEXTENT = 0x0194,
            SETCOLUMNWIDTH = 0x0195,
            ADDFILE = 0x0196,
            SETTOPINDEX = 0x0197,
            GETITEMRECT = 0x0198,
            GETITEMDATA = 0x0199,
            SETITEMDATA = 0x019A,
            SELITEMRANGE = 0x019B,
            SETANCHORINDEX = 0x019C,
            GETANCHORINDEX = 0x019D,
            SETCARETINDEX = 0x019E,
            GETCARETINDEX = 0x019F,
            SETITEMHEIGHT = 0x01A0,
            GETITEMHEIGHT = 0x01A1,
            FINDSTRINGEXACT = 0x01A2,
            SETLOCALE = 0x01A5,
            GETLOCALE = 0x01A6,
            SETCOUNT = 0x01A7,
            INITSTORAGE = 0x01A8,
            ITEMFROMPOINT = 0x01A9,
            MULTIPLEADDSTRING = 0x01B1,
            GETLISTBOXINFO = 0x01B2,
        }
    }
}
