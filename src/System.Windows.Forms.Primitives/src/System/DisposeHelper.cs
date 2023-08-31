// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using Windows.Win32.System.Com;

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

    /// <summary>
    ///  Sets <paramref name="comPointer"/> to null before releasing it. Useful for guarding against field
    ///  access when releasing the field.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe void NullAndRelease<T>(ref T* comPointer) where T : unmanaged, IComIID
    {
        IUnknown* localComPointer = (IUnknown*)comPointer;
        comPointer = null;
        if (localComPointer is not null)
        {
            localComPointer->Release();
        }
    }
}
