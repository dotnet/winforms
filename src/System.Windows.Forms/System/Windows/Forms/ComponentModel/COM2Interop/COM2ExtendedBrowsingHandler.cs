// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.ComponentModel.Com2Interop;

internal abstract unsafe class Com2ExtendedBrowsingHandler<T> : ICom2ExtendedBrowsingHandler where T : unmanaged, IComIID
{
    public bool ObjectSupportsInterface(object @object) => ComHelpers.SupportsInterface<T>(@object);

    public abstract void RegisterEvents(Com2PropertyDescriptor[]? properties);

    /// <summary>
    ///  Simple <see cref="ComHelpers"/> wrapper for convenience and ensuring the right interface is used.
    /// </summary>
    public static ComScope<T> TryGetComScope(object? @object, out HRESULT hr)
        => ComHelpers.TryGetComScope<T>(@object, out hr);
}
