// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.Gdi;

internal partial struct LOGFONTW
{
    public const int LF_FACESIZE = 32;

    // LOGFONTW has space for 32 characters, we use this to ensure we cut to 31 to make room for the null.
    // We should never reference lfFaceName directly and use this property instead.
    public ReadOnlySpan<char> FaceName
    {
        [UnscopedRef]
        get => lfFaceName.AsSpan().SliceAtFirstNull();
        set => SpanHelpers.CopyAndTerminate(value, lfFaceName.AsSpan());
    }
}
