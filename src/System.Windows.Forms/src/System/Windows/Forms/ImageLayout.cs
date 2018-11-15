// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    /// <include file='doc\ImageLayout.uex' path='docs/doc[@for="ImageLayout"]/*' />
    /// <devdoc>
    ///    <para>Specifies the position of the image on the control.</para>
    /// </devdoc>
    public enum ImageLayout {
        /// <include file='doc\ImageLayout.uex' path='docs/doc[@for="ImageLayout.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The image is aligned TOP - LEFT across the controls client rectangle.
        ///    </para>
        /// </devdoc>
        None,
        /// <include file='doc\ImageLayout.uex' path='docs/doc[@for="ImageLayout.Tile"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The image is tiled across the controls client rectangle.
        ///    </para>
        /// </devdoc>
        Tile,
        /// <include file='doc\ImageLayout.uex' path='docs/doc[@for="ImageLayout.Center"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The image is centred within the controls client rectangle.
        ///    </para>
        /// </devdoc>
        Center,
        /// <include file='doc\ImageLayout.uex' path='docs/doc[@for="ImageLayout.Stretch"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The image is streched across the controls client rectangle.
        ///    </para>
        /// </devdoc>
        Stretch,
        /// <include file='doc\ImageLayout.uex' path='docs/doc[@for="ImageLayout.Zoom"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The image is streched across the controls client rectangle.
        ///    </para>
        /// </devdoc>
        Zoom,
    }
}  


