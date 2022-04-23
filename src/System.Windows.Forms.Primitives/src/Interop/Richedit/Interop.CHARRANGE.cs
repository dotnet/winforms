// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Richedit
    {
        // All structures from richedit.h is packed at 4
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct CHARRANGE
        {
            public int cpMin;
            public int cpMax;
        }
    }
}
