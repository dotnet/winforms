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
        public enum DrawItemAction : uint
        {
            ODA_DRAWENTIRE = 0x1,
            ODA_SELECT = 0x2,
            ODA_FOCUS = 0x4,
        }
    }
}
