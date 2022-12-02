// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32.System.Com;

/// <summary>
///  An interface that provides a COM callable wrapper for the implementing class.
///  The implementing class should not be public and unsealed as it can be derived from
///  and COM interfaces can be added. This is meant to be a fixed set of interfaces.
/// </summary>
internal unsafe interface IManagedWrapper
{
    ComInterfaceTable GetComInterfaceTable();
}

/// <summary>
///  Apply to a class to apply a COM callable wrapper of the given <typeparamref name="TComInterface"/>. The class
///  must also derive from the given COM wrapper struct's nested Interface.
/// </summary>
internal unsafe interface IManagedWrapper<TComInterface> : IManagedWrapper
    where TComInterface : unmanaged, IVTable, IComIID
{
    private static ComInterfaceTable InterfaceTable { get; set; } = ComInterfaceTable.Create<TComInterface>();

    ComInterfaceTable IManagedWrapper.GetComInterfaceTable() => InterfaceTable;
}

/// <summary>
///  Apply to a class to apply a COM callable wrapper of the given <typeparamref name="TComInterface1"/> and <typeparamref name="TComInterface2"/>.
///  The class must also derive from both of the given COM wrapper struct's nested Interface.
/// </summary>
internal unsafe interface IManagedWrapper<TComInterface1, TComInterface2> : IManagedWrapper
    where TComInterface1 : unmanaged, IVTable, IComIID
    where TComInterface2 : unmanaged, IVTable, IComIID
{
    private static ComInterfaceTable InterfaceTable { get; set; } = ComInterfaceTable.Create<TComInterface1, TComInterface2>();

    ComInterfaceTable IManagedWrapper.GetComInterfaceTable() => InterfaceTable;
}
