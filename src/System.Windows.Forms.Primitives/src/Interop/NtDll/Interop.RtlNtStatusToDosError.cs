// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class NtDll
    {
        // https://docs.microsoft.com/windows/win32/api/winternl/nf-winternl-rtlntstatustodoserror
        [DllImport(Libraries.NtDll, ExactSpelling = true)]
        public unsafe static extern uint RtlNtStatusToDosError(int Status);
    }
}
