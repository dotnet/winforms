// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Windows.Win32.System.Com;

/// <summary>
///  Windows Forms <see cref="StrategyBasedComWrappers"/> implementation.
/// </summary>
/// <remarks>
///  <para>
///   Deriving from <see cref="StrategyBasedComWrappers"/> allows us to leverage the functionality the runtime
///   has implemented for source generated "RCW"s, including support for <see cref="ComImportAttribute"/> adaption
///   when built-in COM support is available (EnableGeneratedComInterfaceComImportInterop).
///  </para>
///  <para>
///   It isn't immediately clear how we could merge <see cref="WinFormsComWrappers"/> with this as there is no
///   strategy for <see cref="ComWrappers.ComputeVtables(object, CreateComInterfaceFlags, out int)"/>. We rely
///   on <see cref="IManagedWrapper"/> to apply the needed vtable functionality and it doesn't appear that we
///   can apply <see cref="IComExposedDetails"/> without manually implementing (or source generating)
///   <see cref="IComExposedDetails.GetComInterfaceEntries(out int)"/> on our exposed classes.
///  </para>
/// </remarks>
internal unsafe class WinFormsComStrategy : StrategyBasedComWrappers
{
    internal static WinFormsComStrategy Instance { get; } = new();

    protected override IIUnknownStrategy GetOrCreateIUnknownStrategy() => GlobalInterfaceTable.CreateUnknownStrategy();
}
