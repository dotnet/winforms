// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.GdiPlus;

internal partial struct EncoderParameters
{
    [UnscopedRef]
    internal unsafe Span<EncoderParameter> Parameters => Parameter.AsSpan((int)Count);
}
