// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum HBMMENU
        {
            CALLBACK = -1,
            SYSTEM = 1,
            MBAR_RESTORE = 2,
            MBAR_MINIMIZE = 3,
            MBAR_CLOSE = 5,
            MBAR_CLOSE_D = 6,
            MBAR_MINIMIZE_D = 7,
            POPUP_CLOSE = 8,
            POPUP_RESTORE = 9,
            POPUP_MAXIMIZE = 10,
            POPUP_MINIMIZE = 11
        }
    }
}
