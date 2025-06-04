// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Layout;

internal partial class FlowLayout
{
    /// <summary>
    ///  VerticalElementProxy swaps Top/Left, Bottom/Right, and other properties
    ///  so that the same code path used for horizontal flow can be applied to
    ///  vertical flow.
    /// </summary>
    private class VerticalElementProxy : ElementProxy
    {
        public override AnchorStyles AnchorStyles
        {
            get
            {
                AnchorStyles anchorStyles = LayoutUtils.GetUnifiedAnchor(Element!);
                bool isStretch = (anchorStyles & LayoutUtils.HorizontalAnchorStyles) == LayoutUtils.HorizontalAnchorStyles; // whether the control stretches to fill in the whole space
                bool isLeft = (anchorStyles & AnchorStyles.Left) != 0;  // whether the control anchors to left and does not stretch;
                bool isRight = (anchorStyles & AnchorStyles.Right) != 0; // whether the control anchors to right and does not stretch;
                if (isStretch)
                {
                    return LayoutUtils.VerticalAnchorStyles;
                }

                if (isLeft)
                {
                    return AnchorStyles.Top;
                }

                if (isRight)
                {
                    return AnchorStyles.Bottom;
                }

                return AnchorStyles.None;
            }
        }

        public override Rectangle Bounds
        {
            set => base.Bounds = LayoutUtils.FlipRectangle(value);
        }

        public override Padding Margin => LayoutUtils.FlipPadding(base.Margin);

        public override Size MinimumSize => LayoutUtils.FlipSize(base.MinimumSize);

        public override Size SpecifiedSize => LayoutUtils.FlipSize(base.SpecifiedSize);

        public override Size GetPreferredSize(Size proposedSize)
        {
            return LayoutUtils.FlipSize(base.GetPreferredSize(LayoutUtils.FlipSize(proposedSize)));
        }
    }
}
