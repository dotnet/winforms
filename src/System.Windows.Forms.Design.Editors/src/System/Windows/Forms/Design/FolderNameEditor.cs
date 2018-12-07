// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Design
{
    /// <summary>>
    ///         Provides an editor
    ///         for choosing a folder from the filesystem.
    /// </summary>
    [CLSCompliant(false)]
    public class FolderNameEditor : UITypeEditor
    {
        private FolderBrowser folderBrowser;

        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (folderBrowser == null)
            {
                folderBrowser = new FolderBrowser();
                InitializeDialog(folderBrowser);
            }

            if (folderBrowser.ShowDialog() != DialogResult.OK) return value;

            return folderBrowser.DirectoryPath;
        }

        /// <summary>
        ///     Retrieves the editing style of the Edit method.  If the method
        ///     is not supported, this will return None.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        // everything in this assembly is full trust.
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        ///     Initializes the folder browser dialog when it is created.  This gives you
        ///     an opportunity to configure the dialog as you please.  The default
        ///     implementation provides a generic folder browser.
        /// </summary>
        protected virtual void InitializeDialog(FolderBrowser folderBrowser)
        {
        }

        protected sealed class FolderBrowser : Component
        {
            private static readonly int MAX_PATH = 260;

            // Description text to show.
            private string descriptionText = string.Empty;

            // Folder picked by the user.
            private readonly UnsafeNativeMethods.BrowseInfos privateOptions =
                UnsafeNativeMethods.BrowseInfos.NewDialogStyle;

            /// <summary>
            ///     The styles the folder browser will use when browsing
            ///     folders.  This should be a combination of flags from
            ///     the FolderBrowserStyles enum.
            /// </summary>
            public FolderBrowserStyles Style { get; set; } = FolderBrowserStyles.RestrictToFilesystem;

            /// <summary>
            ///     Gets the directory path of the folder the user picked.
            /// </summary>
            public string DirectoryPath { get; private set; } = string.Empty;

            /// <summary>
            ///     Gets/sets the start location of the root node.
            /// </summary>
            public FolderBrowserFolder StartLocation { get; set; } = FolderBrowserFolder.Desktop;

            /// <summary>>
            ///         Gets or sets a description to show above the folders. Here you can provide instructions for
            ///         selecting a folder.
            /// </summary>
            public string Description
            {
                get => descriptionText;
                set => descriptionText = value == null ? string.Empty : value;
            }

            /// <summary>
            ///     Helper function that returns the IMalloc interface used by the shell.
            /// </summary>
            private static UnsafeNativeMethods.IMalloc GetSHMalloc()
            {
                UnsafeNativeMethods.IMalloc[] malloc = new UnsafeNativeMethods.IMalloc[1];

                UnsafeNativeMethods.Shell32.SHGetMalloc(malloc);

                return malloc[0];
            }

            /// <summary>
            ///     Shows the folder browser dialog.
            /// </summary>
            public DialogResult ShowDialog()
            {
                return ShowDialog(null);
            }

            /// <summary>
            ///     Shows the folder browser dialog with the specified owner.
            /// </summary>
            public DialogResult ShowDialog(IWin32Window owner)
            {
                IntPtr pidlRoot = IntPtr.Zero;

                // Get/find an owner HWND for this dialog
                IntPtr hWndOwner;

                if (owner != null)
                    hWndOwner = owner.Handle;
                else
                    hWndOwner = UnsafeNativeMethods.GetActiveWindow();

                // Get the IDL for the specific startLocation
                UnsafeNativeMethods.Shell32.SHGetSpecialFolderLocation(hWndOwner, (int)StartLocation, ref pidlRoot);

                if (pidlRoot == IntPtr.Zero) return DialogResult.Cancel;

                int mergedOptions = (int)Style | (int)privateOptions;

                if ((mergedOptions & (int)UnsafeNativeMethods.BrowseInfos.NewDialogStyle) != 0)
                    Application.OleRequired();

                IntPtr pidlRet = IntPtr.Zero;

                try
                {
                    // Construct a BROWSEINFO
                    UnsafeNativeMethods.BROWSEINFO bi = new UnsafeNativeMethods.BROWSEINFO();

                    IntPtr buffer = Marshal.AllocHGlobal(MAX_PATH);

                    bi.pidlRoot = pidlRoot;
                    bi.hwndOwner = hWndOwner;
                    bi.pszDisplayName = buffer;
                    bi.lpszTitle = descriptionText;
                    bi.ulFlags = mergedOptions;
                    bi.lpfn = IntPtr.Zero;
                    bi.lParam = IntPtr.Zero;
                    bi.iImage = 0;

                    // And show the dialog
                    pidlRet = UnsafeNativeMethods.Shell32.SHBrowseForFolder(bi);

                    if (pidlRet == IntPtr.Zero) return DialogResult.Cancel;

                    // Then retrieve the path from the IDList
                    UnsafeNativeMethods.Shell32.SHGetPathFromIDList(pidlRet, buffer);

                    // Convert to a string
                    DirectoryPath = Marshal.PtrToStringAuto(buffer);

                    // Then free all the stuff we've allocated or the SH API gave us
                    Marshal.FreeHGlobal(buffer);
                }
                finally
                {
                    UnsafeNativeMethods.IMalloc malloc = GetSHMalloc();
                    malloc.Free(pidlRoot);

                    if (pidlRet != IntPtr.Zero) malloc.Free(pidlRet);
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
