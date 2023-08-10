// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal class FolderBrowserDialogDesigner : ComponentDesigner
{
    // Overridden to avoid setting the default property ("SelectedPath")
    // to the Site.Name (i.e. folderBrowserDialog1).
    protected override bool SetTextualDefaultProperty => false;
}
