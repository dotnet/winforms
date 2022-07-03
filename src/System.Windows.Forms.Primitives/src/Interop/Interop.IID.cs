﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal static class IID
    {
        // 618736E0-3C3D-11CF-810C-00AA00389B71
        public static Guid IAccessible { get; } = new(0x618736E0, 0x3C3D, 0x11CF, 0x81, 0x0C, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

        // EAC04BC0-3791-11D2-BB95-0060977B464C
        public static Guid IAutoComplete2 { get; } = new(0xEAC04BC0, 0x3791, 0x11D2, 0xBB, 0x95, 0x00, 0x60, 0x97, 0x7B, 0x46, 0x4C);

        // 0000010E-0000-0000-C000-000000000046
        public static Guid IDataObject { get; } = new(0x0000010E, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        // 00020400-0000-0000-C000-000000000046
        public static Guid IDispatch { get; } = new(0x00020400, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        // 00000121-0000-0000-C000-000000000046
        public static Guid IDropSource { get; } = new(0x00000121, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        // 00000122-0000-0000-C000-000000000046
        public static Guid IDropTarget { get; } = new(0x00000122, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        // 00000101-0000-0000-C000-000000000046
        public static Guid IEnumString { get; } = new(0x00000101, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        // 00000103-0000-0000-C000-000000000046
        public static Guid IEnumFORMATETC { get; } = new(0x00000103, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        // 00000105-0000-0000-C000-000000000046
        public static Guid IEnumSTATDATA { get; } = new(0x00000105, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        // 00020404-0000-0000-C000-000000000046
        public static Guid IEnumVariant { get; } = new(0x00020404, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        // 1CF2B120-547D-101B-8E65-08002B2BD119
        public static Guid IErrorInfo { get; } = new(0x1CF2B120, 0x547D, 0x101B, 0x8E, 0x65, 0x08, 0x00, 0x2B, 0x2B, 0xD1, 0x19);

        // E6FDD21A-163F-4975-9C8C-A69F1BA37034
        internal static Guid IFileDialogCustomize { get; } = new(0xE6FDD21A, 0x163F, 0x4975, 0x9C, 0x8C, 0xA6, 0x9F, 0x1B, 0xA3, 0x70, 0x34);

        // 973510DB-7D7F-452B-8975-74A85828D354
        internal static Guid IFileDialogEvents { get; } = new(0x973510DB, 0x7D7F, 0x452B, 0x89, 0x75, 0x74, 0xA8, 0x58, 0x28, 0xD3, 0x54);

        // D57C7288-D4AD-4768-BE02-9D969532D960
        internal static Guid IFileOpenDialog { get; } = new(0xD57C7288, 0xD4AD, 0x4768, 0xBE, 0x02, 0x9D, 0x96, 0x95, 0x32, 0xD9, 0x60);

        // 84BCCD23-5FDE-4CDB-AEA4-AF64B83D78AB
        internal static Guid IFileSaveDialog { get; } = new(0x84BCCD23, 0x5FDE, 0x4CDB, 0xAE, 0xA4, 0xAF, 0x64, 0xB8, 0x3D, 0x78, 0xAB);

        // E44C3566-915D-4070-99C6-047BFF5A08F5
        internal static Guid ILegacyIAccessibleProvider { get; } = new(0xE44C3566, 0x915D, 0x4070, 0x99, 0xC6, 0x04, 0x7B, 0xFF, 0x5A, 0x08, 0xF5);

        // 0000000A-0000-0000-C000-000000000046
        internal static Guid ILockBytes { get; } = new(0x0000000A, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        // 00000114-0000-0000-C000-000000000046
        internal static Guid IOleWindow { get; } = new(0x00000114, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        // 00000109-0000-0000-C000-000000000046
        public static Guid IPersistStream { get; } = new(0x00000109, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        // 7BF80980-BF32-101A-8BBB-00AA00300CAB
        public static Guid IPicture { get; } = new(0x7BF80980, 0xBF32, 0x101A, 0x8B, 0xBB, 0x00, 0xAA, 0x00, 0x30, 0x0C, 0xAB);

        // D6DD68D1-86FD-4332-8666-9ABEDEA2D24C
        public static Guid IRawElementProviderSimple { get; } = new(0xD6DD68D1, 0x86FD, 0x4332, 0x86, 0x66, 0x9A, 0xBE, 0xDE, 0xA2, 0xD2, 0x4C);

        // F7063DA8-8359-439C-9297-BBC5299A7D87
        public static Guid IRawElementProviderFragment { get; } = new(0xF7063DA8, 0x8359, 0x439C, 0x92, 0x97, 0xBB, 0xC5, 0x29, 0x9A, 0x7D, 0x87);

        // 620CE2A5-AB8F-40A9-86CB-DE3C75599B58
        public static Guid IRawElementProviderFragmentRoot { get; } = new(0x620CE2A5, 0xAB8F, 0x40A9, 0x86, 0xCB, 0xDE, 0x3C, 0x75, 0x59, 0x9B, 0x58);

        // 43826D1E-E718-42EE-BC55-A1E261C37BFE
        internal static Guid IShellItem { get; } = new(0x43826D1E, 0xE718, 0x42EE, 0xBC, 0x55, 0xA1, 0xE2, 0x61, 0xC3, 0x7B, 0xFE);

        // B63EA76D-1F85-456F-A19C-48159EFA858B
        internal static Guid IShellItemArray { get; } = new(0xB63EA76D, 0x1F85, 0x456F, 0xA1, 0x9C, 0x48, 0x15, 0x9E, 0xFA, 0x85, 0x8B);

        // 0000000C-0000-0000-C000-000000000046
        public static Guid IStream { get; } = new(0x0000000C, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);
    }
}
