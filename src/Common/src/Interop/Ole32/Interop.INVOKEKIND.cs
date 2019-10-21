// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [Flags]
        public enum INVOKEKIND : int
        {
            FUNC = 0x1,
            PROPERTYGET = 0x2,
            PROPERTYPUT = 0x4,
            PROPERTYPUTREF = 0x8
        }
    }
}
