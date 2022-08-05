// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.ComponentModel;
using Microsoft.Win32.SafeHandles;
using static Interop;

namespace System.Windows.Forms.Design
{
    public partial class FolderNameEditor
    {
        protected sealed class FolderBrowser : Component
        {
            // Description text to show.
            private string _descriptionText = string.Empty;

            /// <summary>
            ///  The styles the folder browser will use when browsing
            ///  folders. This should be a combination of flags from
            ///  the FolderBrowserStyles enum.
            /// </summary>
            public FolderBrowserStyles Style { get; set; } = FolderBrowserStyles.RestrictToFilesystem;

            /// <summary>
            ///  Gets the directory path of the folder the user picked.
            /// </summary>
            public string DirectoryPath { get; private set; } = string.Empty;

            /// <summary>
            ///  Gets/sets the start location of the root node.
            /// </summary>
            public FolderBrowserFolder StartLocation { get; set; } = FolderBrowserFolder.Desktop;

            /// <summary>
            ///  Gets or sets a description to show above the folders. Here you can provide instructions for
            ///  selecting a folder.
            /// </summary>
            public string Description
            {
                get => _descriptionText;
                set => _descriptionText = value ?? string.Empty;
            }

            /// <summary>
            ///  Shows the folder browser dialog.
            /// </summary>
            public DialogResult ShowDialog() => ShowDialog(null);

            /// <summary>
            ///  Shows the folder browser dialog with the specified owner.
            /// </summary>
            public unsafe DialogResult ShowDialog(IWin32Window owner)
            {
                // Get/find an owner HWND for this dialog.
                HWND hWndOwner = owner != null ? (HWND)owner.Handle : PInvoke.GetActiveWindow();

                // Get the IDL for the specific start location.
                Shell32.SHGetSpecialFolderLocation(hWndOwner, (int)StartLocation, out CoTaskMemSafeHandle listHandle);
                if (listHandle.IsInvalid)
                {
                    return DialogResult.Cancel;
                }

                using (listHandle)
                {
                    uint mergedOptions = (uint)Style | Shell32.BrowseInfoFlags.BIF_NEWDIALOGSTYLE;
                    if ((mergedOptions & (int)Shell32.BrowseInfoFlags.BIF_NEWDIALOGSTYLE) != 0)
                    {
                        Application.OleRequired();
                    }

                    char[] displayName = ArrayPool<char>.Shared.Rent(Kernel32.MAX_PATH + 1);
                    try
                    {
                        fixed (char* pDisplayName = displayName)
                        {
                            var bi = new Shell32.BROWSEINFO
                            {
                                pidlRoot = listHandle,
                                hwndOwner = hWndOwner,
                                pszDisplayName = pDisplayName,
                                lpszTitle = _descriptionText,
                                ulFlags = mergedOptions,
                                lpfn = null,
                                lParam = IntPtr.Zero,
                                iImage = 0
                            };

                            // Show the dialog.
                            using CoTaskMemSafeHandle browseHandle = Shell32.SHBrowseForFolderW(ref bi);
                            if (browseHandle.IsInvalid)
                            {
                                return DialogResult.Cancel;
                            }

                            // Retrieve the path from the IDList.
                            Shell32.SHGetPathFromIDListLongPath(browseHandle.DangerousGetHandle(), out string selectedPath);
                            DirectoryPath = selectedPath;
                            return DialogResult.OK;
                        }
                    }
                    finally
                    {
                        ArrayPool<char>.Shared.Return(displayName);
                    }
                }
            }
        }
    }
}
