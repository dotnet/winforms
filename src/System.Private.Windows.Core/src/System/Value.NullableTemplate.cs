// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System;

internal readonly partial struct Value
{
    [StructLayout(LayoutKind.Sequential)]
    private readonly struct NullableTemplate<T> where T : unmanaged
    {
        public readonly bool _hasValue;
        public readonly T _value;

        public NullableTemplate(T value)
        {
            _value = value;
            _hasValue = true;
        }
    }
}
