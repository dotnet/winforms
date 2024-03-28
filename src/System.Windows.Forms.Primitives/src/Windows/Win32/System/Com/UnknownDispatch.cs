// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.System.Com;

/// <summary>
///  Base <see cref="IDispatch"/> class for <see cref="IUnknown"/>.
/// </summary>
internal abstract unsafe class UnknownDispatch : StandardDispatch<IUnknown>
{
    // StdOle32.tlb
    private static readonly Guid s_stdole = new("00020430-0000-0000-C000-000000000046");

    // We don't release the ITypeInfo to avoid unloading and reloading the standard OLE ITypeLib.
    private static ITypeInfo* TypeInfo { get; } = ComHelpers.GetRegisteredTypeInfo(s_stdole, 2, 0, IUnknown.IID_Guid);

    public UnknownDispatch() : base(TypeInfo) { }
}
