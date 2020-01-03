// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [Flags]
        public enum POINTERINACTIVE : uint
        {
            ACTIVATEONENTRY = 0x1,
            DEACTIVATEONLEAVE = 0x2,
            ACTIVATEONDRAG = 0x4,
        }
    }
}
