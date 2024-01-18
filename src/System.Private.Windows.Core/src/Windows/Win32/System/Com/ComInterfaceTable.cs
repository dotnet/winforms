// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.ComWrappers;

namespace Windows.Win32.System.Com;

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

    /// <summary>
    ///  Create an interface table for the given interfaces.
    /// </summary>
    public static ComInterfaceTable Create<TComInterface1, TComInterface2, TComInterface3, TComInterface4, TComInterface5, TComInterface6, TComInterface7, TComInterface8, TComInterface9, TComInterface10,
        TComInterface11, TComInterface12, TComInterface13, TComInterface14, TComInterface15, TComInterface16, TComInterface17, TComInterface18, TComInterface19, TComInterface20,
        TComInterface21, TComInterface22, TComInterface23, TComInterface24, TComInterface25, TComInterface26, TComInterface27>()
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
        where TComInterface11 : unmanaged, IComIID, IVTable
        where TComInterface12 : unmanaged, IComIID, IVTable
        where TComInterface13 : unmanaged, IComIID, IVTable
        where TComInterface14 : unmanaged, IComIID, IVTable
        where TComInterface15 : unmanaged, IComIID, IVTable
        where TComInterface16 : unmanaged, IComIID, IVTable
        where TComInterface17 : unmanaged, IComIID, IVTable
        where TComInterface18 : unmanaged, IComIID, IVTable
        where TComInterface19 : unmanaged, IComIID, IVTable
        where TComInterface20 : unmanaged, IComIID, IVTable
        where TComInterface21 : unmanaged, IComIID, IVTable
        where TComInterface22 : unmanaged, IComIID, IVTable
        where TComInterface23 : unmanaged, IComIID, IVTable
        where TComInterface24 : unmanaged, IComIID, IVTable
        where TComInterface25 : unmanaged, IComIID, IVTable
        where TComInterface26 : unmanaged, IComIID, IVTable
        where TComInterface27 : unmanaged, IComIID, IVTable
    {
        Span<ComInterfaceEntry> entries = AllocateEntries<TComInterface1>(27);
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
        entries[10] = GetEntry<TComInterface11>();
        entries[11] = GetEntry<TComInterface12>();
        entries[12] = GetEntry<TComInterface13>();
        entries[13] = GetEntry<TComInterface14>();
        entries[14] = GetEntry<TComInterface15>();
        entries[15] = GetEntry<TComInterface16>();
        entries[16] = GetEntry<TComInterface17>();
        entries[17] = GetEntry<TComInterface18>();
        entries[18] = GetEntry<TComInterface19>();
        entries[19] = GetEntry<TComInterface20>();
        entries[20] = GetEntry<TComInterface21>();
        entries[21] = GetEntry<TComInterface22>();
        entries[22] = GetEntry<TComInterface23>();
        entries[23] = GetEntry<TComInterface24>();
        entries[24] = GetEntry<TComInterface25>();
        entries[25] = GetEntry<TComInterface26>();
        entries[26] = GetEntry<TComInterface27>();

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
