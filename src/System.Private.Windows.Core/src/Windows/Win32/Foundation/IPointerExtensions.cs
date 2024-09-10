// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Foundation;
internal static unsafe class IPointerExtensions
{
    public static T* GetPointer<T>(this IPointer pointer) where T : unmanaged => (T*)pointer.Pointer;
}
