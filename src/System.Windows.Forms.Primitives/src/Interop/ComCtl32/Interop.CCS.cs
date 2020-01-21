// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum CCS : uint
        {
            TOP = 0x00000001,
            NOMOVEY = 0x00000002,
            BOTTOM = 0x00000003,
            NORESIZE = 0x00000004,
            NOPARENTALIGN = 0x00000008,
            ADJUSTABLE = 0x00000020,
            NODIVIDER = 0x00000040,
            VERT = 0x00000080,
            LEFT = VERT | TOP,
            RIGHT = VERT | BOTTOM,
            NOMOVEX = VERT | NOMOVEY,
        }
    }
}
