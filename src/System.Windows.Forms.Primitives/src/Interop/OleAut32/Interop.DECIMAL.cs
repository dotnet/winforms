// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Oleaut32
    {
        public struct DECIMAL
        {
            public ushort wReserved;
            public byte scale;
            public byte sign;
            public uint Hi32;
            public uint Lo32;
            public uint Mid32;

            public decimal ToDecimal()
            {
                return new decimal((int)Lo32, (int)Mid32, (int)Hi32, sign == 0x80, scale);
            }
        }
    }
}
