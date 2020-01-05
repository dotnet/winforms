// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum HICF : uint
        {
            OTHER = 0x00000000,
            MOUSE = 0x00000001,
            ARROWKEYS = 0x00000002,
            ACCELERATOR = 0x00000004,
            DUPACCEL = 0x00000008,
            ENTERING = 0x00000010,
            LEAVING = 0x00000020,
            RESELECT = 0x00000040,
            LMOUSE = 0x00000080,
            TOGGLEDROPDOWN = 0x00000100,
        }
    }
}
