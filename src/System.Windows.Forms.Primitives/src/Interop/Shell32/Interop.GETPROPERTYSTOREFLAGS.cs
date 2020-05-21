// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Shell32
    {
        [Flags]
        public enum GETPROPERTYSTOREFLAGS : uint
        {
            DEFAULT = 0x00000000,
            HANDLERPROPERTIESONLY = 0x00000001,
            READWRITE = 0x00000002,
            TEMPORARY = 0x00000004,
            FASTPROPERTIESONLY = 0x00000008,
            OPENSLOWITEM = 0x00000010,
            DELAYCREATION = 0x00000020,
            BESTEFFORT = 0x00000040,
            NO_OPLOCK = 0x00000080,
            PREFERQUERYPROPERTIES = 0x00000100,
            EXTRINSICPROPERTIES = 0x00000200,
            EXTRINSICPROPERTIESONLY = 0x00000400,
            VOLATILEPROPERTIES = 0x00000800,
            VOLATILEPROPERTIESONLY = 0x00001000,
            MASK_VALID = 0x00001FFF
        }
    }
}
