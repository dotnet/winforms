// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.Controls;

internal partial struct TVINSERTSTRUCTW
{
    [UnscopedRef]
    public ref TVITEMW item => ref Anonymous.item;
}
