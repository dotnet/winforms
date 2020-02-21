// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal static partial class Shell32
    {
        public enum SIATTRIBFLAGS : uint
        {
            AND = 0x1,
            OR = 0x2,
            APPCOMPAT = 0x3,
            MASK = 0x3,
            ALLITEMS = 0x4000
        }
    }
}
