// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public ShowWindowCommand showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            // rcNormalPosition was a by-value RECT structure
            public int rcNormalPosition_left;
            public int rcNormalPosition_top;
            public int rcNormalPosition_right;
            public int rcNormalPosition_bottom;
        }
    }
}
