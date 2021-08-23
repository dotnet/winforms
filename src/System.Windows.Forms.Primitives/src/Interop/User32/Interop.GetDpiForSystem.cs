﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        // This is only available on Windows 1607 and later. Avoids needing a DC to get the DPI.
        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern uint GetDpiForSystem();
    }
}
