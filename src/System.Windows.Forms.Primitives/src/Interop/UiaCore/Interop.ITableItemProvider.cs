// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        [ComImport]
        [Guid("b9734fa6-771f-4d78-9c90-2517999349cd")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITableItemProvider
        {
            [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
            object[]? /*IRawElementProviderSimple[]*/ GetRowHeaderItems();

            [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
            object[]? /*IRawElementProviderSimple[]*/ GetColumnHeaderItems();
        }
    }
}
