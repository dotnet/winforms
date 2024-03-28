// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Foundation;

internal unsafe partial struct HGLOBAL
{
    public static HGLOBAL Null => default;
    public bool IsNull => Value == null;
}
