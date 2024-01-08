// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.Controls;

internal readonly partial struct HTREEITEM
{
    public static explicit operator LPARAM(HTREEITEM value) => (LPARAM)value.Value;
    public static explicit operator HTREEITEM(LPARAM value) => (HTREEITEM)value.Value;
}
