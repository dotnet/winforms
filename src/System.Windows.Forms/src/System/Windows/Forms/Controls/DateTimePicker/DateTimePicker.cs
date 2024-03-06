// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms.Layout;
using Microsoft.Win32;
using SourceGenerated;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

/// <summary>
///  Date/DateTime picker control.
/// </summary>
[DefaultProperty(nameof(Value))]
[DefaultEvent(nameof(ValueChanged))]
[DefaultBindingProperty(nameof(Value))]
[Designer($"System.Windows.Forms.Design.DateTimePickerDesigner, {AssemblyRef.SystemDesign}")]
[SRDescription(nameof(SR.DescriptionDateTimePicker))]
public partial class DateTimePicker : Control
{
    /// <summary>
    ///  Specifies the default title back color. This field is read-only.
    /// </summary>
    protected static readonly Color DefaultTitleBackColor = Application.SystemColors.ActiveCaption;

    /// <summary>
    ///  Specifies the default foreground color. This field is read-only.
    /// </summary>
    protected static readonly Color DefaultTitleForeColor = Application.SystemColors.ActiveCaptionText;

    /// <summary>
    ///  Specifies the default month background color. This field is read-only.
    /// </summary>
    protected static readonly Color DefaultMonthBackColor = Application.SystemColors.Window;

    /// <summary>
    ///  Specifies the default trailing foreground color. This field is read-only.
    /// </summary>
    protected static readonly Color DefaultTrailingForeColor = Application.SystemColors.GrayText;

    private static readonly object s_formatChangedEvent = new();

    private static readonly string s_dateTimePickerLocalizedControlTypeString = SR.DateTimePickerLocalizedControlType;

    private const uint TIMEFORMAT_NOUPDOWN = PInvoke.DTS_TIMEFORMAT & (~PInvoke.DTS_UPDOWN);
    private EventHandler? _onCloseUp;
    private EventHandler? _onDropDown;
    private EventHandler? _onValueChanged;
    private EventHandler? _onRightToLeftLayoutChanged;

    private ExpandCollapseState _expandCollapseState;

    // We need to restrict the available dates because of limitations in the comctl DateTime and MonthCalendar controls

    /// <summary>
    ///  Specifies the minimum date value. This field is read-only.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly DateTime MinDateTime = new(1753, 1, 1);

    /// <summary>
    ///  Specifies the maximum date value. This field is read-only.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly DateTime MaxDateTime = new(9998, 12, 31);

    private uint _style;
    private short _prefHeightCache = -1;

    /// <summary>
    ///  Determines whether the CheckBox in the DTP is checked. The CheckBox is only
    ///  displayed when ShowCheckBox is true.
    /// </summary>
    private bool _validTime = true;

    // DateTime changeover: DateTime is a value class, not an object, so we need to keep track
    // of whether or not its values have been initialized in a separate boolean.
    private bool _userHasSetValue;
    private DateTime _value = DateTime.Now;
    private DateTime _creationTime = DateTime.Now;
    // Reconcile out-of-range min/max values in the property getters.
    private DateTime _maxDateTime = DateTime.MaxValue;
    private DateTime _minDateTime = DateTime.MinValue;
    private Color _calendarForeColor = DefaultForeColor;
    private Color _calendarTitleBackColor = DefaultTitleBackColor;
    private Color _calendarTitleForeColor = DefaultTitleForeColor;
    private Color _calendarMonthBackground = DefaultMonthBackColor;
    private Color _calendarTrailingText = DefaultTrailingForeColor;
    private Font? _calendarFont;
    private FontHandleWrapper? _calendarFontHandleWrapper;

    // Since there is no way to get the customFormat from the DTP, we need to cache it. Also we have to track if
    // the user wanted customFormat or shortDate format (shortDate is the lack of being in Long or DateTime format
    // without a customFormat).
    private string? _customFormat;

    private DateTimePickerFormat _format;

    private bool _rightToLeftLayout;

    /// <summary>
    ///  Initializes a new instance of the <see cref="DateTimePicker"/> class.
    /// </summary>
    public DateTimePicker() : base()
    {
        // This class overrides GetPreferredSizeCore, let Control automatically cache the result.
        SetExtendedState(ExtendedStates.UserPreferredSizeCache, true);

        SetStyle(ControlStyles.FixedHeight, true);

        // Since DateTimePicker does its own mouse capturing, we do not want to receive standard click events, or
        // we end up with mismatched mouse button up and button down messages.
        SetStyle(ControlStyles.UserPaint | ControlStyles.StandardClick, false);

        _format = DateTimePickerFormat.Long;

        SetStyle(ControlStyles.UseTextForAccessibility, false);
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Color BackColor
    {
        get => ShouldSerializeBackColor() || IsDarkModeEnabled ? base.BackColor : Application.SystemColors.Window;
        set => base.BackColor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackColorChanged
    {
        add => base.BackColorChanged += value;
        remove => base.BackColorChanged -= value;
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

    /// <summary>
    ///  The current value of the CalendarForeColor property.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DateTimePickerCalendarForeColorDescr))]
    public Color CalendarForeColor
    {
        get => _calendarForeColor;
        set
        {
            if (value.IsEmpty)
            {
                throw new ArgumentException(string.Format(SR.InvalidNullArgument, nameof(value)));
            }

            if (!value.Equals(_calendarForeColor))
            {
                _calendarForeColor = value;
                SetControlColor(PInvoke.MCSC_TEXT, value);
            }
        }
    }

    /// <summary>
    ///  The current value of the CalendarFont property.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [AmbientValue(null)]
    [AllowNull]
    [SRDescription(nameof(SR.DateTimePickerCalendarFontDescr))]
    public Font CalendarFont
    {
        get => _calendarFont ?? Font;
        set
        {
            if ((value is null && _calendarFont is not null) || (value is not null && !value.Equals(_calendarFont)))
            {
                _calendarFont = value;
                _calendarFontHandleWrapper = null;
                SetControlCalendarFont();
            }
        }
    }

    private HFONT CalendarFontHandle
    {
        get
        {
            if (_calendarFont is null)
            {
                Debug.Assert(_calendarFontHandleWrapper is null, "font handle out of sync with Font");
                return FontHandle;
            }

            _calendarFontHandleWrapper ??= new FontHandleWrapper(CalendarFont);
            return _calendarFontHandleWrapper.Handle;
        }
    }

    /// <summary>
    ///  The current value of the CalendarTitleBackColor property.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DateTimePickerCalendarTitleBackColorDescr))]
    public Color CalendarTitleBackColor
    {
        get => _calendarTitleBackColor;
        set
        {
            if (value.IsEmpty)
            {
                throw new ArgumentException(string.Format(SR.InvalidNullArgument, nameof(value)));
            }

            if (!value.Equals(_calendarTitleBackColor))
            {
                _calendarTitleBackColor = value;
                SetControlColor(PInvoke.MCSC_TITLEBK, value);
            }
        }
    }

    /// <summary>
    ///  The current value of the CalendarTitleForeColor property.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DateTimePickerCalendarTitleForeColorDescr))]
    public Color CalendarTitleForeColor
    {
        get => _calendarTitleForeColor;
        set
        {
            if (value.IsEmpty)
            {
                throw new ArgumentException(string.Format(SR.InvalidNullArgument, nameof(value)));
            }

            if (!value.Equals(_calendarTitleForeColor))
            {
                _calendarTitleForeColor = value;
                SetControlColor(PInvoke.MCSC_TITLETEXT, value);
            }
        }
    }

    /// <summary>
    ///  The current value of the CalendarTrailingForeColor property.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DateTimePickerCalendarTrailingForeColorDescr))]
    public Color CalendarTrailingForeColor
    {
        get => _calendarTrailingText;
        set
        {
            if (value.IsEmpty)
            {
                throw new ArgumentException(string.Format(SR.InvalidNullArgument, nameof(value)));
            }

            if (!value.Equals(_calendarTrailingText))
            {
                _calendarTrailingText = value;
                SetControlColor(PInvoke.MCSC_TRAILINGTEXT, value);
            }
        }
    }

    /// <summary>
    ///  The current value of the CalendarMonthBackground property.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DateTimePickerCalendarMonthBackgroundDescr))]
    public Color CalendarMonthBackground
    {
        get => _calendarMonthBackground;
        set
        {
            if (value.IsEmpty)
            {
                throw new ArgumentException(string.Format(SR.InvalidNullArgument, nameof(value)));
            }

            if (!value.Equals(_calendarMonthBackground))
            {
                _calendarMonthBackground = value;
                SetControlColor(PInvoke.MCSC_MONTHBK, value);
            }
        }
    }

    /// <summary>
    ///  Indicates whether the <see cref="Value"/> property has been set.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [Bindable(true)]
    [SRDescription(nameof(SR.DateTimePickerCheckedDescr))]
    public bool Checked
    {
        get
        {
            // The information from win32 DateTimePicker is reliable only when ShowCheckBoxes is True
            if (ShowCheckBox && IsHandleCreated)
            {
                SYSTEMTIME systemTime = default;
                nint result = PInvoke.SendMessage(this, PInvoke.DTM_GETSYSTEMTIME, 0, ref systemTime);
                return result == (nint)NMDATETIMECHANGE_FLAGS.GDT_VALID;
            }
            else
            {
                return _validTime;
            }
        }
        set
        {
            if (Checked != value)
            {
                // set the information into the win32 DateTimePicker only if ShowCheckBoxes is True
                if (ShowCheckBox && IsHandleCreated)
                {
                    if (value)
                    {
                        SYSTEMTIME systemTime = (SYSTEMTIME)_value;
                        PInvoke.SendMessage(this, PInvoke.DTM_SETSYSTEMTIME, (WPARAM)(uint)NMDATETIMECHANGE_FLAGS.GDT_VALID, ref systemTime);
                    }
                    else
                    {
                        PInvoke.SendMessage(this, PInvoke.DTM_SETSYSTEMTIME, (WPARAM)(uint)NMDATETIMECHANGE_FLAGS.GDT_NONE);
                    }
                }

                // this.validTime is used when the DateTimePicker receives date time change notification
                // from the Win32 control. this.validTime will be used to know when we transition from valid time to unvalid time
                // also, validTime will be used when ShowCheckBox == false
                _validTime = value;
            }
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? Click
    {
        add => base.Click += value;
        remove => base.Click -= value;
    }

    /// <summary>
    ///  Returns the CreateParams used to create this window.
    /// </summary>
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ClassName = PInvoke.DATETIMEPICK_CLASS;

            cp.Style |= (int)_style;

            switch (_format)
            {
                case DateTimePickerFormat.Long:
                    cp.Style |= (int)PInvoke.DTS_LONGDATEFORMAT;
                    break;
                case DateTimePickerFormat.Short:
                    break;
                case DateTimePickerFormat.Time:
                    cp.Style |= (int)TIMEFORMAT_NOUPDOWN;
                    break;
                case DateTimePickerFormat.Custom:
                    break;
            }

            cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_CLIENTEDGE;

            if (RightToLeft == RightToLeft.Yes && RightToLeftLayout)
            {
                // We want to turn on mirroring for DateTimePicker explicitly.
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_LAYOUTRTL;
                // Don't need these styles when mirroring is turned on.
                cp.ExStyle &= ~(int)(WINDOW_EX_STYLE.WS_EX_RTLREADING | WINDOW_EX_STYLE.WS_EX_RIGHT | WINDOW_EX_STYLE.WS_EX_LEFTSCROLLBAR);
            }

            return cp;
        }
    }

    /// <summary>
    ///  Returns the custom format.
    /// </summary>
    [DefaultValue(null)]
    [Localizable(true)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DateTimePickerCustomFormatDescr))]
    public string? CustomFormat
    {
        get => _customFormat;
        set
        {
            if ((value is not null && !value.Equals(_customFormat)) ||
                (value is null && _customFormat is not null))
            {
                _customFormat = value;

                if (IsHandleCreated)
                {
                    if (_format == DateTimePickerFormat.Custom)
                    {
                        PInvoke.SendMessage(this, PInvoke.DTM_SETFORMATW, 0, _customFormat);
                    }
                }
            }
        }
    }

    protected override Size DefaultSize => new(200, PreferredHeight);

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override bool DoubleBuffered
    {
        get => base.DoubleBuffered;
        set => base.DoubleBuffered = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? DoubleClick
    {
        add => base.DoubleClick += value;
        remove => base.DoubleClick -= value;
    }

    /// <summary>
    ///  The calendar dropdown can be aligned to the left or right of the control.
    /// </summary>
    [DefaultValue(LeftRightAlignment.Left)]
    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [SRDescription(nameof(SR.DateTimePickerDropDownAlignDescr))]
    public LeftRightAlignment DropDownAlign
    {
        get => (_style & PInvoke.DTS_RIGHTALIGN) != 0 ? LeftRightAlignment.Right : LeftRightAlignment.Left;
        set
        {
            // Valid values are 0x0 to 0x1
            EnumValidator.Validate(value);
            SetStyleBit(value == LeftRightAlignment.Right, PInvoke.DTS_RIGHTALIGN);
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Color ForeColor
    {
        get => ShouldSerializeForeColor() || IsDarkModeEnabled ? base.ForeColor : Application.SystemColors.WindowText;
        set => base.ForeColor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? ForeColorChanged
    {
        add => base.ForeColorChanged += value;
        remove => base.ForeColorChanged -= value;
    }

    /// <summary>
    ///  Returns the current value of the format property. This determines the
    ///  style of format the date is displayed in.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [SRDescription(nameof(SR.DateTimePickerFormatDescr))]
    public DateTimePickerFormat Format
    {
        get => _format;
        set
        {
            EnumValidator.Validate(value);

            if (_format != value)
            {
                _format = value;
                RecreateHandle();

                OnFormatChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.DateTimePickerOnFormatChangedDescr))]
    public event EventHandler? FormatChanged
    {
        add => Events.AddHandler(s_formatChangedEvent, value);
        remove => Events.RemoveHandler(s_formatChangedEvent, value);
    }

    /// <summary>
    ///  DateTimePicker Paint.
    /// </summary>
    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event PaintEventHandler? Paint
    {
        add => base.Paint += value;
        remove => base.Paint -= value;
    }

    // Make sure the passed in minDate respects the current culture: this
    // is especially important if the culture changes from a Gregorian or
    // other calendar with a lowish minDate (see comment on MinimumDateTime)
    // to a calendar, which has a minimum date of 1/1/1912.
    internal static DateTime EffectiveMinDate(DateTime minDate)
    {
        DateTime minSupportedDate = MinimumDateTime;
        if (minDate < minSupportedDate)
        {
            return minSupportedDate;
        }

        return minDate;
    }

    // Similarly, make sure the maxDate respects the current culture.  No
    // problems are anticipated here: I don't believe there are calendars
    // around that have max dates on them.  But if there are, we'll deal with
    // them correctly.
    internal static DateTime EffectiveMaxDate(DateTime maxDate)
    {
        DateTime maxSupportedDate = MaximumDateTime;
        if (maxDate > maxSupportedDate)
        {
            return maxSupportedDate;
        }

        return maxDate;
    }

    /// <summary>
    ///  Indicates the maximum date and time selectable in the control.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DateTimePickerMaxDateDescr))]
    public DateTime MaxDate
    {
        get
        {
            return EffectiveMaxDate(_maxDateTime);
        }
        set
        {
            if (value == _maxDateTime)
            {
                return;
            }

            ArgumentOutOfRangeException.ThrowIfLessThan(value, EffectiveMinDate(_minDateTime));

            if (value > MaximumDateTime)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    string.Format(SR.DateTimePickerMaxDate, FormatDateTime(MaxDateTime)));
            }

            _maxDateTime = value;
            SetRange();

            // If Value (which was once valid) is suddenly greater than the max (since we just set it) then adjust this.
            if (Value > _maxDateTime)
            {
                Value = _maxDateTime;
            }
        }
    }

    /// <summary>
    ///  Specifies the maximum date value. This property is read-only.
    /// </summary>
    public static DateTime MaximumDateTime
    {
        get
        {
            DateTime maxSupportedDateTime = CultureInfo.CurrentCulture.Calendar.MaxSupportedDateTime;
            if (maxSupportedDateTime.Year > MaxDateTime.Year)
            {
                return MaxDateTime;
            }

            return maxSupportedDateTime;
        }
    }

    /// <summary>
    ///  Indicates the minimum date and time selectable in the control.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DateTimePickerMinDateDescr))]
    public DateTime MinDate
    {
        get
        {
            return EffectiveMinDate(_minDateTime);
        }
        set
        {
            if (value == _minDateTime)
            {
                return;
            }

            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, EffectiveMaxDate(_maxDateTime));

            if (value < MinimumDateTime)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    string.Format(SR.DateTimePickerMinDate, FormatDateTime(MinimumDateTime)));
            }

            _minDateTime = value;
            SetRange();

            // If Value (which was once valid) is suddenly less than the min (since we just set it) then adjust this.
            if (Value < _minDateTime)
            {
                Value = _minDateTime;
            }
        }
    }

    // We restrict the available dates to >= 1753 because of oddness in the Gregorian calendar about
    // that time.  We do this even for cultures that don't use the Gregorian calendar -- we're not
    // really that worried about calendars for >250 years ago.

    /// <summary>
    ///  Specifies the minimum date value. This property is read-only.
    /// </summary>
    public static DateTime MinimumDateTime
    {
        get
        {
            DateTime minSupportedDateTime = CultureInfo.CurrentCulture.Calendar.MinSupportedDateTime;
            if (minSupportedDateTime.Year < 1753)
            {
                return new DateTime(1753, 1, 1);
            }

            return minSupportedDateTime;
        }
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
    ///  Indicates the preferred height of the DateTimePicker control. This property is read-only.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int PreferredHeight
    {
        get
        {
            if (_prefHeightCache > -1)
            {
                return _prefHeightCache;
            }

            // Base the preferred height on the current font
            int height = FontHeight;

            // Adjust for the border
            height += SystemInformation.BorderSize.Height * 4 + 3;
            _prefHeightCache = (short)height;

            return height;
        }
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

    /// <summary>
    ///  Indicates whether a check box is displayed to toggle the NoValueSelected property value.
    /// </summary>
    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DateTimePickerShowNoneDescr))]
    public bool ShowCheckBox
    {
        get => (_style & PInvoke.DTS_SHOWNONE) != 0;
        set => SetStyleBit(value, PInvoke.DTS_SHOWNONE);
    }

    /// <summary>
    ///  Indicates whether an up-down control is used to adjust the time values.
    /// </summary>
    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DateTimePickerShowUpDownDescr))]
    public bool ShowUpDown
    {
        get => (_style & PInvoke.DTS_UPDOWN) != 0;
        set
        {
            if (ShowUpDown != value)
            {
                SetStyleBit(value, PInvoke.DTS_UPDOWN);
            }
        }
    }

    internal override bool SupportsUiaProviders => true;

    /// <summary>
    ///  Overrides Text to allow for setting of the value via a string.  Also, returns
    ///  a formatted Value when getting the text.  The DateTime class will throw
    ///  an exception if the string (value) being passed in is invalid.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set
        {
            // Clause to check length
            if (value is null || value.Length == 0)
            {
                ResetValue();
            }
            else
            {
                Value = DateTime.Parse(value, CultureInfo.CurrentCulture);
            }
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event EventHandler? TextChanged
    {
        add => base.TextChanged += value;
        remove => base.TextChanged -= value;
    }

    /// <summary>
    ///  Indicates the DateTime value assigned to the control.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Bindable(true)]
    [RefreshProperties(RefreshProperties.All)]
    [SRDescription(nameof(SR.DateTimePickerValueDescr))]
    public DateTime Value
    {
        get
        {
            // Checkbox clicked, no value set - no value set state should never occur, but just in case.
            return !_userHasSetValue && _validTime ? _creationTime : _value;
        }
        set
        {
            bool valueChanged = !DateTime.Equals(Value, value);

            // Check for value set here; if we've not set the value yet, it'll be Now, so the second part of the
            // test will fail. So, if userHasSetValue isn't set, we don't care if the value is still the same -
            // and we'll update anyway.
            if (_userHasSetValue && !valueChanged)
            {
                return;
            }

            if ((value < MinDate) || (value > MaxDate))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    string.Format(
                        SR.InvalidBoundArgument,
                        nameof(Value),
                        FormatDateTime(value),
                        $"'{nameof(MinDate)}'",
                        $"'{nameof(MaxDate)}'"));
            }

            string oldText = Text;

            _value = value;
            _userHasSetValue = true;

            if (IsHandleCreated)
            {
                // Make sure any changes to this code get propagated to createHandle
                SYSTEMTIME systemTime = (SYSTEMTIME)value;
                PInvoke.SendMessage(this, PInvoke.DTM_SETSYSTEMTIME, (WPARAM)(uint)NMDATETIMECHANGE_FLAGS.GDT_VALID, ref systemTime);
            }

            if (valueChanged)
            {
                OnValueChanged(EventArgs.Empty);
            }

            if (!oldText.Equals(Text))
            {
                OnTextChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    ///  Occurs when the dropdown calendar is dismissed and disappears.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.DateTimePickerOnCloseUpDescr))]
    public event EventHandler? CloseUp
    {
        add => _onCloseUp += value;
        remove => _onCloseUp -= value;
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
    public event EventHandler? RightToLeftLayoutChanged
    {
        add => _onRightToLeftLayoutChanged += value;
        remove => _onRightToLeftLayoutChanged -= value;
    }

    /// <summary>
    ///  Occurs when the value for the control changes.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.valueChangedEventDescr))]
    public event EventHandler? ValueChanged
    {
        add => _onValueChanged += value;
        remove => _onValueChanged -= value;
    }

    /// <summary>
    ///  Occurs when the drop down calendar is shown.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.DateTimePickerOnDropDownDescr))]
    public event EventHandler? DropDown
    {
        add => _onDropDown += value;
        remove => _onDropDown -= value;
    }

    protected override AccessibleObject CreateAccessibilityInstance() => new DateTimePickerAccessibleObject(this);

    /// <summary>
    ///  Creates the physical window handle.
    /// </summary>
    protected override unsafe void CreateHandle()
    {
        if (!RecreatingHandle)
        {
            using ThemingScope scope = new(Application.UseVisualStyles);
            PInvoke.InitCommonControlsEx(new INITCOMMONCONTROLSEX
            {
                dwSize = (uint)sizeof(INITCOMMONCONTROLSEX),
                dwICC = INITCOMMONCONTROLSEX_ICC.ICC_DATE_CLASSES
            });
        }

        _creationTime = DateTime.Now;

        base.CreateHandle();

        if (_userHasSetValue && _validTime)
        {
            // Make sure any changes to this code get propagated to setValue
            SYSTEMTIME systemTime = (SYSTEMTIME)Value;
            PInvoke.SendMessage(this, PInvoke.DTM_SETSYSTEMTIME, (uint)NMDATETIMECHANGE_FLAGS.GDT_VALID, ref systemTime);
        }
        else if (!_validTime)
        {
            PInvoke.SendMessage(this, PInvoke.DTM_SETSYSTEMTIME, (uint)NMDATETIMECHANGE_FLAGS.GDT_NONE);
        }

        if (_format == DateTimePickerFormat.Custom)
        {
            PInvoke.SendMessage(this, PInvoke.DTM_SETFORMATW, 0, _customFormat);
        }

        UpdateUpDown();
        SetAllControlColors();
        SetControlCalendarFont();
        SetRange();
    }

    /// <summary>
    ///  Destroys the physical window handle.
    /// </summary>
    protected override void DestroyHandle()
    {
        _value = Value;
        base.DestroyHandle();
    }

    /// <summary>
    ///  Return a localized string representation of the given DateTime value.
    /// </summary>
    private static string FormatDateTime(DateTime value)
    {
        return value.ToString("G", CultureInfo.CurrentCulture);
    }

    // GetPreferredSize and SetBoundsCore call this method to allow controls to self impose
    // constraints on their size.
    internal override Rectangle ApplyBoundsConstraints(int suggestedX, int suggestedY, int proposedWidth, int proposedHeight)
    {
        // Lock DateTimePicker to its preferred height.
        return base.ApplyBoundsConstraints(suggestedX, suggestedY, proposedWidth, PreferredHeight);
    }

    internal override Size GetPreferredSizeCore(Size proposedConstraints)
    {
        int height = PreferredHeight;
        int width = CommonProperties.GetSpecifiedBounds(this).Width;
        return new Size(width, height);
    }

    /// <summary>
    ///  Handling special input keys, such as pgup, pgdown, home, end, etc...
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

    /// <summary>
    ///  Raises the <see cref="CloseUp"/> event.
    /// </summary>
    protected virtual void OnCloseUp(EventArgs eventargs)
    {
        _onCloseUp?.Invoke(this, eventargs);
        _expandCollapseState = ExpandCollapseState.ExpandCollapseState_Collapsed;

        // Raise automation event to annouce new state.
        if (IsAccessibilityObjectCreated)
        {
            AccessibilityObject.RaiseAutomationPropertyChangedEvent(
                UIA_PROPERTY_ID.UIA_ExpandCollapseExpandCollapseStatePropertyId,
                oldValue: (VARIANT)(int)ExpandCollapseState.ExpandCollapseState_Expanded,
                newValue: (VARIANT)(int)ExpandCollapseState.ExpandCollapseState_Collapsed);
        }
    }

    /// <summary>
    ///  Raises the <see cref="DropDown"/> event.
    /// </summary>
    protected virtual void OnDropDown(EventArgs eventargs)
    {
        _onDropDown?.Invoke(this, eventargs);
        _expandCollapseState = ExpandCollapseState.ExpandCollapseState_Expanded;

        // Raise automation event to announce new state.
        if (IsAccessibilityObjectCreated)
        {
            AccessibilityObject.RaiseAutomationPropertyChangedEvent(
                UIA_PROPERTY_ID.UIA_ExpandCollapseExpandCollapseStatePropertyId,
                oldValue: (VARIANT)(int)ExpandCollapseState.ExpandCollapseState_Collapsed,
                newValue: (VARIANT)(int)ExpandCollapseState.ExpandCollapseState_Expanded);
        }
    }

    protected virtual void OnFormatChanged(EventArgs e)
    {
        if (Events[s_formatChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);

        // Raise automation event to announce the control.
        if (IsAccessibilityObjectCreated)
        {
            _expandCollapseState = ExpandCollapseState.ExpandCollapseState_Collapsed;
            AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
        }
    }

    /// <summary>
    ///  Add/remove SystemEvents in OnHandleCreated/Destroyed for robustness.
    /// </summary>
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(MarshaledUserPreferenceChanged);
    }

    /// <summary>
    ///  Add/remove SystemEvents in OnHandleCreated/Destroyed for robustness.
    /// </summary>
    protected override void OnHandleDestroyed(EventArgs e)
    {
        SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(MarshaledUserPreferenceChanged);
        base.OnHandleDestroyed(e);
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);

        if (IsHandleCreated && Application.RenderWithVisualStyles)
        {
            // The SysDateTimePick32 control caches the style and uses that directly to determine whether the
            // border should be drawn disabled when theming (VisualStyles) is enabled. Setting the window
            // style to itself (which will have the proper WS_DISABLED setting after calling base) will
            // flush the cached value and render the border as one would expect.
            PInvoke.SetWindowLong(
                this,
                WINDOW_LONG_PTR_INDEX.GWL_STYLE,
                PInvoke.GetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_STYLE));
        }
    }

    /// <summary>
    ///  Raises the <see cref="ValueChanged"/> event.
    /// </summary>
    protected virtual void OnValueChanged(EventArgs eventargs)
    {
        _onValueChanged?.Invoke(this, eventargs);

        // Raise automation event to announce changed value.
        if (IsAccessibilityObjectCreated)
        {
            // If date is changed so dtp value is changed too.
            // But I can't receive the previous value here,
            // so I have to use current value twice.
            // Anyway it doesn't matter because the Narrator pronounces actual AO state.
            string? value = AccessibilityObject.Value;
            using VARIANT variantValue = value is null ? default : (VARIANT)value;
            AccessibilityObject.RaiseAutomationPropertyChangedEvent(
                UIA_PROPERTY_ID.UIA_ValueValuePropertyId,
                oldValue: variantValue,
                newValue: variantValue);
        }
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

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);

        // clear the pref height cache
        _prefHeightCache = -1;

        Height = PreferredHeight;

        if (_calendarFont is null)
        {
            _calendarFontHandleWrapper = null;
            SetControlCalendarFont();
        }
    }

    private void ResetCalendarForeColor()
    {
        CalendarForeColor = DefaultForeColor;
    }

    private void ResetCalendarFont()
    {
        CalendarFont = null;
    }

    private void ResetCalendarMonthBackground()
    {
        CalendarMonthBackground = DefaultMonthBackColor;
    }

    private void ResetCalendarTitleBackColor()
    {
        CalendarTitleBackColor = DefaultTitleBackColor;
    }

    private void ResetCalendarTitleForeColor()
    {
        CalendarTitleBackColor = DefaultForeColor;
    }

    private void ResetCalendarTrailingForeColor()
    {
        CalendarTrailingForeColor = DefaultTrailingForeColor;
    }

    /// <summary>
    ///  Resets the <see cref="Format"/> property to its default value.
    /// </summary>
    private void ResetFormat()
    {
        Format = DateTimePickerFormat.Long;
    }

    /// <summary>
    ///  Resets the <see cref="MaxDate"/> property to its default value.
    /// </summary>
    private void ResetMaxDate()
    {
        MaxDate = MaximumDateTime;
    }

    /// <summary>
    ///  Resets the <see cref="MinDate"/> property to its default value.
    /// </summary>
    private void ResetMinDate()
    {
        MinDate = MinimumDateTime;
    }

    /// <summary>
    ///  Resets the <see cref="Value"/> property to its default value.
    /// </summary>
    private void ResetValue()
    {
        // always update on reset with ShowNone = false -- as it'll take the current time.
        _value = DateTime.Now;

        // If ShowCheckBox = true, then userHasSetValue can be false (null value).
        // otherwise, userHasSetValue is valid...
        // userHasSetValue = !ShowCheckBox;

        // After ResetValue() the flag indicating whether the user has set the value should be false.
        _userHasSetValue = false;

        // Update the text displayed in the DateTimePicker.
        if (IsHandleCreated)
        {
            SYSTEMTIME systemTime = (SYSTEMTIME)_value;
            PInvoke.SendMessage(this, PInvoke.DTM_SETSYSTEMTIME, (uint)NMDATETIMECHANGE_FLAGS.GDT_VALID, ref systemTime);
        }

        // Updating Checked to false will set the control to "no date" and clear its checkbox.
        Checked = false;

        OnValueChanged(EventArgs.Empty);
        OnTextChanged(EventArgs.Empty);
    }

    /// <summary>
    ///  If the handle has been created, this applies the color to the control
    /// </summary>
    private void SetControlColor(uint colorIndex, Color value)
    {
        if (IsHandleCreated)
        {
            PInvoke.SendMessage(this, PInvoke.DTM_SETMCCOLOR, (WPARAM)(int)colorIndex, (LPARAM)value);
        }
    }

    /// <summary>
    ///  If the handle has been created, this applies the font to the control.
    /// </summary>
    private void SetControlCalendarFont()
    {
        if (IsHandleCreated)
        {
            PInvoke.SendMessage(this, PInvoke.DTM_SETMCFONT, (WPARAM)CalendarFontHandle, (LPARAM)(-1));
        }
    }

    /// <summary>
    ///  Applies all the colors to the control.
    /// </summary>
    private void SetAllControlColors()
    {
        SetControlColor(PInvoke.MCSC_MONTHBK, _calendarMonthBackground);
        SetControlColor(PInvoke.MCSC_TEXT, _calendarForeColor);
        SetControlColor(PInvoke.MCSC_TITLEBK, _calendarTitleBackColor);
        SetControlColor(PInvoke.MCSC_TITLETEXT, _calendarTitleForeColor);
        SetControlColor(PInvoke.MCSC_TRAILINGTEXT, _calendarTrailingText);
    }

    /// <summary>
    ///  Updates the window handle with the min/max ranges if it has been created.
    /// </summary>
    private void SetRange()
    {
        SetRange(EffectiveMinDate(_minDateTime), EffectiveMaxDate(_maxDateTime));
    }

    private void SetRange(DateTime min, DateTime max)
    {
        if (IsHandleCreated)
        {
            Span<SYSTEMTIME> times = [(SYSTEMTIME)min, (SYSTEMTIME)max];
            uint flags = PInvoke.GDTR_MIN | PInvoke.GDTR_MAX;
            PInvoke.SendMessage(this, PInvoke.DTM_SETRANGE, (WPARAM)flags, ref times[0]);
        }
    }

    /// <summary>
    ///  Turns on or off a given style bit.
    /// </summary>
    private void SetStyleBit(bool flag, uint bit)
    {
        if (((_style & bit) != 0) == flag)
        {
            return;
        }

        if (flag)
        {
            _style |= bit;
        }
        else
        {
            _style &= ~bit;
        }

        if (IsHandleCreated)
        {
            RecreateHandle();
            Invalidate();
            Update();
        }
    }

    /// <summary>
    ///  Determines if the <see cref="CalendarForeColor"/> property needs to be persisted.
    /// </summary>
    private bool ShouldSerializeCalendarForeColor()
    {
        return !CalendarForeColor.Equals(DefaultForeColor);
    }

    /// <summary>
    ///  Determines if the <see cref="CalendarFont"/> property needs to be persisted.
    /// </summary>
    private bool ShouldSerializeCalendarFont()
    {
        return _calendarFont is not null;
    }

    /// <summary>
    ///  Determines if the <see cref="CalendarTitleBackColor"/> property needs to be persisted.
    /// </summary>
    private bool ShouldSerializeCalendarTitleBackColor()
    {
        return !_calendarTitleBackColor.Equals(DefaultTitleBackColor);
    }

    /// <summary>
    ///  Determines if the <see cref="CalendarTitleForeColor"/> property needs to be persisted.
    /// </summary>
    private bool ShouldSerializeCalendarTitleForeColor()
    {
        return !_calendarTitleForeColor.Equals(DefaultTitleForeColor);
    }

    /// <summary>
    ///  Determines if the <see cref="CalendarTrailingForeColor"/> property needs to be persisted.
    /// </summary>
    private bool ShouldSerializeCalendarTrailingForeColor()
    {
        return !_calendarTrailingText.Equals(DefaultTrailingForeColor);
    }

    /// <summary>
    ///  Determines if the <see cref="CalendarMonthBackground"/> property needs to be persisted.
    /// </summary>
    private bool ShouldSerializeCalendarMonthBackground()
    {
        return !_calendarMonthBackground.Equals(DefaultMonthBackColor);
    }

    /// <summary>
    ///  Determines if the <see cref="MaxDate"/> property needs to be persisted.
    /// </summary>
    private bool ShouldSerializeMaxDate()
    {
        return _maxDateTime != MaximumDateTime && _maxDateTime != DateTime.MaxValue;
    }

    /// <summary>
    ///  Determines if the <see cref="MinDate"/> property needs to be persisted.
    /// </summary>
    private bool ShouldSerializeMinDate()
    {
        return _minDateTime != MinimumDateTime && _minDateTime != DateTime.MinValue;
    }

    /// <summary>
    ///  Determines if the <see cref="Value"/> property needs to be persisted.
    /// </summary>
    private bool ShouldSerializeValue()
    {
        return _userHasSetValue;
    }

    /// <summary>
    ///  Determines if the <see cref="Format"/> property needs to be persisted.
    /// </summary>
    private bool ShouldSerializeFormat()
    {
        return (Format != DateTimePickerFormat.Long);
    }

    public override string ToString() => $"{base.ToString()}, Value: {Value:G}";

    /// <summary>
    ///  Forces a repaint of the updown control if it is displayed.
    /// </summary>
    private unsafe void UpdateUpDown()
    {
        // The upDown control doesn't repaint correctly.
        if (ShowUpDown)
        {
            EnumChildren c = new();
            PInvoke.EnumChildWindows(this, c.enumChildren);
            if (!c.hwndFound.IsNull)
            {
                PInvoke.InvalidateRect(c.hwndFound, lpRect: (RECT*)null, bErase: true);
                PInvoke.UpdateWindow(c.hwndFound);
            }
        }
    }

    private void MarshaledUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs pref)
    {
        try
        {
            // Use begininvoke instead of invoke in case the destination thread is not processing messages.
            BeginInvoke(new UserPreferenceChangedEventHandler(UserPreferenceChanged), [sender, pref]);
        }
        catch (InvalidOperationException) { } // If the destination thread does not exist, don't send.
    }

    private void UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs pref)
    {
        if (pref.Category == UserPreferenceCategory.Locale)
        {
            // We need to recreate the monthcalendar handle when the locale changes, because
            // the day names etc. are only updated on a handle recreate (comctl32 limitation).
            RecreateHandle();
        }
    }

    /// <summary>
    ///  Handles the DTN_DATETIMECHANGE notification.
    /// </summary>
    private unsafe void WmDateTimeChange(ref Message m)
    {
        NMDATETIMECHANGE* nmdtc = (NMDATETIMECHANGE*)(nint)m.LParamInternal;
        DateTime temp = _value;
        bool oldvalid = _validTime;
        if (nmdtc->dwFlags != NMDATETIMECHANGE_FLAGS.GDT_NONE)
        {
            _validTime = true;
            _value = (DateTime)nmdtc->st;
            _userHasSetValue = true;
        }
        else
        {
            _validTime = false;
        }

        if (_value != temp || oldvalid != _validTime)
        {
            OnValueChanged(EventArgs.Empty);
            OnTextChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    ///  Handles the DTN_DROPDOWN notification.
    /// </summary>
    private void WmDropDown()
    {
        if (RightToLeftLayout && RightToLeft == RightToLeft.Yes)
        {
            HWND handle = (HWND)PInvoke.SendMessage(this, PInvoke.DTM_GETMONTHCAL);
            if (handle != IntPtr.Zero)
            {
                WINDOW_EX_STYLE style = (WINDOW_EX_STYLE)PInvoke.GetWindowLong(handle, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
                style |= WINDOW_EX_STYLE.WS_EX_LAYOUTRTL | WINDOW_EX_STYLE.WS_EX_NOINHERITLAYOUT;
                style &= ~(WINDOW_EX_STYLE.WS_EX_RIGHT | WINDOW_EX_STYLE.WS_EX_RTLREADING);
                PInvoke.SetWindowLong(handle, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, (nint)style);
                GC.KeepAlive(this);
            }
        }

        OnDropDown(EventArgs.Empty);
    }

    /// <summary>
    ///  Handles system color changes.
    /// </summary>
    protected override void OnSystemColorsChanged(EventArgs e)
    {
        SetAllControlColors();
        base.OnSystemColorsChanged(e);
    }

    /// <summary>
    ///  Handles the WM_COMMAND messages reflected from the parent control.
    /// </summary>
    private unsafe void WmReflectCommand(ref Message m)
    {
        if (m.HWnd == Handle)
        {
            NMHDR* nmhdr = (NMHDR*)(nint)m.LParamInternal;
            switch (nmhdr->code)
            {
                case PInvoke.DTN_CLOSEUP:
                    OnCloseUp(EventArgs.Empty);
                    break;
                case PInvoke.DTN_DATETIMECHANGE:
                    WmDateTimeChange(ref m);
                    break;
                case PInvoke.DTN_DROPDOWN:
                    WmDropDown();
                    break;
            }
        }
    }

    protected override void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case PInvoke.WM_LBUTTONDOWN:
                Focus();
                if (!ValidationCancelled)
                {
                    base.WndProc(ref m);
                }

                break;
            case MessageId.WM_REFLECT_NOTIFY:
                WmReflectCommand(ref m);
                base.WndProc(ref m);
                break;
            case PInvoke.WM_WINDOWPOSCHANGED:
                base.WndProc(ref m);
                UpdateUpDown();
                break;
            default:
                base.WndProc(ref m);
                break;
        }
    }
}
