// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.ObjectModel;

namespace System.Windows.Forms;

public class FileDialogCustomPlacesCollection : Collection<FileDialogCustomPlace>
{
    internal unsafe void Apply(IFileDialog* dialog)
    {
        for (int i = Items.Count - 1; i >= 0; --i)
        {
            FileDialogCustomPlace customPlace = Items[i];
            using ComScope<IShellItem> shellItem = new(customPlace.GetNativePath());
            if (!shellItem.IsNull)
            {
                dialog->AddPlace(shellItem, 0);
            }
        }
    }

    public void Add(string? path) => Add(new FileDialogCustomPlace(path));

    public void Add(Guid knownFolderGuid) => Add(new FileDialogCustomPlace(knownFolderGuid));
}
