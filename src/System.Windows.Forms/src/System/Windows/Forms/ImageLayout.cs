// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the position of the image on the control.
    /// </devdoc>
    public enum ImageLayout
    {
        /// <summary>
        /// The image is aligned TOP - LEFT across the controls client rectangle.
        /// </devdoc>
        None,

        /// <summary>
        /// The image is tiled across the controls client rectangle.
        /// </devdoc>
        Tile,

        /// <summary>
        /// The image is centred within the controls client rectangle.
        /// </devdoc>
        Center,

        /// <summary>
        /// The image is streched across the controls client rectangle.
        /// </devdoc>
        Stretch,

        /// <summary>
        /// The image is streched across the controls client rectangle.
        /// </devdoc>
        Zoom,
    }
}  
