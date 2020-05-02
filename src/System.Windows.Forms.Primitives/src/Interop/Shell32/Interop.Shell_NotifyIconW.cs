﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        [DllImport(Libraries.Shell32, CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern BOOL Shell_NotifyIconW(NIM dwMessage, ref NOTIFYICONDATAW lpData);
    }
}
