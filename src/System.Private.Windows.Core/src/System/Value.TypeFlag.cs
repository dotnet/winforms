// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace System;

internal readonly partial struct Value
{
    /// <summary>
    ///  A flag that represents the <see cref="System.Type"/> of a <see cref="Union"/> in a <see cref="Value"/>.
    ///  Also provides the functionality to convert any <see cref="Value"/> to an <see langword="object"/> that
    ///  already isn't an <see langword="object"/>.
    /// </summary>
    private abstract class TypeFlag
    {
        public abstract Type Type
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        /// <summary>
        ///  Converts the given <see cref="Value"/> to an <see langword="object"/>.
        /// </summary>
        public abstract object ToObject(in Value value);
    }

    /// <summary>
    ///  Strongly typed <see cref="TypeFlag"/>.
    /// </summary>
    private abstract class TypeFlag<T> : TypeFlag
    {
        public sealed override Type Type
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => typeof(T);
        }

        public override object ToObject(in Value value) => To(value)!;
        public abstract T To(in Value value);
    }
}
