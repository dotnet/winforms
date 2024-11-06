// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

public partial class FolderNameEditor
{
    [Flags]
    protected enum FolderBrowserStyles
    {
        BrowseForComputer = (int)PInvoke.BIF_BROWSEFORCOMPUTER,
        BrowseForEverything = (int)PInvoke.BIF_BROWSEINCLUDEFILES,
        BrowseForPrinter = (int)PInvoke.BIF_BROWSEFORPRINTER,
        RestrictToDomain = (int)PInvoke.BIF_DONTGOBELOWDOMAIN,
        RestrictToFilesystem = (int)PInvoke.BIF_RETURNONLYFSDIRS,
        RestrictToSubfolders = (int)PInvoke.BIF_RETURNFSANCESTORS,
        ShowTextBox = (int)PInvoke.BIF_EDITBOX
    }
}
