// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum DTN
        {
            FIRST = 0 - 740,
            LAST = 0 - 745,
            FIRST2 = 0 - 753,
            LAST2 = 0 - 799,
            FORMATQUERYW = FIRST - 2,
            FORMATW = FIRST - 3,
            WMKEYDOWNW = FIRST - 4,
            USERSTRINGW = FIRST - 5,
            CLOSEUP = FIRST2,
            DROPDOWN = FIRST2 - 1,
            DATETIMECHANGE = FIRST2 - 6
        }
    }
}
