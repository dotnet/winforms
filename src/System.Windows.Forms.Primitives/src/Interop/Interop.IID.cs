﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal static class IID
    {
        // 618736E0-3C3D-11CF-810C-00AA00389B71
        public static Guid IAccessible = new Guid(0x618736E0, 0x3C3D, 0x11CF, 0x81, 0x0C, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

        // E6FDD21A-163F-4975-9C8C-A69F1BA37034
        internal static Guid IFileDialogCustomize { get; } = new(0xE6FDD21A, 0x163F, 0x4975, 0x9C, 0x8C, 0xA6, 0x9F, 0x1B, 0xA3, 0x70, 0x34);

        // 973510DB-7D7F-452B-8975-74A85828D354
        internal static Guid IFileDialogEvents { get; } = new(0x973510DB, 0x7D7F, 0x452B, 0x89, 0x75, 0x74, 0xA8, 0x58, 0x28, 0xD3, 0x54);

        // D57C7288-D4AD-4768-BE02-9D969532D960
        internal static Guid IFileOpenDialog { get; } = new(0xD57C7288, 0xD4AD, 0x4768, 0xBE, 0x02, 0x9D, 0x96, 0x95, 0x32, 0xD9, 0x60);

        // 84BCCD23-5FDE-4CDB-AEA4-AF64B83D78AB
        internal static Guid IFileSaveDialog { get; } = new(0x84BCCD23, 0x5FDE, 0x4CDB, 0xAE, 0xA4, 0xAF, 0x64, 0xB8, 0x3D, 0x78, 0xAB);

        // 00000109-0000-0000-C000-000000000046
        public static Guid IPersistStream { get; } = new(0x00000109, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        // 7BF80980-BF32-101A-8BBB-00AA00300CAB
        public static Guid IPicture { get; } = new(0x7BF80980, 0xBF32, 0x101A, 0x8B, 0xBB, 0x00, 0xAA, 0x00, 0x30, 0x0C, 0xAB);

        // 43826D1E-E718-42EE-BC55-A1E261C37BFE
        internal static Guid IShellItem { get; } = new(0x43826D1E, 0xE718, 0x42EE, 0xBC, 0x55, 0xA1, 0xE2, 0x61, 0xC3, 0x7B, 0xFE);

        // B63EA76D-1F85-456F-A19C-48159EFA858B
        internal static Guid IShellItemArray { get; } = new(0xB63EA76D, 0x1F85, 0x456F, 0xA1, 0x9C, 0x48, 0x15, 0x9E, 0xFA, 0x85, 0x8B);

        // 0000000C-0000-0000-C000-000000000046
        public static Guid IStream { get; } = new(0x0000000C, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);
    }
}
