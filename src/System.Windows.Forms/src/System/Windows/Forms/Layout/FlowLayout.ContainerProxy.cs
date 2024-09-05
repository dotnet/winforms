// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Layout;

internal partial class FlowLayout
{
    /// <summary>
    ///  The goal of the FlowLayout Engine is to always layout from left to right. In order to achieve different
    ///  flow directions we have "Proxies" for the Container (the thing laying out) and for setting the bounds of the
    ///  child elements.
    ///
    ///  We have a base ContainerProxy, and derived proxies for all of the flow directions. In order to achieve flow
    ///  direction of RightToLeft, the RightToLeft container proxy detects when we're going to set the bounds
    ///  and translates it to the right.
    ///
    ///  In order to do a vertical flow, such as TopDown, we pretend we're laying out horizontally. The main way this is
    ///  achieved is through the use of the VerticalElementProxy, which flips all rectangles and sizes.
    ///
    ///  In order to do BottomUp, we combine the same techniques of TopDown with the RightToLeft flow. That is,
    ///  we override the bounds, and translate from left to right, AND use the VerticalElementProxy.
    ///
    ///  A final note: This layout engine does all its RightToLeft translation itself. It does not support
    ///  WS_EX_LAYOUTRTL (OS mirroring).
    /// </summary>
    private class ContainerProxy
    {
        private readonly IArrangedElement _container;
        private readonly bool _isContainerRTL;
        private Rectangle _displayRect;
        private ElementProxy? _elementProxy;

        public ContainerProxy(IArrangedElement container)
        {
            _container = container;
            _isContainerRTL = false;
            if (_container is Control control)
            {
                _isContainerRTL = control.RightToLeft == RightToLeft.Yes;
            }
        }

        public virtual Rectangle Bounds
        {
            set
            {
                if (IsContainerRTL)
                {
                    // Offset the X coordinate.
                    if (IsVertical)
                    {
                        // Offset the Y value here, since it is really the X value.
                        value.Y = DisplayRect.Bottom - value.Bottom;
                    }
                    else
                    {
                        value.X = DisplayRect.Right - value.Right;
                    }

                    if (Container is FlowLayoutPanel flp)
                    {
                        Point ptScroll = flp.AutoScrollPosition;
                        if (ptScroll != Point.Empty)
                        {
                            Point pt = new(value.X, value.Y);
                            if (IsVertical)
                            {
                                // Offset the Y value here, since it is really the X value.
                                pt.Offset(0, ptScroll.X);
                            }
                            else
                            {
                                pt.Offset(ptScroll.X, 0);
                            }

                            value.Location = pt;
                        }
                    }
                }

                ElementProxy.Bounds = value;
            }
        }

        public IArrangedElement Container => _container;

        /// <summary>
        ///  Returns the display rectangle of the container - this will be flipped if the layout
        ///  is a vertical layout.
        /// </summary>
        public Rectangle DisplayRect
        {
            get => _displayRect;
            set
            {
                if (_displayRect != value)
                {
                    // flip the displayRect since when we do layout direction TopDown/BottomUp, we layout the controls
                    // on the flipped rectangle as if our layout direction were LeftToRight/RightToLeft. In this case
                    // we can save some code bloat
                    _displayRect = LayoutUtils.FlipRectangleIf(IsVertical, value);
                }
            }
        }

        /// <summary>
        ///  Returns the element proxy to use. A vertical element proxy will typically flip
        ///  all the sizes and rectangles so that it can fake being laid out in a
        ///  horizontal manner.
        /// </summary>
        public ElementProxy ElementProxy
        {
            get
            {
                _elementProxy ??= IsVertical ? new VerticalElementProxy() : new ElementProxy();

                return _elementProxy;
            }
        }

        protected bool IsContainerRTL => _isContainerRTL;

        /// <summary>
        ///  Specifies if we're TopDown or BottomUp and should use the VerticalElementProxy to
        ///  translate
        /// </summary>
        protected virtual bool IsVertical => false;
        /// <summary>
        ///  Used when you want to translate from right to left, but preserve Margin.Right
        ///  and Margin.Left.
        /// </summary>
        protected Rectangle RTLTranslateNoMarginSwap(Rectangle bounds)
        {
            Rectangle newBounds = bounds;
            newBounds.X = DisplayRect.Right - bounds.X - bounds.Width + ElementProxy.Margin.Left - ElementProxy.Margin.Right;

            // Since DisplayRect.Right and bounds.X are both adjusted for the AutoScrollPosition, we need add it back here.
            if (Container is FlowLayoutPanel flp)
            {
                Point ptScroll = flp.AutoScrollPosition;
                if (ptScroll != Point.Empty)
                {
                    Point pt = new(newBounds.X, newBounds.Y);
                    if (IsVertical)
                    {
                        // We need to treat Vertical a little differently. It really helps if you draw this out.
                        // Remember that when we layout BottomUp, we first layout TopDown, then call this method.
                        // When we layout TopDown we layout in flipped rectangles. I.e. x becomes y, y becomes x,
                        // height becomes width, width becomes height. We do our layout, then when we eventually
                        // set the bounds of the child elements, we flip back. Thus, x will eventually
                        // become y. We need to adjust for scrolling - but only in the y direction -
                        // and since x becomes y, we adjust x. But since AutoScrollPosition has not been swapped,
                        // we need to use its Y coordinate when offsetting.

                        pt.Offset(ptScroll.Y, 0);
                    }
                    else
                    {
                        pt.Offset(ptScroll.X, 0);
                    }

                    newBounds.Location = pt;
                }
            }

            return newBounds;
        }
    }
}
