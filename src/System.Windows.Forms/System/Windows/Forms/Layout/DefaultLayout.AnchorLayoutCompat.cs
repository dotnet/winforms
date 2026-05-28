// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Layout;

internal partial class DefaultLayout
{
    private const int MinimumSignificantStretchDelta = 16;

    private static AnchorStyles TryRefreshAnchorInfoForDisplayRectangleGrowth(IArrangedElement element, AnchorInfo anchorInfo, Rectangle displayRect)
    {
        AnchorStyles anchor = GetAnchor(element);
        Rectangle bounds = GetCachedBounds(element);
        bool shouldRefreshStretchAnchors = ShouldRefreshAnchorInfoForStaleStretchAnchors(
            element,
            anchorInfo,
            bounds,
            displayRect,
            anchor,
            out bool useSpecifiedDisplayRectangleForStretchRefresh);
        bool shouldRefreshPositiveAnchors = ShouldRefreshAnchorInfoForStalePositiveAnchors(anchorInfo, bounds, displayRect, anchor);

        if (!shouldRefreshStretchAnchors
            && !shouldRefreshPositiveAnchors)
        {
            return anchor;
        }

        RefreshAnchorInfoForDisplayRectangleGrowth(
            element,
            anchorInfo,
            displayRect,
            anchor,
            shouldRefreshStretchAnchors,
            useSpecifiedDisplayRectangleForStretchRefresh);

        return anchor;
    }

    internal static bool ShouldRefreshAnchorInfoForStalePositiveAnchors(AnchorInfo anchorInfo, Rectangle bounds, Rectangle displayRect, AnchorStyles anchor)
    {
        bool hasStaleRightAnchor = IsAnchored(anchor, AnchorStyles.Right)
            && !IsAnchored(anchor, AnchorStyles.Left)
            && anchorInfo.Right > 0
            && (bounds.Right - displayRect.X <= displayRect.Width
                || displayRect.Width > anchorInfo.DisplayRectangle.Width);

        bool hasStaleBottomAnchor = IsAnchored(anchor, AnchorStyles.Bottom)
            && !IsAnchored(anchor, AnchorStyles.Top)
            && anchorInfo.Bottom > 0
            && (bounds.Bottom - displayRect.Y <= displayRect.Height
                || displayRect.Height > anchorInfo.DisplayRectangle.Height);

        return hasStaleRightAnchor || hasStaleBottomAnchor;
    }

    private static bool ShouldRefreshAnchorInfoForStaleStretchAnchors(
        IArrangedElement element,
        AnchorInfo anchorInfo,
        Rectangle bounds,
        Rectangle displayRect,
        AnchorStyles anchor,
        out bool useSpecifiedDisplayRectangleForStretchRefresh)
    {
        useSpecifiedDisplayRectangleForStretchRefresh = false;

        if (!ScaleHelper.IsScalingRequirementMet || element.Container is not { } container)
        {
            return false;
        }

        Rectangle specifiedContainerBounds = CommonProperties.GetSpecifiedBounds(container);
        Rectangle specifiedElementBounds = CommonProperties.GetSpecifiedBounds(element);

        if (!HasValidSpecifiedBounds(specifiedContainerBounds, specifiedElementBounds))
        {
            return false;
        }

        Rectangle specifiedDisplayRect = GetDisplayRectangleForSpecifiedContainerBounds(container, displayRect, specifiedContainerBounds);
        bool hasDisplayRectangleGrowth = HasDisplayRectangleGrowth(anchorInfo.DisplayRectangle, displayRect);

        int effectiveSpecifiedWidth = GetEffectiveSpecifiedSizeForStretchRefresh(
            anchorInfo.DisplayRectangle.Width,
            anchorInfo.Left,
            anchorInfo.Right,
            specifiedElementBounds.Width);
        int effectiveSpecifiedHeight = GetEffectiveSpecifiedSizeForStretchRefresh(
            anchorInfo.DisplayRectangle.Height,
            anchorInfo.Top,
            anchorInfo.Bottom,
            specifiedElementBounds.Height);

        if (hasDisplayRectangleGrowth
            && IsSpecifiedDisplayRectangleLikelyStale(specifiedDisplayRect, anchorInfo.DisplayRectangle))
        {
            return false;
        }

        bool hasStaleHorizontalStretchAnchor = IsAnchored(anchor, AnchorStyles.Left)
            && IsAnchored(anchor, AnchorStyles.Right)
            && anchorInfo.Right < 0
            && hasDisplayRectangleGrowth
            && IsProjectedStretchAnchorSmallerThanExpected(
                displayRect.Width,
                anchorInfo.Left,
                anchorInfo.Right,
                effectiveSpecifiedWidth,
                specifiedDisplayRect.Width);

        bool hasStaleVerticalStretchAnchor = IsAnchored(anchor, AnchorStyles.Top)
            && IsAnchored(anchor, AnchorStyles.Bottom)
            && anchorInfo.Bottom < 0
            && hasDisplayRectangleGrowth
            && IsProjectedStretchAnchorSmallerThanExpected(
                displayRect.Height,
                anchorInfo.Top,
                anchorInfo.Bottom,
                effectiveSpecifiedHeight,
                specifiedDisplayRect.Height);

        bool hasOversizedHorizontalStretchAnchor = IsAnchored(anchor, AnchorStyles.Left)
            && IsAnchored(anchor, AnchorStyles.Right)
            && anchorInfo.Right > 0
            && IsLargerThanExpectedStretchedSize(bounds.Width, specifiedElementBounds.Width, displayRect.Width, specifiedDisplayRect.Width);

        bool hasOversizedVerticalStretchAnchor = IsAnchored(anchor, AnchorStyles.Top)
            && IsAnchored(anchor, AnchorStyles.Bottom)
            && anchorInfo.Bottom > 0
            && IsLargerThanExpectedStretchedSize(bounds.Height, specifiedElementBounds.Height, displayRect.Height, specifiedDisplayRect.Height);

        useSpecifiedDisplayRectangleForStretchRefresh = hasOversizedHorizontalStretchAnchor || hasOversizedVerticalStretchAnchor;

        return hasStaleHorizontalStretchAnchor
            || hasStaleVerticalStretchAnchor
            || hasOversizedHorizontalStretchAnchor
            || hasOversizedVerticalStretchAnchor;
    }

    internal static void RefreshAnchorInfoForDisplayRectangleGrowth(
        IArrangedElement element,
        AnchorInfo anchorInfo,
        Rectangle displayRect,
        AnchorStyles anchor,
        bool isStretchAnchorRefresh,
        bool useSpecifiedDisplayRectangleForStretchRefresh = false)
    {
        Rectangle cachedElementBounds = GetCachedBounds(element);
        Rectangle elementBounds = cachedElementBounds;
        Rectangle anchorDisplayRect = displayRect;
        Rectangle originalDisplayRect = anchorInfo.DisplayRectangle;

        if (element.Container is { } container)
        {
            Rectangle specifiedContainerBounds = CommonProperties.GetSpecifiedBounds(container);
            Rectangle specifiedElementBounds = CommonProperties.GetSpecifiedBounds(element);

            if (HasValidSpecifiedBounds(specifiedContainerBounds, specifiedElementBounds))
            {
                anchorDisplayRect = isStretchAnchorRefresh && !useSpecifiedDisplayRectangleForStretchRefresh
                    ? originalDisplayRect
                    : GetDisplayRectangleForSpecifiedContainerBounds(container, displayRect, specifiedContainerBounds);

                bool useCachedBoundsForStretchRefresh = isStretchAnchorRefresh
                    && ShouldUseCachedBoundsForStretchRefresh(anchor, cachedElementBounds, specifiedElementBounds, anchorDisplayRect);

                elementBounds = useCachedBoundsForStretchRefresh
                    ? cachedElementBounds
                    : specifiedElementBounds;
            }
        }

        ResetAnchorInfo(anchorInfo, elementBounds, anchorDisplayRect, anchor);
    }

    private static bool ShouldUseCachedBoundsForStretchRefresh(
        AnchorStyles anchor,
        Rectangle cachedElementBounds,
        Rectangle specifiedElementBounds,
        Rectangle anchorDisplayRect)
    {
        bool isHorizontalStretch = IsAnchored(anchor, AnchorStyles.Left)
            && IsAnchored(anchor, AnchorStyles.Right);
        bool isVerticalStretch = IsAnchored(anchor, AnchorStyles.Top)
            && IsAnchored(anchor, AnchorStyles.Bottom);

        bool specifiedIsSignificantlyWiderThanDisplay = specifiedElementBounds.Width > anchorDisplayRect.Width + GetSignificantStretchDelta(anchorDisplayRect.Width);
        bool specifiedIsSignificantlyTallerThanDisplay = specifiedElementBounds.Height > anchorDisplayRect.Height + GetSignificantStretchDelta(anchorDisplayRect.Height);
        bool cachedWidthFitsDisplay = cachedElementBounds.Width <= anchorDisplayRect.Width + 1;
        bool cachedHeightFitsDisplay = cachedElementBounds.Height <= anchorDisplayRect.Height + 1;

        return (isHorizontalStretch && specifiedIsSignificantlyWiderThanDisplay && cachedWidthFitsDisplay)
            || (isVerticalStretch && specifiedIsSignificantlyTallerThanDisplay && cachedHeightFitsDisplay);
    }

    private static int GetSignificantStretchDelta(int size)
    {
        // Ignore tiny differences caused by borders/rounding, but treat large drifts as stale metadata.
        return Math.Max(MinimumSignificantStretchDelta, size / MinimumSignificantStretchDelta);
    }

    private static void ResetAnchorInfo(AnchorInfo anchorInfo, Rectangle elementBounds, Rectangle displayRect, AnchorStyles anchor)
    {
        anchorInfo.DisplayRectangle = displayRect;
        anchorInfo.Left = elementBounds.Left - displayRect.X;
        anchorInfo.Top = elementBounds.Top - displayRect.Y;
        anchorInfo.Right = elementBounds.Right - displayRect.X;
        anchorInfo.Bottom = elementBounds.Bottom - displayRect.Y;

        if (IsAnchored(anchor, AnchorStyles.Right))
        {
            anchorInfo.Right -= displayRect.Width;

            if (!IsAnchored(anchor, AnchorStyles.Left))
            {
                anchorInfo.Left -= displayRect.Width;
            }
        }
        else if (!IsAnchored(anchor, AnchorStyles.Left))
        {
            anchorInfo.Right -= displayRect.Width / 2;
            anchorInfo.Left -= displayRect.Width / 2;
        }

        if (IsAnchored(anchor, AnchorStyles.Bottom))
        {
            anchorInfo.Bottom -= displayRect.Height;

            if (!IsAnchored(anchor, AnchorStyles.Top))
            {
                anchorInfo.Top -= displayRect.Height;
            }
        }
        else if (!IsAnchored(anchor, AnchorStyles.Top))
        {
            anchorInfo.Bottom -= displayRect.Height / 2;
            anchorInfo.Top -= displayRect.Height / 2;
        }
    }

    private static bool HasValidSpecifiedBounds(Rectangle specifiedContainerBounds, Rectangle specifiedElementBounds)
    {
        return specifiedContainerBounds.Width > 0
            && specifiedContainerBounds.Height > 0
            && specifiedElementBounds.Width > 0
            && specifiedElementBounds.Height > 0;
    }

    private static bool HasDisplayRectangleGrowth(Rectangle originalDisplayRect, Rectangle currentDisplayRect)
    {
        return currentDisplayRect.Width > originalDisplayRect.Width
            || currentDisplayRect.Height > originalDisplayRect.Height;
    }

    private static bool IsSmallerThanExpectedStretchedSize(int actualSize, int specifiedSize, int currentDisplaySize, int specifiedDisplaySize)
    {
        if (specifiedSize <= 0 || currentDisplaySize <= 0 || specifiedDisplaySize <= 0)
        {
            return false;
        }

        int expectedSize = specifiedSize + Math.Max(0, currentDisplaySize - specifiedDisplaySize);

        // Allow 1-pixel tolerance for integer rounding.
        return actualSize + 1 < expectedSize;
    }

    private static bool IsProjectedStretchAnchorSmallerThanExpected(
        int currentDisplaySize,
        int leadingAnchor,
        int trailingAnchor,
        int specifiedSize,
        int specifiedDisplaySize)
    {
        if (specifiedSize <= 0 || currentDisplaySize <= 0 || specifiedDisplaySize <= 0)
        {
            return false;
        }

        int projectedSize = Math.Max(0, currentDisplaySize + trailingAnchor - leadingAnchor);

        return IsSmallerThanExpectedStretchedSize(projectedSize, specifiedSize, currentDisplaySize, specifiedDisplaySize);
    }

    private static int GetEffectiveSpecifiedSizeForStretchRefresh(
        int anchorDisplaySize,
        int leadingAnchor,
        int trailingAnchor,
        int specifiedSize)
    {
        if (anchorDisplaySize <= 0 || specifiedSize <= 0)
        {
            return specifiedSize;
        }

        int baselineProjectedSize = Math.Max(0, anchorDisplaySize + trailingAnchor - leadingAnchor);
        int staleSpecifiedDelta = GetSignificantStretchDelta(baselineProjectedSize);

        return specifiedSize > baselineProjectedSize + staleSpecifiedDelta
            ? baselineProjectedSize
            : specifiedSize;
    }

    private static bool IsLargerThanExpectedStretchedSize(int actualSize, int specifiedSize, int currentDisplaySize, int specifiedDisplaySize)
    {
        if (specifiedSize <= 0 || currentDisplaySize <= 0 || specifiedDisplaySize <= 0)
        {
            return false;
        }

        int expectedSize = Math.Max(0, specifiedSize + currentDisplaySize - specifiedDisplaySize);

        // Allow 1-pixel tolerance for integer rounding.
        return actualSize > expectedSize + 1;
    }

    private static bool IsSpecifiedDisplayRectangleLikelyStale(Rectangle specifiedDisplayRect, Rectangle anchorDisplayRect)
    {
        int widthDelta = GetSignificantStretchDelta(anchorDisplayRect.Width);
        int heightDelta = GetSignificantStretchDelta(anchorDisplayRect.Height);

        return specifiedDisplayRect.Width > anchorDisplayRect.Width + widthDelta
            || specifiedDisplayRect.Height > anchorDisplayRect.Height + heightDelta;
    }

    private static Rectangle GetDisplayRectangleForSpecifiedContainerBounds(IArrangedElement container, Rectangle displayRect, Rectangle specifiedContainerBounds)
    {
        int nonClientWidth = Math.Max(0, container.Bounds.Width - displayRect.Width);
        int nonClientHeight = Math.Max(0, container.Bounds.Height - displayRect.Height);

        int displayWidth = Math.Max(0, specifiedContainerBounds.Width - nonClientWidth);
        int displayHeight = Math.Max(0, specifiedContainerBounds.Height - nonClientHeight);

        return new Rectangle(displayRect.X, displayRect.Y, displayWidth, displayHeight);
    }
}
