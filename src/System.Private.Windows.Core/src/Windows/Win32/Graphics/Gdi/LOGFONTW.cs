// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.Gdi;

internal partial struct LOGFONTW
{
    // We should never reference lfFaceName directly and use this property instead.
    public ReadOnlySpan<char> FaceName
    {
        [UnscopedRef]
        get => lfFaceName.AsSpan().SliceAtFirstNull();
        set => SpanHelpers.CopyAndTerminate(value, lfFaceName.AsSpan());
    }
}
