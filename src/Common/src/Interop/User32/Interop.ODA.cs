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
        public enum ODA : uint
        {
            DRAWENTIRE = 0x1,
            SELECT = 0x2,
            FOCUS = 0x4,
        }
    }
}
