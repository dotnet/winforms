// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.Internal;
using System.Windows.Forms.Layout;
using Microsoft.Win32;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This control is an encapsulateion of the Windows month calendar control.
    ///  A month calendar control implements a calendar-like user interface, that
    ///  provides the user with a very intuitive and recognizable method of entering
    ///  or selecting a date.
    ///  Users can also select which days bold. The most efficient way to add the
    ///  bolded dates is via an array all at once. (The below descriptions can be applied
    ///  equally to annually and monthly bolded dates as well)
    ///  The following is an example of this:
    /// <code>
    ///  MonthCalendar mc = new MonthCalendar();
    ///  //     add specific dates to bold
    ///  DateTime[] time = new DateTime[3];
    ///  time[0] = DateTime.Now;
    ///  time[1] = time[0].addDays(2);
    ///  time[2] = time[1].addDays(2);
    ///  mc.setBoldedDates(time);
    /// </code>
    ///  Removal of all bolded dates is accomplished with:
    /// <code>
    ///  mc.removeAllBoldedDates();
    /// </code>
    ///  Although less efficient, the user may need to add or remove bolded dates one at
    ///  a time. To improve the performance of this, neither addBoldedDate nor
    ///  removeBoldedDate repaints the monthcalendar. The user must call UpdateBoldedDates
    ///  to force the repaint of the bolded dates, otherwise the monthCalendar will not
    ///  paint properly.
    ///  The following is an example of this:
    /// <code>
    ///  DateTime time1 = new DateTime("3/5/98");
    ///  DateTime time2 = new DateTime("4/19/98");
    ///  mc.addBoldedDate(time1);
    ///  mc.addBoldedDate(time2);
    ///  mc.removeBoldedDate(time1);
    ///  mc.UpdateBoldedDates();
    /// </code>
    ///  The same applies to addition and removal of annual and monthly bolded dates.
    /// </summary>
    [DefaultProperty(nameof(SelectionRange))]
    [DefaultEvent(nameof(DateChanged))]
    [DefaultBindingProperty(nameof(SelectionRange))]
    [Designer("System.Windows.Forms.Design.MonthCalendarDesigner, " + AssemblyRef.SystemDesign)]
    [SRDescription(nameof(SR.DescriptionMonthCalendar))]
    public partial class MonthCalendar : Control
    {
        private static readonly Color s_defaultTitleBackColor = SystemColors.ActiveCaption;
        private static readonly Color s_defaultTitleForeColor = SystemColors.ActiveCaptionText;
        private static readonly Color s_trailingForeColor = SystemColors.GrayText;
        private const int MinimumAllocSize = 12;  // minimum size to expand the buffer by
        private const int MonthsInYear = 12;

        /// <summary>
        ///  This is the arbitrary number of pixels that the Win32 control
        ///  inserts between calendars horizontally, regardless of font.
        /// </summary>
        private const int InsertWidthSize = 6;
        /// <summary>
        ///  This is the arbitrary number of pixels that the Win32 control
        ///  inserts between calendars vertically, regardless of font.
        /// </summary>
        private const int InsertHeightSize = 6;       // From comctl32 MonthCalendar sources CALBORDER

        private const Day DefaultFirstDayOfWeek = Day.Default;
        private const int DefaultMaxSelectionCount = 7;
        private const int DefaultScrollChange = 0;

        private static readonly Size s_defaultSingleMonthSize = new Size(176, 153);

        private const int MaxScrollChange = 20000;

        private const int ExtraPadding = 2;
        private int _scaledExtraPadding = ExtraPadding;

        private IntPtr _mdsBuffer;
        private int _mdsBufferSize;

        private Color _titleBackColor = s_defaultTitleBackColor;
        private Color _titleForeColor = s_defaultTitleForeColor;
        private Color _trailingForeColor = s_trailingForeColor;
        private bool _showToday = true;
        private bool _showTodayCircle = true;
        private bool _showWeekNumbers;
        private bool _rightToLeftLayout;

        private Size _dimensions = new Size(1, 1);
        private int _maxSelectionCount = DefaultMaxSelectionCount;
        private DateTime _maxDate = DateTime.MaxValue;
        private DateTime _minDate = DateTime.MinValue;
        private int _scrollChange = DefaultScrollChange;
        private bool _todayDateSet;
        private DateTime _todayDate = DateTime.Now.Date;
        private DateTime _selectionStart;
        private DateTime _selectionEnd;
        private Day _firstDayOfWeek = DefaultFirstDayOfWeek;
        private MCMV _mcCurView = MCMV.MONTH;
        private MCMV _mcOldView = MCMV.MONTH;

        /// <summary>
        ///  Bitmask for the annually bolded dates. Months start on January.
        /// </summary>
        private readonly int[] _monthsOfYear = new int[12];

        /// <summary>
        ///  Bitmask for the dates bolded monthly.
        /// </summary>
        private int _datesToBoldMonthly;

        /// <summary>
        ///  Lists are slow, so this section can be optimized.
        ///  Implementation is such that inserts are fast, removals are slow.
        /// </summary>
        private readonly List<DateTime> _arrayOfDates = new List<DateTime>();
        private readonly List<DateTime> _annualArrayOfDates = new List<DateTime>();
        private readonly List<DateTime> _monthlyArrayOfDates = new List<DateTime>();

        private DateRangeEventHandler _onDateChanged;
        private DateRangeEventHandler _onDateSelected;
        private EventHandler _onRightToLeftLayoutChanged;

        /// <summary>
        ///  Creates a new MonthCalendar object. Styles are the default for a regular
        ///  month calendar control.
        /// </summary>
        public MonthCalendar() : base()
        {
            PrepareForDrawing();

            _selectionStart = _todayDate;
            _selectionEnd = _todayDate;
            SetStyle(ControlStyles.UserPaint, false);
            SetStyle(ControlStyles.StandardClick, false);

            TabStop = true;
        }

        protected override AccessibleObject CreateAccessibilityInstance()
            => new MonthCalendarAccessibleObject(this);

        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
        {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            PrepareForDrawing();
        }

        private void PrepareForDrawing()
        {
            if (DpiHelper.IsScalingRequirementMet)
            {
                _scaledExtraPadding = LogicalToDeviceUnits(ExtraPadding);
            }
        }

        /// <summary>
        ///  The array of DateTime objects that determines which annual days are shown
        ///  in bold.
        /// </summary>
        [Localizable(true)]
        [SRDescription(nameof(SR.MonthCalendarAnnuallyBoldedDatesDescr))]
        public DateTime[] AnnuallyBoldedDates
        {
            get => _annualArrayOfDates.ToArray();

            set
            {
                _annualArrayOfDates.Clear();
                for (int i = 0; i < MonthsInYear; ++i)
                {
                    _monthsOfYear[i] = 0;
                }

                if (value != null && value.Length > 0)
                {
                    // Add each bolded date to our List.
                    foreach (var dateTime in value)
                    {
                        _annualArrayOfDates.Add(dateTime);
                        _monthsOfYear[dateTime.Month - 1] |= 0x00000001 << (dateTime.Day - 1);
                    }
                }

                RecreateHandle();
            }
        }

        [SRDescription(nameof(SR.MonthCalendarMonthBackColorDescr))]
        public override Color BackColor
        {
            get
            {
                if (ShouldSerializeBackColor())
                {
                    return base.BackColor;
                }

                return SystemColors.Window;
            }
            set => base.BackColor = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage
        {
            get => base.BackgroundImage;
            set => base.BackgroundImage = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler BackgroundImageChanged
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
        public new event EventHandler BackgroundImageLayoutChanged
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
            get => _arrayOfDates.ToArray();

            set
            {
                _arrayOfDates.Clear();
                if (value != null && value.Length > 0)
                {
                    // Add each bolded date to our List.
                    foreach (var dateTime in value)
                    {
                        _arrayOfDates.Add(dateTime);
                    }
                }

                RecreateHandle();
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
                cp.ClassName = WindowClasses.WC_MONTHCAL;
                cp.Style |= (int)MCS.MULTISELECT | (int)MCS.DAYSTATE;
                if (!_showToday)
                {
                    cp.Style |= (int)MCS.NOTODAY;
                }

                if (!_showTodayCircle)
                {
                    cp.Style |= (int)MCS.NOTODAYCIRCLE;
                }

                if (_showWeekNumbers)
                {
                    cp.Style |= (int)MCS.WEEKNUMBERS;
                }

                if (RightToLeft == RightToLeft.Yes && RightToLeftLayout)
                {
                    // We want to turn on mirroring for Form explicitly.
                    cp.ExStyle |= (int)User32.WS_EX.LAYOUTRTL;

                    // Don't need these styles when mirroring is turned on.
                    cp.ExStyle &= ~(int)(User32.WS_EX.RTLREADING | User32.WS_EX.RIGHT | User32.WS_EX.LEFTSCROLLBAR);
                }

                return cp;
            }
        }

        protected override ImeMode DefaultImeMode => ImeMode.Disable;

        protected override Padding DefaultMargin => new Padding(9);

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
                if (value < Day.Monday || value > Day.Default)
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
                        User32.SendMessageW(this, (User32.WM)ComCtl32.MCM.SETFIRSTDAYOFWEEK, IntPtr.Zero, (IntPtr)value);
                    }
                }
            }
        }

        [SRDescription(nameof(SR.MonthCalendarForeColorDescr))]
        public override Color ForeColor
        {
            get
            {
                if (ShouldSerializeForeColor())
                {
                    return base.ForeColor;
                }

                return SystemColors.WindowText;
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
        public new event EventHandler ImeModeChanged
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

                if (value < DateTimePicker.EffectiveMinDate(_minDate))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(MaxDate), FormatDate(value), nameof(MinDate)));
                }

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
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(MaxSelectionCount), value.ToString("D"), 1));
                }

                if (value == _maxSelectionCount)
                {
                    return;
                }

                if (IsHandleCreated)
                {
                    if (User32.SendMessageW(this, (User32.WM)ComCtl32.MCM.SETMAXSELCOUNT, (IntPtr)value) == IntPtr.Zero)
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

                if (value > DateTimePicker.EffectiveMaxDate(_maxDate))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidHighBoundArgument, nameof(MinDate), FormatDate(value), nameof(MaxDate)));
                }
                if (value < DateTimePicker.MinimumDateTime)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(MinDate), FormatDate(value), FormatDate(DateTimePicker.MinimumDateTime)));
                }

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
            get => _monthlyArrayOfDates.ToArray();

            set
            {
                _monthlyArrayOfDates.Clear();
                _datesToBoldMonthly = 0;

                if (value != null && value.Length > 0)
                {
                    // Add each bolded date to our List.
                    foreach (var  dateTime in value)
                    {
                        _monthlyArrayOfDates.Add(dateTime);
                        _datesToBoldMonthly |= 0x00000001 << (dateTime.Day - 1);
                    }
                }

                RecreateHandle();
            }
        }

        private DateTime Now => DateTime.Now.Date;

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
        public new event EventHandler PaddingChanged
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

                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(ScrollChange), value.ToString("D"), 0));
                }
                if (value > MaxScrollChange)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidHighBoundArgumentEx, nameof(ScrollChange), value.ToString("D"), MaxScrollChange.ToString("D")));
                }

                if (IsHandleCreated)
                {
                    User32.SendMessageW(this, (User32.WM)ComCtl32.MCM.SETMONTHDELTA, (IntPtr)value);
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

                if (value < MinDate)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(SelectionEnd), FormatDate(value), nameof(MinDate)));
                }
                if (value > MaxDate)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidHighBoundArgumentEx, nameof(SelectionEnd), FormatDate(value), nameof(MaxDate)));
                }

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

                if (value < _minDate)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(SelectionStart), FormatDate(value), nameof(MinDate)));
                }
                if (value > _maxDate)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidHighBoundArgumentEx, nameof(SelectionStart), FormatDate(value), nameof(MaxDate)));
                }

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
            get => new SelectionRange(SelectionStart, SelectionEnd);
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
                    RECT rect = new RECT();
                    if (User32.SendMessageW(this, (User32.WM)MCM.GETMINREQRECT, IntPtr.Zero, ref rect) == IntPtr.Zero)
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
        public override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler TextChanged
        {
            add => base.TextChanged += value;
            remove => base.TextChanged -= value;
        }

        /// <summary>
        ///  The date shown as "Today" in the Month Calendar control.
        ///  By default, "Today" is the current date at the time
        ///  the MonthCalendar control is created.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.MonthCalendarTodayDateDescr))]
        public DateTime TodayDate
        {
            get
            {
                if (_todayDateSet)
                {
                    return _todayDate;
                }

                if (IsHandleCreated)
                {
                    var st = new Kernel32.SYSTEMTIME();
                    int res = (int)User32.SendMessageW(this, (User32.WM)User32.MCM.GETTODAY, IntPtr.Zero, ref st);
                    Debug.Assert(res != 0, "MCM_GETTODAY failed");
                    return DateTimePicker.SysTimeToDateTime(st).Date;
                }

                return Now.Date;
            }
            set
            {
                if (!(_todayDateSet) || (DateTime.Compare(value, _todayDate) != 0))
                {
                    // throw if trying to set the TodayDate to a value greater than MaxDate
                    if (DateTime.Compare(value, _maxDate) > 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidHighBoundArgumentEx, nameof(TodayDate), FormatDate(value), FormatDate(_maxDate)));
                    }

                    // throw if trying to set the TodayDate to a value less than MinDate
                    if (DateTime.Compare(value, _minDate) < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgument, nameof(TodayDate), FormatDate(value), FormatDate(_minDate)));
                    }

                    _todayDate = value.Date;
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
                SetControlColor(MCSC.TITLEBK, value);
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
                SetControlColor(MCSC.TITLETEXT, value);
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
                SetControlColor(MCSC.TRAILINGTEXT, value);
            }
        }

        /// <summary>
        ///  Adds a day that will be bolded annually on the month calendar.
        ///  Be sure to call UpdateBoldedDates() afterwards.
        /// </summary>
        public void AddAnnuallyBoldedDate(DateTime date)
        {
            _annualArrayOfDates.Add(date);
            _monthsOfYear[date.Month - 1] |= 0x00000001 << (date.Day - 1);
        }

        /// <summary>
        ///  Adds a day that will be bolded on the month calendar.
        ///  Be sure to call UpdateBoldedDates() afterwards.
        /// </summary>
        public void AddBoldedDate(DateTime date)
        {
            if (!_arrayOfDates.Contains(date))
            {
                _arrayOfDates.Add(date);
            }
        }

        /// <summary>
        ///  Adds a day that will be bolded monthly on the month calendar.
        ///  Be sure to call UpdateBoldedDates() afterwards.
        /// </summary>
        public void AddMonthlyBoldedDate(DateTime date)
        {
            _monthlyArrayOfDates.Add(date);
            _datesToBoldMonthly |= 0x00000001 << (date.Day - 1);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler Click
        {
            add => base.Click += value;
            remove => base.Click -= value;
        }

        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.MonthCalendarOnDateChangedDescr))]
        public event DateRangeEventHandler DateChanged
        {
            add => _onDateChanged += value;
            remove => _onDateChanged -= value;
        }

        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.MonthCalendarOnDateSelectedDescr))]
        public event DateRangeEventHandler DateSelected
        {
            add => _onDateSelected += value;
            remove => _onDateSelected -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler DoubleClick
        {
            add => base.DoubleClick += value;
            remove => base.DoubleClick -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event MouseEventHandler MouseClick
        {
            add => base.MouseClick += value;
            remove => base.MouseClick -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event MouseEventHandler MouseDoubleClick
        {
            add => base.MouseDoubleClick += value;
            remove => base.MouseDoubleClick -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event PaintEventHandler Paint
        {
            add => base.Paint += value;
            remove => base.Paint -= value;
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
        public event EventHandler RightToLeftLayoutChanged
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

        /// <summary>
        ///  Event handler that bolds dates indicated by _arrayOfDates
        /// </summary>
        private void BoldDates(DateBoldEventArgs e)
        {
            int months = e.Size;
            e.DaysToBold = new int[months];
            SelectionRange range = GetDisplayRange(false);
            int startMonth = range.Start.Month;
            int startYear = range.Start.Year;
            int numDates = _arrayOfDates.Count;
            for (int i = 0; i < numDates; ++i)
            {
                DateTime date = _arrayOfDates[i];
                if (DateTime.Compare(date, range.Start) >= 0 && DateTime.Compare(date, range.End) <= 0)
                {
                    int month = date.Month;
                    int year = date.Year;
                    int index = (year == startYear) ? month - startMonth : month + year * MonthsInYear - startYear * MonthsInYear - startMonth;
                    e.DaysToBold[index] |= (0x00000001 << (date.Day - 1));
                }
            }

            // Now we figure out which monthly and annual dates to bold
            --startMonth;
            for (int i = 0; i < months; ++i, ++startMonth)
            {
                e.DaysToBold[i] |= _monthsOfYear[startMonth % MonthsInYear] | _datesToBoldMonthly;
            }
        }

        /// <summary>
        ///  Compares only the day and month of each time.
        /// </summary>
        private bool CompareDayAndMonth(DateTime t1, DateTime t2)
            => (t1.Day == t2.Day && t1.Month == t2.Month);

        protected override void CreateHandle()
        {
            if (!RecreatingHandle)
            {
                IntPtr userCookie = ThemingScope.Activate(Application.UseVisualStyles);
                try
                {
                    var icc = new INITCOMMONCONTROLSEX
                    {
                        dwICC = ICC.DATE_CLASSES
                    };
                    InitCommonControlsEx(ref icc);
                }
                finally
                {
                    ThemingScope.Deactivate(userCookie);
                }
            }

            base.CreateHandle();
        }

        /// <summary>
        ///  Called to cleanup a MonthCalendar. Normally you do not need
        ///  to call this as the garbage collector will cleanup the buffer
        ///  for you. However, there may be times when you may want to expedite
        ///  the garbage collectors cleanup.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (_mdsBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_mdsBuffer);
                _mdsBuffer = IntPtr.Zero;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        ///  Return a localized string representation of the given DateTime value.
        ///  Used for throwing exceptions, etc.
        /// </summary>
        private static string FormatDate(DateTime value)
            => value.ToString("d", CultureInfo.CurrentCulture);

        /// <summary>
        ///  Retrieves date information that represents the low and high limits of the
        ///  control's display.
        /// </summary>
        public SelectionRange GetDisplayRange(bool visible)
        {
            if (visible)
            {
                return GetMonthRange(GMR.VISIBLE);
            }

            return GetMonthRange(GMR.DAYSTATE);
        }

        /// <summary>
        ///  Retrieves the enumeration value corresponding to the hit area.
        /// </summary>
        private HitArea GetHitArea(MCHT hit)
        {
            switch (hit)
            {
                case MCHT.TITLEBK:
                    return HitArea.TitleBackground;
                case MCHT.TITLEMONTH:
                    return HitArea.TitleMonth;
                case MCHT.TITLEYEAR:
                    return HitArea.TitleYear;
                case MCHT.TITLEBTNNEXT:
                    return HitArea.NextMonthButton;
                case MCHT.TITLEBTNPREV:
                    return HitArea.PrevMonthButton;
                case MCHT.CALENDARBK:
                    return HitArea.CalendarBackground;
                case MCHT.CALENDARDATE:
                    return HitArea.Date;
                case MCHT.CALENDARDATENEXT:
                    return HitArea.NextMonthDate;
                case MCHT.CALENDARDATEPREV:
                    return HitArea.PrevMonthDate;
                case MCHT.CALENDARDAY:
                    return HitArea.DayOfWeek;
                case MCHT.CALENDARWEEKNUM:
                    return HitArea.WeekNumbers;
                case MCHT.TODAYLINK:
                    return HitArea.TodayLink;
                default:
                    return HitArea.Nowhere;
            }
        }

        private Size GetMinReqRect() => GetMinReqRect(0, false, false);

        /// <summary>
        ///  Used internally to get the minimum size needed to display the MonthCalendar.
        ///  This is needed because MCM.GETMINREQRECT returns an incorrect value if
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
                    int nCols = (newDimensionLength - _scaledExtraPadding) / minSize.Width;
                    _dimensions.Width = (nCols < 1) ? 1 : nCols;
                }
            }

            minSize.Width = (minSize.Width + InsertWidthSize) * _dimensions.Width - InsertWidthSize;
            minSize.Height = (calendarHeight + InsertHeightSize) * _dimensions.Height - InsertHeightSize + todayHeight;

            // If the width we've calculated is too small to fit the Today string, enlarge the width to fit
            if (IsHandleCreated)
            {
                int maxTodayWidth = unchecked((int)(long)User32.SendMessageW(this, (User32.WM)ComCtl32.MCM.GETMAXTODAYWIDTH));
                if (maxTodayWidth > minSize.Width)
                {
                    minSize.Width = maxTodayWidth;
                }
            }

            // Fudge factor
            minSize.Width += _scaledExtraPadding;
            minSize.Height += _scaledExtraPadding;
            return minSize;
        }

        private SelectionRange GetMonthRange(GMR flag)
        {
            Span<Kernel32.SYSTEMTIME> sa = stackalloc Kernel32.SYSTEMTIME[2];
            User32.SendMessageW(this, (User32.WM)MCM.GETMONTHRANGE, (IntPtr)flag, ref sa[0]);
            return new SelectionRange
            {
                Start = DateTimePicker.SysTimeToDateTime(sa[0]),
                End = DateTimePicker.SysTimeToDateTime(sa[1])
            };
        }

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
        ///  Determines which portion of a month calendar control is at at a given
        ///  point on the screen.
        /// </summary>
        public unsafe HitTestInfo HitTest(int x, int y)
        {
            var mchi = new MCHITTESTINFO
            {
                cbSize = (uint)sizeof(MCHITTESTINFO),
                pt = new Point(x, y),
                st = new Kernel32.SYSTEMTIME()
            };
            User32.SendMessageW(this, (User32.WM)MCM.HITTEST, IntPtr.Zero, ref mchi);

            // If the hit area has an associated valid date, get it
            HitArea hitArea = GetHitArea(mchi.uHit);
            if (HitTestInfo.HitAreaHasValidDateTime(hitArea))
            {
                var sys = new Kernel32.SYSTEMTIME
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
                return new HitTestInfo(mchi.pt, hitArea, DateTimePicker.SysTimeToDateTime(sys));
            }

            return new HitTestInfo(mchi.pt, hitArea);
        }

        /// <summary>
        ///  Determines which portion of a month calendar control is at at a given
        ///  point on the screen.
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

            switch (keyData & Keys.KeyCode)
            {
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.Home:
                case Keys.End:
                    return true;
            }

            return base.IsInputKey(keyData);
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
                User32.SendMessageW(this, (User32.WM)ComCtl32.MCM.SETMAXSELCOUNT, (IntPtr)_maxSelectionCount);
            }
            AdjustSize();

            if (_todayDateSet)
            {
                Kernel32.SYSTEMTIME st = DateTimePicker.DateTimeToSysTime(_todayDate);
                User32.SendMessageW(this, (User32.WM)User32.MCM.SETTODAY, IntPtr.Zero, ref st);
            }

            SetControlColor(MCSC.TEXT, ForeColor);
            SetControlColor(MCSC.MONTHBK, BackColor);
            SetControlColor(MCSC.TITLEBK, _titleBackColor);
            SetControlColor(MCSC.TITLETEXT, _titleForeColor);
            SetControlColor(MCSC.TRAILINGTEXT, _trailingForeColor);

            int firstDay;
            if (_firstDayOfWeek == Day.Default)
            {
                firstDay = (int)Kernel32.LCTYPE.IFIRSTDAYOFWEEK;
            }
            else
            {
                firstDay = (int)_firstDayOfWeek;
            }

            User32.SendMessageW(this, (User32.WM)ComCtl32.MCM.SETFIRSTDAYOFWEEK, IntPtr.Zero, (IntPtr)firstDay);

            SetRange();
            if (_scrollChange != DefaultScrollChange)
            {
                User32.SendMessageW(this, (User32.WM)ComCtl32.MCM.SETMONTHDELTA, (IntPtr)_scrollChange);
            }

            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(MarshaledUserPreferenceChanged);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(MarshaledUserPreferenceChanged);
            base.OnHandleDestroyed(e);
        }

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

            AccessibilityObject.RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            AdjustSize();
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            SetControlColor(MCSC.TEXT, ForeColor);
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            SetControlColor(MCSC.MONTHBK, BackColor);
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
        ///  Removes all annually bolded days. Be sure to call UpdateBoldedDates() afterwards.
        /// </summary>
        public void RemoveAllAnnuallyBoldedDates()
        {
            _annualArrayOfDates.Clear();
            for (int i = 0; i < MonthsInYear; ++i)
            {
                _monthsOfYear[i] = 0;
            }
        }

        /// <summary>
        ///  Removes all the bolded days. Be sure to call UpdateBoldedDates() afterwards.
        /// </summary>
        public void RemoveAllBoldedDates() => _arrayOfDates.Clear();

        /// <summary>
        ///  Removes all monthly bolded days. Be sure to call UpdateBoldedDates() afterwards.
        /// </summary>
        public void RemoveAllMonthlyBoldedDates()
        {
            _monthlyArrayOfDates.Clear();
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
            int length = _annualArrayOfDates.Count;
            int i = 0;
            for (; i < length; ++i)
            {
                if (CompareDayAndMonth(_annualArrayOfDates[i], date))
                {
                    _annualArrayOfDates.RemoveAt(i);
                    break;
                }
            }

            --length;
            for (int j = i; j < length; ++j)
            {
                if (CompareDayAndMonth(_annualArrayOfDates[j], date))
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
            int length = _arrayOfDates.Count;
            for (int i = 0; i < length; ++i)
            {
                if (DateTime.Compare(_arrayOfDates[i].Date, date.Date) == 0)
                {
                    _arrayOfDates.RemoveAt(i);
                    Invalidate();
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
            int length = _monthlyArrayOfDates.Count;
            int i = 0;
            for (; i < length; ++i)
            {
                if (CompareDayAndMonth(_monthlyArrayOfDates[i], date))
                {
                    _monthlyArrayOfDates.RemoveAt(i);
                    break;
                }
            }

            --length;
            for (int j = i; j < length; ++j)
            {
                if (CompareDayAndMonth(_monthlyArrayOfDates[j], date))
                {
                    return;
                }
            }

            _datesToBoldMonthly &= ~(0x00000001 << (date.Day - 1));
        }

        private void ResetAnnuallyBoldedDates() => _annualArrayOfDates.Clear();

        private void ResetBoldedDates() => _arrayOfDates.Clear();

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

        private void ResetMonthlyBoldedDates() => _monthlyArrayOfDates.Clear();

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
        ///  reqSize = # elements in int[] array
        ///
        ///  The size argument should be greater than 0.
        ///  Because of the nature of MonthCalendar, we can expect that
        ///  the requested size will not be ridiculously large, hence
        ///  it is not necessary to decrease the size of an allocated
        ///  block if the new requested size is smaller.
        /// </summary>
        private IntPtr RequestBuffer(int reqSize)
        {
            Debug.Assert(reqSize > 0, "Requesting a ridiculously small buffer");
            int intSize = 4;
            // if the current buffer size is insufficient...
            if (reqSize * intSize > _mdsBufferSize)
            {
                // free and expand the buffer,
                if (_mdsBuffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_mdsBuffer);
                    _mdsBuffer = IntPtr.Zero;
                }

                // Round up to the nearest multiple of MINIMUM_ALLOC_SIZE
                float quotient = (float)(reqSize - 1) / MinimumAllocSize;
                int actualSize = ((int)(quotient + 1)) * MinimumAllocSize;
                Debug.Assert(actualSize >= reqSize, "Tried to round up, but got it wrong");

                _mdsBufferSize = actualSize * intSize;
                _mdsBuffer = Marshal.AllocHGlobal(_mdsBufferSize);
            }

            return _mdsBuffer;
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
            bool updateRowsAndColumns = !DpiHelper.IsScalingRequirementMet || !IsCurrentlyBeingScaled;

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
        private void SetControlColor(MCSC colorIndex, Color value)
        {
            if (IsHandleCreated)
            {
                User32.SendMessageW(this, (User32.WM)ComCtl32.MCM.SETCOLOR, (IntPtr)colorIndex, PARAM.FromColor(value));
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
            SetSelRange(_selectionStart, _selectionEnd);

            // Updated the calendar range
            if (IsHandleCreated)
            {
                Span<Kernel32.SYSTEMTIME> sa = stackalloc Kernel32.SYSTEMTIME[2];
                sa[0] = DateTimePicker.DateTimeToSysTime(minDate);
                sa[1] = DateTimePicker.DateTimeToSysTime(maxDate);
                GDTR flags = GDTR.MIN | GDTR.MAX;
                if ((int)User32.SendMessageW(this, (User32.WM)MCM.SETRANGE, (IntPtr)flags, ref sa[0]) == 0)
                {
                    throw new InvalidOperationException(string.Format(SR.MonthCalendarRange, minDate.ToShortDateString(), maxDate.ToShortDateString()));
                }
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
            if (date.Ticks < _minDate.Ticks)
            {
                throw new ArgumentOutOfRangeException(nameof(date), date, string.Format(SR.InvalidLowBoundArgumentEx, nameof(date), FormatDate(date), nameof(MinDate)));
            }
            if (date.Ticks > _maxDate.Ticks)
            {
                throw new ArgumentOutOfRangeException(nameof(date), date, string.Format(SR.InvalidHighBoundArgumentEx, nameof(date), FormatDate(date), nameof(MaxDate)));
            }

            SetSelectionRange(date, date);
        }

        /// <summary>
        ///  Sets the selection for a month calendar control to a given date range.
        ///  The selection range will not be set if the selection range exceeds the
        ///  maximum selection count.
        /// </summary>
        public void SetSelectionRange(DateTime date1, DateTime date2)
        {
            // Keep the dates within the min and max dates
            if (date1.Ticks < _minDate.Ticks)
            {
                throw new ArgumentOutOfRangeException(nameof(date1), date1, string.Format(SR.InvalidLowBoundArgumentEx, nameof(SelectionStart), FormatDate(date1), nameof(MinDate)));
            }
            if (date1.Ticks > _maxDate.Ticks)
            {
                throw new ArgumentOutOfRangeException(nameof(date1), date1, string.Format(SR.InvalidHighBoundArgumentEx, nameof(SelectionEnd), FormatDate(date1), nameof(MaxDate)));
            }
            if (date2.Ticks < _minDate.Ticks)
            {
                throw new ArgumentOutOfRangeException(nameof(date2), date2, string.Format(SR.InvalidLowBoundArgumentEx, nameof(SelectionStart), FormatDate(date2), nameof(MinDate)));
            }
            if (date2.Ticks > _maxDate.Ticks)
            {
                throw new ArgumentOutOfRangeException(nameof(date2), date2, string.Format(SR.InvalidHighBoundArgumentEx, nameof(SelectionEnd), FormatDate(date2), nameof(MaxDate)));
            }

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
                    // Bring start date forward
                    date1 = date2.AddDays(1 - _maxSelectionCount);
                }
                else
                {
                    // Bring end date back
                    date2 = date1.AddDays(_maxSelectionCount - 1);
                }
            }

            // Set the range
            SetSelRange(date1, date2);
        }

        /// <summary>
        ///  Upper must be greater than Lower
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
                Span<Kernel32.SYSTEMTIME> sa = stackalloc Kernel32.SYSTEMTIME[2];
                sa[0] = DateTimePicker.DateTimeToSysTime(lower);
                sa[1] = DateTimePicker.DateTimeToSysTime(upper);
                User32.SendMessageW(this, (User32.WM)ComCtl32.MCM.SETSELRANGE, IntPtr.Zero, ref sa[0]);
            }

            if (changed)
            {
                OnDateChanged(new DateRangeEventArgs(lower, upper));
            }
        }

        private bool ShouldSerializeAnnuallyBoldedDates()
            => _annualArrayOfDates.Count > 0;

        private bool ShouldSerializeBoldedDates()
            => _arrayOfDates.Count > 0;

        private bool ShouldSerializeCalendarDimensions()
            => !_dimensions.Equals(new Size(1, 1));

        private bool ShouldSerializeTrailingForeColor()
            => !TrailingForeColor.Equals(s_trailingForeColor);

        private bool ShouldSerializeTitleForeColor()
            => !TitleForeColor.Equals(s_defaultTitleForeColor);

        private bool ShouldSerializeTitleBackColor()
            => !TitleBackColor.Equals(s_defaultTitleBackColor);

        private bool ShouldSerializeMonthlyBoldedDates()
            => _monthlyArrayOfDates.Count > 0;

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
            return s + ", " + SelectionRange.ToString();
        }

        /// <summary>
        ///  Forces month calendar to display the current set of bolded dates.
        /// </summary>
        public void UpdateBoldedDates() => RecreateHandle();

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
                    Kernel32.SYSTEMTIME st = DateTimePicker.DateTimeToSysTime(_todayDate);
                    User32.SendMessageW(this, (User32.WM)User32.MCM.SETTODAY, IntPtr.Zero, ref st);
                }
                else
                {
                    User32.SendMessageW(this, (User32.WM)User32.MCM.SETTODAY, IntPtr.Zero, IntPtr.Zero);
                }
            }
        }

        private void MarshaledUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs pref)
        {
            try
            {
                // Use BeginInvoke instead of Invoke in case the destination thread is not processing messages.
                BeginInvoke(new UserPreferenceChangedEventHandler(UserPreferenceChanged), new object[] { sender, pref });
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
            NMSELCHANGE* nmmcsc = (NMSELCHANGE*)m.LParam;
            DateTime start = _selectionStart = DateTimePicker.SysTimeToDateTime(nmmcsc->stSelStart);
            DateTime end = _selectionEnd = DateTimePicker.SysTimeToDateTime(nmmcsc->stSelEnd);

            AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
            AccessibilityNotifyClients(AccessibleEvents.ValueChange, -1);

            MonthCalendarAccessibleObject calendarAccessibleObject = (MonthCalendarAccessibleObject)AccessibilityObject;
            calendarAccessibleObject.RaiseAutomationEventForChild(UiaCore.UIA.AutomationFocusChangedEventId, _selectionStart, _selectionEnd);

            if (start.Ticks < _minDate.Ticks || end.Ticks < _minDate.Ticks)
            {
                SetSelRange(_minDate, _minDate);
            }
            else if (start.Ticks > _maxDate.Ticks || end.Ticks > _maxDate.Ticks)
            {
                SetSelRange(_maxDate, _maxDate);
            }

            OnDateChanged(new DateRangeEventArgs(start, end));
        }

        /// <summary>
        ///  Handles the MCN_GETDAYSTATE notification
        /// </summary>
        private unsafe void WmDateBold(ref Message m)
        {
            NMDAYSTATE* nmmcds = (NMDAYSTATE*)m.LParam;
            DateTime start = DateTimePicker.SysTimeToDateTime(nmmcds->stStart);
            DateBoldEventArgs boldEvent = new DateBoldEventArgs(start, nmmcds->cDayState);
            BoldDates(boldEvent);
            _mdsBuffer = RequestBuffer(boldEvent.Size);
            // copy boldEvent into mdsBuffer
            Marshal.Copy(boldEvent.DaysToBold, 0, _mdsBuffer, boldEvent.Size);
            // now we replug DateBoldEventArgs info into NMDAYSTATE
            nmmcds->prgDayState = _mdsBuffer;
        }

        /// <summary>
        ///  Handles the MCN_VIEWCHANGE  notification
        /// </summary>
        private unsafe void WmCalViewChanged(ref Message m)
        {
            NMVIEWCHANGE* nmmcvm = (NMVIEWCHANGE*)m.LParam;
            Debug.Assert(_mcCurView == nmmcvm->uOldView, "Calendar view mode is out of sync with native control");
            if (_mcCurView != nmmcvm->uNewView)
            {
                _mcOldView = _mcCurView;
                _mcCurView = nmmcvm->uNewView;

                AccessibilityNotifyClients(AccessibleEvents.ValueChange, -1);
                AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
            }
        }

        /// <summary>
        ///  Handles the MCN_SELECT notification
        /// </summary>
        private unsafe void WmDateSelected(ref Message m)
        {
            NMSELCHANGE* nmmcsc = (NMSELCHANGE*)m.LParam;
            DateTime start = _selectionStart = DateTimePicker.SysTimeToDateTime(nmmcsc->stSelStart);
            DateTime end = _selectionEnd = DateTimePicker.SysTimeToDateTime(nmmcsc->stSelEnd);

            AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
            AccessibilityNotifyClients(AccessibleEvents.ValueChange, -1);

            if (start.Ticks < _minDate.Ticks || end.Ticks < _minDate.Ticks)
            {
                SetSelRange(_minDate, _minDate);
            }
            else if (start.Ticks > _maxDate.Ticks || end.Ticks > _maxDate.Ticks)
            {
                SetSelRange(_maxDate, _maxDate);
            }

            OnDateSelected(new DateRangeEventArgs(start, end));
        }

        /// <summary>
        ///  Handles the WM_GETDLGCODE message
        /// </summary>
        private void WmGetDlgCode(ref Message m)
        {
            // The MonthCalendar does its own handling of arrow keys
            m.Result = (IntPtr)User32.DLGC.WANTARROWS;
        }

        /// <summary>
        ///  Handles the WM_COMMAND messages reflected from the parent control.
        /// </summary>
        private unsafe void WmReflectCommand(ref Message m)
        {
            if (m.HWnd == Handle)
            {
                User32.NMHDR* nmhdr = (User32.NMHDR*)m.LParam;
                switch ((MCN)nmhdr->code)
                {
                    case MCN.SELECT:
                        WmDateSelected(ref m);
                        break;
                    case MCN.SELCHANGE:
                        WmDateChanged(ref m);
                        break;
                    case MCN.GETDAYSTATE:
                        WmDateBold(ref m);
                        break;
                    case MCN.VIEWCHANGE:
                        WmCalViewChanged(ref m);
                        break;
                }
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch ((User32.WM)m.Msg)
            {
                case User32.WM.LBUTTONDOWN:
                    Focus();
                    if (!ValidationCancelled)
                    {
                        base.WndProc(ref m);
                    }
                    break;
                case User32.WM.GETDLGCODE:
                    WmGetDlgCode(ref m);
                    break;
                case User32.WM.REFLECT_NOTIFY:
                    WmReflectCommand(ref m);
                    base.WndProc(ref m);
                    break;
                case User32.WM.DESTROY:
                    base.WndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
