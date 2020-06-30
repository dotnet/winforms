﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        public struct DRAWITEMSTRUCT
        {
            public ODT CtlType;
            public uint CtlID;
            public uint itemID;
            public ODA itemAction;
            public ODS itemState;
            public IntPtr hwndItem;
            public Gdi32.HDC hDC;
            public RECT rcItem;
            public IntPtr itemData;
        }
    }
}
