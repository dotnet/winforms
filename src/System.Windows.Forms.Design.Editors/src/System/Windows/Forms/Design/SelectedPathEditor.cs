// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Folder editor for choosing the initial folder of the folder browser dialog.
    /// </summary>
    internal class SelectedPathEditor : FolderNameEditor
    {
        protected override void InitializeDialog(FolderBrowser folderBrowser)
        {
            folderBrowser.Description = SR.SelectedPathEditorLabel;
        }
    }
}
