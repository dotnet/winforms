﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct EncoderParametersPrivate
{
    public uint Count;

    // Native definition
    // EncoderParameter Parameter[1];
    private EncoderParameterPrivate _parameter;

#if NET7_0_OR_GREATER
    [UnscopedRef]
#endif
    public Span<EncoderParameterPrivate> Parameters => MemoryMarshal.CreateSpan(ref _parameter, (int)Count);
}
