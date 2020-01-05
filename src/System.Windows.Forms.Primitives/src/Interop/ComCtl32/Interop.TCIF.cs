// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum TCIF : uint
        {
            TEXT = 0x0001,
            IMAGE = 0x0002,
            RTLREADING = 0x0004,
            PARAM = 0x0008,
            STATE = 0x0010,
        }
    }
}
