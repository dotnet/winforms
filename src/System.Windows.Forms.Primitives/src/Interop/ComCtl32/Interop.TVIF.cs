// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum TVIF : uint
        {
            TEXT = 0x0001,
            IMAGE = 0x0002,
            PARAM = 0x0004,
            STATE = 0x0008,
            HANDLE = 0x0010,
            SELECTEDIMAGE = 0x0020,
            CHILDREN = 0x0040,
            INTEGRAL = 0x0080,
            STATEEX = 0x0100,
            EXPANDEDIMAGE = 0x0200,
            DI_SETITEM = 0x1000,
        }
    }
}
