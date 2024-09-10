// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Foundation;

internal static unsafe class PointerExtensions
{
#pragma warning disable CS0618 // Type or member is obsolete
    public static T* GetPointer<T>(this IPointer<T> pointer) where T : unmanaged => (T*)pointer.Pointer;
#pragma warning restore CS0618
}
