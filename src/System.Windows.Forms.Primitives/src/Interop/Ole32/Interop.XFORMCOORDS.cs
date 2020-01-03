// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [Flags]
        public enum XFORMCOORDS : uint
        {
            POSITION = 0x01,
            SIZE = 0x02,
            HIMETRICTOCONTAINER = 0x04,
            CONTAINERTOHIMETRIC = 0x08,
            EVENTCOMPAT = 0x10,
        }
    }
}
