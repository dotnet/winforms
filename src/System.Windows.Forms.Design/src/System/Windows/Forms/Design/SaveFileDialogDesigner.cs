// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal class SaveFileDialogDesigner : ComponentDesigner
{
    // Overridden to avoid setting the default property ("FileName")
    // to the Site.Name (i.e. saveFileDialog1).
    protected override bool SetTextualDefaultProperty => false;
}
