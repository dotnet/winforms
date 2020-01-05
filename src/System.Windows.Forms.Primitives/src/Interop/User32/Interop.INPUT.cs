// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        public struct INPUT
        {
            public INPUTENUM type;
            public INPUTUNION inputUnion;

            // We need to split the field offset out into a union struct to avoid
            // silent problems in 64 bit
            [StructLayout(LayoutKind.Explicit)]
            public struct INPUTUNION
            {
                [FieldOffset(0)]
                public MOUSEINPUT mi;

                [FieldOffset(0)]
                public KEYBDINPUT ki;

                [FieldOffset(0)]
                public HARDWAREINPUT hi;
            }
        }
    }
}
