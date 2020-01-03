// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Richedit
    {
        [Flags]
        public enum SEL : ushort
        {
            EMPTY = 0x0000,
            TEXT = 0x0001,
            OBJECT = 0x0002,
            MULTICHAR = 0x0004,
            MULTIOBJECT = 0x0008,
        }
    }
}
