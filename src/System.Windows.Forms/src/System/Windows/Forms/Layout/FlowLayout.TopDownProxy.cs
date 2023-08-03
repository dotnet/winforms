// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Layout;

internal partial class FlowLayout
{
    /// <summary>
    ///  For TopDown we're really still laying out horizontally. The element proxy is the one
    ///  which flips all the rectangles and rotates itself into the vertical orientation.
    ///  to achieve right to left, we actually have to do something non-intuitive - instead of
    ///  sending the control to the right, we have to send the control to the bottom. When the rotation
    ///  is complete - that's equivalent to pushing it to the right.
    /// </summary>
    private class TopDownProxy : ContainerProxy
    {
        public TopDownProxy(IArrangedElement container) : base(container)
        {
        }

        protected override bool IsVertical => true;
    }
}
