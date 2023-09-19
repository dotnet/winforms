// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Foundation;

internal readonly unsafe partial struct PCWSTR
{
    public bool IsNull => Value is null;
}
