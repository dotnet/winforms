﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

[Designer($"System.Windows.Forms.Design.ToolStripDropDownDesigner, {AssemblyRef.SystemDesign}")]
public partial class ToolStripDropDownMenu : ToolStripDropDown
{
    private Size _maxItemSize = Size.Empty;
    private Rectangle _checkRectangle = Rectangle.Empty;
    private Rectangle _imageRectangle = Rectangle.Empty;
    private Rectangle _arrowRectangle = Rectangle.Empty;
    private Rectangle _textRectangle = Rectangle.Empty;
    private Rectangle _imageMarginBounds = Rectangle.Empty;
    private int _paddingToTrim;
    private int _tabWidth = -1;

    private ToolStripScrollButton? _upScrollButton;
    private ToolStripScrollButton? _downScrollButton;
    private int _scrollAmount;
    private int _indexOfFirstDisplayedItem = -1;

    private BitVector32 _state;

    private static readonly int s_stateShowImageMargin = BitVector32.CreateMask();
    private static readonly int s_stateShowCheckMargin = BitVector32.CreateMask(s_stateShowImageMargin);
    private static readonly int s_stateMaxItemSizeValid = BitVector32.CreateMask(s_stateShowCheckMargin);

    private Size _defaultImageSize;
    private int _defaultImageMarginWidth;
    private int _defaultImageAndCheckMarginWidth;
    private Padding _imagePadding;
    private Padding _textPadding;
    private Padding _checkPadding;
    private Padding _arrowPadding;
    private int _arrowSize;

    public ToolStripDropDownMenu()
    {
        ScaleConstants(ScaleHelper.OneHundredPercentLogicalDpi);
    }

    /// <summary>
    ///  Constructor to autogenerate
    /// </summary>
    internal ToolStripDropDownMenu(ToolStripItem ownerItem, bool isAutoGenerated)
        : base(ownerItem, isAutoGenerated)
    {
        ScaleConstants(ScaleHelper.InitialSystemDpi);
    }

    protected override AccessibleObject CreateAccessibilityInstance()
        => new ToolStripDropDownMenuAccessibleObject(this);

    internal override bool AllItemsVisible
    {
        get => !RequiresScrollButtons;
        set => RequiresScrollButtons = !value;
    }

    internal Rectangle ArrowRectangle => _arrowRectangle;

    internal Rectangle CheckRectangle => _checkRectangle;

    protected override Padding DefaultPadding
    {
        get
        {
            RightToLeft rightToLeft = RightToLeft;

            int textPadding = (rightToLeft == RightToLeft.Yes) ? _textPadding.Right : _textPadding.Left;
            int padding = (ShowCheckMargin || ShowImageMargin) ? textPadding + ImageMargin.Width : textPadding;

            // scooch in all the items by the margin.
            if (rightToLeft == RightToLeft.Yes)
            {
                return new Padding(1, 2, padding, 2);
            }

            return new Padding(padding, 2, 1, 2);
        }
    }

    public override Rectangle DisplayRectangle
    {
        get
        {
            Rectangle rect = base.DisplayRectangle;
            if (GetToolStripState(STATE_SCROLLBUTTONS))
            {
                rect.Y += UpScrollButton.Height + UpScrollButton.Margin.Vertical;
                rect.Height -= UpScrollButton.Height + UpScrollButton.Margin.Vertical + DownScrollButton.Height + DownScrollButton.Margin.Vertical;
                // Because we're going to draw the scroll buttons on top of the padding, we need to add it back in here.
                rect = LayoutUtils.InflateRect(rect, new Padding(0, Padding.Top, 0, Padding.Bottom));
            }

            return rect;
        }
    }

    internal ToolStripScrollButton DownScrollButton
        => _downScrollButton ??= new ToolStripScrollButton(false) { ParentInternal = this };

    /// <summary>
    ///  the rectangle representing
    /// </summary>
    internal Rectangle ImageRectangle => _imageRectangle;

    internal int PaddingToTrim
    {
        get => _paddingToTrim;

        set
        {
            if (_paddingToTrim != value)
            {
                _paddingToTrim = value;
                AdjustSize();
            }
        }
    }

    /// <summary>
    ///  the rectangle representing the color stripe in the menu - this will appear as AffectedBounds
    ///  in the ToolStripRenderEventArgs
    /// </summary>
    internal Rectangle ImageMargin
    {
        get
        {
            _imageMarginBounds.Height = Height;
            return _imageMarginBounds;
        }
    }

    public override LayoutEngine LayoutEngine => ToolStripDropDownLayoutEngine.LayoutInstance;

    [DefaultValue(ToolStripLayoutStyle.Flow)]
    public new ToolStripLayoutStyle LayoutStyle
    {
        get => base.LayoutStyle;
        set => base.LayoutStyle = value;
    }

    protected internal override Size MaxItemSize
    {
        get
        {
            if (!_state[s_stateMaxItemSizeValid])
            {
                CalculateInternalLayoutMetrics();
            }

            return _maxItemSize;
        }
    }

    [DefaultValue(true)]
    [SRDescription(nameof(SR.ToolStripDropDownMenuShowImageMarginDescr))]
    [SRCategory(nameof(SR.CatAppearance))]
    public bool ShowImageMargin
    {
        get => _state[s_stateShowImageMargin];

        set
        {
            if (value != _state[s_stateShowImageMargin])
            {
                _state[s_stateShowImageMargin] = value;
                LayoutTransaction.DoLayout(this, this, PropertyNames.ShowImageMargin);
            }
        }
    }

    [DefaultValue(false)]
    [SRDescription(nameof(SR.ToolStripDropDownMenuShowCheckMarginDescr))]
    [SRCategory(nameof(SR.CatAppearance))]
    public bool ShowCheckMargin
    {
        get => _state[s_stateShowCheckMargin];

        set
        {
            if (value != _state[s_stateShowCheckMargin])
            {
                _state[s_stateShowCheckMargin] = value;
                LayoutTransaction.DoLayout(this, this, PropertyNames.ShowCheckMargin);
            }
        }
    }

    internal Rectangle TextRectangle
        => _textRectangle;

    internal ToolStripScrollButton UpScrollButton
        => _upScrollButton ??= new ToolStripScrollButton(true) { ParentInternal = this };

    /// <summary>
    ///  this takes a native menu and builds up a managed toolstrip around it.
    ///  Scenario: showing the items from the SystemMenu.
    ///  targetWindow is the window to send WM_COMMAND, WM_SYSCOMMAND to
    ///  hmenu is a handle to the native menu.
    /// </summary>
    internal static unsafe ToolStripDropDownMenu FromHMenu(HMENU hmenu, IWin32Window targetWindow)
    {
        ToolStripDropDownMenu managedDropDown = new ToolStripDropDownMenu();
        managedDropDown.SuspendLayout();

        int count = PInvoke.GetMenuItemCount(hmenu);

        ToolStripItem itemToAdd;

        // surf through the items in the collection, building up TSMenuItems and TSSeparators
        // corresponding to the native menu.
        for (uint i = 0; i < count; i++)
        {
            // peek at the i'th item.
            MENUITEMINFOW info = new()
            {
                cbSize = (uint)sizeof(MENUITEMINFOW),
                fMask = MENU_ITEM_MASK.MIIM_FTYPE
            };

            PInvoke.GetMenuItemInfo(hmenu, i, fByPosition: true, ref info);

            if (info.fType == MENU_ITEM_TYPE.MFT_SEPARATOR)
            {
                // its a separator.
                itemToAdd = new ToolStripSeparator();
            }
            else
            {
                // its a menu item... lets fish out the command id
                info = new()
                {
                    cbSize = (uint)sizeof(MENUITEMINFOW),
                    fMask = MENU_ITEM_MASK.MIIM_ID
                };

                PInvoke.GetMenuItemInfo(hmenu, i, fByPosition: true, ref info);

                // create the managed object - toolstripmenu item knows how to grok hmenu for information.
                itemToAdd = new ToolStripMenuItem(hmenu, (int)info.wID, targetWindow);

                // if there is a submenu fetch it.
                info = new()
                {
                    cbSize = (uint)sizeof(MENUITEMINFOW),
                    fMask = MENU_ITEM_MASK.MIIM_SUBMENU
                };

                PInvoke.GetMenuItemInfo(hmenu, i, fByPosition: true, ref info);

                if (!info.hSubMenu.IsNull)
                {
                    // set the dropdown to be the items from the submenu
                    ((ToolStripMenuItem)itemToAdd).DropDown = FromHMenu(info.hSubMenu, targetWindow);
                }
            }

            managedDropDown.Items.Add(itemToAdd);
        }

        managedDropDown.ResumeLayout();
        return managedDropDown;
    }

    private void CalculateInternalLayoutMetrics()
    {
        Size maxTextSize = Size.Empty;
        Size maxImageSize = Size.Empty;
        Size maxCheckSize = _defaultImageSize;
        Size maxArrowSize = Size.Empty;
        Size maxNonMenuItemSize = Size.Empty;

        // determine Text Metrics
        for (int i = 0; i < Items.Count; i++)
        {
            ToolStripItem item = Items[i];

            if (item is ToolStripMenuItem menuItem)
            {
                Size menuItemTextSize = menuItem.GetTextSize();

                if (menuItem.ShowShortcutKeys)
                {
                    Size shortcutTextSize = menuItem.GetShortcutTextSize();
                    if (_tabWidth == -1)
                    {
                        _tabWidth = TextRenderer.MeasureText("\t", Font).Width;
                    }

                    menuItemTextSize.Width += _tabWidth + shortcutTextSize.Width;
                    menuItemTextSize.Height = Math.Max(menuItemTextSize.Height, shortcutTextSize.Height);
                }

                // we truly only care about the maximum size we find.
                maxTextSize.Width = Math.Max(maxTextSize.Width, menuItemTextSize.Width);
                maxTextSize.Height = Math.Max(maxTextSize.Height, menuItemTextSize.Height);

                // determine Image Metrics
                Size imageSize = Size.Empty;
                if (menuItem.Image is not null)
                {
                    imageSize = (menuItem.ImageScaling == ToolStripItemImageScaling.SizeToFit) ? ImageScalingSize : menuItem.Image.Size;
                }

                maxImageSize.Width = Math.Max(maxImageSize.Width, imageSize.Width);
                maxImageSize.Height = Math.Max(maxImageSize.Height, imageSize.Height);

                if (menuItem.CheckedImage is not null)
                {
                    Size checkedImageSize = menuItem.CheckedImage.Size;
                    maxCheckSize.Width = Math.Max(checkedImageSize.Width, maxCheckSize.Width);
                    maxCheckSize.Height = Math.Max(checkedImageSize.Height, maxCheckSize.Height);
                }
            }
            else if (!(item is ToolStripSeparator))
            {
                maxNonMenuItemSize.Height = Math.Max(item.Bounds.Height, maxNonMenuItemSize.Height);
                maxNonMenuItemSize.Width = Math.Max(item.Bounds.Width, maxNonMenuItemSize.Width);
            }
        }

        _maxItemSize.Height = Math.Max(maxTextSize.Height + _textPadding.Vertical, Math.Max(maxCheckSize.Height + _checkPadding.Vertical, maxArrowSize.Height + _arrowPadding.Vertical));

        if (ShowImageMargin)
        {
            // only add in the image into the calculation if we're going to render it.
            _maxItemSize.Height = Math.Max(maxImageSize.Height + _imagePadding.Vertical, _maxItemSize.Height);
        }

        // Always save space for an arrow
        maxArrowSize = new Size(_arrowSize, _maxItemSize.Height);

        maxTextSize.Height = _maxItemSize.Height - _textPadding.Vertical;
        maxImageSize.Height = _maxItemSize.Height - _imagePadding.Vertical;
        maxCheckSize.Height = _maxItemSize.Height - _checkPadding.Vertical;

        // fixup if there are non-menu items that are larger than our normal menu items
        maxTextSize.Width = Math.Max(maxTextSize.Width, maxNonMenuItemSize.Width);

        Point nextPoint = Point.Empty;
        int checkAndImageMarginWidth = 0;

        int extraImageWidth = Math.Max(0, maxImageSize.Width - _defaultImageSize.Width);

        if (ShowCheckMargin && ShowImageMargin)
        {
            // double column - check margin then image margin
            // default to 46px - grow if necessary.
            checkAndImageMarginWidth = _defaultImageAndCheckMarginWidth;

            // add in the extra space for the image... since the check size is locked down to 16x16.
            checkAndImageMarginWidth += extraImageWidth;

            // align the checkmark
            nextPoint = new Point(_checkPadding.Left, _checkPadding.Top);
            _checkRectangle = LayoutUtils.Align(maxCheckSize, new Rectangle(nextPoint.X, nextPoint.Y, maxCheckSize.Width, _maxItemSize.Height), ContentAlignment.MiddleCenter);

            // align the image rectangle
            nextPoint.X = _checkRectangle.Right + _checkPadding.Right + _imagePadding.Left;
            nextPoint.Y = _imagePadding.Top;
            _imageRectangle = LayoutUtils.Align(maxImageSize, new Rectangle(nextPoint.X, nextPoint.Y, maxImageSize.Width, _maxItemSize.Height), ContentAlignment.MiddleCenter);
        }
        else if (ShowCheckMargin)
        {
            // no images should be shown in a ShowCheckMargin only scenario.
            // default to 24px - grow if necessary.
            checkAndImageMarginWidth = _defaultImageMarginWidth;

            // align the checkmark
            nextPoint = new Point(1, _checkPadding.Top);
            // nextPoint = new Point(scaledCheckPadding.Left, scaledCheckPadding.Top);
            _checkRectangle = LayoutUtils.Align(maxCheckSize, new Rectangle(nextPoint.X, nextPoint.Y, checkAndImageMarginWidth, _maxItemSize.Height), ContentAlignment.MiddleCenter);

            _imageRectangle = Rectangle.Empty;
        }
        else if (ShowImageMargin)
        {
            // checks and images render in the same area.

            // default to 24px - grow if necessary.
            checkAndImageMarginWidth = _defaultImageMarginWidth;

            // add in the extra space for the image... since the check size is locked down to 16x16.
            checkAndImageMarginWidth += extraImageWidth;

            // NOTE due to the Padding property, we're going to have to recalc the vertical alignment in ToolStripMenuItemInternalLayout.
            // Don't fuss here over the Y, X is what's critical.

            // check and image rect are the same - take the max of the image size and the check size and align
            nextPoint = new Point(1, _checkPadding.Top);
            _checkRectangle = LayoutUtils.Align(LayoutUtils.UnionSizes(maxCheckSize, maxImageSize), new Rectangle(nextPoint.X, nextPoint.Y, checkAndImageMarginWidth - 1, _maxItemSize.Height), ContentAlignment.MiddleCenter);

            // align the image
            _imageRectangle = _checkRectangle;
        }
        else
        {
            checkAndImageMarginWidth = 0;
        }

        nextPoint.X = checkAndImageMarginWidth + 1;

        // calculate space for image
        // if we didnt have a check - make sure to ignore check padding

        // consider: should we constrain to a reasonable width?
        // imageMarginBounds = new Rectangle(0, 0, Math.Max(imageMarginWidth,DefaultImageMarginWidth), this.Height);
        _imageMarginBounds = new Rectangle(0, 0, checkAndImageMarginWidth, Height);

        // calculate space for shortcut and text
        nextPoint.X = _imageMarginBounds.Right + _textPadding.Left;
        nextPoint.Y = _textPadding.Top;
        _textRectangle = new Rectangle(nextPoint, maxTextSize);

        // calculate space for arrow
        nextPoint.X = _textRectangle.Right + _textPadding.Right + _arrowPadding.Left;
        nextPoint.Y = _arrowPadding.Top;
        _arrowRectangle = new Rectangle(nextPoint, maxArrowSize);

        // calculate space required for all of these pieces
        _maxItemSize.Width = (_arrowRectangle.Right + _arrowPadding.Right) - _imageMarginBounds.Left;

        Padding = DefaultPadding;
        int trimPadding = _imageMarginBounds.Width;

        if (RightToLeft == RightToLeft.Yes)
        {
            // reverse the rectangle alignment in RightToLeft.Yes
            trimPadding += _textPadding.Right;
            int width = _maxItemSize.Width;
            _checkRectangle.X = width - _checkRectangle.Right;
            _imageRectangle.X = width - _imageRectangle.Right;
            _textRectangle.X = width - _textRectangle.Right;
            _arrowRectangle.X = width - _arrowRectangle.Right;
            _imageMarginBounds.X = width - _imageMarginBounds.Right;
        }
        else
        {
            trimPadding += _textPadding.Left;
        }

        // We need to make sure that the text really appears vertically centered - this can be a problem in
        // systems which force the text rectangle to be odd.

        // force this to be an even height.
        _maxItemSize.Height += _maxItemSize.Height % 2;

        _textRectangle.Y = LayoutUtils.VAlign(_textRectangle.Size, new Rectangle(Point.Empty, _maxItemSize), ContentAlignment.MiddleCenter).Y;
        _textRectangle.Y += (_textRectangle.Height % 2); // if the height is odd, push down by one px
        _state[s_stateMaxItemSizeValid] = true;
        PaddingToTrim = trimPadding;
    }

    internal override void ChangeSelection(ToolStripItem? nextItem)
    {
        if (nextItem is not null)
        {
            Rectangle displayRect = DisplayRectangle;
            if (!displayRect.Contains(displayRect.X, nextItem.Bounds.Top)
                || !displayRect.Contains(displayRect.X, nextItem.Bounds.Bottom))
            {
                int delta;
                if (displayRect.Y > nextItem.Bounds.Top)
                {
                    delta = nextItem.Bounds.Top - displayRect.Y;
                }
                else
                {
                    delta = nextItem.Bounds.Bottom - (displayRect.Y + displayRect.Height);
                    // Now adjust so that the item at the top isn't truncated.
                    int index = Items.IndexOf(nextItem);
                    while (index >= 0)
                    {
                        // we need to roll back to the index which is visible
                        if ((Items[index].Visible && displayRect.Contains(displayRect.X, Items[index].Bounds.Top - delta))
                            || !Items[index].Visible)
                        {
                            --index;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (index >= 0)
                    {
                        if (displayRect.Contains(displayRect.X, Items[index].Bounds.Bottom - delta))
                        {
                            // We found an item which is truncated at the top.
                            delta += (Items[index].Bounds.Bottom - delta) - displayRect.Top;
                        }
                    }
                }

                ScrollInternal(delta);
                UpdateScrollButtonStatus();
            }
        }

        base.ChangeSelection(nextItem);
    }

    protected internal override ToolStripItem CreateDefaultItem(string? text, Image? image, EventHandler? onClick)
    {
        if (text == "-")
        {
            return new ToolStripSeparator();
        }
        else
        {
            return new ToolStripMenuItem(text, image, onClick);
        }
    }

    internal override ToolStripItem? GetNextItem(ToolStripItem? start, ArrowDirection direction, bool rtlAware)
    {
        // for up/down we don't care about flipping left/right tab should still take you down.
        return GetNextItem(start, direction);
    }

    internal override void Initialize()
    {
        base.Initialize();
        Padding = DefaultPadding;
        FlowLayoutSettings settings = new FlowLayoutSettings(this);
        settings.FlowDirection = FlowDirection.TopDown;
        _state[s_stateShowImageMargin] = true;
    }

    protected override void OnLayout(LayoutEventArgs e)
    {
        if (!IsDisposed)
        {
            // We always layout as if we don't need scroll buttons.
            // If we do, then we'll adjust the positions to match.
            RequiresScrollButtons = false;
            CalculateInternalLayoutMetrics();
            base.OnLayout(e);
            if (!RequiresScrollButtons)
            {
                ResetScrollPosition();
            }
        }
    }

    protected override void OnFontChanged(EventArgs e)
    {
        _tabWidth = -1;
        base.OnFontChanged(e);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        base.OnPaintBackground(e);
        if (ShowCheckMargin || ShowImageMargin)
        {
            Renderer.DrawImageMargin(new ToolStripRenderEventArgs(e.Graphics, this, ImageMargin, SystemColors.Control));
        }
    }

    internal override void ReleaseToolStripItemsProviders(ToolStripItemCollection items)
    {
    }

    internal override void ResetScaling(int newDpi)
    {
        base.ResetScaling(newDpi);
        CommonProperties.xClearPreferredSizeCache(this);
        ScaleConstants(newDpi);
    }

    private void ScaleConstants(int dpi)
    {
        const int LogicalDefaultImageSize = 16;
        const int LogicalImagePadding = 2;
        const int LogicalArrowSize = 10;

        // If we have an image or check margin with no image or checks in it use this - which is consistent with Office
        // and an image margin with a 16x16 icon in it.
        const int DefaultImageMarginWidth = 24;
        const int DefaultImageAndCheckMarginWidth = 46;

        int defaultImageSize = ScaleHelper.ScaleToDpi(LogicalDefaultImageSize, dpi);
        _defaultImageSize = new(defaultImageSize, defaultImageSize);

        // Add an additional pixel for the border after scaling for the next two values.
        _defaultImageMarginWidth = ScaleHelper.ScaleToDpi(DefaultImageMarginWidth, dpi) + 1;
        _defaultImageAndCheckMarginWidth = ScaleHelper.ScaleToDpi(DefaultImageAndCheckMarginWidth, dpi) + 1;

        _imagePadding = new(ScaleHelper.ScaleToDpi(LogicalImagePadding, dpi));
        _textPadding = ScaleHelper.ScaleToDpi(new Padding(8, 1, 9, 1), dpi);
        _checkPadding = ScaleHelper.ScaleToDpi(new Padding(5, 2, 2, 2), dpi);
        _arrowPadding = ScaleHelper.ScaleToDpi(new Padding(0, 0, 8, 0), dpi);
        _arrowSize = ScaleHelper.ScaleToDpi(LogicalArrowSize, dpi);
    }

    internal override bool RequiresScrollButtons
    {
        get
        {
            return GetToolStripState(STATE_SCROLLBUTTONS);
        }
        set
        {
            bool changed = (RequiresScrollButtons != value);
            SetToolStripState(STATE_SCROLLBUTTONS, value);
            if (changed)
            {
                UpdateScrollButtonLocations();
                if (this.Items.Count > 0)
                {
                    int delta = this.Items[0].Bounds.Top - this.DisplayRectangle.Top;
                    this.ScrollInternal(delta);
                    this._scrollAmount -= delta;
                    if (value)
                    {
                        RestoreScrollPosition();
                    }
                }
                else
                {
                    this._scrollAmount = 0;
                }
            }
        }
    }

    internal void ResetScrollPosition()
        => _scrollAmount = 0;

    internal void RestoreScrollPosition()
    {
        if (!RequiresScrollButtons || Items.Count == 0)
        {
            return;
        }

        // We don't just scroll by the amount, because that might
        // cause the bottom of the menu to be blank if some items have
        // been removed/hidden since the last time we were displayed.
        // This also deals with items of different height, so that we don't truncate
        // and items under the top scrollbar.

        Rectangle displayRectangle = DisplayRectangle;
        int alreadyScrolled = displayRectangle.Top - Items[0].Bounds.Top;

        int requiredScrollAmount = _scrollAmount - alreadyScrolled;

        int deltaToScroll = 0;
        if (requiredScrollAmount > 0)
        {
            for (int i = 0; i < Items.Count && deltaToScroll < requiredScrollAmount; ++i)
            {
                if (Items[i].Available)
                {
                    Rectangle adjustedLastItemBounds = Items[Items.Count - 1].Bounds;
                    adjustedLastItemBounds.Y -= deltaToScroll;
                    if (displayRectangle.Contains(displayRectangle.X, adjustedLastItemBounds.Top)
                        && displayRectangle.Contains(displayRectangle.X, adjustedLastItemBounds.Bottom))
                    {
                        // Scrolling this amount would make the last item visible, so don't scroll any more.
                        break;
                    }

                    // We use a delta between the tops, since it takes margin's and padding into account.
                    if (i < Items.Count - 1)
                    {
                        deltaToScroll += Items[i + 1].Bounds.Top - Items[i].Bounds.Top;
                    }
                    else
                    {
                        deltaToScroll += Items[i].Bounds.Height;
                    }
                }
            }
        }
        else
        {
            for (int i = Items.Count - 1; i >= 0 && deltaToScroll > requiredScrollAmount; --i)
            {
                if (Items[i].Available)
                {
                    Rectangle adjustedLastItemBounds = Items[0].Bounds;
                    adjustedLastItemBounds.Y -= deltaToScroll;
                    if (displayRectangle.Contains(displayRectangle.X, adjustedLastItemBounds.Top)
                        && displayRectangle.Contains(displayRectangle.X, adjustedLastItemBounds.Bottom))
                    {
                        // Scrolling this amount would make the last item visible, so don't scroll any more.
                        break;
                    }

                    // We use a delta between the tops, since it takes margin's and padding into account.
                    if (i > 0)
                    {
                        deltaToScroll -= Items[i].Bounds.Top - Items[i - 1].Bounds.Top;
                    }
                    else
                    {
                        deltaToScroll -= Items[i].Bounds.Height;
                    }
                }
            }
        }

        ScrollInternal(deltaToScroll);
        _scrollAmount = DisplayRectangle.Top - Items[0].Bounds.Top;
        UpdateScrollButtonLocations();
    }

    internal override void ScrollInternal(int delta)
    {
        base.ScrollInternal(delta);
        _scrollAmount += delta;
    }

    internal void ScrollInternal(bool up)
    {
        UpdateScrollButtonStatus();

        // calling this to get ScrollWindowEx.  In actuality it does nothing
        // to change the display rect!
        int delta;
        if (_indexOfFirstDisplayedItem == -1 || _indexOfFirstDisplayedItem >= Items.Count)
        {
            Debug.Fail("Why wasn't 'UpdateScrollButtonStatus called'? We don't have the item to scroll by");
            int menuHeight = SystemInformation.MenuHeight;

            delta = up ? -menuHeight : menuHeight;
        }
        else
        {
            if (up)
            {
                if (_indexOfFirstDisplayedItem == 0)
                {
                    Debug.Fail("We're trying to scroll up, but the top item is displayed!!!");
                    delta = 0;
                }
                else
                {
                    ToolStripItem itemTop = Items[_indexOfFirstDisplayedItem - 1];
                    ToolStripItem itemBottom = Items[_indexOfFirstDisplayedItem];
                    // We use a delta between the tops, since it takes margin's and padding into account.
                    delta = itemTop.Bounds.Top - itemBottom.Bounds.Top;
                }
            }
            else
            {
                if (_indexOfFirstDisplayedItem == Items.Count - 1)
                {
                    Debug.Fail("We're trying to scroll down, but the top item is displayed!!!");
                    delta = 0;
                }

                ToolStripItem itemTop = Items[_indexOfFirstDisplayedItem];
                ToolStripItem itemBottom = Items[_indexOfFirstDisplayedItem + 1];
                // We use a delta between the tops, since it takes margin's and padding into account.
                delta = itemBottom.Bounds.Top - itemTop.Bounds.Top;
            }
        }

        ScrollInternal(delta);
        UpdateScrollButtonLocations();
    }

    protected override void SetDisplayedItems()
    {
        base.SetDisplayedItems();
        if (RequiresScrollButtons)
        {
            DisplayedItems.Insert(0, UpScrollButton);
            DisplayedItems.Add(DownScrollButton);
            UpdateScrollButtonLocations();
            UpScrollButton.Visible = true;
            DownScrollButton.Visible = true;
        }
        else
        {
            UpScrollButton.Visible = false;
            DownScrollButton.Visible = false;
        }
    }

    private void UpdateScrollButtonLocations()
    {
        if (GetToolStripState(STATE_SCROLLBUTTONS))
        {
            Size upSize = UpScrollButton.GetPreferredSize(Size.Empty);
            //
            Point upLocation = new Point(1, 0);

            UpScrollButton.SetBounds(new Rectangle(upLocation, upSize));

            Size downSize = DownScrollButton.GetPreferredSize(Size.Empty);
            int height = GetDropDownBounds(Bounds).Height;

            Point downLocation = new Point(1, height - downSize.Height);
            DownScrollButton.SetBounds(new Rectangle(downLocation, downSize));
            UpdateScrollButtonStatus();
        }
    }

    private void UpdateScrollButtonStatus()
    {
        Rectangle displayRectangle = DisplayRectangle;

        _indexOfFirstDisplayedItem = -1;
        int minY = int.MaxValue, maxY = 0;

        for (int i = 0; i < Items.Count; ++i)
        {
            ToolStripItem item = Items[i];
            if (UpScrollButton == item)
            {
                continue;
            }

            if (DownScrollButton == item)
            {
                continue;
            }

            if (!item.Available)
            {
                continue;
            }

            if (_indexOfFirstDisplayedItem == -1 && displayRectangle.Contains(displayRectangle.X, item.Bounds.Top))
            {
                _indexOfFirstDisplayedItem = i;
            }

            minY = Math.Min(minY, item.Bounds.Top);
            maxY = Math.Max(maxY, item.Bounds.Bottom);
        }

        UpScrollButton.Enabled = !displayRectangle.Contains(displayRectangle.X, minY);
        DownScrollButton.Enabled = !displayRectangle.Contains(displayRectangle.X, maxY);
    }
}
