// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        public static class OBJID
        {
            public const int WINDOW = 0x00000000;
            public const int NATIVEOM = unchecked((int)0xFFFFFFF0);
            public const int QUERYCLASSNAMEIDX = unchecked((int)0xFFFFFFF4);
            public const int SOUND = unchecked((int)0xFFFFFFF5);
            public const int ALERT = unchecked((int)0xFFFFFFF6);
            public const int CURSOR = unchecked((int)0xFFFFFFF7);
            public const int CARET = unchecked((int)0xFFFFFFF8);
            public const int SIZEGRIP = unchecked((int)0xFFFFFFF9);
            public const int HSCROLL = unchecked((int)0xFFFFFFFA);
            public const int VSCROLL = unchecked((int)0xFFFFFFFB);
            public const int CLIENT = unchecked((int)0xFFFFFFFC);
            public const int MENU = unchecked((int)0xFFFFFFFD);
            public const int TITLEBAR = unchecked((int)0xFFFFFFFE);
            public const int SYSMENU = unchecked((int)0xFFFFFFFF);
        }
    }
}
