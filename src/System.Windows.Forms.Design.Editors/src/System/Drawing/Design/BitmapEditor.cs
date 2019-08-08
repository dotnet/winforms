// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;

namespace System.Drawing.Design
{
    /// <summary>
    ///  Provides an editor that can perform default file searching for bitmap (.bmp)
    ///  files.
    ///</summary>
    [CLSCompliant(false)]
    public class BitmapEditor : ImageEditor
    {
        protected static List<string> BitmapExtensions = new List<string>() { "bmp", "gif", "jpg", "jpeg", "png", "ico" };

        protected override string GetFileDialogDescription() => SR.bitmapFileDescription;

        protected override string[] GetExtensions() => BitmapExtensions.ToArray();

        protected override Image LoadFromStream(Stream stream) => new Bitmap(stream);
    }
}
