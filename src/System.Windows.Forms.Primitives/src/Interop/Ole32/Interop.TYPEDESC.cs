// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        public struct TYPEDESC
        {
            public TYPEDESCUNION union;
            public VARENUM vt;

            [StructLayout(LayoutKind.Explicit)]
            public unsafe struct TYPEDESCUNION
            {
                [FieldOffset(0)]
                public TYPEDESC* lptdesc;

                [FieldOffset(0)]
                public IntPtr lpadesc;

                [FieldOffset(0)]
                public uint hreftype;
            }
        }
    }
}
