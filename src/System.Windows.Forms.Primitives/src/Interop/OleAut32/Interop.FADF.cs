// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Oleaut32
    {
        [Flags]
        public enum FADF : ushort
        {
            AUTO = 0x0001,
            STATIC = 0x0002,
            EMBEDDED = 0x0004,
            FIXEDSIZE = 0x0010,
            RECORD = 0x0020,
            HAVEIID = 0x0040,
            HAVEVARTYPE = 0x0080,
            BSTR = 0x0100,
            UNKNOWN = 0x0200,
            DISPATCH = 0x0400,
            VARIANT = 0x0800
        }
    }
}
