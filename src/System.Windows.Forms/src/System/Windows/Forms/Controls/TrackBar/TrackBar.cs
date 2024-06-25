// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;
using System.Windows.Forms.Primitives;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

/// <summary>
///  The TrackBar is a scrollable control similar to the ScrollBar, but
///  has a different UI. You can configure ranges through which it should
///  scroll, and also define increments for off-button clicks. It can be
///  aligned horizontally or vertically. You can also configure how many
///  'ticks' are shown for the total range of values
/// </summary>
[DefaultProperty(nameof(Value))]
[DefaultEvent(nameof(Scroll))]
[DefaultBindingProperty(nameof(Value))]
[Designer($"System.Windows.Forms.Design.TrackBarDesigner, {AssemblyRef.SystemDesign}")]
[SRDescription(nameof(SR.DescriptionTrackBar))]
public partial class TrackBar : Control, ISupportInitialize
{
    private static readonly object s_scrollEvent = new();
    private static readonly object s_valueChangedEvent = new();
    private static readonly object s_rightToLeftChangedEvent = new();
    private bool _autoSize = true;
    private int _largeChange = 5;
    private int _maximum = 10;
    private int _minimum;
    private Orientation _orientation = Orientation.Horizontal;
    private int _value;
    private int _smallChange = 1;
    private int _tickFrequency = 1;
    private TickStyle _tickStyle = TickStyle.BottomRight;
    private int _requestedDim;
    private bool _autoDrawTicks;
    // Mouse wheel movement
    private int _cumulativeWheelData;

    // Disable value range checking while initializing the control
    private bool _initializing;

    private bool _rightToLeftLayout;

    /// <summary>
    ///  Creates a new TrackBar control with a default range of 0..10 and
    ///  ticks shown every value.
    /// </summary>
    public TrackBar() : base()
    {
        SetStyle(ControlStyles.UserPaint, false);
        SetStyle(ControlStyles.UseTextForAccessibility, false);
        _requestedDim = PreferredDimension;
    }

    /// <summary>
    ///  Indicates if the control is being auto-sized.  If true, the
    ///  TrackBar will adjust either its height or width [depending on
    ///  orientation] to make sure that only the required amount of
    ///  space is used.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.TrackBarAutoSizeDescr))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public override bool AutoSize
    {
        get => _autoSize;
        set
        {
            // Note that we intentionally do not call base.AutoSize. Labels size themselves by
            // overriding SetBoundsCore (legacy behaviour). We let CommonProperties.GetAutoSize
            // continue to return false to keep our LayoutEngines from messing with TextBoxes.
            // This is done for backwards compatibility since the new AutoSize behavior differs.
            if (_autoSize != value)
            {
                _autoSize = value;
                if (_orientation == Orientation.Horizontal)
                {
                    SetStyle(ControlStyles.FixedHeight, _autoSize);
                    SetStyle(ControlStyles.FixedWidth, false);
                }
                else
                {
                    SetStyle(ControlStyles.FixedWidth, _autoSize);
                    SetStyle(ControlStyles.FixedHeight, false);
                }

                AdjustSize();
                OnAutoSizeChanged(EventArgs.Empty);
            }
        }
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

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
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

    protected override CreateParams CreateParams
    {
        get
        {
            // If the user opts out of TrackBarModernRendering
            // then _autoDrawTicks will be set to true
            _autoDrawTicks = ShouldAutoDrawTicks();
            CreateParams cp = base.CreateParams;
            cp.ClassName = PInvoke.TRACKBAR_CLASS;
            switch (_tickStyle)
            {
                case TickStyle.None:
                    cp.Style |= (int)PInvoke.TBS_NOTICKS;
                    break;
                case TickStyle.TopLeft:
                    cp.Style |= (int)(PInvoke.TBS_TOP);
                    EnableAutoTicksIfRequired();
                    break;
                case TickStyle.BottomRight:
                    cp.Style |= (int)(PInvoke.TBS_BOTTOM);
                    EnableAutoTicksIfRequired();
                    break;
                case TickStyle.Both:
                    cp.Style |= (int)(PInvoke.TBS_BOTH);
                    EnableAutoTicksIfRequired();
                    break;
            }

            if (_orientation == Orientation.Vertical)
            {
                cp.Style |= (int)PInvoke.TBS_VERT;
            }

            if (RightToLeft == RightToLeft.Yes && RightToLeftLayout)
            {
                // We want to turn on mirroring for Trackbar explicitly.
                // Don't need these styles when mirroring is turned on.
                cp.ExStyle |= (int)(WINDOW_EX_STYLE.WS_EX_LAYOUTRTL | WINDOW_EX_STYLE.WS_EX_NOINHERITLAYOUT);
                cp.ExStyle &= ~(int)(WINDOW_EX_STYLE.WS_EX_RTLREADING | WINDOW_EX_STYLE.WS_EX_RIGHT | WINDOW_EX_STYLE.WS_EX_LEFTSCROLLBAR);
            }

            return cp;

            void EnableAutoTicksIfRequired()
            {
                if (_autoDrawTicks)
                {
                    cp.Style |= (int)PInvoke.TBS_AUTOTICKS;
                }
            }
        }
    }

    protected override ImeMode DefaultImeMode => ImeMode.Disable;

    protected override Size DefaultSize => new(104, PreferredDimension);

    /// <summary>
    ///  This property is overridden and hidden from statement completion
    ///  on controls that are based on Win32 Native Controls.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override bool DoubleBuffered
    {
        get => base.DoubleBuffered;
        set => base.DoubleBuffered = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AllowNull]
    public override Font Font
    {
        get => base.Font;
        set => base.Font = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? FontChanged
    {
        add => base.FontChanged += value;
        remove => base.FontChanged -= value;
    }

    /// <summary>
    ///  The current foreground color of the TrackBar. Note that users
    ///  are unable to change this. It is always Color.WindowText
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Color ForeColor
    {
        get => Application.ApplicationColors.WindowText;
        set { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? ForeColorChanged
    {
        add => base.ForeColorChanged += value;
        remove => base.ForeColorChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new ImeMode ImeMode
    {
        get => base.ImeMode;
        set => base.ImeMode = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? ImeModeChanged
    {
        add => base.ImeModeChanged += value;
        remove => base.ImeModeChanged -= value;
    }

    /// <summary>
    ///  The number of ticks by which the TrackBar will change when an
    ///  event considered a "large change" occurs.  These include, Clicking the
    ///  mouse to the side of the button, or using the PgUp/PgDn keys on the
    ///  keyboard.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(5)]
    [SRDescription(nameof(SR.TrackBarLargeChangeDescr))]
    public int LargeChange
    {
        get => _largeChange;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.TrackBarLargeChangeError, value));
            }

            if (_largeChange == value)
            {
                return;
            }

            _largeChange = value;
            if (IsHandleCreated)
            {
                PInvoke.SendMessage(this, PInvoke.TBM_SETPAGESIZE, 0, value);
            }
        }
    }

    /// <summary>
    ///  The upper limit of the range this TrackBar is working with.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(10)]
    [RefreshProperties(RefreshProperties.All)]
    [SRDescription(nameof(SR.TrackBarMaximumDescr))]
    public int Maximum
    {
        get => _maximum;
        set
        {
            if (_maximum == value)
            {
                return;
            }

            if (value < _minimum)
            {
                _minimum = value;
            }

            SetRange(_minimum, value);
        }
    }

    /// <summary>
    ///  The lower limit of the range this TrackBar is working with.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(0)]
    [RefreshProperties(RefreshProperties.All)]
    [SRDescription(nameof(SR.TrackBarMinimumDescr))]
    public int Minimum
    {
        get => _minimum;
        set
        {
            if (_minimum == value)
            {
                return;
            }

            if (value > _maximum)
            {
                _maximum = value;
            }

            SetRange(value, _maximum);
        }
    }

    /// <summary>
    ///  The orientation for this TrackBar. Valid values are from
    ///  the Orientation enumeration. The control currently supports being
    ///  oriented horizontally and vertically.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(Orientation.Horizontal)]
    [Localizable(true)]
    [SRDescription(nameof(SR.TrackBarOrientationDescr))]
    public Orientation Orientation
    {
        get => _orientation;
        set
        {
            if (value is < Orientation.Horizontal or > Orientation.Vertical)
            {
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Orientation));
            }

            if (_orientation == value)
            {
                return;
            }

            _orientation = value;

            if (_orientation == Orientation.Horizontal)
            {
                SetStyle(ControlStyles.FixedHeight, _autoSize);
                SetStyle(ControlStyles.FixedWidth, false);
                Width = _requestedDim;
            }
            else
            {
                SetStyle(ControlStyles.FixedHeight, false);
                SetStyle(ControlStyles.FixedWidth, _autoSize);
                Height = _requestedDim;
            }

            if (IsHandleCreated)
            {
                Rectangle r = Bounds;
                RecreateHandle();
                SetBounds(r.X, r.Y, r.Height, r.Width, BoundsSpecified.All);
                AdjustSize();
            }
        }
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
    public new event EventHandler? PaddingChanged
    {
        add => base.PaddingChanged += value;
        remove => base.PaddingChanged -= value;
    }

    /// <summary>
    ///  Little private routine that helps with auto-sizing.
    /// </summary>
    private static int PreferredDimension
    {
        get
        {
            int cyhscroll = PInvokeCore.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYHSCROLL);
            return ((cyhscroll * 8) / 3);
        }
    }

    /// <summary>
    ///  Redraw control, if the handle's created
    /// </summary>
    private void RedrawControl()
    {
        if (!IsHandleCreated)
        {
            return;
        }

        PInvoke.SendMessage(this, PInvoke.TBM_SETRANGEMAX, (WPARAM)(BOOL)true, (LPARAM)_maximum);
        Invalidate();
    }

    /// <summary>
    ///  This is used for international applications where the language is written from RightToLeft.
    ///  When this property is true, and the RightToLeft property is true, mirroring will be turned
    ///  on on the trackbar.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.ControlRightToLeftLayoutDescr))]
    public virtual bool RightToLeftLayout
    {
        get => _rightToLeftLayout;

        set
        {
            if (value == _rightToLeftLayout)
            {
                return;
            }

            _rightToLeftLayout = value;
            using (new LayoutTransaction(this, this, PropertyNames.RightToLeftLayout))
            {
                OnRightToLeftLayoutChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    ///  The number of ticks by which the TrackBar will change when an
    ///  event considered a "small change" occurs.  These are most commonly
    ///  seen by using the arrow keys to move the TrackBar thumb around.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(1)]
    [SRDescription(nameof(SR.TrackBarSmallChangeDescr))]
    public int SmallChange
    {
        get => _smallChange;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.TrackBarSmallChangeError, value));
            }

            if (_smallChange == value)
            {
                return;
            }

            _smallChange = value;
            if (IsHandleCreated)
            {
                PInvoke.SendMessage(this, PInvoke.TBM_SETLINESIZE, 0, value);
            }
        }
    }

    internal override bool SupportsUiaProviders => true;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Bindable(false)]
    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set => base.Text = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TextChanged
    {
        add => base.TextChanged += value;
        remove => base.TextChanged -= value;
    }

    /// <summary>
    ///  Indicates how the TrackBar control will draw itself. This affects
    ///  both where the ticks will be drawn in relation to the moveable thumb,
    ///  and how the thumb itself will be drawn. Values are taken from the
    ///  TickStyle enumeration.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(TickStyle.BottomRight)]
    [SRDescription(nameof(SR.TrackBarTickStyleDescr))]
    public TickStyle TickStyle
    {
        get => _tickStyle;
        set
        {
            if (value is < TickStyle.None or > TickStyle.Both)
            {
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(TickStyle));
            }

            if (_tickStyle == value)
            {
                return;
            }

            _tickStyle = value;
            RecreateHandle();
        }
    }

    /// <summary>
    ///  Indicates just how many ticks will be drawn. For a TrackBar with a
    ///  range of 0..100, it might be impractical to draw all 100 ticks for a
    ///  very small control. Passing in a value of 5 here would only draw
    ///  20 ticks -- i.e. each tick would represent 5 units in the TrackBars
    ///  range of values.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(1)]
    [SRDescription(nameof(SR.TrackBarTickFrequencyDescr))]
    public int TickFrequency
    {
        get => _tickFrequency;
        set
        {
            if (_tickFrequency == value)
            {
                return;
            }

            _tickFrequency = value;
            // Determine if the decision of whether the ticks drawing,
            // is performed by the native control or the Windows Forms runtime
            // is still valid. If it's no longer valid, we'll need to recreate the native control.
            bool recreateHandle = ShouldRecreateHandle();
            if (IsHandleCreated)
            {
                PInvoke.SendMessage(this, PInvoke.TBM_SETTICFREQ, (WPARAM)value);
                // If user opts out of TrackBarModernRendering then recreateHandle
                // will always be false.
                if (recreateHandle)
                {
                    RecreateHandle();
                }
                else
                {
                    DrawTicks();
                    Invalidate();
                }
            }
        }
    }

    /// <summary>
    ///  The current location of the TrackBar thumb. This value must be between
    ///  the lower and upper limits of the TrackBar range.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(0)]
    [Bindable(true)]
    [SRDescription(nameof(SR.TrackBarValueDescr))]
    public int Value
    {
        get
        {
            GetTrackBarValue();
            return _value;
        }
        set
        {
            if (value == _value)
            {
                return;
            }

            if (!_initializing && ((value < _minimum) || (value > _maximum)))
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidBoundArgument, nameof(Value), value, $"'{nameof(Minimum)}'", $"'${nameof(Maximum)}'"));
            }

            _value = value;
            SetTrackBarPosition();
            OnValueChanged(EventArgs.Empty);
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? Click
    {
        add => base.Click += value;
        remove => base.Click -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? DoubleClick
    {
        add => base.DoubleClick += value;
        remove => base.DoubleClick -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event MouseEventHandler? MouseClick
    {
        add => base.MouseClick += value;
        remove => base.MouseClick -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event MouseEventHandler? MouseDoubleClick
    {
        add => base.MouseDoubleClick += value;
        remove => base.MouseDoubleClick -= value;
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
    public event EventHandler? RightToLeftLayoutChanged
    {
        add => Events.AddHandler(s_rightToLeftChangedEvent, value);
        remove => Events.RemoveHandler(s_rightToLeftChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TrackBarOnScrollDescr))]
    public event EventHandler? Scroll
    {
        add => Events.AddHandler(s_scrollEvent, value);
        remove => Events.RemoveHandler(s_scrollEvent, value);
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event PaintEventHandler? Paint
    {
        add => base.Paint += value;
        remove => base.Paint -= value;
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.valueChangedEventDescr))]
    public event EventHandler? ValueChanged
    {
        add => Events.AddHandler(s_valueChangedEvent, value);
        remove => Events.RemoveHandler(s_valueChangedEvent, value);
    }

    private void AdjustSize()
    {
        if (!IsHandleCreated)
        {
            return;
        }

        int saveDim = _requestedDim;
        try
        {
            if (_orientation == Orientation.Horizontal)
            {
                Height = _autoSize ? PreferredDimension : saveDim;
            }
            else
            {
                Width = _autoSize ? PreferredDimension : saveDim;
            }
        }
        finally
        {
            _requestedDim = saveDim;
        }
    }

    /// <summary>
    ///  Handles tasks required when the control is being initialized.
    /// </summary>
    public void BeginInit()
    {
        _initializing = true;
    }

    /// <summary>
    ///  Constrain the current value of the control to be within the minimum and maximum.
    /// </summary>
    private void ConstrainValue()
    {
        // Don't constrain the value while we're initializing the control
        if (_initializing)
        {
            return;
        }

        Debug.Assert(_minimum <= _maximum, "Minimum should be <= Maximum");

        // Keep the current value within the minimum and maximum
        if (Value < _minimum)
        {
            Value = _minimum;
        }

        if (Value > _maximum)
        {
            Value = _maximum;
        }
    }

    protected override AccessibleObject CreateAccessibilityInstance() => new TrackBarAccessibleObject(this);

    protected override unsafe void CreateHandle()
    {
        if (!RecreatingHandle)
        {
            using ThemingScope scope = new(Application.UseVisualStyles);
            PInvoke.InitCommonControlsEx(new INITCOMMONCONTROLSEX
            {
                dwSize = (uint)sizeof(INITCOMMONCONTROLSEX),
                dwICC = INITCOMMONCONTROLSEX_ICC.ICC_BAR_CLASSES
            });
        }

        base.CreateHandle();
    }

    /// <summary>
    ///  Check if the value of the max is greater then the taskbar size.
    ///  If so then we divide the value by size and only that many ticks to be drawn on the screen.
    /// </summary>
    private void DrawTicks()
    {
        // Will be true if they opt out TrackBarModernRendering.
        if (_tickStyle == TickStyle.None || _autoDrawTicks)
        {
            return;
        }

        int drawnTickFrequency = _tickFrequency;
        // Divide by 2 because otherwise the ticks appear as a solid line.
        int maxTickCount = (Orientation == Orientation.Horizontal ? Size.Width : Size.Height) / 2;
        uint range = (uint)(_maximum - _minimum);
        if (range > maxTickCount && maxTickCount != 0)
        {
            int calculatedTickFrequency = (int)(range / maxTickCount);
            if (calculatedTickFrequency > drawnTickFrequency)
            {
                drawnTickFrequency = calculatedTickFrequency;
            }
        }

        PInvoke.SendMessage(this, PInvoke.TBM_CLEARTICS, (WPARAM)1, (LPARAM)0);
        for (int i = _minimum + drawnTickFrequency; i < _maximum - drawnTickFrequency; i += drawnTickFrequency)
        {
            LRESULT lresult = PInvoke.SendMessage(this, PInvoke.TBM_SETTIC, lParam: (IntPtr)i);
            Debug.Assert((bool)(BOOL)lresult);
        }
    }

    /// <summary>
    ///  Called when initialization of the control is complete.
    /// </summary>
    public void EndInit()
    {
        _initializing = false;

        // Make sure the value is constrained by the minimum and maximum
        ConstrainValue();
    }

    private void GetTrackBarValue()
    {
        if (IsHandleCreated)
        {
            _value = (int)PInvoke.SendMessage(this, PInvoke.WM_USER);

            // See SetTrackBarValue() for a description of why we sometimes reflect the trackbar value
            if (_orientation == Orientation.Vertical)
            {
                // Reflect value
                _value = Minimum + Maximum - _value;
            }

            // Reflect for a RightToLeft horizontal trackbar
            if (_orientation == Orientation.Horizontal && RightToLeft == RightToLeft.Yes && !IsMirrored)
            {
                _value = Minimum + Maximum - _value;
            }
        }
    }

    /// <summary>
    ///  Handling special input keys, such as PageUp, PageDown, Home, End, etc.
    /// </summary>
    protected override bool IsInputKey(Keys keyData)
    {
        if ((keyData & Keys.Alt) == Keys.Alt)
        {
            return false;
        }

        return (keyData & Keys.KeyCode) switch
        {
            Keys.PageUp or Keys.PageDown or Keys.Home or Keys.End => true,
            _ => base.IsInputKey(keyData),
        };
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (!IsHandleCreated)
        {
            return;
        }

        Debug.Assert(_autoDrawTicks == ShouldAutoDrawTicks());
        PInvoke.SendMessage(this, PInvoke.TBM_SETRANGEMIN, (WPARAM)(BOOL)false, (LPARAM)_minimum);
        PInvoke.SendMessage(this, PInvoke.TBM_SETRANGEMAX, (WPARAM)(BOOL)false, (LPARAM)_maximum);
        PInvoke.SendMessage(this, PInvoke.TBM_SETTICFREQ, (WPARAM)_tickFrequency);
        DrawTicks();
        PInvoke.SendMessage(this, PInvoke.TBM_SETPAGESIZE, (WPARAM)0, (LPARAM)_largeChange);
        PInvoke.SendMessage(this, PInvoke.TBM_SETLINESIZE, (WPARAM)0, (LPARAM)_smallChange);
        SetTrackBarPosition();
        AdjustSize();
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnRightToLeftLayoutChanged(EventArgs e)
    {
        if (GetAnyDisposingInHierarchy())
        {
            return;
        }

        if (RightToLeft == RightToLeft.Yes)
        {
            RecreateHandle();
        }

        if (Events[s_rightToLeftChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    /// <summary>
    ///  Actually fires the "scroll" event. Inheriting classes should override
    ///  this method in favor of actually adding an EventHandler for this event.
    ///  Inheriting classes should not forget to call base.OnScroll(e)
    /// </summary>
    protected virtual void OnScroll(EventArgs e)
    {
        ((EventHandler?)Events[s_scrollEvent])?.Invoke(this, e);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
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

        Debug.Assert(_cumulativeWheelData > -PInvoke.WHEEL_DELTA, "cumulativeWheelData is too small");
        Debug.Assert(_cumulativeWheelData < PInvoke.WHEEL_DELTA, "cumulativeWheelData is too big");
        _cumulativeWheelData += e.Delta;

        float partialNotches;
        partialNotches = _cumulativeWheelData / (float)PInvoke.WHEEL_DELTA;

        if (wheelScrollLines == -1)
        {
            wheelScrollLines = TickFrequency;
        }

        // Evaluate number of bands to scroll
        int scrollBands = (int)(wheelScrollLines * partialNotches);

        if (scrollBands != 0)
        {
            int absScrollBands;
            if (scrollBands > 0)
            {
                absScrollBands = scrollBands;
                Value = Math.Min(absScrollBands + Value, Maximum);
                _cumulativeWheelData -= (int)(scrollBands * (PInvoke.WHEEL_DELTA / (float)wheelScrollLines));
            }
            else
            {
                absScrollBands = -scrollBands;
                Value = Math.Max(Value - absScrollBands, Minimum);
                _cumulativeWheelData -= (int)(scrollBands * (PInvoke.WHEEL_DELTA / (float)wheelScrollLines));
            }
        }

        if (e.Delta != Value)
        {
            OnScroll(EventArgs.Empty);
            OnValueChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    ///  Actually fires the "ValueChanged" event.
    /// </summary>
    protected virtual void OnValueChanged(EventArgs e)
    {
        if (IsAccessibilityObjectCreated)
        {
            using var nameVariant = (VARIANT)Name;
            AccessibilityObject.RaiseAutomationPropertyChangedEvent(UIA_PROPERTY_ID.UIA_ValueValuePropertyId, nameVariant, nameVariant);
            AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationPropertyChangedEventId);
        }

        ((EventHandler?)Events[s_valueChangedEvent])?.Invoke(this, e);
    }

    protected override void OnBackColorChanged(EventArgs e)
    {
        base.OnBackColorChanged(e);
        RedrawControl();
    }

    protected override void OnSystemColorsChanged(EventArgs e)
    {
        base.OnSystemColorsChanged(e);
        RedrawControl();
    }

    /// <summary>
    ///  Overrides Control.SetBoundsCore to enforce auto sizing.
    /// </summary>
    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        // Sets the height for a control in designer. We should obey the requested
        // height is AutoSize is false.
        _requestedDim = (_orientation == Orientation.Horizontal) ? height : width;

        if (_autoSize)
        {
            if (_orientation == Orientation.Horizontal)
            {
                if ((specified & BoundsSpecified.Height) != BoundsSpecified.None)
                {
                    height = PreferredDimension;
                }
            }
            else
            {
                if ((specified & BoundsSpecified.Width) != BoundsSpecified.None)
                {
                    width = PreferredDimension;
                }
            }
        }

        base.SetBoundsCore(x, y, width, height, specified);
    }

    /// <summary>
    ///  Lets you set the entire range for the TrackBar control at once.
    ///  The values passed are both the lower and upper limits to the range
    ///  with which the control will work.
    /// </summary>
    public void SetRange(int minValue, int maxValue)
    {
        if (_minimum != minValue || _maximum != maxValue)
        {
            // The Minimum and Maximum properties contain the logic for
            // ensuring that minValue <= maxValue. It is possible, however,
            // that this function will be called somewhere other than from
            // these two properties, so we'll check that here anyway.
            if (minValue > maxValue)
            {
                // We'll just adjust maxValue to match minValue
                maxValue = minValue;
            }

            _minimum = minValue;
            _maximum = maxValue;
            // Determine if the decision of whether the ticks drawing,
            // is performed by the native control or the Windows Forms runtime
            // is still valid. If it's no longer valid, we'll need to recreate the native control.
            bool recreateHandle = ShouldRecreateHandle();
            // If user opts out of TrackBarModernRendering then recreateHandle
            // will always be false.
            if (IsHandleCreated && !recreateHandle)
            {
                PInvoke.SendMessage(this, PInvoke.TBM_SETRANGEMIN, (WPARAM)(BOOL)false, (LPARAM)_minimum);
                PInvoke.SendMessage(this, PInvoke.TBM_SETRANGEMAX, (WPARAM)(BOOL)true, (LPARAM)_maximum);
                if (!_autoDrawTicks)
                {
                    DrawTicks();
                }

                Invalidate();
            }

            // When we change the range, the comctl32 trackbar's internal position can change
            // (because of the reflection that occurs with vertical trackbars)
            // so we make sure to explicitly set the trackbar position.
            if (_value < _minimum)
            {
                _value = _minimum;
            }

            if (_value > _maximum)
            {
                _value = _maximum;
            }

            // If user opts out of TrackBarModernRendering then recreateHandle
            // will always be false.
            if (recreateHandle)
            {
                RecreateHandle();
            }
            else
            {
                SetTrackBarPosition();
            }
        }
    }

    private void SetTrackBarPosition()
    {
        if (IsHandleCreated)
        {
            // There are two situations where we want to reflect the track bar position:
            //
            // 1. For a vertical trackbar, it seems to make more sense for the trackbar to increase in value
            //    as the slider moves up the trackbar (this is opposite what the underlying winctl control does)
            // 2. For a RightToLeft horizontal trackbar, we want to reflect the position.
            int reflectedValue = _value;

            // 1. Reflect for a vertical trackbar
            if (_orientation == Orientation.Vertical)
            {
                reflectedValue = Minimum + Maximum - _value;
            }

            // 2. Reflect for a RightToLeft horizontal trackbar
            if (_orientation == Orientation.Horizontal && RightToLeft == RightToLeft.Yes && !IsMirrored)
            {
                reflectedValue = Minimum + Maximum - _value;
            }

            PInvoke.SendMessage(this, PInvoke.TBM_SETPOS, (WPARAM)(BOOL)true, (LPARAM)reflectedValue);
        }
    }

    /// <summary>
    /// This checks all the use cases that we potentially might want to keep `TBS_AUTOTICKS`.
    /// </summary>
    private bool ShouldAutoDrawTicks()
    {
        // If the user decides to opt out of TrackBarModernRendering for drawing ticks,
        // by returning true autoticks will be set to true which will
        // use the old way of rendering ticks on the trackbar.
        // TBS_AUTOTICKS will be enabled and sending
        // the TBM_SETTICFREQ message to set tick frequency.
        if (!LocalAppContextSwitches.TrackBarModernRendering)
        {
            return true;
        }

        if (TickStyle == TickStyle.None)
        {
            return true;
        }

        int size = Orientation == Orientation.Horizontal ? Size.Width : Size.Height;
        if (size == 0)
        {
            return true;
        }

        uint range = (uint)(_maximum - _minimum);
        return range <= (size / 2);
    }

    /// <summary>
    /// Determine if the decision of whether the ticks drawing,
    /// is performed by the native control or the Windows Forms runtime
    /// is still valid. If it's no longer valid, we'll need to recreate the native control.
    /// If user opts out of TrackBarModernRendering then this will always return false.
    /// </summary>
    private bool ShouldRecreateHandle() => IsHandleCreated && _autoDrawTicks != ShouldAutoDrawTicks();

    public override string ToString() => $"{base.ToString()}, Minimum: {Minimum}, Maximum: {Maximum}, Value: {_value}";

    protected override void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case MessageId.WM_REFLECT_HSCROLL:
            case MessageId.WM_REFLECT_VSCROLL:
                switch ((uint)m.WParamInternal.LOWORD)
                {
                    case PInvoke.TB_LINEUP:
                    case PInvoke.TB_LINEDOWN:
                    case PInvoke.TB_PAGEUP:
                    case PInvoke.TB_PAGEDOWN:
                    case PInvoke.TB_THUMBTRACK:
                    case PInvoke.TB_TOP:
                    case PInvoke.TB_BOTTOM:
                    case PInvoke.TB_ENDTRACK:
                        if (_value != Value)
                        {
                            OnScroll(EventArgs.Empty);
                            OnValueChanged(EventArgs.Empty);
                        }

                        break;
                }

                break;
            default:
                base.WndProc(ref m);
                break;
        }
    }
}
