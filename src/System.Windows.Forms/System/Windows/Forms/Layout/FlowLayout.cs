// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Layout;

internal partial class FlowLayout : LayoutEngine
{
    internal static FlowLayout Instance { get; } = new();

    private static readonly int s_wrapContentsProperty = PropertyStore.CreateKey();
    private static readonly int s_flowDirectionProperty = PropertyStore.CreateKey();

    private protected override bool LayoutCore(IArrangedElement container, LayoutEventArgs args)
    {
        // ScrollableControl will first try to get the layoutbounds from the derived control when
        // trying to figure out if ScrollBars should be added.
        CommonProperties.SetLayoutBounds(container, TryCalculatePreferredSize(container, container.DisplayRectangle, measureOnly: false));
        return CommonProperties.GetAutoSize(container);
    }

    internal override Size GetPreferredSize(IArrangedElement container, Size proposedConstraints)
    {
        Rectangle measureBounds = new(new Point(0, 0), proposedConstraints);
        Size prefSize = TryCalculatePreferredSize(container, measureBounds, measureOnly: true);

        if (prefSize.Width > proposedConstraints.Width || prefSize.Height > proposedConstraints.Height)
        {
            // Controls measured earlier than a control which couldn't be fit to constraints may
            // shift around with the new bounds. We need to make a 2nd pass through the
            // controls using these bounds which are guaranteed to fit.
            measureBounds.Size = prefSize;
            prefSize = TryCalculatePreferredSize(container, measureBounds, measureOnly: true);
        }

        return prefSize;
    }

    private static ContainerProxy CreateContainerProxy(IArrangedElement container, FlowDirection flowDirection) =>
        flowDirection switch
        {
            FlowDirection.RightToLeft => new RightToLeftProxy(container),
            FlowDirection.TopDown => new TopDownProxy(container),
            FlowDirection.BottomUp => new BottomUpProxy(container),
            _ => new ContainerProxy(container),
        };

    /// <summary>
    ///  Both LayoutCore and GetPreferredSize forward to this method.
    ///  The measureOnly flag determines which behavior we get.
    /// </summary>
    private static Size TryCalculatePreferredSize(IArrangedElement container, Rectangle displayRect, bool measureOnly)
    {
        FlowDirection flowDirection = GetFlowDirection(container);
        bool wrapContents = GetWrapContents(container);

        ContainerProxy containerProxy = CreateContainerProxy(container, flowDirection);
        containerProxy.DisplayRect = displayRect;

        // refetch as it's now adjusted for Vertical.
        displayRect = containerProxy.DisplayRect;

        ElementProxy elementProxy = containerProxy.ElementProxy;
        Size layoutSize = Size.Empty;

        if (!wrapContents)
        {
            // pretend that the container is infinitely wide to prevent wrapping.
            // DisplayRectangle.Right is Width + X - subtract X to prevent overflow.
            displayRect.Width = int.MaxValue - displayRect.X;
        }

        for (int i = 0; i < container.Children.Count;)
        {
            Rectangle measureBounds = new(displayRect.X, displayRect.Y, displayRect.Width, displayRect.Height - layoutSize.Height);
            Size rowSize = MeasureRow(containerProxy, elementProxy, i, measureBounds, out int breakIndex);

            // if we are not wrapping contents, then the breakIndex (as set in MeasureRow)
            // should be equal to the count of child items in the container.
            Debug.Assert(wrapContents || breakIndex == container.Children.Count,
                "We should not be trying to break the row if we are not wrapping contents.");

            if (!measureOnly)
            {
                Rectangle rowBounds = new(displayRect.X,
                    layoutSize.Height + displayRect.Y,
                    rowSize.Width,
                    rowSize.Height);
                LayoutRow(containerProxy, elementProxy, startIndex: i, endIndex: breakIndex, rowBounds: rowBounds);
            }

            layoutSize.Width = Math.Max(layoutSize.Width, rowSize.Width);
            layoutSize.Height += rowSize.Height;
            i = breakIndex;
        }

        // Verify that our alignment is correct
        if (container.Children.Count != 0 && !measureOnly)
        {
            Debug_VerifyAlignment(container, flowDirection);
        }

        return LayoutUtils.FlipSizeIf(flowDirection == FlowDirection.TopDown || GetFlowDirection(container) == FlowDirection.BottomUp, layoutSize);
    }

    /// <summary>
    ///  Just forwards to TryCalculatePreferredSizeRow. This will layout elements from the start index to the end
    ///  index. RowBounds was computed by a call to measure row and is used for alignment/boxstretch.
    ///  See the ElementProxy class for an explanation of the elementProxy parameter.
    /// </summary>
    private static void LayoutRow(
        ContainerProxy containerProxy,
        ElementProxy elementProxy,
        int startIndex,
        int endIndex,
        Rectangle rowBounds)
    {
        TryCalculatePreferredSizeRow(
            containerProxy,
            elementProxy,
            startIndex,
            endIndex,
            rowBounds,
            breakIndex: out int dummy,
            measureOnly: false);

        Debug.Assert(dummy == endIndex, "EndIndex / BreakIndex mismatch.");
    }

    /// <summary>
    ///  Just forwards to TryCalculatePreferredSizeRow. breakIndex is the index of the first control not to
    ///  fit in the displayRectangle. The returned Size is the size required to layout the
    ///  controls from startIndex up to but not including breakIndex. See the ElementProxy
    ///  class for an explanation of the elementProxy parameter.
    /// </summary>
    private static Size MeasureRow(
        ContainerProxy containerProxy,
        ElementProxy elementProxy,
        int startIndex,
        Rectangle displayRectangle,
        out int breakIndex) =>
        TryCalculatePreferredSizeRow(
            containerProxy,
            elementProxy,
            startIndex,
            endIndex: containerProxy.Container.Children.Count,
            rowBounds: displayRectangle,
            breakIndex: out breakIndex,
            measureOnly: true);

    /// <summary>
    ///  LayoutRow and MeasureRow both forward to this method. The measureOnly flag
    ///  determines which behavior we get.
    /// </summary>
    private static Size TryCalculatePreferredSizeRow(
        ContainerProxy containerProxy,
        ElementProxy elementProxy,
        int startIndex,
        int endIndex,
        Rectangle rowBounds,
        out int breakIndex,
        bool measureOnly)
    {
        Debug.Assert(startIndex < endIndex, "Loop should be in forward Z-order.");
        Point location = rowBounds.Location;
        Size rowSize = Size.Empty;
        int laidOutItems = 0;
        breakIndex = startIndex;

        bool wrapContents = GetWrapContents(containerProxy.Container);
        bool breakOnNextItem = false;

        ArrangedElementCollection collection = containerProxy.Container.Children;
        for (int i = startIndex; i < endIndex; i++, breakIndex++)
        {
            elementProxy.Element = collection[i];
            if (!elementProxy.ParticipatesInLayout)
            {
                continue;
            }

            // Figure out how much space this element is going to need (requiredSize)
            Size prefSize;
            if (elementProxy.AutoSize)
            {
                Size elementConstraints = new(int.MaxValue, rowBounds.Height - elementProxy.Margin.Size.Height);
                if (i == startIndex)
                {
                    // If the element is the first in the row, attempt to pack it to the row width. (If its not 1st, it will wrap
                    // to the first on the next row if its too long and then be packed if needed by the next call to TryCalculatePreferredSizeRow).
                    elementConstraints.Width = rowBounds.Width - rowSize.Width - elementProxy.Margin.Size.Width;
                }

                // Make sure that subtracting the margins does not cause width/height to be <= 0, or we will
                // size as if we had infinite space when in fact we are trying to be as small as possible.
                elementConstraints = LayoutUtils.UnionSizes(new Size(1, 1), elementConstraints);
                prefSize = elementProxy.GetPreferredSize(elementConstraints);
            }
            else
            {
                // If autosizing is turned off, we just use the element's current size as its preferred size.
                prefSize = elementProxy.SpecifiedSize;

                // except if it is stretching - then ignore the affect of the height dimension.
                if (elementProxy.Stretches)
                {
                    prefSize.Height = 0;
                }

                // Enforce MinimumSize
                if (prefSize.Height < elementProxy.MinimumSize.Height)
                {
                    prefSize.Height = elementProxy.MinimumSize.Height;
                }
            }

            Size requiredSize = prefSize + elementProxy.Margin.Size;

            // Position the element (if applicable).
            if (!measureOnly)
            {
                // If measureOnly = false, rowBounds.Height = measured row height
                // (otherwise its the remaining displayRect of the container)

                Rectangle cellBounds = new(location, new Size(requiredSize.Width, rowBounds.Height));

                // We laid out the rows with the elementProxy's margins included.
                // We now deflate the rect to get the actual elementProxy bounds.
                cellBounds = LayoutUtils.DeflateRect(cellBounds, elementProxy.Margin);

                AnchorStyles anchorStyles = elementProxy.AnchorStyles;
                containerProxy.Bounds = LayoutUtils.AlignAndStretch(prefSize, cellBounds, anchorStyles);
            }

            // Keep track of how much space is being used in this row
            location.X += requiredSize.Width;
            if (laidOutItems > 0)
            {
                // If control does not fit on this row, exclude it from row and stop now.
                //   Exception: If row is empty, allow this control to fit on it. So controls
                //   that exceed the maximum row width will end up occupying their own rows.
                if (location.X > rowBounds.Right)
                {
                    break;
                }
            }

            // Control fits on this row, so update the row size.
            //   rowSize.Width != location.X because with a scrollable control
            //   we could have started with a location like -100.
            rowSize.Width = location.X - rowBounds.X;
            rowSize.Height = Math.Max(rowSize.Height, requiredSize.Height);

            // check for line breaks.
            if (wrapContents)
            {
                if (breakOnNextItem)
                {
                    break;
                }
                else if (i + 1 < endIndex && CommonProperties.GetFlowBreak(elementProxy.Element))
                {
                    if (laidOutItems == 0)
                    {
                        breakOnNextItem = true;
                    }
                    else
                    {
                        breakIndex++;
                        break;
                    }
                }
            }

            laidOutItems++;
        }

        return rowSize;
    }

    public static bool GetWrapContents(IArrangedElement container) =>
        !container.Properties.TryGetValue(s_wrapContentsProperty, out bool wrap) || wrap;

    public static void SetWrapContents(IArrangedElement container, bool value)
    {
        container.Properties.AddValue(s_wrapContentsProperty, value);
        LayoutTransaction.DoLayout(container, container, PropertyNames.WrapContents);
        Debug.Assert(GetWrapContents(container) == value, "GetWrapContents should return the same value as we set");
    }

    public static FlowDirection GetFlowDirection(IArrangedElement container) =>
        container.Properties.GetValueOrDefault<FlowDirection>(s_flowDirectionProperty);

    public static void SetFlowDirection(IArrangedElement container, FlowDirection value)
    {
        SourceGenerated.EnumValidator.Validate(value);

        container.Properties.AddValue(s_flowDirectionProperty, value);
        LayoutTransaction.DoLayout(container, container, PropertyNames.FlowDirection);
        Debug.Assert(GetFlowDirection(container) == value, "GetFlowDirection should return the same value as we set");
    }

    [Conditional("DEBUG_VERIFY_ALIGNMENT")]
    private static void Debug_VerifyAlignment(IArrangedElement container, FlowDirection flowDirection)
    {
#if DEBUG
        // We cannot apply any of these checks @ design-time since dragging new children into a FlowLayoutPanel
        // will attempt to set the children at the mouse position when the child was dropped - we rely on the control
        // to reposition the children once added.
        if (container is Control flp && flp.Site is not null && flp.Site.DesignMode)
        {
            return;
        }

        // Check to see if the first element is in its right place
        Padding margin = CommonProperties.GetMargin(container.Children[0]);
        switch (flowDirection)
        {
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

        // Next check to see if everything is in bound
        ArrangedElementCollection collection = container.Children;
        for (int i = 1; i < collection.Count; i++)
        {
            switch (flowDirection)
            {
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
}
