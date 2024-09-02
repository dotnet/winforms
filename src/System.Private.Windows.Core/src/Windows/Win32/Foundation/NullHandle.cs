// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Foundation;

internal class NullHandle<T> : IHandle<T> where T : unmanaged
{
    private NullHandle() { }

    public static NullHandle<T> Instance { get; } = new();

    public T Handle => default;
}
