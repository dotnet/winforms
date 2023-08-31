// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="SHCreateShellItem(ITEMIDLIST*, IShellFolder*, ITEMIDLIST*, IShellItem**)"/>
    public static unsafe IShellItem* SHCreateShellItem(string path)
    {
        IShellItem* ppsi = default;
        if (SHParseDisplayName(path, pbc: null, out ITEMIDLIST* ppidl, sfgaoIn: 0, psfgaoOut: null).Succeeded)
        {
            // No parent specified
            SHCreateShellItem(pidlParent: null, psfParent: null, ppidl, &ppsi);
        }

        return ppsi;
    }
}
