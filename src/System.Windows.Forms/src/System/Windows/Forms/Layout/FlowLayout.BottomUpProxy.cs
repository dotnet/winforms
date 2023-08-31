// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Layout;

internal partial class FlowLayout
{
    private class BottomUpProxy : ContainerProxy
    {
        public BottomUpProxy(IArrangedElement container) : base(container)
        {
        }

        /// <summary>
        ///  For BottomUp we're really still laying out horizontally. The element proxy is the one
        ///  which flips all the rectangles and rotates itself into the vertical orientation.
        ///  BottomUp is the analog of RightToLeft - meaning, in order to place a control at the bottom,
        ///  the control has to be placed to the right. When the rotation is complete, that's the equivalent of
        ///  pushing it to the right. This must be done all the time.
        ///
        ///  To achieve right to left, we actually have to do something non-intuitive - instead of
        ///  sending the control to the right, we have to send the control to the bottom. When the rotation
        ///  is complete - that's equivalent to pushing it to the right.
        /// </summary>
        public override Rectangle Bounds
        {
            set
            {
                // push the control to the bottom.
                // Do NOT use LayoutUtils.RTLTranslate as we want to preserve the padding.Right on the right...
                base.Bounds = RTLTranslateNoMarginSwap(value);
            }
        }

        protected override bool IsVertical => true;
    }
}
