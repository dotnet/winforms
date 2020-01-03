// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        public enum VARENUM : ushort
        {
            EMPTY = 0x0000,
            NULL = 0x0001,
            I2 = 0x0002,
            I4 = 0x0003,
            R4 = 0x0004,
            R8 = 0x0005,
            CY = 0x0006,
            DATE = 0x0007,
            BSTR = 0x0008,
            DISPATCH = 0x0009,
            ERROR = 0x000A,
            BOOL = 0x000B,
            VARIANT = 0x000C,
            UNKNOWN = 0x000D,
            DECIMAL = 0x000E,
            I1 = 0x0010,
            UI1 = 0x0011,
            UI2 = 0x0012,
            UI4 = 0x0013,
            I8 = 0x0014,
            UI8 = 0x0015,
            INT = 0x0016,
            UINT = 0x0017,
            VOID = 0x0018,
            HRESULT = 0x0019,
            PTR = 0x001A,
            SAFEARRAY = 0x001B,
            CARRAY = 0x001C,
            USERDEFINED = 0x001D,
            LPSTR = 0x001E,
            LPWSTR = 0x001F,
            RECORD = 0x0024,
            INT_PTR = 0x0025,
            UINT_PTR = 0x0026,
            FILETIME = 0x0040,
            BLOB = 0x0041,
            STREAM = 0x0042,
            STORAGE = 0x0043,
            STREAMED_OBJECT = 0x0044,
            STORED_OBJECT = 0x0045,
            BLOB_OBJECT = 0x0046,
            CF = 0x0047,
            CLSID = 0x0048,
            VERSIONED_STREAM = 0x0049,
            BSTR_BLOB = 0x0FFF,
            VECTOR = 0x1000,
            ARRAY = 0x2000,
            BYREF = 0x4000,
            RESERVED = 0x8000,
            ILLEGAL = 0xFFFF,
            ILLEGALMASKED = 0x0FFF,
            TYPEMASK = 0x0FFF
        }
    }
}
