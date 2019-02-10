// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using Microsoft.Win32;



    /// <devdoc>
    ///    <para>
    ///       Specifies
    ///       the sides of a rectangle to apply a three-dimensional border to.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true), Flags]
    public enum Border3DSide {


        /// <devdoc>
        ///    <para>
        ///       A three-dimensional border on
        ///       the left edge
        ///       of the control.
        ///    </para>
        /// </devdoc>
        Left = NativeMethods.BF_LEFT,


        /// <devdoc>
        ///    <para>
        ///       A three-dimensional border on
        ///       the top edge
        ///       of the rectangle.
        ///    </para>
        /// </devdoc>
        Top = NativeMethods.BF_TOP,


        /// <devdoc>
        ///    <para>
        ///       A three-dimensional border on
        ///       the right side
        ///       of the rectangle.
        ///    </para>
        /// </devdoc>
        Right = NativeMethods.BF_RIGHT,


        /// <devdoc>
        ///    <para>
        ///       A three-dimensional border on
        ///       the bottom side
        ///       of the rectangle.
        ///    </para>
        /// </devdoc>
        Bottom = NativeMethods.BF_BOTTOM,


        /// <devdoc>
        ///    <para>
        ///       The interior of the rectangle is filled with the
        ///       color defined for three-dimensional controls instead of the
        ///       background color
        ///       for the form.
        ///    </para>
        /// </devdoc>
        Middle = NativeMethods.BF_MIDDLE,


        /// <devdoc>
        ///    <para>
        ///       A three-dimensional border on all four
        ///       edges and fill the middle of
        ///       the rectangle with
        ///       the color defeined for three-dimensional controls.
        ///    </para>
        /// </devdoc>
        All  = Left | Top | Right | Bottom | Middle,

    }
}
