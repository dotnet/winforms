// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum LVIF : uint
        {
            TEXT = 0x00000001,
            IMAGE = 0x00000002,
            PARAM = 0x00000004,
            STATE = 0x00000008,
            INDENT = 0x00000010,
            NORECOMPUTE = 0x00000800,
            GROUPID = 0x00000100,
            COLUMNS = 0x00000200,
            COLFMT = 0x00010000,
        }
    }
}
