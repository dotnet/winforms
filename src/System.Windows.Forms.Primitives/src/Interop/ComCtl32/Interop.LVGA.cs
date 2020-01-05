// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum LVGA : uint
        {
            HEADER_LEFT = 0x00000001,
            HEADER_CENTER = 0x00000002,
            HEADER_RIGHT = 0x00000004,
            FOOTER_LEFT = 0x00000008,
            FOOTER_CENTER = 0x00000010,
            FOOTER_RIGHT = 0x00000020,
        }
    }
}
