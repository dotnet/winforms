// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum TB : int
        {
            FIRST = 0x1000,
            GETBUTTONINFOW = FIRST + 63,
            SETBUTTONINFOW = FIRST + 64,
            INSERTBUTTONW = FIRST + 67,
        }
    }
}
