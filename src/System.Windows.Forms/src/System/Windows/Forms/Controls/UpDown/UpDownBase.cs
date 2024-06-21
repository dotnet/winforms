// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;
using Microsoft.Win32;

namespace System.Windows.Forms;

/// <summary>
///  Implements the basic functionality required by an up-down control.
/// </summary>
[Designer($"System.Windows.Forms.Design.UpDownBaseDesigner, {AssemblyRef.SystemDesign}")]
public abstract partial class UpDownBase : ContainerControl
{
    private const int DefaultWheelScrollLinesPerPage = 1;
    private const int DefaultButtonsWidth = 16;
    private const int DefaultControlWidth = 120;
    private const int ThemedBorderWidth = 1; // width of custom border we draw when themed
    private const BorderStyle DefaultBorderStyle = BorderStyle.Fixed3D;
    private const LeftRightAlignment DefaultUpDownAlign = LeftRightAlignment.Right;
    private const int DefaultTimerInterval = 500;

    // Child controls
    internal UpDownEdit _upDownEdit;
    internal UpDownButtons _upDownButtons;

    private LeftRightAlignment _upDownAlign = DefaultUpDownAlign;

    /// <summary>
    ///  The current border for this edit control.
    /// </summary>
    private BorderStyle _borderStyle = DefaultBorderStyle;

    // Mouse wheel movement
    private int _wheelDelta;
    private bool _doubleClickFired;
    internal int _defaultButtonsWidth = DefaultButtonsWidth;

    /// <summary>
    ///  Initializes a new instance of the <see cref="UpDownBase"/> class.
    /// </summary>
    public UpDownBase()
    {
        _defaultButtonsWidth = LogicalToDeviceUnits(DefaultButtonsWidth);

        _upDownButtons = new UpDownButtons(this);
        _upDownEdit = new UpDownEdit(this)
        {
            BorderStyle = BorderStyle.None,
            AutoSize = false
        };

        _upDownEdit.KeyDown += OnTextBoxKeyDown;
        _upDownEdit.KeyPress += OnTextBoxKeyPress;
        _upDownEdit.TextChanged += OnTextBoxTextChanged;
        _upDownEdit.LostFocus += OnTextBoxLostFocus;
        _upDownEdit.Resize += OnTextBoxResize;
        _upDownButtons.TabStop = false;
        _upDownButtons.Size = new Size(_defaultButtonsWidth, PreferredHeight);
        _upDownButtons.UpDown += OnUpDown;

        Controls.AddRange([_upDownButtons, _upDownEdit]);

        SetStyle(ControlStyles.Opaque | ControlStyles.FixedHeight | ControlStyles.ResizeRedraw, true);
        SetStyle(ControlStyles.StandardClick, false);
        SetStyle(ControlStyles.UseTextForAccessibility, false);
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override bool AutoScroll
    {
        get => false;
        set
        {
            // Don't allow AutoScroll to be set to anything
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Size AutoScrollMargin
    {
        get => base.AutoScrollMargin;
        set => base.AutoScrollMargin = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Size AutoScrollMinSize
    {
        get => base.AutoScrollMinSize;
        set => base.AutoScrollMinSize = value;
    }

    /// <summary>
    ///  Override to re-expose AutoSize.
    /// </summary>
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public override bool AutoSize
    {
        get => base.AutoSize;
        set => base.AutoSize = value;
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnAutoSizeChangedDescr))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public new event EventHandler? AutoSizeChanged
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
        get => _upDownEdit.BackColor;
        set
        {
            base.BackColor = value; // Don't remove this or you will break serialization.
            _upDownEdit.BackColor = value;
            Invalidate();
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Image? BackgroundImage
    {
        get => base.BackgroundImage;
        set => base.BackgroundImage = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageChanged
    {
        add => base.BackgroundImageChanged += value;
        remove => base.BackgroundImageChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override ImageLayout BackgroundImageLayout
    {
        get => base.BackgroundImageLayout;
        set => base.BackgroundImageLayout = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageLayoutChanged
    {
        add => base.BackgroundImageLayoutChanged += value;
        remove => base.BackgroundImageLayoutChanged -= value;
    }

    /// <summary>
    ///  Gets or sets the border style for the up-down control.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(BorderStyle.Fixed3D)]
    [DispId(PInvokeCore.DISPID_BORDERSTYLE)]
    [SRDescription(nameof(SR.UpDownBaseBorderStyleDescr))]
    public BorderStyle BorderStyle
    {
        get => _borderStyle;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (_borderStyle != value)
            {
                _borderStyle = value;
                RecreateHandle();
            }
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the text property is being
    ///  changed internally by its parent class.
    /// </summary>
    protected bool ChangingText { get; set; }

    public override ContextMenuStrip? ContextMenuStrip
    {
        get => base.ContextMenuStrip;
        set
        {
            base.ContextMenuStrip = value;
            _upDownEdit.ContextMenuStrip = value;
        }
    }

    protected override AccessibleObject CreateAccessibilityInstance()
        => new UpDownBaseAccessibleObject(this);

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;

            cp.Style &= ~(int)WINDOW_STYLE.WS_BORDER;
            if (!Application.RenderWithVisualStyles)
            {
                switch (_borderStyle)
                {
                    case BorderStyle.Fixed3D:
                        cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_CLIENTEDGE;
                        break;
                    case BorderStyle.FixedSingle:
                        cp.Style |= (int)WINDOW_STYLE.WS_BORDER;
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
    protected override Size DefaultSize => new(DefaultControlWidth, PreferredHeight);

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new DockPaddingEdges DockPadding => base.DockPadding;

    /// <summary>
    ///  Returns true if this control has focus.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlFocusedDescr))]
    public override bool Focused => _upDownEdit.Focused;

    /// <summary>
    ///  Indicates the foreground color for the control.
    /// </summary>
    public override Color ForeColor
    {
        get => _upDownEdit.ForeColor;
        set
        {
            base.ForeColor = value;
            _upDownEdit.ForeColor = value;
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the user can use the UP ARROW and
    ///  DOWN ARROW keys to select values.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.UpDownBaseInterceptArrowKeysDescr))]
    public bool InterceptArrowKeys { get; set; } = true;

    public override Size MaximumSize
    {
        get => base.MaximumSize;
        set => base.MaximumSize = new Size(value.Width, 0);
    }

    public override Size MinimumSize
    {
        get => base.MinimumSize;
        set => base.MinimumSize = new Size(value.Width, 0);
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? MouseEnter
    {
        add => base.MouseEnter += value;
        remove => base.MouseEnter -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? MouseLeave
    {
        add => base.MouseLeave += value;
        remove => base.MouseLeave -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? MouseHover
    {
        add => base.MouseHover += value;
        remove => base.MouseHover -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event MouseEventHandler? MouseMove
    {
        add => base.MouseMove += value;
        remove => base.MouseMove -= value;
    }

    /// <summary>
    ///  Gets the height of the up-down control.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.UpDownBasePreferredHeightDescr))]
    public int PreferredHeight
    {
        get
        {
            int height = FontHeight;

            // Adjust for the border style
            if (_borderStyle != BorderStyle.None)
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
    ///  Gets or sets a value indicating whether the text may only be changed by
    ///  the use of the up or down buttons.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.UpDownBaseReadOnlyDescr))]
    public bool ReadOnly
    {
        get => _upDownEdit.ReadOnly;
        set => _upDownEdit.ReadOnly = value;
    }

    /// <summary>
    ///  Gets or sets the text displayed in the up-down control.
    /// </summary>
    [Localizable(true)]
    [AllowNull]
    public override string Text
    {
        get => _upDownEdit.Text;
        set
        {
            // The text changed event will at this point be triggered. After returning, the value of UserEdit will
            // reflect whether or not the current upDownEditbox text is in sync with any internally stored values.
            // If UserEdit is true, we must validate the text the user typed or set.

            _upDownEdit.Text = value;

            // Usually, the code in the Text changed event handler sets ChangingText back to false. If the text
            // hasn't actually changed though, the event handler never fires. ChangingText should always be false
            // on exit from this property.

            ChangingText = false;

            if (UserEdit)
            {
                ValidateEditText();
            }
        }
    }

    /// <summary>
    ///  Gets or sets the alignment of the text in the up-down control.
    /// </summary>
    [Localizable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(HorizontalAlignment.Left)]
    [SRDescription(nameof(SR.UpDownBaseTextAlignDescr))]
    public HorizontalAlignment TextAlign
    {
        get => _upDownEdit.TextAlign;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            _upDownEdit.TextAlign = value;
        }
    }

    internal TextBox TextBox => _upDownEdit;

    /// <summary>
    ///  Gets or sets the alignment of the up and down buttons on the up-down control.
    /// </summary>
    [Localizable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(LeftRightAlignment.Right)]
    [SRDescription(nameof(SR.UpDownBaseAlignmentDescr))]
    public LeftRightAlignment UpDownAlign
    {
        get => _upDownAlign;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (_upDownAlign != value)
            {
                _upDownAlign = value;
                PositionControls();
                Invalidate();
            }
        }
    }

    internal UpDownButtons UpDownButtonsInternal => _upDownButtons;

    /// <summary>
    ///  Gets or sets a value indicating whether a value has been entered by the user.
    /// </summary>
    protected bool UserEdit { get; set; }

    /// <summary>
    ///  When overridden in a derived class, handles the pressing of the down button
    ///  on the up-down control.
    /// </summary>
    public abstract void DownButton();

    internal override Rectangle ApplyBoundsConstraints(int suggestedX, int suggestedY, int proposedWidth, int proposedHeight)
    {
        return base.ApplyBoundsConstraints(suggestedX, suggestedY, proposedWidth, PreferredHeight);
    }

    internal override void ReleaseUiaProvider(HWND handle)
    {
        // UpDownEdit as TextBox is a control, that should disconnect its accessible object itself,
        // but if it supports Uia providers. If no, force disconnecting for UpDownEdit accessible object
        // as a part of UIA tree of Domain/NumericUpDown controls.
        if (!_upDownEdit.SupportsUiaProviders)
        {
            _upDownEdit.ReleaseUiaProvider(_upDownEdit.HWNDInternal);
        }

        base.ReleaseUiaProvider(handle);
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
        _defaultButtonsWidth = LogicalToDeviceUnits(DefaultButtonsWidth);
        _upDownButtons.Width = _defaultButtonsWidth;
    }

    /// <summary>
    ///  When overridden in a derived class, raises the Changed event.
    /// </summary>
    protected virtual void OnChanged(object? source, EventArgs e)
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

        Rectangle editBounds = _upDownEdit.Bounds;
        Color backColor = BackColor;

        if (Application.RenderWithVisualStyles)
        {
            if (_borderStyle != BorderStyle.None)
            {
                Rectangle bounds = ClientRectangle;
                Rectangle clipBounds = e.ClipRectangle;

                // Draw a themed textbox-like border, which is what the spin control does
                VisualStyleRenderer vsr = new(VisualStyleElement.TextBox.TextEdit.Normal);
                int border = ThemedBorderWidth;
                Rectangle clipLeft = new(bounds.Left, bounds.Top, border, bounds.Height);
                Rectangle clipTop = new(bounds.Left, bounds.Top, bounds.Width, border);
                Rectangle clipRight = new(bounds.Right - border, bounds.Top, border, bounds.Height);
                Rectangle clipBottom = new(bounds.Left, bounds.Bottom - border, bounds.Width, border);
                clipLeft.Intersect(clipBounds);
                clipTop.Intersect(clipBounds);
                clipRight.Intersect(clipBounds);
                clipBottom.Intersect(clipBounds);

                using DeviceContextHdcScope hdc = new(e);
                vsr.DrawBackground(hdc, bounds, clipLeft, HWNDInternal);
                vsr.DrawBackground(hdc, bounds, clipTop, HWNDInternal);
                vsr.DrawBackground(hdc, bounds, clipRight, HWNDInternal);
                vsr.DrawBackground(hdc, bounds, clipBottom, HWNDInternal);

                // Draw a rectangle around edit control with the background color.
                Rectangle backRect = editBounds;
                backRect.X--;
                backRect.Y--;
                backRect.Width += 2;
                backRect.Height += 2;
                using CreatePenScope hpen = new(backColor);
                hdc.DrawRectangle(backRect, hpen);
            }
        }
        else
        {
            // Draw a rectangle around edit control with the background color.
            Rectangle backRect = editBounds;
            backRect.Inflate(1, 1);
            if (!Enabled)
            {
                backRect.X--;
                backRect.Y--;
                backRect.Width++;
                backRect.Height++;
            }

            int width = Enabled ? 2 : 1;

            backRect.Width++;
            backRect.Height++;
            using DeviceContextHdcScope hdc = new(e);
            using CreatePenScope hpen = new(backColor, width);
            hdc.DrawRectangle(backRect, hpen);
        }

        if (!Enabled && BorderStyle != BorderStyle.None && !_upDownEdit.ShouldSerializeBackColor())
        {
            // Draws a grayed rectangle around the upDownEdit, since otherwise we will have a white
            // border around the upDownEdit, which is inconsistent with Windows' behavior
            // we only want to do this when BackColor is not serialized, since otherwise
            // we should display the backcolor instead of the usual grayed textbox.
            editBounds.Inflate(1, 1);
            ControlPaint.DrawBorderSimple(e, editBounds, Application.SystemColors.Control);
        }
    }

    /// <summary>
    ///  Raises the <see cref="Control.KeyDown"/> event.
    /// </summary>
    protected virtual void OnTextBoxKeyDown(object? source, KeyEventArgs e)
    {
        OnKeyDown(e);
        if (InterceptArrowKeys)
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
        if (e.KeyCode == Keys.Return && UserEdit)
        {
            ValidateEditText();
        }
    }

    /// <summary>
    ///  Raises the <see cref="Control.KeyPress"/> event.
    /// </summary>
    protected virtual void OnTextBoxKeyPress(object? source, KeyPressEventArgs e)
        => OnKeyPress(e);

    /// <summary>
    ///  Raises the <see cref="Control.LostFocus"/> event.
    /// </summary>
    protected virtual void OnTextBoxLostFocus(object? source, EventArgs e)
    {
        if (UserEdit)
        {
            ValidateEditText();
        }
    }

    /// <summary>
    ///  Raises the <see cref="Control.Resize"/> event.
    /// </summary>
    protected virtual void OnTextBoxResize(object? source, EventArgs e)
    {
        Height = PreferredHeight;
        PositionControls();
    }

    /// <summary>
    ///  Raises the TextBoxTextChanged event.
    ///  event.
    /// </summary>
    protected virtual void OnTextBoxTextChanged(object? source, EventArgs e)
    {
        if (ChangingText)
        {
            Debug.Assert(!UserEdit, "OnTextBoxTextChanged() - UserEdit == true");
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
    ///  Raises the <see cref="Control.OnMouseDown"/> event.
    /// </summary>
    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Clicks == 2 && e.Button == MouseButtons.Left)
        {
            _doubleClickFired = true;
        }

        base.OnMouseDown(e);
    }

    /// <summary>
    ///  Raises the <see cref="Control.OnMouseUp"/> event.
    /// </summary>
    protected override void OnMouseUp(MouseEventArgs mevent)
    {
        if (mevent.Button == MouseButtons.Left)
        {
            if (PInvoke.WindowFromPoint(PointToScreen(mevent.Location)) == HWND && !ValidationCancelled)
            {
                if (!_doubleClickFired)
                {
                    OnClick(mevent);
                    OnMouseClick(mevent);
                }
                else
                {
                    _doubleClickFired = false;
                    OnDoubleClick(mevent);
                    OnMouseDoubleClick(mevent);
                }
            }

            _doubleClickFired = false;
        }

        base.OnMouseUp(mevent);
    }

    /// <summary>
    ///  Raises the <see cref="Control.OnMouseWheel"/> event.
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
            // Do not scroll when Shift or Alt key is down, or when a mouse button is down.
            return;
        }

        int wheelScrollLines = SystemInformation.MouseWheelScrollLines;
        if (wheelScrollLines == 0)
        {
            // Do not scroll when the user system setting is 0 lines per notch
            return;
        }

        Debug.Assert(_wheelDelta > -PInvoke.WHEEL_DELTA, "wheelDelta is too small");
        Debug.Assert(_wheelDelta < PInvoke.WHEEL_DELTA, "wheelDelta is too big");
        _wheelDelta += e.Delta;

        float partialNotches;
        partialNotches = _wheelDelta / (float)PInvoke.WHEEL_DELTA;

        if (wheelScrollLines == -1)
        {
            wheelScrollLines = DefaultWheelScrollLinesPerPage;
        }

        // Evaluate number of bands to scroll
        int scrollBands = (int)(wheelScrollLines * partialNotches);
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

                _wheelDelta -= (int)(scrollBands * (PInvoke.WHEEL_DELTA / (float)wheelScrollLines));
            }
            else
            {
                absScrollBands = -scrollBands;
                while (absScrollBands > 0)
                {
                    DownButton();
                    absScrollBands--;
                }

                _wheelDelta -= (int)(scrollBands * (PInvoke.WHEEL_DELTA / (float)wheelScrollLines));
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
    ///  Handles UpDown events, which are generated by clicking on the updown
    ///  buttons in the child updown control.
    /// </summary>
    private void OnUpDown(object? source, UpDownEventArgs e)
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
    ///  Calculates the size and position of the upDownEdit control and the updown buttons.
    /// </summary>
    private void PositionControls()
    {
        Rectangle upDownEditBounds = Rectangle.Empty;
        Rectangle upDownButtonsBounds = Rectangle.Empty;

        Rectangle clientArea = new(Point.Empty, ClientSize);
        int totalClientWidth = clientArea.Width;
        bool themed = Application.RenderWithVisualStyles;
        BorderStyle borderStyle = BorderStyle;

        // Determine how much to squish in - Fixed3D and FixedSingle have 2PX border
        int borderWidth = (borderStyle == BorderStyle.None) ? 0 : 2;
        clientArea.Inflate(-borderWidth, -borderWidth);

        // Reposition and resize the upDownEdit control
        if (_upDownEdit is not null)
        {
            upDownEditBounds = clientArea;
            upDownEditBounds.Size = new Size(clientArea.Width - _defaultButtonsWidth, clientArea.Height);
        }

        // Reposition and resize the updown buttons
        if (_upDownButtons is not null)
        {
            int borderFixup = (themed) ? 1 : 2;
            if (borderStyle == BorderStyle.None)
            {
                borderFixup = 0;
            }

            upDownButtonsBounds = new Rectangle(
                clientArea.Right - _defaultButtonsWidth + borderFixup,
                clientArea.Top - borderFixup,
                _defaultButtonsWidth,
                clientArea.Height + (borderFixup * 2));
        }

        // Right to left translation
        LeftRightAlignment updownAlign = UpDownAlign;
        updownAlign = RtlTranslateLeftRight(updownAlign);

        // Left/right updown align translation
        if (updownAlign == LeftRightAlignment.Left)
        {
            // If the buttons are aligned to the left, swap position of text box/buttons
            upDownButtonsBounds.X = totalClientWidth - upDownButtonsBounds.Right;
            upDownEditBounds.X = totalClientWidth - upDownEditBounds.Right;
        }

        // Apply locations
        if (_upDownEdit is not null)
        {
            _upDownEdit.Bounds = upDownEditBounds;
        }

        if (_upDownButtons is not null)
        {
            _upDownButtons.Bounds = upDownButtonsBounds;
            _upDownButtons.Invalidate();
        }
    }

    /// <summary>
    ///  Selects a range of text in the up-down control.
    /// </summary>
    public void Select(int start, int length) => _upDownEdit.Select(start, length);

    /// <summary>
    ///  Create a new <see cref="MouseEventArgs"/> with the points translated from the <paramref name="child"/>
    ///  coordinates to this control's.
    /// </summary>
    private MouseEventArgs TranslateMouseEvent(Control child, MouseEventArgs e)
    {
        if (child is not null && IsHandleCreated)
        {
            // Same control as PointToClient or PointToScreen, just
            // with two specific controls in mind.
            Point point = e.Location;
            point = WindowsFormsUtils.TranslatePoint(point, child, this);
            return new MouseEventArgs(e.Button, e.Clicks, point.X, point.Y, e.Delta);
        }

        return e;
    }

    /// <summary>
    ///  When overridden in a derived class, handles the pressing of the up button on the up-down control.
    /// </summary>
    public abstract void UpButton();

    /// <summary>
    ///  When overridden in a derived class, updates the text displayed in the up-down control.
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
    ///  When overridden in a derived class, validates the text displayed in the up-down control.
    /// </summary>
    protected virtual void ValidateEditText()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case PInvoke.WM_SETFOCUS:
                if (!HostedInWin32DialogManager)
                {
                    if (ActiveControl is null)
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
                        PInvoke.SetFocus(TextBox);
                    }

                    base.WndProc(ref m);
                }

                break;
            case PInvoke.WM_KILLFOCUS:
                DefWndProc(ref m);
                break;
            default:
                base.WndProc(ref m);
                break;
        }
    }

    internal override void SetToolTip(ToolTip toolTip)
    {
        if (toolTip is null)
        {
            return;
        }

        string? caption = toolTip.GetToolTip(this);
        toolTip.SetToolTip(_upDownEdit, caption);
        toolTip.SetToolTip(_upDownButtons, caption);
    }

    internal override void RemoveToolTip(ToolTip toolTip)
    {
        if (toolTip is null)
        {
            return;
        }

        string? caption = toolTip.GetToolTip(this);
        toolTip.SetToolTip(_upDownEdit, caption);
        toolTip.SetToolTip(_upDownButtons, caption);
    }
}
