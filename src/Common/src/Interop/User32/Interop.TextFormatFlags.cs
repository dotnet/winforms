// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum TextFormatFlags : uint
        {
            DT_DEFAULT = DT_TOP | DT_LEFT,
            DT_TOP = 0x00000000,
            DT_LEFT = 0x00000000,
            DT_CENTER = 0x00000001,
            DT_RIGHT = 0x00000002,
            DT_VCENTER = 0x00000004,
            DT_BOTTOM = 0x00000008,
            DT_WORDBREAK = 0x00000010,
            DT_SINGLELINE = 0x00000020,
            DT_EXPANDTABS = 0x00000040,
            DT_TABSTOP = 0x00000080,
            DT_NOCLIP = 0x00000100,
            DT_EXTERNALLEADING = 0x00000200,
            DT_CALCRECT = 0x00000400,
            DT_NOPREFIX = 0x00000800,
            DT_INTERNAL = 0x00001000,
            DT_EDITCONTROL = 0x00002000,
            DT_PATH_ELLIPSIS = 0x00004000,
            DT_END_ELLIPSIS = 0x00008000,
            DT_MODIFYSTRING = 0x00010000,
            DT_RTLREADING = 0x00020000,
            DT_WORD_ELLIPSIS = 0x00040000,
            DT_NOFULLWIDTHCHARBREAK = 0x00080000,
            DT_HIDEPREFIX = 0x00100000,
            DT_PREFIXONLY = 0x00200000,
        }
    }
}
