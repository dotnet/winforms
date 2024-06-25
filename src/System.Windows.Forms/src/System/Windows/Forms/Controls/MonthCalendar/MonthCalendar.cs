// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms.Layout;
using Microsoft.Win32;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

/// <summary>
///  This control is an encapsulation of the Windows month calendar control. A month calendar control implements a
///  calendar-like user interface that provides the user with a intuitive and recognizable method of entering or
///  selecting a date.
/// </summary>
/// <remarks>
///  <para>
///   Users can also select which days to bold. The most efficient way to add the bolded dates is via an array.
///   The following is an example of this:
///  </para>
///  <code>
///   MonthCalendar mc = new MonthCalendar();
///   DateTime[] time = new DateTime[3];
///   time[0] = DateTime.Now;
///   time[1] = time[0].AddDays(2);
///   time[2] = time[1].AddDays(2);
///   mc.BoldedDates = time;
///  </code>
///  <para>
///   Removal of all bolded dates is accomplished with:
///  </para>
///  <code>
///   mc.RemoveAllBoldedDates();
///  </code>
///  <para>
///   Although less efficient, the user may need to add or remove bolded dates one at a time. To improve the performance
///   of this, neither <see cref="AddBoldedDate(DateTime)"/> nor <see cref="RemoveBoldedDate(DateTime)"/> repaints the
///   <see cref="MonthCalendar"/>. The user must call <see cref="UpdateBoldedDates"/> to force the repaint of the bolded
///   dates, otherwise the <see cref="MonthCalendar"/> will not paint properly. The following is an example of this:
///  </para>
///  <code>
///   DateTime time1 = new DateTime("3/5/98");
///   DateTime time2 = new DateTime("4/19/98");
///   mc.AddBoldedDate(time1);
///   mc.AddBoldedDate(time2);
///   mc.RemoveBoldedDate(time1);
///   mc.UpdateBoldedDates();
///  </code>
///  <para>
///   The same applies to addition and removal of annual and monthly bolded dates.
///  </para>
/// </remarks>
[DefaultProperty(nameof(SelectionRange))]
[DefaultEvent(nameof(DateChanged))]
[DefaultBindingProperty(nameof(SelectionRange))]
[Designer($"System.Windows.Forms.Design.MonthCalendarDesigner, {AssemblyRef.SystemDesign}")]
[SRDescription(nameof(SR.DescriptionMonthCalendar))]
public partial class MonthCalendar : Control
{
    private static readonly Color s_defaultTitleBackColor = Application.ApplicationColors.ActiveCaption;
    private static readonly Color s_defaultTitleForeColor = Application.ApplicationColors.ActiveCaptionText;
    private static readonly Color s_trailingForeColor = Application.ApplicationColors.GrayText;
    private const int MonthsInYear = 12;

    /// <summary>
    ///  This is the arbitrary number of pixels that the Win32 control
    ///  inserts between calendars horizontally, regardless of font.
    /// </summary>
    private const int InsertWidthSize = 6;

    /// <summary>
    ///  This is the arbitrary number of pixels that the Win32 control
    ///  inserts between calendars vertically, regardless of font.
    ///  From ComCtl32 MonthCalendar sources CALBORDER.
    /// </summary>
    private const int InsertHeightSize = 6;

    private const Day DefaultFirstDayOfWeek = Day.Default;
    private const int DefaultMaxSelectionCount = 7;
    private const int DefaultScrollChange = 0;

    private static readonly Size s_defaultSingleMonthSize = new(176, 153);

    private const int MaxScrollChange = 20000;

    private int _extraPadding;

    private Color _titleBackColor = s_defaultTitleBackColor;
    private Color _titleForeColor = s_defaultTitleForeColor;
    private Color _trailingForeColor = s_trailingForeColor;
    private bool _showToday = true;
    private bool _showTodayCircle = true;
    private bool _showWeekNumbers;
    private bool _rightToLeftLayout;

    private Size _dimensions = new(1, 1);
    private int _maxSelectionCount = DefaultMaxSelectionCount;
    private DateTime _maxDate = DateTime.MaxValue;
    private DateTime _minDate = DateTime.MinValue;
    private int _scrollChange = DefaultScrollChange;
    private bool _todayDateSet;
    private DateTime _todaysDate = DateTime.Now.Date;
    private DateTime _selectionStart;
    private DateTime _selectionEnd;
    private DateTime _focusedDate;
    private SelectionRange? _currentDisplayRange;
    private Day _firstDayOfWeek = DefaultFirstDayOfWeek;
    private MONTH_CALDENDAR_MESSAGES_VIEW _mcCurView = MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH;
    private MONTH_CALDENDAR_MESSAGES_VIEW _mcOldView = MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH;

    /// <summary>
    ///  Bitmask for the annually bolded dates. Months start on January.
    /// </summary>
    private readonly int[] _monthsOfYear = new int[MonthsInYear];

    /// <summary>
    ///  Bitmask for the dates bolded monthly.
    /// </summary>
    private int _datesToBoldMonthly;

    private readonly List<DateTime> _boldDates = [];
    private readonly List<DateTime> _annualBoldDates = [];
    private readonly List<DateTime> _monthlyBoldDates = [];

    private DateRangeEventHandler? _onDateChanged;
    private DateRangeEventHandler? _onDateSelected;
    private EventHandler? _onRightToLeftLayoutChanged;
    private EventHandler? _onCalendarViewChanged;
    private EventHandler? _onDisplayRangeChanged;

    public MonthCalendar() : base()
    {
        _selectionStart = _todaysDate;
        _selectionEnd = _todaysDate;
        _focusedDate = _todaysDate;
        SetStyle(ControlStyles.UserPaint, false);
        SetStyle(ControlStyles.StandardClick, false);

        TabStop = true;
    }

    protected override AccessibleObject CreateAccessibilityInstance()
        => new MonthCalendarAccessibleObject(this);

    protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
    {
        base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
        ScaleConstants();
    }

    private protected override void InitializeConstantsForInitialDpi(int initialDpi) => ScaleConstants();

    private void ScaleConstants()
    {
        const int LogicalExtraPadding = 2;
        _extraPadding = LogicalToDeviceUnits(LogicalExtraPadding);
    }

    /// <summary>
    ///  The array of DateTime objects that determines which annual days are shown in bold.
    /// </summary>
    [Localizable(true)]
    [SRDescription(nameof(SR.MonthCalendarAnnuallyBoldedDatesDescr))]
    public DateTime[] AnnuallyBoldedDates
    {
        get => [.. _annualBoldDates];
        set
        {
            _annualBoldDates.Clear();
            for (int i = 0; i < MonthsInYear; ++i)
            {
                _monthsOfYear[i] = 0;
            }

            if (value is not null && value.Length > 0)
            {
                // Add each bolded date to our List.
                _annualBoldDates.AddRange(value);
                foreach (var dateTime in value)
                {
                    _monthsOfYear[dateTime.Month - 1] |= 0x00000001 << (dateTime.Day - 1);
                }
            }

            UpdateBoldedDates();
        }
    }

    [SRDescription(nameof(SR.MonthCalendarMonthBackColorDescr))]
    public override Color BackColor
    {
        get
        {
            if (ShouldSerializeBackColor() || IsDarkModeEnabled)
            {
                return base.BackColor;
            }

            return Application.ApplicationColors.Window;
        }
        set => base.BackColor = value;
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
    ///  The array of DateTime objects that determines which non-recurring
    ///  specified dates are shown in bold.
    /// </summary>
    [Localizable(true)]
    public DateTime[] BoldedDates
    {
        get => [.. _boldDates];

        set
        {
            _boldDates.Clear();
            if (value is not null && value.Length > 0)
            {
                // Add each bolded date to our list.
                _boldDates.AddRange(value);
            }

            UpdateBoldedDates();
        }
    }

    /// <summary>
    ///  The number of columns and rows of months that will be displayed
    ///  in the MonthCalendar control.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [SRDescription(nameof(SR.MonthCalendarDimensionsDescr))]
    public Size CalendarDimensions
    {
        get => _dimensions;
        set
        {
            if (_dimensions.Equals(value))
            {
                return;
            }

            SetCalendarDimensions(value.Width, value.Height);
        }
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ClassName = PInvoke.MONTHCAL_CLASS;
            cp.Style |= (int)PInvoke.MCS_MULTISELECT | (int)PInvoke.MCS_DAYSTATE;
            if (!_showToday)
            {
                cp.Style |= (int)PInvoke.MCS_NOTODAY;
            }

            if (!_showTodayCircle)
            {
                cp.Style |= (int)PInvoke.MCS_NOTODAYCIRCLE;
            }

            if (_showWeekNumbers)
            {
                cp.Style |= (int)PInvoke.MCS_WEEKNUMBERS;
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

    protected override ImeMode DefaultImeMode => ImeMode.Disable;

    protected override Padding DefaultMargin => new(9);

    protected override Size DefaultSize => GetMinReqRect();

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

    internal void FillMonthDayStates(Span<uint> monthDayStates, SelectionRange displayRange)
    {
        // Run through all displayed dates to set a binary marker that the date is bolded
        // if BoldedDates, AnnualArrayOfDates, or MonthlyArrayOfDates contain this date.
        DateTime currentDate = displayRange.Start;
        while (currentDate <= displayRange.End)
        {
            bool currentDateIsBolded = _boldDates.Contains(currentDate)
                || _annualBoldDates.Any(d => d.Month == currentDate.Month && d.Day == currentDate.Day)
                || _monthlyBoldDates.Any(d => d.Day == currentDate.Day);

            if (currentDateIsBolded)
            {
                // Calculate an index of a month of the current date in the display range,
                // starting from the first displayed month.
                // The display range may include gray dates of the first and last months.
                // So the max count of visible months is 14 and the max index is 13.
                int currentMonthIndex = GetIndexInMonths(displayRange.Start, currentDate);

                // Set bolded state for the current date of the current month
                // to prepare the states array before sending to Windows
                monthDayStates[currentMonthIndex] |= 1U << currentDate.Day - 1;
            }

            // Set the next day for check
            currentDate = currentDate.AddDays(1);
        }
    }

    /// <summary>
    ///  The first day of the week for the month calendar control.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [DefaultValue(DefaultFirstDayOfWeek)]
    [SRDescription(nameof(SR.MonthCalendarFirstDayOfWeekDescr))]
    public Day FirstDayOfWeek
    {
        get => _firstDayOfWeek;

        set
        {
            if (value is < Day.Monday or > Day.Default)
            {
                throw new InvalidEnumArgumentException(nameof(FirstDayOfWeek), (int)value, typeof(Day));
            }

            if (value == _firstDayOfWeek)
            {
                return;
            }

            _firstDayOfWeek = value;
            if (IsHandleCreated)
            {
                if (value == Day.Default)
                {
                    RecreateHandle();
                }
                else
                {
                    PInvoke.SendMessage(this, PInvoke.MCM_SETFIRSTDAYOFWEEK, 0, (nint)value);
                }

                UpdateDisplayRange();

                // Add the extra call to make the accessibility tree to rebuild correctly
                OnDisplayRangeChanged(EventArgs.Empty);
            }
        }
    }

    [SRDescription(nameof(SR.MonthCalendarForeColorDescr))]
    public override Color ForeColor
    {
        get
        {
            if (ShouldSerializeForeColor() || IsDarkModeEnabled)
            {
                return base.ForeColor;
            }

            return Application.ApplicationColors.WindowText;
        }
        set => base.ForeColor = value;
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
    ///  The maximum allowable date that can be selected. By default, there
    ///  is no maximum date. The maximum date is not set if max less than the
    ///  current minimum date.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MonthCalendarMaxDateDescr))]
    public DateTime MaxDate
    {
        get => DateTimePicker.EffectiveMaxDate(_maxDate);
        set
        {
            if (value == _maxDate)
            {
                return;
            }

            ArgumentOutOfRangeException.ThrowIfLessThan(value, DateTimePicker.EffectiveMinDate(_minDate));

            _maxDate = value;
            SetRange();
        }
    }

    /// <summary>
    ///  The maximum number of days that can be selected in a
    ///  month calendar control. This method does not affect the current
    ///  selection range.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(DefaultMaxSelectionCount)]
    [SRDescription(nameof(SR.MonthCalendarMaxSelectionCountDescr))]
    public int MaxSelectionCount
    {
        get => _maxSelectionCount;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);

            if (value == _maxSelectionCount)
            {
                return;
            }

            if (IsHandleCreated)
            {
                if (PInvoke.SendMessage(this, PInvoke.MCM_SETMAXSELCOUNT, (WPARAM)value) == 0)
                {
                    throw new ArgumentException(string.Format(SR.MonthCalendarMaxSelCount, value.ToString("D")), nameof(value));
                }
            }

            _maxSelectionCount = value;
        }
    }

    /// <summary>
    ///  The minimum allowable date that can be selected. By default, there
    ///  is no minimum date. The minimum date is not set if min greater than the
    ///  current maximum date. MonthCalendar does not support dates prior to 1753.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MonthCalendarMinDateDescr))]
    public DateTime MinDate
    {
        get => DateTimePicker.EffectiveMinDate(_minDate);
        set
        {
            if (value == _minDate)
            {
                return;
            }

            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, DateTimePicker.EffectiveMaxDate(_maxDate));
            ArgumentOutOfRangeException.ThrowIfLessThan(value, DateTimePicker.MinimumDateTime);

            _minDate = value;
            SetRange();
        }
    }

    /// <summary>
    ///  The array of DateTime objects that determine which monthly days to bold.
    /// </summary>
    [Localizable(true)]
    [SRDescription(nameof(SR.MonthCalendarMonthlyBoldedDatesDescr))]
    public DateTime[] MonthlyBoldedDates
    {
        get => [.. _monthlyBoldDates];

        set
        {
            _monthlyBoldDates.Clear();
            _datesToBoldMonthly = 0;

            if (value is not null && value.Length > 0)
            {
                // Add each bolded date to our List.
                _monthlyBoldDates.AddRange(value);

                foreach (var dateTime in value)
                {
                    _datesToBoldMonthly |= 0x00000001 << (dateTime.Day - 1);
                }
            }

            UpdateBoldedDates();
        }
    }

    private static DateTime Now => DateTime.Now.Date;

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
    ///  The scroll rate for a month calendar control. The scroll rate is the
    ///  number of months that the control moves its display when the user clicks
    ///  a scroll button. If this value is zero, the month delta is reset to the
    ///  default, which is the number of months displayed in the control.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(DefaultScrollChange)]
    [SRDescription(nameof(SR.MonthCalendarScrollChangeDescr))]
    public int ScrollChange
    {
        get => _scrollChange;
        set
        {
            if (_scrollChange == value)
            {
                return;
            }

            ArgumentOutOfRangeException.ThrowIfNegative(value);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxScrollChange);

            if (IsHandleCreated)
            {
                PInvoke.SendMessage(this, PInvoke.MCM_SETMONTHDELTA, (WPARAM)value);
            }

            _scrollChange = value;
        }
    }

    /// <summary>
    ///  Indicates the end date of the selected range of dates.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.MonthCalendarSelectionEndDescr))]
    public DateTime SelectionEnd
    {
        get => _selectionEnd;
        set
        {
            if (_selectionEnd == value)
            {
                return;
            }

            ArgumentOutOfRangeException.ThrowIfLessThan(value, MinDate);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxDate);

            // If we've moved SelectionEnd before SelectionStart, move SelectionStart back
            if (_selectionStart > value)
            {
                _selectionStart = value;
            }

            // If we've moved SelectionEnd too far beyond SelectionStart, move SelectionStart forward
            if ((value - _selectionStart).Days >= _maxSelectionCount)
            {
                _selectionStart = value.AddDays(1 - _maxSelectionCount);
            }

            // Set the new selection range
            SetSelRange(_selectionStart, value);
        }
    }

    /// <summary>
    ///  Indicates the start date of the selected range of dates.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.MonthCalendarSelectionStartDescr))]
    public DateTime SelectionStart
    {
        get => _selectionStart;
        set
        {
            if (_selectionStart == value)
            {
                return;
            }

            ArgumentOutOfRangeException.ThrowIfLessThan(value, _minDate);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, _maxDate);

            // If we've moved SelectionStart beyond SelectionEnd, move SelectionEnd forward
            if (_selectionEnd < value)
            {
                _selectionEnd = value;
            }

            // If we've moved SelectionStart too far back from SelectionEnd, move SelectionEnd back
            if ((_selectionEnd - value).Days >= _maxSelectionCount)
            {
                _selectionEnd = value.AddDays(_maxSelectionCount - 1);
            }

            // Set the new selection range
            SetSelRange(value, _selectionEnd);
        }
    }

    /// <summary>
    ///  Retrieves the selection range for a month calendar control.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MonthCalendarSelectionRangeDescr))]
    [Bindable(true)]
    public SelectionRange SelectionRange
    {
        get => new(SelectionStart, SelectionEnd);
        set => SetSelectionRange(value.Start, value.End);
    }

    /// <summary>
    ///  Indicates whether the month calendar control will display
    ///  the "today" date at the bottom of the control.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.MonthCalendarShowTodayDescr))]
    public bool ShowToday
    {
        get => _showToday;
        set
        {
            if (value == _showToday)
            {
                return;
            }

            _showToday = value;
            UpdateStyles();
            AdjustSize();
        }
    }

    /// <summary>
    ///  Indicates whether the month calendar control will circle
    ///  the "today" date.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.MonthCalendarShowTodayCircleDescr))]
    public bool ShowTodayCircle
    {
        get => _showTodayCircle;
        set
        {
            if (value == _showTodayCircle)
            {
                return;
            }

            _showTodayCircle = value;
            UpdateStyles();
        }
    }

    /// <summary>
    ///  Indicates whether the month calendar control will the display
    ///  week numbers (1-52) to the left of each row of days.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.MonthCalendarShowWeekNumbersDescr))]
    public bool ShowWeekNumbers
    {
        get => _showWeekNumbers;
        set
        {
            if (value == _showWeekNumbers)
            {
                return;
            }

            _showWeekNumbers = value;
            UpdateStyles();
            AdjustSize();
        }
    }

    /// <summary>
    ///  The minimum size required to display a full month. The size information
    ///  is presented in the form of a Point, with the x and y members representing
    ///  the minimum width and height required for the control. The minimum
    ///  required window size for a month calendar control depends on the
    ///  currently selected font.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.MonthCalendarSingleMonthSizeDescr))]
    public Size SingleMonthSize
    {
        get
        {
            if (IsHandleCreated)
            {
                RECT rect = default;
                if (PInvoke.SendMessage(this, PInvoke.MCM_GETMINREQRECT, 0, ref rect) == 0)
                {
                    throw new InvalidOperationException(SR.InvalidSingleMonthSize);
                }

                return new Size(rect.right, rect.bottom);
            }

            return s_defaultSingleMonthSize;
        }
    }

    /// <summary>
    ///  Unlike most controls, serializing the MonthCalendar's Size is really bad:
    ///  when it's restored at runtime, it uses a a default SingleMonthSize, which
    ///  may not be right, especially for JPN/CHS machines.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Localizable(false)]
    public new Size Size
    {
        get => base.Size;
        set => base.Size = value;
    }

    internal override bool SupportsUiaProviders => true;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Bindable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
    ///  The date shown as "Today" in the Month Calendar control. By default, "Today" is the current date at the
    ///  time the MonthCalendar control is created.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MonthCalendarTodayDateDescr))]
    public DateTime TodayDate
    {
        get
        {
            if (_todayDateSet)
            {
                return _todaysDate;
            }

            if (IsHandleCreated)
            {
                SYSTEMTIME systemTime = default;
                int result = (int)PInvoke.SendMessage(this, PInvoke.MCM_GETTODAY, 0, ref systemTime);
                Debug.Assert(result != 0, "MCM_GETTODAY failed");
                return ((DateTime)systemTime).Date;
            }

            return Now.Date;
        }
        set
        {
            if (!_todayDateSet || (DateTime.Compare(value, _todaysDate) != 0))
            {
                // Throw if trying to set the TodayDate to a value greater than MaxDate.
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, _maxDate);

                // Throw if trying to set the TodayDate to a value less than MinDate.
                ArgumentOutOfRangeException.ThrowIfLessThan(value, _minDate);

                _todaysDate = value.Date;
                _todayDateSet = true;
                UpdateTodayDate();
            }
        }
    }

    /// <summary>
    ///  Indicates whether or not the TodayDate property has been explicitly
    ///  set by the user. If TodayDateSet is true, TodayDate will return whatever
    ///  the user has set it to. If TodayDateSet is false, TodayDate will follow
    ///  wall-clock time; ie. TodayDate will always equal the current system date.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.MonthCalendarTodayDateSetDescr))]
    public bool TodayDateSet => _todayDateSet;

    /// <summary>
    ///  The background color displayed in the month calendar's title.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.MonthCalendarTitleBackColorDescr))]
    public Color TitleBackColor
    {
        get => _titleBackColor;
        set
        {
            if (value.IsEmpty)
            {
                throw new ArgumentException(string.Format(SR.InvalidNullArgument, nameof(value)), nameof(value));
            }

            _titleBackColor = value;
            SetControlColor(PInvoke.MCSC_TITLEBK, value);
        }
    }

    /// <summary>
    ///  The foreground color used to display text within the month
    ///  calendar's title.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.MonthCalendarTitleForeColorDescr))]
    public Color TitleForeColor
    {
        get => _titleForeColor;
        set
        {
            if (value.IsEmpty)
            {
                throw new ArgumentException(string.Format(SR.InvalidNullArgument, nameof(value)), nameof(value));
            }

            _titleForeColor = value;
            SetControlColor(PInvoke.MCSC_TITLETEXT, value);
        }
    }

    /// <summary>
    ///  The color used to display the previous and following months that
    ///  appear on the current month calendar.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.MonthCalendarTrailingForeColorDescr))]
    public Color TrailingForeColor
    {
        get => _trailingForeColor;
        set
        {
            if (value.IsEmpty)
            {
                throw new ArgumentException(string.Format(SR.InvalidNullArgument, nameof(value)), nameof(value));
            }

            _trailingForeColor = value;
            SetControlColor(PInvoke.MCSC_TRAILINGTEXT, value);
        }
    }

    /// <summary>
    ///  Adds a day that will be bolded annually on the month calendar.
    ///  Be sure to call UpdateBoldedDates() afterwards.
    /// </summary>
    public void AddAnnuallyBoldedDate(DateTime date)
    {
        _annualBoldDates.Add(date);
        _monthsOfYear[date.Month - 1] |= 0x00000001 << (date.Day - 1);
    }

    /// <summary>
    ///  Adds a day that will be bolded on the month calendar.
    ///  Be sure to call UpdateBoldedDates() afterwards.
    /// </summary>
    public void AddBoldedDate(DateTime date)
    {
        if (!_boldDates.Contains(date))
        {
            _boldDates.Add(date);
        }
    }

    /// <summary>
    ///  Adds a day that will be bolded monthly on the month calendar.
    ///  Be sure to call UpdateBoldedDates() afterwards.
    /// </summary>
    public void AddMonthlyBoldedDate(DateTime date)
    {
        _monthlyBoldDates.Add(date);
        _datesToBoldMonthly |= 0x00000001 << (date.Day - 1);
    }

    private event EventHandler? CalendarViewChanged
    {
        add => _onCalendarViewChanged += value;
        remove => _onCalendarViewChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? Click
    {
        add => base.Click += value;
        remove => base.Click -= value;
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.MonthCalendarOnDateChangedDescr))]
    public event DateRangeEventHandler? DateChanged
    {
        add => _onDateChanged += value;
        remove => _onDateChanged -= value;
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.MonthCalendarOnDateSelectedDescr))]
    public event DateRangeEventHandler? DateSelected
    {
        add => _onDateSelected += value;
        remove => _onDateSelected -= value;
    }

    private event EventHandler? DisplayRangeChanged
    {
        add => _onDisplayRangeChanged += value;
        remove => _onDisplayRangeChanged -= value;
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

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event PaintEventHandler? Paint
    {
        add => base.Paint += value;
        remove => base.Paint -= value;
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
    public event EventHandler? RightToLeftLayoutChanged
    {
        add => _onRightToLeftLayoutChanged += value;
        remove => _onRightToLeftLayoutChanged -= value;
    }

    /// <summary>
    ///  Used to auto-size the control. The requested number of rows and columns are
    ///  restricted by the maximum size of the parent control, hence the requested number
    ///  of rows and columns may not be what you get.
    /// </summary>
    private void AdjustSize()
    {
        Size minSize = GetMinReqRect();
        Size = minSize;
    }

    private void WriteBoldDates(Span<int> boldDates)
    {
        boldDates.Clear();

        SelectionRange range = GetDisplayRange(visible: false);
        DateTime start = range.Start;
        int startMonth = start.Month;
        int startYear = start.Year;

        // Add the non-recurrent dates to bold.
        foreach (DateTime date in _boldDates)
        {
            if (DateTime.Compare(date, start) >= 0 && DateTime.Compare(date, range.End) <= 0)
            {
                // Date is within the display range, convert it into month count starting from the first displayed month.
                int monthIndex = (date.Year - startYear) * MonthsInYear + date.Month - startMonth;

                boldDates[monthIndex] |= (0x00000001 << (date.Day - 1));
            }
        }

        // Add monthly and annual dates to bold.
        --startMonth;
        for (int i = 0; i < boldDates.Length; ++i, ++startMonth)
        {
            boldDates[i] |= _monthsOfYear[startMonth % MonthsInYear] | _datesToBoldMonthly;
        }
    }

    /// <summary>
    ///  Compares only the day and month of each time.
    /// </summary>
    private static bool CompareDayAndMonth(DateTime t1, DateTime t2)
        => (t1.Day == t2.Day && t1.Month == t2.Month);

    protected override unsafe void CreateHandle()
    {
        if (!RecreatingHandle)
        {
            using ThemingScope scope = new(Application.UseVisualStyles);
            PInvoke.InitCommonControlsEx(new INITCOMMONCONTROLSEX()
            {
                dwSize = (uint)sizeof(INITCOMMONCONTROLSEX),
                dwICC = INITCOMMONCONTROLSEX_ICC.ICC_DATE_CLASSES
            });
        }

        base.CreateHandle();
    }

    /// <summary>
    ///  Return a localized string representation of the given DateTime value.
    ///  Used for throwing exceptions, etc.
    /// </summary>
    private static string FormatDate(DateTime value) => value.ToString("d", CultureInfo.CurrentCulture);

    /// <summary>
    ///  Retrieves date information that represents the low and high limits of the
    ///  control's display.
    /// </summary>
    public SelectionRange GetDisplayRange(bool visible)
    {
        if (visible)
        {
            return GetMonthRange(PInvoke.GMR_VISIBLE);
        }

        return GetMonthRange(PInvoke.GMR_DAYSTATE);
    }

    /// <summary>
    ///  Retrieves the enumeration value corresponding to the hit area.
    /// </summary>
    private static HitArea GetHitArea(MCHITTESTINFO_HIT_FLAGS hit) => hit switch
    {
        MCHITTESTINFO_HIT_FLAGS.MCHT_TITLEBK => HitArea.TitleBackground,
        MCHITTESTINFO_HIT_FLAGS.MCHT_TITLEMONTH => HitArea.TitleMonth,
        MCHITTESTINFO_HIT_FLAGS.MCHT_TITLEYEAR => HitArea.TitleYear,
        MCHITTESTINFO_HIT_FLAGS.MCHT_TITLEBTNNEXT => HitArea.NextMonthButton,
        MCHITTESTINFO_HIT_FLAGS.MCHT_TITLEBTNPREV => HitArea.PrevMonthButton,
        MCHITTESTINFO_HIT_FLAGS.MCHT_CALENDARBK => HitArea.CalendarBackground,
        MCHITTESTINFO_HIT_FLAGS.MCHT_CALENDARDATE => HitArea.Date,
        MCHITTESTINFO_HIT_FLAGS.MCHT_CALENDARDATENEXT => HitArea.NextMonthDate,
        MCHITTESTINFO_HIT_FLAGS.MCHT_CALENDARDATEPREV => HitArea.PrevMonthDate,
        MCHITTESTINFO_HIT_FLAGS.MCHT_CALENDARDAY => HitArea.DayOfWeek,
        MCHITTESTINFO_HIT_FLAGS.MCHT_CALENDARWEEKNUM => HitArea.WeekNumbers,
        MCHITTESTINFO_HIT_FLAGS.MCHT_TODAYLINK => HitArea.TodayLink,
        _ => HitArea.Nowhere,
    };

    private static int GetIndexInMonths(DateTime startDate, DateTime currentDate)
        => (currentDate.Year - startDate.Year) * MonthsInYear + currentDate.Month - startDate.Month;

    private Size GetMinReqRect() => GetMinReqRect(0, false, false);

    /// <summary>
    ///  Used internally to get the minimum size needed to display the MonthCalendar.
    ///  This is needed because PInvoke.MCM_GETMINREQRECT returns an incorrect value if
    ///  showToday is set to false. If updateRows is true, then the number of
    ///  rows will be updated according to height.
    /// </summary>
    private Size GetMinReqRect(int newDimensionLength, bool updateRows, bool updateCols)
    {
        Size minSize = SingleMonthSize;

        // Calculate calendar height
        Size textExtent;

        using (var hfont = GdiCache.GetHFONT(Font))
        using (var screen = GdiCache.GetScreenHdc())
        {
            // this is the string that Windows uses to determine the extent of the today string
            textExtent = screen.HDC.GetTextExtent(DateTime.Now.ToShortDateString(), hfont);
        }

        int todayHeight = textExtent.Height + 4;  // The constant 4 is from the comctl32 MonthCalendar source code
        int calendarHeight = minSize.Height;
        if (ShowToday)
        {
            // If ShowToday is true, then minSize already includes the height of the today string.
            // So we remove it to get the actual calendar height.
            calendarHeight -= todayHeight;
        }

        if (updateRows)
        {
            if (calendarHeight + InsertHeightSize == 0)
            {
                _dimensions.Height = 1;
            }
            else
            {
                int nRows = (newDimensionLength - todayHeight + InsertHeightSize) / (calendarHeight + InsertHeightSize);
                _dimensions.Height = (nRows < 1) ? 1 : nRows;
            }
        }

        if (updateCols)
        {
            if (minSize.Width == 0)
            {
                _dimensions.Width = 1;
            }
            else
            {
                int nCols = (newDimensionLength - _extraPadding) / minSize.Width;
                _dimensions.Width = (nCols < 1) ? 1 : nCols;
            }
        }

        minSize.Width = (minSize.Width + InsertWidthSize) * _dimensions.Width - InsertWidthSize;
        minSize.Height = (calendarHeight + InsertHeightSize) * _dimensions.Height - InsertHeightSize + todayHeight;

        // If the width we've calculated is too small to fit the Today string, enlarge the width to fit
        if (IsHandleCreated)
        {
            int maxTodayWidth = (int)PInvoke.SendMessage(this, PInvoke.MCM_GETMAXTODAYWIDTH);
            if (maxTodayWidth > minSize.Width)
            {
                minSize.Width = maxTodayWidth;
            }
        }

        // Fudge factor
        minSize.Width += _extraPadding;
        minSize.Height += _extraPadding;
        return minSize;
    }

    private SelectionRange GetMonthRange(uint flag)
    {
        Span<SYSTEMTIME> times = stackalloc SYSTEMTIME[2];
        PInvoke.SendMessage(this, PInvoke.MCM_GETMONTHRANGE, (WPARAM)(int)flag, ref times[0]);
        return new SelectionRange
        {
            Start = (DateTime)times[0],
            End = (DateTime)times[1]
        };
    }

    /// <summary>
    ///  Calculate the number of visible months, even though they may be partially visible.
    ///  It is necessary to send to Windows correct info about all bolded dates that are visible.
    ///  Get an index of the last month, that starts from 0, and add 1 to get months count.
    /// </summary>
    private static int GetMonthsCountOfRange(SelectionRange displayRange)
        => GetIndexInMonths(displayRange.Start, displayRange.End) + 1;

    /// <summary>
    ///  Called by SetBoundsCore. If updateRows is true, then the number of rows
    ///  will be updated according to height.
    /// </summary>
    private int GetPreferredHeight(int height, bool updateRows)
    {
        Size preferredSize = GetMinReqRect(height, updateRows, false);
        return preferredSize.Height;
    }

    /// <summary>
    ///  Called by SetBoundsCore. If updateCols is true, then the number of columns
    ///  will be updated according to width.
    /// </summary>
    private int GetPreferredWidth(int width, bool updateCols)
    {
        Size preferredSize = GetMinReqRect(width, false, updateCols);
        return preferredSize.Width;
    }

    /// <summary>
    ///  Determines which portion of a month calendar control is at a given point on the screen.
    /// </summary>
    public unsafe HitTestInfo HitTest(int x, int y)
    {
        MCHITTESTINFO mchi = new()
        {
            cbSize = (uint)sizeof(MCHITTESTINFO),
            pt = new Point(x, y),
            st = default
        };

        PInvoke.SendMessage(this, PInvoke.MCM_HITTEST, 0, ref mchi);

        // If the hit area has an associated valid date, get it.
        HitArea hitArea = GetHitArea(mchi.uHit);
        if (HitTestInfo.HitAreaHasValidDateTime(hitArea))
        {
            SYSTEMTIME systemTime = new()
            {
                wYear = mchi.st.wYear,
                wMonth = mchi.st.wMonth,
                wDayOfWeek = mchi.st.wDayOfWeek,
                wDay = mchi.st.wDay,
                wHour = mchi.st.wHour,
                wMinute = mchi.st.wMinute,
                wSecond = mchi.st.wSecond,
                wMilliseconds = mchi.st.wMilliseconds
            };

            return new HitTestInfo(mchi.pt, hitArea, (DateTime)systemTime);
        }

        return new HitTestInfo(mchi.pt, hitArea);
    }

    /// <summary>
    ///  Determines which portion of a month calendar control is at a given point on the screen.
    /// </summary>
    public HitTestInfo HitTest(Point point) => HitTest(point.X, point.Y);

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

        SetSelRange(_selectionStart, _selectionEnd);
        if (_maxSelectionCount != DefaultMaxSelectionCount)
        {
            PInvoke.SendMessage(this, PInvoke.MCM_SETMAXSELCOUNT, (WPARAM)_maxSelectionCount);
        }

        AdjustSize();

        if (_todayDateSet)
        {
            SYSTEMTIME systemTime = (SYSTEMTIME)_todaysDate;
            PInvoke.SendMessage(this, PInvoke.MCM_SETTODAY, (WPARAM)0, ref systemTime);
        }

        SetControlColor(PInvoke.MCSC_TEXT, ForeColor);
        SetControlColor(PInvoke.MCSC_MONTHBK, BackColor);
        SetControlColor(PInvoke.MCSC_TITLEBK, _titleBackColor);
        SetControlColor(PInvoke.MCSC_TITLETEXT, _titleForeColor);
        SetControlColor(PInvoke.MCSC_TRAILINGTEXT, _trailingForeColor);

        int firstDay;
        if (_firstDayOfWeek == Day.Default)
        {
            firstDay = (int)PInvoke.LCTYPE.IFIRSTDAYOFWEEK;
        }
        else
        {
            firstDay = (int)_firstDayOfWeek;
        }

        PInvoke.SendMessage(this, PInvoke.MCM_SETFIRSTDAYOFWEEK, (WPARAM)0, (LPARAM)firstDay);

        SetRange();
        if (_scrollChange != DefaultScrollChange)
        {
            PInvoke.SendMessage(this, PInvoke.MCM_SETMONTHDELTA, (WPARAM)_scrollChange);
        }

        SystemEvents.UserPreferenceChanged += MarshaledUserPreferenceChanged;
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        SystemEvents.UserPreferenceChanged -= MarshaledUserPreferenceChanged;
        base.OnHandleDestroyed(e);
    }

    /// <summary>
    ///  Fires the event indicating that the currently
    ///  calendar view has changed.
    /// </summary>
    private void OnCalendarViewChanged(EventArgs e)
        => _onCalendarViewChanged?.Invoke(this, e);

    /// <summary>
    ///  Fires the event indicating that the currently selected date
    ///  or range of dates has changed.
    /// </summary>
    protected virtual void OnDateChanged(DateRangeEventArgs drevent)
        => _onDateChanged?.Invoke(this, drevent);

    /// <summary>
    ///  Fires the event indicating that the user has changed the selection.
    /// </summary>
    protected virtual void OnDateSelected(DateRangeEventArgs drevent)
        => _onDateSelected?.Invoke(this, drevent);

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);

        if (IsAccessibilityObjectCreated)
        {
            ((MonthCalendarAccessibleObject)AccessibilityObject).FocusedCell?.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
        }
    }

    /// <summary>
    ///  Fires the event indicating that the current display range is changed.
    /// </summary>
    private void OnDisplayRangeChanged(EventArgs e)
        => _onDisplayRangeChanged?.Invoke(this, e);

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        AdjustSize();
    }

    protected override void OnForeColorChanged(EventArgs e)
    {
        base.OnForeColorChanged(e);
        SetControlColor(PInvoke.MCSC_TEXT, ForeColor);
    }

    protected override void OnBackColorChanged(EventArgs e)
    {
        base.OnBackColorChanged(e);
        SetControlColor(PInvoke.MCSC_MONTHBK, BackColor);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        UpdateDisplayRange();
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

    internal override void ReleaseUiaProvider(HWND handle)
    {
        if (OsVersion.IsWindows8OrGreater() && IsAccessibilityObjectCreated)
        {
            (AccessibilityObject as MonthCalendarAccessibleObject)?.DisconnectChildren();
        }

        base.ReleaseUiaProvider(handle);
    }

    /// <summary>
    ///  Removes all annually bolded days. Be sure to call UpdateBoldedDates() afterwards.
    /// </summary>
    public void RemoveAllAnnuallyBoldedDates()
    {
        _annualBoldDates.Clear();

        for (int i = 0; i < MonthsInYear; ++i)
        {
            _monthsOfYear[i] = 0;
        }
    }

    /// <summary>
    ///  Removes all the bolded days. Be sure to call UpdateBoldedDates() afterwards.
    /// </summary>
    public void RemoveAllBoldedDates() => _boldDates.Clear();

    /// <summary>
    ///  Removes all monthly bolded days. Be sure to call UpdateBoldedDates() afterwards.
    /// </summary>
    public void RemoveAllMonthlyBoldedDates()
    {
        _monthlyBoldDates.Clear();
        _datesToBoldMonthly = 0;
    }

    /// <summary>
    ///  Removes an annually bolded date. If the date is not found in the
    ///  bolded date list, then no action is taken. If date occurs more than
    ///  once in the bolded date list, then only the first date is removed. When
    ///  comparing dates, only the day and month are used. Be sure to call
    ///  UpdateBoldedDates afterwards.
    /// </summary>
    public void RemoveAnnuallyBoldedDate(DateTime date)
    {
        int length = _annualBoldDates.Count;
        int i = 0;
        for (; i < length; ++i)
        {
            if (CompareDayAndMonth(_annualBoldDates[i], date))
            {
                _annualBoldDates.RemoveAt(i);
                break;
            }
        }

        --length;
        for (int j = i; j < length; ++j)
        {
            if (CompareDayAndMonth(_annualBoldDates[j], date))
            {
                return;
            }
        }

        _monthsOfYear[date.Month - 1] &= ~(0x00000001 << (date.Day - 1));
    }

    /// <summary>
    ///  Removes a bolded date. If the date is not found in the bolded date list,
    ///  then no action is taken. If date occurs more than once in the bolded
    ///  date list, then only the first date is removed.
    ///  Be sure to call UpdateBoldedDates() afterwards.
    /// </summary>
    public void RemoveBoldedDate(DateTime date)
    {
        DateTime toRemove = date.Date;
        for (int i = 0; i < _boldDates.Count; i++)
        {
            if (DateTime.Compare(_boldDates[i].Date, toRemove) == 0)
            {
                _boldDates.RemoveAt(i);
                return;
            }
        }
    }

    /// <summary>
    ///  Removes a monthly bolded date. If the date is not found in the
    ///  bolded date list, then no action is taken. If date occurs more than
    ///  once in the bolded date list, then only the first date is removed. When
    ///  comparing dates, only the day and month are used. Be sure to call
    ///  UpdateBoldedDates afterwards.
    /// </summary>
    public void RemoveMonthlyBoldedDate(DateTime date)
    {
        int length = _monthlyBoldDates.Count;
        int i = 0;
        for (; i < length; ++i)
        {
            if (CompareDayAndMonth(_monthlyBoldDates[i], date))
            {
                _monthlyBoldDates.RemoveAt(i);
                break;
            }
        }

        --length;
        for (int j = i; j < length; ++j)
        {
            if (CompareDayAndMonth(_monthlyBoldDates[j], date))
            {
                return;
            }
        }

        _datesToBoldMonthly &= ~(0x00000001 << (date.Day - 1));
    }

    private void ResetAnnuallyBoldedDates() => _annualBoldDates.Clear();

    private void ResetBoldedDates() => _boldDates.Clear();

    private void ResetCalendarDimensions() => CalendarDimensions = new Size(1, 1);

    /// <summary>
    ///  Resets the maximum selectable date. By default value, there is no
    ///  upper limit.
    /// </summary>
    private void ResetMaxDate() => MaxDate = DateTime.MaxValue;

    /// <summary>
    ///  Resets the minimum selectable date. By default value, there is no
    ///  lower limit.
    /// </summary>
    private void ResetMinDate() => MinDate = DateTime.MinValue;

    private void ResetMonthlyBoldedDates() => _monthlyBoldDates.Clear();

    /// <summary>
    ///  Resets the limits of the selection range. By default value, the upper
    ///  and lower limit is the current date.
    /// </summary>
    private void ResetSelectionRange() => SetSelectionRange(Now, Now);

    private void ResetTrailingForeColor() => TrailingForeColor = s_trailingForeColor;

    private void ResetTitleForeColor() => TitleForeColor = s_defaultTitleForeColor;

    private void ResetTitleBackColor() => TitleBackColor = s_defaultTitleBackColor;

    /// <summary>
    ///  Resets the "today"'s date. By default value, "today" is the
    ///  current date (and is automatically updated when the clock crosses
    ///  over to the next day).
    ///  If you set the today date yourself (using the TodayDate property)
    ///  the control will no longer automatically update the current day
    ///  for you. To re-enable this behavior, ResetTodayDate() is used.
    /// </summary>
    private void ResetTodayDate()
    {
        _todayDateSet = false;
        UpdateTodayDate();
    }

    /// <summary>
    ///  Overrides Control.SetBoundsCore to enforce auto-sizing.
    /// </summary>
    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        Rectangle oldBounds = Bounds;
        Size max = SystemInformation.MaxWindowTrackSize;

        // Second argument to GetPreferredWidth and GetPreferredHeight is a boolean specifying if we should update the number of rows/columns.
        // We only want to update the number of rows/columns if we are not currently being scaled.
        bool updateRowsAndColumns = !ScaleHelper.IsScalingRequirementMet || !ScalingInProgress;

        if (width != oldBounds.Width)
        {
            if (width > max.Width)
            {
                width = max.Width;
            }

            width = GetPreferredWidth(width, updateRowsAndColumns);
        }

        if (height != oldBounds.Height)
        {
            if (height > max.Height)
            {
                height = max.Height;
            }

            height = GetPreferredHeight(height, updateRowsAndColumns);
        }

        base.SetBoundsCore(x, y, width, height, specified);
    }

    /// <summary>
    ///  If the handle has been created, this applies the color to the control
    /// </summary>
    private void SetControlColor(uint colorIndex, Color value)
    {
        if (IsHandleCreated)
        {
            PInvoke.SendMessage(this, PInvoke.MCM_SETCOLOR, (WPARAM)(int)colorIndex, (LPARAM)value);
        }
    }

    /// <summary>
    ///  Updates the window handle with the min/max ranges if it has been created.
    /// </summary>
    private void SetRange()
    {
        SetRange(DateTimePicker.EffectiveMinDate(_minDate), DateTimePicker.EffectiveMaxDate(_maxDate));
    }

    private void SetRange(DateTime minDate, DateTime maxDate)
    {
        // Keep selection range within passed in minDate and maxDate
        if (_selectionStart < minDate)
        {
            _selectionStart = minDate;
        }

        if (_selectionStart > maxDate)
        {
            _selectionStart = maxDate;
        }

        if (_selectionEnd < minDate)
        {
            _selectionEnd = minDate;
        }

        if (_selectionEnd > maxDate)
        {
            _selectionEnd = maxDate;
        }

        if (_selectionStart > _focusedDate)
        {
            _focusedDate = _selectionStart.Date;
        }

        if (_selectionEnd < _focusedDate)
        {
            _focusedDate = _selectionEnd.Date;
        }

        SetSelRange(_selectionStart, _selectionEnd);

        // Updated the calendar range
        if (IsHandleCreated)
        {
            Span<SYSTEMTIME> times = [(SYSTEMTIME)minDate, (SYSTEMTIME)maxDate];
            uint flags = PInvoke.GDTR_MIN | PInvoke.GDTR_MAX;
            if (PInvoke.SendMessage(this, PInvoke.MCM_SETRANGE, (WPARAM)flags, ref times[0]) == 0)
            {
                throw new InvalidOperationException(
                    string.Format(SR.MonthCalendarRange, minDate.ToShortDateString(), maxDate.ToShortDateString()));
            }

            UpdateDisplayRange();
        }
    }

    /// <summary>
    ///  Sets the number of columns and rows to display.
    /// </summary>
    public void SetCalendarDimensions(int x, int y)
    {
        if (x < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(x), string.Format(SR.MonthCalendarInvalidDimensions, (x).ToString("D", CultureInfo.CurrentCulture), (y).ToString("D", CultureInfo.CurrentCulture)));
        }

        if (y < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(y), string.Format(SR.MonthCalendarInvalidDimensions, (x).ToString("D", CultureInfo.CurrentCulture), (y).ToString("D", CultureInfo.CurrentCulture)));
        }

        // MonthCalendar limits the dimensions to x * y <= 12
        // i.e. a maximum of twelve months can be displayed at a time
        // The following code emulates what is done inside monthcalendar (in comctl32.dll):
        // The dimensions are gradually reduced until the inequality above holds.
        //
        while (x * y > 12)
        {
            if (x > y)
            {
                x--;
            }
            else
            {
                y--;
            }
        }

        if (_dimensions.Width != x || _dimensions.Height != y)
        {
            _dimensions.Width = x;
            _dimensions.Height = y;
            AdjustSize();
        }
    }

    /// <summary>
    ///  Sets date as the current selected date. The start and begin of
    ///  the selection range will both be equal to date.
    /// </summary>
    public void SetDate(DateTime date)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(date, _minDate);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(date, _maxDate);

        SetSelectionRange(date, date);
    }

    /// <summary>
    ///  Update states of displayed dates. Make the native control redraw bolded dates.
    /// </summary>
    private unsafe void SetMonthViewBoldedDates()
    {
        Debug.Assert(_mcCurView == MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH, "This logic should work only in the Month view.");

        // Get the first and the last visible dates even they are in not fully displayed months
        SelectionRange displayRange = GetDisplayRange(false);
        int monthsCount = GetMonthsCountOfRange(displayRange);

        // Create a special collection for storage states of dates of some displayed month.
        // This collection will be send to Windows to update displayed dates states - bolded/unbolded.
        Span<uint> monthDayStates = stackalloc uint[monthsCount];
        // Run through all displayed bolded dates and fill the Span collection
        FillMonthDayStates(monthDayStates, displayRange);

        fixed (uint* arr = monthDayStates)
        {
            // Update display dates states.
            // For more info see docs: https://docs.microsoft.com/windows/win32/controls/mcm-setdaystate
            PInvoke.SendMessage(HWND, PInvoke.MCM_SETDAYSTATE, (WPARAM)monthsCount, (LPARAM)arr);
        }
    }

    /// <summary>
    ///  Sets the selection for a month calendar control to a given date range.
    ///  The selection range will not be set if the selection range exceeds the
    ///  maximum selection count.
    /// </summary>
    public void SetSelectionRange(DateTime date1, DateTime date2)
    {
        // Keep the dates within the min and max dates
        ArgumentOutOfRangeException.ThrowIfLessThan(date1, _minDate);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(date1, _maxDate);
        ArgumentOutOfRangeException.ThrowIfLessThan(date2, _minDate);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(date2, _maxDate);

        // If date1 > date2, we just select date2 (compat)
        if (date1 > date2)
        {
            date2 = date1;
        }

        // If the range exceeds maxSelectionCount, compare with the previous range and adjust whichever
        // limit hasn't changed.
        if ((date2 - date1).Days >= _maxSelectionCount)
        {
            if (date1.Ticks == _selectionStart.Ticks)
            {
                // Bring start date forward.
                date1 = date2.AddDays(1 - _maxSelectionCount);
            }
            else
            {
                // Bring end date back.
                date2 = date1.AddDays(_maxSelectionCount - 1);
            }
        }

        // Set the range.
        SetSelRange(date1, date2);
    }

    /// <summary>
    ///  <paramref name="upper"/> must be greater than <paramref name="lower"/>.
    /// </summary>
    private void SetSelRange(DateTime lower, DateTime upper)
    {
        Debug.Assert(lower.Ticks <= upper.Ticks, "lower must be less than upper");

        bool changed = false;
        if (_selectionStart != lower || _selectionEnd != upper)
        {
            changed = true;
            _selectionStart = lower;
            _selectionEnd = upper;
        }

        // Always set the value on the control, to ensure that it is up to date.
        if (IsHandleCreated)
        {
            Span<SYSTEMTIME> times = [(SYSTEMTIME)lower, (SYSTEMTIME)upper];
            PInvoke.SendMessage(this, PInvoke.MCM_SETSELRANGE, 0, ref times[0]);
        }

        if (changed)
        {
            OnDateChanged(new DateRangeEventArgs(lower, upper));
        }
    }

    private bool ShouldSerializeAnnuallyBoldedDates()
        => _annualBoldDates.Count > 0;

    private bool ShouldSerializeBoldedDates()
        => _boldDates.Count > 0;

    private bool ShouldSerializeCalendarDimensions()
        => !_dimensions.Equals(new Size(1, 1));

    private bool ShouldSerializeTrailingForeColor()
        => !TrailingForeColor.Equals(s_trailingForeColor);

    private bool ShouldSerializeTitleForeColor()
        => !TitleForeColor.Equals(s_defaultTitleForeColor);

    private bool ShouldSerializeTitleBackColor()
        => !TitleBackColor.Equals(s_defaultTitleBackColor);

    private bool ShouldSerializeMonthlyBoldedDates()
        => _monthlyBoldDates.Count > 0;

    /// <summary>
    ///  Retrieves true if the maxDate should be persisted in code gen.
    /// </summary>
    private bool ShouldSerializeMaxDate()
        => _maxDate != DateTimePicker.MaximumDateTime && _maxDate != DateTime.MaxValue;

    /// <summary>
    ///  Retrieves true if the minDate should be persisted in code gen.
    /// </summary>
    private bool ShouldSerializeMinDate()
        => _minDate != DateTimePicker.MinimumDateTime && _minDate != DateTime.MinValue;

    /// <summary>
    ///  Retrieves true if the selectionRange should be persisted in code gen.
    /// </summary>
    private bool ShouldSerializeSelectionRange()
        => !DateTime.Equals(_selectionEnd, _selectionStart);

    /// <summary>
    ///  Retrieves true if the todayDate should be persisted in code gen.
    /// </summary>
    private bool ShouldSerializeTodayDate() => _todayDateSet;

    /// <summary>
    ///  Returns a string representation for this control.
    /// </summary>
    public override string ToString()
    {
        string s = base.ToString();
        return $"{s}, {SelectionRange}";
    }

    /// <summary>
    ///  Forces month calendar to display the current set of bolded dates.
    /// </summary>
    public void UpdateBoldedDates()
    {
        if (IsHandleCreated && _mcCurView == MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH)
        {
            // Use a specific implementation in the Month view
            // to avoid MonthCalendar display dates range resets
            // and flickers when a MonthCalendar shows several months.
            SetMonthViewBoldedDates();
        }
    }

    /// <summary>
    ///  Updates the current display range of the calendar.
    ///  Includes gray dates of previous and next calendars.
    ///  This method is called when Size, MaxDate, MinDate, TodayDate,
    ///  the current calendar view are changed and
    ///  if Next and Previous buttons in the title is clicked.
    /// </summary>
    private void UpdateDisplayRange()
    {
        if (!IsHandleCreated)
        {
            return;
        }

        SelectionRange newRange = GetDisplayRange(false);

        if (_currentDisplayRange is null)
        {
            _currentDisplayRange = newRange;
            return;
        }

        if (_currentDisplayRange.Start != newRange.Start || _currentDisplayRange.End != newRange.End)
        {
            _currentDisplayRange = newRange;
            OnDisplayRangeChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    ///  Updates the current setting for "TODAY" in the MonthCalendar control
    ///  If the today date is set, the control will be set to that. Otherwise,
    ///  it will be set to null (running clock mode - the today date will be
    ///  automatically updated).
    /// </summary>
    private void UpdateTodayDate()
    {
        if (IsHandleCreated)
        {
            if (_todayDateSet)
            {
                SYSTEMTIME systemTime = (SYSTEMTIME)_todaysDate;
                PInvoke.SendMessage(this, PInvoke.MCM_SETTODAY, 0, ref systemTime);
            }
            else
            {
                PInvoke.SendMessage(this, PInvoke.MCM_SETTODAY, 0, 0);
            }
        }
    }

    private void MarshaledUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs pref)
    {
        try
        {
            // Use BeginInvoke instead of Invoke in case the destination thread is not processing messages.
            BeginInvoke(new UserPreferenceChangedEventHandler(UserPreferenceChanged), [sender, pref]);
        }
        catch (InvalidOperationException)
        {
            // If the destination thread does not exist, don't send.
        }
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
    ///  Handles the MCN_SELCHANGE notification
    /// </summary>
    private unsafe void WmDateChanged(ref Message m)
    {
        NMSELCHANGE* nmmcsc = (NMSELCHANGE*)(nint)m.LParamInternal;
        DateTime start = (DateTime)nmmcsc->stSelStart;
        DateTime end = (DateTime)nmmcsc->stSelEnd;

        // Windows doesn't provide API to get the focused cell.
        // To get the correct focused cell consider 3 cases:
        // 1) One cell is selected:
        //    In this case, the previous _selectionStart value changes.
        //    Moreover the selected start equals the end. So, take the start value as focused.
        // 2) Several cells are selected. Forward selection:
        //    It means that a user selects cells to the right (moves from early dates to late).
        //    In this case, the previous _selectionStart value doesn't change (start == _selectionStart)
        //    and _selectionEnd changes. So, the focused date is the end of the selection range.
        // 3) Several cells are selected. Backward selection:
        //    It means that a user selects cells to the left (moves from late dates to early).
        //    In this case, the previous _selectionStart value changes (start != _selectionStart)
        //    and _selectionEnd doesn't change. So, the focused date is the start of the selection range.
        _focusedDate = start == _selectionStart ? end.Date : start.Date;

        _selectionStart = start;
        _selectionEnd = end;

        AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
        AccessibilityNotifyClients(AccessibleEvents.ValueChange, -1);

        // We should use the Date for comparison in this case. The user can work in the calendar only with dates,
        // while the minimum / maximum date can contain the date and custom time, which, when comparing Ticks,
        // may lead to incorrect calculation.
        if (start.Date < _minDate.Date || end.Date < _minDate.Date)
        {
            // When calendar control is switched from a date display mode to year, decade or century mode, displayed range
            // is changed proportional to the scale change. Thus we need to enforce user-defined selection again.
            SetSelRange(_minDate, _minDate);
        }
        else if (start.Date > _maxDate.Date || end.Date > _maxDate.Date)
        {
            SetSelRange(_maxDate, _maxDate);
        }

        if (IsHandleCreated)
        {
            UpdateDisplayRange();
        }

        if (IsAccessibilityObjectCreated)
        {
            MonthCalendarAccessibleObject calendarAccessibleObject = (MonthCalendarAccessibleObject)AccessibilityObject;
            calendarAccessibleObject.RaiseAutomationEventForChild(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
        }

        OnDateChanged(new DateRangeEventArgs(start, end));
    }

    /// <summary>
    ///  Handles the MCN_GETDAYSTATE notification by returning an array of bitmasks, one entry per month,
    ///  that specifies which dates to display in bold.
    /// </summary>
    private unsafe void WmDateBold(ref Message m)
    {
        NMDAYSTATE* nmmcds = (NMDAYSTATE*)(nint)m.LParamInternal;
        Span<int> boldDates = new((int*)nmmcds->prgDayState, nmmcds->cDayState);
        WriteBoldDates(boldDates);
    }

    /// <summary>
    ///  Handles the MCN_VIEWCHANGE  notification
    /// </summary>
    private unsafe void WmCalViewChanged(ref Message m)
    {
        NMVIEWCHANGE* nmmcvm = (NMVIEWCHANGE*)(nint)m.LParamInternal;
        Debug.Assert(_mcCurView == nmmcvm->dwOldView, "Calendar view mode is out of sync with native control");
        if (_mcCurView != nmmcvm->dwNewView)
        {
            _mcOldView = _mcCurView;
            _mcCurView = nmmcvm->dwNewView;

            OnCalendarViewChanged(EventArgs.Empty);
            AccessibilityNotifyClients(AccessibleEvents.ValueChange, -1);
            AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
        }
    }

    /// <summary>
    ///  Handles the MCN_SELECT notification.
    /// </summary>
    private unsafe void WmDateSelected(ref Message m)
    {
        NMSELCHANGE* nmmcsc = (NMSELCHANGE*)(nint)m.LParamInternal;
        DateTime start = _selectionStart = (DateTime)nmmcsc->stSelStart;
        DateTime end = _selectionEnd = (DateTime)nmmcsc->stSelEnd;

        AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
        AccessibilityNotifyClients(AccessibleEvents.ValueChange, -1);

        // We should use the Date for comparison in this case. The user can work in the calendar only with dates,
        // while the minimum / maximum date can contain the date and custom time, which, when comparing Ticks,
        // may lead to incorrect calculation.
        if (start.Date < _minDate.Date || end.Date < _minDate.Date)
        {
            // When calendar control is switched from a date display mode to year, decade or century mode, displayed range
            // is changed proportional to the scale change. Thus we need to enforce user-defined selection again.
            SetSelRange(_minDate, _minDate);
        }
        else if (start.Date > _maxDate.Date || end.Date > _maxDate.Date)
        {
            SetSelRange(_maxDate, _maxDate);
        }

        OnDateSelected(new DateRangeEventArgs(start, end));
    }

    /// <summary>
    ///  Handles the WM_GETDLGCODE message.
    /// </summary>
    private static void WmGetDlgCode(ref Message m)
    {
        // The MonthCalendar does its own handling of arrow keys.
        m.ResultInternal = (LRESULT)(nint)PInvoke.DLGC_WANTARROWS;
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
                case PInvoke.MCN_SELECT:
                    WmDateSelected(ref m);
                    break;
                case PInvoke.MCN_SELCHANGE:
                    WmDateChanged(ref m);
                    break;
                case PInvoke.MCN_GETDAYSTATE:
                    WmDateBold(ref m);
                    UpdateDisplayRange();
                    break;
                case PInvoke.MCN_VIEWCHANGE:
                    WmCalViewChanged(ref m);
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
            case PInvoke.WM_GETDLGCODE:
                WmGetDlgCode(ref m);
                break;
            case MessageId.WM_REFLECT_NOTIFY:
                WmReflectCommand(ref m);
                base.WndProc(ref m);
                break;
            case PInvoke.WM_PAINT:
                base.WndProc(ref m);

                if (_mcCurView != MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH)
                {
                    // Check if the display range is changed and update it.
                    // Win32 doesn't provide a notification about the display range is changed,
                    // so we have to use PInvoke.WM_PAINT and check it manually in the Year, Decade and Century views.
                    // MCN.GETDAYSTATE handles the display range changes in the Month view.
                    UpdateDisplayRange();
                }

                break;
            default:
                base.WndProc(ref m);
                break;
        }
    }
}
