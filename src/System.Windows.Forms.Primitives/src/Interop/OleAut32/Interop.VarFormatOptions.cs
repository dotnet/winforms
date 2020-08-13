// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal partial class Oleaut32
    {
        [Flags]
        public enum VarFormatOptions : uint
        {
            CALENDAR_HIJRI = 0x008,
            FORMAT_NOSUBSTITUTE = 0x020,
        }
    }
}
