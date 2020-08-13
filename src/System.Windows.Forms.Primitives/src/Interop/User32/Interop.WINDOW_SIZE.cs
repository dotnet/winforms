// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        // We named this enum WINDOW_SIZE instead of just SIZE to avoid possible future confusion with SIZE struct.
        public enum WINDOW_SIZE
        {
            RESTORED = 0,
            MINIMIZED = 1,
            MAXIMIZED = 2,
            MAXSHOW = 3,
            MAXHIDE = 4
        }
    }
}
