// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design
{
    public partial class FolderNameEditor
    {
        [Flags]
        protected enum FolderBrowserStyles
        {
            BrowseForComputer = unchecked((int)PInvoke.BIF_BROWSEFORCOMPUTER),
            BrowseForEverything = unchecked((int)0x00004000),
            BrowseForPrinter = unchecked((int)PInvoke.BIF_BROWSEFORPRINTER),
            RestrictToDomain = unchecked((int)PInvoke.BIF_DONTGOBELOWDOMAIN),
            RestrictToFilesystem = unchecked((int)PInvoke.BIF_RETURNONLYFSDIRS),
            RestrictToSubfolders = unchecked((int)PInvoke.BIF_RETURNFSANCESTORS),
            ShowTextBox = unchecked((int)PInvoke.BIF_EDITBOX)
        }
    }
}
