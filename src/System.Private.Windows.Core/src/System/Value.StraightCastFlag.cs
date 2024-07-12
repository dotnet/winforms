// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace System;

internal readonly partial struct Value
{
    /// <summary>
    ///  <see cref="TypeFlag"/> that handles types that are a simple cast from a <see cref="Union"/>
    ///  to a <typeparamref name="T"/>.
    /// </summary>
    private sealed class StraightCastFlag<T> : TypeFlag<T>
    {
        public static StraightCastFlag<T> Instance { get; } = new();

        public override T To(in Value value) => Unsafe.As<Union, T>(ref Unsafe.AsRef(in value._union));
    }
}
