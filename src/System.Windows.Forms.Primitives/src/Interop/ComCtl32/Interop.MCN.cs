// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum MCN
        {
            FIRST = 0 - 746,
            LAST = 0 - 752,
            SELECT = FIRST,
            GETDAYSTATE = FIRST - 1,
            SELCHANGE = FIRST - 3,
            VIEWCHANGE = FIRST - 4
        }
    }
}
