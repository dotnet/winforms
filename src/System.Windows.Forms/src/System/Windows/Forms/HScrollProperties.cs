// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Basic Properties for HScroll.
    /// </summary>
    public class HScrollProperties : ScrollProperties
    {
        public HScrollProperties(ScrollableControl? container) : base(container)
        {
        }

        private protected override int GetPageSize(ScrollableControl parent) => parent.ClientRectangle.Width;

        private protected override User32.SB Orientation => User32.SB.HORZ;

        private protected override int GetHorizontalDisplayPosition(ScrollableControl parent) => -_value;

        private protected override int GetVerticalDisplayPosition(ScrollableControl parent) => parent.DisplayRectangle.Y;
    }
}
