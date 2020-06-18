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
        [Guid("9c860395-97b3-490a-b52a-858cc22af166")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITableProvider
        {
            [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
            object[]? /*IRawElementProviderSimple[]*/ GetRowHeaders();

            [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
            object[]? /*IRawElementProviderSimple[]*/ GetColumnHeaders();

            RowOrColumnMajor RowOrColumnMajor { get; }
        }
    }
}
