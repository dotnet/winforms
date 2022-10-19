// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32.UI.Shell;

internal interface IPopulateVTable<TVTable>
    where TVTable : unmanaged
{
    protected internal static abstract unsafe void PopulateVTable(TVTable* vtable);
}
