// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Layout {
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    internal class FlowLayout : LayoutEngine {

        // Singleton instance shared by all FlowPanels.
        internal static readonly FlowLayout Instance = new FlowLayout();

        private static readonly int _wrapContentsProperty = PropertyStore.CreateKey();
        private static readonly int _flowDirectionProperty = PropertyStore.CreateKey();

        internal static FlowLayoutSettings CreateSettings(IArrangedElement owner) {
            return new FlowLayoutSettings(owner);
        }
        
        // Entry point from LayoutEngine
        internal override bool LayoutCore(IArrangedElement container, LayoutEventArgs args) {
#if DEBUG        
            if (CompModSwitches.FlowLayout.TraceInfo) {
                Debug.WriteLine("FlowLayout::Layout("
                    + "container=" + container.ToString() + ", "
                    + "displayRect=" + container.DisplayRectangle.ToString() + ", "
                    + "args=" + args.ToString() + ")");
            }
            Debug.Indent();
#endif            

            // ScrollableControl will first try to get the layoutbounds from the derived control when 
            // trying to figure out if ScrollBars should be added.         
            CommonProperties.SetLayoutBounds(container, xLayout(container, container.DisplayRectangle, /* measureOnly = */ false));
#if DEBUG
            Debug.Unindent();
#endif
            return CommonProperties.GetAutoSize(container);
        }

        internal override Size GetPreferredSize(IArrangedElement container, Size proposedConstraints) {
#if DEBUG        
            if (CompModSwitches.FlowLayout.TraceInfo) {            
                Debug.WriteLine("FlowLayout::GetPreferredSize("
                    + "container=" + container.ToString() + ", "
                    + "proposedConstraints=" + proposedConstraints.ToString() + ")");
                Debug.Indent();
            }
#endif
            Rectangle measureBounds = new Rectangle(new Point(0, 0), proposedConstraints);
            Size prefSize = xLayout(container, measureBounds, /* measureOnly = */ true);

            if(prefSize.Width > proposedConstraints.Width || prefSize.Height> proposedConstraints.Height) {
                // Controls measured earlier than a control which couldn't be fit to constraints may
                // shift around with the new bounds.  We need to make a 2nd pass through the
                // controls using these bounds which are gauranteed to fit.
                measureBounds.Size = prefSize;
                prefSize = xLayout(container, measureBounds, /* measureOnly = */ true);
            }

#if DEBUG
            if (CompModSwitches.FlowLayout.TraceInfo) {
                Debug.Unindent();
                Debug.WriteLine("GetPreferredSize returned " + prefSize);
            }
#endif
            return prefSize;
        }

        private static ContainerProxy CreateContainerProxy(IArrangedElement container, FlowDirection flowDirection) {

            switch (flowDirection) {
                case FlowDirection.RightToLeft:
                    return new RightToLeftProxy(container);
                case FlowDirection.TopDown:
                    return new TopDownProxy(container);
                case FlowDirection.BottomUp:
                    return new BottomUpProxy(container);
                case FlowDirection.LeftToRight:
                default:
                    return new ContainerProxy(container);
            }
        
        }


        // Both LayoutCore and GetPreferredSize forward to this method.  The measureOnly flag determines which
        // behavior we get.
        private Size xLayout(IArrangedElement container, Rectangle displayRect, bool measureOnly) {
            FlowDirection flowDirection = GetFlowDirection(container);
            bool wrapContents = GetWrapContents(container);

            ContainerProxy containerProxy = CreateContainerProxy(container, flowDirection);
            containerProxy.DisplayRect = displayRect;
            
            // refetch as it's now adjusted for Vertical.
            displayRect = containerProxy.DisplayRect;
           
            ElementProxy elementProxy = containerProxy.ElementProxy;
            Size layoutSize = Size.Empty;

            if(!wrapContents) {
                // pretend that the container is infinitely wide to prevent wrapping.
                // DisplayRectangle.Right is Width + X - subtract X to prevent overflow.
                displayRect.Width = Int32.MaxValue - displayRect.X;
            }

            for(int i = 0; i < container.Children.Count;) {
                int breakIndex;
                Size rowSize = Size.Empty;

                Rectangle measureBounds = new Rectangle(displayRect.X, displayRect.Y, displayRect.Width, displayRect.Height - layoutSize.Height);
                rowSize = MeasureRow(containerProxy, elementProxy, i, measureBounds, out breakIndex);


                // if we are not wrapping contents, then the breakIndex (as set in MeasureRow)
                // should be equal to the count of child items in the container.
                Debug.Assert(wrapContents == true || breakIndex == container.Children.Count,
                    "We should not be trying to break the row if we are not wrapping contents.");
                
                if(!measureOnly) {
                    Rectangle rowBounds = new Rectangle(displayRect.X,
                        layoutSize.Height + displayRect.Y,
                        rowSize.Width,
                        rowSize.Height);
                    LayoutRow(containerProxy, elementProxy, /* startIndex = */ i, /* endIndex = */ breakIndex, rowBounds);
                }
                layoutSize.Width = Math.Max(layoutSize.Width, rowSize.Width);
                layoutSize.Height += rowSize.Height;
                i = breakIndex;
            }
            //verify that our alignment is correct
            if (container.Children.Count != 0 && !measureOnly) {
                Debug_VerifyAlignment(container, flowDirection);
            }
            return LayoutUtils.FlipSizeIf(flowDirection == FlowDirection.TopDown || GetFlowDirection(container) == FlowDirection.BottomUp, layoutSize);
        }

        // Just forwards to xLayoutRow.  This will layout elements from the start index to the end index.  RowBounds
        // was computed by a call to measure row and is used for alignment/boxstretch.  See the ElementProxy class
        // for an explaination of the elementProxy parameter.
        private void LayoutRow(ContainerProxy containerProxy, ElementProxy elementProxy, int startIndex, int endIndex, Rectangle rowBounds) {
            int dummy;
            Size outSize = xLayoutRow(containerProxy, elementProxy, startIndex, endIndex, rowBounds, /* breakIndex = */ out dummy, /* measureOnly = */ false);
            Debug.Assert(dummy == endIndex, "EndIndex / BreakIndex mismatch.");
        }

        // Just forwards to xLayoutRow.  breakIndex is the index of the first control not to fit in the displayRectangle.  The
        // returned Size is the size required to layout the controls from startIndex up to but not including breakIndex.  See
        // the ElementProxy class for an explaination of the elementProxy parameter.
        private Size MeasureRow(ContainerProxy containerProxy, ElementProxy elementProxy, int startIndex, Rectangle displayRectangle, out int breakIndex) {
            return xLayoutRow(containerProxy, elementProxy, startIndex, /* endIndex = */ containerProxy.Container.Children.Count, displayRectangle, out breakIndex, /* measureOnly = */ true);
        }

        // LayoutRow and MeasureRow both forward to this method.  The measureOnly flag determines which behavior we get.
        private Size xLayoutRow(ContainerProxy containerProxy, ElementProxy elementProxy, int startIndex, int endIndex, Rectangle rowBounds, out int breakIndex, bool measureOnly) {
            Debug.Assert(startIndex < endIndex, "Loop should be in forward Z-order.");
            Point location = rowBounds.Location;
            Size rowSize = Size.Empty;
            int laidOutItems = 0;
            breakIndex = startIndex;

            bool wrapContents = GetWrapContents(containerProxy.Container);
            bool breakOnNextItem = false;

            
            ArrangedElementCollection collection = containerProxy.Container.Children;
            for(int i = startIndex; i < endIndex; i++, breakIndex++) {
                elementProxy.Element = collection[i];

                if(!elementProxy.ParticipatesInLayout) {
                    continue;
                }

                // Figure out how much space this element is going to need (requiredSize)
                //
                Size prefSize;
                if(elementProxy.AutoSize) {
                    Size elementConstraints = new Size(Int32.MaxValue, rowBounds.Height - elementProxy.Margin.Size.Height);
                    if(i == startIndex) {
                        // If the element is the first in the row, attempt to pack it to the row width.  (If its not 1st, it will wrap
                        // to the first on the next row if its too long and then be packed if needed by the next call to xLayoutRow).
                        elementConstraints.Width = rowBounds.Width - rowSize.Width - elementProxy.Margin.Size.Width;
                    }

                    // Make sure that subtracting the margins does not cause width/height to be <= 0, or we will
                    // size as if we had infinite space when in fact we are trying to be as small as possible.
                    elementConstraints = LayoutUtils.UnionSizes(new Size(1, 1), elementConstraints);                    
                    prefSize = elementProxy.GetPreferredSize(elementConstraints);
                } else {
                    // If autosizing is turned off, we just use the element's current size as its preferred size.
                    prefSize = elementProxy.SpecifiedSize;
                    
                    // except if it is stretching - then ignore the affect of the height dimension.
                    if (elementProxy.Stretches) {
                        prefSize.Height = 0;
                    } 

                    // Enforce MinimumSize
                    if (prefSize.Height < elementProxy.MinimumSize.Height) {
                        prefSize.Height = elementProxy.MinimumSize.Height;
                    }
                }
                Size requiredSize = prefSize + elementProxy.Margin.Size;

                // Position the element (if applicable).
                //
                if(!measureOnly) {
                    // If measureOnly = false, rowBounds.Height = measured row hieght
                    // (otherwise its the remaining displayRect of the container)
                    
                    Rectangle cellBounds = new Rectangle(location, new Size(requiredSize.Width, rowBounds.Height));

                    // We laid out the rows with the elementProxy's margins included.
                    // We now deflate the rect to get the actual elementProxy bounds.
                    cellBounds = LayoutUtils.DeflateRect(cellBounds, elementProxy.Margin);
                    
                    AnchorStyles anchorStyles = elementProxy.AnchorStyles;
                    containerProxy.Bounds = LayoutUtils.AlignAndStretch(prefSize, cellBounds, anchorStyles);
                }
        
                // Keep track of how much space is being used in this row
                //
              
                location.X += requiredSize.Width;
                

               
                if (laidOutItems > 0) {
                    // If control does not fit on this row, exclude it from row and stop now.
                    //   Exception: If row is empty, allow this control to fit on it. So controls
                    //   that exceed the maximum row width will end up occupying their own rows.
                    if(location.X > rowBounds.Right) {
                        break;
                    }

                }
                // Control fits on this row, so update the row size.
                //   rowSize.Width != location.X because with a scrollable control
                //   we could have started with a location like -100.
                
                rowSize.Width = location.X - rowBounds.X;
                
                rowSize.Height = Math.Max(rowSize.Height, requiredSize.Height);

                // check for line breaks.
                if (wrapContents) {
                    if (breakOnNextItem) {
                        break;
                    }
                    else if (i+1 < endIndex && CommonProperties.GetFlowBreak(elementProxy.Element)) {
                        if (laidOutItems == 0) {
                            breakOnNextItem = true;
                        }
                        else {
                            breakIndex++;
                            break;
                        }
                   }
                }
                ++laidOutItems;
            }

            return rowSize;
        }

        #region Provided properties
        public static bool GetWrapContents(IArrangedElement container) {
            int wrapContents = container.Properties.GetInteger(_wrapContentsProperty);
            return (wrapContents == 0);  // if not set return true.
        }
        
        public static void SetWrapContents(IArrangedElement container, bool value) {
            container.Properties.SetInteger(_wrapContentsProperty, value ? 0 : 1);  // set to 0 if true, 1 if false
            LayoutTransaction.DoLayout(container, container, PropertyNames.WrapContents);
            Debug.Assert(GetWrapContents(container) == value, "GetWrapContents should return the same value as we set");
        }

        public static FlowDirection GetFlowDirection(IArrangedElement container) {
            return (FlowDirection) container.Properties.GetInteger(_flowDirectionProperty);
        }
        
        public static void SetFlowDirection(IArrangedElement container, FlowDirection value) {
            //valid values are 0x0 to 0x3
            if (!ClientUtils.IsEnumValid(value, (int)value, (int)FlowDirection.LeftToRight, (int)FlowDirection.BottomUp)){
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(FlowDirection));
            }
            container.Properties.SetInteger(_flowDirectionProperty, (int) value);
            LayoutTransaction.DoLayout(container, container, PropertyNames.FlowDirection);
            Debug.Assert(GetFlowDirection(container) == value, "GetFlowDirection should return the same value as we set");
        }
        #endregion Provided properties

        #region ContainerProxy

        // What is a ContainerProxy?
        //
        // The goal of the FlowLayout Engine is to always layout from left to right.  In order to achieve different
        // flow directions we have "Proxies" for the Container (the thing laying out) and for setting the bounds of the 
        // child elements.  
        //
        // We have a base ContainerProxy, and derived proxies for all of the flow directions.  In order to achieve flow direction of RightToLeft, 
        // the RightToLeft container proxy detects when we're going to set the bounds and translates it to the right.
        //
        // In order to do a vertical flow, such as TopDown, we pretend we're laying out horizontally.  The main way this is
        // achieved is through the use of the VerticalElementProxy, which flips all rectangles and sizes.
        //
        // In order to do BottomUp, we combine the same techniques of TopDown with the RightToLeft flow.  That is,
        // we override the bounds, and translate from left to right, AND use the VerticalElementProxy.
        //
        // A final note: This layout engine does all its RightToLeft translation itself.  It does not support
        // WS_EX_LAYOUTRTL (OS mirroring).

        private class ContainerProxy {
            private IArrangedElement _container;
            private ElementProxy _elementProxy;
            private Rectangle _displayRect;
            private bool _isContainerRTL;
            public ContainerProxy(IArrangedElement container) {
                this._container = container;
                this._isContainerRTL = false;
                if (_container is Control) {
                   _isContainerRTL = ((Control)(_container)).RightToLeft == RightToLeft.Yes;
                }
            }

            // method for setting the bounds of a child element.
            public virtual Rectangle Bounds {
                set { 
                    if (IsContainerRTL) {
                        // Offset the X coordinate...
                        if (IsVertical) {
                            //Offset the Y value here, since it is really the X value....                            
                            value.Y = DisplayRect.Bottom - value.Bottom;
                        }
                        else {
                            value.X = DisplayRect.Right - value.Right;                            
                        }

                        FlowLayoutPanel flp = Container as FlowLayoutPanel;
                        if (flp != null) {
                            Point ptScroll = flp.AutoScrollPosition;
                            if (ptScroll != Point.Empty) {
                                Point pt = new Point(value.X, value.Y);
                                if (IsVertical) {
                                    //Offset the Y value here, since it is really the X value....
                                    pt.Offset(0, ptScroll.X);
                                }
                                else {
                                    pt.Offset(ptScroll.X, 0);
                                }
                                value.Location = pt;
                            }
                        }                        
                    }
                    
                ElementProxy.Bounds = value; 

                }
            }

            // specifies the container laying out
            public IArrangedElement Container {
                get { return _container; }
            }

            // specifies if we're TopDown or BottomUp and should use the VerticalElementProxy to translate
            protected virtual bool IsVertical {
                get { return false; }
            }

            // returns true if container is RTL.Yes
            protected bool IsContainerRTL {
                get { return _isContainerRTL; }
            }

            // returns the display rectangle of the container - this WILL BE FLIPPED if the layout
            // is a vertical layout.
            public Rectangle DisplayRect {
                get {
                    return _displayRect; 
                }
                set { 
                    if (_displayRect != value) {
                        
                        //flip the displayRect since when we do layout direction TopDown/BottomUp, we layout the controls
                        //on the flipped rectangle as if our layout direction were LeftToRight/RightToLeft. In this case
                        //we can save some code bloat
                        _displayRect = LayoutUtils.FlipRectangleIf(IsVertical, value);
                    }                    
                } 
            }

            // returns the element proxy to use.  A vertical element proxy will typically
            // flip all the sizes and rectangles so that it can fake being laid out in a horizontal manner.
            public ElementProxy ElementProxy {
                get { 
                    if (_elementProxy ==  null) {
                        _elementProxy = (IsVertical) ? new VerticalElementProxy() : new ElementProxy();
                    }
                    return _elementProxy;

                }
               
            }  

            // used when you want to translate from right to left, but preserve Margin.Right & Margin.Left.
            protected Rectangle RTLTranslateNoMarginSwap(Rectangle bounds) {

                Rectangle newBounds = bounds;
                
                newBounds.X = DisplayRect.Right - bounds.X - bounds.Width + ElementProxy.Margin.Left - ElementProxy.Margin.Right;

                // Since DisplayRect.Right and bounds.X are both adjusted for the AutoScrollPosition, we need add it back here.                
                FlowLayoutPanel flp = Container as FlowLayoutPanel;
                if (flp != null) {
                    Point ptScroll = flp.AutoScrollPosition;
                    if (ptScroll != Point.Empty) {
                        Point pt = new Point(newBounds.X, newBounds.Y);

                        if (IsVertical) {
                            
                            // We need to treat Vertical a litte differently. It really helps if you draw this out.
                            // Remember that when we layout BottomUp, we first layout TopDown, then call this method.
                            // When we layout TopDown we layout in flipped rectangles. I.e. x becomes y, y becomes x, 
                            // height becomes width, width becomes height. We do our layout, then when we eventually
                            // set the bounds of the child elements, we flip back. Thus, x will eventually
                            // become y. We need to adjust for scrolling - but only in the y direction - 
                            // and since x becomes y, we adjust x. But since AutoScrollPoisition has not been swapped, 
                            // we need to use its Y coordinate when offsetting.

                            pt.Offset(ptScroll.Y, 0);
                        } else {
                            pt.Offset(ptScroll.X, 0);
                        }
                        newBounds.Location = pt;
                    }
                }

                return newBounds;
            }

        }

        
        // FlowDirection.RightToLeft proxy.
        private class RightToLeftProxy : ContainerProxy {
            public RightToLeftProxy(IArrangedElement container) : base(container) {
            }

            
            public override Rectangle Bounds {
                set {
                     // if the container is RTL, align to the left, otherwise, align to the right.
                     // Do NOT use LayoutUtils.RTLTranslate as we want to preserve the padding.Right on the right...
                     base.Bounds = RTLTranslateNoMarginSwap(value);
                }
            }    
        }
        
      
        // FlowDirection.TopDown proxy.
        // For TopDown we're really still laying out horizontally.  The element proxy is the one 
        // which flips all the rectangles and rotates itself into the vertical orientation.  
        // to achieve right to left, we actually have to do something non-intuitive - instead of 
        // sending the control to the right, we have to send the control to the bottom.  When the rotation 
        // is complete - that's equivilant to pushing it to the right.
        private class TopDownProxy : ContainerProxy {
            public TopDownProxy(IArrangedElement container) : base(container) {
            }
            protected override bool IsVertical {
                get { return true; }
            }
       }

        // FlowDirection.BottomUp proxy.
        private class BottomUpProxy : ContainerProxy {
            public BottomUpProxy(IArrangedElement container) : base(container) {
            }
            protected override bool IsVertical {
                get { return true; }
            }

             // For BottomUp we're really still laying out horizontally.  The element proxy is the one 
             // which flips all the rectangles and rotates itself into the vertical orientation. 
             // BottomUp is the analog of RightToLeft - meaning, in order to place a control at the bottom, 
             // the control has to be placed to the right.  When the rotation is complete, that's the equivilant of
             // pushing it to the right.  This must be done all the time.
             //
             // To achieve right to left, we actually have to do something non-intuitive - instead of 
             // sending the control to the right, we have to send the control to the bottom.  When the rotation 
             // is complete - that's equivilant to pushing it to the right.

             public override Rectangle Bounds {
                set {
                     // push the control to the bottom.
                     // Do NOT use LayoutUtils.RTLTranslate as we want to preserve the padding.Right on the right...   
                     base.Bounds = RTLTranslateNoMarginSwap(value);
                }
            } 
        }

        #endregion ContainerProxy
        #region ElementProxy
        // ElementProxy inserts a level of indirection between the LayoutEngine
        // and the IArrangedElement that allows us to use the same code path
        // for Vertical and Horizontal flow layout.  (see VerticalElementProxy)
        private class ElementProxy {
            private IArrangedElement _element;  
            
            public virtual AnchorStyles AnchorStyles {
                get {
                    AnchorStyles anchorStyles = LayoutUtils.GetUnifiedAnchor(Element);
                    bool isStretch = (anchorStyles & LayoutUtils.VerticalAnchorStyles) == LayoutUtils.VerticalAnchorStyles; //whether the control stretches to fill in the whole space
                    bool isTop = (anchorStyles & AnchorStyles.Top) != 0;   //whether the control anchors to top and does not stretch;
                    bool isBottom = (anchorStyles & AnchorStyles.Bottom) != 0;  //whether the control anchors to bottom and does not stretch;

                    if(isStretch) {
                        //the element stretches to fill in the whole row. Equivalent to AnchorStyles.Top|AnchorStyles.Bottom
                        return LayoutUtils.VerticalAnchorStyles;
                    }
                    if(isTop) {
                        //the element anchors to top and doesn't stretch
                        return AnchorStyles.Top;
                    }
                    if(isBottom) {
                        //the element anchors to bottom and doesn't stretch
                        return AnchorStyles.Bottom;
                    }
                    return AnchorStyles.None;
                }
            }

            public bool AutoSize {
                get { return CommonProperties.GetAutoSize(_element); }
            }
            
            public virtual Rectangle Bounds {
                set { _element.SetBounds(value, BoundsSpecified.None); }
            }

            public IArrangedElement Element {
                get { return _element; }
                set { 
                    _element = value; 
                    Debug.Assert(Element == value, "Element should be the same as we set it to");
                }
            }

            public bool Stretches {
                get {
                    AnchorStyles styles = AnchorStyles;
                    if ((LayoutUtils.VerticalAnchorStyles & styles) == LayoutUtils.VerticalAnchorStyles) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            }

            public virtual Padding Margin {
                get { return CommonProperties.GetMargin(Element); }
            }

            public virtual Size MinimumSize {
                get { return CommonProperties.GetMinimumSize(Element, Size.Empty); }
            }
            
            public bool ParticipatesInLayout {
                get { return _element.ParticipatesInLayout; }
            }
          
            public virtual Size SpecifiedSize {
                get { return CommonProperties.GetSpecifiedBounds(_element).Size; }
            }

            public virtual Size GetPreferredSize(Size proposedSize) {
                return _element.GetPreferredSize(proposedSize);
            }   
        }

        // VerticalElementProxy swaps Top/Left, Bottom/Right, and other properties
        // so that the same code path used for horizantal flow can be applied to
        // vertical flow.
        private class VerticalElementProxy   : ElementProxy {
            public override AnchorStyles AnchorStyles {
                get {
                    AnchorStyles anchorStyles = LayoutUtils.GetUnifiedAnchor(Element);
                    bool isStretch = (anchorStyles & LayoutUtils.HorizontalAnchorStyles) == LayoutUtils.HorizontalAnchorStyles; //whether the control stretches to fill in the whole space
                    bool isLeft = (anchorStyles & AnchorStyles.Left) != 0;  //whether the control anchors to left and does not stretch;
                    bool isRight = (anchorStyles & AnchorStyles.Right) != 0; //whether the control anchors to right and does not stretch;
                    if(isStretch) {
                        return LayoutUtils.VerticalAnchorStyles;
                    }
                    if(isLeft) {
                        return AnchorStyles.Top;
                    }
                    if(isRight) {
                        return AnchorStyles.Bottom;
                    }
                    return AnchorStyles.None;
                }
            }

            public override Rectangle Bounds {
                set { base.Bounds = LayoutUtils.FlipRectangle(value); }
            }

            public override Padding Margin {
                get { return LayoutUtils.FlipPadding(base.Margin); }
            }

            public override Size MinimumSize {
                get { return LayoutUtils.FlipSize(base.MinimumSize); }
            }

            public override Size SpecifiedSize {
                get { return LayoutUtils.FlipSize(base.SpecifiedSize); }
            }

            public override Size GetPreferredSize(Size proposedSize) {
                return LayoutUtils.FlipSize(base.GetPreferredSize(LayoutUtils.FlipSize(proposedSize)));
            }
        }
        #endregion ElementProxy

        #region DEBUG
        [Conditional("DEBUG_VERIFY_ALIGNMENT")]
        private void Debug_VerifyAlignment(IArrangedElement container, FlowDirection flowDirection) {
#if DEBUG
            //We cannot apply any of these checks @ design-time since dragging new children into a FlowLayoutPanel
            //will attempt to set the children at the mouse position when the child was dropped - we rely on the controil
            //to reposition the children once added.
            Control flp = container as Control;
            if (flp != null && flp.Site != null && flp.Site.DesignMode) {
                return;
            }

            //check to see if the first element is in its right place
            Padding margin = CommonProperties.GetMargin(container.Children[0]);
            switch (flowDirection) {
                case FlowDirection.LeftToRight:
                case FlowDirection.TopDown:
                    Debug.Assert(container.Children[0].Bounds.Y == margin.Top + container.DisplayRectangle.Y);
                    Debug.Assert(container.Children[0].Bounds.X == margin.Left + container.DisplayRectangle.X);
                    break;
                case FlowDirection.RightToLeft:
                    Debug.Assert(container.Children[0].Bounds.X == container.DisplayRectangle.X + container.DisplayRectangle.Width - container.Children[0].Bounds.Width - margin.Right);
                    Debug.Assert(container.Children[0].Bounds.Y == margin.Top + container.DisplayRectangle.Y);
                    break;
                case FlowDirection.BottomUp:
                    Debug.Assert(container.Children[0].Bounds.Y == container.DisplayRectangle.Y + container.DisplayRectangle.Height - container.Children[0].Bounds.Height - margin.Bottom);
                    Debug.Assert(container.Children[0].Bounds.X == margin.Left + container.DisplayRectangle.X);
                    break;
            }                
            //next check to see if everything is in bound
            ArrangedElementCollection collection = container.Children;
            for (int i = 1; i < collection.Count; i++) {
                switch (flowDirection) {
                    case FlowDirection.LeftToRight:
                    case FlowDirection.TopDown:
                        Debug.Assert(collection[i].Bounds.Y >= container.DisplayRectangle.Y);
                        Debug.Assert(collection[i].Bounds.X >= container.DisplayRectangle.X);
                        break;
                    case FlowDirection.RightToLeft:
                        Debug.Assert(collection[i].Bounds.Y >= container.DisplayRectangle.Y);
                        Debug.Assert(collection[i].Bounds.X + collection[i].Bounds.Width <= container.DisplayRectangle.X + container.DisplayRectangle.Width);
                        break;
                    case FlowDirection.BottomUp:
                        Debug.Assert(collection[i].Bounds.Y + collection[i].Bounds.Height <= container.DisplayRectangle.Y + container.DisplayRectangle.Height);
                        Debug.Assert(collection[i].Bounds.X >= container.DisplayRectangle.X);
                        break;
                }
            }              
#endif
        }
        #endregion DEBUG
    }
}
