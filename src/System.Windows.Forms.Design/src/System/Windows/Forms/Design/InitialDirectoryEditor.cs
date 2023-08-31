// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

/// <summary>
///  Folder editor for choosing the starting directory of the folder browser dialog.
/// </summary>
internal class InitialDirectoryEditor : FolderNameEditor
{
    protected override void InitializeDialog(FolderBrowser folderBrowser)
    {
        folderBrowser.Description = SR.InitialDirectoryEditorLabel;
    }
}
