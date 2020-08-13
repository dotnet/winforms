// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum DESKTOP : uint
        {
            READOBJECTS = 0x00000001,
            CREATEWINDOW = 0x00000002,
            CREATEMENU = 0x00000004,
            HOOKCONTROL = 0x00000008,
            JOURNALRECORD = 0x00000010,
            JOURNALPLAYBACK = 0x00000020,
            ENUMERATE = 0x00000040,
            WRITEOBJECTS = 0x00000080,
            SWITCHDESKTOP = 0x00000100,
            DELETE = 0x00010000,
            READ_CONTROL = 0x00020000,
            SYNCHRONIZE = 0x00100000,
            WRITE_DAC = 0x00040000,
            WRITE_OWNER = 0x00080000,
        }
    }
}
