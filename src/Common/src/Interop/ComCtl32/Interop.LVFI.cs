// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum LVFI : uint
        {
            PARAM = 0x0001,
            STRING = 0x0002,
            SUBSTRING = 0x0004,
            PARTIAL = 0x0008,
            WRAP = 0x0020,
            NEARESTXY = 0x0040,
        }
    }
}
