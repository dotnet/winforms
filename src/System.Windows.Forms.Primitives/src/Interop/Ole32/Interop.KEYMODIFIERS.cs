// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [Flags]
        public enum KEYMODIFIERS : uint
        {
            SHIFT = 0x0001,
            CONTROL = 0x0002,
            ALT = 0x0004
        }
    }
}
