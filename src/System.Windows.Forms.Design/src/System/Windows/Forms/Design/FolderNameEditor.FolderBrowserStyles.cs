// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms.Design
{
    public partial class FolderNameEditor
    {
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
