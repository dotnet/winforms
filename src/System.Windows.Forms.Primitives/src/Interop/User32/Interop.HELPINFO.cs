﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

internal static partial class Interop
{
    internal static partial class User32
    {
        public struct HELPINFO
        {
            public uint cbSize;
            public HELPINFOENUM iContextType;
            public int iCtrlId;
            public IntPtr hItemHandle;
            public IntPtr dwContextId;
            public Point MousePos;
        }
    }
}
