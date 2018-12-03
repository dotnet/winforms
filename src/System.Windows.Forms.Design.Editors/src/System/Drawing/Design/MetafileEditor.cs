// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms.Design.Editors.Resources;

namespace System.Drawing.Design
{
    /// <summary>
    ///     Extends Image's editor class to provide default file searching for metafile (.emf)
    ///     files.
    /// </summary>
    [CLSCompliant(false)]
    public class MetafileEditor : ImageEditor
    {
        protected override string GetFileDialogDescription()
        {
            return SR.metafileFileDescription;
        }

        protected override string[] GetExtensions()
        {
            return new string[] { "emf", "wmf" };
        }

        protected override Image LoadFromStream(Stream stream)
        {
            return new Metafile(stream);
        }
    }
}
