// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal partial class Kernel32
    {
        [Flags]
        public enum FormatMessageOptions : uint
        {
            ALLOCATE_BUFFER = 0x00000100,
            IGNORE_INSERTS = 0x00000200,
            FROM_STRING = 0x00000400,
            FROM_HMODULE = 0x00000800,
            FROM_SYSTEM = 0x00001000,
            ARGUMENT_ARRAY = 0x00002000,
            MAX_WIDTH_MASK = 0x000000FF
        }
    }
}
