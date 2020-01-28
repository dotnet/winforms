﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum SBARS : uint
        {
            SIZEGRIP = 0x0100,
            TOOLTIPS = 0x0800,
        }
    }
}
