// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Mshtml
    {
        [Flags]
        public enum DOCHOSTUIFLAG
        {
            DIALOG = 0x00000001,
            DISABLE_HELP_MENU = 0x00000002,
            NO3DBORDER = 0x00000004,
            SCROLL_NO = 0x00000008,
            DISABLE_SCRIPT_INACTIVE = 0x00000010,
            OPENNEWWIN = 0x00000020,
            DISABLE_OFFSCREEN = 0x00000040,
            FLAT_SCROLLBAR = 0x00000080,
            DIV_BLOCKDEFAULT = 0x00000100,
            ACTIVATE_CLIENTHIT_ONLY = 0x00000200,
            OVERRIDEBEHAVIORFACTORY = 0x00000400,
            CODEPAGELINKEDFONTS = 0x00000800,
            URL_ENCODING_DISABLE_UTF8 = 0x00001000,
            URL_ENCODING_ENABLE_UTF8 = 0x00002000,
            ENABLE_FORMS_AUTOCOMPLETE = 0x00004000,
            ENABLE_INPLACE_NAVIGATION = 0x00010000,
            IME_ENABLE_RECONVERSION = 0x00020000,
            THEME = 0x00040000,
            NOTHEME = 0x00080000,
            NOPICS = 0x00100000,
            NO3DOUTERBORDER = 0x00200000,
            DISABLE_EDIT_NS_FIXUP = 0x00400000,
            LOCAL_MACHINE_ACCESS_CHECK = 0x00800000,
            DISABLE_UNTRUSTEDPROTOCOL = 0x01000000,
            HOST_NAVIGATES = 0x02000000,
            ENABLE_REDIRECT_NOTIFICATION = 0x04000000,
            USE_WINDOWLESS_SELECTCONTROL = 0x08000000,
            USE_WINDOWED_SELECTCONTROL = 0x10000000,
            ENABLE_ACTIVEX_INACTIVATE_MODE = 0x20000000,
            DPI_AWARE = 0x40000000
        }
    }
}
