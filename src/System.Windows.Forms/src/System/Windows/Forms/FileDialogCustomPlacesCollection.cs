// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.IO;

namespace System.Windows.Forms
{
    public class FileDialogCustomPlacesCollection : Collection<FileDialogCustomPlace>
    {
        internal void Apply(FileDialogNative.IFileDialog dialog)
        {
            // Walk backwards
            for (int i = Items.Count - 1; i >= 0; --i)
            {
                FileDialogCustomPlace customPlace = Items[i];

                try
                {
                    FileDialogNative.IShellItem shellItem = customPlace.GetNativePath();
                    if (null != shellItem)
                    {
                        dialog.AddPlace(shellItem, 0);
                    }
                }
                catch (FileNotFoundException)
                {
                    // Silently absorb FileNotFound exceptions (these could be caused by a
                    // path that disappeared after the place was added to the dialog).
                }
            }
        }

        public void Add(string path) => Add(new FileDialogCustomPlace(path));

        public void Add(Guid knownFolderGuid) => Add(new FileDialogCustomPlace(knownFolderGuid));
    }
}
