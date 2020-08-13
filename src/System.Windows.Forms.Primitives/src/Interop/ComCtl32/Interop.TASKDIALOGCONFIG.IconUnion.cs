// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public partial struct TASKDIALOGCONFIG
        {
            [StructLayout(LayoutKind.Explicit, Pack = 1)]
            public unsafe struct IconUnion
            {
                [FieldOffset(0)]
                public IntPtr hIcon;

                [FieldOffset(0)]
                public char* pszIcon;
            }
        }
    }
}
