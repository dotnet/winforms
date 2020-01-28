// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies how an image is positioned within a <see cref='PictureBox'/>.
    /// </summary>
    public enum PictureBoxSizeMode
    {
        /// <summary>
        ///  The image is placed in the top-left corner of the
        /// <see cref='PictureBox'/>. The image is clipped
        ///  if the <see cref='PictureBox'/> is to small.
        /// </summary>
        Normal = 0,

        /// <summary>
        ///  The image within the <see cref='PictureBox'/> is stretched or shrunk to fit the
        ///  current size of the <see cref='System.Windows.Forms.PictureBox'/>.
        /// </summary>
        StretchImage = 1,

        /// <summary>
        ///  The <see cref='PictureBox'/> is sized to fit the
        ///  size of the image that is displayed.
        /// </summary>
        AutoSize = 2,

        /// <summary>
        ///  The image is displayed in the center if the
        /// <see cref='PictureBox'/> is larger than the
        ///  image. If the image is larger than the <see cref='PictureBox'/>,
        ///  the center of the picture is placed in the center of the
        /// <see cref='PictureBox'/> and the outside edges are
        ///  clipped.
        /// </summary>
        CenterImage = 3,

        /// <summary>
        ///  The size of image is increased or decresed maintaining the aspect ratio.
        /// </summary>
        Zoom = 4
    }
}
