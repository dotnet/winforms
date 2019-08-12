// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the sides of a rectangle to apply a three-dimensional border to.
    /// </summary>
    [ComVisible(true)]
    [Flags]
    public enum Border3DSide
    {
        /// <summary>
        ///  A three-dimensional border on the left edge of the control.
        /// </summary>
        Left = NativeMethods.BF_LEFT,

        /// <summary>
        ///  A three-dimensional border on the top edge of the rectangle.
        /// </summary>
        Top = NativeMethods.BF_TOP,

        /// <summary>
        ///  A three-dimensional border on the right side of the rectangle.
        /// </summary>
        Right = NativeMethods.BF_RIGHT,

        /// <summary>
        ///  A three-dimensional border on the bottom side of the rectangle.
        /// </summary>
        Bottom = NativeMethods.BF_BOTTOM,

        /// <summary>
        ///  The interior of the rectangle is filled with the color defined for
        ///  three-dimensional controls instead of the background color for the form.
        /// </summary>
        Middle = NativeMethods.BF_MIDDLE,

        /// <summary>
        ///  A three-dimensional border on all four edges and fill the middle of
        ///  the rectangle with the color defeined for three-dimensional controls.
        /// </summary>
        All = Left | Top | Right | Bottom | Middle,
    }
}
