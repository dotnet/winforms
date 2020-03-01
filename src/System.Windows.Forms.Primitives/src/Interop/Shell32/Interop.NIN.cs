// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using static Interop.User32;

internal partial class Interop
{
    internal static partial class Shell32
    {
        private const int NINF_KEY = 0x1;

        public enum NIN
        {
            SELECT = (int)(WM.USER + 0),
            KEYSELECT = (int)(SELECT | NINF_KEY),
            BALLOONSHOW = (int)(WM.USER + 2),
            BALLOONHIDE = (int)(WM.USER + 3),
            BALLOONTIMEOUT = (int)(WM.USER + 4),
            BALLOONUSERCLICK = (int)(WM.USER + 5),
            POPUPOPEN = (int)(WM.USER + 6),
            POPUPCLOSE = (int)(WM.USER + 7)
        }
    }
}
