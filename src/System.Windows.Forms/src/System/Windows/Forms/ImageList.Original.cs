// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms
{
    public sealed partial class ImageList
    {
        /// <summary>
        ///  An image before we add it to the image list, along with a few details about how to add it.
        /// </summary>
        private class Original
        {
            internal object _image;
            internal OriginalOptions _options;
            internal Color _customTransparentColor = Color.Transparent;

            internal int _nImages = 1;

            internal Original(object image, OriginalOptions options) : this(image, options, Color.Transparent)
            {
            }

            internal Original(object image, OriginalOptions options, int nImages) : this(image, options, Color.Transparent)
            {
                _nImages = nImages;
            }

            internal Original(object image, OriginalOptions options, Color customTransparentColor)
            {
                if (!(image is Icon) && !(image is Image))
                {
                    throw new InvalidOperationException(SR.ImageListEntryType);
                }

                _image = image;
                _options = options;
                _customTransparentColor = customTransparentColor;
                if ((options & OriginalOptions.CustomTransparentColor) == 0)
                {
                    Debug.Assert(customTransparentColor.Equals(Color.Transparent), "Specified a custom transparent color then told us to ignore it");
                }
            }
        }
    }
}
