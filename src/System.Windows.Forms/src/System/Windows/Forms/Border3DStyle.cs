// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;

    /// <include file='doc\Border3DStyle.uex' path='docs/doc[@for="Border3DStyle"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the style of a three-dimensional border.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public enum Border3DStyle {

        /// <include file='doc\Border3DStyle.uex' path='docs/doc[@for="Border3DStyle.Adjust"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The border is drawn
        ///       outside the
        ///       specified rectangle, preserving the dimensions of the rectangle for drawing.
        ///    </para>
        /// </devdoc>
        Adjust = NativeMethods.BF_ADJUST,

        /// <include file='doc\Border3DStyle.uex' path='docs/doc[@for="Border3DStyle.Bump"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The border has
        ///       a raised outer edge and a sunken inner edge.
        ///    </para>
        /// </devdoc>
        Bump = NativeMethods.EDGE_BUMP,

        /// <include file='doc\Border3DStyle.uex' path='docs/doc[@for="Border3DStyle.Etched"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The border has a
        ///       with a sunken inner edge and a raised outer edge.
        ///    </para>
        /// </devdoc>
        Etched = NativeMethods.EDGE_ETCHED,

        /// <include file='doc\Border3DStyle.uex' path='docs/doc[@for="Border3DStyle.Flat"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The border has a with no three-dimensional effects.
        ///    </para>
        /// </devdoc>
        Flat = NativeMethods.BF_FLAT | NativeMethods.EDGE_SUNKEN,

        /// <include file='doc\Border3DStyle.uex' path='docs/doc[@for="Border3DStyle.Raised"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The border has a with
        ///       raised inner and outer edges.
        ///    </para>
        /// </devdoc>
        Raised = NativeMethods.EDGE_RAISED,

        /// <include file='doc\Border3DStyle.uex' path='docs/doc[@for="Border3DStyle.RaisedInner"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The border has a with a raised inner edge and no outer edge.
        ///    </para>
        /// </devdoc>
        RaisedInner = NativeMethods.BDR_RAISEDINNER,

        /// <include file='doc\Border3DStyle.uex' path='docs/doc[@for="Border3DStyle.RaisedOuter"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The border has a
        ///       with a raised outer edge and no inner edge.
        ///    </para>
        /// </devdoc>
        RaisedOuter = NativeMethods.BDR_RAISEDOUTER,

        /// <include file='doc\Border3DStyle.uex' path='docs/doc[@for="Border3DStyle.Sunken"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The border has a with sunken inner and outer edges.
        ///    </para>
        /// </devdoc>
        Sunken = NativeMethods.EDGE_SUNKEN,

        /// <include file='doc\Border3DStyle.uex' path='docs/doc[@for="Border3DStyle.SunkenInner"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The border has a with
        ///       a sunken inner edge and no outer edge.
        ///    </para>
        /// </devdoc>
        SunkenInner = NativeMethods.BDR_SUNKENINNER,

        /// <include file='doc\Border3DStyle.uex' path='docs/doc[@for="Border3DStyle.SunkenOuter"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The border has a with a sunken outer edge and no inner edge.
        ///    </para>
        /// </devdoc>
        SunkenOuter = NativeMethods.BDR_SUNKENOUTER,
    }
}
