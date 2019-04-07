// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Basic Properties for VScroll.
    /// </devdoc>
    public class VScrollProperties : ScrollProperties
    {
        public VScrollProperties(ScrollableControl container) : base(container)
        {
        }

        internal override int PageSize => ParentControl.ClientRectangle.Height;

        internal override int Orientation => NativeMethods.SB_VERT;

        internal override int HorizontalDisplayPosition => ParentControl.DisplayRectangle.X;

        internal override int VerticalDisplayPosition => -_value;
    }
}
