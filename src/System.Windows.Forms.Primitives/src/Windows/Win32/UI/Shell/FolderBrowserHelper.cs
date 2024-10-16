// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Windows.Forms.Primitives.Resources;

namespace Windows.Win32.UI.Shell;

internal static class FolderBrowserHelper
{
    /// <summary>
    ///  Helper for the legacy <see cref="PInvoke.SHGetPathFromIDListEx(ITEMIDLIST*, PWSTR, uint, GPFIDL_FLAGS)" /> API.
    /// </summary>
    /// <returns>The selected path if successful, <see langword="null"/> if failed.</returns>
    /// <exception cref="InvalidOperationException">Could not get the root folder.</exception>
    internal static unsafe string? BrowseForFolder(
        string title,
        int rootFolderCsidl,
        uint flags,
        HWND owner,
        delegate* unmanaged[Stdcall]<HWND, uint, LPARAM, LPARAM, int> callback,
        LPARAM lParam)
    {
        PInvoke.SHGetSpecialFolderLocation(rootFolderCsidl, out ITEMIDLIST* rootFolderId);
        if (rootFolderId is null)
        {
            PInvoke.SHGetSpecialFolderLocation((int)Environment.SpecialFolder.Desktop, out rootFolderId);
            if (rootFolderId is null)
            {
                throw new InvalidOperationException(SR.FolderBrowserDialogNoRootFolder);
            }
        }

        using BufferScope<char> buffer = new((int)PInvokeCore.MAX_PATH + 1);

        fixed (char* b = buffer)
        fixed (char* t = title)
        {
            BROWSEINFOW bi = new()
            {
                pidlRoot = rootFolderId,
                hwndOwner = owner,
                // This is assumed to be MAX_PATH. We don't use it, but we need to have the buffer available.
                pszDisplayName = b,
                lpszTitle = t,
                ulFlags = flags,
                lpfn = callback,
                lParam = lParam,
                iImage = 0
            };

            // Show the dialog
            ITEMIDLIST* resultId = PInvoke.SHBrowseForFolder(&bi);
            Marshal.FreeCoTaskMem((nint)rootFolderId);

            if (resultId is null)
            {
                return null;
            }

            // Retrieve the path from the IDList (GPFIDL_UNCPRINTER is what gets passed by SHGETPathFromWidList).
            // In theory this will handle long paths, but the underlying HRESULT is lost and there is no other
            // immediately apparent way to get the result. Could potentially retry a few times to catch the most common cases.
            bool success = PInvoke.SHGetPathFromIDListEx(resultId, b, (uint)buffer.Length, GPFIDL_FLAGS.GPFIDL_UNCPRINTER);
            Marshal.FreeCoTaskMem((nint)resultId);

            return success ? new(b) : null;
        }
    }
}
