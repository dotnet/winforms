// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum UISF : uint
        {
            HIDEFOCUS = 0x1,
            HIDEACCEL = 0x2,
            ACTIVE = 0x4,
        }
    }
}
