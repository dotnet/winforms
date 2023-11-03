// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        [ComImport]
        [Guid("d02541f1-fb81-4d64-ae32-f520f8a6dbd1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IGridItemProvider
        {
            int Row { get; }

            int Column { get; }

            int RowSpan { get; }

            int ColumnSpan { get; }

            Windows.Win32.UI.Accessibility.IRawElementProviderSimple.Interface? ContainingGrid { get; }
        }
    }
}
