// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Windows.Win32.UI.Shell.Common;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        [DllImport(Libraries.Shell32, ExactSpelling = true)]
        public static extern HRESULT SHCreateShellItem(IntPtr pidlParent, IntPtr psfParent, IntPtr pidl, out IntPtr ppsi);

        public unsafe static IShellItem GetShellItemForPath(string path)
        {
            if (PInvoke.SHParseDisplayName(path, null, out ITEMIDLIST* ppidl, 0, null).Succeeded)
            {
                // No parent specified
                if (SHCreateShellItem(IntPtr.Zero, IntPtr.Zero, (nint)ppidl, out IntPtr ret).Succeeded())
                {
                    var obj = WinFormsComWrappers.Instance
                        .GetOrCreateObjectForComInstance(ret, CreateObjectFlags.None);
                    return (IShellItem)obj;
                }
            }

            throw new FileNotFoundException();
        }
    }
}
