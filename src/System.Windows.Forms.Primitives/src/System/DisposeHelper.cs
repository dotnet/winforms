// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace System;

internal static class DisposeHelper
{
    /// <summary>
    ///  Sets <paramref name="disposable"/> to null before disposing it. Useful for guarding against field
    ///  access when disposing the field.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void NullAndDispose<T>(ref T? disposable) where T : class, IDisposable
    {
        IDisposable? localDisposable = disposable;
        disposable = null;
        localDisposable?.Dispose();
    }
}
