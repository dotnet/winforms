// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    /// <summary>
    ///  ToolStripItem that can host Controls.
    /// </summary>
    public partial class ToolStripControlHost : ToolStripItem
    {
        private Control _control;
        private int _suspendSyncSizeCount;
        private ContentAlignment _controlAlign = ContentAlignment.MiddleCenter;
        private bool _inSetVisibleCore;

        internal static readonly object s_gotFocusEvent = new object();
        internal static readonly object s_lostFocusEvent = new object();
        internal static readonly object s_keyDownEvent = new object();
        internal static readonly object s_keyPressEvent = new object();
        internal static readonly object s_keyUpEent = new object();
        internal static readonly object s_enterEvent = new object();
        internal static readonly object s_leaveEvent = new object();
        internal static readonly object s_validatedEvent = new object();
        internal static readonly object s_validatingEvent = new object();

        /// <summary>
        ///  Constructs a ToolStripControlHost
        /// </summary>
        public ToolStripControlHost(Control c)
        {
            _control = c ?? throw new ArgumentNullException(nameof(c), SR.ControlCannotBeNull);
            SyncControlParent();
            c.Visible = true;
            SetBounds(c.Bounds);

            // now that we have a control set in, update the bounds.
            Rectangle bounds = Bounds;
            CommonProperties.UpdateSpecifiedBounds(c, bounds.X, bounds.Y, bounds.Width, bounds.Height);

            c.ToolStripControlHost = this;

            OnSubscribeControlEvents(c);
        }

        public ToolStripControlHost(Control c, string name) : this(c)
        {
            Name = name;
        }

        public override Color BackColor
        {
            get => Control.BackColor;
            set => Control.BackColor = value;
        }

        /// <summary>
        ///  Gets or sets the image that is displayed on a <see cref='Label'/>.
        /// </summary>
        [Localizable(true)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripItemImageDescr))]
        [DefaultValue(null)]
        public override Image BackgroundImage
        {
            get => Control.BackgroundImage;
            set => Control.BackgroundImage = value;
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(ImageLayout.Tile)]
        [Localizable(true)]
        [SRDescription(nameof(SR.ControlBackgroundImageLayoutDescr))]
        public override ImageLayout BackgroundImageLayout
        {
            get => Control.BackgroundImageLayout;
            set => Control.BackgroundImageLayout = value;
        }

        /// <summary>
        ///  Overriden to return value from Control.CanSelect.
        /// </summary>
        public override bool CanSelect
        {
            get
            {
                if (_control != null)
                {
                    return (DesignMode || Control.CanSelect);
                }

                return false;
            }
        }

        [SRCategory(nameof(SR.CatFocus))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.ControlCausesValidationDescr))]
        public bool CausesValidation
        {
            get => Control.CausesValidation;
            set => Control.CausesValidation = value;
        }

        [DefaultValue(ContentAlignment.MiddleCenter)]
        [Browsable(false)]
        public ContentAlignment ControlAlign
        {
            get => _controlAlign;
            set
            {
                if (!WindowsFormsUtils.EnumValidator.IsValidContentAlignment(value))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ContentAlignment));
                }

                if (_controlAlign != value)
                {
                    _controlAlign = value;
                    OnBoundsChanged();
                }
            }
        }

        /// <summary>
        ///  The control that this item is hosting.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Control Control => _control;

        internal AccessibleObject ControlAccessibilityObject => Control?.AccessibilityObject;

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected override Size DefaultSize
        {
            get
            {
                if (Control != null)
                {
                    // When you create the control - it sets up its size as its default size.
                    // Since the property is protected we dont know for sure, but this is a pretty good guess.
                    return Control.Size;
                }

                return base.DefaultSize;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ToolStripItemDisplayStyle DisplayStyle
        {
            get => base.DisplayStyle;
            set => base.DisplayStyle = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler DisplayStyleChanged
        {
            add => Events.AddHandler(s_displayStyleChangedEvent, value);
            remove => Events.RemoveHandler(s_displayStyleChangedEvent, value);
        }

        /// <summary>
        ///  For control hosts, this property has no effect
        ///  as they get their own clicks.  Use ControlStyles.StandardClick
        ///  instead.
        /// </summary>
        [DefaultValue(false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new bool DoubleClickEnabled
        {
            get => base.DoubleClickEnabled;
            set => base.DoubleClickEnabled = value;
        }

        public override Font Font
        {
            get => Control.Font;
            set => Control.Font = value;
        }

        public override bool Enabled
        {
            get => Control.Enabled;
            set => Control.Enabled = value;
        }

        [SRCategory(nameof(SR.CatFocus))]
        [SRDescription(nameof(SR.ControlOnEnterDescr))]
        public event EventHandler Enter
        {
            add => Events.AddHandler(s_enterEvent, value);
            remove => Events.RemoveHandler(s_enterEvent, value);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public virtual bool Focused => Control.Focused;

        public override Color ForeColor
        {
            get => Control.ForeColor;
            set => Control.ForeColor = value;
        }

        [SRCategory(nameof(SR.CatFocus))]
        [SRDescription(nameof(SR.ToolStripItemOnGotFocusDescr))]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler GotFocus
        {
            add => Events.AddHandler(s_gotFocusEvent, value);
            remove => Events.RemoveHandler(s_gotFocusEvent, value);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image Image
        {
            get => base.Image;
            set => base.Image = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ToolStripItemImageScaling ImageScaling
        {
            get => base.ImageScaling;
            set => base.ImageScaling = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Color ImageTransparentColor
        {
            get => base.ImageTransparentColor;
            set => base.ImageTransparentColor = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ContentAlignment ImageAlign
        {
            get => base.ImageAlign;
            set => base.ImageAlign = value;
        }

        [SRCategory(nameof(SR.CatFocus))]
        [SRDescription(nameof(SR.ControlOnLeaveDescr))]
        public event EventHandler Leave
        {
            add => Events.AddHandler(s_leaveEvent, value);
            remove => Events.RemoveHandler(s_leaveEvent, value);
        }

        /// <summary>
        ///  Occurs when the control loses focus.
        /// </summary>
        [SRCategory(nameof(SR.CatFocus))]
        [SRDescription(nameof(SR.ToolStripItemOnLostFocusDescr))]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler LostFocus
        {
            add => Events.AddHandler(s_lostFocusEvent, value);
            remove => Events.RemoveHandler(s_lostFocusEvent, value);
        }

        /// <summary>
        ///  Occurs when a key is pressed down while the control has focus.
        /// </summary>
        [SRCategory(nameof(SR.CatKey))]
        [SRDescription(nameof(SR.ControlOnKeyDownDescr))]
        public event KeyEventHandler KeyDown
        {
            add => Events.AddHandler(s_keyDownEvent, value);
            remove => Events.RemoveHandler(s_keyDownEvent, value);
        }

        /// <summary>
        ///  Occurs when a key is pressed while the control has focus.
        /// </summary>
        [SRCategory(nameof(SR.CatKey))]
        [SRDescription(nameof(SR.ControlOnKeyPressDescr))]
        public event KeyPressEventHandler KeyPress
        {
            add => Events.AddHandler(s_keyPressEvent, value);
            remove => Events.RemoveHandler(s_keyPressEvent, value);
        }

        /// <summary>
        ///  Occurs when a key is released while the control has focus.
        /// </summary>
        [SRCategory(nameof(SR.CatKey))]
        [SRDescription(nameof(SR.ControlOnKeyUpDescr))]
        public event KeyEventHandler KeyUp
        {
            add => Events.AddHandler(s_keyUpEent, value);
            remove => Events.RemoveHandler(s_keyUpEent, value);
        }

        /// <summary>
        ///  This is used for international applications where the language
        ///  is written from RightToLeft. When this property is true,
        ///  control placement and text will be from right to left.
        /// </summary>
        public override RightToLeft RightToLeft
        {
            get
            {
                if (_control != null)
                {
                    return _control.RightToLeft;
                }

                return base.RightToLeft;
            }
            set
            {
                if (_control != null)
                {
                    _control.RightToLeft = value;
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool RightToLeftAutoMirrorImage
        {
            get => base.RightToLeftAutoMirrorImage;
            set => base.RightToLeftAutoMirrorImage = value;
        }

        public override bool Selected => Control != null && Control.Focused;

        public override Size Size
        {
            get => base.Size;
            set
            {
                Rectangle specifiedBounds = Rectangle.Empty;
                if (_control != null)
                {
                    // we dont normally update the specified bounds, but if someone explicitly sets
                    // the size we should.
                    specifiedBounds = _control.Bounds;
                    specifiedBounds.Size = value;
                    CommonProperties.UpdateSpecifiedBounds(_control, specifiedBounds.X, specifiedBounds.Y, specifiedBounds.Width, specifiedBounds.Height);
                }

                base.Size = value;

                if (_control != null)
                {
                    // checking again in case the control has adjusted the size.
                    Rectangle bounds = _control.Bounds;
                    if (bounds != specifiedBounds)
                    {
                        CommonProperties.UpdateSpecifiedBounds(_control, bounds.X, bounds.Y, bounds.Width, bounds.Height);
                    }
                }
            }
        }

        /// <summary>
        ///  Overriden to set the Site for the control hosted. This is set at DesignTime when the component is added to the Container.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public override ISite Site
        {
            get => base.Site;
            set
            {
                base.Site = value;
                if (value != null)
                {
                    Control.Site = new StubSite(Control, this);
                }
                else
                {
                    Control.Site = null;
                }
            }
        }

        /// <summary>
        ///  Overriden to modify hosted control's text.
        /// </summary>
        [DefaultValue("")]
        public override string Text
        {
            get => Control.Text;
            set => Control.Text = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new ContentAlignment TextAlign
        {
            get => base.TextAlign;
            set => base.TextAlign = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DefaultValue(ToolStripTextDirection.Horizontal)]
        public override ToolStripTextDirection TextDirection
        {
            get => base.TextDirection;
            set => base.TextDirection = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new TextImageRelation TextImageRelation
        {
            get => base.TextImageRelation;
            set => base.TextImageRelation = value;
        }

        [SRCategory(nameof(SR.CatFocus))]
        [SRDescription(nameof(SR.ControlOnValidatingDescr))]
        public event CancelEventHandler Validating
        {
            add => Events.AddHandler(s_validatingEvent, value);
            remove => Events.RemoveHandler(s_validatingEvent, value);
        }

        [SRCategory(nameof(SR.CatFocus))]
        [SRDescription(nameof(SR.ControlOnValidatedDescr))]
        public event EventHandler Validated
        {
            add => Events.AddHandler(s_validatedEvent, value);
            remove => Events.RemoveHandler(s_validatedEvent, value);
        }

        /// <summary>
        ///  Cleans up and destroys the hosted control.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            // Call base first so other things stop trying to talk to the control.  This will
            // unparent the host item which will cause a SyncControlParent, so the control
            // will be correctly unparented before being disposed.
            base.Dispose(disposing);

            if (disposing && Control != null)
            {
                OnUnsubscribeControlEvents(Control);

                // we only call control.Dispose if we are NOT being disposed in the finalizer.
                Control.Dispose();
                _control = null;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void Focus() => Control.Focus();

        public override Size GetPreferredSize(Size constrainingSize)
        {
            if (_control != null)
            {
                return Control.GetPreferredSize(constrainingSize - Padding.Size) + Padding.Size;
            }

            return base.GetPreferredSize(constrainingSize);
        }

        ///  Handle* wrappers:
        ///  We sync the event from the hosted control and call resurface it on ToolStripItem.
        private void HandleClick(object sender, EventArgs e) => OnClick(e);

        private void HandleBackColorChanged(object sender, EventArgs e) => OnBackColorChanged(e);

        private void HandleDoubleClick(object sender, EventArgs e) => OnDoubleClick(e);

        private void HandleDragDrop(object sender, DragEventArgs e) => OnDragDrop(e);

        private void HandleDragEnter(object sender, DragEventArgs e) => OnDragEnter(e);

        private void HandleDragLeave(object sender, EventArgs e) => OnDragLeave(e);

        private void HandleDragOver(object sender, DragEventArgs e) => OnDragOver(e);

        private void HandleEnter(object sender, EventArgs e) => OnEnter(e);

        private void HandleEnabledChanged(object sender, EventArgs e) => OnEnabledChanged(e);

        private void HandleForeColorChanged(object sender, EventArgs e) => OnForeColorChanged(e);

        private void HandleGiveFeedback(object sender, GiveFeedbackEventArgs e) => OnGiveFeedback(e);

        private void HandleGotFocus(object sender, EventArgs e) => OnGotFocus(e);

        private void HandleLocationChanged(object sender, EventArgs e) => OnLocationChanged(e);

        private void HandleLostFocus(object sender, EventArgs e) => OnLostFocus(e);

        private void HandleKeyDown(object sender, KeyEventArgs e) => OnKeyDown(e);

        private void HandleKeyPress(object sender, KeyPressEventArgs e) => OnKeyPress(e);

        private void HandleKeyUp(object sender, KeyEventArgs e) => OnKeyUp(e);

        private void HandleLeave(object sender, EventArgs e) => OnLeave(e);

        private void HandleMouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(e);
            RaiseMouseEvent(ToolStripItem.s_mouseDownEvent, e);
        }

        private void HandleMouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter(e);
            RaiseEvent(ToolStripItem.s_mouseEnterEvent, e);
        }

        private void HandleMouseLeave(object sender, EventArgs e)
        {
            OnMouseLeave(e);
            RaiseEvent(ToolStripItem.s_mouseLeaveEvent, e);
        }

        private void HandleMouseHover(object sender, EventArgs e)
        {
            OnMouseHover(e);
            RaiseEvent(ToolStripItem.s_mouseHoverEvent, e);
        }

        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
            OnMouseMove(e);
            RaiseMouseEvent(ToolStripItem.s_mouseMoveEvent, e);
        }

        private void HandleMouseUp(object sender, MouseEventArgs e)
        {
            OnMouseUp(e);
            RaiseMouseEvent(ToolStripItem.s_mouseUpEvent, e);
        }

        private void HandlePaint(object sender, PaintEventArgs e)
        {
            OnPaint(e);
            RaisePaintEvent(ToolStripItem.s_paintEvent, e);
        }

        private void HandleQueryAccessibilityHelp(object sender, QueryAccessibilityHelpEventArgs e)
        {
            ((QueryAccessibilityHelpEventHandler)Events[ToolStripItem.s_queryAccessibilityHelpEvent])?.Invoke(this, e);
        }

        private void HandleQueryContinueDrag(object sender, QueryContinueDragEventArgs e) => OnQueryContinueDrag(e);

        private void HandleRightToLeftChanged(object sender, EventArgs e) => OnRightToLeftChanged(e);

        private void HandleResize(object sender, EventArgs e)
        {
            if (_suspendSyncSizeCount == 0)
            {
                OnHostedControlResize(e);
            }
        }

        private void HandleTextChanged(object sender, EventArgs e) => OnTextChanged(e);

        private void HandleControlVisibleChanged(object sender, EventArgs e)
        {
            // check the STATE_VISIBLE flag rather than using Control.Visible.
            // if we check while it's unparented it will return visible false.
            // the easiest way to do this is to use ParticipatesInLayout.
            bool controlVisibleStateFlag = ((IArrangedElement)Control).ParticipatesInLayout;
            bool itemVisibleStateFlag = ((IArrangedElement)(this)).ParticipatesInLayout;

            if (itemVisibleStateFlag != controlVisibleStateFlag)
            {
                Visible = Control.Visible;
                // this should fire the OnVisibleChanged and raise events appropriately.
            };
        }

        private void HandleValidating(object sender, CancelEventArgs e) => OnValidating(e);

        private void HandleValidated(object sender, EventArgs e) => OnValidated(e);

        internal override void OnAccessibleDescriptionChanged(EventArgs e)
        {
            Control.AccessibleDescription = AccessibleDescription;
        }

        internal override void OnAccessibleNameChanged(EventArgs e)
        {
            Control.AccessibleName = AccessibleName;
        }

        internal override void OnAccessibleDefaultActionDescriptionChanged(EventArgs e)
        {
            Control.AccessibleDefaultActionDescription = AccessibleDefaultActionDescription;
        }

        internal override void OnAccessibleRoleChanged(EventArgs e)
        {
            Control.AccessibleRole = AccessibleRole;
        }

        protected virtual void OnEnter(EventArgs e) => RaiseEvent(s_enterEvent, e);

        /// <summary>
        ///  called when the control has lost focus
        /// </summary>
        protected virtual void OnGotFocus(EventArgs e) => RaiseEvent(s_gotFocusEvent, e);

        protected virtual void OnLeave(EventArgs e) => RaiseEvent(s_leaveEvent, e);

        /// <summary>
        ///  called when the control has lost focus
        /// </summary>
        protected virtual void OnLostFocus(EventArgs e) => RaiseEvent(s_lostFocusEvent, e);

        protected virtual void OnKeyDown(KeyEventArgs e) => RaiseKeyEvent(s_keyDownEvent, e);

        protected virtual void OnKeyPress(KeyPressEventArgs e) => RaiseKeyPressEvent(s_keyPressEvent, e);

        protected virtual void OnKeyUp(KeyEventArgs e) => RaiseKeyEvent(s_keyUpEent, e);

        /// <summary>
        ///  Called when the items bounds are changed.  Here, we update the Control's bounds.
        /// </summary>
        protected override void OnBoundsChanged()
        {
            if (!(_control is IArrangedElement element))
            {
                return;
            }

            SuspendSizeSync();

            Size size = LayoutUtils.DeflateRect(Bounds, Padding).Size;
            Rectangle bounds = LayoutUtils.Align(size, Bounds, ControlAlign);

            // use BoundsSpecified.None so we dont deal w/specified bounds - this way we can tell what someone has set the size to.
            element.SetBounds(bounds, BoundsSpecified.None);

            // sometimes a control can ignore the size passed in, use the adjustment
            // to re-align.
            if (bounds != _control.Bounds)
            {
                bounds = LayoutUtils.Align(_control.Size, Bounds, ControlAlign);
                element.SetBounds(bounds, BoundsSpecified.None);
            }

            ResumeSizeSync();
        }

        /// <summary>
        ///  Called when the control fires its Paint event.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            // do nothing....
        }

        protected internal override void OnLayout(LayoutEventArgs e)
        {
            // do nothing... called via the controls collection
        }

        /// <summary>
        ///  Called when the item's parent has been changed.
        /// </summary>
        protected override void OnParentChanged(ToolStrip oldParent, ToolStrip newParent)
        {
            if (oldParent != null && Owner is null && newParent is null && Control != null)
            {
                // if we've really been removed from the item collection,
                // politely remove ourselves from the control collection
                ReadOnlyControlCollection oldControlCollection
                                = GetControlCollection(Control.ParentInternal as ToolStrip);
                if (oldControlCollection != null)
                {
                    oldControlCollection.RemoveInternal(Control);
                }
            }
            else
            {
                SyncControlParent();
            }

            base.OnParentChanged(oldParent, newParent);
        }

        /// <summary>
        ///  The events from the hosted control are subscribed here.
        ///  Override to add/prevent syncing of control events.
        ///  NOTE: if you override and hook up events here, you should unhook in OnUnsubscribeControlEvents.
        /// </summary>
        protected virtual void OnSubscribeControlEvents(Control control)
        {
            if (control != null)
            {
                // Please keep this alphabetized and in sync with Unsubscribe
                control.Click += new EventHandler(HandleClick);
                control.BackColorChanged += new EventHandler(HandleBackColorChanged);
                control.DoubleClick += new EventHandler(HandleDoubleClick);
                control.DragDrop += new DragEventHandler(HandleDragDrop);
                control.DragEnter += new DragEventHandler(HandleDragEnter);
                control.DragLeave += new EventHandler(HandleDragLeave);
                control.DragOver += new DragEventHandler(HandleDragOver);
                control.Enter += new EventHandler(HandleEnter);
                control.EnabledChanged += new EventHandler(HandleEnabledChanged);
                control.ForeColorChanged += new EventHandler(HandleForeColorChanged);
                control.GiveFeedback += new GiveFeedbackEventHandler(HandleGiveFeedback);
                control.GotFocus += new EventHandler(HandleGotFocus);
                control.Leave += new EventHandler(HandleLeave);
                control.LocationChanged += new EventHandler(HandleLocationChanged);
                control.LostFocus += new EventHandler(HandleLostFocus);
                control.KeyDown += new KeyEventHandler(HandleKeyDown);
                control.KeyPress += new KeyPressEventHandler(HandleKeyPress);
                control.KeyUp += new KeyEventHandler(HandleKeyUp);
                control.MouseDown += new MouseEventHandler(HandleMouseDown);
                control.MouseEnter += new EventHandler(HandleMouseEnter);
                control.MouseHover += new EventHandler(HandleMouseHover);
                control.MouseLeave += new EventHandler(HandleMouseLeave);
                control.MouseMove += new MouseEventHandler(HandleMouseMove);
                control.MouseUp += new MouseEventHandler(HandleMouseUp);
                control.Paint += new PaintEventHandler(HandlePaint);
                control.QueryAccessibilityHelp += new QueryAccessibilityHelpEventHandler(HandleQueryAccessibilityHelp);
                control.QueryContinueDrag += new QueryContinueDragEventHandler(HandleQueryContinueDrag);
                control.Resize += new EventHandler(HandleResize);
                control.RightToLeftChanged += new EventHandler(HandleRightToLeftChanged);
                control.TextChanged += new EventHandler(HandleTextChanged);
                control.VisibleChanged += new EventHandler(HandleControlVisibleChanged);
                control.Validating += new CancelEventHandler(HandleValidating);
                control.Validated += new EventHandler(HandleValidated);
            }
        }

        /// <summary>
        ///  The events from the hosted control are unsubscribed here.
        ///  Override to unhook events subscribed in OnSubscribeControlEvents.
        /// </summary>
        protected virtual void OnUnsubscribeControlEvents(Control control)
        {
            if (control != null)
            {
                // Please keep this alphabetized and in sync with Subscribe
                control.Click -= new EventHandler(HandleClick);
                control.BackColorChanged -= new EventHandler(HandleBackColorChanged);
                control.DoubleClick -= new EventHandler(HandleDoubleClick);
                control.DragDrop -= new DragEventHandler(HandleDragDrop);
                control.DragEnter -= new DragEventHandler(HandleDragEnter);
                control.DragLeave -= new EventHandler(HandleDragLeave);
                control.DragOver -= new DragEventHandler(HandleDragOver);
                control.Enter -= new EventHandler(HandleEnter);
                control.EnabledChanged -= new EventHandler(HandleEnabledChanged);
                control.ForeColorChanged -= new EventHandler(HandleForeColorChanged);
                control.GiveFeedback -= new GiveFeedbackEventHandler(HandleGiveFeedback);
                control.GotFocus -= new EventHandler(HandleGotFocus);
                control.Leave -= new EventHandler(HandleLeave);
                control.LocationChanged -= new EventHandler(HandleLocationChanged);
                control.LostFocus -= new EventHandler(HandleLostFocus);
                control.KeyDown -= new KeyEventHandler(HandleKeyDown);
                control.KeyPress -= new KeyPressEventHandler(HandleKeyPress);
                control.KeyUp -= new KeyEventHandler(HandleKeyUp);
                control.MouseDown -= new MouseEventHandler(HandleMouseDown);
                control.MouseEnter -= new EventHandler(HandleMouseEnter);
                control.MouseHover -= new EventHandler(HandleMouseHover);
                control.MouseLeave -= new EventHandler(HandleMouseLeave);
                control.MouseMove -= new MouseEventHandler(HandleMouseMove);
                control.MouseUp -= new MouseEventHandler(HandleMouseUp);
                control.Paint -= new PaintEventHandler(HandlePaint);
                control.QueryAccessibilityHelp -= new QueryAccessibilityHelpEventHandler(HandleQueryAccessibilityHelp);
                control.QueryContinueDrag -= new QueryContinueDragEventHandler(HandleQueryContinueDrag);
                control.Resize -= new EventHandler(HandleResize);
                control.RightToLeftChanged -= new EventHandler(HandleRightToLeftChanged);
                control.TextChanged -= new EventHandler(HandleTextChanged);
                control.VisibleChanged -= new EventHandler(HandleControlVisibleChanged);
                control.Validating -= new CancelEventHandler(HandleValidating);
                control.Validated -= new EventHandler(HandleValidated);
            }
        }

        protected virtual void OnValidating(CancelEventArgs e) => RaiseCancelEvent(s_validatingEvent, e);

        protected virtual void OnValidated(EventArgs e) => RaiseEvent(s_validatedEvent, e);

        private static ReadOnlyControlCollection GetControlCollection(ToolStrip toolStrip)
            => (ReadOnlyControlCollection)toolStrip?.Controls;

        /// <remarks>
        ///  Ensures the hosted Control is parented to the ToolStrip hosting this ToolStripItem.
        /// </remarks>
        private void SyncControlParent()
        {
            ReadOnlyControlCollection newControls = GetControlCollection(ParentInternal);
            if (newControls != null)
            {
                newControls.AddInternal(Control);
            }
        }

        protected virtual void OnHostedControlResize(EventArgs e)
        {
            // support for syncing the wrapper when the control size has changed
            Size = Control.Size;
        }

        protected internal override bool ProcessCmdKey(ref Message m, Keys keyData) => false;

        protected internal override bool ProcessMnemonic(char charCode)
        {
            if (_control != null)
            {
                return _control.ProcessMnemonic(charCode);
            }

            return base.ProcessMnemonic(charCode);
        }

        protected internal override bool ProcessDialogKey(Keys keyData) => false;

        protected override void SetVisibleCore(bool visible)
        {
            // This is needed, because if you try and set set visible to true before the parent is visible,
            // we will get called back into here, and set it back to false, since the parent is not visible.
            if (_inSetVisibleCore)
            {
                return;
            }

            _inSetVisibleCore = true;
            Control.SuspendLayout();
            try
            {
                Control.Visible = visible;
            }
            finally
            {
                Control.ResumeLayout(false);
                // this will go ahead and perform layout.
                base.SetVisibleCore(visible);
                _inSetVisibleCore = false;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void ResetBackColor() => Control.ResetBackColor();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void ResetForeColor() => Control.ResetForeColor();

        private void SuspendSizeSync() => _suspendSyncSizeCount++;

        private void ResumeSizeSync() => _suspendSyncSizeCount--;

        internal override bool ShouldSerializeBackColor()
        {
            if (_control != null)
            {
                return _control.ShouldSerializeBackColor();
            }

            return base.ShouldSerializeBackColor();
        }

        internal override bool ShouldSerializeForeColor()
        {
            if (_control != null)
            {
                return _control.ShouldSerializeForeColor();
            }

            return base.ShouldSerializeForeColor();
        }

        internal override bool ShouldSerializeFont()
        {
            if (_control != null)
            {
                return _control.ShouldSerializeFont();
            }

            return base.ShouldSerializeFont();
        }

        internal override bool ShouldSerializeRightToLeft()
        {
            if (_control != null)
            {
                return _control.ShouldSerializeRightToLeft();
            }

            return base.ShouldSerializeRightToLeft();
        }

        internal override void OnKeyboardToolTipHook(ToolTip toolTip)
        {
            base.OnKeyboardToolTipHook(toolTip);

            KeyboardToolTipStateMachine.Instance.Hook(Control, toolTip);
        }

        internal override void OnKeyboardToolTipUnhook(ToolTip toolTip)
        {
            base.OnKeyboardToolTipUnhook(toolTip);

            KeyboardToolTipStateMachine.Instance.Unhook(Control, toolTip);
        }

        /// <summary>
        ///     Constructs the new instance of the accessibility object for this ToolStripControlHost ToolStrip item.
        /// </summary>
        /// <returns>
        ///     The new instance of the accessibility object for this ToolStripControlHost ToolStrip item
        /// </returns>
        protected override AccessibleObject CreateAccessibilityInstance()
            => new ToolStripControlHostAccessibleObject(this);
    }
}
