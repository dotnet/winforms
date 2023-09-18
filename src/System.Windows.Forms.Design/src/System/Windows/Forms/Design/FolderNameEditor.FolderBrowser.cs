// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Design;

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
        [AllowNull]
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
        public unsafe DialogResult ShowDialog(IWin32Window? owner)
        {
            uint mergedOptions = (uint)Style | PInvoke.BIF_NEWDIALOGSTYLE;
            if ((mergedOptions & (int)PInvoke.BIF_NEWDIALOGSTYLE) != 0)
            {
                Application.OleRequired();
            }

            string? folder = FolderBrowserHelper.BrowseForFolder(
                _descriptionText,
                (int)StartLocation,
                mergedOptions,
                owner is not null ? (HWND)owner.Handle : PInvoke.GetActiveWindow(),
                callback: null,
                lParam: default);

            if (folder is not null)
            {
                DirectoryPath = folder;
                return DialogResult.OK;
            }

            return DialogResult.Cancel;
        }
    }
}
