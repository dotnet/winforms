﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

/// <summary>
///  Base class for ToolStripItems that display DropDown windows.
/// </summary>
[Designer($"System.Windows.Forms.Design.ToolStripMenuItemDesigner, {AssemblyRef.SystemDesign}")]
[DefaultProperty(nameof(DropDownItems))]
public abstract class ToolStripDropDownItem : ToolStripItem
{
    private ToolStripDropDown? _dropDown;
    private ToolStripDropDownDirection _toolStripDropDownDirection = ToolStripDropDownDirection.Default;
    private ToolTip? _hookedKeyboardTooltip;
    private static readonly object s_dropDownShowEvent = new();
    private static readonly object s_dropDownHideEvent = new();
    private static readonly object s_dropDownOpenedEvent = new();
    private static readonly object s_dropDownClosedEvent = new();
    private static readonly object s_dropDownItemClickedEvent = new();

    /// <summary>
    ///  Protected ctor so you can't create one of these without deriving from it.
    /// </summary>
    protected ToolStripDropDownItem()
    {
    }

    protected ToolStripDropDownItem(string? text, Image? image, EventHandler? onClick)
        : base(text, image, onClick)
    {
    }

    protected ToolStripDropDownItem(string? text, Image? image, EventHandler? onClick, string? name)
        : base(text, image, onClick, name)
    {
    }

    protected ToolStripDropDownItem(string? text, Image? image, params ToolStripItem[]? dropDownItems)
        : this(text, image, onClick: null)
    {
        if (dropDownItems is not null)
        {
            DropDownItems.AddRange(dropDownItems);
        }
    }

    /// <summary>
    ///  The ToolStripDropDown that will be displayed when this item is clicked.
    /// </summary>
    [TypeConverter(typeof(ReferenceConverter))]
    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.ToolStripDropDownDescr))]
    [AllowNull]
    public ToolStripDropDown DropDown
    {
        get
        {
            if (_dropDown is null)
            {
                DropDown = CreateDefaultDropDown();
                if (this is not ToolStripOverflowButton)
                {
                    _dropDown!.SetAutoGeneratedInternal(true);
                }

                if (ParentInternal is not null)
                {
                    _dropDown!.ShowItemToolTips = ParentInternal.ShowItemToolTips;
                }
            }

            return _dropDown!;
        }
        set
        {
            if (_dropDown != value)
            {
                if (_dropDown is not null)
                {
                    if (_hookedKeyboardTooltip is not null)
                    {
                        KeyboardToolTipStateMachine.Instance.Unhook(_dropDown, _hookedKeyboardTooltip);
                    }

                    _dropDown.Opened -= new EventHandler(DropDown_Opened);
                    _dropDown.Closed -= new ToolStripDropDownClosedEventHandler(DropDown_Closed);
                    _dropDown.ItemClicked -= new ToolStripItemClickedEventHandler(DropDown_ItemClicked);
                    _dropDown.UnassignDropDownItem();
                }

                _dropDown = value;
                if (_dropDown is not null)
                {
                    if (_hookedKeyboardTooltip is not null)
                    {
                        KeyboardToolTipStateMachine.Instance.Hook(_dropDown, _hookedKeyboardTooltip);
                    }

                    _dropDown.Opened += new EventHandler(DropDown_Opened);
                    _dropDown.Closed += new ToolStripDropDownClosedEventHandler(DropDown_Closed);
                    _dropDown.ItemClicked += new ToolStripItemClickedEventHandler(DropDown_ItemClicked);
                    _dropDown.AssignToDropDownItem();
                }
            }
        }
    }

    // the area which activates the dropdown.
    internal virtual Rectangle DropDownButtonArea
        => Bounds;

    [Browsable(false)]
    [SRDescription(nameof(SR.ToolStripDropDownItemDropDownDirectionDescr))]
    [SRCategory(nameof(SR.CatBehavior))]
    public ToolStripDropDownDirection DropDownDirection
    {
        get
        {
            if (_toolStripDropDownDirection == ToolStripDropDownDirection.Default)
            {
                ToolStrip? parent = ParentInternal;
                if (parent is not null)
                {
                    ToolStripDropDownDirection dropDownDirection = parent.DefaultDropDownDirection;
                    if (OppositeDropDownAlign ||
                        (RightToLeft != parent.RightToLeft && (RightToLeft != RightToLeft.Inherit)))
                    {
                        dropDownDirection = RTLTranslateDropDownDirection(dropDownDirection, RightToLeft);
                    }

                    if (IsOnDropDown)
                    {
                        // we gotta make sure that we don't collide with the existing menu.
                        Rectangle bounds = GetDropDownBounds(dropDownDirection);
                        Rectangle ownerItemBounds = new(TranslatePoint(Point.Empty, ToolStripPointType.ToolStripItemCoords, ToolStripPointType.ScreenCoords), Size);
                        Rectangle intersectionBetweenChildAndParent = Rectangle.Intersect(bounds, ownerItemBounds);

                        // grab the intersection
                        if (intersectionBetweenChildAndParent.Width >= 2)
                        {
                            RightToLeft toggledRightToLeft = (RightToLeft == RightToLeft.Yes) ? RightToLeft.No : RightToLeft.Yes;
                            ToolStripDropDownDirection newDropDownDirection = RTLTranslateDropDownDirection(dropDownDirection, toggledRightToLeft);

                            // verify that changing the dropdown direction actually causes less intersection.
                            int newIntersectionWidth = Rectangle.Intersect(GetDropDownBounds(newDropDownDirection), ownerItemBounds).Width;
                            if (newIntersectionWidth < intersectionBetweenChildAndParent.Width)
                            {
                                dropDownDirection = newDropDownDirection;
                            }
                        }
                    }

                    return dropDownDirection;
                }
            }

            // someone has set a custom override
            return _toolStripDropDownDirection;
        }
        set
        {
            // can't use Enum.IsValid as its not sequential
            switch (value)
            {
                case ToolStripDropDownDirection.AboveLeft:
                case ToolStripDropDownDirection.AboveRight:
                case ToolStripDropDownDirection.BelowLeft:
                case ToolStripDropDownDirection.BelowRight:
                case ToolStripDropDownDirection.Left:
                case ToolStripDropDownDirection.Right:
                case ToolStripDropDownDirection.Default:
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolStripDropDownDirection));
            }

            if (_toolStripDropDownDirection != value)
            {
                _toolStripDropDownDirection = value;
                if (HasDropDownItems && DropDown.Visible)
                {
                    DropDown.Location = DropDownLocation;
                }
            }
        }
    }

    /// <summary>
    ///  Occurs when the dropdown is closed
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ToolStripDropDownClosedDecr))]
    public event EventHandler? DropDownClosed
    {
        add => Events.AddHandler(s_dropDownClosedEvent, value);
        remove => Events.RemoveHandler(s_dropDownClosedEvent, value);
    }

    protected internal virtual Point DropDownLocation
    {
        get
        {
            if (ParentInternal is null || !HasDropDownItems)
            {
                return Point.Empty;
            }

            ToolStripDropDownDirection dropDownDirection = DropDownDirection;
            return GetDropDownBounds(dropDownDirection).Location;
        }
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ToolStripDropDownOpeningDescr))]
    public event EventHandler? DropDownOpening
    {
        add => Events.AddHandler(s_dropDownShowEvent, value);
        remove => Events.RemoveHandler(s_dropDownShowEvent, value);
    }

    /// <summary>
    ///  Occurs when the dropdown is opened
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ToolStripDropDownOpenedDescr))]
    public event EventHandler? DropDownOpened
    {
        add => Events.AddHandler(s_dropDownOpenedEvent, value);
        remove => Events.RemoveHandler(s_dropDownOpenedEvent, value);
    }

    /// <summary>
    ///  Returns the DropDown's items collection.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.ToolStripDropDownItemsDescr))]
    public ToolStripItemCollection DropDownItems
        => DropDown.Items;

    /// <summary>
    ///  Occurs when the dropdown is opened
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    public event ToolStripItemClickedEventHandler? DropDownItemClicked
    {
        add => Events.AddHandler(s_dropDownItemClickedEvent, value);
        remove => Events.RemoveHandler(s_dropDownItemClickedEvent, value);
    }

    [Browsable(false)]
    public virtual bool HasDropDownItems
        =>
            // Use count of visible DisplayedItems instead so that we take into account things that aren't visible
            (_dropDown is not null) && _dropDown.HasVisibleItems;

    [Browsable(false)]
    public bool HasDropDown
        => _dropDown is not null;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override bool Pressed
    {
        get
        {
            if (_dropDown is not null)
            {
                if (DropDown.AutoClose || !IsInDesignMode || (IsInDesignMode && !IsOnDropDown))
                {
                    return DropDown.OwnerItem == this && DropDown.Visible;
                }
            }

            return base.Pressed;
        }
    }

    internal virtual bool OppositeDropDownAlign
        => false;

    internal virtual void AutoHide(ToolStripItem otherItemBeingSelected)
        => HideDropDown();

    protected override AccessibleObject CreateAccessibilityInstance()
        => new ToolStripDropDownItemAccessibleObject(this);

    protected virtual ToolStripDropDown CreateDefaultDropDown()
    {
        // AutoGenerate a ToolStrip DropDown - set the property so we hook events
        return new ToolStripDropDown(this, true);
    }

    private Rectangle DropDownDirectionToDropDownBounds(ToolStripDropDownDirection dropDownDirection, Rectangle dropDownBounds)
    {
        Point offset = Point.Empty;

        switch (dropDownDirection)
        {
            case ToolStripDropDownDirection.AboveLeft:
                offset.X = -dropDownBounds.Width + Width;
                offset.Y = -dropDownBounds.Height + 1;
                break;
            case ToolStripDropDownDirection.AboveRight:
                offset.Y = -dropDownBounds.Height + 1;
                break;
            case ToolStripDropDownDirection.BelowRight:
                offset.Y = Height - 1;
                break;
            case ToolStripDropDownDirection.BelowLeft:
                offset.X = -dropDownBounds.Width + Width;
                offset.Y = Height - 1;
                break;
            case ToolStripDropDownDirection.Right:
                offset.X = Width;
                if (!IsOnDropDown)
                {
                    // overlap the toplevel toolstrip
                    offset.X--;
                }

                break;

            case ToolStripDropDownDirection.Left:
                offset.X = -dropDownBounds.Width;
                break;
        }

        Point itemScreenLocation = TranslatePoint(Point.Empty, ToolStripPointType.ToolStripItemCoords, ToolStripPointType.ScreenCoords);
        dropDownBounds.Location = new Point(itemScreenLocation.X + offset.X, itemScreenLocation.Y + offset.Y);
        dropDownBounds = WindowsFormsUtils.ConstrainToScreenWorkingAreaBounds(dropDownBounds);
        return dropDownBounds;
    }

    private void DropDown_Closed(object? sender, ToolStripDropDownClosedEventArgs e)
        => OnDropDownClosed(EventArgs.Empty);

    private void DropDown_Opened(object? sender, EventArgs e)
        => OnDropDownOpened(EventArgs.Empty);

    private void DropDown_ItemClicked(object? sender, ToolStripItemClickedEventArgs e)
        => OnDropDownItemClicked(e);

    /// <summary>
    ///  Make sure we unhook dropdown events.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (_dropDown is not null)
        {
            if (_hookedKeyboardTooltip is not null)
            {
                KeyboardToolTipStateMachine.Instance.Unhook(_dropDown, _hookedKeyboardTooltip);
            }

            _dropDown.Opened -= new EventHandler(DropDown_Opened);
            _dropDown.Closed -= new ToolStripDropDownClosedEventHandler(DropDown_Closed);
            _dropDown.ItemClicked -= new ToolStripItemClickedEventHandler(DropDown_ItemClicked);

            if (disposing && _dropDown.IsAutoGenerated)
            {
                // if we created the dropdown, dispose it and its children.
                _dropDown.Dispose();
                _dropDown = null;
            }
        }

        base.Dispose(disposing);
    }

    private Rectangle GetDropDownBounds(ToolStripDropDownDirection dropDownDirection)
    {
        Rectangle dropDownBounds = new(Point.Empty, DropDown.GetSuggestedSize());
        // calculate the offset from the upper left hand corner of the item.
        dropDownBounds = DropDownDirectionToDropDownBounds(dropDownDirection, dropDownBounds);

        // we should make sure we don't obscure the owner item.
        Rectangle itemScreenBounds = new(TranslatePoint(Point.Empty, ToolStripPointType.ToolStripItemCoords, ToolStripPointType.ScreenCoords), Size);

        if (Rectangle.Intersect(dropDownBounds, itemScreenBounds).Height > 1)
        {
            bool rtl = (RightToLeft == RightToLeft.Yes);

            // try positioning to the left
            if (Rectangle.Intersect(dropDownBounds, itemScreenBounds).Width > 1)
            {
                dropDownBounds = DropDownDirectionToDropDownBounds(!rtl ? ToolStripDropDownDirection.Right : ToolStripDropDownDirection.Left, dropDownBounds);
            }

            // try positioning to the right
            if (Rectangle.Intersect(dropDownBounds, itemScreenBounds).Width > 1)
            {
                dropDownBounds = DropDownDirectionToDropDownBounds(!rtl ? ToolStripDropDownDirection.Left : ToolStripDropDownDirection.Right, dropDownBounds);
            }
        }

        return dropDownBounds;
    }

    /// <summary>
    ///  Hides the DropDown, if it is visible.
    /// </summary>
    public void HideDropDown()
    {
        // consider - CloseEventArgs to prevent shutting down.
        OnDropDownHide(EventArgs.Empty);

        if (_dropDown is not null && _dropDown.Visible)
        {
            DropDown.Visible = false;

            AccessibilityNotifyClients(AccessibleEvents.StateChange);
            AccessibilityNotifyClients(AccessibleEvents.NameChange);
        }
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        _dropDown?.OnOwnerItemFontChanged(EventArgs.Empty);
    }

    protected override void OnBoundsChanged()
    {
        base.OnBoundsChanged();
        // Reset the Bounds...
        if (_dropDown is not null && _dropDown.Visible)
        {
            _dropDown.Bounds = GetDropDownBounds(DropDownDirection);
        }
    }

    protected override void OnRightToLeftChanged(EventArgs e)
    {
        base.OnRightToLeftChanged(e);
        if (HasDropDownItems)
        {
            // only perform a layout on a visible dropdown - otherwise clear the preferred size cache.
            if (DropDown.Visible)
            {
                LayoutTransaction.DoLayout(DropDown, this, PropertyNames.RightToLeft);
            }
            else
            {
                CommonProperties.xClearPreferredSizeCache(DropDown);
                DropDown.LayoutRequired = true;
            }
        }
    }

    internal override void OnImageScalingSizeChanged(EventArgs e)
    {
        base.OnImageScalingSizeChanged(e);
        if (HasDropDown && DropDown.IsAutoGenerated)
        {
            DropDown.DoLayoutIfHandleCreated(new ToolStripItemEventArgs(this));
        }
    }

    /// <summary>
    ///  Called as a response to HideDropDown
    /// </summary>
    protected virtual void OnDropDownHide(EventArgs e)
    {
        Invalidate();

        ((EventHandler?)Events[s_dropDownHideEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Last chance to stick in the DropDown before it is shown.
    /// </summary>
    protected virtual void OnDropDownShow(EventArgs e)
    {
        ((EventHandler?)Events[s_dropDownShowEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  called when the default item is clicked
    /// </summary>
    protected internal virtual void OnDropDownOpened(EventArgs e)
    {
        // only send the event if we're the thing that currently owns the DropDown.

        if (DropDown.OwnerItem != this)
        {
            return;
        }

        ((EventHandler?)Events[s_dropDownOpenedEvent])?.Invoke(this, e);

        bool accessibilityIsOn = IsAccessibilityObjectCreated ||
                (IsOnDropDown
                    ? OwnerItem?.IsAccessibilityObjectCreated ?? false
                    : IsParentAccessibilityObjectCreated);

        if (accessibilityIsOn && AccessibilityObject is ToolStripItemAccessibleObject accessibleObject)
        {
            accessibleObject.RaiseAutomationPropertyChangedEvent(
                UIA_PROPERTY_ID.UIA_ExpandCollapseExpandCollapseStatePropertyId,
                (VARIANT)(int)ExpandCollapseState.ExpandCollapseState_Collapsed,
                (VARIANT)(int)ExpandCollapseState.ExpandCollapseState_Expanded);
        }
    }

    /// <summary>
    ///  called when the default item is clicked
    /// </summary>
    protected internal virtual void OnDropDownClosed(EventArgs e)
    {
        // only send the event if we're the thing that currently owns the DropDown.
        Invalidate();

        if (DropDown.OwnerItem != this)
        {
            return;
        }

        ((EventHandler?)Events[s_dropDownClosedEvent])?.Invoke(this, e);

        if (!DropDown.IsAutoGenerated)
        {
            DropDown.OwnerItem = null;
        }

        if (IsAccessibilityObjectCreated && AccessibilityObject is ToolStripItemAccessibleObject accessibleObject)
        {
            accessibleObject.RaiseAutomationPropertyChangedEvent(
                UIA_PROPERTY_ID.UIA_ExpandCollapseExpandCollapseStatePropertyId,
                (VARIANT)(int)ExpandCollapseState.ExpandCollapseState_Expanded,
                (VARIANT)(int)ExpandCollapseState.ExpandCollapseState_Collapsed);
        }
    }

    /// <summary>
    ///  called when the default item is clicked
    /// </summary>
    protected internal virtual void OnDropDownItemClicked(ToolStripItemClickedEventArgs e)
    {
        // only send the event if we're the thing that currently owns the DropDown.

        if (DropDown.OwnerItem == this)
        {
            ((ToolStripItemClickedEventHandler?)Events[s_dropDownItemClickedEvent])?.Invoke(this, e);
        }
    }

    protected internal override bool ProcessCmdKey(ref Message m, Keys keyData)
    {
        if (HasDropDownItems)
        {
            return DropDown.ProcessCmdKeyInternal(ref m, keyData);
        }

        return base.ProcessCmdKey(ref m, keyData);
    }

    protected internal override bool ProcessDialogKey(Keys keyData)
    {
        Keys keyCode = keyData & Keys.KeyCode;

        // Items on the overflow should have the same kind of keyboard handling as a toplevel.
        bool isTopLevel = (!IsOnDropDown || IsOnOverflow);

        if (HasDropDownItems)
        {
            if (isTopLevel && (keyCode == Keys.Down || keyCode == Keys.Up || keyCode == Keys.Enter || (SupportsSpaceKey && keyCode == Keys.Space)))
            {
                ToolStrip.s_selectionDebug.TraceVerbose("[SelectDBG ProcessDialogKey] open submenu from toplevel item");

                if (Enabled || DesignMode)
                {
                    // |__[ * File ]_____|  * is where you are.  Up or down arrow hit should expand menu.
                    ShowDropDown();
                    KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(this);
                    DropDown.SelectNextToolStripItem(start: null, forward: true);
                }// else eat the key

                return true;
            }
            else if (!isTopLevel)
            {
                // if we're on a DropDown - then cascade out.
                bool menusCascadeRight = (((int)DropDownDirection & 0x0001) == 0);
                bool forward = ((keyCode == Keys.Enter) || (SupportsSpaceKey && keyCode == Keys.Space));
                forward = (forward || (menusCascadeRight && keyCode == Keys.Left) || (!menusCascadeRight && keyCode == Keys.Right));

                if (forward)
                {
                    ToolStrip.s_selectionDebug.TraceVerbose("[SelectDBG ProcessDialogKey] open submenu from NON-toplevel item");

                    if (Enabled || DesignMode)
                    {
                        ShowDropDown();
                        KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(this);
                        DropDown.SelectNextToolStripItem(start: null, forward: true);
                    } // else eat the key

                    return true;
                }
            }
        }
        else
        {
            // For a top-level item without sub-items: do nothing on Up/Down.
            if (isTopLevel && (keyCode == Keys.Down || keyCode == Keys.Up))
            {
                return true;
            }
        }

        if (IsOnDropDown)
        {
            bool menusCascadeRight = (((int)DropDownDirection & 0x0001) == 0);
            bool backward = ((menusCascadeRight && keyCode == Keys.Right) || (!menusCascadeRight && keyCode == Keys.Left));

            if (backward)
            {
                ToolStrip.s_selectionDebug.TraceVerbose("[SelectDBG ProcessDialogKey] close submenu from NON-toplevel item");

                // we're on a drop down but we're heading back up the chain.
                // remember to select the item that displayed this dropdown.
                ToolStripDropDown? parent = GetCurrentParentDropDown();
                if (parent is not null && !parent.IsFirstDropDown)
                {
                    // we're walking back up the dropdown chain.
                    parent.SetCloseReason(ToolStripDropDownCloseReason.Keyboard);
                    KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(this);
                    parent.SelectPreviousToolStrip();
                    return true;
                }

                // else if (parent.IsFirstDropDown)
                //    the base handling (ToolStripDropDown.ProcessArrowKey) will perform auto-expansion of
                //    the previous item in the menu.
            }
        }

        ToolStrip.s_selectionDebug.TraceVerbose("[SelectDBG ProcessDialogKey] ddi calling base");
        return base.ProcessDialogKey(keyData);
    }

    internal override void ReleaseUiaProvider()
    {
        _dropDown?.ReleaseUiaProvider(HWND.Null);
        base.ReleaseUiaProvider();
    }

    private ToolStripDropDownDirection RTLTranslateDropDownDirection(ToolStripDropDownDirection dropDownDirection, RightToLeft rightToLeft)
    {
        switch (dropDownDirection)
        {
            case ToolStripDropDownDirection.AboveLeft:
                return ToolStripDropDownDirection.AboveRight;
            case ToolStripDropDownDirection.AboveRight:
                return ToolStripDropDownDirection.AboveLeft;
            case ToolStripDropDownDirection.BelowRight:
                return ToolStripDropDownDirection.BelowLeft;
            case ToolStripDropDownDirection.BelowLeft:
                return ToolStripDropDownDirection.BelowRight;
            case ToolStripDropDownDirection.Right:
                return ToolStripDropDownDirection.Left;
            case ToolStripDropDownDirection.Left:
                return ToolStripDropDownDirection.Right;
        }

        Debug.Fail("Why are we here");

        // don't expect it to come to this but just in case here are the real defaults.
        if (IsOnDropDown)
        {
            return (rightToLeft == RightToLeft.Yes) ? ToolStripDropDownDirection.Left : ToolStripDropDownDirection.Right;
        }
        else
        {
            return (rightToLeft == RightToLeft.Yes) ? ToolStripDropDownDirection.BelowLeft : ToolStripDropDownDirection.BelowRight;
        }
    }

    /// <summary>
    ///  Shows the DropDown, if one is set.
    /// </summary>
    public void ShowDropDown()
        => ShowDropDown(false);

    internal void ShowDropDown(bool mousePush)
    {
        ShowDropDownInternal();
        if (_dropDown is ToolStripDropDownMenu menu)
        {
            if (!mousePush)
            {
                menu.ResetScrollPosition();
            }

            menu.RestoreScrollPosition();
        }
    }

    private void ShowDropDownInternal()
    {
        if (_dropDown is null || (!_dropDown.Visible))
        {
            // We want to show if there's no dropdown
            // or if the dropdown is not visible.
            OnDropDownShow(EventArgs.Empty);
        }

        // the act of setting the drop down visible the first time sets the parent
        // it seems that Visible returns true if your parent is null.
        if (_dropDown is not null && !_dropDown.Visible)
        {
            if (_dropDown.IsAutoGenerated && DropDownItems.Count <= 0)
            {
                return;  // this is a no-op for autogenerated drop downs.
            }

            if (DropDown == ParentInternal)
            {
                throw new InvalidOperationException(SR.ToolStripShowDropDownInvalidOperation);
            }

            _dropDown.OwnerItem = this;
            _dropDown.Location = DropDownLocation;
            _dropDown.Show();
            Invalidate();

            // Render the current tooltip (if there is one) on top of the dropdown element.
            ParentInternal?.UpdateToolTip(this, refresh: true);

            AccessibilityNotifyClients(AccessibleEvents.StateChange);
            AccessibilityNotifyClients(AccessibleEvents.NameChange);
        }
    }

    private bool ShouldSerializeDropDown()
        => _dropDown is not null && !_dropDown.IsAutoGenerated;

    private bool ShouldSerializeDropDownDirection()
        => _toolStripDropDownDirection != ToolStripDropDownDirection.Default;

    private bool ShouldSerializeDropDownItems()
        => _dropDown is not null && _dropDown.IsAutoGenerated;

    internal override void OnKeyboardToolTipHook(ToolTip toolTip)
    {
        base.OnKeyboardToolTipHook(toolTip);
        _hookedKeyboardTooltip = toolTip;
        if (_dropDown is not null)
        {
            KeyboardToolTipStateMachine.Instance.Hook(_dropDown, toolTip);
        }
    }

    internal override void OnKeyboardToolTipUnhook(ToolTip toolTip)
    {
        base.OnKeyboardToolTipUnhook(toolTip);
        _hookedKeyboardTooltip = null;
        if (_dropDown is not null)
        {
            KeyboardToolTipStateMachine.Instance.Unhook(_dropDown, toolTip);
        }
    }

    internal override void ToolStrip_RescaleConstants(int oldDpi, int newDpi)
    {
        RescaleConstantsInternal(newDpi);

        // Traversing the tree of DropDownMenuItems non-recursively to set new
        // Font (where necessary because not inherited from parent), DeviceDpi and reset the scaling.
        Stack<ToolStripDropDownItem> itemsStack = new();

        itemsStack.Push(this);

        while (itemsStack.Count > 0)
        {
            var item = itemsStack.Pop();

            if (item._dropDown is not null)
            {
                // The following does not get set, since dropDown has no parent/is not part of the
                // controls collection, so this gets never called through the normal inheritance chain.
                item._dropDown._deviceDpi = newDpi;
                item._dropDown.ResetScaling(newDpi);

                foreach (ToolStripItem childItem in item.DropDown.Items)
                {
                    if (childItem is null)
                        continue;

                    // Checking if font was inherited from parent.
                    Font currentFont = childItem.Font;
                    if (!currentFont.Equals(childItem.OwnerItem?.Font))
                    {
                        float factor = (float)newDpi / oldDpi;
                        childItem.Font = currentFont.WithSize(currentFont.Size * factor);
                    }

                    childItem.DeviceDpi = newDpi;

                    if (typeof(ToolStripDropDownItem).IsAssignableFrom(childItem.GetType()))
                    {
                        if (((ToolStripDropDownItem)childItem)._dropDown is not null)
                        {
                            itemsStack.Push((ToolStripDropDownItem)childItem);
                        }
                    }
                }
            }
        }

        // It's important to call the base class method only AFTER we processed all DropDown items,
        // because we need the new DPI in place, before a Font change triggers new layout calc.
        base.ToolStrip_RescaleConstants(oldDpi, newDpi);
    }
}
