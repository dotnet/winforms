﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms.Primitives;

namespace System.Windows.Forms.Layout
{
    internal partial class DefaultLayout : LayoutEngine
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
                    Size proposedConstraints = LayoutUtils.s_maxSize;

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
        ///  Gets the bounds of the control after growing to newSize (note that depending on
        ///  anchoring the control may grow to the left/upwards rather than to the
        ///  right/downwards. i.e., it may be translated.)
        /// </summary>
        private static Rectangle GetGrowthBounds(IArrangedElement element, Size newSize)
        {
            GrowthDirection direction = GetGrowthDirection(element);
            Rectangle oldBounds = GetCachedBounds(element);
            Point location = oldBounds.Location;

            Debug.Assert(CommonProperties.GetAutoSizeMode(element) == AutoSizeMode.GrowAndShrink || (newSize.Height >= oldBounds.Height && newSize.Width >= oldBounds.Width),
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

            Debug.Assert(CommonProperties.GetAutoSizeMode(element) == AutoSizeMode.GrowAndShrink || newBounds.Contains(oldBounds), "How did we resize in such a way we no longer contain our old bounds?");

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
                // Control is anchored to the right, but not the left.
                growthDirection |= GrowthDirection.Left;
            }
            else
            {
                // Otherwise we grow towards the right (common case)
                growthDirection |= GrowthDirection.Right;
            }

            if ((anchor & AnchorStyles.Bottom) != AnchorStyles.None
                && (anchor & AnchorStyles.Top) == AnchorStyles.None)
            {
                // Control is anchored to the bottom, but not the top.
                growthDirection |= GrowthDirection.Upward;
            }
            else
            {
                // Otherwise we grow towards the bottom. (common case)
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
            // Container can not be null since we AnchorControls takes a non-null container.
            //
            // NB: DO NOT convert the following into Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "...")
            // because it WILL execute GetCachedBounds(control).ToString() calls even if CompModSwitches.RichLayout.TraceInfo=false
            // This in turn will lead to a cascade of native calls and callbacks
            if (CompModSwitches.RichLayout.TraceInfo)
            {
                Debug.WriteLine($"\t\t'{element}' is anchored at {GetCachedBounds(element)}");
            }

            return UseAnchorLayoutV2(element)
                ? ComputeAnchoredBoundsV2(element, displayRect)
                : ComputeAnchoredBounds(element, displayRect, measureOnly);
        }

        private static Rectangle ComputeAnchoredBoundsV2(IArrangedElement element, Rectangle displayRect)
        {
            if (displayRect.IsEmpty)
            {
                return element.Bounds;
            }

            AnchorInfo anchorInfo = GetAnchorInfo(element);
            if (anchorInfo is null)
            {
                return element.Bounds;
            }

            Rectangle elementBounds = element.Bounds;
            int width = elementBounds.Width;
            int height = elementBounds.Height;

            Debug.WriteLineIf(width < 0 || height < 0, $"\t\t'{element}' destination bounds resulted in negative");

            // Compute control bounds according to AnchorStyles set on it.
            AnchorStyles anchors = GetAnchor(element);
            if (IsAnchored(anchors, AnchorStyles.Left))
            {
                // If anchored both Left and Right, the control's width should be adjusted according to
                // the parent's width.
                if (IsAnchored(anchors, AnchorStyles.Right))
                {
                    width = displayRect.Width - (anchorInfo.Right + anchorInfo.Left);
                }
            }
            else
            {
                // If anchored Right but not Left, the control's X-coordinate should be adjusted according
                // to the parent's width.
                if (IsAnchored(anchors, AnchorStyles.Right))
                {
                    anchorInfo.Left = displayRect.Width - elementBounds.Width - anchorInfo.Right;
                }
                else
                {
                    // The control neither anchored Right nor Left but anchored Top or Bottom, the control's
                    // X-coordinate should be adjusted according to the parent's width.
                    int growOrShrink = (displayRect.Width - (anchorInfo.Left + anchorInfo.Right + elementBounds.Width)) / 2;
                    anchorInfo.Left += growOrShrink;
                    anchorInfo.Right += growOrShrink;
                }
            }

            if (IsAnchored(anchors, AnchorStyles.Top))
            {
                if (IsAnchored(anchors, AnchorStyles.Bottom))
                {
                    // If anchored both Top and Bottom, the control's height should be adjusted according to
                    // the parent's height.
                    height = displayRect.Height - (anchorInfo.Bottom + anchorInfo.Top);
                }
            }
            else
            {
                // If anchored Bottom but not Top, the control's Y-coordinate should be adjusted according to
                // the parent's height.
                if (IsAnchored(anchors, AnchorStyles.Bottom))
                {
                    anchorInfo.Top = displayRect.Height - elementBounds.Height - anchorInfo.Bottom;
                }
                else
                {
                    // The control neither anchored Top or Bottom but anchored Right or Left, the control's
                    // Y-coordinate is adjusted accoring to the parent's height.
                    int growOrShrink = (displayRect.Height - (anchorInfo.Bottom + anchorInfo.Top + elementBounds.Height)) / 2;
                    anchorInfo.Top += growOrShrink;
                    anchorInfo.Bottom += growOrShrink;
                }
            }

            return new Rectangle(anchorInfo.Left, anchorInfo.Top, width, height);
        }

        private static Rectangle ComputeAnchoredBounds(IArrangedElement element, Rectangle displayRect, bool measureOnly)
        {
            AnchorInfo layout = GetAnchorInfo(element);

            int left = layout.Left + displayRect.X;
            int top = layout.Top + displayRect.Y;
            int right = layout.Right + displayRect.X;
            int bottom = layout.Bottom + displayRect.Y;
            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, $"\t\t...anchor dim (l,t,r,b) {{{left}, {top}, {right}, {bottom}}}");

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
                int center = displayRect.Width / 2;
                right += center;
                left += center;
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
                int center = displayRect.Height / 2;
                bottom += center;
                top += center;
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
                // cachedBounds != control.Bounds means  the control's size has changed
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
                // cachedBounds != control.Bounds means  the control's size has changed
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

            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, $"\t\t...new anchor dim (l,t,r,b) {{{left}, {top}, {right}, {bottom}}}");

            return new Rectangle(left, top, right - left, bottom - top);
        }

        /// <summary>
        ///  Determines if AnchorLayoutV2 should be used to compute anchors of the element
        ///  and to layout anchored children controls with V2 version.
        /// </summary>
        internal static bool UseAnchorLayoutV2(IArrangedElement element)
        {
            // AnchorLayoutV2  only supports Control types. If the feature is disabled or
            // the element is not of Control type, use the original layout method.
            return LocalAppContextSwitches.AnchorLayoutV2 && element is Control control;
        }

        private static void LayoutAnchoredControlsV2(Control container)
        {
            Debug.Assert(LocalAppContextSwitches.AnchorLayoutV2, $"AnchorLayoutV2 should be called only when {LocalAppContextSwitches.AnchorLayoutV2SwitchName} is enabled.");

            // If the container handle is not created, no need to compute anchors for its children controls.
            if (!container.IsHandleCreated)
            {
                return;
            }

            LayoutAnchoredControls(container, updateAnchorInfoIfNeeded: true);
        }

        private static void LayoutAnchoredControls(IArrangedElement container, bool updateAnchorInfoIfNeeded = false)
        {
            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "\tAnchor Processing");
            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, $"\t\tdisplayRect: {container.DisplayRectangle.ToString()}");

            Rectangle displayRectangle = container.DisplayRectangle;
            if (CommonProperties.GetAutoSize(container) && ((displayRectangle.Width == 0) || (displayRectangle.Height == 0)))
            {
                // We haven't set ourselves to the preferred size yet. Proceeding will
                // just set all the control widths to zero.
                return;
            }

            ArrangedElementCollection children = container.Children;
            for (int i = children.Count - 1; i >= 0; i--)
            {
                IArrangedElement element = children[i];
                if (!CommonProperties.GetNeedsAnchorLayout(element))
                {
                    continue;
                }

                if (GetAnchorInfo(element) is null)
                {
                    // It is possible that the parent's handle might not have been created
                    // when the control's handle was created. If that's the case, compute the control's anchors.
                    if (updateAnchorInfoIfNeeded)
                    {
                        UpdateAnchorInfoV2((Control)element);
                    }
                    else
                    {
                        Debug.Assert(GetAnchorInfo(element) is not null, "AnchorInfo should be initialized before LayoutAnchorControls().");
                    }
                }

                SetCachedBounds(element, GetAnchorDestination(element, displayRectangle, measureOnly: false));
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

                                // What we are really doing here: top += control.Bounds.Height;
                                remainingBounds.Y += element.Bounds.Height;
                                remainingBounds.Height -= element.Bounds.Height;
                                break;
                            }

                        case DockStyle.Bottom:
                            {
                                Size elementSize = GetVerticalDockedSize(element, remainingBounds.Size, measureOnly);
                                Rectangle newElementBounds = new Rectangle(remainingBounds.X, remainingBounds.Bottom - elementSize.Height, elementSize.Width, elementSize.Height);

                                TryCalculatePreferredSizeDockedControl(element, newElementBounds, measureOnly, ref preferredSize, ref remainingBounds);

                                // What we are really doing here: bottom -= control.Bounds.Height;
                                remainingBounds.Height -= element.Bounds.Height;

                                break;
                            }

                        case DockStyle.Left:
                            {
                                Size elementSize = GetHorizontalDockedSize(element, remainingBounds.Size, measureOnly);
                                Rectangle newElementBounds = new Rectangle(remainingBounds.X, remainingBounds.Y, elementSize.Width, elementSize.Height);

                                TryCalculatePreferredSizeDockedControl(element, newElementBounds, measureOnly, ref preferredSize, ref remainingBounds);

                                // What we are really doing here: left += control.Bounds.Width;
                                remainingBounds.X += element.Bounds.Width;
                                remainingBounds.Width -= element.Bounds.Width;
                                break;
                            }

                        case DockStyle.Right:
                            {
                                Size elementSize = GetHorizontalDockedSize(element, remainingBounds.Size, measureOnly);
                                Rectangle newElementBounds = new Rectangle(remainingBounds.Right - elementSize.Width, remainingBounds.Y, elementSize.Width, elementSize.Height);

                                TryCalculatePreferredSizeDockedControl(element, newElementBounds, measureOnly, ref preferredSize, ref remainingBounds);

                                // What we are really doing here: right -= control.Bounds.Width;
                                remainingBounds.Width -= element.Bounds.Width;
                                break;
                            }

                        case DockStyle.Fill:
                            if (element is MdiClient)
                            {
                                Debug.Assert(mdiClient is null, "How did we end up with multiple MdiClients?");
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
                if (mdiClient is not null)
                {
                    SetCachedBounds(mdiClient, remainingBounds);
                }
            }

            return preferredSize;
        }

        /// <summary>
        ///  Helper method that either sets the control bounds or does the preferredSize computation based on
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
            Size newSize = xGetDockedSize(element, /* constraints = */ new Size(remainingSize.Width, 1));
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
            Size newSize = xGetDockedSize(element, /* constraints = */ new Size(1, remainingSize.Height));
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

        private static Size xGetDockedSize(IArrangedElement element, Size constraints)
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

            Debug.Assert(desiredSize.Width >= 0 && desiredSize.Height >= 0, "Error detected in xGetDockSize: Element size was negative.");
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

            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, $"\tanchor : {anchor.ToString()}");
            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, $"\tdock :   {dock.ToString()}");

            Size preferredSizeForDocking = Size.Empty;
            Size preferredSizeForAnchoring = Size.Empty;

            if (dock)
            {
                preferredSizeForDocking = LayoutDockedControls(container, measureOnly);
            }

            if (anchor && !measureOnly)
            {
                // In the case of anchors, where we currently are defines the preferred size,
                // so don't recalculate the positions of everything.
                if (UseAnchorLayoutV2(container))
                {
                    LayoutAnchoredControlsV2((Control)container);
                }
                else
                {
                    LayoutAnchoredControls(container);
                }
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
        ///  Updates the control's anchors information based on the control's current bounds.
        /// </summary>
        private static void UpdateAnchorInfo(IArrangedElement element)
        {
            Debug.Assert(!HasCachedBounds(element.Container), "Do not call this method with an active cached bounds list.");

            if (!CommonProperties.GetNeedsAnchorLayout(element))
            {
                return;
            }

            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "Update anchor info");
            Debug.Indent();
            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, element.Container is null ? "No parent" : "Parent");

            if (element.Container is null)
            {
                return;
            }

            // If AnchorLayoutV2 switch is enabled, use V2 Layout.
            if (UseAnchorLayoutV2(element))
            {
                UpdateAnchorInfoV2((Control)element);
                return;
            }

            AnchorInfo anchorInfo = GetAnchorInfo(element);
            if (anchorInfo is null)
            {
                anchorInfo = new AnchorInfo();
                SetAnchorInfo(element, anchorInfo);
            }

            Rectangle cachedBounds = GetCachedBounds(element);
            AnchorInfo oldAnchorInfo = new()
            {
                Left = anchorInfo.Left,
                Top = anchorInfo.Top,
                Right = anchorInfo.Right,
                Bottom = anchorInfo.Bottom
            };

            Rectangle elementBounds = element.Bounds;
            anchorInfo.Left = elementBounds.Left;
            anchorInfo.Top = elementBounds.Top;
            anchorInfo.Right = elementBounds.Right;
            anchorInfo.Bottom = elementBounds.Bottom;

            Rectangle parentDisplayRect = element.Container.DisplayRectangle;
            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, $"Parent displayRectangle{parentDisplayRect}");
            int parentWidth = parentDisplayRect.Width;
            int parentHeight = parentDisplayRect.Height;

            // The anchors is relative to the parent DisplayRectangle, so offset the anchors
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
                    // Parent was resized to fit its parent, or screen, we need to reuse old anchors info to prevent losing control beyond right edge.
                    anchorInfo.Right = oldAnchorInfo.Right;
                    if (!IsAnchored(anchor, AnchorStyles.Left))
                    {
                        // Control might have been resized, update Left anchors.
                        anchorInfo.Left = oldAnchorInfo.Right - cachedBounds.Width;
                    }
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
                anchorInfo.Right -= parentWidth / 2;
                anchorInfo.Left -= parentWidth / 2;
            }

            if (IsAnchored(anchor, AnchorStyles.Bottom))
            {
                if (DpiHelper.IsScalingRequirementMet && (anchorInfo.Bottom - parentHeight > 0) && (oldAnchorInfo.Bottom < 0))
                {
                    // The parent was resized to fit its parent or the screen, we need to reuse the old anchors info to prevent positioning the control beyond the bottom edge.
                    anchorInfo.Bottom = oldAnchorInfo.Bottom;

                    if (!IsAnchored(anchor, AnchorStyles.Top))
                    {
                        // The control might have been resized, update the Top anchor.
                        anchorInfo.Top = oldAnchorInfo.Bottom - cachedBounds.Height;
                    }
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
                anchorInfo.Bottom -= parentHeight / 2;
                anchorInfo.Top -= parentHeight / 2;
            }

            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, $"anchor info (l,t,r,b): ({anchorInfo.Left}, {anchorInfo.Top}, {anchorInfo.Right}, {anchorInfo.Bottom})");
            Debug.Unindent();
        }

        /// <summary>
        ///  Updates anchors calculations if the control is parented and both control's and its parent's handle are created.
        /// </summary>
        /// <devdoc>
        ///  This is the new behavior introduced in .NET 8.0. Refer to
        ///  https://github.com/microsoft/winforms/blob/tree/main/docs/design/anchor_layout_changes_in_net80.md for more details.
        ///  Developers may opt-out of this new behavior using switch <see cref="LocalAppContextSwitches.AnchorLayoutV2"/>.
        /// </devdoc>
        internal static void UpdateAnchorInfoV2(Control control)
        {
            if (!CommonProperties.GetNeedsAnchorLayout(control))
            {
                return;
            }

            Debug.Assert(LocalAppContextSwitches.AnchorLayoutV2, $"AnchorLayoutV2 should be called only when {LocalAppContextSwitches.AnchorLayoutV2SwitchName} is enabled.");

            Control parent = control.Parent;

            // Check if control is ready for anchors calculation.
            if (parent is null
                || !control.IsHandleCreated
                || !parent.IsHandleCreated)
            {
                return;
            }

            // Anchors are scaled for the new DPI and doesn't need to be recalculated.
            if (IsDPIScalingInProgress(control, parent))
            {
                return;
            }

            AnchorInfo anchorInfo = GetAnchorInfo(control);
            if (anchorInfo is null)
            {
                anchorInfo = new AnchorInfo();
                SetAnchorInfo(control, anchorInfo);
            }

            Rectangle displayRect = control.Parent.DisplayRectangle;
            Rectangle elementBounds = control.Bounds;
            int x = elementBounds.X;
            int y = elementBounds.Y;

            anchorInfo.Left = x;
            anchorInfo.Top = y;

            anchorInfo.Right = displayRect.Width - (x + elementBounds.Width);
            anchorInfo.Bottom = displayRect.Height - (y + elementBounds.Height);

            // Walk through parent heirarchy and check if scaling due to DPI change is in progress.
            static bool IsDPIScalingInProgress(Control control, Control parent)
            {
                if (control.IsCurrentlyBeingScaled)
                {
                    return true;
                }

                while (parent is not null)
                {
                    if (parent is ContainerControl container && container.ContainersDpiScalingInProgress)
                    {
                        return true;
                    }

                    parent = parent.Parent;
                }

                return false;
            }
        }

        public static AnchorStyles GetAnchor(IArrangedElement element) => CommonProperties.xGetAnchor(element);

        public static void SetAnchor(IArrangedElement element, AnchorStyles value)
        {
            AnchorStyles oldValue = GetAnchor(element);
            if (oldValue != value)
            {
                if (CommonProperties.GetNeedsDockLayout(element))
                {
                    // We set dock back to none to cause the control to size back to its original bounds.
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

                if (element.Container is not null)
                {
                    bool rightReleased = IsAnchored(oldValue, AnchorStyles.Right) && !IsAnchored(value, AnchorStyles.Right);
                    bool bottomReleased = IsAnchored(oldValue, AnchorStyles.Bottom) && !IsAnchored(value, AnchorStyles.Bottom);
                    if (element.Container.Container is not null && (rightReleased || bottomReleased))
                    {
                        // If the right or bottom anchors is being released, we have a special case where the control's
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
                SourceGenerated.EnumValidator.Validate(value);

                bool dockNeedsLayout = CommonProperties.GetNeedsDockLayout(element);
                CommonProperties.xSetDock(element, value);

                using (new LayoutTransaction(element.Container as Control, element, PropertyNames.Dock))
                {
                    // if the item is autosized, calling setbounds performs a layout, which
                    // if we haven't set the anchors info properly yet makes dock/anchors layout cranky.
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
            if (anchorInfo is not null)
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
            if (element.Container is not null)
            {
                IDictionary dictionary = (IDictionary)element.Container.Properties.GetObject(s_cachedBoundsProperty);
                if (dictionary is not null)
                {
                    object bounds = dictionary[element];
                    if (bounds is not null)
                    {
                        return (Rectangle)bounds;
                    }
                }
            }

            return element.Bounds;
        }

        private static bool HasCachedBounds(IArrangedElement container)
        {
            return container is not null && container.Properties.GetObject(s_cachedBoundsProperty) is not null;
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
            if (dictionary is not null)
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
                if (dictionary is null)
                {
                    dictionary = new HybridDictionary();
                    element.Container.Properties.SetObject(s_cachedBoundsProperty, dictionary);
                }

                dictionary[element] = bounds;
            }
        }

        internal static AnchorInfo GetAnchorInfo(IArrangedElement element)
        {
            return (AnchorInfo)element.Properties.GetObject(s_layoutInfoProperty);
        }

        internal static void SetAnchorInfo(IArrangedElement element, AnchorInfo value)
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
            bool useV2Layout = UseAnchorLayoutV2(container);

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
                        // If we are right anchored, see what the anchors distance between our right edge and
                        // the container is, and make sure our container is large enough to accomodate us.
                        if (useV2Layout)
                        {
                            AnchorInfo anchorInfo = GetAnchorInfo(element);
                            Rectangle bounds = element.Bounds;
                            prefSize.Width = Math.Max(prefSize.Width, anchorInfo is null ? bounds.Right : bounds.Right + anchorInfo.Right);
                        }
                        else
                        {
                            Rectangle anchorDest = GetAnchorDestination(element, Rectangle.Empty, measureOnly: true);
                            prefSize.Width = anchorDest.Width < 0
                                ? Math.Max(prefSize.Width, elementSpace.Right + anchorDest.Width)
                                : Math.Max(prefSize.Width, anchorDest.Right);
                        }
                    }

                    if (IsAnchored(anchor, AnchorStyles.Bottom))
                    {
                        // If we are right anchored, see what the anchors distance between our right edge and
                        // the container is, and make sure our container is large enough to accomodate us.
                        Rectangle anchorDest = GetAnchorDestination(element, Rectangle.Empty, measureOnly: true);
                        if (useV2Layout)
                        {
                            AnchorInfo anchorInfo = GetAnchorInfo(element);
                            Rectangle bounds = element.Bounds;
                            prefSize.Height = Math.Max(prefSize.Height, anchorInfo is null ? bounds.Bottom : bounds.Bottom + anchorInfo.Bottom);
                        }
                        else
                        {
                            prefSize.Height = anchorDest.Height < 0
                                ? Math.Max(prefSize.Height, elementSpace.Bottom + anchorDest.Height)
                                : Math.Max(prefSize.Height, anchorDest.Bottom);
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
    }
}
