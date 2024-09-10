﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Foundation;

/// <summary>
///  Used to indicate ownership of a native resource pointer.
/// </summary>
/// <remarks>
///  <para>
///   This should never be put on a struct.
///  </para>
/// </remarks>
internal unsafe interface IPointer<TPointer> where TPointer : unmanaged
{
    [Obsolete("Use extension method GetPointer on IPointer<T> to get the typed pointer instead.")]
    nint Pointer { get; }
}
