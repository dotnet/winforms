// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum TCN : int
        {
            FIRST = 0 - 550,
            KEYDOWN = FIRST - 0,
            SELCHANGE = FIRST - 1,
            SELCHANGING = FIRST - 2,
            GETOBJECT = FIRST - 3,
            FOCUSCHANGE = FIRST - 4,
        }
    }
}
