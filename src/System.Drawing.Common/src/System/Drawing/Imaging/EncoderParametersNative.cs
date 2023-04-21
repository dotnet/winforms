// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct EncoderParametersNative
{
    public uint Count;

    // Native definition
    // EncoderParameter Parameter[1];
    private EncoderParameterNative _parameter;

#if NET7_0_OR_GREATER
    [UnscopedRef]
#endif
    public Span<EncoderParameterNative> Parameters => MemoryMarshal.CreateSpan(ref _parameter, (int)Count);
}
