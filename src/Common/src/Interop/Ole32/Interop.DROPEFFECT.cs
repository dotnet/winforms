// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [Flags]
        public enum DROPEFFECT : uint
        {
            NONE = 0,
            COPY = 1,
            MOVE = 2,
            LINK = 4,
            SCROLL = 0x80000000,
        }
    }
}
