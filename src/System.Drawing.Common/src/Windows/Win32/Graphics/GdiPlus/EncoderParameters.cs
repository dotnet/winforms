// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace Windows.Win32.Graphics.GdiPlus;

internal partial struct EncoderParameters
{
    [UnscopedRef]
    internal unsafe Span<EncoderParameter> Parameters => MemoryMarshal.CreateSpan(ref Parameter._0, (int)Count);
}
