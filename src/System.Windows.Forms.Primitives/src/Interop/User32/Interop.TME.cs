// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum TME : uint
        {
            HOVER = 0x00000001,
            LEAVE = 0x00000002,
            NONCLIENT = 0x00000010,
            QUERY = 0x40000000,
            CANCEL = 0x80000000,
        }
    }
}
