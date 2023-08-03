// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using Windows.Win32.System.Com;
using static System.Runtime.InteropServices.ComWrappers;

namespace Windows.Win32.Foundation;

internal readonly unsafe struct ComInterfaceTable
{
    public ComInterfaceEntry* Entries { get; init; }
    public int Count { get; init; }

    /// <summary>
    ///  Create an interface table for the given interface.
    /// </summary>
    public static ComInterfaceTable Create<TComInterface>()
        where TComInterface : unmanaged, IComIID, IVTable
    {
        Span<ComInterfaceEntry> entries = AllocateEntries<TComInterface>(1);
        entries[0] = GetEntry<TComInterface>();

        return new()
        {
            Entries = (ComInterfaceEntry*)Unsafe.AsPointer(ref entries[0]),
            Count = entries.Length
        };
    }

    /// <summary>
    ///  Create an interface table for the given interfaces.
    /// </summary>
    public static ComInterfaceTable Create<TComInterface1, TComInterface2>()
        where TComInterface1 : unmanaged, IComIID, IVTable
        where TComInterface2 : unmanaged, IComIID, IVTable
    {
        Span<ComInterfaceEntry> entries = AllocateEntries<TComInterface1>(2);
        entries[0] = GetEntry<TComInterface1>();
        entries[1] = GetEntry<TComInterface2>();

        return new()
        {
            Entries = (ComInterfaceEntry*)Unsafe.AsPointer(ref entries[0]),
            Count = entries.Length
        };
    }

    /// <summary>
    ///  Create an interface table for the given interfaces.
    /// </summary>
    public static ComInterfaceTable Create<TComInterface1, TComInterface2, TComInterface3>()
        where TComInterface1 : unmanaged, IComIID, IVTable
        where TComInterface2 : unmanaged, IComIID, IVTable
        where TComInterface3 : unmanaged, IComIID, IVTable
    {
        Span<ComInterfaceEntry> entries = AllocateEntries<TComInterface1>(3);
        entries[0] = GetEntry<TComInterface1>();
        entries[1] = GetEntry<TComInterface2>();
        entries[2] = GetEntry<TComInterface3>();

        return new()
        {
            Entries = (ComInterfaceEntry*)Unsafe.AsPointer(ref entries[0]),
            Count = entries.Length
        };
    }

    /// <summary>
    ///  Create an interface table for the given interfaces.
    /// </summary>
    public static ComInterfaceTable Create<TComInterface1, TComInterface2, TComInterface3, TComInterface4>()
        where TComInterface1 : unmanaged, IComIID, IVTable
        where TComInterface2 : unmanaged, IComIID, IVTable
        where TComInterface3 : unmanaged, IComIID, IVTable
        where TComInterface4 : unmanaged, IComIID, IVTable
    {
        Span<ComInterfaceEntry> entries = AllocateEntries<TComInterface1>(4);
        entries[0] = GetEntry<TComInterface1>();
        entries[1] = GetEntry<TComInterface2>();
        entries[2] = GetEntry<TComInterface3>();
        entries[3] = GetEntry<TComInterface4>();

        return new()
        {
            Entries = (ComInterfaceEntry*)Unsafe.AsPointer(ref entries[0]),
            Count = entries.Length
        };
    }

    /// <summary>
    ///  Create an interface table for the given interfaces.
    /// </summary>
    public static ComInterfaceTable Create<TComInterface1, TComInterface2, TComInterface3, TComInterface4, TComInterface5>()
        where TComInterface1 : unmanaged, IComIID, IVTable
        where TComInterface2 : unmanaged, IComIID, IVTable
        where TComInterface3 : unmanaged, IComIID, IVTable
        where TComInterface4 : unmanaged, IComIID, IVTable
        where TComInterface5 : unmanaged, IComIID, IVTable
    {
        Span<ComInterfaceEntry> entries = AllocateEntries<TComInterface1>(5);
        entries[0] = GetEntry<TComInterface1>();
        entries[1] = GetEntry<TComInterface2>();
        entries[2] = GetEntry<TComInterface3>();
        entries[3] = GetEntry<TComInterface4>();
        entries[4] = GetEntry<TComInterface5>();

        return new()
        {
            Entries = (ComInterfaceEntry*)Unsafe.AsPointer(ref entries[0]),
            Count = entries.Length
        };
    }

    /// <summary>
    ///  Create an interface table for the given interfaces.
    /// </summary>
    public static ComInterfaceTable Create<TComInterface1, TComInterface2, TComInterface3, TComInterface4, TComInterface5, TComInterface6>()
        where TComInterface1 : unmanaged, IComIID, IVTable
        where TComInterface2 : unmanaged, IComIID, IVTable
        where TComInterface3 : unmanaged, IComIID, IVTable
        where TComInterface4 : unmanaged, IComIID, IVTable
        where TComInterface5 : unmanaged, IComIID, IVTable
        where TComInterface6 : unmanaged, IComIID, IVTable
    {
        Span<ComInterfaceEntry> entries = AllocateEntries<TComInterface1>(6);
        entries[0] = GetEntry<TComInterface1>();
        entries[1] = GetEntry<TComInterface2>();
        entries[2] = GetEntry<TComInterface3>();
        entries[3] = GetEntry<TComInterface4>();
        entries[4] = GetEntry<TComInterface5>();
        entries[5] = GetEntry<TComInterface6>();

        return new()
        {
            Entries = (ComInterfaceEntry*)Unsafe.AsPointer(ref entries[0]),
            Count = entries.Length
        };
    }

    /// <summary>
    ///  Create an interface table for the given interfaces.
    /// </summary>
    public static ComInterfaceTable Create<TComInterface1, TComInterface2, TComInterface3, TComInterface4, TComInterface5, TComInterface6, TComInterface7, TComInterface8, TComInterface9, TComInterface10>()
        where TComInterface1 : unmanaged, IComIID, IVTable
        where TComInterface2 : unmanaged, IComIID, IVTable
        where TComInterface3 : unmanaged, IComIID, IVTable
        where TComInterface4 : unmanaged, IComIID, IVTable
        where TComInterface5 : unmanaged, IComIID, IVTable
        where TComInterface6 : unmanaged, IComIID, IVTable
        where TComInterface7 : unmanaged, IComIID, IVTable
        where TComInterface8 : unmanaged, IComIID, IVTable
        where TComInterface9 : unmanaged, IComIID, IVTable
        where TComInterface10 : unmanaged, IComIID, IVTable
    {
        Span<ComInterfaceEntry> entries = AllocateEntries<TComInterface1>(10);
        entries[0] = GetEntry<TComInterface1>();
        entries[1] = GetEntry<TComInterface2>();
        entries[2] = GetEntry<TComInterface3>();
        entries[3] = GetEntry<TComInterface4>();
        entries[4] = GetEntry<TComInterface5>();
        entries[5] = GetEntry<TComInterface6>();
        entries[6] = GetEntry<TComInterface7>();
        entries[7] = GetEntry<TComInterface8>();
        entries[8] = GetEntry<TComInterface9>();
        entries[9] = GetEntry<TComInterface10>();

        return new()
        {
            Entries = (ComInterfaceEntry*)Unsafe.AsPointer(ref entries[0]),
            Count = entries.Length
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Span<ComInterfaceEntry> AllocateEntries<T>(int count)
    {
        Span<ComInterfaceEntry> entries = new(
            (ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(T), sizeof(ComInterfaceEntry) * (count + 1)),
            count + 1);

        // Add our sentinel interface.
        entries[^1] = GetEntry<IComCallableWrapper>();
        return entries;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ComInterfaceEntry GetEntry<TComInterface>() where TComInterface : unmanaged, IComIID, IVTable
        => new()
        {
            Vtable = (nint)TComInterface.VTable,
            IID = *IID.Get<TComInterface>()
        };
}
