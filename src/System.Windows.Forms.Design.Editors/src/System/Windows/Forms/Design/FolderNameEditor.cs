// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using static Interop;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides an editor for choosing a folder from the filesystem.
    /// </summary>
    [CLSCompliant(false)]
    public class FolderNameEditor : UITypeEditor
    {
        private FolderBrowser _folderBrowser;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (_folderBrowser == null)
            {
                _folderBrowser = new FolderBrowser();
                InitializeDialog(_folderBrowser);
            }

            if (_folderBrowser.ShowDialog() == DialogResult.OK)
            {
                return _folderBrowser.DirectoryPath;
            }

            return value;
        }

        /// <summary>
        ///  Retrieves the editing style of the Edit method.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        ///  Initializes the folder browser dialog when it is created. This gives you an opportunity
        ///  to configure the dialog as you please. The default implementation provides a generic folder browser.
        /// </summary>
        protected virtual void InitializeDialog(FolderBrowser folderBrowser)
        {
        }

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
                // Get/find an owner HWND for this dialog
                IntPtr hWndOwner = owner == null ? owner.Handle : UnsafeNativeMethods.GetActiveWindow();

                // Get the IDL for the specific startLocation
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
                            using (CoTaskMemSafeHandle browseHandle = Shell32.SHBrowseForFolderW(ref bi))
                            {
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
                    }
                    finally
                    {
                        ArrayPool<char>.Shared.Return(displayName);
                    }
                }
            }
        }

        protected enum FolderBrowserFolder
        {
            Desktop = 0x0000,

            Favorites = 0x0006,

            MyComputer = 0x0011,

            MyDocuments = 0x0005,

            MyPictures = 0x0027,

            NetAndDialUpConnections = 0x0031,

            NetworkNeighborhood = 0x0012,

            Printers = 0x0004,

            Recent = 0x0008,

            SendTo = 0x0009,

            StartMenu = 0x000b,

            Templates = 0x0015
        }

        [Flags]
        protected enum FolderBrowserStyles
        {
            BrowseForComputer = unchecked((int)Shell32.BrowseInfoFlags.BIF_BROWSEFORCOMPUTER),

            BrowseForEverything = unchecked((int)Shell32.BrowseInfoFlags.BIF_BROWSEFOREVERYTHING),

            BrowseForPrinter = unchecked((int)Shell32.BrowseInfoFlags.BIF_BROWSEFORPRINTER),

            RestrictToDomain = unchecked((int)Shell32.BrowseInfoFlags.BIF_DONTGOBELOWDOMAIN),

            RestrictToFilesystem = unchecked((int)Shell32.BrowseInfoFlags.BIF_RETURNONLYFSDIRS),

            RestrictToSubfolders = unchecked((int)Shell32.BrowseInfoFlags.BIF_RETURNFSANCESTORS),

            ShowTextBox = unchecked((int)Shell32.BrowseInfoFlags.BIF_EDITBOX)
        }
    }
}
