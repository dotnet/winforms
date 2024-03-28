// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Layout;

internal partial class FlowLayout
{
    /// <summary>
    ///  ElementProxy inserts a level of indirection between the LayoutEngine
    ///  and the IArrangedElement that allows us to use the same code path
    ///  for Vertical and Horizontal flow layout. (see VerticalElementProxy)
    /// </summary>
    private class ElementProxy
    {
        private IArrangedElement? _element;

        public virtual AnchorStyles AnchorStyles
        {
            get
            {
                AnchorStyles anchorStyles = LayoutUtils.GetUnifiedAnchor(Element!);
                bool isStretch = (anchorStyles & LayoutUtils.VerticalAnchorStyles) == LayoutUtils.VerticalAnchorStyles; // whether the control stretches to fill in the whole space
                bool isTop = (anchorStyles & AnchorStyles.Top) != 0;   // whether the control anchors to top and does not stretch;
                bool isBottom = (anchorStyles & AnchorStyles.Bottom) != 0;  // whether the control anchors to bottom and does not stretch;

                if (isStretch)
                {
                    // the element stretches to fill in the whole row. Equivalent to AnchorStyles.Top|AnchorStyles.Bottom
                    return LayoutUtils.VerticalAnchorStyles;
                }

                if (isTop)
                {
                    // the element anchors to top and doesn't stretch
                    return AnchorStyles.Top;
                }

                if (isBottom)
                {
                    // the element anchors to bottom and doesn't stretch
                    return AnchorStyles.Bottom;
                }

                return AnchorStyles.None;
            }
        }

        public bool AutoSize => CommonProperties.GetAutoSize(_element!);

        public virtual Rectangle Bounds
        {
            set => _element!.SetBounds(value, BoundsSpecified.None);
        }

        public IArrangedElement? Element
        {
            get => _element;
            set
            {
                _element = value;
                Debug.Assert(Element == value, "Element should be the same as we set it to");
            }
        }

        public virtual Padding Margin => CommonProperties.GetMargin(Element!);

        public virtual Size MinimumSize => CommonProperties.GetMinimumSize(Element!, Size.Empty);

        public bool ParticipatesInLayout => _element!.ParticipatesInLayout;

        public virtual Size SpecifiedSize => CommonProperties.GetSpecifiedBounds(_element!).Size;

        public bool Stretches
        {
            get
            {
                AnchorStyles styles = AnchorStyles;
                return (LayoutUtils.VerticalAnchorStyles & styles) == LayoutUtils.VerticalAnchorStyles;
            }
        }

        public virtual Size GetPreferredSize(Size proposedSize) => _element!.GetPreferredSize(proposedSize);
    }
}
