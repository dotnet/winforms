// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        [DllImport(Libraries.Shell32, CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern HRESULT SHCreateItemFromParsingName(string pszPath, IntPtr pbc, in Guid riid, out IntPtr ppv);

        public static IShellItem CreateItemFromParsingName(string path)
        {
            Guid shellItemIID = IID.IShellItem;
            HRESULT hr = SHCreateItemFromParsingName(path, IntPtr.Zero, in shellItemIID, out IntPtr ppv);
            if (hr.Failed)
            {
                throw new Win32Exception((int)hr);
            }

            return (IShellItem)WinFormsComWrappers.Instance
                .GetOrCreateObjectForComInstance(ppv, CreateObjectFlags.None);
        }
    }
}
