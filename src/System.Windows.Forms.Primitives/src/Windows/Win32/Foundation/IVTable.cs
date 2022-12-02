// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using Windows.Win32.System.Com;

namespace Windows.Win32.Foundation;

/// <summary>
///  Non generic interface that allows constraining against a COM wrapper type directly. COM structs should
///  implement <see cref="IVTable{TComInterface, TVTable}"/>.
/// </summary>
internal unsafe interface IVTable
{
    static abstract IUnknown.Vtbl* GetVTable();
}

internal unsafe interface IVTable<TComInterface, TVTable> : IVTable
    where TComInterface : IVTable<TComInterface, TVTable>, IComIID
    where TVTable : unmanaged
{
    private static sealed TVTable* VTable { get; set; }

    /// <summary>
    ///  Populate <paramref name="vtable"/> with function pointers specific to the COM Interface.
    /// </summary>
    private protected static abstract void PopulateComInterfaceVTable(TVTable* vtable);

    static IUnknown.Vtbl* IVTable.GetVTable()
    {
        if (VTable is null)
        {
            TVTable* vtable = (TVTable*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(TVTable), sizeof(TVTable));
            Interop.WinFormsComWrappers.PopulateIUnknownVTable((IUnknown.Vtbl*)vtable);
            VTable = vtable;
            TComInterface.PopulateComInterfaceVTable(VTable);
        }

        return (IUnknown.Vtbl*)VTable;
    }
}
