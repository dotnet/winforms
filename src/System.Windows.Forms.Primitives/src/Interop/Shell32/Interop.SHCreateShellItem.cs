// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        [DllImport(Libraries.Shell32, ExactSpelling = true)]
        public static extern HRESULT SHCreateShellItem(IntPtr pidlParent, IntPtr psfParent, IntPtr pidl, out IShellItem ppsi);

        [DllImport(Libraries.Shell32, EntryPoint = "SHCreateShellItem", ExactSpelling = true)]
        public static extern HRESULT SHCreateShellItemViaComWrappers(IntPtr pidlParent, IntPtr psfParent, IntPtr pidl, out IntPtr ppsi);

        public static IShellItem GetShellItemForPath(string path)
        {
            if (SHParseDisplayName(path, IntPtr.Zero, out IntPtr pidl, 0, out uint _).Succeeded())
            {
                // No parent specified
                if (SHCreateShellItem(IntPtr.Zero, IntPtr.Zero, pidl, out IShellItem ret).Succeeded())
                {
                    return ret;
                }
            }

            throw new FileNotFoundException();
        }

        public static IShellItem GetShellItemForPathViaComWrappers(string path)
        {
            if (SHParseDisplayName(path, IntPtr.Zero, out IntPtr pidl, 0, out uint _).Succeeded())
            {
                // No parent specified
                if (SHCreateShellItemViaComWrappers(IntPtr.Zero, IntPtr.Zero, pidl, out IntPtr ret).Succeeded())
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
