// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Kernel32
    {
        [DllImport(Libraries.Kernel32, ExactSpelling = true)]
        public static extern IntPtr DuplicateHandle(
            IntPtr hSourceProcessHandle,
            IntPtr hSourceHandle,
            IntPtr hTargetProcessHandle,
            ref IntPtr lpTargetHandle,
            uint dwDesiredAccess = 0,
            BOOL bInheritHandle = BOOL.FALSE,
            DuplicateOptions dwOptions = DuplicateOptions.DUPLICATE_SAME_ACCESS);

        [Flags]
        public enum DuplicateOptions
        {
            DUPLICATE_CLOSE_SOURCE = 0x00000001,
            DUPLICATE_SAME_ACCESS = 0x00000002
        }
    }
}
