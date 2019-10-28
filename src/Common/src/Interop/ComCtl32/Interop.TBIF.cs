// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum TBIF : uint
        {
            IMAGE = 0x00000001,
            TEXT = 0x00000002,
            STATE = 0x00000004,
            STYLE = 0x00000008,
            LPARAM = 0x00000010,
            COMMAND = 0x00000020,
            SIZE = 0x00000040,
            BYINDEX = 0x80000000,
        }
    }
}
