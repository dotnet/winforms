// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;

    /// <include file='doc\ProgressBarStyle.uex' path='docs/doc[@for="ProgressBarStyle"]/*' />
    /// <devdoc>
    ///    <para>
    ///       This Enumeration represents the styles the ProgressBar can take.
    ///       Blocks and Continuous.  
    ///    </para>
    /// </devdoc>
    public enum ProgressBarStyle
    {
        /// <include file='doc\ProgressBarStyle.uex' path='docs/doc[@for="ProgressBarStyle.Blocks"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The progress bar displays the progress status as a segmented bar.  
        ///    </para>
        /// </devdoc>
        Blocks,

        /// <include file='doc\ProgressBarStyle.uex' path='docs/doc[@for="ProgressBarStyle.Continuous"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The progress bar displays the progress status in a smooth scrolling bar.  
        ///    </para>
        /// </devdoc>
        Continuous,

        /// <include file='doc\ProgressBarStyle.uex' path='docs/doc[@for="ProgressBarStyle.Marquee"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The progress bar displays the progress status in the marquee style.  
        ///    </para>
        /// </devdoc>
        Marquee
    }
}
