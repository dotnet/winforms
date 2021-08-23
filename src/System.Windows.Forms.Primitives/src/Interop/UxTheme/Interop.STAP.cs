﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    public static partial class UxTheme
    {
        [Flags]
        public enum STAP : uint
        {
            ALLOW_NONCLIENT = (1 << 0),
            ALLOW_CONTROLS = (1 << 1),
            ALLOW_WEBCONTENT = (1 << 2),
        }
    }
}
