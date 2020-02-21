// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum TVN : int
        {
            FIRST = 0 - 400,
            KEYDOWN = FIRST - 12,
            GETINFOTIPW = FIRST - 14,
            SINGLEEXPAND = FIRST - 15,
            SELCHANGINGW = FIRST - 50,
            SELCHANGEDW = FIRST - 51,
            GETDISPINFOW = FIRST - 52,
            SETDISPINFOW = FIRST - 53,
            ITEMEXPANDINGW = FIRST - 54,
            ITEMEXPANDEDW = FIRST - 55,
            BEGINDRAGW = FIRST - 56,
            BEGINRDRAGW = FIRST - 57,
            DELETEITEMW = FIRST - 58,
            BEGINLABELEDITW = FIRST - 59,
            ENDLABELEDITW = FIRST - 60,
        }
    }
}
