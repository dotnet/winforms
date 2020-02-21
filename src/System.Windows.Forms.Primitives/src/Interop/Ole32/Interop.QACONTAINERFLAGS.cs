// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [Flags]
        public enum QACONTAINERFLAGS : uint
        {
            SHOWHATCHING = 0x0001,
            SHOWGRABHANDLES = 0x0002,
            USERMODE = 0x0004,
            DISPLAYASDEFAULT= 0x0008,
            UIDEAD = 0x0010,
            AUTOCLIP = 0x0020,
            MESSAGEREFLECT = 0x0040,
            SUPPORTSMNEMONICS = 0x0080,
        }
    }
}
