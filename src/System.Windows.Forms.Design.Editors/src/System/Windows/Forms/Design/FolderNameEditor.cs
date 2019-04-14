﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Design
{
    /// <summary>>
    /// Provides an editor for choosing a folder from the filesystem.
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
        /// Retrieves the editing style of the Edit method.
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        /// Initializes the folder browser dialog when it is created. This gives you an opportunity
        /// to configure the dialog as you please. The default implementation provides a generic folder browser.
        /// </summary>
        protected virtual void InitializeDialog(FolderBrowser folderBrowser)
        {
        }

        protected sealed class FolderBrowser : Component
        {
            // Description text to show.
            private string _descriptionText = string.Empty;

            // Folder picked by the user.
            private readonly UnsafeNativeMethods.BrowseInfos _privateOptions =
                UnsafeNativeMethods.BrowseInfos.NewDialogStyle;

            /// <summary>
            /// The styles the folder browser will use when browsing
            /// folders. This should be a combination of flags from
            /// the FolderBrowserStyles enum.
            /// </summary>
            public FolderBrowserStyles Style { get; set; } = FolderBrowserStyles.RestrictToFilesystem;

            /// <summary>
            /// Gets the directory path of the folder the user picked.
            /// </summary>
            public string DirectoryPath { get; private set; } = string.Empty;

            /// <summary>
            /// Gets/sets the start location of the root node.
            /// </summary>
            public FolderBrowserFolder StartLocation { get; set; } = FolderBrowserFolder.Desktop;

            /// <summary>>
            /// Gets or sets a description to show above the folders. Here you can provide instructions for
            /// selecting a folder.
            /// </summary>
            public string Description
            {
                get => _descriptionText;
                set => _descriptionText = value ?? string.Empty;
            }

            /// <summary>
            /// Shows the folder browser dialog.
            /// </summary>
            public DialogResult ShowDialog() => ShowDialog(null);

            /// <summary>
            /// Shows the folder browser dialog with the specified owner.
            /// </summary>
            public DialogResult ShowDialog(IWin32Window owner)
            {
                IntPtr pidlRoot = IntPtr.Zero;

                // Get/find an owner HWND for this dialog
                IntPtr hWndOwner;

                if (owner != null)
                {
                    hWndOwner = owner.Handle;
                }
                else
                {
                    hWndOwner = UnsafeNativeMethods.GetActiveWindow();
                }

                // Get the IDL for the specific startLocation
                Interop.Shell32.SHGetSpecialFolderLocation(hWndOwner, (int)StartLocation, ref pidlRoot);
                if (pidlRoot == IntPtr.Zero)
                {
                    return DialogResult.Cancel;
                }

                int mergedOptions = (int)Style | (int)_privateOptions;
                if ((mergedOptions & (int)UnsafeNativeMethods.BrowseInfos.NewDialogStyle) != 0)
                {
                    Application.OleRequired();
                }

                IntPtr pidlRet = IntPtr.Zero;
                IntPtr pszDisplayName = IntPtr.Zero;
                IntPtr pszSelectedPath = IntPtr.Zero;

                try
                {
                    pszDisplayName = Marshal.AllocHGlobal(Interop.Kernel32.MAX_PATH * sizeof(char));
                    pszSelectedPath = Marshal.AllocHGlobal((Interop.Kernel32.MAX_PATH + 1) * sizeof(char));

                    var bi = new Interop.Shell32.BROWSEINFO();
                    bi.pidlRoot = pidlRoot;
                    bi.hwndOwner = hWndOwner;
                    bi.pszDisplayName = pszDisplayName;
                    bi.lpszTitle = _descriptionText;
                    bi.ulFlags = mergedOptions;
                    bi.lpfn = null;
                    bi.lParam = IntPtr.Zero;
                    bi.iImage = 0;

                    // And show the dialog
                    pidlRet = Interop.Shell32.SHBrowseForFolder(bi);
                    if (pidlRet == IntPtr.Zero)
                    {
                        return DialogResult.Cancel;
                    }

                    // Then retrieve the path from the IDList
                    Interop.Shell32.SHGetPathFromIDListLongPath(pidlRet, ref pszSelectedPath);

                    // Convert to a string
                    DirectoryPath = Marshal.PtrToStringAuto(pszSelectedPath);
                }
                finally
                {
                    Marshal.FreeCoTaskMem(pidlRoot);
                    Marshal.FreeCoTaskMem(pidlRet);

                    // Then free all the stuff we've allocated or the SH API gave us
                    Marshal.FreeHGlobal(pszSelectedPath);
                    Marshal.FreeHGlobal(pszDisplayName);
                }

                return DialogResult.OK;
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
            BrowseForComputer = UnsafeNativeMethods.BrowseInfos.BrowseForComputer,

            BrowseForEverything = UnsafeNativeMethods.BrowseInfos.BrowseForEverything,

            BrowseForPrinter = UnsafeNativeMethods.BrowseInfos.BrowseForPrinter,

            RestrictToDomain = UnsafeNativeMethods.BrowseInfos.DontGoBelowDomain,

            RestrictToFilesystem = UnsafeNativeMethods.BrowseInfos.ReturnOnlyFSDirs,

            RestrictToSubfolders = UnsafeNativeMethods.BrowseInfos.ReturnFSAncestors,

            ShowTextBox = UnsafeNativeMethods.BrowseInfos.EditBox
        }
    }
}
