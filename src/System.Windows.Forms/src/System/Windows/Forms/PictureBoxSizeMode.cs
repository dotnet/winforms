// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\PictureBoxSizeMode.uex' path='docs/doc[@for="PictureBoxSizeMode"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies how an image is positioned within a <see cref='System.Windows.Forms.PictureBox'/>.
    ///    </para>
    /// </devdoc>
    public enum PictureBoxSizeMode {

        /// <include file='doc\PictureBoxSizeMode.uex' path='docs/doc[@for="PictureBoxSizeMode.Normal"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The image is placed in the top-left corner of the
        ///    <see cref='System.Windows.Forms.PictureBox'/>. The image is clipped if
        ///       the <see cref='System.Windows.Forms.PictureBox'/>
        ///       is to small.
        ///    </para>
        /// </devdoc>
        Normal = 0,

        /// <include file='doc\PictureBoxSizeMode.uex' path='docs/doc[@for="PictureBoxSizeMode.StretchImage"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The image within the <see cref='System.Windows.Forms.PictureBox'/> is stretched or shrunk to fit the
        ///       current size of the <see cref='System.Windows.Forms.PictureBox'/>.
        ///    </para>
        /// </devdoc>
        StretchImage = 1,

        /// <include file='doc\PictureBoxSizeMode.uex' path='docs/doc[@for="PictureBoxSizeMode.AutoSize"]/*' />
        /// <devdoc>
        ///    The <see cref='System.Windows.Forms.PictureBox'/> is sized to fit the
        ///    size of the image that is displayed.
        /// </devdoc>
        AutoSize = 2,

        /// <include file='doc\PictureBoxSizeMode.uex' path='docs/doc[@for="PictureBoxSizeMode.CenterImage"]/*' />
        /// <devdoc>
        ///    The image is displayed in the center if the <see cref='System.Windows.Forms.PictureBox'/> is larger than the image. If the image
        ///    is larger than the <see cref='System.Windows.Forms.PictureBox'/>, the center of the picture is placed in the
        ///    center of the <see cref='System.Windows.Forms.PictureBox'/> and the outside edges are clipped.
        /// </devdoc>
        CenterImage = 3,

        /// <include file='doc\PictureBoxSizeMode.uex' path='docs/doc[@for="PictureBoxSizeMode.Zoom"]/*' />
        /// <devdoc>
        ///    The size of image is increased or decresed maintaining the aspect ratio.
        /// </devdoc>
        Zoom = 4

    }
}
