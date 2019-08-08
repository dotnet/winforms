// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms.Layout
{
    internal class DefaultLayout : LayoutEngine
    {
        internal static readonly DefaultLayout Instance = new DefaultLayout();

        private static readonly int s_layoutInfoProperty = PropertyStore.CreateKey();
        private static readonly int s_cachedBoundsProperty = PropertyStore.CreateKey();

        /// <summary>
        ///  Loop through the AutoSized controls and expand them if they are smaller than
        ///  their preferred size. If expanding the controls causes overlap, bump the overlapped
        ///  control if it is AutoRelocatable.
        /// </summary>
        private static void LayoutAutoSizedControls(IArrangedElement container)
        {
            ArrangedElementCollection children = container.Children;
            for (int i = children.Count - 1; i >= 0; i--)
            {
                IArrangedElement element = children[i];
                if (CommonProperties.xGetAutoSizedAndAnchored(element))
                {
                    Rectangle bounds = GetCachedBounds(element);

                    AnchorStyles anchor = GetAnchor(element);
                    Size proposedConstraints = LayoutUtils.MaxSize;

                    if ((anchor & (AnchorStyles.Left | AnchorStyles.Right)) == (AnchorStyles.Left | AnchorStyles.Right))
                    {
                        proposedConstraints.Width = bounds.Width;
                    }
                    if ((anchor & (AnchorStyles.Top | AnchorStyles.Bottom)) == (AnchorStyles.Top | AnchorStyles.Bottom))
                    {
                        proposedConstraints.Height = bounds.Height;
                    }

                    Size prefSize = element.GetPreferredSize(proposedConstraints);
                    Rectangle newBounds = bounds;
                    if (CommonProperties.GetAutoSizeMode(element) == AutoSizeMode.GrowAndShrink)
                    {
                        // this is the case for simple things like radio button, checkbox, etc.
                        newBounds = GetGrowthBounds(element, prefSize);
                    }
                    else
                    {
                        // we had whacked this check, but it turns out it causes undesirable
                        // behavior in things like panel. a panel with no elements sizes to 0,0.
                        if (bounds.Width < prefSize.Width || bounds.Height < prefSize.Height)
                        {
                            Size newSize = LayoutUtils.UnionSizes(bounds.Size, prefSize);
                            newBounds = GetGrowthBounds(element, newSize);
                        }
                    }

                    if (newBounds != bounds)
                    {
                        SetCachedBounds(element, newBounds);
                    }
                }
            }
        }

        /// <summary>
        ///  Gets the bounds of the element after growing to newSize (note that depending on
        ///  anchoring the element may grow to the left/updwards rather than to the
        ///  right/downwards. i.e., it may be translated.)
        /// </summary>
        private static Rectangle GetGrowthBounds(IArrangedElement element, Size newSize)
        {
            GrowthDirection direction = GetGrowthDirection(element);
            Rectangle oldBounds = GetCachedBounds(element);
            Point location = oldBounds.Location;

            Debug.Assert((CommonProperties.GetAutoSizeMode(element) == AutoSizeMode.GrowAndShrink || newSize.Height >= oldBounds.Height && newSize.Width >= oldBounds.Width),
                "newSize expected to be >= current size.");

            if ((direction & GrowthDirection.Left) != GrowthDirection.None)
            {
                // We are growing towards the left, translate X
                location.X -= newSize.Width - oldBounds.Width;
            }

            if ((direction & GrowthDirection.Upward) != GrowthDirection.None)
            {
                // We are growing towards the top, translate Y
                location.Y -= newSize.Height - oldBounds.Height;
            }

            Rectangle newBounds = new Rectangle(location, newSize);

            Debug.Assert((CommonProperties.GetAutoSizeMode(element) == AutoSizeMode.GrowAndShrink || newBounds.Contains(oldBounds)), "How did we resize in such a way we no longer contain our old bounds?");

            return newBounds;
        }

        /// <summary>
        ///  Examines an elements anchoring to figure out which direction it should grow.
        /// </summary>
        private static GrowthDirection GetGrowthDirection(IArrangedElement element)
        {
            AnchorStyles anchor = GetAnchor(element);
            GrowthDirection growthDirection = GrowthDirection.None;

            if ((anchor & AnchorStyles.Right) != AnchorStyles.None
                && (anchor & AnchorStyles.Left) == AnchorStyles.None)
            {
                // element is anchored to the right, but not the left.
                growthDirection |= GrowthDirection.Left;
            }
            else
            {
                // otherwise we grow towards the right (common case)
                growthDirection |= GrowthDirection.Right;
            }

            if ((anchor & AnchorStyles.Bottom) != AnchorStyles.None
                && (anchor & AnchorStyles.Top) == AnchorStyles.None)
            {
                // element is anchored to the bottom, but not the top.
                growthDirection |= GrowthDirection.Upward;
            }
            else
            {
                // otherwise we grow towards the bottom. (common case)
                growthDirection |= GrowthDirection.Downward;
            }

            Debug.Assert((growthDirection & GrowthDirection.Left) == GrowthDirection.None
                || (growthDirection & GrowthDirection.Right) == GrowthDirection.None,
                "We shouldn't allow growth to both the left and right.");
            Debug.Assert((growthDirection & GrowthDirection.Upward) == GrowthDirection.None
                || (growthDirection & GrowthDirection.Downward) == GrowthDirection.None,
                "We shouldn't allow both upward and downward growth.");
            return growthDirection;
        }

        /// <summary>
        ///  Layout for a single anchored control. There's no order dependency when laying out anchored controls.
        /// </summary>
        private static Rectangle GetAnchorDestination(IArrangedElement element, Rectangle displayRect, bool measureOnly)
        {
            // Container can not be null since we AschorControls takes a non-null container.
            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "\t\t'" + element + "' is anchored at " + GetCachedBounds(element).ToString());

            AnchorInfo layout = GetAnchorInfo(element);

            int left = layout.Left + displayRect.X;
            int top = layout.Top + displayRect.Y;
            int right = layout.Right + displayRect.X;
            int bottom = layout.Bottom + displayRect.Y;

            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "\t\t...anchor dim (l,t,r,b) {"
                              + (left)
                              + ", " + (top)
                              + ", " + (right)
                              + ", " + (bottom)
                              + "}");

            AnchorStyles anchor = GetAnchor(element);

            if (IsAnchored(anchor, AnchorStyles.Right))
            {
                Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "\t\t...adjusting right");
                right += displayRect.Width;

                if (!IsAnchored(anchor, AnchorStyles.Left))
                {
                    Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "\t\t...adjusting left");
                    left += displayRect.Width;
                }
            }
            else if (!IsAnchored(anchor, AnchorStyles.Left))
            {
                Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "\t\t...adjusting left & right");
                right += (displayRect.Width / 2);
                left += (displayRect.Width / 2);
            }

            if (IsAnchored(anchor, AnchorStyles.Bottom))
            {
                Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "\t\t...adjusting bottom");
                bottom += displayRect.Height;

                if (!IsAnchored(anchor, AnchorStyles.Top))
                {
                    Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "\t\t...adjusting top");
                    top += displayRect.Height;
                }
            }
            else if (!IsAnchored(anchor, AnchorStyles.Top))
            {
                Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "\t\t...adjusting top & bottom");
                bottom += (displayRect.Height / 2);
                top += (displayRect.Height / 2);
            }

            if (!measureOnly)
            {
                // the size is actually zero, set the width and heights appropriately.
                if (right < left)
                {
                    right = left;
                }

                if (bottom < top)
                {
                    bottom = top;
                }
            }
            else
            {
                Rectangle cachedBounds = GetCachedBounds(element);
                // in this scenario we've likely been passed a 0 sized display rectangle to determine our height.
                // we will need to translate the right and bottom edges as necessary to the positive plane.

                // right < left means the control is anchored both left and right.
                // cachedBounds != element.Bounds means  the element's size has changed
                // any, all, or none of these can be true.
                if (right < left || cachedBounds.Width != element.Bounds.Width || cachedBounds.X != element.Bounds.X)
                {
                    if (cachedBounds != element.Bounds)
                    {
                        left = Math.Max(Math.Abs(left), Math.Abs(cachedBounds.Left));
                    }
                    right = left + Math.Max(element.Bounds.Width, cachedBounds.Width) + Math.Abs(right);
                }
                else
                {
                    left = left > 0 ? left : element.Bounds.Left;
                    right = right > 0 ? right : element.Bounds.Right + Math.Abs(right);
                }

                // bottom < top means the control is anchored both top and bottom.
                // cachedBounds != element.Bounds means  the element's size has changed
                // any, all, or none of these can be true.
                if (bottom < top || cachedBounds.Height != element.Bounds.Height || cachedBounds.Y != element.Bounds.Y)
                {
                    if (cachedBounds != element.Bounds)
                    {
                        top = Math.Max(Math.Abs(top), Math.Abs(cachedBounds.Top));
                    }
                    bottom = top + Math.Max(element.Bounds.Height, cachedBounds.Height) + Math.Abs(bottom);
                }
                else
                {
                    top = top > 0 ? top : element.Bounds.Top;
                    bottom = bottom > 0 ? bottom : element.Bounds.Bottom + Math.Abs(bottom);
                }
            }

            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "\t\t...new anchor dim (l,t,r,b) {"
                                                                      + (left)
                                                                      + ", " + (top)
                                                                      + ", " + (right)
                                                                      + ", " + (bottom)
                                                                      + "}");

            return new Rectangle(left, top, right - left, bottom - top);
        }

        private static void LayoutAnchoredControls(IArrangedElement container)
        {
            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "\tAnchor Processing");
            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "\t\tdisplayRect: " + container.DisplayRectangle.ToString());

            Rectangle displayRectangle = container.DisplayRectangle;
            if (CommonProperties.GetAutoSize(container) && ((displayRectangle.Width == 0) || (displayRectangle.Height == 0)))
            {
                // we havent set oursleves to the preferred size yet. proceeding will
                // just set all the control widths to zero. let's return here
                return;
            }

            ArrangedElementCollection children = container.Children;
            for (int i = children.Count - 1; i >= 0; i--)
            {
                IArrangedElement element = children[i];
                if (CommonProperties.GetNeedsAnchorLayout(element))
                {
                    Debug.Assert(GetAnchorInfo(element) != null, "AnchorInfo should be initialized before LayoutAnchorControls().");
                    SetCachedBounds(element, GetAnchorDestination(element, displayRectangle, /*measureOnly=*/false));
                }
            }
        }

        private static Size LayoutDockedControls(IArrangedElement container, bool measureOnly)
        {
            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "\tDock Processing");
            Debug.Assert(!HasCachedBounds(container), "Do not call this method with an active cached bounds list.");

            // If measuring, we start with an empty rectangle and add as needed.
            // If doing actual layout, we start with the container's rect and subtract as we layout.
            Rectangle remainingBounds = measureOnly ? Rectangle.Empty : container.DisplayRectangle;
            Size preferredSize = Size.Empty;

            IArrangedElement mdiClient = null;

            // Docking layout is order dependent. After much debate, we decided to use z-order as the
            // docking order. (Introducing a DockOrder property was a close second)
            ArrangedElementCollection children = container.Children;
            for (int i = children.Count - 1; i >= 0; i--)
            {
                IArrangedElement element = children[i];
                Debug.Assert(element.Bounds == GetCachedBounds(element), "Why do we have cachedBounds for a docked element?");
                if (CommonProperties.GetNeedsDockLayout(element))
                {
                    // Some controls modify their bounds when you call SetBoundsCore. We
                    // therefore need to read the value of bounds back when adjusting our layout rectangle.
                    switch (GetDock(element))
                    {
                        case DockStyle.Top:
                            {
                                Size elementSize = GetVerticalDockedSize(element, remainingBounds.Size, measureOnly);
                                Rectangle newElementBounds = new Rectangle(remainingBounds.X, remainingBounds.Y, elementSize.Width, elementSize.Height);

                                TryCalculatePreferredSizeDockedControl(element, newElementBounds, measureOnly, ref preferredSize, ref remainingBounds);

                                // What we are really doing here: top += element.Bounds.Height;
                                remainingBounds.Y += element.Bounds.Height;
                                remainingBounds.Height -= element.Bounds.Height;
                                break;
                            }
                        case DockStyle.Bottom:
                            {
                                Size elementSize = GetVerticalDockedSize(element, remainingBounds.Size, measureOnly);
                                Rectangle newElementBounds = new Rectangle(remainingBounds.X, remainingBounds.Bottom - elementSize.Height, elementSize.Width, elementSize.Height);

                                TryCalculatePreferredSizeDockedControl(element, newElementBounds, measureOnly, ref preferredSize, ref remainingBounds);

                                // What we are really doing here: bottom -= element.Bounds.Height;
                                remainingBounds.Height -= element.Bounds.Height;

                                break;
                            }
                        case DockStyle.Left:
                            {
                                Size elementSize = GetHorizontalDockedSize(element, remainingBounds.Size, measureOnly);
                                Rectangle newElementBounds = new Rectangle(remainingBounds.X, remainingBounds.Y, elementSize.Width, elementSize.Height);

                                TryCalculatePreferredSizeDockedControl(element, newElementBounds, measureOnly, ref preferredSize, ref remainingBounds);

                                // What we are really doing here: left += element.Bounds.Width;
                                remainingBounds.X += element.Bounds.Width;
                                remainingBounds.Width -= element.Bounds.Width;
                                break;
                            }
                        case DockStyle.Right:
                            {
                                Size elementSize = GetHorizontalDockedSize(element, remainingBounds.Size, measureOnly);
                                Rectangle newElementBounds = new Rectangle(remainingBounds.Right - elementSize.Width, remainingBounds.Y, elementSize.Width, elementSize.Height);

                                TryCalculatePreferredSizeDockedControl(element, newElementBounds, measureOnly, ref preferredSize, ref remainingBounds);

                                // What we are really doing here: right -= element.Bounds.Width;
                                remainingBounds.Width -= element.Bounds.Width;
                                break;
                            }
                        case DockStyle.Fill:
                            if (element is MdiClient)
                            {
                                Debug.Assert(mdiClient == null, "How did we end up with multiple MdiClients?");
                                mdiClient = element;
                            }
                            else
                            {
                                Size elementSize = remainingBounds.Size;
                                Rectangle newElementBounds = new Rectangle(remainingBounds.X, remainingBounds.Y, elementSize.Width, elementSize.Height);

                                TryCalculatePreferredSizeDockedControl(element, newElementBounds, measureOnly, ref preferredSize, ref remainingBounds);
                            }
                            break;
                        default:
                            Debug.Fail("Unsupported value for dock.");
                            break;
                    }
                }

                // Treat the MDI client specially, since it's supposed to blend in with the parent form
                if (mdiClient != null)
                {
                    SetCachedBounds(mdiClient, remainingBounds);
                }
            }

            return preferredSize;
        }

        /// <summary>
        ///  Helper method that either sets the element bounds or does the preferredSize computation based on
        ///  the value of measureOnly.
        /// </summary>
        private static void TryCalculatePreferredSizeDockedControl(IArrangedElement element, Rectangle newElementBounds, bool measureOnly, ref Size preferredSize, ref Rectangle remainingBounds)
        {
            if (measureOnly)
            {
                Size neededSize = new Size(
                    Math.Max(0, newElementBounds.Width - remainingBounds.Width),
                    Math.Max(0, newElementBounds.Height - remainingBounds.Height));

                DockStyle dockStyle = GetDock(element);
                if ((dockStyle == DockStyle.Top) || (dockStyle == DockStyle.Bottom))
                {
                    neededSize.Width = 0;
                }
                if ((dockStyle == DockStyle.Left) || (dockStyle == DockStyle.Right))
                {
                    neededSize.Height = 0;
                }
                if (dockStyle != DockStyle.Fill)
                {
                    preferredSize += neededSize;
                    remainingBounds.Size += neededSize;
                }
                else if (dockStyle == DockStyle.Fill && CommonProperties.GetAutoSize(element))
                {
                    Size elementPrefSize = element.GetPreferredSize(neededSize);
                    remainingBounds.Size += elementPrefSize;
                    preferredSize += elementPrefSize;
                }
            }
            else
            {
                element.SetBounds(newElementBounds, BoundsSpecified.None);

#if DEBUG
                Control control = element as Control;
                newElementBounds.Size = control.ApplySizeConstraints(newElementBounds.Size);

                // This usually happens when a Control overrides its SetBoundsCore or sets size during OnResize
                // to enforce constraints like AutoSize. Generally you can just move this code to Control.GetAdjustedSize
                // and then PreferredSize will also pick up these constraints. See ComboBox as an example.
                if (CommonProperties.GetAutoSize(element) && !CommonProperties.GetSelfAutoSizeInDefaultLayout(element))
                {
                    Debug.Assert(
                        (newElementBounds.Width < 0 || element.Bounds.Width == newElementBounds.Width) &&
                        (newElementBounds.Height < 0 || element.Bounds.Height == newElementBounds.Height),
                        "Element modified its bounds during docking -- PreferredSize will be wrong. See comment near this assert.");
                }
#endif
            }
        }

        private static Size GetVerticalDockedSize(IArrangedElement element, Size remainingSize, bool measureOnly)
        {
            Size newSize = xGetDockedSize(element, remainingSize, /* constraints = */ new Size(remainingSize.Width, 1), measureOnly);
            if (!measureOnly)
            {
                newSize.Width = remainingSize.Width;
            }
            else
            {
                newSize.Width = Math.Max(newSize.Width, remainingSize.Width);
            }

            Debug.Assert((measureOnly && (newSize.Width >= remainingSize.Width)) || (newSize.Width == remainingSize.Width),
                "Error detected in GetVerticalDockedSize: Dock size computed incorrectly during layout.");
            return newSize;
        }

        private static Size GetHorizontalDockedSize(IArrangedElement element, Size remainingSize, bool measureOnly)
        {
            Size newSize = xGetDockedSize(element, remainingSize, /* constraints = */ new Size(1, remainingSize.Height), measureOnly);
            if (!measureOnly)
            {
                newSize.Height = remainingSize.Height;
            }
            else
            {
                newSize.Height = Math.Max(newSize.Height, remainingSize.Height);
            }

            Debug.Assert((measureOnly && (newSize.Height >= remainingSize.Height)) || (newSize.Height == remainingSize.Height),
                "Error detected in GetHorizontalDockedSize: Dock size computed incorrectly during layout.");
            return newSize;
        }

        private static Size xGetDockedSize(IArrangedElement element, Size remainingSize, Size constraints, bool measureOnly)
        {
            Size desiredSize;
            if (CommonProperties.GetAutoSize(element))
            {
                // Ask control for its desired size using the provided constraints.
                // (e.g., a control docked to top will constrain width to remaining width
                // and minimize height.)
                desiredSize = element.GetPreferredSize(constraints);
            }
            else
            {
                desiredSize = element.Bounds.Size;
            }

            Debug.Assert((desiredSize.Width >= 0 && desiredSize.Height >= 0), "Error detected in xGetDockSize: Element size was negative.");
            return desiredSize;
        }

        private protected override bool LayoutCore(IArrangedElement container, LayoutEventArgs args)
        {
            return TryCalculatePreferredSize(container, measureOnly: false, preferredSize: out Size _);
        }

        /// <remarks>
        ///  PreferredSize is only computed if measureOnly = true.
        /// </remarks>
        private static bool TryCalculatePreferredSize(IArrangedElement container, bool measureOnly, out Size preferredSize)
        {
            ArrangedElementCollection children = container.Children;
            // PreferredSize is garbage unless measureOnly is specified
            preferredSize = new Size(-7103, -7105);

            // Short circuit for items with no children
            if (!measureOnly && children.Count == 0)
            {
                return CommonProperties.GetAutoSize(container);
            }

            bool dock = false;
            bool anchor = false;
            bool autoSize = false;
            for (int i = children.Count - 1; i >= 0; i--)
            {
                IArrangedElement element = children[i];
                if (CommonProperties.GetNeedsDockAndAnchorLayout(element))
                {
                    if (!dock && CommonProperties.GetNeedsDockLayout(element))
                    {
                        dock = true;
                    }

                    if (!anchor && CommonProperties.GetNeedsAnchorLayout(element))
                    {
                        anchor = true;
                    }

                    if (!autoSize && CommonProperties.xGetAutoSizedAndAnchored(element))
                    {
                        autoSize = true;
                    }
                }
            }

            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "\tanchor : " + anchor.ToString());
            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "\tdock :   " + dock.ToString());

            Size preferredSizeForDocking = Size.Empty;
            Size preferredSizeForAnchoring = Size.Empty;

            if (dock)
            {
                preferredSizeForDocking = LayoutDockedControls(container, measureOnly);
            }

            if (anchor && !measureOnly)
            {
                // In the case of anchor, where we currently are defines the preferred size,
                // so dont recalculate the positions of everything.
                LayoutAnchoredControls(container);
            }

            if (autoSize)
            {
                LayoutAutoSizedControls(container);
            }

            if (!measureOnly)
            {
                // Set the anchored controls to their computed positions.
                ApplyCachedBounds(container);
            }
            else
            {
                // Finish the preferredSize computation and clear cached anchored positions.
                preferredSizeForAnchoring = GetAnchorPreferredSize(container);

                Padding containerPadding = Padding.Empty;
                if (container is Control control)
                {
                    // Calling this will respect Control.DefaultPadding.
                    containerPadding = control.Padding;
                }
                else
                {
                    // Not likely to happen but handle this gracefully.
                    containerPadding = CommonProperties.GetPadding(container, Padding.Empty);
                }

                preferredSizeForAnchoring.Width -= containerPadding.Left;
                preferredSizeForAnchoring.Height -= containerPadding.Top;

                ClearCachedBounds(container);
                preferredSize = LayoutUtils.UnionSizes(preferredSizeForDocking, preferredSizeForAnchoring);
            }

            return CommonProperties.GetAutoSize(container);
        }

        /// <summary>
        ///  Updates the Anchor information based on the controls current bounds. This should only be called
        ///  when the parent control changes or the anchor mode changes.
        /// </summary>
        private static void UpdateAnchorInfo(IArrangedElement element)
        {
            Debug.Assert(!HasCachedBounds(element.Container), "Do not call this method with an active cached bounds list.");

            AnchorInfo anchorInfo = GetAnchorInfo(element);
            if (anchorInfo == null)
            {
                anchorInfo = new AnchorInfo();
                SetAnchorInfo(element, anchorInfo);
            }

            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "Update anchor info");
            Debug.Indent();
            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, element.Container == null ? "No parent" : "Parent");

            if (CommonProperties.GetNeedsAnchorLayout(element) && element.Container != null)
            {
                Rectangle bounds = GetCachedBounds(element);
                AnchorInfo oldAnchorInfo = new AnchorInfo
                {
                    Left = anchorInfo.Left,
                    Top = anchorInfo.Top,
                    Right = anchorInfo.Right,
                    Bottom = anchorInfo.Bottom
                };

                anchorInfo.Left = element.Bounds.Left;
                anchorInfo.Top = element.Bounds.Top;
                anchorInfo.Right = element.Bounds.Right;
                anchorInfo.Bottom = element.Bounds.Bottom;

                Rectangle parentDisplayRect = element.Container.DisplayRectangle;
                Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "Parent displayRectangle" + parentDisplayRect);
                int parentWidth = parentDisplayRect.Width;
                int parentHeight = parentDisplayRect.Height;

                // The anchor is relative to the parent DisplayRectangle, so offset the anchor
                // by the DisplayRect origin
                anchorInfo.Left -= parentDisplayRect.X;
                anchorInfo.Top -= parentDisplayRect.Y;
                anchorInfo.Right -= parentDisplayRect.X;
                anchorInfo.Bottom -= parentDisplayRect.Y;

                AnchorStyles anchor = GetAnchor(element);
                if (IsAnchored(anchor, AnchorStyles.Right))
                {
                    if (DpiHelper.IsScalingRequirementMet && (anchorInfo.Right - parentWidth > 0) && (oldAnchorInfo.Right < 0))
                    {
                        // parent was resized to fit its parent, or screen, we need to reuse old anchor info to prevent losing control beyond right edge
                        anchorInfo.Right = oldAnchorInfo.Right;
                        // control might have been resized, update Left anchor
                        anchorInfo.Left = oldAnchorInfo.Right - bounds.Width;
                    }
                    else
                    {
                        anchorInfo.Right -= parentWidth;

                        if (!IsAnchored(anchor, AnchorStyles.Left))
                        {
                            anchorInfo.Left -= parentWidth;
                        }
                    }
                }
                else if (!IsAnchored(anchor, AnchorStyles.Left))
                {
                    anchorInfo.Right -= (parentWidth / 2);
                    anchorInfo.Left -= (parentWidth / 2);
                }

                if (IsAnchored(anchor, AnchorStyles.Bottom))
                {
                    if (DpiHelper.IsScalingRequirementMet && (anchorInfo.Bottom - parentHeight > 0) && (oldAnchorInfo.Bottom < 0))
                    {
                        // parent was resized to fit its parent, or screen, we need to reuse old anchor info to prevent losing control beyond bottom edge
                        anchorInfo.Bottom = oldAnchorInfo.Bottom;
                        // control might have been resized, update Top anchor
                        anchorInfo.Top = oldAnchorInfo.Bottom - bounds.Height;
                    }
                    else
                    {
                        anchorInfo.Bottom -= parentHeight;

                        if (!IsAnchored(anchor, AnchorStyles.Top))
                        {
                            anchorInfo.Top -= parentHeight;
                        }
                    }
                }
                else if (!IsAnchored(anchor, AnchorStyles.Top))
                {
                    anchorInfo.Bottom -= (parentHeight / 2);
                    anchorInfo.Top -= (parentHeight / 2);
                }

                Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "anchor info (l,t,r,b): (" + anchorInfo.Left + ", " + anchorInfo.Top + ", " + anchorInfo.Right + ", " + anchorInfo.Bottom + ")");
            }

            Debug.Unindent();
        }

        public static AnchorStyles GetAnchor(IArrangedElement element) => CommonProperties.xGetAnchor(element);

        public static void SetAnchor(IArrangedElement container, IArrangedElement element, AnchorStyles value)
        {
            AnchorStyles oldValue = GetAnchor(element);
            if (oldValue != value)
            {
                if (CommonProperties.GetNeedsDockLayout(element))
                {
                    // We set dock back to none to cause the element to size back to its original bounds.
                    SetDock(element, DockStyle.None);
                }

                CommonProperties.xSetAnchor(element, value);

                if (CommonProperties.GetNeedsAnchorLayout(element))
                {
                    UpdateAnchorInfo(element);
                }
                else
                {
                    SetAnchorInfo(element, null);
                }

                if (element.Container != null)
                {
                    bool rightReleased = IsAnchored(oldValue, AnchorStyles.Right) && !IsAnchored(value, AnchorStyles.Right);
                    bool bottomReleased = IsAnchored(oldValue, AnchorStyles.Bottom) && !IsAnchored(value, AnchorStyles.Bottom);
                    if (element.Container.Container != null && (rightReleased || bottomReleased))
                    {
                        // If the right or bottom anchor is being released, we have a special case where the element's
                        // margin may affect preferredSize where it didn't previously. Rather than do an expensive
                        // check for this in OnLayout, we just detect the case her and force a relayout.
                        LayoutTransaction.DoLayout(element.Container.Container, element, PropertyNames.Anchor);
                    }
                    LayoutTransaction.DoLayout(element.Container, element, PropertyNames.Anchor);
                }
            }
        }

        public static DockStyle GetDock(IArrangedElement element) => CommonProperties.xGetDock(element);

        public static void SetDock(IArrangedElement element, DockStyle value)
        {
            Debug.Assert(!HasCachedBounds(element.Container), "Do not call this method with an active cached bounds list.");

            if (GetDock(element) != value)
            {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DockStyle.None, (int)DockStyle.Fill))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DockStyle));
                }

                bool dockNeedsLayout = CommonProperties.GetNeedsDockLayout(element);
                CommonProperties.xSetDock(element, value);

                using (new LayoutTransaction(element.Container as Control, element, PropertyNames.Dock))
                {
                    // if the item is autosized, calling setbounds performs a layout, which
                    // if we havent set the anchor info properly yet makes dock/anchor layout cranky.
                    if (value == DockStyle.None)
                    {
                        if (dockNeedsLayout)
                        {
                            // We are transitioning from docked to not docked, restore the original bounds.
                            element.SetBounds(CommonProperties.GetSpecifiedBounds(element), BoundsSpecified.None);
                            // Restore Anchor information as its now relevant again.
                            UpdateAnchorInfo(element);
                        }
                    }
                    else
                    {
                        // Now setup the new bounds.
                        element.SetBounds(CommonProperties.GetSpecifiedBounds(element), BoundsSpecified.All);
                    }
                }
            }

            Debug.Assert(GetDock(element) == value, "Error setting Dock value.");
        }

        public static void ScaleAnchorInfo(IArrangedElement element, SizeF factor)
        {
            AnchorInfo anchorInfo = GetAnchorInfo(element);

            // some controls don't have AnchorInfo, i.e. Panels
            if (anchorInfo != null)
            {
                anchorInfo.Left = (int)((float)anchorInfo.Left * factor.Width);
                anchorInfo.Top = (int)((float)anchorInfo.Top * factor.Height);
                anchorInfo.Right = (int)((float)anchorInfo.Right * factor.Width);
                anchorInfo.Bottom = (int)((float)anchorInfo.Bottom * factor.Height);

                SetAnchorInfo(element, anchorInfo);
            }
        }

        private static Rectangle GetCachedBounds(IArrangedElement element)
        {
            if (element.Container != null)
            {
                IDictionary dictionary = (IDictionary)element.Container.Properties.GetObject(s_cachedBoundsProperty);
                if (dictionary != null)
                {
                    object bounds = dictionary[element];
                    if (bounds != null)
                    {
                        return (Rectangle)bounds;
                    }
                }
            }

            return element.Bounds;
        }

        private static bool HasCachedBounds(IArrangedElement container)
        {
            return container != null && container.Properties.GetObject(s_cachedBoundsProperty) != null;
        }

        private static void ApplyCachedBounds(IArrangedElement container)
        {
            if (CommonProperties.GetAutoSize(container))
            {
                // Avoiding calling DisplayRectangle before checking AutoSize for Everett compat
                Rectangle displayRectangle = container.DisplayRectangle;
                if ((displayRectangle.Width == 0) || (displayRectangle.Height == 0))
                {
                    ClearCachedBounds(container);
                    return;
                }
            }

            IDictionary dictionary = (IDictionary)container.Properties.GetObject(s_cachedBoundsProperty);
            if (dictionary != null)
            {
#if DEBUG
                // In debug builds, we need to modify the collection, so we add a break and an
                // outer loop to prevent attempting to IEnumerator.MoveNext() on a modified
                // collection.
                while (dictionary.Count > 0)
                {
#endif
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        IArrangedElement element = (IArrangedElement)entry.Key;

                        Debug.Assert(element.Container == container, "We have non-children in our containers cached bounds store.");
#if DEBUG
                        // We are about to set the bounds to the cached value. We clear the cached value
                        // before SetBounds because some controls fiddle with the bounds on SetBounds
                        // and will callback InitLayout with a different bounds and BoundsSpecified.
                        dictionary.Remove(entry.Key);
#endif
                        Rectangle bounds = (Rectangle)entry.Value;
                        element.SetBounds(bounds, BoundsSpecified.None);
#if DEBUG
                        break;
                    }
#endif
                }

                ClearCachedBounds(container);
            }
        }

        private static void ClearCachedBounds(IArrangedElement container)
        {
            container.Properties.SetObject(s_cachedBoundsProperty, null);
        }

        private static void SetCachedBounds(IArrangedElement element, Rectangle bounds)
        {
            if (bounds != GetCachedBounds(element))
            {
                IDictionary dictionary = (IDictionary)element.Container.Properties.GetObject(s_cachedBoundsProperty);
                if (dictionary == null)
                {
                    dictionary = new HybridDictionary();
                    element.Container.Properties.SetObject(s_cachedBoundsProperty, dictionary);
                }

                dictionary[element] = bounds;
            }
        }

        private static AnchorInfo GetAnchorInfo(IArrangedElement element)
        {
            return (AnchorInfo)element.Properties.GetObject(s_layoutInfoProperty);
        }

        private static void SetAnchorInfo(IArrangedElement element, AnchorInfo value)
        {
            element.Properties.SetObject(s_layoutInfoProperty, value);
        }

        private protected override void InitLayoutCore(IArrangedElement element, BoundsSpecified specified)
        {
            Debug.Assert(specified == BoundsSpecified.None || GetCachedBounds(element) == element.Bounds,
                "Attempt to InitLayout while element has active cached bounds.");

            if (specified != BoundsSpecified.None && CommonProperties.GetNeedsAnchorLayout(element))
            {
                UpdateAnchorInfo(element);
            }
        }

        internal override Size GetPreferredSize(IArrangedElement container, Size proposedBounds)
        {
            Debug.Assert(!HasCachedBounds(container), "Do not call this method with an active cached bounds list.");

            TryCalculatePreferredSize(container, measureOnly: true, preferredSize: out Size prefSize);
            return prefSize;
        }

        private static Size GetAnchorPreferredSize(IArrangedElement container)
        {
            Size prefSize = Size.Empty;

            ArrangedElementCollection children = container.Children;
            for (int i = children.Count - 1; i >= 0; i--)
            {
                IArrangedElement element = container.Children[i];
                if (!CommonProperties.GetNeedsDockLayout(element) && element.ParticipatesInLayout)
                {
                    AnchorStyles anchor = GetAnchor(element);
                    Padding margin = CommonProperties.GetMargin(element);
                    Rectangle elementSpace = LayoutUtils.InflateRect(GetCachedBounds(element), margin);

                    if (IsAnchored(anchor, AnchorStyles.Left) && !IsAnchored(anchor, AnchorStyles.Right))
                    {
                        // If we are anchored to the left we make sure the container is large enough not to clip us
                        // (unless we are right anchored, in which case growing the container will just resize us.)
                        prefSize.Width = Math.Max(prefSize.Width, elementSpace.Right);
                    }

                    if (!IsAnchored(anchor, AnchorStyles.Bottom))
                    {
                        // If we are anchored to the top we make sure the container is large enough not to clip us
                        // (unless we are bottom anchored, in which case growing the container will just resize us.)
                        prefSize.Height = Math.Max(prefSize.Height, elementSpace.Bottom);
                    }

                    if (IsAnchored(anchor, AnchorStyles.Right))
                    {
                        // If we are right anchored, see what the anchor distance between our right edge and
                        // the container is, and make sure our container is large enough to accomodate us.
                        Rectangle anchorDest = GetAnchorDestination(element, Rectangle.Empty, /*measureOnly=*/true);
                        if (anchorDest.Width < 0)
                        {
                            prefSize.Width = Math.Max(prefSize.Width, elementSpace.Right + anchorDest.Width);
                        }
                        else
                        {
                            prefSize.Width = Math.Max(prefSize.Width, anchorDest.Right);
                        }
                    }

                    if (IsAnchored(anchor, AnchorStyles.Bottom))
                    {
                        // If we are right anchored, see what the anchor distance between our right edge and
                        // the container is, and make sure our container is large enough to accomodate us.
                        Rectangle anchorDest = GetAnchorDestination(element, Rectangle.Empty, /*measureOnly=*/true);
                        if (anchorDest.Height < 0)
                        {
                            prefSize.Height = Math.Max(prefSize.Height, elementSpace.Bottom + anchorDest.Height);
                        }
                        else
                        {
                            prefSize.Height = Math.Max(prefSize.Height, anchorDest.Bottom);
                        }
                    }
                }
            }

            return prefSize;
        }

        public static bool IsAnchored(AnchorStyles anchor, AnchorStyles desiredAnchor)
        {
            return (anchor & desiredAnchor) == desiredAnchor;
        }

        [Flags]
        private enum GrowthDirection
        {
            None = 0,
            Upward = 0x01,
            Downward = 0x02,
            Left = 0x04,
            Right = 0x08
        }

#if DEBUG_PAINT_ANCHOR
	    // handy method for drawing out the child anchor infos
        internal static void  DebugPaintAnchor(Graphics g, Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                AnchorInfo layout = GetAnchorInfo(child as IArrangedElement);
                Rectangle displayRect = parent.DisplayRectangle;
                if (layout == null)
                {
                    continue;
                }

                int left = layout.Left + displayRect.X;
                int top = layout.Top + displayRect.Y;
                int right = layout.Right + displayRect.X;
                int bottom = layout.Bottom + displayRect.Y;

                AnchorStyles anchor = GetAnchor(child as IArrangedElement);

                // Repeat of GetAnchorDestination
                if (IsAnchored(anchor, AnchorStyles.Right))
                {
                    Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "\t\t...adjusting right");
                    right += displayRect.Width;

                    if (!IsAnchored(anchor, AnchorStyles.Left))
                    {
                        Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "\t\t...adjusting left");
                        left += displayRect.Width;
                    }
                }
                else if (!IsAnchored(anchor, AnchorStyles.Left))
                {
                    Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "\t\t...adjusting left & right");
                    right += (displayRect.Width / 2);
                    left += (displayRect.Width / 2);
                }

                if (IsAnchored(anchor, AnchorStyles.Bottom))
                {
                    Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "\t\t...adjusting bottom");
                    bottom += displayRect.Height;

                    if (!IsAnchored(anchor, AnchorStyles.Top))
                    {
                        Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "\t\t...adjusting top");
                        top += displayRect.Height;
                    }
                }
                else if (!IsAnchored(anchor, AnchorStyles.Top))
                {
                    Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "\t\t...adjusting top & bottom");
                    bottom += (displayRect.Height/2);
                    top += (displayRect.Height/2);
                }

                if (IsAnchored(anchor, AnchorStyles.Right))
                {
                    TextRenderer.DrawText(g, "right " + layout.Right.ToString(), parent.Font, new Point(right/2, child.Top - 20), Color.HotPink);
                    g.FillRectangle(Brushes.HotPink, 0,  child.Top -4, right, 1);
                }
                if (IsAnchored(anchor, AnchorStyles.Left))
                {
                    TextRenderer.DrawText(g, "left " + layout.Left.ToString(), parent.Font, new Point(left/2, child.Top - 16), Color.Green);
                    g.FillRectangle(Brushes.Green, 0, child.Top -2, left, 1);
                }
                if (IsAnchored(anchor, AnchorStyles.Top))
                {
                    TextRenderer.DrawText(g, "top " + layout.Top.ToString(), parent.Font, new Point(child.Left -100, top/2), Color.Blue);

                    g.FillRectangle(Brushes.Blue, child.Left -1, 0, 1, top);
                }
                if (IsAnchored(anchor, AnchorStyles.Bottom))
                {
                    TextRenderer.DrawText(g, "bottom " + layout.Bottom.ToString(), parent.Font, new Point(child.Left -100, right/2), Color.Red);
                    g.FillRectangle(Brushes.Red, child.Left -2, 1, 1, bottom);
                }
            }
        }
#endif

#if DEBUG_LAYOUT
#if DEBUG
        internal static string Debug_GetAllLayoutInformation(Control start, int indents)
        {
            string info = Debug_GetLayoutInfo(start, indents) + Environment.NewLine;
            for (int i = 0; i < start.Controls.Count; i++)
            {
                info += Debug_GetIndents(indents) + "+-->" + Debug_GetAllLayoutInformation(start.Controls[i], indents + 1) + "\r\n";
            }

            return info;
        }

        internal static string Debug_GetIndents(int indents)
        {
            string str = string.Empty;
            for (int i = 0; i < indents; i++)
            {
                str += '\t';
            }
            return str;
        }

        internal static string Debug_GetLayoutInfo(Control control, int indents)
        {
            string lineBreak = Environment.NewLine + Debug_GetIndents(indents + 1);
            string layoutInfo = string.Format(System.Globalization.CultureInfo.CurrentCulture,
                                            "Handle {9} Name {1} Type {2} {0} Bounds {3} {0} AutoSize {4} {0} Dock [{5}] Anchor [{6}] {0} Padding [{7}] Margin [{8}]",
                                                lineBreak,
                                                control.Name,
                                                control.GetType().Name,
                                                control.Bounds,
                                                control.AutoSize,
                                                control.Dock,
                                                control.Anchor,
                                                control.Padding,
                                                control.Margin,
                                                !control.IsHandleCreated ? "[not created]" : "0x" + ((int)(control.Handle)).ToString("x"));
            if (control is TableLayoutPanel panelControl)
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(TableLayoutSettings));
                layoutInfo += lineBreak + converter.ConvertTo(panelControl, typeof(string));
            }

            return layoutInfo;
        }
#endif
#endif

        private sealed class AnchorInfo
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}
