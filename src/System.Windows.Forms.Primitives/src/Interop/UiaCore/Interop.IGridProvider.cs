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
        [Guid("b17d6187-0907-464b-a168-0ef17a1572b1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IGridProvider
        {
            [return: MarshalAs(UnmanagedType.IUnknown)]
            object? /*IRawElementProviderSimple*/ GetItem(int row, int column);

            int RowCount { get; }

            int ColumnCount { get; }
        }
    }
}
