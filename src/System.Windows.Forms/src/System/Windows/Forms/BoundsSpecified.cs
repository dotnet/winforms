// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;

    /// <include file='doc\BoundsSpecified.uex' path='docs/doc[@for="BoundsSpecified"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the bounds of the control to
    ///       use when defining a control's size and position.
    ///    </para>
    /// </devdoc>
    [Flags]
    public enum BoundsSpecified {
        /// <include file='doc\BoundsSpecified.uex' path='docs/doc[@for="BoundsSpecified.X"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies the
        ///       left
        ///       edge of the
        ///       control is defined.
        ///    </para>
        /// </devdoc>
        X = 0x1,
        /// <include file='doc\BoundsSpecified.uex' path='docs/doc[@for="BoundsSpecified.Y"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies the
        ///       top edge of the
        ///       control of the
        ///       control is defined.
        ///    </para>
        /// </devdoc>
        Y = 0x2,
        /// <include file='doc\BoundsSpecified.uex' path='docs/doc[@for="BoundsSpecified.Width"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies
        ///       the width
        ///       of the control is defined.
        ///    </para>
        /// </devdoc>
        Width = 0x4,
        /// <include file='doc\BoundsSpecified.uex' path='docs/doc[@for="BoundsSpecified.Height"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies
        ///       the
        ///       height of the control is defined.
        ///    </para>
        /// </devdoc>
        Height = 0x8,
        /// <include file='doc\BoundsSpecified.uex' path='docs/doc[@for="BoundsSpecified.Location"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Both <see langword='X'/> and <see langword='Y'/> coordinates of the control are
        ///       defined.
        ///    </para>
        /// </devdoc>
        Location = X | Y,
        /// <include file='doc\BoundsSpecified.uex' path='docs/doc[@for="BoundsSpecified.Size"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Both <see cref='System.Windows.Forms.Control.Width'/> and <see cref='System.Windows.Forms.Control.Height'/> property values of the control are
        ///       defined.
        ///    </para>
        /// </devdoc>
        Size = Width | Height,
        /// <include file='doc\BoundsSpecified.uex' path='docs/doc[@for="BoundsSpecified.All"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Both <see cref='System.Windows.Forms.Control.Location'/> and <see cref='System.Windows.Forms.Control.Size'/> property values are
        ///       defined.
        ///    </para>
        /// </devdoc>
        All = Location | Size,
        /// <include file='doc\BoundsSpecified.uex' path='docs/doc[@for="BoundsSpecified.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       No
        ///       bounds
        ///       are specified.
        ///    </para>
        /// </devdoc>
        None = 0,
    }
}


