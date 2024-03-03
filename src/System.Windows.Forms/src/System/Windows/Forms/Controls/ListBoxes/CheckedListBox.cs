// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

/// <summary>
///  Displays a list with a checkbox to the left of each item.
/// </summary>
[LookupBindingProperties]
[SRDescription(nameof(SR.DescriptionCheckedListBox))]
public partial class CheckedListBox : ListBox
{
    private int _idealCheckSize = 13;

    private const int LB_CHECKED = 1;
    private const int LB_UNCHECKED = 0;
    private const int BORDER_SIZE = 1;

    /// <summary>
    ///  Decides whether or not to ignore the next LBN_SELCHANGE message - used to prevent cursor keys from
    ///  toggling checkboxes.
    /// </summary>
    private bool _killNextSelect;

    /// <summary>
    ///  Current listener of the onItemCheck event.
    /// </summary>
    private ItemCheckEventHandler? _onItemCheck;

    /// <summary>
    ///  Should we use 3d checkboxes or flat ones?
    /// </summary>
    private bool _flat = true;

    /// <summary>
    ///  Indicates which item was last selected.  We want to keep track of this so we can be a little less
    ///  aggressive about checking/unchecking the items as the user moves around.
    /// </summary>
    private int _lastSelected = -1;

    /// <summary>
    ///  The collection of checked items in the CheckedListBox.
    /// </summary>
    private CheckedItemCollection? _checkedItemCollection;
    private CheckedIndexCollection? _checkedIndexCollection;

    private static uint LBC_GETCHECKSTATE { get; } = PInvoke.RegisterWindowMessage("LBC_GETCHECKSTATE");
    private static uint LBC_SETCHECKSTATE { get; } = PInvoke.RegisterWindowMessage("LBC_SETCHECKSTATE");

    /// <summary>
    ///  Creates a new CheckedListBox for the user.
    /// </summary>
    public CheckedListBox() : base()
    {
        // If we eat WM_ERASEBKGRND messages, the background will be
        // painted sometimes but not others.
        // SetStyle(ControlStyles.Opaque, true);

        // If a long item is drawn with ellipsis, we must redraw the ellipsed part
        // as well as the newly uncovered region.
        SetStyle(ControlStyles.ResizeRedraw, true);
    }

    /// <summary>
    ///  Indicates whether or not the checkbox should be toggled whenever an
    ///  item is selected.  The default behaviour is to just change the
    ///  selection, and then make the user click again to check it.  However,
    ///  some may prefer checking the item as soon as it is clicked.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.CheckedListBoxCheckOnClickDescr))]
    public bool CheckOnClick { get; set; }

    /// <summary>
    ///  Collection of checked indices in this CheckedListBox.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public CheckedIndexCollection CheckedIndices => _checkedIndexCollection ??= new CheckedIndexCollection(this);

    /// <summary>
    ///  Collection of checked items in this CheckedListBox.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public CheckedItemCollection CheckedItems => _checkedItemCollection ??= new CheckedItemCollection(this);

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.Style |= PInvoke.LBS_OWNERDRAWFIXED | PInvoke.LBS_WANTKEYBOARDINPUT;
            return cp;
        }
    }

    /// <summary>
    ///  CheckedListBox DataSource.
    /// </summary>
    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new object? DataSource
    {
        get => base.DataSource;
        set => base.DataSource = value;
    }

    /// <summary>
    ///  CheckedListBox DisplayMember.
    /// </summary>
    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new string DisplayMember
    {
        get => base.DisplayMember;
        set => base.DisplayMember = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override DrawMode DrawMode
    {
        get
        {
            return DrawMode.Normal;
        }
        set
        {
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override int ItemHeight
    {
        get
        {
            // this should take FontHeight + buffer into Consideration.
            return Font.Height + _listItemBordersHeight;
        }
        set
        {
        }
    }

    /// <summary>
    ///  Collection of items in this <see cref="ListBox"/>
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ListBoxItemsDescr))]
    [Editor($"System.Windows.Forms.Design.ListControlStringCollectionEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [MergableProperty(false)]
    public new ObjectCollection Items
    {
        get
        {
            return (ObjectCollection)base.Items;
        }
    }

    internal override int MaxItemWidth
    {
        get
        {
            // Overridden to include the size of the checkbox
            return base.MaxItemWidth + _idealCheckSize + _listItemPaddingBuffer;
        }
    }

    /// <summary>
    ///  For CheckedListBoxes, multi-selection is not supported.  You can set
    ///  selection to be able to select one item or no items.
    /// </summary>
    public override SelectionMode SelectionMode
    {
        get => base.SelectionMode;
        set
        {
            // valid values are 0x0 to 0x3
            SourceGenerated.EnumValidator.Validate(value);
            if (value is not SelectionMode.One and not SelectionMode.None)
            {
                throw new ArgumentException(SR.CheckedListBoxInvalidSelectionMode);
            }

            if (value != SelectionMode)
            {
                base.SelectionMode = value;
                RecreateHandle();
            }
        }
    }

    /// <summary>
    ///  Indicates if the CheckBoxes should show up as flat or 3D in appearance.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.CheckedListBoxThreeDCheckBoxesDescr))]
    public bool ThreeDCheckBoxes
    {
        get
        {
            return !_flat;
        }
        set
        {
            // change the style and repaint.
            //
            if (_flat == value)
            {
                _flat = !value;

                // see if we have some items, and only invalidate if we do.
                ObjectCollection items = Items;
                if ((items is not null) && (items.Count > 0))
                {
                    Invalidate();
                }
            }
        }
    }

    /// <summary>
    ///  Determines whether to use compatible text rendering engine (GDI+) or not (GDI).
    /// </summary>
    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.UseCompatibleTextRenderingDescr))]
    public bool UseCompatibleTextRendering
    {
        get => UseCompatibleTextRenderingInternal;
        set => UseCompatibleTextRenderingInternal = value;
    }

    /// <summary>
    ///  Determines whether the control supports rendering text using GDI+ and GDI.
    ///  This is provided for container controls to iterate through its children to set UseCompatibleTextRendering to the same
    ///  value if the child control supports it.
    /// </summary>
    internal override bool SupportsUseCompatibleTextRendering
    {
        get
        {
            return true;
        }
    }

    /// <summary>
    ///  CheckedListBox ValueMember.
    /// </summary>
    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new string ValueMember
    {
        get => base.ValueMember;
        set => base.ValueMember = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? DataSourceChanged
    {
        add => base.DataSourceChanged += value;
        remove => base.DataSourceChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? DisplayMemberChanged
    {
        add => base.DisplayMemberChanged += value;
        remove => base.DisplayMemberChanged -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.CheckedListBoxItemCheckDescr))]
    public event ItemCheckEventHandler? ItemCheck
    {
        add => _onItemCheck += value;
        remove => _onItemCheck -= value;
    }

    /// <hideinheritance/>
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public new event EventHandler? Click
    {
        add => base.Click += value;
        remove => base.Click -= value;
    }

    /// <hideinheritance/>
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public new event MouseEventHandler? MouseClick
    {
        add => base.MouseClick += value;
        remove => base.MouseClick -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event DrawItemEventHandler? DrawItem
    {
        add => base.DrawItem += value;
        remove => base.DrawItem -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event MeasureItemEventHandler? MeasureItem
    {
        add => base.MeasureItem += value;
        remove => base.MeasureItem -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Padding Padding
    {
        get => base.Padding;
        set => base.Padding = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? ValueMemberChanged
    {
        add => base.ValueMemberChanged += value;
        remove => base.ValueMemberChanged -= value;
    }

    /// <summary>
    ///  Constructs the new instance of the accessibility object for this control. Subclasses
    ///  should not call base.CreateAccessibilityObject.
    /// </summary>
    protected override AccessibleObject CreateAccessibilityInstance()
    {
        return new CheckedListBoxAccessibleObject(this);
    }

    protected override ListBox.ObjectCollection CreateItemCollection()
    {
        return new ObjectCollection(this);
    }

    /// <summary>
    ///  Gets the check value of the current item.  This value will be from the
    ///  System.Windows.Forms.CheckState enumeration.
    /// </summary>
    public CheckState GetItemCheckState(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Items.Count);

        return CheckedItems.GetCheckedState(index);
    }

    /// <summary>
    ///  Indicates if the given item is, in any way, shape, or form, checked.
    ///  This will return true if the item is fully or indeterminately checked.
    /// </summary>
    public bool GetItemChecked(int index) => GetItemCheckState(index) != CheckState.Unchecked;

    /// <summary>
    ///  Invalidates the given item in the <see cref="ListBox"/>
    /// </summary>
    private unsafe void InvalidateItem(int index)
    {
        if (IsHandleCreated)
        {
            RECT rect = default;
            PInvoke.SendMessage(this, PInvoke.LB_GETITEMRECT, (WPARAM)index, ref rect);
            PInvoke.InvalidateRect(this, &rect, bErase: false);
        }
    }

    /// <summary>
    ///  A redirected LBN_SELCHANGE message notification.
    /// </summary>
    private void LbnSelChange()
    {
        // prepare to change the selection.  we'll fire an event for
        // this.  Note that we'll only change the selection when the
        // user clicks again on a currently selected item, or when the
        // user has CheckOnClick set to true.  Otherwise
        // just using the up and down arrows selects or deselects
        // every item around town ...
        //

        // Get the index of the item to check/uncheck
        int index = SelectedIndex;

        // make sure we have a valid index, otherwise we're going to
        // fail ahead...
        if (index < 0 || index >= Items.Count)
        {
            return;
        }

        // Send an accessibility notification
        //
        AccessibilityNotifyClients(AccessibleEvents.Focus, index);
        AccessibilityNotifyClients(AccessibleEvents.Selection, index);

        // # VS7 86
        if (!_killNextSelect && (index == _lastSelected || CheckOnClick))
        {
            CheckState currentValue = CheckedItems.GetCheckedState(index);
            CheckState newValue = (currentValue != CheckState.Unchecked)
                                  ? CheckState.Unchecked
                                  : CheckState.Checked;

            ItemCheckEventArgs itemCheckEvent = new(index, newValue, currentValue);
            OnItemCheck(itemCheckEvent);

            // take whatever value the user set, and set that as the value.
            //
            CheckedItems.SetCheckedState(index, itemCheckEvent.NewValue);

            // Send accessibility notifications for state change
            AccessibilityNotifyClients(AccessibleEvents.StateChange, index);
            AccessibilityNotifyClients(AccessibleEvents.NameChange, index);
        }

        _lastSelected = index;
        InvalidateItem(index);
    }

    /// <summary>
    ///  Ensures that mouse clicks can toggle...
    /// </summary>
    protected override void OnClick(EventArgs e)
    {
        _killNextSelect = false;
        base.OnClick(e);
    }

    /// <summary>
    ///  When the handle is created we can dump any cached item-check pairs.
    /// </summary>
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        PInvoke.SendMessage(this, PInvoke.LB_SETITEMHEIGHT, (WPARAM)0, (LPARAM)ItemHeight);
    }

    /// <summary>
    ///  Actually goes and fires the drawItem event.  Inheriting controls
    ///  should use this to know when the event is fired [this is preferable to
    ///  adding an event handler yourself for this event].  They should,
    ///  however, remember to call base.OnDrawItem(e); to ensure the event is
    ///  still fired to external listeners
    /// </summary>
    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        object item;

        if (Font.Height < 0)
        {
            Font = DefaultFont;
        }

        if (e.Index >= 0)
        {
            if (e.Index < Items.Count)
            {
                item = Items[e.Index];
            }
            else
            {
                // If the item is not part of our collection, we will just get the string for it and display it.

                item = NativeGetItemText(e.Index);
            }

            Rectangle bounds = e.Bounds;
            int height = ItemHeight;

            // Set up the appearance of the checkbox

            ButtonState state = ButtonState.Normal;
            if (_flat)
            {
                state |= ButtonState.Flat;
            }

            if (e.Index < Items.Count)
            {
                switch (CheckedItems.GetCheckedState(e.Index))
                {
                    case CheckState.Checked:
                        state |= ButtonState.Checked;
                        break;
                    case CheckState.Indeterminate:
                        state |= ButtonState.Checked | ButtonState.Inactive;
                        break;
                }
            }

            // If we are drawing themed CheckBox get the size from the renderer which can change with DPI.
            if (Application.RenderWithVisualStyles)
            {
                VisualStyles.CheckBoxState cbState = CheckBoxRenderer.ConvertFromButtonState(
                    state,
                    isMixed: false,
                    (e.State & DrawItemState.HotLight) == DrawItemState.HotLight);

                _idealCheckSize = CheckBoxRenderer.GetGlyphSize(e, cbState, HWNDInternal).Width;
            }

            // Determine bounds for the checkbox
            int centeringFactor = Math.Max((height - _idealCheckSize) / 2, 0);

            // Keep the checkbox within the item's upper and lower bounds
            if (centeringFactor + _idealCheckSize > bounds.Height)
            {
                centeringFactor = bounds.Height - _idealCheckSize;
            }

            Rectangle box = new(
                bounds.X + _listItemStartPosition,
                bounds.Y + centeringFactor,
                _idealCheckSize,
                _idealCheckSize);

            if (RightToLeft == RightToLeft.Yes)
            {
                // Draw the CheckBox at the right.
                box.X = bounds.X + bounds.Width - _idealCheckSize - _listItemStartPosition;
            }

            // Draw the checkbox.

            if (Application.RenderWithVisualStyles)
            {
                VisualStyles.CheckBoxState cbState = CheckBoxRenderer.ConvertFromButtonState(
                    state,
                    isMixed: false,
                    ((e.State & DrawItemState.HotLight) == DrawItemState.HotLight));

                CheckBoxRenderer.DrawCheckBoxWithVisualStyles(e, new Point(box.X, box.Y), cbState, HWNDInternal);
            }
            else
            {
                ControlPaint.DrawCheckBox(e.Graphics, box, state);
            }

            // Determine bounds for the text portion of the item
            Rectangle textBounds = new(
                bounds.X + _idealCheckSize + (_listItemStartPosition * 2),
                bounds.Y,
                bounds.Width - (_idealCheckSize + (_listItemStartPosition * 2)),
                bounds.Height);

            if (RightToLeft == RightToLeft.Yes)
            {
                // Draw text at the left.
                textBounds.X = bounds.X;
            }

            Color backColor = (SelectionMode != SelectionMode.None) ? e.BackColor : BackColor;
            Color foreColor = (SelectionMode != SelectionMode.None) ? e.ForeColor : ForeColor;
            if (!Enabled)
            {
                foreColor = Application.SystemColors.GrayText;
            }

            Font font = Font;

            // Setup text font, color, and text

            string? text = GetItemText(item);

            if (SelectionMode != SelectionMode.None && (e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                if (Enabled)
                {
                    backColor = Application.SystemColors.Highlight;
                    foreColor = Application.SystemColors.HighlightText;
                }
                else
                {
                    backColor = Application.SystemColors.InactiveBorder;
                    foreColor = Application.SystemColors.GrayText;
                }
            }

            // Draw the text

            // Due to some sort of unpredictable painting optimization in the Windows ListBox control,
            // we need to always paint the background rectangle for the current line.

            if (!backColor.HasTransparency())
            {
                using DeviceContextHdcScope hdc = new(e);
                using CreateBrushScope hbrush = new(backColor);
                hdc.FillRectangle(textBounds, hbrush);
            }
            else
            {
                // Need to use GDI+
                using var brush = backColor.GetCachedSolidBrushScope();
                e.GraphicsInternal.FillRectangle(brush, textBounds);
            }

            Rectangle stringBounds = new(
                textBounds.X + BORDER_SIZE,
                textBounds.Y,
                textBounds.Width - BORDER_SIZE,
                textBounds.Height - 2 * BORDER_SIZE); // minus borders

            if (UseCompatibleTextRendering)
            {
                using StringFormat format = new();
                if (UseTabStops)
                {
                    // Set tab stops so it looks similar to a ListBox, at least with the default font size.
                    float tabDistance = 3.6f * Font.Height; // about 7 characters
                    float[] tabStops = new float[15];
                    float tabOffset = -(_idealCheckSize + (_listItemStartPosition * 2));
                    for (int i = 1; i < tabStops.Length; i++)
                    {
                        tabStops[i] = tabDistance;
                    }

                    if (Math.Abs(tabOffset) < tabDistance)
                    {
                        tabStops[0] = tabDistance + tabOffset;
                    }
                    else
                    {
                        tabStops[0] = tabDistance;
                    }

                    format.SetTabStops(0, tabStops);
                }
                else if (UseCustomTabOffsets)
                {
                    // Set TabStops to userDefined values
                    int wpar = CustomTabOffsets.Count;
                    float[] tabStops = new float[wpar];
                    CustomTabOffsets.CopyTo(tabStops, 0);
                    format.SetTabStops(0, tabStops);
                }

                // Adjust string format for Rtl controls
                if (RightToLeft == RightToLeft.Yes)
                {
                    format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                }

                // ListBox doesn't word-wrap its items, so neither should CheckedListBox
                //
                format.FormatFlags |= StringFormatFlags.NoWrap;

                // Set Trimming to None to prevent DrawString() from whacking the entire
                // string when there is only one character per tab included in the string.
                format.Trimming = StringTrimming.None;

                // Do actual drawing
                using var brush = foreColor.GetCachedSolidBrushScope();
                e.Graphics.DrawString(text, font, brush, stringBounds, format);
            }
            else
            {
                TextFormatFlags flags = TextFormatFlags.Default;
                flags |= TextFormatFlags.NoPrefix;

                if (UseTabStops || UseCustomTabOffsets)
                {
                    flags |= TextFormatFlags.ExpandTabs;
                }

                // Adjust string format for Rtl controls
                if (RightToLeft == RightToLeft.Yes)
                {
                    flags |= TextFormatFlags.RightToLeft;
                    flags |= TextFormatFlags.Right;
                }

                // Do actual drawing
                TextRenderer.DrawText(e, text, font, stringBounds, foreColor, flags);
            }

            // Draw the focus rect if required

            if ((e.State & DrawItemState.Focus) == DrawItemState.Focus &&
                (e.State & DrawItemState.NoFocusRect) != DrawItemState.NoFocusRect)
            {
                ControlPaint.DrawBlackWhiteFocusRectangle(e.Graphics, textBounds, backColor);
            }
        }

        if (Items.Count == 0 &&
            e.Bounds.Width > 2 * BORDER_SIZE && e.Bounds.Height > 2 * BORDER_SIZE)
        {
            Color backColor = (SelectionMode != SelectionMode.None) ? e.BackColor : BackColor;
            Rectangle bounds = e.Bounds;
            Rectangle emptyRectangle = new(
                bounds.X + BORDER_SIZE,
                bounds.Y,
                bounds.Width - BORDER_SIZE,
                bounds.Height - 2 * BORDER_SIZE); // Upper and lower borders.

            if (Focused)
            {
                // Draw focus rectangle for virtual first item in the list if there are no items in the list.
                Color foreColor = (SelectionMode != SelectionMode.None) ? e.ForeColor : ForeColor;
                if (!Enabled)
                {
                    foreColor = Application.SystemColors.GrayText;
                }

                ControlPaint.DrawFocusRectangle(e.Graphics, emptyRectangle, foreColor, backColor);
            }
            else if (!Application.RenderWithVisualStyles)
            {
                // If VisualStyles are off, rectangle needs to be explicitly erased, when focus is lost.
                // This is because of persisting empty focus rectangle when VisualStyles are off.
                using var brush = backColor.GetCachedSolidBrushScope();
                e.Graphics.FillRectangle(brush, emptyRectangle);
            }
        }
    }

    protected override unsafe void OnBackColorChanged(EventArgs e)
    {
        base.OnBackColorChanged(e);

        if (IsHandleCreated)
        {
            PInvoke.InvalidateRect(this, null, true);
        }
    }

    protected override void OnFontChanged(EventArgs e)
    {
        // Update the item height
        if (IsHandleCreated)
        {
            PInvoke.SendMessage(this, PInvoke.LB_SETITEMHEIGHT, (WPARAM)0, (LPARAM)ItemHeight);
        }

        // The base OnFontChanged will adjust the height of the CheckedListBox accordingly
        base.OnFontChanged(e);
    }

    /// <summary>
    ///  This is the code that actually fires the "keyPress" event.  The Checked
    ///  ListBox overrides this to look for space characters, since we
    ///  want to use those to check or uncheck items periodically.  Don't
    ///  forget to call base.OnKeyPress() to ensure that KeyPrese events
    ///  are correctly fired for all other keys.
    /// </summary>
    protected override void OnKeyPress(KeyPressEventArgs e)
    {
        if (e.KeyChar == ' ' && SelectionMode != SelectionMode.None)
        {
            LbnSelChange();
        }

        if (FormattingEnabled) // We want to fire KeyPress only when FormattingEnabled (this is a whidbey property)
        {
            base.OnKeyPress(e);
        }
    }

    /// <summary>
    ///  This is the code that actually fires the itemCheck event.  Don't
    ///  forget to call base.onItemCheck() to ensure that itemCheck vents
    ///  are correctly fired for all other keys.
    /// </summary>
    protected virtual void OnItemCheck(ItemCheckEventArgs ice)
    {
        _onItemCheck?.Invoke(this, ice);

        if (IsAccessibilityObjectCreated)
        {
            AccessibleObject? checkedItem = AccessibilityObject.GetChild(ice.Index);

            checkedItem?.RaiseAutomationPropertyChangedEvent(UIA_PROPERTY_ID.UIA_ToggleToggleStatePropertyId, (VARIANT)(int)ice.CurrentValue, (VARIANT)(int)ice.NewValue);
        }
    }

    protected override void OnMeasureItem(MeasureItemEventArgs e)
    {
        base.OnMeasureItem(e);

        // we'll use the ideal checkbox size plus enough for padding on the top
        // and bottom
        if (e.ItemHeight < _idealCheckSize + 2)
        {
            e.ItemHeight = _idealCheckSize + 2;
        }
    }

    /// <summary>
    ///  Actually goes and fires the selectedIndexChanged event.  Inheriting controls
    ///  should use this to know when the event is fired [this is preferable to
    ///  adding an event handler on yourself for this event].  They should,
    ///  however, remember to call base.OnSelectedIndexChanged(e); to ensure the event is
    ///  still fired to external listeners
    /// </summary>
    protected override void OnSelectedIndexChanged(EventArgs e)
    {
        base.OnSelectedIndexChanged(e);
        _lastSelected = SelectedIndex;
    }

    /// <summary>
    ///  Reparses the objects, getting new text strings for them.
    /// </summary>
    protected override void RefreshItems()
    {
        CheckState[] savedCheckedItems = new CheckState[Items.Count];
        for (int i = 0; i < Items.Count; i++)
        {
            savedCheckedItems[i] = CheckedItems.GetCheckedState(i);
        }

        // call the base
        base.RefreshItems();

        // restore the checkedItems...
        for (int j = 0; j < Items.Count; j++)
        {
            CheckedItems.SetCheckedState(j, savedCheckedItems[j]);
        }
    }

    /// <summary>
    ///  Sets the checked value of the given item.  This value should be from
    ///  the System.Windows.Forms.CheckState enumeration.
    /// </summary>
    public void SetItemCheckState(int index, CheckState value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Items.Count);

        // valid values are 0-2 inclusive.
        SourceGenerated.EnumValidator.Validate(value);
        CheckState currentValue = CheckedItems.GetCheckedState(index);

        if (value != currentValue)
        {
            ItemCheckEventArgs itemCheckEvent = new(index, value, currentValue);
            OnItemCheck(itemCheckEvent);

            if (itemCheckEvent.NewValue != currentValue)
            {
                CheckedItems.SetCheckedState(index, itemCheckEvent.NewValue);
                InvalidateItem(index);
            }
        }
    }

    /// <summary>
    ///  Sets the checked value of the given item.  This value should be a
    ///  boolean.
    /// </summary>
    public void SetItemChecked(int index, bool value)
    {
        SetItemCheckState(index, value ? CheckState.Checked : CheckState.Unchecked);
    }

    /// <summary>
    ///  We need to get LBN_SELCHANGE notifications.
    /// </summary>
    protected override void WmReflectCommand(ref Message m)
    {
        switch ((uint)m.WParamInternal.SIGNEDHIWORD)
        {
            case PInvoke.LBN_SELCHANGE:
                LbnSelChange();
                base.WmReflectCommand(ref m);
                break;

            case PInvoke.LBN_DBLCLK:
                // We want double-clicks to change the checkState on each click - just like the CheckBox control
                LbnSelChange();
                base.WmReflectCommand(ref m);
                break;

            default:
                base.WmReflectCommand(ref m);
                break;
        }
    }

    /// <summary>
    ///  Handle keyboard input to prevent arrow keys from toggling
    ///  checkboxes in CheckOnClick mode.
    /// </summary>
    private void WmReflectVKeyToItem(ref Message m)
    {
        Keys keycode = (Keys)m.WParamInternal.LOWORD;
        _killNextSelect = keycode switch
        {
            Keys.Up or Keys.Down or Keys.PageUp or Keys.PageDown or Keys.Home or Keys.End or Keys.Left or Keys.Right => true,
            _ => false,
        };

        m.ResultInternal = (LRESULT)(-1);
    }

    /// <summary>
    ///  The listBox's window procedure.  Inheriting classes can override this
    ///  to add extra functionality, but should not forget to call
    ///  base.wndProc(m); to ensure the button continues to function properly.
    /// </summary>
    protected override void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case MessageId.WM_REFLECT_CHARTOITEM:
                m.ResultInternal = (LRESULT)(-1);
                break;
            case MessageId.WM_REFLECT_VKEYTOITEM:
                WmReflectVKeyToItem(ref m);
                break;
            default:
                if (m.MsgInternal == LBC_GETCHECKSTATE)
                {
                    int item = (int)m.WParamInternal;
                    if (item < 0 || item >= Items.Count)
                    {
                        m.ResultInternal = (LRESULT)PInvoke.LB_ERR;
                    }
                    else
                    {
                        m.ResultInternal = (LRESULT)(GetItemChecked(item) ? LB_CHECKED : LB_UNCHECKED);
                    }
                }
                else if (m.MsgInternal == LBC_SETCHECKSTATE)
                {
                    int item = (int)m.WParamInternal;
                    int state = (int)m.LParamInternal;
                    if (item < 0 || item >= Items.Count || (state != LB_CHECKED && state != LB_UNCHECKED))
                    {
                        m.ResultInternal = (LRESULT)0;
                    }
                    else
                    {
                        SetItemChecked(item, (state == LB_CHECKED));
                        m.ResultInternal = (LRESULT)1;
                    }
                }
                else
                {
                    base.WndProc(ref m);
                }

                break;
        }
    }
}
