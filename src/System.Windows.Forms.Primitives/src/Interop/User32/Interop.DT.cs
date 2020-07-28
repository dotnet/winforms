// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        /// <summary>
        ///  Text format flags for <see cref="DrawTextExW(Gdi32.HDC, ReadOnlySpan{char}, ref RECT, DT, ref DRAWTEXTPARAMS)"/>
        ///  and <see cref="DrawTextW(Gdi32.HDC, string, int, ref RECT, DT)"/>
        /// </summary>
        [Flags]
        public enum DT : uint
        {
            DEFAULT = TOP | LEFT,
            TOP = 0x00000000,
            LEFT = 0x00000000,
            CENTER = 0x00000001,
            RIGHT = 0x00000002,
            VCENTER = 0x00000004,
            BOTTOM = 0x00000008,
            WORDBREAK = 0x00000010,
            SINGLELINE = 0x00000020,
            EXPANDTABS = 0x00000040,
            TABSTOP = 0x00000080,
            NOCLIP = 0x00000100,
            EXTERNALLEADING = 0x00000200,
            CALCRECT = 0x00000400,
            NOPREFIX = 0x00000800,
            INTERNAL = 0x00001000,
            EDITCONTROL = 0x00002000,
            PATH_ELLIPSIS = 0x00004000,
            END_ELLIPSIS = 0x00008000,
            MODIFYSTRING = 0x00010000,
            RTLREADING = 0x00020000,
            WORD_ELLIPSIS = 0x00040000,
            NOFULLWIDTHCHARBREAK = 0x00080000,
            HIDEPREFIX = 0x00100000,
            PREFIXONLY = 0x00200000,
        }
    }
}
