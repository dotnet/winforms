// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

/// <summary>
///  Folder editor for choosing the initial folder of the folder browser dialog.
/// </summary>
internal class SelectedPathEditor : FolderNameEditor
{
    protected override void InitializeDialog(FolderBrowserDialog folderBrowserDialog)
    {
        folderBrowserDialog.UseDescriptionForTitle = true;
        folderBrowserDialog.Description = SR.SelectedPathTitle;
    }
}
