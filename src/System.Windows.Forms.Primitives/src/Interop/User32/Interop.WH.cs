// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum WH : int
        {
            MSGFILTER = -1,
            JOURNALRECORD = 0,
            JOURNALPLAYBACK = 1,
            KEYBOARD = 2,
            GETMESSAGE = 3,
            CALLWNDPROC = 4,
            CBT = 5,
            SYSMSGFILTER = 6,
            MOUSE = 7,
            HARDWARE = 8,
            DEBUG = 9,
            SHELL = 10,
            FOREGROUNDIDLE = 11,
            CALLWNDPROCRET = 12,
            KEYBOARD_LL = 13,
            MOUSE_LL = 14,
        }
    }
}
