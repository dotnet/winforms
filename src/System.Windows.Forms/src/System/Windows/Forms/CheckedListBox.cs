// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using Hashtable = System.Collections.Hashtable;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///
    ///  Displays a list with a checkbox to the left
    ///
    ///  of each item.
    /// </summary>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    LookupBindingProperties(), // ...overrides equivalent attribute in ListControl
    SRDescription(nameof(SR.DescriptionCheckedListBox))
    ]
    public class CheckedListBox : ListBox
    {
        private int idealCheckSize = 13;

        private const int LB_CHECKED = 1;
        private const int LB_UNCHECKED = 0;
        private const int LB_ERROR = -1;
        private const int BORDER_SIZE = 1;

        /// <summary>
        ///  Decides whether or not to ignore the next LBN_SELCHANGE
        ///  message - used to prevent cursor keys from toggling checkboxes
        /// </summary>
        private bool killnextselect = false;

        /// <summary>
        ///  Current listener of the onItemCheck event.
        /// </summary>
        private ItemCheckEventHandler onItemCheck;

        /// <summary>
        ///  Indicates whether or not we should toggle check state on the first
        ///  click on an item, or whether we should wait for the user to click
        ///  again.
        /// </summary>
        private bool checkOnClick = false;

        /// <summary>
        ///  Should we use 3d checkboxes or flat ones?
        /// </summary>
        private bool flat = true;

        /// <summary>
        ///  Indicates which item was last selected.  We want to keep track
        ///  of this so we can be a little less aggressive about checking/
        ///  unchecking the items as the user moves around.
        /// </summary>
        private int lastSelected = -1;

        /// <summary>
        ///  The collection of checked items in the CheckedListBox.
        /// </summary>
        private CheckedItemCollection checkedItemCollection = null;
        private CheckedIndexCollection checkedIndexCollection = null;

        private static readonly int LBC_GETCHECKSTATE;
        private static readonly int LBC_SETCHECKSTATE;

        static CheckedListBox()
        {
            LBC_GETCHECKSTATE = SafeNativeMethods.RegisterWindowMessage("LBC_GETCHECKSTATE");
            LBC_SETCHECKSTATE = SafeNativeMethods.RegisterWindowMessage("LBC_SETCHECKSTATE");
        }

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
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.CheckedListBoxCheckOnClickDescr))
        ]
        public bool CheckOnClick
        {
            get
            {
                return checkOnClick;
            }

            set
            {
                checkOnClick = value;
            }
        }

        /// <summary>
        ///  Collection of checked indices in this CheckedListBox.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        ]
        public CheckedIndexCollection CheckedIndices
        {
            get
            {
                if (checkedIndexCollection == null)
                {
                    checkedIndexCollection = new CheckedIndexCollection(this);
                }
                return checkedIndexCollection;
            }
        }

        /// <summary>
        ///  Collection of checked items in this CheckedListBox.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        ]
        public CheckedItemCollection CheckedItems
        {
            get
            {
                if (checkedItemCollection == null)
                {
                    checkedItemCollection = new CheckedItemCollection(this);
                }
                return checkedItemCollection;
            }
        }

        /// <summary>
        ///  This is called when creating a window.  Inheriting classes can ovveride
        ///  this to add extra functionality, but should not forget to first call
        ///  base.CreateParams() to make sure the control continues to work
        ///  correctly.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= NativeMethods.LBS_OWNERDRAWFIXED | NativeMethods.LBS_WANTKEYBOARDINPUT;
                return cp;
            }
        }

        /// <summary>
        ///  CheckedListBox DataSource.
        /// </summary>
        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new object DataSource
        {
            get
            {
                return base.DataSource;
            }
            set
            {
                base.DataSource = value;
            }
        }

        /// <summary>
        ///  CheckedListBox DisplayMember.
        /// </summary>
        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new string DisplayMember
        {
            get
            {
                return base.DisplayMember;
            }
            set
            {
                base.DisplayMember = value;
            }
        }

        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        ]
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

        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        ]
        public override int ItemHeight
        {
            get
            {
                // this should take FontHeight + buffer into Consideration.
                return Font.Height + scaledListItemBordersHeight;
            }
            set
            {
            }
        }

        /// <summary>
        ///  Collection of items in this listbox.
        /// </summary>
        [
        SRCategory(nameof(SR.CatData)),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Localizable(true),
        SRDescription(nameof(SR.ListBoxItemsDescr)),
        Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))
        ]
        new public ObjectCollection Items
        {
            get
            {
                return (ObjectCollection)base.Items;
            }
        }

        // Computes the maximum width of all items in the ListBox
        //
        internal override int MaxItemWidth
        {
            get
            {
                // Overridden to include the size of the checkbox
                // Allows for one pixel either side of the checkbox, plus another 1 pixel buffer = 3 pixels
                //
                return base.MaxItemWidth + idealCheckSize + scaledListItemPaddingBuffer;
            }
        }

        /// <summary>
        ///  For CheckedListBoxes, multi-selection is not supported.  You can set
        ///  selection to be able to select one item or no items.
        /// </summary>
        public override SelectionMode SelectionMode
        {
            get
            {
                return base.SelectionMode;
            }
            set
            {
                //valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)SelectionMode.None, (int)SelectionMode.MultiExtended))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(SelectionMode));
                }
                if (value != SelectionMode.One
                    && value != SelectionMode.None)
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
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(false),
        SRDescription(nameof(SR.CheckedListBoxThreeDCheckBoxesDescr))
        ]
        public bool ThreeDCheckBoxes
        {
            get
            {
                return !flat;
            }
            set
            {
                // change the style and repaint.
                //
                if (flat == value)
                {
                    flat = !value;

                    // see if we have some items, and only invalidate if we do.
                    ObjectCollection items = (ObjectCollection)Items;
                    if ((items != null) && (items.Count > 0))
                    {
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        ///  Determines whether to use compatible text rendering engine (GDI+) or not (GDI).
        /// </summary>
        [
        DefaultValue(false),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.UseCompatibleTextRenderingDescr))
        ]
        public bool UseCompatibleTextRendering
        {
            get
            {
                return base.UseCompatibleTextRenderingInt;
            }
            set
            {
                base.UseCompatibleTextRenderingInt = value;
            }
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
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new string ValueMember
        {
            get
            {
                return base.ValueMember;
            }
            set
            {
                base.ValueMember = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler DataSourceChanged
        {
            add => base.DataSourceChanged += value;
            remove => base.DataSourceChanged -= value;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler DisplayMemberChanged
        {
            add => base.DisplayMemberChanged += value;
            remove => base.DisplayMemberChanged -= value;
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.CheckedListBoxItemCheckDescr))]
        public event ItemCheckEventHandler ItemCheck
        {
            add => onItemCheck += value;
            remove => onItemCheck -= value;
        }

        /// <hideinheritance/>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public new event EventHandler Click
        {
            add => base.Click += value;
            remove => base.Click -= value;
        }

        /// <hideinheritance/>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public new event MouseEventHandler MouseClick
        {
            add => base.MouseClick += value;
            remove => base.MouseClick -= value;
        }

        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event DrawItemEventHandler DrawItem
        {
            add => base.DrawItem += value;
            remove => base.DrawItem -= value;
        }

        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event MeasureItemEventHandler MeasureItem
        {
            add => base.MeasureItem += value;
            remove => base.MeasureItem -= value;
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new Padding Padding
        {
            get { return base.Padding; }
            set { base.Padding = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler ValueMemberChanged
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
            if (index < 0 || index >= Items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
            }

            return CheckedItems.GetCheckedState(index);
        }

        /// <summary>
        ///  Indicates if the given item is, in any way, shape, or form, checked.
        ///  This will return true if the item is fully or indeterminately checked.
        /// </summary>
        public bool GetItemChecked(int index)
        {
            return (GetItemCheckState(index) != CheckState.Unchecked);
        }

        /// <summary>
        ///  Invalidates the given item in the listbox
        /// </summary>
        private void InvalidateItem(int index)
        {
            if (IsHandleCreated)
            {
                RECT rect = new RECT();
                SendMessage(NativeMethods.LB_GETITEMRECT, index, ref rect);
                SafeNativeMethods.InvalidateRect(new HandleRef(this, Handle), ref rect, false);
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
            // just using the up and down arrows selects or unselects
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

            //# VS7 86
            if (!killnextselect && (index == lastSelected || checkOnClick == true))
            {
                CheckState currentValue = CheckedItems.GetCheckedState(index);
                CheckState newValue = (currentValue != CheckState.Unchecked)
                                      ? CheckState.Unchecked
                                      : CheckState.Checked;

                ItemCheckEventArgs itemCheckEvent = new ItemCheckEventArgs(index, newValue, currentValue);
                OnItemCheck(itemCheckEvent);

                // take whatever value the user set, and set that as the value.
                //
                CheckedItems.SetCheckedState(index, itemCheckEvent.NewValue);

                // Send accessibility notifications for state change
                AccessibilityNotifyClients(AccessibleEvents.StateChange, index);
                AccessibilityNotifyClients(AccessibleEvents.NameChange, index);
            }

            lastSelected = index;
            InvalidateItem(index);
        }

        /// <summary>
        ///  Ensures that mouse clicks can toggle...
        /// </summary>
        protected override void OnClick(EventArgs e)
        {
            killnextselect = false;
            base.OnClick(e);
        }

        /// <summary>
        ///  When the handle is created we can dump any cached item-check pairs.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SendMessage(NativeMethods.LB_SETITEMHEIGHT, 0, ItemHeight);

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
                Font = Control.DefaultFont;
            }

            if (e.Index >= 0)
            {
                if (e.Index < Items.Count)
                {
                    item = Items[e.Index];
                }
                else
                {
                    // If the item is not part of our collection, we will just
                    // get the string for it and display it.
                    //
                    item = NativeGetItemText(e.Index);
                }

                Rectangle bounds = e.Bounds;
                int height = ItemHeight;

                // set up the appearance of the checkbox
                //
                ButtonState state = ButtonState.Normal;
                if (flat)
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

                // If we are drawing themed CheckBox .. get the size from renderer..
                // the Renderer might return a different size in different DPI modes..
                if (Application.RenderWithVisualStyles)
                {
                    VisualStyles.CheckBoxState cbState = CheckBoxRenderer.ConvertFromButtonState(state, false, ((e.State & DrawItemState.HotLight) == DrawItemState.HotLight));
                    idealCheckSize = (int)(CheckBoxRenderer.GetGlyphSize(e.Graphics, cbState, HandleInternal)).Width;
                }

                // Determine bounds for the checkbox
                //
                int centeringFactor = Math.Max((height - idealCheckSize) / 2, 0);

                // Keep the checkbox within the item's upper and lower bounds
                if (centeringFactor + idealCheckSize > bounds.Height)
                {
                    centeringFactor = bounds.Height - idealCheckSize;
                }

                Rectangle box = new Rectangle(bounds.X + scaledListItemStartPosition,
                                              bounds.Y + centeringFactor,
                                              idealCheckSize,
                                              idealCheckSize);

                if (RightToLeft == RightToLeft.Yes)
                {
                    // For a RightToLeft checked list box, we want the checkbox
                    // to be drawn at the right.
                    // So we override the X position.
                    box.X = bounds.X + bounds.Width - idealCheckSize - scaledListItemStartPosition;
                }

                // Draw the checkbox.
                //
                if (Application.RenderWithVisualStyles)
                {
                    VisualStyles.CheckBoxState cbState = CheckBoxRenderer.ConvertFromButtonState(state, false, ((e.State & DrawItemState.HotLight) == DrawItemState.HotLight));
                    CheckBoxRenderer.DrawCheckBox(e.Graphics, new Point(box.X, box.Y), cbState, HandleInternal);
                }
                else
                {
                    ControlPaint.DrawCheckBox(e.Graphics, box, state);
                }

                // Determine bounds for the text portion of the item
                //
                Rectangle textBounds = new Rectangle(
                                                    bounds.X + idealCheckSize + (scaledListItemStartPosition * 2),
                                                    bounds.Y,
                                                    bounds.Width - (idealCheckSize + (scaledListItemStartPosition * 2)),
                                                    bounds.Height);
                if (RightToLeft == RightToLeft.Yes)
                {
                    // For a RightToLeft checked list box, we want the text
                    // to be drawn at the left.
                    // So we override the X position.
                    textBounds.X = bounds.X;
                }

                // Setup text font, color, and text
                //
                string text = string.Empty;
                Color backColor = (SelectionMode != SelectionMode.None) ? e.BackColor : BackColor;
                Color foreColor = (SelectionMode != SelectionMode.None) ? e.ForeColor : ForeColor;
                if (!Enabled)
                {
                    foreColor = SystemColors.GrayText;
                }
                Font font = Font;

                text = GetItemText(item);

                if (SelectionMode != SelectionMode.None && (e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    if (Enabled)
                    {
                        backColor = SystemColors.Highlight;
                        foreColor = SystemColors.HighlightText;
                    }
                    else
                    {
                        backColor = SystemColors.InactiveBorder;
                        foreColor = SystemColors.GrayText;
                    }
                }

                // Draw the text
                //

                // Due to some sort of unpredictable painting optimization in the Windows ListBox control,
                // we need to always paint the background rectangle for the current line.
                using (Brush b = new SolidBrush(backColor))
                {
                    e.Graphics.FillRectangle(b, textBounds);
                }

                Rectangle stringBounds = new Rectangle(
                                                      textBounds.X + BORDER_SIZE,
                                                      textBounds.Y,
                                                      textBounds.Width - BORDER_SIZE,
                                                      textBounds.Height - 2 * BORDER_SIZE); // minus borders

                if (UseCompatibleTextRendering)
                {
                    using (StringFormat format = new StringFormat())
                    {
                        if (UseTabStops)
                        {
                            //  Set tab stops so it looks similar to a ListBox, at least with the default font size.
                            float tabDistance = 3.6f * Font.Height; // about 7 characters
                            float[] tabStops = new float[15];
                            float tabOffset = -(idealCheckSize + (scaledListItemStartPosition * 2));
                            for (int i = 1; i < tabStops.Length; i++)
                            {
                                tabStops[i] = tabDistance;
                            }

                            //(
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
                            //Set TabStops to userDefined values
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
                        using (SolidBrush brush = new SolidBrush(foreColor))
                        {
                            e.Graphics.DrawString(text, font, brush, stringBounds, format);
                        }
                    }
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
                    TextRenderer.DrawText(e.Graphics, text, font, stringBounds, foreColor, flags);
                }

                // Draw the focus rect if required
                //
                if ((e.State & DrawItemState.Focus) == DrawItemState.Focus &&
                    (e.State & DrawItemState.NoFocusRect) != DrawItemState.NoFocusRect)
                {
                    ControlPaint.DrawFocusRectangle(e.Graphics, textBounds, foreColor, backColor);
                }
            }

            if (Items.Count == 0 &&
                e.Bounds.Width > 2 * BORDER_SIZE && e.Bounds.Height > 2 * BORDER_SIZE)
            {
                Color backColor = (SelectionMode != SelectionMode.None) ? e.BackColor : BackColor;
                Rectangle bounds = e.Bounds;
                Rectangle emptyRectangle = new Rectangle(
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
                        foreColor = SystemColors.GrayText;
                    }

                    ControlPaint.DrawFocusRectangle(e.Graphics, emptyRectangle, foreColor, backColor);
                }
                else if (!Application.RenderWithVisualStyles)
                {
                    // If VisualStyles are off, rectangle needs to be explicitly erased, when focus is lost.
                    // This is because of persisting empty focus rectangle when VisualStyles are off.
                    using (Brush brush = new SolidBrush(backColor))
                    {
                        e.Graphics.FillRectangle(brush, emptyRectangle);
                    }
                }
            }
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);

            if (IsHandleCreated)
            {
                SafeNativeMethods.InvalidateRect(new HandleRef(this, Handle), null, true);
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            // Update the item height
            //
            if (IsHandleCreated)
            {
                SendMessage(NativeMethods.LB_SETITEMHEIGHT, 0, ItemHeight);
            }

            // The base OnFontChanged will adjust the height of the CheckedListBox accordingly
            //
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
            if (FormattingEnabled) //We want to fire KeyPress only when FormattingEnabled (this is a whidbey property)
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
            onItemCheck?.Invoke(this, ice);
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            base.OnMeasureItem(e);

            // we'll use the ideal checkbox size plus enough for padding on the top
            // and bottom
            //
            if (e.ItemHeight < idealCheckSize + 2)
            {
                e.ItemHeight = idealCheckSize + 2;
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
            lastSelected = SelectedIndex;

        }

        /// <summary>
        ///  Reparses the objects, getting new text strings for them.
        /// </summary>
        protected override void RefreshItems()
        {
            Hashtable savedcheckedItems = new Hashtable();
            for (int i = 0; i < Items.Count; i++)
            {
                savedcheckedItems[i] = CheckedItems.GetCheckedState(i);
            }

            //call the base
            base.RefreshItems();
            // restore the checkedItems...

            for (int j = 0; j < Items.Count; j++)
            {
                CheckedItems.SetCheckedState(j, (CheckState)savedcheckedItems[j]);
            }
        }

        /// <summary>
        ///  Sets the checked value of the given item.  This value should be from
        ///  the System.Windows.Forms.CheckState enumeration.
        /// </summary>
        public void SetItemCheckState(int index, CheckState value)
        {
            if (index < 0 || index >= Items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
            }
            // valid values are 0-2 inclusive.
            if (!ClientUtils.IsEnumValid(value, (int)value, (int)CheckState.Unchecked, (int)CheckState.Indeterminate))
            {
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(CheckState));
            }
            CheckState currentValue = CheckedItems.GetCheckedState(index);

            if (value != currentValue)
            {
                ItemCheckEventArgs itemCheckEvent = new ItemCheckEventArgs(index, value, currentValue);
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
        ///  We need to get LBN_SELCHANGE notifications
        /// </summary>
        protected override void WmReflectCommand(ref Message m)
        {
            switch (NativeMethods.Util.HIWORD(m.WParam))
            {
                case NativeMethods.LBN_SELCHANGE:
                    LbnSelChange();
                    // finally, fire the OnSelectionChange event.
                    base.WmReflectCommand(ref m);
                    break;

                case NativeMethods.LBN_DBLCLK:
                    // We want double-clicks to change the checkstate on each click - just like the CheckBox control
                    //
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
            int keycode = NativeMethods.Util.LOWORD(m.WParam);
            switch ((Keys)keycode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.Home:
                case Keys.End:
                case Keys.Left:
                case Keys.Right:
                    killnextselect = true;
                    break;
                default:
                    killnextselect = false;
                    break;
            }
            m.Result = NativeMethods.InvalidIntPtr;
        }

        /// <summary>
        ///  The listbox's window procedure.  Inheriting classes can override this
        ///  to add extra functionality, but should not forget to call
        ///  base.wndProc(m); to ensure the button continues to function properly.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WindowMessages.WM_REFLECT + WindowMessages.WM_CHARTOITEM:
                    m.Result = NativeMethods.InvalidIntPtr;
                    break;
                case WindowMessages.WM_REFLECT + WindowMessages.WM_VKEYTOITEM:
                    WmReflectVKeyToItem(ref m);
                    break;
                default:
                    if (m.Msg == LBC_GETCHECKSTATE)
                    {
                        int item = unchecked((int)(long)m.WParam);
                        if (item < 0 || item >= Items.Count)
                        {
                            m.Result = (IntPtr)LB_ERROR;
                        }
                        else
                        {
                            m.Result = (IntPtr)(GetItemChecked(item) ? LB_CHECKED : LB_UNCHECKED);
                        }
                    }
                    else if (m.Msg == LBC_SETCHECKSTATE)
                    {
                        int item = unchecked((int)(long)m.WParam);
                        int state = unchecked((int)(long)m.LParam);
                        if (item < 0 || item >= Items.Count || (state != LB_CHECKED && state != LB_UNCHECKED))
                        {
                            m.Result = IntPtr.Zero;
                        }
                        else
                        {
                            SetItemChecked(item, (state == LB_CHECKED));
                            m.Result = (IntPtr)1;
                        }
                    }
                    else
                    {
                        base.WndProc(ref m);
                    }
                    break;
            }
        }

        new public class ObjectCollection : ListBox.ObjectCollection
        {
            private readonly CheckedListBox owner;

            public ObjectCollection(CheckedListBox owner)
            : base(owner)
            {
                this.owner = owner;
            }

            /// <summary>
            ///  Lets the user add an item to the listbox with the given initial value
            ///  for the Checked portion of the item.
            /// </summary>
            public int Add(object item, bool isChecked)
            {
                return Add(item, isChecked ? CheckState.Checked : CheckState.Unchecked);
            }

            /// <summary>
            ///  Lets the user add an item to the listbox with the given initial value
            ///  for the Checked portion of the item.
            /// </summary>
            public int Add(object item, CheckState check)
            {

                //validate the enum that's passed in here
                //
                // Valid values are 0-2 inclusive.
                if (!ClientUtils.IsEnumValid(check, (int)check, (int)CheckState.Unchecked, (int)CheckState.Indeterminate))
                {
                    throw new InvalidEnumArgumentException(nameof(check), (int)check, typeof(CheckState));
                }

                int index = base.Add(item);
                owner.SetItemCheckState(index, check);

                return index;
            }
        }

        public class CheckedIndexCollection : IList
        {
            private readonly CheckedListBox owner;

            internal CheckedIndexCollection(CheckedListBox owner)
            {
                this.owner = owner;
            }

            /// <summary>
            ///  Number of current checked items.
            /// </summary>
            public int Count
            {
                get
                {
                    return owner.CheckedItems.Count;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            bool IList.IsFixedSize
            {
                get
                {
                    return true;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            /// <summary>
            ///  Retrieves the specified checked item.
            /// </summary>
            [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public int this[int index]
            {
                get
                {
                    object identifier = InnerArray.GetEntryObject(index, CheckedItemCollection.AnyMask);
                    return InnerArray.IndexOfIdentifier(identifier, 0);
                }
            }

            object IList.this[int index]
            {
                get
                {
                    return this[index];
                }
                set
                {
                    throw new NotSupportedException(SR.CheckedListBoxCheckedIndexCollectionIsReadOnly);
                }
            }

            int IList.Add(object value)
            {
                throw new NotSupportedException(SR.CheckedListBoxCheckedIndexCollectionIsReadOnly);
            }

            void IList.Clear()
            {
                throw new NotSupportedException(SR.CheckedListBoxCheckedIndexCollectionIsReadOnly);
            }

            void IList.Insert(int index, object value)
            {
                throw new NotSupportedException(SR.CheckedListBoxCheckedIndexCollectionIsReadOnly);
            }

            void IList.Remove(object value)
            {
                throw new NotSupportedException(SR.CheckedListBoxCheckedIndexCollectionIsReadOnly);
            }

            void IList.RemoveAt(int index)
            {
                throw new NotSupportedException(SR.CheckedListBoxCheckedIndexCollectionIsReadOnly);
            }

            public bool Contains(int index)
            {
                return (IndexOf(index) != -1);
            }

            bool IList.Contains(object index)
            {
                if (index is int)
                {
                    return Contains((int)index);
                }
                else
                {
                    return false;
                }
            }

            public void CopyTo(Array dest, int index)
            {
                int cnt = owner.CheckedItems.Count;
                for (int i = 0; i < cnt; i++)
                {
                    dest.SetValue(this[i], i + index);
                }
            }

            /// <summary>
            ///  This is the item array that stores our data.  We share this backing store
            ///  with the main object collection.
            /// </summary>
            private ItemArray InnerArray
            {
                get
                {
                    return ((ObjectCollection)owner.Items).InnerArray;
                }
            }

            public IEnumerator GetEnumerator()
            {
                int[] indices = new int[Count];
                CopyTo(indices, 0);
                return indices.GetEnumerator();
            }

            public int IndexOf(int index)
            {
                if (index >= 0 && index < owner.Items.Count)
                {
                    object value = InnerArray.GetEntryObject(index, 0);
                    return owner.CheckedItems.IndexOfIdentifier(value);
                }
                return -1;
            }

            int IList.IndexOf(object index)
            {
                if (index is int)
                {
                    return IndexOf((int)index);
                }
                else
                {
                    return -1;
                }
            }

        }

        public class CheckedItemCollection : IList
        {
            internal static int CheckedItemMask = ItemArray.CreateMask();
            internal static int IndeterminateItemMask = ItemArray.CreateMask();
            internal static int AnyMask = CheckedItemMask | IndeterminateItemMask;

            private readonly CheckedListBox owner;

            internal CheckedItemCollection(CheckedListBox owner)
            {
                this.owner = owner;
            }

            /// <summary>
            ///  Number of current checked items.
            /// </summary>
            public int Count
            {
                get
                {
                    return InnerArray.GetCount(AnyMask);
                }
            }

            /// <summary>
            ///  This is the item array that stores our data.  We share this backing store
            ///  with the main object collection.
            /// </summary>
            private ItemArray InnerArray
            {
                get
                {
                    return ((ListBox.ObjectCollection)owner.Items).InnerArray;
                }
            }

            /// <summary>
            ///  Retrieves the specified checked item.
            /// </summary>
            [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public object this[int index]
            {
                get
                {
                    return InnerArray.GetItem(index, AnyMask);
                }
                set
                {
                    throw new NotSupportedException(SR.CheckedListBoxCheckedItemCollectionIsReadOnly);
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            bool IList.IsFixedSize
            {
                get
                {
                    return true;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            public bool Contains(object item)
            {
                return IndexOf(item) != -1;
            }

            public int IndexOf(object item)
            {
                return InnerArray.IndexOf(item, AnyMask);
            }

            internal int IndexOfIdentifier(object item)
            {
                return InnerArray.IndexOfIdentifier(item, AnyMask);
            }

            int IList.Add(object value)
            {
                throw new NotSupportedException(SR.CheckedListBoxCheckedItemCollectionIsReadOnly);
            }

            void IList.Clear()
            {
                throw new NotSupportedException(SR.CheckedListBoxCheckedItemCollectionIsReadOnly);
            }

            void IList.Insert(int index, object value)
            {
                throw new NotSupportedException(SR.CheckedListBoxCheckedItemCollectionIsReadOnly);
            }

            void IList.Remove(object value)
            {
                throw new NotSupportedException(SR.CheckedListBoxCheckedItemCollectionIsReadOnly);
            }

            void IList.RemoveAt(int index)
            {
                throw new NotSupportedException(SR.CheckedListBoxCheckedItemCollectionIsReadOnly);
            }

            public void CopyTo(Array dest, int index)
            {
                int cnt = InnerArray.GetCount(AnyMask);
                for (int i = 0; i < cnt; i++)
                {
                    dest.SetValue(InnerArray.GetItem(i, AnyMask), i + index);
                }
            }

            /// <summary>
            ///  This method returns if the actual item index is checked.  The index is the index to the MAIN
            ///  collection, not this one.
            /// </summary>
            internal CheckState GetCheckedState(int index)
            {
                bool isChecked = InnerArray.GetState(index, CheckedItemMask);
                bool isIndeterminate = InnerArray.GetState(index, IndeterminateItemMask);
                Debug.Assert(!isChecked || !isIndeterminate, "Can't be both checked and indeterminate.  Somebody broke our state.");
                if (isIndeterminate)
                {
                    return CheckState.Indeterminate;
                }
                else if (isChecked)
                {
                    return CheckState.Checked;
                }

                return CheckState.Unchecked;
            }

            public IEnumerator GetEnumerator()
            {
                return InnerArray.GetEnumerator(AnyMask, true);
            }

            /// <summary>
            ///  Same thing for GetChecked.
            /// </summary>
            internal void SetCheckedState(int index, CheckState value)
            {
                bool isChecked;
                bool isIndeterminate;

                switch (value)
                {
                    case CheckState.Checked:
                        isChecked = true;
                        isIndeterminate = false;
                        break;

                    case CheckState.Indeterminate:
                        isChecked = false;
                        isIndeterminate = true;
                        break;

                    default:
                        isChecked = false;
                        isIndeterminate = false;
                        break;
                }

                bool wasChecked = InnerArray.GetState(index, CheckedItemMask);
                bool wasIndeterminate = InnerArray.GetState(index, IndeterminateItemMask);

                InnerArray.SetState(index, CheckedItemMask, isChecked);
                InnerArray.SetState(index, IndeterminateItemMask, isIndeterminate);

                if (wasChecked != isChecked || wasIndeterminate != isIndeterminate)
                {
                    // Raise a notify event that this item has changed.
                    owner.AccessibilityNotifyClients(AccessibleEvents.StateChange, index);
                }
            }
        }

        internal override bool SupportsUiaProviders => false;

        [ComVisible(true)]
        internal class CheckedListBoxAccessibleObject : ControlAccessibleObject
        {
            /// <summary>
            /// </summary>
            public CheckedListBoxAccessibleObject(CheckedListBox owner) : base(owner)
            {
            }

            private CheckedListBox CheckedListBox
            {
                get
                {
                    return (CheckedListBox)Owner;
                }
            }

            /// <summary>
            /// </summary>
            public override AccessibleObject GetChild(int index)
            {
                if (index >= 0 && index < CheckedListBox.Items.Count)
                {
                    return new CheckedListBoxItemAccessibleObject(CheckedListBox.GetItemText(CheckedListBox.Items[index]), index, this);
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// </summary>
            public override int GetChildCount()
            {
                return CheckedListBox.Items.Count;
            }

            public override AccessibleObject GetFocused()
            {
                int index = CheckedListBox.FocusedIndex;
                if (index >= 0)
                {
                    return GetChild(index);
                }

                return null;
            }

            public override AccessibleObject GetSelected()
            {
                int index = CheckedListBox.SelectedIndex;
                if (index >= 0)
                {
                    return GetChild(index);
                }

                return null;
            }

            public override AccessibleObject HitTest(int x, int y)
            {

                // Within a child element?
                //
                int count = GetChildCount();
                for (int index = 0; index < count; ++index)
                {
                    AccessibleObject child = GetChild(index);
                    if (child.Bounds.Contains(x, y))
                    {
                        return child;
                    }
                }

                // Within the CheckedListBox bounds?
                //
                if (Bounds.Contains(x, y))
                {
                    return this;
                }

                return null;
            }

            public override AccessibleObject Navigate(AccessibleNavigation direction)
            {
                if (GetChildCount() > 0)
                {
                    if (direction == AccessibleNavigation.FirstChild)
                    {
                        return GetChild(0);
                    }
                    if (direction == AccessibleNavigation.LastChild)
                    {
                        return GetChild(GetChildCount() - 1);
                    }
                }
                return base.Navigate(direction);
            }
        }

        [ComVisible(true)]
        internal class CheckedListBoxItemAccessibleObject : AccessibleObject
        {
            private string name;
            private readonly int index;
            private readonly CheckedListBoxAccessibleObject parent;

            public CheckedListBoxItemAccessibleObject(string name, int index, CheckedListBoxAccessibleObject parent) : base()
            {
                this.name = name;
                this.parent = parent;
                this.index = index;
            }

            public override Rectangle Bounds
            {
                get
                {
                    Rectangle rect = ParentCheckedListBox.GetItemRectangle(index);

                    if (rect.IsEmpty)
                    {
                        return rect;
                    }

                    // Translate rect to screen coordinates
                    //
                    rect = ParentCheckedListBox.RectangleToScreen(rect);
                    Rectangle visibleArea = ParentCheckedListBox.RectangleToScreen(ParentCheckedListBox.ClientRectangle);

                    if (visibleArea.Bottom < rect.Bottom)
                    {
                        rect.Height = visibleArea.Bottom - rect.Top;
                    }

                    rect.Width = visibleArea.Width;

                    return rect;
                }
            }

            public override string DefaultAction
            {
                get
                {
                    if (ParentCheckedListBox.GetItemChecked(index))
                    {
                        return SR.AccessibleActionUncheck;
                    }
                    else
                    {
                        return SR.AccessibleActionCheck;
                    }
                }
            }

            private CheckedListBox ParentCheckedListBox
            {
                get
                {
                    return (CheckedListBox)parent.Owner;
                }
            }

            public override string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
            }

            public override AccessibleObject Parent
            {
                get
                {
                    return parent;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.CheckButton;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable;

                    // Checked state
                    //
                    switch (ParentCheckedListBox.GetItemCheckState(index))
                    {
                        case CheckState.Checked:
                            state |= AccessibleStates.Checked;
                            break;
                        case CheckState.Indeterminate:
                            state |= AccessibleStates.Indeterminate;
                            break;
                        case CheckState.Unchecked:
                            // No accessible state corresponding to unchecked
                            break;
                    }

                    // Selected state
                    //
                    if (ParentCheckedListBox.SelectedIndex == index)
                    {
                        state |= AccessibleStates.Selected | AccessibleStates.Focused;
                    }

                    if (ParentCheckedListBox.Focused && ParentCheckedListBox.SelectedIndex == -1)
                    {
                        state |= AccessibleStates.Focused;
                    }

                    return state;

                }
            }

            public override string Value
            {
                get
                {
                    return ParentCheckedListBox.GetItemChecked(index).ToString();
                }
            }

            public override void DoDefaultAction()
            {
                ParentCheckedListBox.SetItemChecked(index, !ParentCheckedListBox.GetItemChecked(index));
            }

            public override AccessibleObject Navigate(AccessibleNavigation direction)
            {
                // Down/Next
                //
                if (direction == AccessibleNavigation.Down ||
                    direction == AccessibleNavigation.Next)
                {
                    if (index < parent.GetChildCount() - 1)
                    {
                        return parent.GetChild(index + 1);
                    }
                }

                // Up/Previous
                //
                if (direction == AccessibleNavigation.Up ||
                    direction == AccessibleNavigation.Previous)
                {
                    if (index > 0)
                    {
                        return parent.GetChild(index - 1);
                    }
                }

                return base.Navigate(direction);
            }

            public override void Select(AccessibleSelection flags)
            {
                try
                {
                    ParentCheckedListBox.AccessibilityObject.GetSystemIAccessibleInternal().accSelect((int)flags, index + 1);
                }
                catch (ArgumentException)
                {
                    // In Everett, the CheckedListBox accessible children did not have any selection capability.
                    // In Whidbey, they delegate the selection capability to OLEACC.
                    // However, OLEACC does not deal w/ several Selection flags: ExtendSelection, AddSelection, RemoveSelection.
                    // OLEACC instead throws an ArgumentException.
                    // Since Whidbey API's should not throw an exception in places where Everett API's did not, we catch
                    // the ArgumentException and fail silently.
                }
            }
        }

    }
}
