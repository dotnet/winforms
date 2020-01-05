// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum MWMO : uint
        {
            WAITALL = 0x0001,
            ALERTABLE = 0x0002,
            INPUTAVAILABLE = 0x0004,
        }
    }
}
