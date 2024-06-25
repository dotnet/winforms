// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;
using Microsoft.Win32;

namespace System.Windows.Forms;

/// <summary>
///  Represents a Windows progress bar control.
/// </summary>
[DefaultProperty(nameof(Value))]
[DefaultBindingProperty(nameof(Value))]
[SRDescription(nameof(SR.DescriptionProgressBar))]
public partial class ProgressBar : Control
{
    // These four values define the range of possible values, how to navigate through them and the
    // current position
    private int _minimum;
    private int _maximum = 100;
    private int _step = 10;
    private int _value;

    private int _marqueeAnimationSpeed = 100;

    private static readonly Color s_defaultForeColor = Application.ApplicationColors.Highlight;

    private ProgressBarStyle _style = ProgressBarStyle.Blocks;

    private EventHandler? _onRightToLeftLayoutChanged;
    private bool _rightToLeftLayout;

    /// <summary>
    ///  Initializes a new instance of the <see cref="ProgressBar"/> class in its default state.
    /// </summary>
    public ProgressBar() : base()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.UseTextForAccessibility | ControlStyles.Selectable, false);
        ForeColor = s_defaultForeColor;
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ClassName = PInvoke.PROGRESS_CLASS;
            if (Style == ProgressBarStyle.Continuous)
            {
                cp.Style |= (int)PInvoke.PBS_SMOOTH;
            }
            else if (Style == ProgressBarStyle.Marquee && !DesignMode)
            {
                cp.Style |= (int)PInvoke.PBS_MARQUEE;
            }

            if (RightToLeft == RightToLeft.Yes && RightToLeftLayout)
            {
                // We want to turn on mirroring for Form explicitly.
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_LAYOUTRTL;

                // Don't need these styles when mirroring is turned on.
                cp.ExStyle &= ~(int)(WINDOW_EX_STYLE.WS_EX_RTLREADING | WINDOW_EX_STYLE.WS_EX_RIGHT | WINDOW_EX_STYLE.WS_EX_LEFTSCROLLBAR);
            }

            return cp;
        }
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();

        // If DarkMode is enabled, we need to disable the Visual Styles
        // so Windows allows setting Fore- and Background color.
        // There are more ideal ways imaginable, but this does the trick for now.
        if (IsDarkModeEnabled)
        {
            // Disables Visual Styles for the ProgressBar.
            PInvoke.SetWindowTheme(HWND, " ", " ");
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool AllowDrop
    {
        get => base.AllowDrop;
        set => base.AllowDrop = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Image? BackgroundImage
    {
        get => base.BackgroundImage;
        set => base.BackgroundImage = value;
    }

    /// <summary>
    ///  Gets or sets the style of the ProgressBar. This is can be either Blocks or Continuous.
    /// </summary>
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DefaultValue(ProgressBarStyle.Blocks)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ProgressBarStyleDescr))]
    public ProgressBarStyle Style
    {
        get => _style;
        set
        {
            if (_style != value)
            {
                SourceGenerated.EnumValidator.Validate(value);

                _style = value;
                if (IsHandleCreated)
                {
                    RecreateHandle();
                }

                if (_style == ProgressBarStyle.Marquee)
                {
                    StartMarquee();
                }
            }
        }
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

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool CausesValidation
    {
        get => base.CausesValidation;
        set => base.CausesValidation = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? CausesValidationChanged
    {
        add => base.CausesValidationChanged += value;
        remove => base.CausesValidationChanged -= value;
    }

    protected override ImeMode DefaultImeMode => ImeMode.Disable;

    protected override Size DefaultSize => new(100, 23);

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
    ///  Gets or sets the marquee animation speed of the <see cref="ProgressBar"/>.
    ///  Sets the value to a positive number causes the progressBar to move, while setting it to 0
    ///  stops the ProgressBar.
    /// </summary>
    [DefaultValue(100)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ProgressBarMarqueeAnimationSpeed))]
    public int MarqueeAnimationSpeed
    {
        get => _marqueeAnimationSpeed;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            _marqueeAnimationSpeed = value;
            if (!DesignMode)
            {
                StartMarquee();
            }
        }
    }

    /// <summary>
    ///  Start the Marquee rolling (or stop it, if the speed = 0)
    /// </summary>
    private void StartMarquee()
    {
        if (IsHandleCreated && _style == ProgressBarStyle.Marquee)
        {
            if (_marqueeAnimationSpeed == 0)
            {
                PInvoke.SendMessage(this, PInvoke.PBM_SETMARQUEE, (WPARAM)(BOOL)false, (LPARAM)_marqueeAnimationSpeed);
            }
            else
            {
                PInvoke.SendMessage(this, PInvoke.PBM_SETMARQUEE, (WPARAM)(BOOL)true, (LPARAM)_marqueeAnimationSpeed);
            }
        }
    }

    /// <summary>
    ///  Gets or sets the maximum value of the <see cref="ProgressBar"/>.
    /// </summary>
    [DefaultValue(100)]
    [SRCategory(nameof(SR.CatBehavior))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [SRDescription(nameof(SR.ProgressBarMaximumDescr))]
    public int Maximum
    {
        get => _maximum;
        set
        {
            if (_maximum != value)
            {
                // Ensure that value is in the Win32 control's acceptable range
                ArgumentOutOfRangeException.ThrowIfNegative(value);

                if (_minimum > value)
                {
                    _minimum = value;
                }

                _maximum = value;

                if (_value > _maximum)
                {
                    _value = _maximum;
                }

                if (IsHandleCreated)
                {
                    PInvoke.SendMessage(this, PInvoke.PBM_SETRANGE32, (WPARAM)_minimum, (LPARAM)_maximum);
                    UpdatePos();
                }
            }
        }
    }

    /// <summary>
    ///  Gets or sets the minimum value of the <see cref="ProgressBar"/>.
    /// </summary>
    [DefaultValue(0)]
    [SRCategory(nameof(SR.CatBehavior))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [SRDescription(nameof(SR.ProgressBarMinimumDescr))]
    public int Minimum
    {
        get => _minimum;
        set
        {
            if (_minimum != value)
            {
                // Ensure that value is in the Win32 control's acceptable range
                ArgumentOutOfRangeException.ThrowIfNegative(value);

                if (_maximum < value)
                {
                    _maximum = value;
                }

                _minimum = value;

                if (_value < _minimum)
                {
                    _value = _minimum;
                }

                if (IsHandleCreated)
                {
                    PInvoke.SendMessage(this, PInvoke.PBM_SETRANGE32, (WPARAM)_minimum, (LPARAM)_maximum);
                    UpdatePos();
                }
            }
        }
    }

    protected override void OnBackColorChanged(EventArgs e)
    {
        base.OnBackColorChanged(e);
        if (IsHandleCreated)
        {
            PInvoke.SendMessage(this, PInvoke.PBM_SETBKCOLOR, 0, BackColor.ToWin32());
        }
    }

    protected override void OnForeColorChanged(EventArgs e)
    {
        base.OnForeColorChanged(e);
        if (IsHandleCreated)
        {
            PInvoke.SendMessage(this, PInvoke.PBM_SETBARCOLOR, 0, ForeColor.ToWin32());
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
    ///  This is used for international applications where the language is written from RightToLeft.
    ///  When this property is true, and the RightToLeft is true, mirroring will be turned on on
    ///  the form, and control placement and text will be from right to left.
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
            if (value != _rightToLeftLayout)
            {
                _rightToLeftLayout = value;
                using (new LayoutTransaction(this, this, PropertyNames.RightToLeftLayout))
                {
                    OnRightToLeftLayoutChanged(EventArgs.Empty);
                }
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
    public event EventHandler? RightToLeftLayoutChanged
    {
        add => _onRightToLeftLayoutChanged += value;
        remove => _onRightToLeftLayoutChanged -= value;
    }

    /// <summary>
    ///  Gets or sets the amount that a call to <see cref="PerformStep"/> increases the progress
    ///  bar's current position.
    /// </summary>
    [DefaultValue(10)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ProgressBarStepDescr))]
    public int Step
    {
        get => _step;
        set
        {
            _step = value;
            if (IsHandleCreated)
            {
                PInvoke.SendMessage(this, PInvoke.PBM_SETSTEP, (WPARAM)_step);
            }
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool TabStop
    {
        get => base.TabStop;
        set => base.TabStop = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TabStopChanged
    {
        add => base.TabStopChanged += value;
        remove => base.TabStopChanged -= value;
    }

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
    ///  Gets or sets the current position of the <see cref="ProgressBar"/>.
    /// </summary>
    [DefaultValue(0)]
    [SRCategory(nameof(SR.CatBehavior))]
    [Bindable(true)]
    [SRDescription(nameof(SR.ProgressBarValueDescr))]
    public int Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                if (value < _minimum || value > _maximum)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidBoundArgument, nameof(Value), value, nameof(Minimum), nameof(Maximum)));
                }

                _value = value;
                UpdatePos();
            }
        }
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
    public new event MouseEventHandler? MouseDoubleClick
    {
        add => base.MouseDoubleClick += value;
        remove => base.MouseDoubleClick -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event KeyEventHandler? KeyUp
    {
        add => base.KeyUp += value;
        remove => base.KeyUp -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event KeyEventHandler? KeyDown
    {
        add => base.KeyDown += value;
        remove => base.KeyDown -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event KeyPressEventHandler? KeyPress
    {
        add => base.KeyPress += value;
        remove => base.KeyPress -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? Enter
    {
        add => base.Enter += value;
        remove => base.Enter -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? Leave
    {
        add => base.Leave += value;
        remove => base.Leave -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event PaintEventHandler? Paint
    {
        add => base.Paint += value;
        remove => base.Paint -= value;
    }

    protected override unsafe void CreateHandle()
    {
        if (!RecreatingHandle)
        {
            using ThemingScope scope = new(Application.UseVisualStyles);
            PInvoke.InitCommonControlsEx(new INITCOMMONCONTROLSEX()
            {
                dwSize = (uint)sizeof(INITCOMMONCONTROLSEX),
                dwICC = INITCOMMONCONTROLSEX_ICC.ICC_PROGRESS_CLASS
            });
        }

        base.CreateHandle();
    }

    /// <summary>
    ///  Advances the current position of the <see cref="ProgressBar"/> by the specified increment
    ///  and redraws the control to reflect the new position.
    /// </summary>
    public void Increment(int value)
    {
        if (Style == ProgressBarStyle.Marquee)
        {
            throw new InvalidOperationException(SR.ProgressBarIncrementMarqueeException);
        }

        _value += value;

        // Enforce that value is within the range (minimum, maximum)
        if (_value < _minimum)
        {
            _value = _minimum;
        }

        if (_value > _maximum)
        {
            _value = _maximum;
        }

        UpdatePos();
    }

    /// <summary>
    ///  Overridden to set up our properties.
    /// </summary>
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (IsHandleCreated)
        {
            PInvoke.SendMessage(this, PInvoke.PBM_SETRANGE32, (WPARAM)_minimum, (LPARAM)_maximum);
            PInvoke.SendMessage(this, PInvoke.PBM_SETSTEP, (WPARAM)_step);
            PInvoke.SendMessage(this, PInvoke.PBM_SETPOS, (WPARAM)_value);
            PInvoke.SendMessage(this, PInvoke.PBM_SETBKCOLOR, (WPARAM)0, (LPARAM)BackColor);
            PInvoke.SendMessage(this, PInvoke.PBM_SETBARCOLOR, (WPARAM)0, (LPARAM)ForeColor);
        }

        StartMarquee();
        SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(UserPreferenceChangedHandler);
    }

    /// <summary>
    ///  Overridden to remove event handler.
    /// </summary>
    protected override void OnHandleDestroyed(EventArgs e)
    {
        SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(UserPreferenceChangedHandler);
        base.OnHandleDestroyed(e);
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

        _onRightToLeftLayoutChanged?.Invoke(this, e);
    }

    /// <summary>
    ///  Advances the current position of the <see cref="ProgressBar"/> by the amount of the
    ///  <see cref="Step"/> property, and redraws the control to reflect the new position.
    /// </summary>
    public void PerformStep()
    {
        if (Style == ProgressBarStyle.Marquee)
        {
            throw new InvalidOperationException(SR.ProgressBarPerformStepMarqueeException);
        }

        Increment(_step);
    }

    /// <summary>
    ///  Resets the fore color to be based on the parent's fore color.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override void ResetForeColor()
    {
        ForeColor = s_defaultForeColor;
    }

    /// <summary>
    ///  Returns true if the ForeColor should be persisted in code gen.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal override bool ShouldSerializeForeColor()
    {
        return ForeColor != s_defaultForeColor;
    }

    internal override bool SupportsUiaProviders => true;

    /// <summary>
    ///  Returns a string representation for this control.
    /// </summary>
    public override string ToString()
        => $"{base.ToString()}, Minimum: {Minimum}, Maximum: {Maximum}, Value: {_value}";

    /// <summary>
    ///  Sends the underlying window a PBM_SETPOS message to update the current value of the
    ///  <see cref="ProgressBar"/>.
    /// </summary>
    private void UpdatePos()
    {
        if (IsHandleCreated)
        {
            PInvoke.SendMessage(this, PInvoke.PBM_SETPOS, (WPARAM)_value);
        }
    }

    /// <remarks>
    ///  <para>
    ///   Note: <see cref="ProgressBar"/> doesn't work like other controls as far as setting ForeColor/BackColor.
    ///   You need to send messages to update the colors.
    ///  </para>
    /// </remarks>
    private void UserPreferenceChangedHandler(object o, UserPreferenceChangedEventArgs e)
    {
        if (IsHandleCreated)
        {
            PInvoke.SendMessage(this, PInvoke.PBM_SETBARCOLOR, 0, ForeColor.ToWin32());
            PInvoke.SendMessage(this, PInvoke.PBM_SETBKCOLOR, 0, BackColor.ToWin32());
        }
    }

    /// <summary>
    ///  Creates a new AccessibleObject for this <see cref="ProgressBar"/> instance.
    ///  The AccessibleObject instance returned by this method supports ControlType UIA property.
    /// </summary>
    /// <returns>
    ///  AccessibleObject for this <see cref="ProgressBar"/> instance.
    /// </returns>
    protected override AccessibleObject CreateAccessibilityInstance()
        => new ProgressBarAccessibleObject(this);
}
