// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [Flags]
        public enum OLECMDF : uint
        {
            SUPPORTED = 0x1,
            ENABLED = 0x2,
            LATCHED = 0x4,
            NINCHED = 0x8,
            INVISIBLE = 0x10,
            DEFHIDEONCTXTMENU = 0x20
        }
    }
}
