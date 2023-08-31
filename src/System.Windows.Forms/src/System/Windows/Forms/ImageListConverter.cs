// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

internal class ImageListConverter : ComponentConverter
{
    public ImageListConverter() : base(typeof(ImageList))
    {
    }

    /// <summary>
    ///  Gets a value indicating whether this object supports properties using the
    ///  specified context.
    /// </summary>
    public override bool GetPropertiesSupported(ITypeDescriptorContext? context) => true;
}
