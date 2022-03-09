// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal static class CLSID
    {
        // 00BB2763-6A77-11D0-A535-00C04FD7D062
        internal static Guid AutoComplete = new Guid(0x00BB2763, 0x6A77, 0x11D0, 0xA5, 0x35, 0x00, 0xC0, 0x4F, 0xD7, 0xD0, 0x62);

        // C0B4E2F3-BA21-4773-8DBA-335EC946EB8B
        internal static Guid FileSaveDialog = new Guid(0xC0B4E2F3, 0xBA21, 0x4773, 0x8D, 0xBA, 0x33, 0x5E, 0xC9, 0x46, 0xEB, 0x8B);

        // DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7
        internal static Guid FileOpenDialog = new Guid(0xDC1C5A9C, 0xE88A, 0x4DDE, 0xA5, 0xA1, 0x60, 0xF8, 0x2A, 0x20, 0xAE, 0xF7);
    }
}
