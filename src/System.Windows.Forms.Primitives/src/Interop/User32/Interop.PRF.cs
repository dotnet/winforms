// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum PRF : uint
        {
            CHECKVISIBLE = 0x00000001,
            NONCLIENT = 0x00000002,
            CLIENT = 0x00000004,
            ERASEBKGND = 0x00000008,
            CHILDREN = 0x00000010,
            OWNED = 0x00000020,
        }
    }
}
