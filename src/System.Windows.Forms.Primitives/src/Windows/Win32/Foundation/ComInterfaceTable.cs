// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.ComWrappers;

namespace Windows.Win32.Foundation;

internal unsafe struct ComInterfaceTable
{
    public ComInterfaceEntry* Entries { get; init; }
    public int Count { get; init; }

    public static ComInterfaceTable Create<TComInterface>()
        where TComInterface : unmanaged, IComIID, IVTable
    {
        ComInterfaceEntry* entries =
            (ComInterfaceEntry*)(void*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(TComInterface), sizeof(ComInterfaceEntry));

        entries[0] = new()
        {
            IID = *IID.Get<TComInterface>(),
            Vtable = (nint)(void*)TComInterface.GetVTable()
        };

        return new ComInterfaceTable()
        {
            Entries = entries,
            Count = 1
        };
    }

    public static ComInterfaceTable Create<TComInterface1, TComInterface2>()
        where TComInterface1 : unmanaged, IComIID, IVTable
        where TComInterface2 : unmanaged, IComIID, IVTable
    {
        ComInterfaceEntry* entries =
            (ComInterfaceEntry*)(void*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(TComInterface1), sizeof(ComInterfaceEntry) * 2);

        entries[0] = new()
        {
            IID = *IID.Get<TComInterface1>(),
            Vtable = (nint)(void*)TComInterface1.GetVTable()
        };

        entries[1] = new()
        {
            IID = *IID.Get<TComInterface2>(),
            Vtable = (nint)(void*)TComInterface2.GetVTable()
        };

        return new ComInterfaceTable()
        {
            Entries = entries,
            Count = 2
        };
    }
}
