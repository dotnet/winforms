// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Windows.Win32.UI.Shell.Common;
using Shell = Windows.Win32.UI.Shell;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public unsafe static IShellItem SHCreateShellItem(string path)
        {
            if (SHParseDisplayName(path, pbc: null, out ITEMIDLIST* ppidl, 0, psfgaoOut: null).Succeeded)
            {
                // No parent specified
                Shell.IShellItem** ppsi = default;
                if (SHCreateShellItem(pidlParent: null, psfParent: null, ppidl, ppsi).Succeeded)
                {
                    var obj = Interop.WinFormsComWrappers.Instance
                        .GetOrCreateObjectForComInstance((nint)ppsi, CreateObjectFlags.None);
                    return (IShellItem)obj;
                }
            }

            throw new FileNotFoundException();
        }
    }
}
