// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Windows.Forms.Design.Editors.Resources;

namespace System.Drawing.Design
{
    /// <summary>
    /// Provides an editor that can perform default file searching for bitmap (.bmp)
    /// files.
    ///</summary>
    [CLSCompliant(false)]
    public class BitmapEditor : ImageEditor
    {
        protected override string GetFileDialogDescription()
        {
            return Resource.bitmapFileDescription;
        }

        protected override string[] GetExtensions()
        {
            return new[] { "bmp", "gif", "jpg", "jpeg", "png", "ico" };
        }

        protected override Image LoadFromStream(Stream stream)
        {
            return new Bitmap(stream);
        }
    }
}
