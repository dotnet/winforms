// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal static partial class Hhctl
    {
        public enum HH : uint
        {
            DISPLAY_TOPIC = 0x00,
            HELP_FINDER = 0x00,
            DISPLAY_TOC = 0x01,
            DISPLAY_INDEX = 0x02,
            DISPLAY_SEARCH = 0x03,
            SET_WIN_TYPE = 0x04,
            GET_WIN_TYPE = 0x05,
            GET_WIN_HANDLE = 0x06,
            ENUM_INFO_TYPE = 0x07,
            SET_INFO_TYPE = 0x08,
            SYNC = 0x09,
            RESERVED1 = 0x0A,
            RESERVED2 = 0x0B,
            RESERVED3 = 0x0C,
            KEYWORD_LOOKUP = 0x0D,
            DISPLAY_TEXT_POPUP = 0x0E,
            HELP_CONTEXT = 0x0F,
            TP_HELP_CONTEXTMENU = 0x10,
            TP_HELP_WM_HELP = 0x11,
            CLOSE_ALL = 0x12,
            ALINK_LOOKUP = 0x13,
            GET_LAST_ERROR = 0x14,
            ENUM_CATEGORY = 0x15,
            ENUM_CATEGORY_IT = 0x16,
            RESET_IT_FILTER = 0x17,
            SET_INCLUSIVE_FILTER = 0x18,
            SET_EXCLUSIVE_FILTER = 0x19,
            INITIALIZE = 0x1C,
            UNINITIALIZE = 0x1D,
            SAFE_DISPLAY_TOPIC = 0x20,
            PRETRANSLATEMESSAGE = 0xFD,
            SET_GLOBAL_PROPERTY = 0xFC
        }
    }
}
