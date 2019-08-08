// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    internal class ToolStripSplitStackLayout : LayoutEngine
    {
        private Point noMansLand;
        private Rectangle displayRectangle = Rectangle.Empty;

#if DEBUG
        internal static readonly TraceSwitch DebugLayoutTraceSwitch = new TraceSwitch("DebugLayout", "Debug ToolStrip Layout code");
#else
        internal static readonly TraceSwitch DebugLayoutTraceSwitch;
#endif
        internal ToolStripSplitStackLayout(ToolStrip owner)
        {
            ToolStrip = owner;
        }

        /// <summary>
        ///  This is the index we use to send items to the overflow if we run out of room
        /// </summary>
        protected int BackwardsWalkingIndex { get; set; }

        /// <summary>
        // This is the index we use to walk the items and make  decisions if there is enough room.
        /// </summary>
        protected int ForwardsWalkingIndex { get; set; }

        private Size OverflowButtonSize
        {
            get
            {
                ToolStrip toolStrip = ToolStrip;
                if (!toolStrip.CanOverflow)
                {
                    return Size.Empty;
                }

                // since we havent parented the item yet - the auto size wont have reset the size yet.
                Size overflowButtonSize = toolStrip.OverflowButton.AutoSize ? toolStrip.OverflowButton.GetPreferredSize(displayRectangle.Size) : toolStrip.OverflowButton.Size;
                return overflowButtonSize + toolStrip.OverflowButton.Margin.Size;
            }
        }

        private int OverflowSpace { get; set; }

        private bool OverflowRequired { get; set; }

        /// <summary>
        ///  The current ToolStrip we're operating over.
        /// </summary>
        public ToolStrip ToolStrip { get; }

        /// <summary>
        ///  This method will mark whether items should be placed in the overflow or on the main ToolStrip.
        /// </summary>
        private void CalculatePlacementsHorizontal()
        {
            ResetItemPlacements();

            ToolStrip toolStrip = ToolStrip;
            int currentWidth = 0;

            if (ToolStrip.CanOverflow)
            {
                // determine the locations of all the items.
                for (ForwardsWalkingIndex = 0; ForwardsWalkingIndex < toolStrip.Items.Count; ForwardsWalkingIndex++)
                {
                    ToolStripItem item = toolStrip.Items[ForwardsWalkingIndex];

                    if (!((IArrangedElement)item).ParticipatesInLayout)
                    {
                        // skip over items not participating in layout.  E.G. not visible items
                        continue;
                    }

                    // if we have something set to overflow always we need to show an overflow button
                    if (item.Overflow == ToolStripItemOverflow.Always)
                    {
#if DEBUG
                        if (DebugLayoutTraceSwitch.TraceVerbose)
                        {
                            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "OverflowRequired - item set to alaways overflow: {0} ", item));
                        }
#endif
                        OverflowRequired = true;
                    }

                    if (item.Overflow != ToolStripItemOverflow.Always && item.Placement == ToolStripItemPlacement.None)
                    {
                        // since we havent parented the item yet - the auto size wont have reset the size yet.
                        Size itemSize = item.AutoSize ? item.GetPreferredSize(displayRectangle.Size) : item.Size;

                        currentWidth += itemSize.Width + item.Margin.Horizontal;

                        int overflowWidth = (OverflowRequired) ? OverflowButtonSize.Width : 0;

                        if (currentWidth > displayRectangle.Width - overflowWidth)
                        {
#if DEBUG
                            if (DebugLayoutTraceSwitch.TraceVerbose)
                            {
                                Debug.WriteLine("SendNextItemToOverflow to fres space for " + item.ToString());
                            }
#endif
                            int spaceRecovered = SendNextItemToOverflow((currentWidth + overflowWidth) - displayRectangle.Width, true);

                            currentWidth -= spaceRecovered;
                        }
                    }
                }
            }

            PlaceItems();
        }

        /// <summary>
        ///  This method will mark whether items should be placed in the overflow or on the main ToolStrip.
        /// </summary>
        private void CalculatePlacementsVertical()
        {
            ResetItemPlacements();

            ToolStrip toolStrip = ToolStrip;
            int currentHeight = 0; //toolStrip.Padding.Vertical;

            if (ToolStrip.CanOverflow)
            {
                // determine the locations of all the items.
                for (ForwardsWalkingIndex = 0; ForwardsWalkingIndex < ToolStrip.Items.Count; ForwardsWalkingIndex++)
                {
                    ToolStripItem item = toolStrip.Items[ForwardsWalkingIndex];

                    if (!((IArrangedElement)item).ParticipatesInLayout)
                    {
                        // skip over items not participating in layout.  E.G. not visible items
                        continue;
                    }

                    // if we have something set to overflow always we need to show an overflow button
                    if (item.Overflow == ToolStripItemOverflow.Always)
                    {
                        OverflowRequired = true;
#if DEBUG
                        if (DebugLayoutTraceSwitch.TraceVerbose)
                        {
                            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "OverflowRequired - item set to always overflow: {0} ", item));
                        }
#endif
                    }

                    if (item.Overflow != ToolStripItemOverflow.Always && item.Placement == ToolStripItemPlacement.None)
                    {
                        // since we havent parented the item yet - the auto size wont have reset the size yet.
                        Size itemSize = item.AutoSize ? item.GetPreferredSize(displayRectangle.Size) : item.Size;
                        int overflowWidth = (OverflowRequired) ? OverflowButtonSize.Height : 0;

                        currentHeight += itemSize.Height + item.Margin.Vertical;
#if DEBUG
                        if (DebugLayoutTraceSwitch.TraceVerbose)
                        { Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Adding {0} Size {1} to currentHeight = {2}", item.ToString(), itemSize, currentHeight)); }
#endif
                        if (currentHeight > displayRectangle.Height - overflowWidth)
                        {
#if DEBUG
                            if (DebugLayoutTraceSwitch.TraceVerbose)
                            { Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Got to {0} and realized that currentHeight = {1} is larger than displayRect {2} minus overflow {3}", item.ToString(), currentHeight, displayRectangle, overflowWidth)); }
#endif
                            int spaceRecovered = SendNextItemToOverflow(currentHeight - displayRectangle.Height, false);

                            currentHeight -= spaceRecovered;
                        }
                    }
                }
            }

            PlaceItems();
        }

        internal override Size GetPreferredSize(IArrangedElement container, Size proposedConstraints)
        {
            // undone be more clever here - perhaps figure out the biggest element and return that.
            if (!(container is ToolStrip))
            {
                throw new NotSupportedException(SR.ToolStripSplitStackLayoutContainerMustBeAToolStrip);
            }

            if (ToolStrip.LayoutStyle == ToolStripLayoutStyle.HorizontalStackWithOverflow)
            {
                return ToolStrip.GetPreferredSizeHorizontal(container, proposedConstraints);
            }
            else
            {
                return ToolStrip.GetPreferredSizeVertical(container, proposedConstraints);
            }
        }

        private void InvalidateLayout()
        {
            ForwardsWalkingIndex = 0;
            BackwardsWalkingIndex = -1;
            OverflowSpace = 0;
            OverflowRequired = false;
            displayRectangle = Rectangle.Empty;
        }

        private protected override bool LayoutCore(IArrangedElement container, LayoutEventArgs layoutEventArgs)
        {
            if (!(container is ToolStrip))
            {
                throw new NotSupportedException(SR.ToolStripSplitStackLayoutContainerMustBeAToolStrip);
            }

            InvalidateLayout();
            displayRectangle = ToolStrip.DisplayRectangle;

            // pick a location that's outside of the displayed region to send
            // items that will potentially clobber/overlay others.
            noMansLand = displayRectangle.Location;
            noMansLand.X += ToolStrip.ClientSize.Width + 1;
            noMansLand.Y += ToolStrip.ClientSize.Height + 1;

            if (ToolStrip.LayoutStyle == ToolStripLayoutStyle.HorizontalStackWithOverflow)
            {
                LayoutHorizontal();
            }
            else
            {
                LayoutVertical();
            }

            return CommonProperties.GetAutoSize(container);
        }

        private bool LayoutHorizontal()
        {
            ToolStrip toolStrip = ToolStrip;
            Rectangle clientRectangle = toolStrip.ClientRectangle;
#if DEBUG
            if (DebugLayoutTraceSwitch.TraceVerbose)
            {
                Debug.WriteLine("_________________________\r\nHorizontal Layout:" + toolStrip.ToString() + displayRectangle.ToString());
            }
#endif

            int lastRight = displayRectangle.Right;
            int lastLeft = displayRectangle.Left;
            bool needsMoreSpace = false;
            Size itemSize = Size.Empty;
            Rectangle alignedLeftItems = Rectangle.Empty;
            Rectangle alignedRightItems = Rectangle.Empty;

            // this will determine where the item should be placed.
            CalculatePlacementsHorizontal();

            bool needOverflow = toolStrip.CanOverflow && ((OverflowRequired) || (OverflowSpace >= OverflowButtonSize.Width));
            toolStrip.OverflowButton.Visible = needOverflow;

            // if we require the overflow, it should stick up against the edge of the toolstrip.
            if (needOverflow)
            {
                if (toolStrip.RightToLeft == RightToLeft.No)
                {
                    lastRight = clientRectangle.Right;
                }
                else
                {
                    lastLeft = clientRectangle.Left;
                }
            }

            for (int j = -1; j < toolStrip.Items.Count; j++)
            {
                ToolStripItem item = null;

                if (j == -1)
                {
                    // the first time through place the overflow button if its required.
                    if (needOverflow)
                    {
                        item = toolStrip.OverflowButton;
                        item.SetPlacement(ToolStripItemPlacement.Main);
                        itemSize = OverflowButtonSize;
                    }
                    else
                    {
                        item = toolStrip.OverflowButton;
                        item.SetPlacement(ToolStripItemPlacement.None);
                        continue;
                    }
                }
                else
                {
                    item = toolStrip.Items[j];

                    if (!((IArrangedElement)item).ParticipatesInLayout)
                    {
                        // skip over items not participating in layout.  E.G. not visible items
                        continue;
                    }

                    // since we havent parented the item yet - the auto size wont have reset the size yet.
                    itemSize = item.AutoSize ? item.GetPreferredSize(Size.Empty) : item.Size;
                }

                // if it turns out we dont need the overflow (because there are no Overflow.Always items and the width of everything
                // in the overflow is less than the width of the overflow button then reset the placement of the as needed items to
                // main.
                if (!needOverflow && (item.Overflow == ToolStripItemOverflow.AsNeeded && item.Placement == ToolStripItemPlacement.Overflow))
                {
#if DEBUG
                    if (DebugLayoutTraceSwitch.TraceVerbose)
                    { Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Resetting {0} to Main - we dont need it to overflow", item)); }
#endif
                    item.SetPlacement(ToolStripItemPlacement.Main);
                }

                // Now do the guts of setting X, Y and parenting.
                // We need to honor left to right and head and tail.
                //      In RTL.Yes, Head is to the Right, Tail is to the Left
                //      In RTL.No,  Head is to the Left,  Tail is to the Right
                if ((item != null) && (item.Placement == ToolStripItemPlacement.Main))
                {
                    int x = displayRectangle.Left;
                    int y = displayRectangle.Top;
                    Padding itemMargin = item.Margin;

                    if (((item.Alignment == ToolStripItemAlignment.Right) && (toolStrip.RightToLeft == RightToLeft.No)) || ((item.Alignment == ToolStripItemAlignment.Left) && (toolStrip.RightToLeft == RightToLeft.Yes)))
                    {
                        //                  lastRight   x     Margin.Right
                        //             [Item]<----------[Item]----------->|
                        //                   Margin.Left
                        // this item should be placed to the right
                        // we work backwards from the right edge - that is place items from right to left.
                        x = lastRight - (itemMargin.Right + itemSize.Width);
                        y += itemMargin.Top;
                        lastRight = x - itemMargin.Left;
                        alignedRightItems = (alignedRightItems == Rectangle.Empty) ? new Rectangle(x, y, itemSize.Width, itemSize.Height)
                                                : Rectangle.Union(alignedRightItems, new Rectangle(x, y, itemSize.Width, itemSize.Height));

                    }
                    else
                    {
                        //             x     Margin.Right lastLeft
                        // |<----------[Item]------------>|
                        //  Margin.Left
                        // this item should be placed to the left
                        // we work forwards from the left - that is place items from left to right
                        x = lastLeft + itemMargin.Left;
                        y += itemMargin.Top;
                        lastLeft = x + itemSize.Width + itemMargin.Right;
                        alignedLeftItems = (alignedLeftItems == Rectangle.Empty) ? new Rectangle(x, y, itemSize.Width, itemSize.Height)
                                                : Rectangle.Union(alignedLeftItems, new Rectangle(x, y, itemSize.Width, itemSize.Height));

                    }

                    item.ParentInternal = ToolStrip;

                    Point itemLocation = new Point(x, y);
                    if (!clientRectangle.Contains(x, y))
                    {
                        item.SetPlacement(ToolStripItemPlacement.None);
                    }
                    else if (alignedRightItems.Width > 0 && alignedLeftItems.Width > 0 && alignedRightItems.IntersectsWith(alignedLeftItems))
                    {
                        itemLocation = noMansLand;
                        item.SetPlacement(ToolStripItemPlacement.None);
                    }

                    if (item.AutoSize)
                    {
                        // autosized items stretch from edge-edge
                        itemSize.Height = Math.Max(displayRectangle.Height - itemMargin.Vertical, 0);
                    }
                    else
                    {
                        // non autosized items are vertically centered
                        Rectangle bounds = LayoutUtils.VAlign(item.Size, displayRectangle, AnchorStyles.None);
                        itemLocation.Y = bounds.Y;
                    }

                    SetItemLocation(item, itemLocation, itemSize);
                }
                else
                {
                    item.ParentInternal = (item.Placement == ToolStripItemPlacement.Overflow) ? toolStrip.OverflowButton.DropDown : null;
                }
#if DEBUG
                if (DebugLayoutTraceSwitch.TraceVerbose)
                {
                    Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Item {0} Placement {1} Bounds {2} Parent {3}", item.ToString(), item.Placement.ToString(), item.Bounds.ToString(), (item.ParentInternal == null) ? "null" : item.ParentInternal.ToString()));
                }
#endif
            }

            return needsMoreSpace;
        }

        private bool LayoutVertical()
        {
#if DEBUG
            if (DebugLayoutTraceSwitch.TraceVerbose)
            {
                Debug.WriteLine("_________________________\r\nVertical Layout" + displayRectangle.ToString());
            }
#endif

            ToolStrip toolStrip = ToolStrip;
            Rectangle clientRectangle = toolStrip.ClientRectangle;
            int lastBottom = displayRectangle.Bottom;
            int lastTop = displayRectangle.Top;
            bool needsMoreSpace = false;
            Size itemSize = Size.Empty;
            Rectangle alignedLeftItems = Rectangle.Empty;
            Rectangle alignedRightItems = Rectangle.Empty;

            Size toolStripPreferredSize = displayRectangle.Size;
            DockStyle dock = toolStrip.Dock;
            if (toolStrip.AutoSize && (!toolStrip.IsInToolStripPanel && (dock == DockStyle.Left) || (dock == DockStyle.Right)))
            {
                // if we're autosizing, make sure we pad out items to the preferred width, not the
                // width of the display rectangle.
                toolStripPreferredSize = ToolStrip.GetPreferredSizeVertical(toolStrip, Size.Empty) - toolStrip.Padding.Size;
            }

            CalculatePlacementsVertical();

            bool needOverflow = toolStrip.CanOverflow && ((OverflowRequired) || (OverflowSpace >= OverflowButtonSize.Height));

            toolStrip.OverflowButton.Visible = needOverflow;

            for (int j = -1; j < ToolStrip.Items.Count; j++)
            {
                ToolStripItem item = null;

                if (j == -1)
                {
                    // the first time through place the overflow button if its required.
                    if (needOverflow)
                    {
                        item = toolStrip.OverflowButton;
                        item.SetPlacement(ToolStripItemPlacement.Main);
                    }
                    else
                    {
                        item = toolStrip.OverflowButton;
                        item.SetPlacement(ToolStripItemPlacement.None);
                        continue;
                    }

                    itemSize = OverflowButtonSize;
                }
                else
                {
                    item = toolStrip.Items[j];
                    if (!((IArrangedElement)item).ParticipatesInLayout)
                    {
                        // skip over items not participating in layout.  E.G. not visible items
                        continue;
                    }

                    // since we havent parented the item yet - the auto size wont have reset the size yet.
                    itemSize = item.AutoSize ? item.GetPreferredSize(Size.Empty) : item.Size;

                }

                // if it turns out we dont need the overflow (because there are no Overflow.Always items and the height of everything
                // in the overflow is less than the width of the overflow button then reset the placement of the as needed items to
                // main.
                if (!needOverflow && (item.Overflow == ToolStripItemOverflow.AsNeeded && item.Placement == ToolStripItemPlacement.Overflow))
                {
#if DEBUG
                    if (DebugLayoutTraceSwitch.TraceVerbose)
                    {
                        Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Resetting {0} to Main - we dont need it to overflow", item));
                    }
#endif
                    item.SetPlacement(ToolStripItemPlacement.Main);
                }

                // Now do the guts of setting X, Y and parenting.
                // Vertical split stack management ignores left to right.
                //      Items aligned to the Head are placed from Top to Bottom
                //      Items aligned to the Tail are placed from Bottom to Top
                if ((item != null) && (item.Placement == ToolStripItemPlacement.Main))
                {
                    Padding itemMargin = item.Margin;
                    int x = displayRectangle.Left + itemMargin.Left;
                    int y = displayRectangle.Top;

                    switch (item.Alignment)
                    {
                        case ToolStripItemAlignment.Right:
                            y = lastBottom - (itemMargin.Bottom + itemSize.Height);
                            lastBottom = y - itemMargin.Top;
                            alignedRightItems = (alignedRightItems == Rectangle.Empty) ? new Rectangle(x, y, itemSize.Width, itemSize.Height)
                                                : Rectangle.Union(alignedRightItems, new Rectangle(x, y, itemSize.Width, itemSize.Height));
                            break;

                        case ToolStripItemAlignment.Left:
                        default:
                            y = lastTop + itemMargin.Top;
                            lastTop = y + itemSize.Height + itemMargin.Bottom;
                            alignedLeftItems = (alignedLeftItems == Rectangle.Empty) ? new Rectangle(x, y, itemSize.Width, itemSize.Height)
                                                    : Rectangle.Union(alignedLeftItems, new Rectangle(x, y, itemSize.Width, itemSize.Height));
                            break;
                    }
                    item.ParentInternal = ToolStrip;
                    Point itemLocation = new Point(x, y);

                    if (!clientRectangle.Contains(x, y))
                    {
                        item.SetPlacement(ToolStripItemPlacement.None);
                    }
                    else if (alignedRightItems.Width > 0 && alignedLeftItems.Width > 0 && alignedRightItems.IntersectsWith(alignedLeftItems))
                    {
                        itemLocation = noMansLand;
                        item.SetPlacement(ToolStripItemPlacement.None);
                    }

                    if (item.AutoSize)
                    {
                        // autosized items stretch from edge-edge
                        itemSize.Width = Math.Max(toolStripPreferredSize.Width - itemMargin.Horizontal - 1, 0);
                    }
                    else
                    {
                        // non autosized items are horizontally centered
                        Rectangle bounds = LayoutUtils.HAlign(item.Size, displayRectangle, AnchorStyles.None);
                        itemLocation.X = bounds.X;
                    }

                    SetItemLocation(item, itemLocation, itemSize);
                }
                else
                {
                    item.ParentInternal = (item.Placement == ToolStripItemPlacement.Overflow) ? toolStrip.OverflowButton.DropDown : null;
                }
#if DEBUG
                if (DebugLayoutTraceSwitch.TraceVerbose)
                {
                    Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Item {0} Placement {1} Bounds {2} Parent {3}", item.ToString(), item.Placement.ToString(), item.Bounds.ToString(), (item.ParentInternal == null) ? "null" : item.ParentInternal.ToString()));
                }
#endif
            }

            return needsMoreSpace;
        }

        private void SetItemLocation(ToolStripItem item, Point itemLocation, Size itemSize)
        {
            // make sure that things that dont fit within the display rectangle arent laid out.
            if ((item.Placement == ToolStripItemPlacement.Main) && !(item is ToolStripOverflowButton))
            {
                // overflow buttons can be placed outside the display rect.
                bool horizontal = (ToolStrip.LayoutStyle == ToolStripLayoutStyle.HorizontalStackWithOverflow);
                Rectangle displayRect = displayRectangle;
                Rectangle itemBounds = new Rectangle(itemLocation, itemSize);

                // in horizontal if something bleeds over the top/bottom that's ok - its left/right we care about
                // same in vertical.
                if (horizontal)
                {
                    if ((itemBounds.Right > displayRectangle.Right) || (itemBounds.Left < displayRectangle.Left))
                    {
#if DEBUG
                        if (DebugLayoutTraceSwitch.TraceVerbose)
                        {
                            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "[SplitStack.SetItemLocation] Sending Item {0} to NoMansLand as it doesnt fit horizontally within the DRect", item));
                        }
#endif
                        itemLocation = noMansLand;
                        item.SetPlacement(ToolStripItemPlacement.None);
                    }

                }
                else
                {
                    if ((itemBounds.Bottom > displayRectangle.Bottom) || (itemBounds.Top < displayRectangle.Top))
                    {
#if DEBUG
                        if (DebugLayoutTraceSwitch.TraceVerbose)
                        {
                            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "[SplitStack.SetItemLocation] Sending Item {0} to NoMansLand as it doesnt fit verticallu within the DRect", item));
                        }
#endif

                        itemLocation = noMansLand;
                        item.SetPlacement(ToolStripItemPlacement.None);
                    }
                }
            }

            item.SetBounds(new Rectangle(itemLocation, itemSize));
        }

        private void PlaceItems()
        {
            ToolStrip toolStrip = ToolStrip;
            for (int i = 0; i < toolStrip.Items.Count; i++)
            {
                ToolStripItem item = toolStrip.Items[i];
                // if we havent placed the items, place them now.
                if (item.Placement == ToolStripItemPlacement.None)
                {
                    if (item.Overflow != ToolStripItemOverflow.Always)
                    {
                        // as needed items will have already been placed into the overflow if they
                        // needed to move over.
                        item.SetPlacement(ToolStripItemPlacement.Main);
                    }
                    else
                    {
                        item.SetPlacement(ToolStripItemPlacement.Overflow);
                    }
                }
            }
        }

        private void ResetItemPlacements()
        {
            ToolStrip toolStrip = ToolStrip;

            for (int i = 0; i < toolStrip.Items.Count; i++)
            {
                if (toolStrip.Items[i].Placement == ToolStripItemPlacement.Overflow)
                {
                    toolStrip.Items[i].ParentInternal = null;
                }

                toolStrip.Items[i].SetPlacement(ToolStripItemPlacement.None);
            }
        }

        /// <summary>
        ///  This method is called when we are walking through the item collection and we have realized that we
        ///  need to free up "X" amount of space to be able to fit an item onto the ToolStrip.
        /// </summary>
        private int SendNextItemToOverflow(int spaceNeeded, bool horizontal)
        {
#if DEBUG
            if (DebugLayoutTraceSwitch.TraceVerbose)
            {
                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "SendNextItemToOverflow attempting to free {0}", spaceNeeded));
            }
#endif
            Debug.Indent();

            int freedSpace = 0;
            int backIndex = BackwardsWalkingIndex;

            BackwardsWalkingIndex = (backIndex == -1) ? ToolStrip.Items.Count - 1 : backIndex - 1;
            for (; BackwardsWalkingIndex >= 0; BackwardsWalkingIndex--)
            {
                ToolStripItem item = ToolStrip.Items[BackwardsWalkingIndex];
                if (!((IArrangedElement)item).ParticipatesInLayout)
                {
                    // skip over items not participating in layout.  E.G. not visible items
                    continue;
                }

                Padding itemMargin = item.Margin;

                // look for items that say they're ok for overflowing.
                // not looking at ones that Always overflow - as the forward walker already skips these.
                if (item.Overflow == ToolStripItemOverflow.AsNeeded && item.Placement != ToolStripItemPlacement.Overflow)
                {
#if DEBUG
                    if (DebugLayoutTraceSwitch.TraceVerbose)
                    {
                        Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Found candidate for sending to overflow {0}", item.ToString()));
                    }
#endif

                    // since we havent parented the item yet - the auto size wont have reset the size yet.
                    Size itemSize = item.AutoSize ? item.GetPreferredSize(displayRectangle.Size) : item.Size;

                    if (BackwardsWalkingIndex <= ForwardsWalkingIndex)
                    {
                        // we've found an item that the forwards walking guy has already marched past,
                        // we need to let him know how much space we're freeing by sending this guy over
                        // to the overflow.
                        freedSpace += (horizontal) ? itemSize.Width + itemMargin.Horizontal : itemSize.Height + itemMargin.Vertical;
#if DEBUG
                        if (DebugLayoutTraceSwitch.TraceVerbose)
                        {
                            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Sweet! {0} FreedSpace - which is now {1}", itemSize, freedSpace.ToString(CultureInfo.InvariantCulture)));
                        }
#endif
                    }

                    // send the item to the overflow.
                    item.SetPlacement(ToolStripItemPlacement.Overflow);
                    if (OverflowRequired == false)
                    {
                        // this is the first item we're sending down.
                        // we now need to account for the width or height of the overflow button
                        spaceNeeded += (horizontal) ? OverflowButtonSize.Width : OverflowButtonSize.Height;
#if DEBUG
                        if (DebugLayoutTraceSwitch.TraceVerbose)
                        { Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Turns out we now need an overflow button, space needed now: {0}", spaceNeeded.ToString(CultureInfo.InvariantCulture))); }
#endif
                        OverflowRequired = true;
                    }

                    OverflowSpace += (horizontal) ? itemSize.Width + itemMargin.Horizontal : itemSize.Height + itemMargin.Vertical;
                }

                if (freedSpace > spaceNeeded)
                {
                    break;
                }
            }

            Debug.Unindent();
            return freedSpace;
        }
    }
}
