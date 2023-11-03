// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("E44C3566-915D-4070-99C6-047BFF5A08F5")]
        public unsafe interface ILegacyIAccessibleProvider
        {
            void Select(int flagsSelect);

            void DoDefaultAction();

            void SetValue([MarshalAs(UnmanagedType.LPWStr)] string szValue);

            [PreserveSig]
            HRESULT GetIAccessible(Windows.Win32.UI.Accessibility.IAccessible** ppAccessible);

            int ChildId { get; }

            string? Name { get; }

            string? Value { get; }

            string? Description { get; }

            uint Role { get; }

            uint State { get; }

            string? Help { get; }

            string? KeyboardShortcut { get; }

            [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
            Windows.Win32.UI.Accessibility.IRawElementProviderSimple.Interface[] GetSelection();

            string? DefaultAction { get; }
        }
    }
}
