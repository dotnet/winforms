// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Implements the basic
    ///  functionality required by an up-down control.
    /// </summary>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    Designer("System.Windows.Forms.Design.UpDownBaseDesigner, " + AssemblyRef.SystemDesign),
    ]
    public abstract class UpDownBase : ContainerControl
    {
        private const int DefaultWheelScrollLinesPerPage = 1;
        private const int DefaultButtonsWidth = 16;
        private const int DefaultControlWidth = 120;
        private const int ThemedBorderWidth = 1; // width of custom border we draw when themed
        private const BorderStyle DefaultBorderStyle = BorderStyle.Fixed3D;
        private static readonly bool DefaultInterceptArrowKeys = true;
        private const LeftRightAlignment DefaultUpDownAlign = LeftRightAlignment.Right;
        private const int DefaultTimerInterval = 500;

        ////////////////////////////////////////////////////////////////////////
        // Member variables
        //
        ////////////////////////////////////////////////////////////////////////
        // Child controls
        internal UpDownEdit upDownEdit; // See nested class at end of this file
        internal UpDownButtons upDownButtons; // See nested class at end of this file

        // Intercept arrow keys?
        private bool interceptArrowKeys = DefaultInterceptArrowKeys;

        // If true, the updown buttons will be drawn on the left-hand side of the control.
        private LeftRightAlignment upDownAlign = DefaultUpDownAlign;

        // userEdit is true when the text of the control has been changed,
        // and the internal value of the control has not yet been updated.
        // We do not always want to keep the internal value up-to-date,
        // hence this variable.
        private bool userEdit = false;

        /// <summary>
        ///  The current border for this edit control.
        /// </summary>
        private BorderStyle borderStyle = DefaultBorderStyle;

        // Mouse wheel movement
        private int wheelDelta = 0;

        // Indicates if the edit text is being changed
        private bool changingText = false;

        // Indicates whether we have doubleClicked
        private bool doubleClickFired = false;

        internal int defaultButtonsWidth = DefaultButtonsWidth;

        /// <summary>
        ///  Initializes a new instance of the <see cref='UpDownBase'/>
        ///  class.
        /// </summary>
        public UpDownBase()
        {
            if (DpiHelper.IsScalingRequired)
            {
                defaultButtonsWidth = LogicalToDeviceUnits(DefaultButtonsWidth);
            }

            upDownButtons = new UpDownButtons(this);
            upDownEdit = new UpDownEdit(this)
            {
                BorderStyle = BorderStyle.None,
                AutoSize = false
            };
            upDownEdit.KeyDown += new KeyEventHandler(OnTextBoxKeyDown);
            upDownEdit.KeyPress += new KeyPressEventHandler(OnTextBoxKeyPress);
            upDownEdit.TextChanged += new EventHandler(OnTextBoxTextChanged);
            upDownEdit.LostFocus += new EventHandler(OnTextBoxLostFocus);
            upDownEdit.Resize += new EventHandler(OnTextBoxResize);
            upDownButtons.TabStop = false;
            upDownButtons.Size = new Size(defaultButtonsWidth, PreferredHeight);
            upDownButtons.UpDown += new UpDownEventHandler(OnUpDown);

            Controls.AddRange(new Control[] { upDownButtons, upDownEdit });

            SetStyle(ControlStyles.Opaque | ControlStyles.FixedHeight | ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.StandardClick, false);
            SetStyle(ControlStyles.UseTextForAccessibility, false);
        }

        ////////////////////////////////////////////////////////////////////////
        // Properties
        //
        ////////////////////////////////////////////////////////////////////////
        // AutoScroll is not relevant to an UpDownBase
        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AutoScroll
        {
            get
            {
                return false;
            }
            set
            {
                // Don't allow AutoScroll to be set to anything
            }
        }

        // AutoScrollMargin is not relevant to an UpDownBase
        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        new public Size AutoScrollMargin
        {
            get
            {
                return base.AutoScrollMargin;
            }
            set
            {
                base.AutoScrollMargin = value;
            }
        }

        // AutoScrollMinSize is not relevant to an UpDownBase
        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        new public Size AutoScrollMinSize
        {
            get
            {
                return base.AutoScrollMinSize;
            }
            set
            {
                base.AutoScrollMinSize = value;
            }
        }

        /// <summary>
        ///  Override to re-expose AutoSize.
        /// </summary>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = value;
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnAutoSizeChangedDescr))]
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        new public event EventHandler AutoSizeChanged
        {
            add => base.AutoSizeChanged += value;
            remove => base.AutoSizeChanged -= value;
        }

        /// <summary>
        ///  Gets or sets the background color for the
        ///  text box portion of the up-down control.
        /// </summary>
        public override Color BackColor
        {
            get
            {
                return upDownEdit.BackColor;
            }
            set
            {
                base.BackColor = value; // Don't remove this or you will break serialization.
                upDownEdit.BackColor = value;
                Invalidate();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageChanged
        {
            add => base.BackgroundImageChanged += value;
            remove => base.BackgroundImageChanged -= value;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }
            set
            {
                base.BackgroundImageLayout = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageLayoutChanged
        {
            add => base.BackgroundImageLayoutChanged += value;
            remove => base.BackgroundImageLayoutChanged -= value;
        }

        /// <summary>
        ///  Gets or sets the border style for
        ///  the up-down control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(BorderStyle.Fixed3D),
        DispId(NativeMethods.ActiveX.DISPID_BORDERSTYLE),
        SRDescription(nameof(SR.UpDownBaseBorderStyleDescr))
        ]
        public BorderStyle BorderStyle
        {
            get
            {
                return borderStyle;
            }

            set
            {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)BorderStyle.None, (int)BorderStyle.Fixed3D))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(BorderStyle));
                }

                if (borderStyle != value)
                {
                    borderStyle = value;
                    RecreateHandle();
                }
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the text
        ///  property is being changed internally by its parent class.
        /// </summary>
        protected bool ChangingText
        {
            get
            {
                return changingText;
            }

            set
            {
                changingText = value;
            }
        }

        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return base.ContextMenuStrip;
            }
            set
            {
                base.ContextMenuStrip = value;
                upDownEdit.ContextMenuStrip = value;
            }
        }

        /// <summary>
        ///  Returns the parameters needed to create the handle. Inheriting classes
        ///  can override this to provide extra functionality. They should not,
        ///  however, forget to call base.getCreateParams() first to get the struct
        ///  filled up with the basic info.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                cp.Style &= (~NativeMethods.WS_BORDER);
                if (!Application.RenderWithVisualStyles)
                {
                    switch (borderStyle)
                    {
                        case BorderStyle.Fixed3D:
                            cp.ExStyle |= NativeMethods.WS_EX_CLIENTEDGE;
                            break;
                        case BorderStyle.FixedSingle:
                            cp.Style |= NativeMethods.WS_BORDER;
                            break;
                    }
                }
                return cp;
            }
        }

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(DefaultControlWidth, PreferredHeight);
            }
        }

        // DockPadding is not relevant to UpDownBase
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        new public DockPaddingEdges DockPadding
        {
            get
            {
                return base.DockPadding;
            }
        }

        /// <summary>
        ///  Returns true if this control has focus.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlFocusedDescr))
        ]
        public override bool Focused
        {
            get
            {
                return upDownEdit.Focused;
            }
        }

        /// <summary>
        ///  Indicates the foreground color for the control.
        /// </summary>
        public override Color ForeColor
        {
            get
            {
                return upDownEdit.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                upDownEdit.ForeColor = value;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether
        ///  the user can use the UP
        ///  ARROW and DOWN ARROW keys to select values.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.UpDownBaseInterceptArrowKeysDescr))
        ]
        public bool InterceptArrowKeys
        {
            get
            {
                return interceptArrowKeys;
            }

            set
            {
                interceptArrowKeys = value;
            }
        }

        public override Size MaximumSize
        {
            get { return base.MaximumSize; }
            set
            {
                base.MaximumSize = new Size(value.Width, 0);
            }
        }

        public override Size MinimumSize
        {
            get { return base.MinimumSize; }
            set
            {
                base.MinimumSize = new Size(value.Width, 0);
            }
        }

        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler MouseEnter
        {
            add => base.MouseEnter += value;
            remove => base.MouseEnter -= value;
        }

        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler MouseLeave
        {
            add => base.MouseLeave += value;
            remove => base.MouseLeave -= value;
        }

        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler MouseHover
        {
            add => base.MouseHover += value;
            remove => base.MouseHover -= value;
        }

        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event MouseEventHandler MouseMove
        {
            add => base.MouseMove += value;
            remove => base.MouseMove -= value;
        }

        /// <summary>
        ///  Gets the height of
        ///  the up-down control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.UpDownBasePreferredHeightDescr))
        ]
        public int PreferredHeight
        {
            get
            {

                int height = FontHeight;

                // Adjust for the border style
                if (borderStyle != BorderStyle.None)
                {
                    height += SystemInformation.BorderSize.Height * 4 + 3;
                }
                else
                {
                    height += 3;
                }

                return height;
            }
        }

        /// <summary>
        ///  Gets or sets
        ///  a
        ///  value
        ///  indicating whether the text may only be changed by the
        ///  use
        ///  of the up or down buttons.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.UpDownBaseReadOnlyDescr))
        ]
        public bool ReadOnly
        {
            get
            {
                return upDownEdit.ReadOnly;
            }

            set
            {
                upDownEdit.ReadOnly = value;
            }
        }

        /// <summary>
        ///  Gets or sets the text
        ///  displayed in the up-down control.
        /// </summary>
        [
        Localizable(true)
        ]
        public override string Text
        {
            get
            {
                return upDownEdit.Text;
            }

            set
            {
                upDownEdit.Text = value;
                // The text changed event will at this point be triggered.
                // After returning, the value of UserEdit will reflect
                // whether or not the current upDownEditbox text is in sync
                // with any internally stored values. If UserEdit is true,
                // we must validate the text the user typed or set.

                ChangingText = false;
                // Details: Usually, the code in the Text changed event handler
                // sets ChangingText back to false.
                // If the text hasn't actually changed though, the event handler
                // never fires. ChangingText should always be false on exit from
                // this property.

                if (UserEdit)
                {
                    ValidateEditText();
                }
            }
        }

        /// <summary>
        ///  Gets or
        ///  sets the alignment of the text in the up-down
        ///  control.
        /// </summary>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(HorizontalAlignment.Left),
        SRDescription(nameof(SR.UpDownBaseTextAlignDescr))
        ]
        public HorizontalAlignment TextAlign
        {
            get
            {
                return upDownEdit.TextAlign;
            }
            set
            {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)HorizontalAlignment.Left, (int)HorizontalAlignment.Center))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(HorizontalAlignment));
                }
                upDownEdit.TextAlign = value;
            }
        }

        internal TextBox TextBox
        {
            get
            {
                return upDownEdit;
            }
        }

        /// <summary>
        ///  Gets
        ///  or sets the
        ///  alignment
        ///  of the up and down buttons on the up-down control.
        /// </summary>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(LeftRightAlignment.Right),
        SRDescription(nameof(SR.UpDownBaseAlignmentDescr))
        ]
        public LeftRightAlignment UpDownAlign
        {
            get
            {
                return upDownAlign;
            }

            set
            {
                //valid values are 0x0 to 0x1
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)LeftRightAlignment.Left, (int)LeftRightAlignment.Right))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(LeftRightAlignment));
                }

                if (upDownAlign != value)
                {

                    upDownAlign = value;
                    PositionControls();
                    Invalidate();
                }
            }
        }

        internal UpDownButtons UpDownButtonsInternal
        {
            get
            {
                return upDownButtons;
            }
        }

        /// <summary>
        ///  Gets
        ///  or sets a value indicating whether a value has been entered by the
        ///  user.
        /// </summary>
        protected bool UserEdit
        {
            get
            {
                return userEdit;
            }

            set
            {
                userEdit = value;
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Methods
        //
        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  When overridden in a derived class, handles the pressing of the down button
        ///  on the up-down control.
        /// </summary>
        public abstract void DownButton();

        // GetPreferredSize and SetBoundsCore call this method to allow controls to self impose
        // constraints on their size.
        internal override Rectangle ApplyBoundsConstraints(int suggestedX, int suggestedY, int proposedWidth, int proposedHeight)
        {
            return base.ApplyBoundsConstraints(suggestedX, suggestedY, proposedWidth, PreferredHeight);
        }

        /// <summary>
        ///  Gets an accessible name.
        /// </summary>
        /// <param name="baseName">The base name.</param>
        /// <returns>The accessible name.</returns>
        internal string GetAccessibleName(string baseName)
        {
            if (baseName == null)
            {
                return SR.SpinnerAccessibleName;
            }

            return baseName;
        }

        /// <summary>
        ///  When overridden in a derived class, handles rescaling of any magic numbers used in control painting.
        ///  For UpDown controls, scale the width of the up/down buttons.
        ///  Must call the base class method to get the current DPI values. This method is invoked only when
        ///  Application opts-in into the Per-monitor V2 support, targets .NETFX 4.7 and has
        ///  EnableDpiChangedMessageHandling config switch turned on.
        /// </summary>
        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
        {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            defaultButtonsWidth = LogicalToDeviceUnits(DefaultButtonsWidth);
            upDownButtons.Width = defaultButtonsWidth;
        }

        /// <summary>
        ///  When overridden in a derived class, raises the Changed event.
        ///  event.
        /// </summary>
        protected virtual void OnChanged(object source, EventArgs e)
        {
        }

        /// <summary>
        ///  Initialize the updown. Adds the upDownEdit and updown buttons.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            PositionControls();
            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(UserPreferenceChanged);
        }

        /// <summary>
        ///  Tear down the updown.
        /// </summary>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(UserPreferenceChanged);
            base.OnHandleDestroyed(e);
        }

        /// <summary>
        ///  Handles painting the buttons on the control.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle editBounds = upDownEdit.Bounds;
            if (Application.RenderWithVisualStyles)
            {
                if (borderStyle != BorderStyle.None)
                {
                    Rectangle bounds = ClientRectangle;
                    Rectangle clipBounds = e.ClipRectangle;

                    //Draw a themed textbox-like border, which is what the spin control does
                    VisualStyleRenderer vsr = new VisualStyleRenderer(VisualStyleElement.TextBox.TextEdit.Normal);
                    int border = ThemedBorderWidth;
                    Rectangle clipLeft = new Rectangle(bounds.Left, bounds.Top, border, bounds.Height);
                    Rectangle clipTop = new Rectangle(bounds.Left, bounds.Top, bounds.Width, border);
                    Rectangle clipRight = new Rectangle(bounds.Right - border, bounds.Top, border, bounds.Height);
                    Rectangle clipBottom = new Rectangle(bounds.Left, bounds.Bottom - border, bounds.Width, border);
                    clipLeft.Intersect(clipBounds);
                    clipTop.Intersect(clipBounds);
                    clipRight.Intersect(clipBounds);
                    clipBottom.Intersect(clipBounds);
                    vsr.DrawBackground(e.Graphics, bounds, clipLeft, HandleInternal);
                    vsr.DrawBackground(e.Graphics, bounds, clipTop, HandleInternal);
                    vsr.DrawBackground(e.Graphics, bounds, clipRight, HandleInternal);
                    vsr.DrawBackground(e.Graphics, bounds, clipBottom, HandleInternal);
                    // Draw rectangle around edit control with background color
                    using (Pen pen = new Pen(BackColor))
                    {
                        Rectangle backRect = editBounds;
                        backRect.X--;
                        backRect.Y--;
                        backRect.Width++;
                        backRect.Height++;
                        e.Graphics.DrawRectangle(pen, backRect);
                    }
                }
            }
            else
            {
                // Draw rectangle around edit control with background color
                using (Pen pen = new Pen(BackColor, Enabled ? 2 : 1))
                {
                    Rectangle backRect = editBounds;
                    backRect.Inflate(1, 1);
                    if (!Enabled)
                    {
                        backRect.X--;
                        backRect.Y--;
                        backRect.Width++;
                        backRect.Height++;
                    }
                    e.Graphics.DrawRectangle(pen, backRect);
                }
            }
            if (!Enabled && BorderStyle != BorderStyle.None && !upDownEdit.ShouldSerializeBackColor())
            {
                //draws a grayed rectangled around the upDownEdit, since otherwise we will have a white
                //border around the upDownEdit, which is inconsistent with Windows' behavior
                //we only want to do this when BackColor is not serialized, since otherwise
                //we should display the backcolor instead of the usual grayed textbox.
                editBounds.Inflate(1, 1);
                ControlPaint.DrawBorder(e.Graphics, editBounds, SystemColors.Control, ButtonBorderStyle.Solid);
            }
        }

        /// <summary>
        ///  Raises the <see cref='Control.KeyDown'/>
        ///  event.
        /// </summary>
        protected virtual void OnTextBoxKeyDown(object source, KeyEventArgs e)
        {
            OnKeyDown(e);
            if (interceptArrowKeys)
            {

                // Intercept up arrow
                if (e.KeyData == Keys.Up)
                {
                    UpButton();
                    e.Handled = true;
                }

                // Intercept down arrow
                else if (e.KeyData == Keys.Down)
                {
                    DownButton();
                    e.Handled = true;
                }
            }

            // Perform text validation if ENTER is pressed
            //
            if (e.KeyCode == Keys.Return && UserEdit)
            {
                ValidateEditText();
            }
        }

        /// <summary>
        ///  Raises the <see cref='Control.KeyPress'/>
        ///  event.
        /// </summary>
        protected virtual void OnTextBoxKeyPress(object source, KeyPressEventArgs e)
        {
            OnKeyPress(e);

        }

        /// <summary>
        ///  Raises the <see cref='Control.LostFocus'/> event.
        /// </summary>
        protected virtual void OnTextBoxLostFocus(object source, EventArgs e)
        {
            if (UserEdit)
            {
                ValidateEditText();
            }
        }

        /// <summary>
        ///  Raises the <see cref='Control.Resize'/> event.
        /// </summary>
        protected virtual void OnTextBoxResize(object source, EventArgs e)
        {
            Height = PreferredHeight;
            PositionControls();
        }

        /// <summary>
        ///  Raises the TextBoxTextChanged event.
        ///  event.
        /// </summary>
        protected virtual void OnTextBoxTextChanged(object source, EventArgs e)
        {
            if (changingText)
            {
                Debug.Assert(UserEdit == false, "OnTextBoxTextChanged() - UserEdit == true");
                ChangingText = false;
            }
            else
            {
                UserEdit = true;
            }

            OnTextChanged(e);
            OnChanged(source, EventArgs.Empty);
        }

        /// <summary>
        ///  Called from the UpDownButtons member. Provided for derived controls to have a finer way to handle the event.
        /// </summary>
        internal virtual void OnStartTimer()
        {
        }

        internal virtual void OnStopTimer()
        {
        }

        /// <summary>
        ///  Raises the <see cref='Control.OnMouseDown'/> event.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Clicks == 2 && e.Button == MouseButtons.Left)
            {
                doubleClickFired = true;
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        ///  Raises the <see cref='Control.OnMouseUp'/> event.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            if (mevent.Button == MouseButtons.Left)
            {
                Point pt = PointToScreen(new Point(mevent.X, mevent.Y));
                if (UnsafeNativeMethods.WindowFromPoint(pt) == Handle && !ValidationCancelled)
                {
                    if (!doubleClickFired)
                    {
                        OnClick(mevent);
                        OnMouseClick(mevent);
                    }
                    else
                    {
                        doubleClickFired = false;
                        OnDoubleClick(mevent);
                        OnMouseDoubleClick(mevent);
                    }
                }
                doubleClickFired = false;
            }
            base.OnMouseUp(mevent);
        }

        /// <summary>
        ///  Raises the <see cref='Control.OnMouseWheel'/> event.
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e is HandledMouseEventArgs hme)
            {
                if (hme.Handled)
                {
                    return;
                }
                hme.Handled = true;
            }

            if ((ModifierKeys & (Keys.Shift | Keys.Alt)) != 0 || MouseButtons != MouseButtons.None)
            {
                return; // Do not scroll when Shift or Alt key is down, or when a mouse button is down.
            }

            int wheelScrollLines = SystemInformation.MouseWheelScrollLines;
            if (wheelScrollLines == 0)
            {
                return; // Do not scroll when the user system setting is 0 lines per notch
            }

            Debug.Assert(wheelDelta > -NativeMethods.WHEEL_DELTA, "wheelDelta is too smal");
            Debug.Assert(wheelDelta < NativeMethods.WHEEL_DELTA, "wheelDelta is too big");
            wheelDelta += e.Delta;

            float partialNotches;
            partialNotches = (float)wheelDelta / (float)NativeMethods.WHEEL_DELTA;

            if (wheelScrollLines == -1)
            {
                wheelScrollLines = DefaultWheelScrollLinesPerPage;
            }

            // Evaluate number of bands to scroll
            int scrollBands = (int)((float)wheelScrollLines * partialNotches);
            if (scrollBands != 0)
            {
                int absScrollBands;
                if (scrollBands > 0)
                {
                    absScrollBands = scrollBands;
                    while (absScrollBands > 0)
                    {
                        UpButton();
                        absScrollBands--;
                    }
                    wheelDelta -= (int)((float)scrollBands * ((float)NativeMethods.WHEEL_DELTA / (float)wheelScrollLines));
                }
                else
                {
                    absScrollBands = -scrollBands;
                    while (absScrollBands > 0)
                    {
                        DownButton();
                        absScrollBands--;
                    }
                    wheelDelta -= (int)((float)scrollBands * ((float)NativeMethods.WHEEL_DELTA / (float)wheelScrollLines));
                }
            }
        }

        /// <summary>
        ///  Handle the layout event. The size of the upDownEdit control, and the
        ///  position of the UpDown control must be modified.
        /// </summary>
        protected override void OnLayout(LayoutEventArgs e)
        {
            PositionControls();
            base.OnLayout(e);
        }

        /// <summary>
        ///  Raises the FontChanged event.
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            // Clear the font height cache
            FontHeight = -1;

            Height = PreferredHeight;
            PositionControls();

            base.OnFontChanged(e);
        }

        /// <summary>
        ///
        ///  Handles UpDown events, which are generated by clicking on
        ///  the updown buttons in the child updown control.
        /// </summary>
        private void OnUpDown(object source, UpDownEventArgs e)
        {
            // Modify the value
            if (e.ButtonID == (int)ButtonID.Up)
            {
                UpButton();
            }
            else if (e.ButtonID == (int)ButtonID.Down)
            {
                DownButton();
            }
        }

        /// <summary>
        ///  Calculates the size and position of the upDownEdit control and
        ///  the updown buttons.
        /// </summary>
        private void PositionControls()
        {
            Rectangle upDownEditBounds = Rectangle.Empty,
                      upDownButtonsBounds = Rectangle.Empty;

            Rectangle clientArea = new Rectangle(Point.Empty, ClientSize);
            int totalClientWidth = clientArea.Width;
            bool themed = Application.RenderWithVisualStyles;
            BorderStyle borderStyle = BorderStyle;

            // determine how much to squish in - Fixed3d and FixedSingle have 2PX border
            int borderWidth = (borderStyle == BorderStyle.None) ? 0 : 2;
            clientArea.Inflate(-borderWidth, -borderWidth);

            // Reposition and resize the upDownEdit control
            //
            if (upDownEdit != null)
            {
                upDownEditBounds = clientArea;
                upDownEditBounds.Size = new Size(clientArea.Width - defaultButtonsWidth, clientArea.Height);
            }

            // Reposition and resize the updown buttons
            //
            if (upDownButtons != null)
            {
                int borderFixup = (themed) ? 1 : 2;
                if (borderStyle == BorderStyle.None)
                {
                    borderFixup = 0;
                }
                upDownButtonsBounds = new Rectangle(/*x*/clientArea.Right - defaultButtonsWidth + borderFixup,
                                                    /*y*/clientArea.Top - borderFixup,
                                                    /*w*/defaultButtonsWidth,
                                                    /*h*/clientArea.Height + (borderFixup * 2));
            }

            // Right to left translation
            LeftRightAlignment updownAlign = UpDownAlign;
            updownAlign = RtlTranslateLeftRight(updownAlign);

            // left/right updown align translation
            if (updownAlign == LeftRightAlignment.Left)
            {
                // if the buttons are aligned to the left, swap position of text box/buttons
                upDownButtonsBounds.X = totalClientWidth - upDownButtonsBounds.Right;
                upDownEditBounds.X = totalClientWidth - upDownEditBounds.Right;
            }

            // apply locations
            if (upDownEdit != null)
            {
                upDownEdit.Bounds = upDownEditBounds;
            }
            if (upDownButtons != null)
            {
                upDownButtons.Bounds = upDownButtonsBounds;
                upDownButtons.Invalidate();
            }

        }

        /// <summary>
        ///  Selects a range of
        ///  text in the up-down control.
        /// </summary>
        public void Select(int start, int length)
        {
            upDownEdit.Select(start, length);
        }

        /// <summary>
        ///  Child controls run their
        /// </summary>
        private MouseEventArgs TranslateMouseEvent(Control child, MouseEventArgs e)
        {
            if (child != null && IsHandleCreated)
            {
                // same control as PointToClient or PointToScreen, just
                // with two specific controls in mind.
                var point = new Point(e.X, e.Y);
                UnsafeNativeMethods.MapWindowPoints(new HandleRef(child, child.Handle), new HandleRef(this, Handle), ref point, 1);
                return new MouseEventArgs(e.Button, e.Clicks, point.X, point.Y, e.Delta);
            }
            return e;
        }

        /// <summary>
        ///  When overridden in a derived class, handles the pressing of the up button on the up-down control.
        /// </summary>
        public abstract void UpButton();

        /// <summary>
        ///  When overridden
        ///  in a derived class, updates the text displayed in the up-down control.
        /// </summary>
        protected abstract void UpdateEditText();

        private void UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs pref)
        {
            if (pref.Category == UserPreferenceCategory.Locale)
            {
                UpdateEditText();
            }
        }

        /// <summary>
        ///  When overridden in a
        ///  derived class, validates the text displayed in the up-down control.
        /// </summary>
        protected virtual void ValidateEditText()
        {
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WindowMessages.WM_SETFOCUS:
                    if (!HostedInWin32DialogManager)
                    {
                        if (ActiveControl == null)
                        {
                            SetActiveControl(TextBox);
                        }
                        else
                        {
                            FocusActiveControlInternal();
                        }
                    }
                    else
                    {
                        if (TextBox.CanFocus)
                        {
                            UnsafeNativeMethods.SetFocus(new HandleRef(TextBox, TextBox.Handle));
                        }
                        base.WndProc(ref m);
                    }
                    break;
                case WindowMessages.WM_KILLFOCUS:
                    DefWndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <summary>
        ///  This Function sets the ToolTip for this composite control.
        /// </summary>
        internal void SetToolTip(ToolTip toolTip, string caption)
        {
            toolTip.SetToolTip(upDownEdit, caption);
            toolTip.SetToolTip(upDownButtons, caption);
        }

        internal class UpDownEdit : TextBox
        {
            /////////////////////////////////////////////////////////////////////
            // Member variables
            //
            /////////////////////////////////////////////////////////////////////
            // Parent control
            private readonly UpDownBase parent;
            private bool doubleClickFired = false;
            /////////////////////////////////////////////////////////////////////
            // Constructors
            //
            /////////////////////////////////////////////////////////////////////
            internal UpDownEdit(UpDownBase parent)
            : base()
            {

                SetStyle(ControlStyles.FixedHeight |
                         ControlStyles.FixedWidth, true);

                SetStyle(ControlStyles.Selectable, false);

                this.parent = parent;
            }

            public override string Text
            {
                get
                {
                    return base.Text;
                }
                set
                {
                    bool valueChanged = (value != base.Text);
                    base.Text = value;
                    if (valueChanged)
                    {
                        AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
                    }
                }
            }

            protected override AccessibleObject CreateAccessibilityInstance()
            {
                return new UpDownEditAccessibleObject(this, parent);
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                if (e.Clicks == 2 && e.Button == MouseButtons.Left)
                {
                    doubleClickFired = true;
                }
                parent.OnMouseDown(parent.TranslateMouseEvent(this, e));
            }

            /// <summary>
            ///
            ///  Handles detecting when the mouse button is released.
            ///
            /// </summary>
            protected override void OnMouseUp(MouseEventArgs e)
            {

                Point pt = new Point(e.X, e.Y);
                pt = PointToScreen(pt);

                MouseEventArgs me = parent.TranslateMouseEvent(this, e);
                if (e.Button == MouseButtons.Left)
                {
                    if (!parent.ValidationCancelled && UnsafeNativeMethods.WindowFromPoint(pt) == Handle)
                    {
                        if (!doubleClickFired)
                        {
                            parent.OnClick(me);
                            parent.OnMouseClick(me);
                        }
                        else
                        {
                            doubleClickFired = false;
                            parent.OnDoubleClick(me);
                            parent.OnMouseDoubleClick(me);
                        }
                    }
                    doubleClickFired = false;
                }

                parent.OnMouseUp(me);
            }

            internal override void WmContextMenu(ref Message m)
            {
                // Want to make the SourceControl to be the UpDownBase, not the Edit.
                if (ContextMenuStrip != null)
                {
                    WmContextMenu(ref m, parent);
                }
                else
                {
                    WmContextMenu(ref m, this);
                }
            }

            /// <summary>
            ///  Raises the <see cref='Control.KeyUp'/>
            ///  event.
            /// </summary>
            protected override void OnKeyUp(KeyEventArgs e)
            {
                parent.OnKeyUp(e);
            }

            protected override void OnGotFocus(EventArgs e)
            {
                parent.SetActiveControl(this);
                parent.InvokeGotFocus(parent, e);
            }

            protected override void OnLostFocus(EventArgs e)
            {
                parent.InvokeLostFocus(parent, e);
            }

            // Microsoft: Focus fixes. The XXXUpDown control will
            //         also fire a Leave event. We don't want
            //         to fire two of them.
            // protected override void OnLeave(EventArgs e) {
            //     parent.OnLeave(e);
            // }

            // Create our own accessibility object to map the accessible name
            // back to our parent.  They should track.
            internal class UpDownEditAccessibleObject : ControlAccessibleObject
            {
                readonly UpDownBase parent;

                public UpDownEditAccessibleObject(UpDownEdit owner, UpDownBase parent) : base(owner)
                {
                    this.parent = parent;
                }

                public override string Name
                {
                    get
                    {
                        return parent.AccessibilityObject.Name;
                    }
                    set
                    {
                        parent.AccessibilityObject.Name = value;
                    }
                }

                public override string KeyboardShortcut
                {
                    get
                    {
                        return parent.AccessibilityObject.KeyboardShortcut;
                    }
                }
            }
        }

        /// <summary>
        ///
        ///  Nested class UpDownButtons
        ///
        ///  A control representing the pair of buttons on the end of the upDownEdit control.
        ///  This class handles drawing the updown buttons, and detecting mouse actions
        ///  on these buttons. Acceleration on the buttons is handled. The control
        ///  sends UpDownEventArgss to the parent UpDownBase class when a button is pressed,
        ///  or when the acceleration determines that another event should be generated.
        /// </summary>
        internal class UpDownButtons : Control
        {
            //

            /////////////////////////////////////////////////////////////////////
            // Member variables
            //
            /////////////////////////////////////////////////////////////////////
            // Parent control
            private readonly UpDownBase parent;

            // Button state
            private ButtonID pushed = ButtonID.None;
            private ButtonID captured = ButtonID.None;
            private ButtonID mouseOver = ButtonID.None;

            // UpDown event handler
            private UpDownEventHandler upDownEventHandler;

            // Timer
            private Timer timer;                    // generates UpDown events
            private int timerInterval;              // milliseconds between events

            private bool doubleClickFired = false;

            /////////////////////////////////////////////////////////////////////
            // Constructors
            //
            /////////////////////////////////////////////////////////////////////
            internal UpDownButtons(UpDownBase parent)

            : base()
            {

                SetStyle(ControlStyles.Opaque | ControlStyles.FixedHeight |
                         ControlStyles.FixedWidth, true);

                SetStyle(ControlStyles.Selectable, false);

                this.parent = parent;
            }

            /////////////////////////////////////////////////////////////////////
            // Methods
            //
            /////////////////////////////////////////////////////////////////////
            /// <summary>
            ///
            ///  Adds a handler for the updown button event.
            /// </summary>
            public event UpDownEventHandler UpDown
            {
                add => upDownEventHandler += value;
                remove => upDownEventHandler -= value;
            }

            // Called when the mouse button is pressed - we need to start
            // spinning the value of the updown.
            //
            private void BeginButtonPress(MouseEventArgs e)
            {

                int half_height = Size.Height / 2;

                if (e.Y < half_height)
                {

                    // Up button
                    //
                    pushed = captured = ButtonID.Up;
                    Invalidate();

                }
                else
                {

                    // Down button
                    //
                    pushed = captured = ButtonID.Down;
                    Invalidate();
                }

                // Capture the mouse
                //
                CaptureInternal = true;

                // Generate UpDown event
                //
                OnUpDown(new UpDownEventArgs((int)pushed));

                // Start the timer for new updown events
                //
                StartTimer();
            }

            protected override AccessibleObject CreateAccessibilityInstance()
            {
                return new UpDownButtonsAccessibleObject(this);
            }

            // Called when the mouse button is released - we need to stop
            // spinning the value of the updown.
            //
            private void EndButtonPress()
            {

                pushed = ButtonID.None;
                captured = ButtonID.None;

                // Stop the timer
                StopTimer();

                // Release the mouse
                CaptureInternal = false;

                // Redraw the buttons
                Invalidate();
            }

            /// <summary>
            ///
            ///  Handles detecting mouse hits on the buttons. This method
            ///  detects which button was hit (up or down), fires a
            ///  updown event, captures the mouse, and starts a timer
            ///  for repeated updown events.
            ///
            /// </summary>
            protected override void OnMouseDown(MouseEventArgs e)
            {
                // Begin spinning the value
                //

                // Focus the parent
                //
                parent.Focus();

                if (!parent.ValidationCancelled && e.Button == MouseButtons.Left)
                {
                    BeginButtonPress(e);
                }
                if (e.Clicks == 2 && e.Button == MouseButtons.Left)
                {
                    doubleClickFired = true;
                }
                // At no stage should a button be pushed, and the mouse
                // not captured.
                //
                Debug.Assert(!(pushed != ButtonID.None && captured == ButtonID.None),
                             "Invalid button pushed/captured combination");

                parent.OnMouseDown(parent.TranslateMouseEvent(this, e));
            }

            /// <summary>
            ///
            ///  Handles detecting mouse movement.
            ///
            /// </summary>
            protected override void OnMouseMove(MouseEventArgs e)
            {

                // If the mouse is captured by the buttons (i.e. an updown button
                // was pushed, and the mouse button has not yet been released),
                // determine the new state of the buttons depending on where
                // the mouse pointer has moved.

                if (Capture)
                {

                    // Determine button area

                    Rectangle rect = ClientRectangle;
                    rect.Height /= 2;

                    if (captured == ButtonID.Down)
                    {
                        rect.Y += rect.Height;
                    }

                    // Test if the mouse has moved outside the button area

                    if (rect.Contains(e.X, e.Y))
                    {

                        // Inside button
                        // Repush the button if necessary

                        if (pushed != captured)
                        {

                            // Restart the timer
                            StartTimer();

                            pushed = captured;
                            Invalidate();
                        }

                    }
                    else
                    {

                        // Outside button
                        // Retain the capture, but pop the button up whilst
                        // the mouse remains outside the button and the
                        // mouse button remains pressed.

                        if (pushed != ButtonID.None)
                        {

                            // Stop the timer for updown events
                            StopTimer();

                            pushed = ButtonID.None;
                            Invalidate();
                        }
                    }
                }

                //Logic for seeing which button is Hot if any
                Rectangle rectUp = ClientRectangle, rectDown = ClientRectangle;
                rectUp.Height /= 2;
                rectDown.Y += rectDown.Height / 2;

                //Check if the mouse is on the upper or lower button. Note that it could be in neither.
                if (rectUp.Contains(e.X, e.Y))
                {
                    mouseOver = ButtonID.Up;
                    Invalidate();
                }
                else if (rectDown.Contains(e.X, e.Y))
                {
                    mouseOver = ButtonID.Down;
                    Invalidate();
                }

                // At no stage should a button be pushed, and the mouse
                // not captured.
                Debug.Assert(!(pushed != ButtonID.None && captured == ButtonID.None),
                             "Invalid button pushed/captured combination");

                parent.OnMouseMove(parent.TranslateMouseEvent(this, e));
            }

            /// <summary>
            ///
            ///  Handles detecting when the mouse button is released.
            ///
            /// </summary>
            protected override void OnMouseUp(MouseEventArgs e)
            {

                if (!parent.ValidationCancelled && e.Button == MouseButtons.Left)
                {
                    EndButtonPress();
                }

                // At no stage should a button be pushed, and the mouse
                // not captured.
                Debug.Assert(!(pushed != ButtonID.None && captured == ButtonID.None),
                             "Invalid button pushed/captured combination");

                Point pt = new Point(e.X, e.Y);
                pt = PointToScreen(pt);

                MouseEventArgs me = parent.TranslateMouseEvent(this, e);
                if (e.Button == MouseButtons.Left)
                {
                    if (!parent.ValidationCancelled && UnsafeNativeMethods.WindowFromPoint(pt) == Handle)
                    {
                        if (!doubleClickFired)
                        {
                            parent.OnClick(me);
                        }
                        else
                        {
                            doubleClickFired = false;
                            parent.OnDoubleClick(me);
                            parent.OnMouseDoubleClick(me);
                        }
                    }
                    doubleClickFired = false;
                }

                parent.OnMouseUp(me);
            }

            /// <summary>
            ///
            ///  Handles detecting when the mouse leaves.
            ///
            /// </summary>
            protected override void OnMouseLeave(EventArgs e)
            {
                mouseOver = ButtonID.None;
                Invalidate();

                parent.OnMouseLeave(e);
            }

            /// <summary>
            ///  Handles painting the buttons on the control.
            ///
            /// </summary>
            protected override void OnPaint(PaintEventArgs e)
            {
                int half_height = ClientSize.Height / 2;

                /* Draw the up and down buttons */

                if (Application.RenderWithVisualStyles)
                {
                    VisualStyleRenderer vsr = new VisualStyleRenderer(mouseOver == ButtonID.Up ? VisualStyleElement.Spin.Up.Hot : VisualStyleElement.Spin.Up.Normal);

                    if (!Enabled)
                    {
                        vsr.SetParameters(VisualStyleElement.Spin.Up.Disabled);
                    }
                    else if (pushed == ButtonID.Up)
                    {
                        vsr.SetParameters(VisualStyleElement.Spin.Up.Pressed);
                    }

                    vsr.DrawBackground(e.Graphics, new Rectangle(0, 0, parent.defaultButtonsWidth, half_height), HandleInternal);

                    if (!Enabled)
                    {
                        vsr.SetParameters(VisualStyleElement.Spin.Down.Disabled);
                    }
                    else if (pushed == ButtonID.Down)
                    {
                        vsr.SetParameters(VisualStyleElement.Spin.Down.Pressed);
                    }
                    else
                    {
                        vsr.SetParameters(mouseOver == ButtonID.Down ? VisualStyleElement.Spin.Down.Hot : VisualStyleElement.Spin.Down.Normal);
                    }

                    vsr.DrawBackground(e.Graphics, new Rectangle(0, half_height, parent.defaultButtonsWidth, half_height), HandleInternal);
                }
                else
                {
                    ControlPaint.DrawScrollButton(e.Graphics,
                                                  new Rectangle(0, 0, parent.defaultButtonsWidth, half_height),
                                                  ScrollButton.Up,
                                                  pushed == ButtonID.Up ? ButtonState.Pushed : (Enabled ? ButtonState.Normal : ButtonState.Inactive));

                    ControlPaint.DrawScrollButton(e.Graphics,
                                                  new Rectangle(0, half_height, parent.defaultButtonsWidth, half_height),
                                                  ScrollButton.Down,
                                                  pushed == ButtonID.Down ? ButtonState.Pushed : (Enabled ? ButtonState.Normal : ButtonState.Inactive));
                }

                if (half_height != (ClientSize.Height + 1) / 2)
                {
                    // When control has odd height, a line needs to be drawn below the buttons with the backcolor.
                    using (Pen pen = new Pen(parent.BackColor))
                    {
                        Rectangle clientRect = ClientRectangle;
                        e.Graphics.DrawLine(pen, clientRect.Left, clientRect.Bottom - 1, clientRect.Right - 1, clientRect.Bottom - 1);
                    }
                }

                base.OnPaint(e); // raise paint event, just in case this inner class goes public some day
            }

            /// <summary>
            ///  Occurs when the UpDown buttons are pressed and when the acceleration timer tick event is raised.
            /// </summary>
            protected virtual void OnUpDown(UpDownEventArgs upevent)
            {
                upDownEventHandler?.Invoke(this, upevent);
            }

            /// <summary>
            ///  Starts the timer for generating updown events
            /// </summary>
            protected void StartTimer()
            {
                parent.OnStartTimer();
                if (timer == null)
                {
                    timer = new Timer();      // generates UpDown events
                    // Add the timer handler
                    timer.Tick += new EventHandler(TimerHandler);
                }

                timerInterval = DefaultTimerInterval;

                timer.Interval = timerInterval;
                timer.Start();
            }

            /// <summary>
            ///  Stops the timer for generating updown events
            /// </summary>
            protected void StopTimer()
            {
                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                    timer = null;
                }
                parent.OnStopTimer();
            }

            /// <summary>
            ///  Generates updown events when the timer calls this function.
            /// </summary>
            private void TimerHandler(object source, EventArgs args)
            {

                // Make sure we've got mouse capture
                if (!Capture)
                {
                    EndButtonPress();
                    return;
                }

                // onUpDown method calls customer's ValueCHanged event handler which might enter the message loop and
                // process the mouse button up event, which results in timer being disposed
                OnUpDown(new UpDownEventArgs((int)pushed));

                if (timer != null)
                {
                    // Accelerate timer.
                    timerInterval *= 7;
                    timerInterval /= 10;

                    if (timerInterval < 1)
                    {
                        timerInterval = 1;
                    }

                    timer.Interval = timerInterval;
                }
            }

            internal class UpDownButtonsAccessibleObject : ControlAccessibleObject
            {

                private DirectionButtonAccessibleObject upButton;
                private DirectionButtonAccessibleObject downButton;

                public UpDownButtonsAccessibleObject(UpDownButtons owner) : base(owner)
                {
                }

                public override string Name
                {
                    get
                    {
                        string baseName = base.Name;
                        if (baseName == null || baseName.Length == 0)
                        {
                            // Spinner is already announced so use type name.
                            return Owner.ParentInternal.GetType().Name;
                        }

                        return baseName;
                    }
                    set
                    {
                        base.Name = value;
                    }
                }

                public override AccessibleRole Role
                {
                    get
                    {
                        AccessibleRole role = Owner.AccessibleRole;
                        if (role != AccessibleRole.Default)
                        {
                            return role;
                        }
                        return AccessibleRole.SpinButton;
                    }
                }

                private DirectionButtonAccessibleObject UpButton
                {
                    get
                    {
                        if (upButton == null)
                        {
                            upButton = new DirectionButtonAccessibleObject(this, true);
                        }
                        return upButton;
                    }
                }

                private DirectionButtonAccessibleObject DownButton
                {
                    get
                    {
                        if (downButton == null)
                        {
                            downButton = new DirectionButtonAccessibleObject(this, false);
                        }
                        return downButton;
                    }
                }

                /// <summary>
                /// </summary>
                public override AccessibleObject GetChild(int index)
                {

                    // Up button
                    //
                    if (index == 0)
                    {
                        return UpButton;
                    }

                    // Down button
                    //
                    if (index == 1)
                    {
                        return DownButton;
                    }

                    return null;
                }

                /// <summary>
                /// </summary>
                public override int GetChildCount()
                {
                    return 2;
                }

                internal class DirectionButtonAccessibleObject : AccessibleObject
                {
                    private readonly bool up;
                    private readonly UpDownButtonsAccessibleObject parent;

                    public DirectionButtonAccessibleObject(UpDownButtonsAccessibleObject parent, bool up)
                    {
                        this.parent = parent;
                        this.up = up;
                    }

                    public override Rectangle Bounds
                    {
                        get
                        {
                            // Get button bounds
                            //
                            Rectangle bounds = ((UpDownButtons)parent.Owner).Bounds;
                            bounds.Height /= 2;
                            if (!up)
                            {
                                bounds.Y += bounds.Height;
                            }

                            // Convert to screen co-ords
                            //
                            return (((UpDownButtons)parent.Owner).ParentInternal).RectangleToScreen(bounds);
                        }
                    }

                    public override string Name
                    {
                        get
                        {
                            if (up)
                            {
                                return SR.UpDownBaseUpButtonAccName;
                            }
                            return SR.UpDownBaseDownButtonAccName;
                        }
                        set
                        {
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
                            return AccessibleRole.PushButton;
                        }
                    }
                }
            }

        } // end class UpDownButtons

        // Button identifiers

        internal enum ButtonID
        {
            None = 0,
            Up = 1,
            Down = 2,
        }
    } // end class UpDownBase
}

