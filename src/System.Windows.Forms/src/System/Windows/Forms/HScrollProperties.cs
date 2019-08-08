// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Basic Properties for HScroll.
    /// </summary>
    public class HScrollProperties : ScrollProperties
    {
        public HScrollProperties(ScrollableControl container) : base(container)
        {
        }

        internal override int PageSize => ParentControl.ClientRectangle.Width;

        internal override int Orientation => NativeMethods.SB_HORZ;

        internal override int HorizontalDisplayPosition => -_value;

        internal override int VerticalDisplayPosition => ParentControl.DisplayRectangle.Y;
    }
}
