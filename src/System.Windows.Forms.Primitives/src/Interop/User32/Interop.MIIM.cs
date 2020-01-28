// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum MIIM : uint
        {
            STATE = 0x00000001,
            ID = 0x00000002,
            SUBMENU = 0x00000004,
            CHECKMARKS = 0x00000008,
            TYPE = 0x00000010,
            DATA = 0x00000020,
            STRING = 0x00000040,
            BITMAP = 0x00000080,
            FTYPE = 0x00000100,
        }
    }
}
