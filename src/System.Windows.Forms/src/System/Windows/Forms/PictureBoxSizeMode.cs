﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies how an image is positioned within a <see cref='System.Windows.Forms.PictureBox'/>.
    /// </devdoc>
    public enum PictureBoxSizeMode
    {
        /// <devdoc>
        /// The image is placed in the top-left corner of the
        /// <see cref='System.Windows.Forms.PictureBox'/>. The image is clipped
        /// if the <see cref='System.Windows.Forms.PictureBox'/> is to small.
        /// </devdoc>
        Normal = 0,

        /// <devdoc>
        /// The image within the <see cref='System.Windows.Forms.PictureBox'/> is
        /// stretched or shrunk to fit the current size of the
        // <see cref='System.Windows.Forms.PictureBox'/>.
        /// </devdoc>
        StretchImage = 1,

        /// <devdoc>
        /// The <see cref='System.Windows.Forms.PictureBox'/> is sized to fit the
        /// size of the image that is displayed.
        /// </devdoc>
        AutoSize = 2,

        /// <devdoc>
        /// The image is displayed in the center if the
        /// <see cref='System.Windows.Forms.PictureBox'/> is larger than the
        /// image. If the image is larger than the <see cref='System.Windows.Forms.PictureBox'/>,
        /// the center of the picture is placed in the center of the
        /// <see cref='System.Windows.Forms.PictureBox'/> and the outside edges are
        /// clipped.
        /// </devdoc>
        CenterImage = 3,

        /// <devdoc>
        /// The size of image is increased or decresed maintaining the aspect ratio.
        /// </devdoc>
        Zoom = 4
    }
}
