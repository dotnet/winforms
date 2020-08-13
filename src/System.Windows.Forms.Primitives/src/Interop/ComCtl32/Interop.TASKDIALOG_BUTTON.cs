// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        // Packing is defined as 1 in CommCtrl.h ("pack(1)").
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct TASKDIALOG_BUTTON
        {
            public int nButtonID;
            public char* pszButtonText;
        }
    }
}
