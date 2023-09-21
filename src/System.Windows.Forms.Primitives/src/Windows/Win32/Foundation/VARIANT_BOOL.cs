// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Foundation;

internal readonly partial struct VARIANT_BOOL
{
    public static implicit operator bool(VARIANT_BOOL value) => value != VARIANT_FALSE;

    public static implicit operator VARIANT_BOOL(bool value) => value ? VARIANT_TRUE : VARIANT_FALSE;
}
