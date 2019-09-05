// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        public struct DRAWITEMSTRUCT
        {
            public ItemControlType CtlType;
            public uint CtlID;
            public uint itemID;
            public DrawItemAction itemAction;
            public DrawItemState itemState;
            public IntPtr hwndItem;
            public IntPtr hDC;
            public RECT rcItem;
            public IntPtr itemData;
        }
    }
}
