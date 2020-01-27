// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.User32;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        public enum BFFM : uint
        {
            INITIALIZED = 1,
            SELCHANGED = 2,
            VALIDATEFAILEDA = 3,
            VALIDATEFAILEDW = 4,
            IUNKNOWN = 5,
            ENABLEOK = WM.USER + 101,
            SETSELECTIONW = WM.USER + 103,
            SETSTATUSTEXTW = WM.USER + 104,
            SETOKTEXT = WM.USER + 105,
            SETEXPANDED = WM.USER + 106,
        }
    }
}
