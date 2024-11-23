// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.Drawing;

namespace System.Windows.Forms.Layout;

// Some LayoutEngines extend the same properties to their children. We want
// these extended properties to retain their value when moved from one container
// to another. (For example, set BoxStretchInternal on a control in FlowPanel and then move
// the control to GridPanel.)  CommonProperties is a place to define keys and
// accessors for such properties.
internal partial class CommonProperties
{
    private static readonly int s_layoutStateProperty = PropertyStore.CreateKey();
    private static readonly int s_specifiedBoundsProperty = PropertyStore.CreateKey();
    private static readonly int s_preferredSizeCacheProperty = PropertyStore.CreateKey();
    private static readonly int s_paddingProperty = PropertyStore.CreateKey();

    private static readonly int s_marginProperty = PropertyStore.CreateKey();
    private static readonly int s_minimumSizeProperty = PropertyStore.CreateKey();
    private static readonly int s_maximumSizeProperty = PropertyStore.CreateKey();
    private static readonly int s_layoutBoundsProperty = PropertyStore.CreateKey();

    internal const ContentAlignment DefaultAlignment = ContentAlignment.TopLeft;
    internal const AnchorStyles DefaultAnchor = AnchorStyles.Top | AnchorStyles.Left;
    internal const bool DefaultAutoSize = false;

    internal const DockStyle DefaultDock = DockStyle.None;
    internal static Padding DefaultMargin { get; } = new(3);
    internal static Size DefaultMinimumSize { get; } = new(0, 0);
    internal static Size DefaultMaximumSize { get; } = new(0, 0);

    // DO NOT MOVE THE FOLLOWING 4 SECTIONS
    // We have done some special arranging here so that if the first 7 bits of state are zero, we know
    // that the control is purely absolutely positioned and DefaultLayout does not need to do anything.

    private static readonly BitVector32.Section s_dockAndAnchorNeedsLayoutSection = BitVector32.CreateSection(0x7F);
    private static readonly BitVector32.Section s_dockAndAnchorSection = BitVector32.CreateSection(0x0F);
    private static readonly BitVector32.Section s_dockModeSection = BitVector32.CreateSection(0x01, s_dockAndAnchorSection);
    private static readonly BitVector32.Section s_autoSizeSection = BitVector32.CreateSection(0x01, s_dockModeSection);
    private static readonly BitVector32.Section s_boxStretchInternalSection = BitVector32.CreateSection(0x03, s_autoSizeSection);
    private static readonly BitVector32.Section s_anchorNeverShrinksSection = BitVector32.CreateSection(0x01, s_boxStretchInternalSection);
    private static readonly BitVector32.Section s_flowBreakSection = BitVector32.CreateSection(0x01, s_anchorNeverShrinksSection);
    private static readonly BitVector32.Section s_selfAutoSizingSection = BitVector32.CreateSection(0x01, s_flowBreakSection);
    private static readonly BitVector32.Section s_autoSizeModeSection = BitVector32.CreateSection(0x01, s_selfAutoSizingSection);

    #region AppliesToAllLayouts

    /// <summary>
    ///  Removes the maximum size from the property store, making it "unset".
    /// </summary>
    internal static void ClearMaximumSize(IArrangedElement element) => element.Properties.RemoveValue(s_maximumSizeProperty);

    /// <summary>
    ///  Determines whether or not the <see cref="Layout"/> <see cref="LayoutEngine"/>s
    ///  think the element is auto sized.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   A control can thwart the layout engine by overriding its virtual <see cref="Control.AutoSize"/>
    ///   property and not calling base. If <see cref="GetAutoSize(IArrangedElement)"/> is false, a layout engine will
    ///   treat it as AutoSize = false and not size the element to its preferred size.
    ///  </para>
    /// </remarks>
    internal static bool GetAutoSize(IArrangedElement element)
    {
        BitVector32 state = GetLayoutState(element);
        int value = state[s_autoSizeSection];
        return value != 0;
    }

    /// <summary>
    ///  Returns the margin (exterior space) for an item.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   We can not use our pattern of passing the default value into <see cref="Control.Margin"/> because the
    ///   LayoutEngines read this property and do not know each element's <see cref="Control.DefaultMargin"/>.
    ///   Instead the element sets the margin in its ctor.
    ///  </para>
    /// </remarks>
    internal static Padding GetMargin(IArrangedElement element)
    {
        if (element.Properties.TryGetValue(s_marginProperty, out Padding padding))
        {
            return padding;
        }

        return DefaultMargin;
    }

    /// <summary>
    ///  Returns the maximum size for an element.
    /// </summary>
    internal static Size GetMaximumSize(IArrangedElement element, Size defaultMaximumSize)
    {
        if (element.Properties.TryGetValue(s_maximumSizeProperty, out Size size))
        {
            return size;
        }

        return defaultMaximumSize;
    }

    /// <summary>
    ///  Returns the minimum size for an element.
    /// </summary>
    internal static Size GetMinimumSize(IArrangedElement element, Size defaultMinimumSize)
    {
        if (element.Properties.TryGetValue(s_minimumSizeProperty, out Size size))
        {
            return size;
        }

        return defaultMinimumSize;
    }

    /// <summary>
    ///  Returns the padding for an element.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Typically the padding is accounted for in either the <see cref="Control.DisplayRectangle"/> calculation
    ///   and/or the <see cref="Control.GetPreferredSize(Size)"/> calculation of a control.
    ///  </para>
    ///  <para>
    ///   NOTE:  <see cref="LayoutEngine"/>s should never read this property. Padding gets incorporated into
    ///   layout by modifying what the control reports for preferred size.
    ///  </para>
    /// </remarks>
    internal static Padding GetPadding(IArrangedElement element, Padding defaultPadding)
    {
        if (element.Properties.TryGetValue(s_paddingProperty, out Padding padding))
        {
            return padding;
        }

        return defaultPadding;
    }

    /// <summary>
    ///  Returns the last size manually set into the element.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   See <see cref="UpdateSpecifiedBounds(IArrangedElement, int, int, int, int)"/>.
    ///  </para>
    /// </remarks>
    internal static Rectangle GetSpecifiedBounds(IArrangedElement element) =>
        element.Properties.TryGetValue(s_specifiedBoundsProperty, out Rectangle rectangle)
            && rectangle != LayoutUtils.s_maxRectangle
                ? rectangle
                : element.Bounds;

    /// <summary>
    ///  Clears out the padding from the property store.
    /// </summary>
    internal static void ResetPadding(IArrangedElement element) => element.Properties.RemoveValue(s_paddingProperty);

    /// <summary>
    ///  Sets whether or not the layout engines should treat this control as auto sized.
    /// </summary>
    internal static void SetAutoSize(IArrangedElement element, bool value)
    {
        Debug.Assert(value != GetAutoSize(element), "PERF: Caller should guard against setting AutoSize to original value.");

        BitVector32 state = GetLayoutState(element);
        state[s_autoSizeSection] = value ? 1 : 0;
        SetLayoutState(element, state);
        if (!value)
        {
            // If autoSize is being turned off, restore the control to its specified bounds.
            element.SetBounds(GetSpecifiedBounds(element), BoundsSpecified.None);
        }

        Debug.Assert(GetAutoSize(element) == value, "Error detected setting AutoSize.");
    }

    /// <summary>
    ///  Sets the margin (exterior space) for an element.
    /// </summary>
    internal static void SetMargin(IArrangedElement element, Padding value)
    {
        Debug.Assert(value != GetMargin(element), "PERF: Caller should guard against setting Margin to original value.");

        element.Properties.AddValue(s_marginProperty, value);

        Debug.Assert(GetMargin(element) == value, "Error detected setting Margin.");

        LayoutTransaction.DoLayout(element.Container, element, PropertyNames.Margin);
    }

    /// <summary>
    ///  Sets the maximum size for an element.
    /// </summary>
    internal static void SetMaximumSize(IArrangedElement element, Size value)
    {
        Debug.Assert(value != GetMaximumSize(element, new Size(-7109, -7107)),
            "PERF: Caller should guard against setting MaximumSize to original value.");

        element.Properties.AddValue(s_maximumSizeProperty, value);

        // Element bounds may need to truncated to new maximum
        Rectangle bounds = element.Bounds;
        bounds.Width = Math.Min(bounds.Width, value.Width);
        bounds.Height = Math.Min(bounds.Height, value.Height);
        element.SetBounds(bounds, BoundsSpecified.Size);

        // element.SetBounds does a SetBoundsCore. We still need to explicitly refresh parent layout.
        LayoutTransaction.DoLayout(element.Container, element, PropertyNames.MaximumSize);

        Debug.Assert(GetMaximumSize(element, new Size(-7109, -7107)) == value, "Error detected setting MaximumSize.");
    }

    /// <summary>
    ///  Sets the minimum size for an element.
    /// </summary>
    internal static void SetMinimumSize(IArrangedElement element, Size value)
    {
        Debug.Assert(value != GetMinimumSize(element, new Size(-7109, -7107)),
            "PERF: Caller should guard against setting MinimumSize to original value.");

        element.Properties.AddValue(s_minimumSizeProperty, value);

        using (new LayoutTransaction(element.Container as Control, element, PropertyNames.MinimumSize))
        {
            // Element bounds may need to inflated to new minimum
            Rectangle bounds = element.Bounds;
            bounds.Width = Math.Max(bounds.Width, value.Width);
            bounds.Height = Math.Max(bounds.Height, value.Height);
            element.SetBounds(bounds, BoundsSpecified.Size);
        }

        Debug.Assert(GetMinimumSize(element, new Size(-7109, -7107)) == value, "Error detected setting MinimumSize.");
    }

    /// <summary>
    ///  Sets the padding (interior space) for an element.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   See <see cref="GetPadding(IArrangedElement, Padding)"/> for more details. NOTE: It is the callers
    ///   responsibility to do layout. See <see cref="Control.Padding"/> for details.
    ///  </para>
    /// </remarks>
    internal static void SetPadding(IArrangedElement element, Padding value)
    {
        Debug.Assert(value != GetPadding(element, new Padding(-7105)),
            "PERF: Caller should guard against setting Padding to original value.");

        value = LayoutUtils.ClampNegativePaddingToZero(value);
        element.Properties.AddValue(s_paddingProperty, value);

        Debug.Assert(GetPadding(element, new Padding(-7105)) == value, "Error detected setting Padding.");
    }

    /// <summary>
    ///  Updates the specified bounds for an element.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The main purpose of this function is to remember what size someone specified in the <see cref="Control.Size"/>,
    ///   <see cref="Control.Width"/>, <see cref="Control.Height"/>, <see cref="Control.Bounds"/>, property. (Its the
    ///   whole reason the <see cref="BoundsSpecified"/> enum exists.) Consider this scenario. You set a <see cref="Button"/>
    ///   to <see cref="DockStyle.Fill"/>, then <see cref="DockStyle.None"/>. When filled, the <see cref="Control.Size"/>
    ///   changed to 300,300. When you set it back to <see cref="DockStyle.None"/> the size switches back to 100,23.
    ///   How does this happen?
    ///  </para>
    ///  <para>
    ///   Setting the control to <see cref="DockStyle.Fill"/> (via <see cref="DefaultLayout"/> engine)
    ///   element.SetBounds(newElementBounds, BoundsSpecified.None);
    ///  </para>
    ///  <para>
    ///   (If someone happens to set the Size property here the specified bounds gets updated via Control.Size)
    ///   SetBounds(x, y, value.Width, value.Height, BoundsSpecified.Size);
    ///  </para>
    ///  <para>
    ///   Setting the control to <see cref="DockStyle.None"/> (via DefaultLayout.SetDock)
    ///   element.SetBounds(CommonProperties.GetSpecifiedBounds(element), BoundsSpecified.None);
    ///  </para>
    /// </remarks>
    internal static void UpdateSpecifiedBounds(IArrangedElement element, int x, int y, int width, int height, BoundsSpecified specified)
    {
        Rectangle originalBounds = GetSpecifiedBounds(element);

        // PERF note: Bitwise operator usage intentional to optimize out branching.

        bool xChangedButNotSpecified = ((specified & BoundsSpecified.X) == BoundsSpecified.None) & x != originalBounds.X;
        bool yChangedButNotSpecified = ((specified & BoundsSpecified.Y) == BoundsSpecified.None) & y != originalBounds.Y;
        bool wChangedButNotSpecified = ((specified & BoundsSpecified.Width) == BoundsSpecified.None) & width != originalBounds.Width;
        bool hChangedButNotSpecified = ((specified & BoundsSpecified.Height) == BoundsSpecified.None) & height != originalBounds.Height;

        if (xChangedButNotSpecified | yChangedButNotSpecified | wChangedButNotSpecified | hChangedButNotSpecified)
        {
            // If any of them are changed and specified cache the new value.

            if (!xChangedButNotSpecified)
            {
                originalBounds.X = x;
            }

            if (!yChangedButNotSpecified)
            {
                originalBounds.Y = y;
            }

            if (!wChangedButNotSpecified)
            {
                originalBounds.Width = width;
            }

            if (!hChangedButNotSpecified)
            {
                originalBounds.Height = height;
            }

            element.Properties.AddValue(s_specifiedBoundsProperty, originalBounds);
        }
        else
        {
            // SetBoundsCore is going to call this a lot with the same bounds. Avoid the set object
            // (which indirectly may causes an allocation) if we can.
            if (element.Properties.ContainsKey(s_specifiedBoundsProperty))
            {
                // use MaxRectangle instead of null so we can reuse the SizeWrapper in the property store.
                element.Properties.AddValue(s_specifiedBoundsProperty, LayoutUtils.s_maxRectangle);
            }
        }
    }

    // Used by ToolStripControlHost.Size.
    internal static void UpdateSpecifiedBounds(IArrangedElement element, int x, int y, int width, int height)
    {
        Rectangle bounds = new(x, y, width, height);
        element.Properties.AddValue(s_specifiedBoundsProperty, bounds);
    }

    /// <summary>
    ///  Clears the preferred size cached for any control that overrides the internal
    ///  <see cref="Control.GetPreferredSizeCore(Size)"/> method. DO NOT CALL DIRECTLY
    ///  unless it is understood how the size of the control is going to be updated.
    /// </summary>
    internal static void xClearPreferredSizeCache(IArrangedElement element)
    {
        element.Properties.AddValue(s_preferredSizeCacheProperty, LayoutUtils.s_invalidSize);
        Debug.Assert(xGetPreferredSizeCache(element) == Size.Empty, "Error detected in xClearPreferredSizeCache.");
    }

    /// <summary>
    ///  Clears all the caching for an <see cref="IArrangedElement"/> hierarchy. Typically done in dispose.
    /// </summary>
    internal static void xClearAllPreferredSizeCaches(IArrangedElement start)
    {
        xClearPreferredSizeCache(start);

        ArrangedElementCollection controlsCollection = start.Children;

        // This may have changed the sizes of our children.
        // PERFNOTE: This is more efficient than using Foreach. Foreach
        // forces the creation of an array subset enum each time we
        // enumerate
        for (int i = 0; i < controlsCollection.Count; i++)
        {
            xClearAllPreferredSizeCaches(controlsCollection[i]);
        }
    }

    /// <summary>
    ///  This value is the cached result of the return value from a control's
    ///  <see cref="Control.GetPreferredSizeCore(Size)"/> implementation when asked for a constraining
    ///  value of <see cref="LayoutUtils.s_maxSize"/> (or <see cref="Size.Empty"/> too).
    /// </summary>
    internal static Size xGetPreferredSizeCache(IArrangedElement element)
    {
        if (element.Properties.TryGetValue(s_preferredSizeCacheProperty, out Size size) && (size != LayoutUtils.s_invalidSize))
        {
            return size;
        }

        return Size.Empty;
    }

    /// <summary>
    ///  Sets a control's preferred size. See <see cref="xGetPreferredSizeCache(IArrangedElement)"/>.
    /// </summary>
    internal static void xSetPreferredSizeCache(IArrangedElement element, Size value)
    {
        Debug.Assert(
            value == Size.Empty || value != xGetPreferredSizeCache(element),
            "PERF: Caller should guard against setting PreferredSizeCache to original value.");
        element.Properties.AddValue(s_preferredSizeCacheProperty, value);
        Debug.Assert(xGetPreferredSizeCache(element) == value, "Error detected in xGetPreferredSizeCache.");
    }

    #endregion

    #region DockAndAnchorLayoutSpecific

    /// <summary>
    ///  Returns whether or not a control should snap to its smallest size
    ///  or retain its original size and only grow if the preferred size is larger.
    ///  We tried not having GrowOnly as the default, but it becomes difficult
    ///  to design panels or have Buttons maintain their default size of 100,23
    /// </summary>
    internal static AutoSizeMode GetAutoSizeMode(IArrangedElement element)
    {
        BitVector32 state = GetLayoutState(element);
        return state[s_autoSizeModeSection] == 0 ? AutoSizeMode.GrowOnly : AutoSizeMode.GrowAndShrink;
    }

    /// <summary>
    ///  Do not use. Internal property for DockAndAnchor layout.
    ///  Returns <see langword="true"/> if DefaultLayout needs to do any work for this element.
    ///  (Returns <see langword="false"/> if the element is purely absolutely positioned)
    /// </summary>
    internal static bool GetNeedsDockAndAnchorLayout(IArrangedElement element)
    {
        BitVector32 state = GetLayoutState(element);
        bool result = state[s_dockAndAnchorNeedsLayoutSection] != 0;

        Debug.Assert(
            (xGetAnchor(element) == DefaultAnchor
            && xGetDock(element) == DefaultDock
            && GetAutoSize(element) == DefaultAutoSize) != result,
            "Individual values of Anchor/Dock/AutoRelocate/Autosize contradict GetNeedsDockAndAnchorLayout().");

        return result;
    }

    /// <summary>
    ///  Do not use. Internal property for DockAndAnchor layout.
    ///  Returns <see langword="true"/> if DefaultLayout needs to do anchoring for this element.
    /// </summary>
    internal static bool GetNeedsAnchorLayout(IArrangedElement element)
    {
        BitVector32 state = GetLayoutState(element);
        bool result = (state[s_dockAndAnchorNeedsLayoutSection] != 0) && (state[s_dockModeSection] == (int)DockAnchorMode.Anchor);

        Debug.Assert(
            (xGetAnchor(element) != DefaultAnchor
            || (GetAutoSize(element) != DefaultAutoSize && xGetDock(element) == DockStyle.None)) == result,
            "Individual values of Anchor/Dock/AutoRelocate/Autosize contradict GetNeedsAnchorLayout().");

        return result;
    }

    /// <summary>
    ///  Do not use. Internal property for DockAndAnchor layout.
    ///  Returns <see langword="true"/> if DefaultLayout needs to do docking for this element.
    /// </summary>
    internal static bool GetNeedsDockLayout(IArrangedElement element)
    {
        BitVector32 state = GetLayoutState(element);
        bool result = state[s_dockModeSection] == (int)DockAnchorMode.Dock && element.ParticipatesInLayout;

        Debug.Assert(((xGetDock(element) != DockStyle.None) && element.ParticipatesInLayout) == result,
            "Error detected in GetNeedsDockLayout().");

        return result;
    }

    /// <summary>
    ///  Compat flag for controls that previously sized themselves.
    ///  Some controls rolled their own implementation of AutoSize in V1 for Dock and Anchor
    ///  In V2, the LayoutEngine is the one responsible for sizing the child items when
    ///  they're AutoSized. For new layout engines, the controls will let the layout engine
    ///  size them, but for DefaultLayout, they're left to size themselves.
    /// </summary>
    internal static bool GetSelfAutoSizeInDefaultLayout(IArrangedElement element)
    {
        BitVector32 state = GetLayoutState(element);
        int value = state[s_selfAutoSizingSection];
        return (value == 1);
    }

    /// <summary>
    ///  Returns whether or not a control should snap to its smallest size
    ///  or retain its original size and only grow if the preferred size is larger.
    ///  We tried not having GrowOnly as the default, but it becomes difficult
    ///  to design panels or have Buttons maintain their default size of 100,23.
    /// </summary>
    internal static void SetAutoSizeMode(IArrangedElement element, AutoSizeMode mode)
    {
        BitVector32 state = GetLayoutState(element);
        state[s_autoSizeModeSection] = mode == AutoSizeMode.GrowAndShrink ? 1 : 0;
        SetLayoutState(element, state);
    }

    /// <summary>
    ///  Compat flag for controls that previously sized themselves.
    ///  See <see cref="GetSelfAutoSizeInDefaultLayout(IArrangedElement)"/> comments.
    /// </summary>
    internal static bool ShouldSelfSize(IArrangedElement element)
    {
        if (GetAutoSize(element))
        {
            // Check for legacy layout engine
            if (element.Container is Control { LayoutEngine: DefaultLayout })
            {
                return GetSelfAutoSizeInDefaultLayout(element);
            }

            // Unknown element type or new LayoutEngine which should set the size to the preferredSize anyways.
            return false;
        }

        // Autosize false things should selfsize.
        return true;
    }

    /// <summary>
    ///  Compat flag for controls that previously sized themselves.
    ///  See <see cref="GetSelfAutoSizeInDefaultLayout(IArrangedElement)"/> comments.
    /// </summary>
    internal static void SetSelfAutoSizeInDefaultLayout(IArrangedElement element, bool value)
    {
        Debug.Assert(value != GetSelfAutoSizeInDefaultLayout(element), "PERF: Caller should guard against setting AutoSize to original value.");

        BitVector32 state = GetLayoutState(element);
        state[s_selfAutoSizingSection] = value ? 1 : 0;
        SetLayoutState(element, state);

        Debug.Assert(GetSelfAutoSizeInDefaultLayout(element) == value, "Error detected setting AutoSize.");
    }

    /// <summary>
    ///  Do not use this. Use <see cref="DefaultLayout.GetAnchor(IArrangedElement)"/>.
    ///  NOTE that Dock and Anchor are exclusive, so we store their enums in the same section.
    /// </summary>
    internal static AnchorStyles xGetAnchor(IArrangedElement element)
    {
        BitVector32 state = GetLayoutState(element);
        AnchorStyles value = (AnchorStyles)state[s_dockAndAnchorSection];
        DockAnchorMode mode = (DockAnchorMode)state[s_dockModeSection];

        // If we are docked, or if it the value is 0, we return DefaultAnchor
        value = mode == DockAnchorMode.Anchor ? xTranslateAnchorValue(value) : DefaultAnchor;

        Debug.Assert(mode == DockAnchorMode.Anchor || value == DefaultAnchor, "xGetAnchor needs to return DefaultAnchor when docked.");
        return value;
    }

    /// <summary>
    ///  Do not use. Internal property for DockAndAnchor layout.
    ///  Returns <see langword="true"/> if the element is both AutoSize and Anchored.
    /// </summary>
    internal static bool xGetAutoSizedAndAnchored(IArrangedElement element)
    {
        BitVector32 state = GetLayoutState(element);

        if (state[s_selfAutoSizingSection] != 0)
        {
            return false;
        }

        bool result = (state[s_autoSizeSection] != 0) && (state[s_dockModeSection] == (int)DockAnchorMode.Anchor);
        Debug.Assert(result == (GetAutoSize(element) && xGetDock(element) == DockStyle.None),
            "Error detected in xGetAutoSizeAndAnchored.");

        return result;
    }

    ///  xGetDock
    ///  Do not use this. Use DefaultLayout.GetDock.
    ///  Note that Dock and Anchor are exclusive, so we store their enums in the same section.
    internal static DockStyle xGetDock(IArrangedElement element)
    {
        BitVector32 state = GetLayoutState(element);
        DockStyle value = (DockStyle)state[s_dockAndAnchorSection];
        DockAnchorMode mode = (DockAnchorMode)state[s_dockModeSection];

        // If we are anchored we return DefaultDock
        value = mode == DockAnchorMode.Dock ? value : DefaultDock;
        SourceGenerated.EnumValidator.Validate(value);

        Debug.Assert(mode == DockAnchorMode.Dock || value == DefaultDock,
            "xGetDock needs to return the DefaultDock style when not docked.");

        return value;
    }

    ///  xSetAnchor -
    ///  Do not use this. Use DefaultLayout.SetAnchor.
    ///  Note that Dock and Anchor are exclusive, so we store their enums in the same section.
    internal static void xSetAnchor(IArrangedElement element, AnchorStyles value)
    {
        Debug.Assert(value != xGetAnchor(element), "PERF: Caller should guard against setting Anchor to original value.");

        BitVector32 state = GetLayoutState(element);

        // We translate DefaultAnchor to zero - see the _dockAndAnchorNeedsLayoutSection section above.
        state[s_dockAndAnchorSection] = (int)xTranslateAnchorValue(value);
        state[s_dockModeSection] = (int)DockAnchorMode.Anchor;

        SetLayoutState(element, state);

        Debug.Assert(GetLayoutState(element)[s_dockModeSection] == (int)DockAnchorMode.Anchor,
            "xSetAnchor did not set mode to Anchor.");
    }

    ///  xSetDock
    ///  Do not use this. Use DefaultLayout.SetDock.
    ///  Note that Dock and Anchor are exclusive, so we store their enums in the same section.
    internal static void xSetDock(IArrangedElement element, DockStyle value)
    {
        Debug.Assert(value != xGetDock(element), "PERF: Caller should guard against setting Dock to original value.");
        SourceGenerated.EnumValidator.Validate(value);

        BitVector32 state = GetLayoutState(element);

        state[s_dockAndAnchorSection] = (int)value;     // See xTranslateAnchorValue for why this works with Dock.None.
        state[s_dockModeSection] = (int)(value == DockStyle.None ? DockAnchorMode.Anchor : DockAnchorMode.Dock);

        SetLayoutState(element, state);

        Debug.Assert(xGetDock(element) == value, "Error detected setting Dock.");
        Debug.Assert((GetLayoutState(element)[s_dockModeSection] == (int)DockAnchorMode.Dock)
            == (value != DockStyle.None), "xSetDock set DockMode incorrectly.");
    }

    /// <summary>
    ///  Helper method for xGetAnchor / xSetAnchor.
    ///  We store anchor DefaultAnchor as None and vice versa.
    ///  We either had to do this or map Dock.None to DefaultAnchor (Dock and Anchor share the same section
    ///  in LayoutState.) Mapping DefaultAnchor to 0 is nicer because we do not need to allocate anything in
    ///  the PropertyStore to get a 0 back from PropertyStore.GetInteger().
    /// </summary>
    private static AnchorStyles xTranslateAnchorValue(AnchorStyles anchor) => anchor switch
    {
        AnchorStyles.None => DefaultAnchor,
        DefaultAnchor => AnchorStyles.None,
        _ => anchor,
    };

    #endregion

    #region FlowLayoutSpecific
    internal static bool GetFlowBreak(IArrangedElement element)
    {
        BitVector32 state = GetLayoutState(element);
        int value = state[s_flowBreakSection];
        return value == 1;
    }

    ///  SetFlowBreak
    ///  Use FlowLayoutSettings.SetFlowBreak instead.
    ///  See GetFlowBreak.
    internal static void SetFlowBreak(IArrangedElement element, bool value)
    {
        Debug.Assert(value != GetFlowBreak(element), "PERF: Caller should guard against setting FlowBreak to original value.");

        BitVector32 state = GetLayoutState(element);
        state[s_flowBreakSection] = value ? 1 : 0;
        SetLayoutState(element, state);

        LayoutTransaction.DoLayout(element.Container, element, PropertyNames.FlowBreak);

        Debug.Assert(GetFlowBreak(element) == value, "Error detected setting SetFlowBreak.");
    }
    #endregion
    #region AutoScrollSpecific

    /// <summary>
    ///  This is the size used to determine whether or not we need scrollbars.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Used if the layout engine always wants to return the same layout bounds regardless
    ///   of how it lays out. Example is TLP in RTL and LTR.
    ///  </para>
    /// </remarks>
    internal static Size GetLayoutBounds(IArrangedElement element)
    {
        if (element.Properties.TryGetValue(s_layoutBoundsProperty, out Size size))
        {
            return size;
        }

        return Size.Empty;
    }

    /// <summary>
    ///  This is the size used to determine whether or not we need scrollbars.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The <see cref="TableLayout"/> engine now calls <see cref="SetLayoutBounds(IArrangedElement, Size)"/> when it
    ///   is done with its layout. The layout bounds are the total column width and the total row height.
    ///   <see cref="ScrollableControl"/> checks if the layout bounds has been set in the <see cref="CommonProperties"/>
    ///   when it tries to figure out if it should add scrollbars - but only if the layout engine is not the default
    ///   layout engine. If the bounds has been set, <see cref="ScrollableControl"/> will use those bounds to check if
    ///   scrollbars should be added, rather than doing its own magic to figure it out.
    ///  </para>
    /// </remarks>
    internal static void SetLayoutBounds(IArrangedElement element, Size value)
    {
        element.Properties.AddValue(s_layoutBoundsProperty, value);
    }

    /// <summary>
    ///  Returns whether we have layout bounds stored for this element.
    /// </summary>
    internal static bool HasLayoutBounds(IArrangedElement element)
    {
        return element.Properties.ContainsKey(s_layoutBoundsProperty);
    }

    #endregion
    #region InternalCommonPropertiesHelpers

    /// <summary>
    ///  Returns the layout state bit vector from the property store.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   CAREFUL: this is a copy of the state. You need to <see cref="SetLayoutState(IArrangedElement, BitVector32)"/>
    ///   to save your changes.
    ///  </para>
    /// </remarks>
    internal static BitVector32 GetLayoutState(IArrangedElement element) =>
        element.Properties.GetValueOrDefault<BitVector32>(s_layoutStateProperty);

    internal static void SetLayoutState(IArrangedElement element, BitVector32 state) =>
        element.Properties.AddValue(s_layoutStateProperty, state);
    #endregion
}
