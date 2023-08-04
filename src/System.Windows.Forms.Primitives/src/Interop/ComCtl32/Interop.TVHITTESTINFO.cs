﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public struct TVHITTESTINFO
        {
            public Point pt;
            public TVHT flags;
            public IntPtr hItem;
        }
    }
}
