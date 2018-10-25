// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    /// <include file='doc\ScrollOrientation.uex' path='docs/doc[@for="ScrollOrientation"]/*' />
    /// <devdoc>
    /// <para>Provides data for the <see cref='System.Windows.Forms.ScrollBar.Scroll'/>
    /// event. This enumeration gives the orientation of the scroll that took place</para>
    /// </devdoc>
    public enum ScrollOrientation {

        /// <include file='doc\ScrollOrientation.uex' path='docs/doc[@for="ScrollOrientation.HorizontalScroll"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Denotes that horizontal scrolling took place.
        ///    </para>
        /// </devdoc>
        HorizontalScroll,

        /// <include file='doc\ScrollOrientation.uex' path='docs/doc[@for="ScrollOrientation.VerticalScroll"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Denotes that vertical scrolling took place.
        ///    </para>
        /// </devdoc>
        VerticalScroll
    }
}
