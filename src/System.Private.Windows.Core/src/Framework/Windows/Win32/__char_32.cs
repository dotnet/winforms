// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal partial struct __char_32
{
    [UnscopedRef]
    internal unsafe Span<char> AsSpan()
    {
        fixed (char* p = &Value[0])
        {
            return new(p, SpanLength);
        }
    }
}
