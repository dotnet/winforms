// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms.Design;
using System.Windows.Forms.Layout;
using static Interop;

namespace System.Windows.Forms
{
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    [DefaultEvent(nameof(ButtonClick))]
    public class ToolStripSplitButton : ToolStripDropDownItem
    {
        private ToolStripItem _defaultItem;
        private ToolStripSplitButtonButton _splitButtonButton;
        private Rectangle _dropDownButtonBounds = Rectangle.Empty;
        private ToolStripSplitButtonButtonLayout _splitButtonButtonLayout;
        private int _dropDownButtonWidth;
        private int _splitterWidth = 1;
        private Rectangle _splitterBounds = Rectangle.Empty;
        private byte _openMouseId;
        private long _lastClickTime;

        private const int DefaultDropDownWidth = 11;

        private static readonly object s_eventDefaultItemChanged = new object();
        private static readonly object s_eventButtonClick = new object();
        private static readonly object s_eventButtonDoubleClick = new object();

        private static bool s_isScalingInitialized;
        private static int s_scaledDropDownButtonWidth = DefaultDropDownWidth;

        public ToolStripSplitButton()
        {
            Initialize(); // all additional work should be done in Initialize
        }

        public ToolStripSplitButton(string text) : base(text, null, (EventHandler)null)
        {
            Initialize();
        }

        public ToolStripSplitButton(Image image) : base(null, image, (EventHandler)null)
        {
            Initialize();
        }

        public ToolStripSplitButton(string text, Image image) : base(text, image, (EventHandler)null)
        {
            Initialize();
        }

        public ToolStripSplitButton(string text, Image image, EventHandler onClick) : base(text, image, onClick)
        {
            Initialize();
        }

        public ToolStripSplitButton(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name)
        {
            Initialize();
        }
        public ToolStripSplitButton(string text, Image image, params ToolStripItem[] dropDownItems) : base(text, image, dropDownItems)
        {
            Initialize();
        }

        [DefaultValue(true)]
        public new bool AutoToolTip
        {
            get => base.AutoToolTip;
            set => base.AutoToolTip = value;
        }

        [Browsable(false)]
        public Rectangle ButtonBounds
        {
            get
            {
                //Rectangle bounds = SplitButtonButton.Bounds;
                //bounds.Offset(this.Bounds.Location);
                return SplitButtonButton.Bounds;
            }
        }

        [Browsable(false)]
        public bool ButtonPressed
        {
            get
            {
                return SplitButtonButton.Pressed;
            }
        }

        [Browsable(false)]
        public bool ButtonSelected
        {
            get
            {
                return SplitButtonButton.Selected || DropDownButtonPressed;
            }
        }

        /// <summary>
        ///  Occurs when the button portion of a split button is clicked.
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.ToolStripSplitButtonOnButtonClickDescr))]
        public event EventHandler ButtonClick
        {
            add => Events.AddHandler(s_eventButtonClick, value);
            remove => Events.RemoveHandler(s_eventButtonClick, value);
        }
        /// <summary>
        ///  Occurs when the utton portion of a split button  is double clicked.
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.ToolStripSplitButtonOnButtonDoubleClickDescr))]
        public event EventHandler ButtonDoubleClick
        {
            add => Events.AddHandler(s_eventButtonDoubleClick, value);
            remove => Events.RemoveHandler(s_eventButtonDoubleClick, value);
        }

        protected override bool DefaultAutoToolTip
        {
            get
            {
                return true;
            }
        }

        [DefaultValue(null)]
        [Browsable(false)]
        public ToolStripItem DefaultItem
        {
            get
            {
                return _defaultItem;
            }
            set
            {
                if (_defaultItem != value)
                {
                    OnDefaultItemChanged(EventArgs.Empty);
                    _defaultItem = value;
                }
            }
        }

        /// <summary>
        ///  Occurs when the default item has changed
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.ToolStripSplitButtonOnDefaultItemChangedDescr))]
        public event EventHandler DefaultItemChanged
        {
            add => Events.AddHandler(s_eventDefaultItemChanged, value);
            remove => Events.RemoveHandler(s_eventDefaultItemChanged, value);
        }

        /// <summary>
        ///  specifies the default behavior of these items on ToolStripDropDowns when clicked.
        /// </summary>
        internal protected override bool DismissWhenClicked
        {
            get
            {
                return DropDown.Visible != true;
            }
        }

        internal override Rectangle DropDownButtonArea
        {
            get { return DropDownButtonBounds; }
        }

        /// <summary>
        ///  The bounds of the DropDown in ToolStrip coordinates.
        /// </summary>
        [Browsable(false)]
        public Rectangle DropDownButtonBounds
        {
            get
            {
                return _dropDownButtonBounds;
            }
        }

        [Browsable(false)]
        public bool DropDownButtonPressed
        {
            get
            {
                //
                return DropDown.Visible;
            }
        }

        [Browsable(false)]
        public bool DropDownButtonSelected
        {
            get
            {
                return Selected;
            }
        }

        [SRCategory(nameof(SR.CatLayout))]
        [SRDescription(nameof(SR.ToolStripSplitButtonDropDownButtonWidthDescr))]
        public int DropDownButtonWidth
        {
            get
            {
                return _dropDownButtonWidth;
            }
            set
            {
                if (value < 0)
                {
                    // throw if less than 0.
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(DropDownButtonWidth), value, 0));
                }

                if (_dropDownButtonWidth != value)
                {
                    _dropDownButtonWidth = value;
                    InvalidateSplitButtonLayout();
                    InvalidateItemLayout(PropertyNames.DropDownButtonWidth);
                }
            }
        }

        /// <summary>
        ///  This is here for serialization purposes.
        /// </summary>
        private int DefaultDropDownButtonWidth
        {
            get
            {
                // lets start off with a size roughly equivalent to a combobox dropdown
                if (!s_isScalingInitialized)
                {
                    if (DpiHelper.IsScalingRequired)
                    {
                        s_scaledDropDownButtonWidth = DpiHelper.LogicalToDeviceUnitsX(DefaultDropDownWidth);
                    }
                    s_isScalingInitialized = true;
                }

                return s_scaledDropDownButtonWidth;
            }
        }

        /// <summary>
        ///  Just used as a convenience to help manage layout
        /// </summary>
        private ToolStripSplitButtonButton SplitButtonButton
        {
            get
            {
                if (_splitButtonButton is null)
                {
                    _splitButtonButton = new ToolStripSplitButtonButton(this);
                }
                _splitButtonButton.Image = Image;
                _splitButtonButton.Text = Text;
                _splitButtonButton.BackColor = BackColor;
                _splitButtonButton.ForeColor = ForeColor;
                _splitButtonButton.Font = Font;
                _splitButtonButton.ImageAlign = ImageAlign;
                _splitButtonButton.TextAlign = TextAlign;
                _splitButtonButton.TextImageRelation = TextImageRelation;
                return _splitButtonButton;
            }
        }

        internal ToolStripItemInternalLayout SplitButtonButtonLayout
        {
            get
            {
                // For preferred size caching reasons, we need to keep our two
                // internal layouts (button, dropdown button) in sync.

                if (InternalLayout != null /*if layout is invalid - calls CreateInternalLayout - which resets splitButtonButtonLayout to null*/
                    && _splitButtonButtonLayout is null)
                {
                    _splitButtonButtonLayout = new ToolStripSplitButtonButtonLayout(this);
                }
                return _splitButtonButtonLayout;
            }
        }

        /// <summary>
        ///  the width of the separator between the default and drop down button
        /// </summary>
        [SRDescription(nameof(SR.ToolStripSplitButtonSplitterWidthDescr))]
        [SRCategory(nameof(SR.CatLayout))]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal int SplitterWidth
        {
            get
            {
                return _splitterWidth;
            }
            set
            {
                if (value < 0)
                {
                    _splitterWidth = 0;
                }
                else
                {
                    _splitterWidth = value;
                }
                InvalidateSplitButtonLayout();
            }
        }
        /// <summary>
        ///  the boundaries of the separator between the default and drop down button, exposed for custom
        ///  painting purposes.
        /// </summary>
        [Browsable(false)]
        public Rectangle SplitterBounds
        {
            get
            {
                return _splitterBounds;
            }
        }

        private void CalculateLayout()
        {
            // Figure out where the DropDown image goes.
            Rectangle dropDownButtonBounds = new Rectangle(Point.Empty, Size);
            Rectangle splitButtonButtonBounds = Rectangle.Empty;

            dropDownButtonBounds = new Rectangle(Point.Empty, new Size(Math.Min(Width, DropDownButtonWidth), Height));

            // Figure out the height and width of the selected item.
            int splitButtonButtonWidth = Math.Max(0, Width - dropDownButtonBounds.Width);
            int splitButtonButtonHeight = Math.Max(0, Height);

            splitButtonButtonBounds = new Rectangle(Point.Empty, new Size(splitButtonButtonWidth, splitButtonButtonHeight));

            // grow the selected item by one since we're overlapping the borders.
            splitButtonButtonBounds.Width -= _splitterWidth;

            if (RightToLeft == RightToLeft.No)
            {
                // the dropdown button goes on the right
                dropDownButtonBounds.Offset(splitButtonButtonBounds.Right + _splitterWidth, 0);
                _splitterBounds = new Rectangle(splitButtonButtonBounds.Right, splitButtonButtonBounds.Top, _splitterWidth, splitButtonButtonBounds.Height);
            }
            else
            {
                // the split button goes on the right.
                splitButtonButtonBounds.Offset(DropDownButtonWidth + _splitterWidth, 0);
                _splitterBounds = new Rectangle(dropDownButtonBounds.Right, dropDownButtonBounds.Top, _splitterWidth, dropDownButtonBounds.Height);
            }

            SplitButtonButton.SetBounds(splitButtonButtonBounds);
            SetDropDownButtonBounds(dropDownButtonBounds);
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new ToolStripSplitButtonUiaProvider(this);
        }

        protected override ToolStripDropDown CreateDefaultDropDown()
        {
            // AutoGenerate a ToolStrip DropDown - set the property so we hook events
            return new ToolStripDropDownMenu(this, /*isAutoGenerated=*/true);
        }

        private protected override ToolStripItemInternalLayout CreateInternalLayout()
        {
            // whenever the master layout is invalidated - invalidate the splitbuttonbutton layout.
            _splitButtonButtonLayout = null;
            return new ToolStripItemInternalLayout(this);
        }

        public override Size GetPreferredSize(Size constrainingSize)
        {
            Size preferredSize = SplitButtonButtonLayout.GetPreferredSize(constrainingSize);
            preferredSize.Width += DropDownButtonWidth + SplitterWidth + Padding.Horizontal;
            return preferredSize;
        }

        private void InvalidateSplitButtonLayout()
        {
            _splitButtonButtonLayout = null;
            CalculateLayout();
        }

        private void Initialize()
        {
            _dropDownButtonWidth = DefaultDropDownButtonWidth;
            SupportsSpaceKey = true;
        }

        protected internal override bool ProcessDialogKey(Keys keyData)
        {
            if (Enabled && (keyData == Keys.Enter || (SupportsSpaceKey && keyData == Keys.Space)))
            {
                PerformButtonClick();
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        protected internal override bool ProcessMnemonic(char charCode)
        {
            // checking IsMnemonic is not necessary - toolstrip does this for us
            PerformButtonClick();
            return true;
        }

        /// <summary>
        ///  called when the button portion of a split button is clicked
        ///  if there is a default item, this will route the click to the default item
        /// </summary>
        protected virtual void OnButtonClick(EventArgs e)
        {
            DefaultItem?.FireEvent(ToolStripItemEventType.Click);
            ((EventHandler)Events[s_eventButtonClick])?.Invoke(this, e);
        }

        /// <summary>
        ///  called when the button portion of a split button is double clicked
        ///  if there is a default item, this will route the double click to the default item
        /// </summary>
        public virtual void OnButtonDoubleClick(EventArgs e)
        {
            DefaultItem?.FireEvent(ToolStripItemEventType.DoubleClick);
            ((EventHandler)Events[s_eventButtonDoubleClick])?.Invoke(this, e);
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        /// </summary>
        protected virtual void OnDefaultItemChanged(EventArgs e)
        {
            InvalidateSplitButtonLayout();
            if (CanRaiseEvents)
            {
                if (Events[s_eventDefaultItemChanged] is EventHandler eh)
                {
                    eh(this, e);
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (DropDownButtonBounds.Contains(e.Location))
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (!DropDown.Visible)
                    {
                        Debug.Assert(ParentInternal != null, "Parent is null here, not going to get accurate ID");
                        _openMouseId = (ParentInternal is null) ? (byte)0 : ParentInternal.GetMouseId();
                        ShowDropDown(/*mousePress = */true);
                    }
                }
            }
            else
            {
                SplitButtonButton.Push(true);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }

            SplitButtonButton.Push(false);

            if (DropDownButtonBounds.Contains(e.Location))
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (DropDown.Visible)
                    {
                        Debug.Assert(ParentInternal != null, "Parent is null here, not going to get accurate ID");
                        byte closeMouseId = (ParentInternal is null) ? (byte)0 : ParentInternal.GetMouseId();
                        if (closeMouseId != _openMouseId)
                        {
                            _openMouseId = 0;  // reset the mouse id, we should never get this value from toolstrip.
                            ToolStripManager.ModalMenuFilter.CloseActiveDropDown(DropDown, ToolStripDropDownCloseReason.AppClicked);
                            Select();
                        }
                    }
                }
            }
            Point clickPoint = new Point(e.X, e.Y);
            if ((e.Button == MouseButtons.Left) && SplitButtonButton.Bounds.Contains(clickPoint))
            {
                bool shouldFireDoubleClick = false;
                if (DoubleClickEnabled)
                {
                    long newTime = DateTime.Now.Ticks;
                    long deltaTicks = newTime - _lastClickTime;
                    _lastClickTime = newTime;
                    // use >= for cases where the succession of click events is so fast it's not picked up by
                    // DateTime resolution.
                    Debug.Assert(deltaTicks >= 0, "why are deltaticks less than zero? thats some mighty fast clicking");
                    // if we've seen a mouse up less than the double click time ago, we should fire.
                    if (deltaTicks >= 0 && deltaTicks < DoubleClickTicks)
                    {
                        shouldFireDoubleClick = true;
                    }
                }
                if (shouldFireDoubleClick)
                {
                    OnButtonDoubleClick(EventArgs.Empty);
                    // If we actually fired DoubleClick - reset the lastClickTime.
                    _lastClickTime = 0;
                }
                else
                {
                    OnButtonClick(EventArgs.Empty);
                }
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            _openMouseId = 0;  // reset the mouse id, we should never get this value from toolstrip.
            SplitButtonButton.Push(false);
            base.OnMouseLeave(e);
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            InvalidateSplitButtonLayout();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            ToolStripRenderer renderer = Renderer;
            if (renderer != null)
            {
                InvalidateSplitButtonLayout();
                Graphics g = e.Graphics;

                renderer.DrawSplitButton(new ToolStripItemRenderEventArgs(g, this));

                if ((DisplayStyle & ToolStripItemDisplayStyle.Image) != ToolStripItemDisplayStyle.None)
                {
                    renderer.DrawItemImage(new ToolStripItemImageRenderEventArgs(g, this, SplitButtonButtonLayout.ImageRectangle));
                }

                if ((DisplayStyle & ToolStripItemDisplayStyle.Text) != ToolStripItemDisplayStyle.None)
                {
                    renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(g, this, SplitButtonButton.Text, SplitButtonButtonLayout.TextRectangle, ForeColor, Font, SplitButtonButtonLayout.TextFormat));
                }
            }
        }

        public void PerformButtonClick()
        {
            if (Enabled && Available)
            {
                PerformClick();
                OnButtonClick(EventArgs.Empty);
            }
        }

        /// <summary>
        ///  Resets the RightToLeft to be the default.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetDropDownButtonWidth()
        {
            DropDownButtonWidth = DefaultDropDownButtonWidth;
        }

        private void SetDropDownButtonBounds(Rectangle rect)
        {
            _dropDownButtonBounds = rect;
        }
        /// <summary>
        ///  Determines if the <see cref='ToolStripItem.Size'/> property needs to be persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeDropDownButtonWidth()
        {
            return (DropDownButtonWidth != DefaultDropDownButtonWidth);
        }

        /// <summary>
        ///  This class represents the item to the left of the dropdown [ A |v]  (e.g the "A")
        ///  It exists so that we can use our existing methods for text and image layout
        ///  and have a place to stick certain state information like pushed and selected
        ///  Note since this is NOT an actual item hosted on the ToolStrip - it wont get things
        ///  like MouseOver, wont be laid out by the ToolStrip, etc etc.  This is purely internal
        ///  convenience.
        /// </summary>
        private class ToolStripSplitButtonButton : ToolStripButton
        {
            private readonly ToolStripSplitButton _owner;

            public ToolStripSplitButtonButton(ToolStripSplitButton owner)
            {
                this._owner = owner;
            }

            public override bool Enabled
            {
                get
                {
                    return _owner.Enabled;
                }
                set
                {
                    // do nothing
                }
            }

            public override ToolStripItemDisplayStyle DisplayStyle
            {
                get
                {
                    return _owner.DisplayStyle;
                }
                set
                {
                    // do nothing
                }
            }

            public override Padding Padding
            {
                get
                {
                    return _owner.Padding;
                }
                set
                {
                    // do nothing
                }
            }

            public override ToolStripTextDirection TextDirection
            {
                get
                {
                    return _owner.TextDirection;
                }
            }

            public override Image Image
            {
                get
                {
                    if ((_owner.DisplayStyle & ToolStripItemDisplayStyle.Image) == ToolStripItemDisplayStyle.Image)
                    {
                        return _owner.Image;
                    }
                    else
                    {
                        return null;
                    }
                }
                set
                {
                    // do nothing
                }
            }

            public override bool Selected
            {
                get
                {
                    if (_owner != null)
                    {
                        return _owner.Selected;
                    }
                    return base.Selected;
                }
            }

            public override string Text
            {
                get
                {
                    if ((_owner.DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text)
                    {
                        return _owner.Text;
                    }
                    else
                    {
                        return null;
                    }
                }
                set
                {
                    // do nothing
                }
            }
        }

        /// <summary>
        ///  This class performs internal layout for the "split button button" portion of a split button.
        ///  Its main job is to make sure the inner button has the same parent as the split button, so
        ///  that layout can be performed using the correct graphics context.
        /// </summary>
        private class ToolStripSplitButtonButtonLayout : ToolStripItemInternalLayout
        {
            readonly ToolStripSplitButton _owner;

            public ToolStripSplitButtonButtonLayout(ToolStripSplitButton owner) : base(owner.SplitButtonButton)
            {
                this._owner = owner;
            }

            protected override ToolStripItem Owner
            {
                get { return _owner; }
            }

            protected override ToolStrip ParentInternal
            {
                get
                {
                    return _owner.ParentInternal;
                }
            }
            public override Rectangle ImageRectangle
            {
                get
                {
                    Rectangle imageRect = base.ImageRectangle;
                    // translate to ToolStripItem coordinates
                    imageRect.Offset(_owner.SplitButtonButton.Bounds.Location);
                    return imageRect;
                }
            }

            public override Rectangle TextRectangle
            {
                get
                {
                    Rectangle textRect = base.TextRectangle;
                    // translate to ToolStripItem coordinates
                    textRect.Offset(_owner.SplitButtonButton.Bounds.Location);
                    return textRect;
                }
            }
        }

        public class ToolStripSplitButtonAccessibleObject : ToolStripItemAccessibleObject
        {
            private readonly ToolStripSplitButton _owner;

            public ToolStripSplitButtonAccessibleObject(ToolStripSplitButton item) : base(item)
            {
                _owner = item;
            }

            public override void DoDefaultAction()
            {
                _owner.PerformButtonClick();
            }
        }

        internal class ToolStripSplitButtonExAccessibleObject : ToolStripSplitButtonAccessibleObject
        {
            private readonly ToolStripSplitButton _ownerItem;

            public ToolStripSplitButtonExAccessibleObject(ToolStripSplitButton item)
                : base(item)
            {
                _ownerItem = item;
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                if (propertyID == UiaCore.UIA.ControlTypePropertyId)
                {
                    return UiaCore.UIA.ButtonControlTypeId;
                }
                else
                {
                    return base.GetPropertyValue(propertyID);
                }
            }

            internal override bool IsIAccessibleExSupported()
            {
                if (_ownerItem != null)
                {
                    return true;
                }
                else
                {
                    return base.IsIAccessibleExSupported();
                }
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.ExpandCollapsePatternId && _ownerItem.HasDropDownItems)
                {
                    return true;
                }
                else
                {
                    return base.IsPatternSupported(patternId);
                }
            }

            internal override void Expand()
            {
                DoDefaultAction();
            }

            internal override void Collapse()
            {
                if (_ownerItem != null && _ownerItem.DropDown != null && _ownerItem.DropDown.Visible)
                {
                    _ownerItem.DropDown.Close();
                }
            }

            internal override UiaCore.ExpandCollapseState ExpandCollapseState
            {
                get
                {
                    return _ownerItem.DropDown.Visible ? UiaCore.ExpandCollapseState.Expanded : UiaCore.ExpandCollapseState.Collapsed;
                }
            }

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.FirstChild:
                        return DropDownItemsCount > 0 ? _ownerItem.DropDown.Items[0].AccessibilityObject : null;
                    case UiaCore.NavigateDirection.LastChild:
                        return DropDownItemsCount > 0 ? _ownerItem.DropDown.Items[_ownerItem.DropDown.Items.Count - 1].AccessibilityObject : null;
                }
                return base.FragmentNavigate(direction);
            }

            private int DropDownItemsCount
            {
                get
                {
                    // Do not expose child items when the drop-down is collapsed to prevent Narrator from announcing
                    // invisible menu items when Narrator is in item's mode (CAPSLOCK + Arrow Left/Right) or
                    // in scan mode (CAPSLOCK + Space)
                    if (ExpandCollapseState == UiaCore.ExpandCollapseState.Collapsed)
                    {
                        return 0;
                    }

                    return _ownerItem.DropDownItems.Count;
                }
            }
        }

        internal class ToolStripSplitButtonUiaProvider : ToolStripDropDownItemAccessibleObject
        {
            private readonly ToolStripSplitButtonExAccessibleObject _accessibleObject;

            public ToolStripSplitButtonUiaProvider(ToolStripSplitButton owner) : base(owner)
            {
                _accessibleObject = new ToolStripSplitButtonExAccessibleObject(owner);
            }

            public override void DoDefaultAction()
            {
                _accessibleObject.DoDefaultAction();
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                return _accessibleObject.GetPropertyValue(propertyID);
            }

            internal override bool IsIAccessibleExSupported() => true;

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                return _accessibleObject.IsPatternSupported(patternId);
            }

            internal override void Expand()
            {
                DoDefaultAction();
            }

            internal override void Collapse()
            {
                _accessibleObject.Collapse();
            }

            internal override UiaCore.ExpandCollapseState ExpandCollapseState
            {
                get
                {
                    return _accessibleObject.ExpandCollapseState;
                }
            }

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                return _accessibleObject.FragmentNavigate(direction);
            }
        }
    }
}
