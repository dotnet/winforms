// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.Internal;
using System.Windows.Forms.Layout;
using Microsoft.Win32;

using static Interop;
using ArrayList = System.Collections.ArrayList;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This control is an encapsulateion of the Windows month calendar control.
    ///  A month calendar control implements a calendar-like user interface, that
    ///  provides the user with a very intuitive and recognizable method of entering
    ///  or selecting a date.
    ///  Users can also select which days bold.  The most efficient way to add the
    ///  bolded dates is via an array all at once.  (The below descriptions can be applied
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
    ///  a time.  To improve the performance of this, neither addBoldedDate nor
    ///  removeBoldedDate repaints the monthcalendar.  The user must call updateBoldedDates
    ///  to force the repaint of the bolded dates, otherwise the monthCalendar will not
    ///  paint properly.
    ///  The following is an example of this:
    /// <code>
    ///  DateTime time1 = new DateTime("3/5/98");
    ///  DateTime time2 = new DateTime("4/19/98");
    ///  mc.addBoldedDate(time1);
    ///  mc.addBoldedDate(time2);
    ///  mc.removeBoldedDate(time1);
    ///  mc.updateBoldedDates();
    /// </code>
    ///  The same applies to addition and removal of annual and monthly bolded dates.
    /// </summary>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultProperty(nameof(SelectionRange)),
    DefaultEvent(nameof(DateChanged)),
    DefaultBindingProperty(nameof(SelectionRange)),
    Designer("System.Windows.Forms.Design.MonthCalendarDesigner, " + AssemblyRef.SystemDesign),
    SRDescription(nameof(SR.DescriptionMonthCalendar))
    ]
    public partial class MonthCalendar : Control
    {
        const long DAYS_TO_1601 = 548229;
        const long DAYS_TO_10000 = 3615900;
        static readonly Color DEFAULT_TITLE_BACK_COLOR = SystemColors.ActiveCaption;
        static readonly Color DEFAULT_TITLE_FORE_COLOR = SystemColors.ActiveCaptionText;
        static readonly Color DEFAULT_TRAILING_FORE_COLOR = SystemColors.GrayText;
        private const int MINIMUM_ALLOC_SIZE = 12;  // minimum size to expand the buffer by
        private const int MONTHS_IN_YEAR = 12;
        /// <summary>
        ///  This is the arbitrary number of pixels that the Win32 control
        ///  inserts between calendars horizontally, regardless of font.
        /// </summary>
        private const int INSERT_WIDTH_SIZE = 6;
        /// <summary>
        ///  This is the arbitrary number of pixels that the Win32 control
        ///  inserts between calendars vertically, regardless of font.
        /// </summary>
        private const int INSERT_HEIGHT_SIZE = 6;       // From comctl32 MonthCalendar sources CALBORDER
        private const Day DEFAULT_FIRST_DAY_OF_WEEK = Day.Default;
        private const int DEFAULT_MAX_SELECTION_COUNT = 7;
        private const int DEFAULT_SCROLL_CHANGE = 0;
        private const int UNIQUE_DATE = 0;
        private const int ANNUAL_DATE = 1;
        private const int MONTHLY_DATE = 2;

        private static readonly Size DefaultSingleMonthSize = new Size(176, 153);

        private const int MaxScrollChange = 20000;

        private const int ExtraPadding = 2;
        private int scaledExtraPadding = ExtraPadding;

        private IntPtr mdsBuffer = IntPtr.Zero;
        private int mdsBufferSize = 0;

        // styles
        private Color titleBackColor = DEFAULT_TITLE_BACK_COLOR;
        private Color titleForeColor = DEFAULT_TITLE_FORE_COLOR;
        private Color trailingForeColor = DEFAULT_TRAILING_FORE_COLOR;
        private bool showToday = true;
        private bool showTodayCircle = true;
        private bool showWeekNumbers = false;
        private bool rightToLeftLayout = false;

        // properties
        private Size dimensions = new Size(1, 1);
        private int maxSelectionCount = DEFAULT_MAX_SELECTION_COUNT;
        // Reconcile out-of-range min/max values in the property getters.
        private DateTime maxDate = DateTime.MaxValue;
        private DateTime minDate = DateTime.MinValue;
        private int scrollChange = DEFAULT_SCROLL_CHANGE;
        private bool todayDateSet = false;           // Has TodayDate been explicitly set?
        private DateTime todayDate = DateTime.Now.Date;
        private DateTime selectionStart;
        private DateTime selectionEnd;
        private Day firstDayOfWeek = DEFAULT_FIRST_DAY_OF_WEEK;
        private NativeMethods.MONTCALENDAR_VIEW_MODE mcCurView = NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_MONTH;
        private NativeMethods.MONTCALENDAR_VIEW_MODE mcOldView = NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_MONTH;

        /// <summary>
        ///  Bitmask for the annually bolded dates.  Months start on January.
        /// </summary>
        private readonly int[] monthsOfYear = new int[12];
        /// <summary>
        ///  Bitmask for the dates bolded monthly.
        /// </summary>
        private int datesToBoldMonthly = 0;
        /// <summary>
        ///  Lists are slow, so this section can be optimized.
        ///  Implementation is such that inserts are fast, removals are slow.
        /// </summary>
        private readonly ArrayList arrayOfDates = new ArrayList();
        private readonly ArrayList annualArrayOfDates = new ArrayList(); // we have to maintain these lists too.
        private readonly ArrayList monthlyArrayOfDates = new ArrayList();

        // notifications
        private DateRangeEventHandler onDateChanged;
        private DateRangeEventHandler onDateSelected;
        private EventHandler onRightToLeftLayoutChanged;

        /// <summary>
        ///  Creates a new MonthCalendar object.  Styles are the default for a
        ///  regular month calendar control.
        /// </summary>
        public MonthCalendar()
        : base()
        {
            PrepareForDrawing();

            selectionStart = todayDate;
            selectionEnd = todayDate;
            SetStyle(ControlStyles.UserPaint, false);
            SetStyle(ControlStyles.StandardClick, false);

            TabStop = true;
        }

        /// <summary>
        ///  MonthCalendar control  accessbile object.
        /// </summary>
        /// <returns></returns>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new MonthCalendarAccessibleObject(this);
        }

        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
        {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            PrepareForDrawing();
        }

        private void PrepareForDrawing()
        {
            if (DpiHelper.IsScalingRequirementMet)
            {
                scaledExtraPadding = LogicalToDeviceUnits(ExtraPadding);
            }
        }

        /// <summary>
        ///  The array of DateTime objects that determines which annual days are shown
        ///  in bold.
        /// </summary>
        [
        Localizable(true),
        SRDescription(nameof(SR.MonthCalendarAnnuallyBoldedDatesDescr))
        ]
        public DateTime[] AnnuallyBoldedDates
        {
            get
            {
                DateTime[] dateTimes = new DateTime[annualArrayOfDates.Count];

                for (int i = 0; i < annualArrayOfDates.Count; ++i)
                {
                    dateTimes[i] = (DateTime)annualArrayOfDates[i];
                }
                return dateTimes;
            }
            set
            {
                //

                annualArrayOfDates.Clear();
                for (int i = 0; i < MONTHS_IN_YEAR; ++i)
                {
                    monthsOfYear[i] = 0;
                }

                if (value != null && value.Length > 0)
                {

                    //add each boldeddate to our ArrayList...
                    for (int i = 0; i < value.Length; i++)
                    {
                        annualArrayOfDates.Add(value[i]);
                    }

                    for (int i = 0; i < value.Length; ++i)
                    {
                        monthsOfYear[value[i].Month - 1] |= 0x00000001 << (value[i].Day - 1);
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
                else
                {
                    return SystemColors.Window;
                }
            }
            set
            {
                base.BackColor = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
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

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
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
        ///  The array of DateTime objects that determines which non-recurring
        ///  specified dates are shown in bold.
        /// </summary>
        /*Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),*/
        [Localizable(true)]
        public DateTime[] BoldedDates
        {
            get
            {
                DateTime[] dateTimes = new DateTime[arrayOfDates.Count];

                for (int i = 0; i < arrayOfDates.Count; ++i)
                {
                    dateTimes[i] = (DateTime)arrayOfDates[i];
                }
                return dateTimes;
            }
            set
            {
                //

                arrayOfDates.Clear();
                if (value != null && value.Length > 0)
                {

                    //add each boldeddate to our ArrayList...
                    for (int i = 0; i < value.Length; i++)
                    {
                        arrayOfDates.Add(value[i]);
                    }

                }
                RecreateHandle();
            }
        }

        /// <summary>
        ///  The number of columns and rows of months that will be displayed
        ///  in the MonthCalendar control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        SRDescription(nameof(SR.MonthCalendarDimensionsDescr))
        ]
        public Size CalendarDimensions
        {
            get
            {
                return dimensions;
            }
            set
            {
                if (!dimensions.Equals(value))
                {
                    SetCalendarDimensions(value.Width, value.Height);
                }
            }
        }

        /// <summary>
        ///  This is called when creating a window.  Inheriting classes can ovveride
        ///  this to add extra functionality, but should not forget to first call
        ///  base.getCreateParams() to make sure the control continues to work
        ///  correctly.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassName = NativeMethods.WC_MONTHCAL;
                cp.Style |= (int)ComCtl32.MCS.MULTISELECT | (int)ComCtl32.MCS.DAYSTATE;
                if (!showToday)
                {
                    cp.Style |= (int)ComCtl32.MCS.NOTODAY;
                }

                if (!showTodayCircle)
                {
                    cp.Style |= (int)ComCtl32.MCS.NOTODAYCIRCLE;
                }

                if (showWeekNumbers)
                {
                    cp.Style |= (int)ComCtl32.MCS.WEEKNUMBERS;
                }

                if (RightToLeft == RightToLeft.Yes && RightToLeftLayout == true)
                {
                    //We want to turn on mirroring for Form explicitly.
                    cp.ExStyle |= NativeMethods.WS_EX_LAYOUTRTL;
                    //Don't need these styles when mirroring is turned on.
                    cp.ExStyle &= ~(NativeMethods.WS_EX_RTLREADING | NativeMethods.WS_EX_RIGHT | NativeMethods.WS_EX_LEFTSCROLLBAR);
                }

                return cp;
            }
        }

        protected override ImeMode DefaultImeMode
        {
            get
            {
                return ImeMode.Disable;
            }
        }

        protected override Padding DefaultMargin
        {
            get { return new Padding(9); }
        }

        protected override Size DefaultSize
        {
            get
            {
                return GetMinReqRect();
            }
        }

        /// <summary>
        ///  This property is overridden and hidden from statement completion
        ///  on controls that are based on Win32 Native Controls.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override bool DoubleBuffered
        {
            get
            {
                return base.DoubleBuffered;
            }
            set
            {
                base.DoubleBuffered = value;
            }
        }

        /// <summary>
        ///  The first day of the week for the month calendar control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        DefaultValue(DEFAULT_FIRST_DAY_OF_WEEK),
        SRDescription(nameof(SR.MonthCalendarFirstDayOfWeekDescr))
        ]
        public Day FirstDayOfWeek
        {
            get
            {
                return firstDayOfWeek;
            }

            set
            {
                //valid values are 0x0 to 0x7
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)Day.Monday, (int)Day.Default))
                {
                    throw new InvalidEnumArgumentException(nameof(FirstDayOfWeek), (int)value, typeof(Day));
                }

                if (value != firstDayOfWeek)
                {
                    firstDayOfWeek = value;
                    if (IsHandleCreated)
                    {
                        if (value == Day.Default)
                        {
                            RecreateHandle();
                        }
                        else
                        {
                            SendMessage((int)ComCtl32.MCM.SETFIRSTDAYOFWEEK, 0, (int)value);
                        }
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
                else
                {
                    return SystemColors.WindowText;
                }
            }
            set
            {
                base.ForeColor = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public ImeMode ImeMode
        {
            get
            {
                return base.ImeMode;
            }
            set
            {
                base.ImeMode = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler ImeModeChanged
        {
            add => base.ImeModeChanged += value;
            remove => base.ImeModeChanged -= value;
        }

        /// <summary>
        ///  The maximum allowable date that can be selected.  By default, there
        ///  is no maximum date.  The maximum date is not set if max less than the
        ///  current minimum date.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MonthCalendarMaxDateDescr))
        ]
        public DateTime MaxDate
        {
            get
            {
                return DateTimePicker.EffectiveMaxDate(maxDate);
            }
            set
            {
                if (value != maxDate)
                {
                    if (value < DateTimePicker.EffectiveMinDate(minDate))
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(MaxDate), FormatDate(value), nameof(MinDate)));
                    }
                    maxDate = value;
                    SetRange();
                }
            }
        }

        /// <summary>
        ///  The maximum number of days that can be selected in a
        ///  month calendar control.  This method does not affect the current
        ///  selection range.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(DEFAULT_MAX_SELECTION_COUNT),
        SRDescription(nameof(SR.MonthCalendarMaxSelectionCountDescr))
        ]
        public int MaxSelectionCount
        {
            get
            {
                return maxSelectionCount;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(MaxSelectionCount), value.ToString("D"), 1));
                }

                if (value != maxSelectionCount)
                {
                    if (IsHandleCreated)
                    {
                        if (unchecked((int)(long)SendMessage((int)ComCtl32.MCM.SETMAXSELCOUNT, value, 0)) == 0)
                        {
                            throw new ArgumentException(string.Format(SR.MonthCalendarMaxSelCount, value.ToString("D")), nameof(value));
                        }
                    }
                    maxSelectionCount = value;
                }
            }
        }

        /// <summary>
        ///  The minimum allowable date that can be selected.  By default, there
        ///  is no minimum date.  The minimum date is not set if min greater than the
        ///  current maximum date.  MonthCalendar does not support dates prior to 1753.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MonthCalendarMinDateDescr))
        ]
        public DateTime MinDate
        {
            get
            {
                return DateTimePicker.EffectiveMinDate(minDate);
            }
            set
            {
                if (value != minDate)
                {
                    if (value > DateTimePicker.EffectiveMaxDate(maxDate))
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidHighBoundArgument, nameof(MinDate), FormatDate(value), nameof(MaxDate)));
                    }

                    // If trying to set the minimum less than DateTimePicker.MinimumDateTime, throw
                    // an exception.
                    if (value < DateTimePicker.MinimumDateTime)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(MinDate), FormatDate(value), FormatDate(DateTimePicker.MinimumDateTime)));
                    }

                    minDate = value;
                    SetRange();
                }
            }
        }

        /// <summary>
        ///  The array of DateTime objects that determine which monthly days to bold.
        /// </summary>
        [
        Localizable(true),
        SRDescription(nameof(SR.MonthCalendarMonthlyBoldedDatesDescr))
        ]
        public DateTime[] MonthlyBoldedDates
        {
            get
            {
                DateTime[] dateTimes = new DateTime[monthlyArrayOfDates.Count];

                for (int i = 0; i < monthlyArrayOfDates.Count; ++i)
                {
                    dateTimes[i] = (DateTime)monthlyArrayOfDates[i];
                }
                return dateTimes;
            }
            set
            {
                //

                monthlyArrayOfDates.Clear();
                datesToBoldMonthly = 0;

                if (value != null && value.Length > 0)
                {

                    //add each boldeddate to our ArrayList...
                    for (int i = 0; i < value.Length; i++)
                    {
                        monthlyArrayOfDates.Add(value[i]);
                    }

                    for (int i = 0; i < value.Length; ++i)
                    {
                        datesToBoldMonthly |= 0x00000001 << (value[i].Day - 1);
                    }

                }
                RecreateHandle();
            }
        }

        private DateTime Now
        {
            get
            {
                return DateTime.Now.Date;
            }
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

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public new event EventHandler PaddingChanged
        {
            add => base.PaddingChanged += value;
            remove => base.PaddingChanged -= value;
        }

        /// <summary>
        ///  This is used for international applications where the language
        ///  is written from RightToLeft. When this property is true,
        //      and the RightToLeft is true, mirroring will be turned on on the form, and
        ///  control placement and text will be from right to left.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        DefaultValue(false),
        SRDescription(nameof(SR.ControlRightToLeftLayoutDescr))
        ]
        public virtual bool RightToLeftLayout
        {
            get
            {

                return rightToLeftLayout;
            }

            set
            {
                if (value != rightToLeftLayout)
                {
                    rightToLeftLayout = value;
                    using (new LayoutTransaction(this, this, PropertyNames.RightToLeftLayout))
                    {
                        OnRightToLeftLayoutChanged(EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        ///  The scroll rate for a month calendar control. The scroll
        ///  rate is the number of months that the control moves its display
        ///  when the user clicks a scroll button.  If this value is zero,
        ///  the month delta is reset to the default, which is the number of
        ///  months displayed in the control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(DEFAULT_SCROLL_CHANGE),
        SRDescription(nameof(SR.MonthCalendarScrollChangeDescr))
        ]
        public int ScrollChange
        {
            get
            {
                return scrollChange;
            }
            set
            {
                if (scrollChange != value)
                {

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
                        SendMessage((int)ComCtl32.MCM.SETMONTHDELTA, value, 0);
                    }
                    scrollChange = value;
                }
            }
        }

        /// <summary>
        ///  Indicates the end date of the selected range of dates.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.MonthCalendarSelectionEndDescr))
        ]
        public DateTime SelectionEnd
        {
            get
            {
                return selectionEnd;
            }
            set
            {
                if (selectionEnd != value)
                {

                    // Keep SelectionEnd within min and max
                    if (value < MinDate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(SelectionEnd), FormatDate(value), nameof(MinDate)));
                    }
                    if (value > MaxDate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidHighBoundArgumentEx, nameof(SelectionEnd), FormatDate(value), nameof(MaxDate)));
                    }

                    // If we've moved SelectionEnd before SelectionStart, move SelectionStart back
                    if (selectionStart > value)
                    {
                        selectionStart = value;
                    }

                    // If we've moved SelectionEnd too far beyond SelectionStart, move SelectionStart forward
                    if ((value - selectionStart).Days >= maxSelectionCount)
                    {
                        selectionStart = value.AddDays(1 - maxSelectionCount);
                    }

                    // Set the new selection range
                    SetSelRange(selectionStart, value);
                }
            }
        }

        /// <summary>
        ///  Indicates
        ///  the start date of the selected range of dates.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.MonthCalendarSelectionStartDescr))
        ]
        public DateTime SelectionStart
        {
            get
            {
                return selectionStart;
            }
            set
            {
                if (selectionStart != value)
                {

                    // Keep SelectionStart within min and max
                    //
                    if (value < minDate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(SelectionStart), FormatDate(value), nameof(MinDate)));
                    }
                    if (value > maxDate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidHighBoundArgumentEx, nameof(SelectionStart), FormatDate(value), nameof(MaxDate)));
                    }

                    // If we've moved SelectionStart beyond SelectionEnd, move SelectionEnd forward
                    if (selectionEnd < value)
                    {
                        selectionEnd = value;
                    }

                    // If we've moved SelectionStart too far back from SelectionEnd, move SelectionEnd back
                    if ((selectionEnd - value).Days >= maxSelectionCount)
                    {
                        selectionEnd = value.AddDays(maxSelectionCount - 1);
                    }

                    // Set the new selection range
                    SetSelRange(value, selectionEnd);
                }
            }
        }

        /// <summary>
        ///  Retrieves the selection range for a month calendar control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MonthCalendarSelectionRangeDescr)),
        Bindable(true)
        ]
        public SelectionRange SelectionRange
        {
            get
            {
                return new SelectionRange(SelectionStart, SelectionEnd);
            }
            set
            {
                SetSelectionRange(value.Start, value.End);
            }
        }

        /// <summary>
        ///  Indicates whether the month calendar control will display
        ///  the "today" date at the bottom of the control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.MonthCalendarShowTodayDescr))
        ]
        public bool ShowToday
        {
            get
            {
                return showToday;
            }
            set
            {
                if (showToday != value)
                {
                    showToday = value;
                    UpdateStyles();
                    AdjustSize();
                }
            }
        }

        /// <summary>
        ///  Indicates whether the month calendar control will circle
        ///  the "today" date.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.MonthCalendarShowTodayCircleDescr))
        ]
        public bool ShowTodayCircle
        {
            get
            {
                return showTodayCircle;
            }
            set
            {
                if (showTodayCircle != value)
                {
                    showTodayCircle = value;
                    UpdateStyles();
                }
            }
        }

        /// <summary>
        ///  Indicates whether the month calendar control will the display
        ///  week numbers (1-52) to the left of each row of days.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        DefaultValue(false),
        SRDescription(nameof(SR.MonthCalendarShowWeekNumbersDescr))
        ]
        public bool ShowWeekNumbers
        {
            get
            {
                return showWeekNumbers;
            }
            set
            {
                if (showWeekNumbers != value)
                {
                    showWeekNumbers = value;
                    UpdateStyles();
                    AdjustSize();
                }
            }
        }

        /// <summary>
        ///  The minimum size required to display a full month.  The size
        ///  information is presented in the form of a Point, with the x
        ///  and y members representing the minimum width and height required
        ///  for the control.  The minimum required window size for a month calendar
        ///  control depends on the currently selected font.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.MonthCalendarSingleMonthSizeDescr))
        ]
        public Size SingleMonthSize
        {
            get
            {
                RECT rect = new RECT();

                if (IsHandleCreated)
                {

                    if (unchecked((int)(long)SendMessage((int)ComCtl32.MCM.GETMINREQRECT, 0, ref rect)) == 0)
                    {
                        throw new InvalidOperationException(SR.InvalidSingleMonthSize);
                    }

                    return new Size(rect.right, rect.bottom);
                }

                return DefaultSingleMonthSize;
            }
        }

        /// <summary>
        ///  Unlike most controls, serializing the MonthCalendar's Size is really bad:
        ///  when it's restored at runtime, it uses a a default SingleMonthSize, which
        ///  may not be right, especially for JPN/CHS machines.
        /// </summary>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Localizable(false)
        ]
        public new Size Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
            }
        }

        internal override bool SupportsUiaProviders => true;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Bindable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged
        {
            add => base.TextChanged += value;
            remove => base.TextChanged -= value;
        }

        /// <summary>
        ///  The date shown as "Today" in the Month Calendar control.
        ///  By default, "Today" is the current date at the time
        ///  the MonthCalendar control is created.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MonthCalendarTodayDateDescr))
        ]
        public DateTime TodayDate
        {
            get
            {
                if (todayDateSet)
                {
                    return todayDate;
                }

                if (IsHandleCreated)
                {
                    Kernel32.SYSTEMTIME st = new Kernel32.SYSTEMTIME();
                    int res = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), (int)ComCtl32.MCM.GETTODAY, 0, ref st);
                    Debug.Assert(res != 0, "MCM_GETTODAY failed");
                    return DateTimePicker.SysTimeToDateTime(st).Date;
                }
                else
                {
                    return Now.Date;
                }
            }
            set
            {
                if (!(todayDateSet) || (DateTime.Compare(value, todayDate) != 0))
                {

                    // throw if trying to set the TodayDate to a value greater than MaxDate
                    if (DateTime.Compare(value, maxDate) > 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidHighBoundArgumentEx, nameof(TodayDate), FormatDate(value), FormatDate(maxDate)));
                    }

                    // throw if trying to set the TodayDate to a value less than MinDate
                    if (DateTime.Compare(value, minDate) < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgument, nameof(TodayDate), FormatDate(value), FormatDate(minDate)));
                    }

                    todayDate = value.Date;
                    todayDateSet = true;
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
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.MonthCalendarTodayDateSetDescr))
        ]
        public bool TodayDateSet
        {
            get
            {
                return todayDateSet;
            }
        }

        /// <summary>
        ///  The background color displayed in the month calendar's
        ///  title.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.MonthCalendarTitleBackColorDescr))
        ]
        public Color TitleBackColor
        {
            get
            {
                return titleBackColor;
            }
            set
            {
                if (value.IsEmpty)
                {
                    throw new ArgumentException(string.Format(SR.InvalidNullArgument,
                                                              "value"));
                }
                titleBackColor = value;
                SetControlColor(ComCtl32.MCSC.TITLEBK, value);
            }
        }

        /// <summary>
        ///  The foreground color used to display text within the month
        ///  calendar's title.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.MonthCalendarTitleForeColorDescr))
        ]
        public Color TitleForeColor
        {
            get
            {
                return titleForeColor;
            }
            set
            {
                if (value.IsEmpty)
                {
                    throw new ArgumentException(string.Format(SR.InvalidNullArgument,
                                                              "value"));
                }
                titleForeColor = value;
                SetControlColor(ComCtl32.MCSC.TITLETEXT, value);
            }
        }

        /// <summary>
        ///  The color used to display the previous and following months that
        ///  appear on the current month calendar.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.MonthCalendarTrailingForeColorDescr))
        ]
        public Color TrailingForeColor
        {
            get
            {
                return trailingForeColor;
            }
            set
            {
                if (value.IsEmpty)
                {
                    throw new ArgumentException(string.Format(SR.InvalidNullArgument,
                                                              "value"));
                }
                trailingForeColor = value;
                SetControlColor(ComCtl32.MCSC.TRAILINGTEXT, value);
            }
        }

        /// <summary>
        ///  Adds a day that will be bolded annually on the month calendar.
        ///  Be sure to call updateBoldedDates() afterwards.
        /// </summary>
        public void AddAnnuallyBoldedDate(DateTime date)
        {
            annualArrayOfDates.Add(date);
            monthsOfYear[date.Month - 1] |= 0x00000001 << (date.Day - 1);
        }

        /// <summary>
        ///  Adds a day that will be bolded on the month calendar.
        ///  Be sure to call updateBoldedDates() afterwards.
        /// </summary>
        public void AddBoldedDate(DateTime date)
        {
            if (!arrayOfDates.Contains(date))
            {
                arrayOfDates.Add(date);
            }
        }

        /// <summary>
        ///  Adds a day that will be bolded monthly on the month calendar.
        ///  Be sure to call updateBoldedDates() afterwards.
        /// </summary>
        public void AddMonthlyBoldedDate(DateTime date)
        {
            monthlyArrayOfDates.Add(date);
            datesToBoldMonthly |= 0x00000001 << (date.Day - 1);
        }

        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler Click
        {
            add => base.Click += value;
            remove => base.Click -= value;
        }

        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.MonthCalendarOnDateChangedDescr))]
        public event DateRangeEventHandler DateChanged
        {
            add => onDateChanged += value;
            remove => onDateChanged -= value;
        }

        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.MonthCalendarOnDateSelectedDescr))]
        public event DateRangeEventHandler DateSelected
        {
            add => onDateSelected += value;
            remove => onDateSelected -= value;
        }

        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler DoubleClick
        {
            add => base.DoubleClick += value;
            remove => base.DoubleClick -= value;
        }

        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event MouseEventHandler MouseClick
        {
            add => base.MouseClick += value;
            remove => base.MouseClick -= value;
        }

        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event MouseEventHandler MouseDoubleClick
        {
            add => base.MouseDoubleClick += value;
            remove => base.MouseDoubleClick -= value;
        }

        /// <summary>
        ///  MonthCalendar Onpaint.
        /// </summary>
        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event PaintEventHandler Paint
        {
            add => base.Paint += value;
            remove => base.Paint -= value;
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
        public event EventHandler RightToLeftLayoutChanged
        {
            add => onRightToLeftLayoutChanged += value;
            remove => onRightToLeftLayoutChanged -= value;
        }

        /// <summary>
        ///  Used to auto-size the control.  The requested number of rows and columns are
        ///  restricted by the maximum size of the parent control, hence the requested number
        ///  of rows and columns may not be what you get.
        /// </summary>
        private void AdjustSize()
        {
            Size minSize = GetMinReqRect();
            Size = minSize;
        }

        /// <summary>
        ///  Event handler that bolds dates indicated by arrayOfDates
        /// </summary>
        private void BoldDates(DateBoldEventArgs e)
        {
            int months = e.Size;
            e.DaysToBold = new int[months];
            SelectionRange range = GetDisplayRange(false);
            int startMonth = range.Start.Month;
            int startYear = range.Start.Year;
            int numDates = arrayOfDates.Count;
            for (int i = 0; i < numDates; ++i)
            {
                DateTime date = (DateTime)arrayOfDates[i];
                if (DateTime.Compare(date, range.Start) >= 0 && DateTime.Compare(date, range.End) <= 0)
                {
                    int month = date.Month;
                    int year = date.Year;
                    int index = (year == startYear) ? month - startMonth : month + year * MONTHS_IN_YEAR - startYear * MONTHS_IN_YEAR - startMonth;
                    e.DaysToBold[index] |= (0x00000001 << (date.Day - 1));
                }
            }
            //now we figure out which monthly and annual dates to bold
            --startMonth;
            for (int i = 0; i < months; ++i, ++startMonth)
            {
                e.DaysToBold[i] |= monthsOfYear[startMonth % MONTHS_IN_YEAR] | datesToBoldMonthly;
            }
        }

        /// <summary>
        ///  Compares only the day and month of each time.
        /// </summary>
        private bool CompareDayAndMonth(DateTime t1, DateTime t2)
        {
            return (t1.Day == t2.Day && t1.Month == t2.Month);
        }

        protected override void CreateHandle()
        {
            if (!RecreatingHandle)
            {
                IntPtr userCookie = UnsafeNativeMethods.ThemingScope.Activate();
                try
                {
                    NativeMethods.INITCOMMONCONTROLSEX icc = new NativeMethods.INITCOMMONCONTROLSEX
                    {
                        dwICC = NativeMethods.ICC_DATE_CLASSES
                    };
                    SafeNativeMethods.InitCommonControlsEx(icc);
                }
                finally
                {
                    UnsafeNativeMethods.ThemingScope.Deactivate(userCookie);
                }
            }
            base.CreateHandle();
        }

        /// <summary>
        ///  Called to cleanup a MonthCalendar.  Normally you do not need
        ///  to call this as the garbage collector will cleanup the buffer
        ///  for you.  However, there may be times when you may want to expedite
        ///  the garbage collectors cleanup.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (mdsBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(mdsBuffer);
                mdsBuffer = IntPtr.Zero;
            }
            base.Dispose(disposing);
        }

        // Return a localized string representation of the given DateTime value.
        // Used for throwing exceptions, etc.
        //
        private static string FormatDate(DateTime value)
        {
            return value.ToString("d", CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///  Retrieves date information that represents the low and high limits of the
        ///  control's display.
        /// </summary>
        public SelectionRange GetDisplayRange(bool visible)
        {
            if (visible)
            {
                return GetMonthRange(NativeMethods.GMR_VISIBLE);
            }
            else
            {
                return GetMonthRange(NativeMethods.GMR_DAYSTATE);
            }
        }

        /// <summary>
        ///  Retrieves the enumeration value corresponding to the hit area.
        /// </summary>
        private HitArea GetHitArea(int hit)
        {
            switch ((ComCtl32.MCHT)hit)
            {
                case ComCtl32.MCHT.TITLEBK:
                    return HitArea.TitleBackground;
                case ComCtl32.MCHT.TITLEMONTH:
                    return HitArea.TitleMonth;
                case ComCtl32.MCHT.TITLEYEAR:
                    return HitArea.TitleYear;
                case ComCtl32.MCHT.TITLEBTNNEXT:
                    return HitArea.NextMonthButton;
                case ComCtl32.MCHT.TITLEBTNPREV:
                    return HitArea.PrevMonthButton;
                case ComCtl32.MCHT.CALENDARBK:
                    return HitArea.CalendarBackground;
                case ComCtl32.MCHT.CALENDARDATE:
                    return HitArea.Date;
                case ComCtl32.MCHT.CALENDARDATENEXT:
                    return HitArea.NextMonthDate;
                case ComCtl32.MCHT.CALENDARDATEPREV:
                    return HitArea.PrevMonthDate;
                case ComCtl32.MCHT.CALENDARDAY:
                    return HitArea.DayOfWeek;
                case ComCtl32.MCHT.CALENDARWEEKNUM:
                    return HitArea.WeekNumbers;
                case ComCtl32.MCHT.TODAYLINK:
                    return HitArea.TodayLink;
                default:
                    return HitArea.Nowhere;
            }
        }

        /// <summary>
        ///  stub for getMinReqRect (int, boolean)
        /// </summary>
        private Size GetMinReqRect()
        {
            return GetMinReqRect(0, false, false);
        }

        /// <summary>
        ///  Used internally to get the minimum size needed to display the
        ///  MonthCalendar. This is needed because
        ///  ComCtl32.MCM.GETMINREQRECT
        ///  returns an incorrect value if showToday
        ///  is set to false. If updateRows is true, then the
        ///  number of rows will be updated according to height.
        /// </summary>
        private Size GetMinReqRect(int newDimensionLength, bool updateRows, bool updateCols)
        {
            Size minSize = SingleMonthSize;

            // Calculate calendar height
            //
            Size textExtent;
            using (WindowsFont font = WindowsFont.FromFont(Font))
            {
                // this is the string that Windows uses to determine the extent of the today string
                textExtent = WindowsGraphicsCacheManager.MeasurementGraphics.GetTextExtent(DateTime.Now.ToShortDateString(), font);
            }
            int todayHeight = textExtent.Height + 4;  // The constant 4 is from the comctl32 MonthCalendar source code
            int calendarHeight = minSize.Height;
            if (ShowToday)
            {
                // If ShowToday is true, then minSize already includes the height of the today string.
                // So we remove it to get the actual calendar height.
                //
                calendarHeight -= todayHeight;
            }

            if (updateRows)
            {
                Debug.Assert(calendarHeight > INSERT_HEIGHT_SIZE, "Divide by 0");
                int nRows = (newDimensionLength - todayHeight + INSERT_HEIGHT_SIZE) / (calendarHeight + INSERT_HEIGHT_SIZE);
                dimensions.Height = (nRows < 1) ? 1 : nRows;
            }

            if (updateCols)
            {
                Debug.Assert(minSize.Width > INSERT_WIDTH_SIZE, "Divide by 0");
                int nCols = (newDimensionLength - scaledExtraPadding) / minSize.Width;
                dimensions.Width = (nCols < 1) ? 1 : nCols;
            }

            minSize.Width = (minSize.Width + INSERT_WIDTH_SIZE) * dimensions.Width - INSERT_WIDTH_SIZE;
            minSize.Height = (calendarHeight + INSERT_HEIGHT_SIZE) * dimensions.Height - INSERT_HEIGHT_SIZE + todayHeight;

            // If the width we've calculated is too small to fit the Today string, enlarge the width to fit
            //
            if (IsHandleCreated)
            {
                int maxTodayWidth = unchecked((int)(long)SendMessage((int)ComCtl32.MCM.GETMAXTODAYWIDTH, 0, 0));
                if (maxTodayWidth > minSize.Width)
                {
                    minSize.Width = maxTodayWidth;
                }
            }

            // Fudge factor
            //
            minSize.Width += scaledExtraPadding;
            minSize.Height += scaledExtraPadding;
            return minSize;
        }

        private SelectionRange GetMonthRange(int flag)
        {
            NativeMethods.SYSTEMTIMEARRAY sa = new NativeMethods.SYSTEMTIMEARRAY();
            SelectionRange range = new SelectionRange();
            UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), (int)ComCtl32.MCM.GETMONTHRANGE, flag, sa);

            Kernel32.SYSTEMTIME st = new Kernel32.SYSTEMTIME
            {
                wYear = sa.wYear1,
                wMonth = sa.wMonth1,
                wDayOfWeek = sa.wDayOfWeek1,
                wDay = sa.wDay1
            };

            range.Start = DateTimePicker.SysTimeToDateTime(st);
            st.wYear = sa.wYear2;
            st.wMonth = sa.wMonth2;
            st.wDayOfWeek = sa.wDayOfWeek2;
            st.wDay = sa.wDay2;
            range.End = DateTimePicker.SysTimeToDateTime(st);

            return range;
        }

        /// <summary>
        ///  Called by setBoundsCore.  If updateRows is true, then the
        ///  number of rows will be updated according to height.
        /// </summary>
        private int GetPreferredHeight(int height, bool updateRows)
        {
            Size preferredSize = GetMinReqRect(height, updateRows, false);
            return preferredSize.Height;
        }

        /// <summary>
        ///  Called by setBoundsCore.  If updateCols is true, then the
        ///  number of columns will be updated according to width.
        /// </summary>
        private int GetPreferredWidth(int width, bool updateCols)
        {
            Size preferredSize = GetMinReqRect(width, false, updateCols);
            return preferredSize.Width;
        }

        /// <summary>
        ///  Determines which portion of a month calendar control is at
        ///  at a given point on the screen.
        /// </summary>
        public HitTestInfo HitTest(int x, int y)
        {
            ComCtl32.MCHITTESTINFO mchi = new ComCtl32.MCHITTESTINFO
            {
                pt = new POINT
                {
                    x = x,
                    y = y
                },
                st = new Kernel32.SYSTEMTIME(),
                cbSize = Marshal.SizeOf<ComCtl32.MCHITTESTINFO>()
            };
            UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), (int)ComCtl32.MCM.HITTEST, 0, ref mchi);

            // If the hit area has an associated valid date, get it
            //
            HitArea hitArea = GetHitArea(mchi.uHit);
            if (HitTestInfo.HitAreaHasValidDateTime(hitArea))
            {
                Kernel32.SYSTEMTIME sys = new Kernel32.SYSTEMTIME
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
                return new HitTestInfo(new Point(mchi.pt.x, mchi.pt.y), hitArea, DateTimePicker.SysTimeToDateTime(sys));
            }
            else
            {
                return new HitTestInfo(new Point(mchi.pt.x, mchi.pt.y), hitArea);
            }
        }

        /// <summary>
        ///  Determines which portion of a month calendar control is at
        ///  at a given point on the screen.
        /// </summary>
        public HitTestInfo HitTest(Point point)
        {
            return HitTest(point.X, point.Y);
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

        /// <summary>
        ///  Overrides Control.OnHandleCreated()
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SetSelRange(selectionStart, selectionEnd);
            if (maxSelectionCount != DEFAULT_MAX_SELECTION_COUNT)
            {
                SendMessage((int)ComCtl32.MCM.SETMAXSELCOUNT, maxSelectionCount, 0);
            }
            AdjustSize();

            if (todayDateSet)
            {
                Kernel32.SYSTEMTIME st = DateTimePicker.DateTimeToSysTime(todayDate);
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), (int)ComCtl32.MCM.SETTODAY, 0, ref st);
            }

            SetControlColor(ComCtl32.MCSC.TEXT, ForeColor);
            SetControlColor(ComCtl32.MCSC.MONTHBK, BackColor);
            SetControlColor(ComCtl32.MCSC.TITLEBK, titleBackColor);
            SetControlColor(ComCtl32.MCSC.TITLETEXT, titleForeColor);
            SetControlColor(ComCtl32.MCSC.TRAILINGTEXT, trailingForeColor);

            int firstDay;
            if (firstDayOfWeek == Day.Default)
            {
                firstDay = NativeMethods.LOCALE_IFIRSTDAYOFWEEK;
            }
            else
            {
                firstDay = (int)firstDayOfWeek;
            }
            SendMessage((int)ComCtl32.MCM.SETFIRSTDAYOFWEEK, 0, firstDay);

            SetRange();
            if (scrollChange != DEFAULT_SCROLL_CHANGE)
            {
                SendMessage((int)ComCtl32.MCM.SETMONTHDELTA, scrollChange, 0);
            }

            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(MarshaledUserPreferenceChanged);
        }

        /// <summary>
        ///  Overrides Control.OnHandleDestroyed()
        /// </summary>
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
        {
            onDateChanged?.Invoke(this, drevent);
        }

        /// <summary>
        ///  Fires the event indicating that the user has changed his\her selection.
        /// </summary>
        protected virtual void OnDateSelected(DateRangeEventArgs drevent)
        {
            onDateSelected?.Invoke(this, drevent);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            AccessibilityObject.RaiseAutomationEvent(NativeMethods.UIA_AutomationFocusChangedEventId);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            AdjustSize();
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            SetControlColor(ComCtl32.MCSC.TEXT, ForeColor);
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            SetControlColor(ComCtl32.MCSC.MONTHBK, BackColor);
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

            onRightToLeftLayoutChanged?.Invoke(this, e);
        }

        /// <summary>
        ///  Removes all annually bolded days.  Be sure to call updateBoldedDates() afterwards.
        /// </summary>
        public void RemoveAllAnnuallyBoldedDates()
        {
            annualArrayOfDates.Clear();
            for (int i = 0; i < MONTHS_IN_YEAR; ++i)
            {
                monthsOfYear[i] = 0;
            }
        }

        /// <summary>
        ///  Removes all the bolded days.  Be sure to call updateBoldedDates() afterwards.
        /// </summary>
        public void RemoveAllBoldedDates()
        {
            arrayOfDates.Clear();
        }

        /// <summary>
        ///  Removes all monthly bolded days.  Be sure to call updateBoldedDates() afterwards.
        /// </summary>
        public void RemoveAllMonthlyBoldedDates()
        {
            monthlyArrayOfDates.Clear();
            datesToBoldMonthly = 0;
        }

        /// <summary>
        ///  Removes an annually bolded date.  If the date is not found in the
        ///  bolded date list, then no action is taken.  If date occurs more than
        ///  once in the bolded date list, then only the first date is removed.  When
        ///  comparing dates, only the day and month are used. Be sure to call
        ///  updateBoldedDates afterwards.
        /// </summary>
        public void RemoveAnnuallyBoldedDate(DateTime date)
        {
            int length = annualArrayOfDates.Count;
            int i = 0;
            for (; i < length; ++i)
            {
                if (CompareDayAndMonth((DateTime)annualArrayOfDates[i], date))
                {
                    annualArrayOfDates.RemoveAt(i);
                    break;
                }
            }
            --length;
            for (int j = i; j < length; ++j)
            {
                if (CompareDayAndMonth((DateTime)annualArrayOfDates[j], date))
                {
                    return;
                }
            }
            monthsOfYear[date.Month - 1] &= ~(0x00000001 << (date.Day - 1));
        }

        /// <summary>
        ///  Removes a bolded date.  If the date is not found in the
        ///  bolded date list, then no action is taken.  If date occurs more than
        ///  once in the bolded date list, then only the first date is removed.
        ///  Be sure to call updateBoldedDates() afterwards.
        /// </summary>
        public void RemoveBoldedDate(DateTime date)
        {
            int length = arrayOfDates.Count;
            for (int i = 0; i < length; ++i)
            {
                if (DateTime.Compare(((DateTime)arrayOfDates[i]).Date, date.Date) == 0)
                {
                    arrayOfDates.RemoveAt(i);
                    Invalidate();
                    return;
                }
            }
        }

        /// <summary>
        ///  Removes a monthly bolded date.  If the date is not found in the
        ///  bolded date list, then no action is taken.  If date occurs more than
        ///  once in the bolded date list, then only the first date is removed.  When
        ///  comparing dates, only the day and month are used.  Be sure to call
        ///  updateBoldedDates afterwards.
        /// </summary>
        public void RemoveMonthlyBoldedDate(DateTime date)
        {
            int length = monthlyArrayOfDates.Count;
            int i = 0;
            for (; i < length; ++i)
            {
                if (CompareDayAndMonth((DateTime)monthlyArrayOfDates[i], date))
                {
                    monthlyArrayOfDates.RemoveAt(i);
                    break;
                }
            }
            --length;
            for (int j = i; j < length; ++j)
            {
                if (CompareDayAndMonth((DateTime)monthlyArrayOfDates[j], date))
                {
                    return;
                }
            }
            datesToBoldMonthly &= ~(0x00000001 << (date.Day - 1));
        }

        private void ResetAnnuallyBoldedDates()
        {
            annualArrayOfDates.Clear();
        }

        private void ResetBoldedDates()
        {
            arrayOfDates.Clear();
        }

        private void ResetCalendarDimensions()
        {
            CalendarDimensions = new Size(1, 1);
        }

        /// <summary>
        ///  Resets the maximum selectable date.  By default value, there is no
        ///  upper limit.
        /// </summary>
        private void ResetMaxDate()
        {
            MaxDate = DateTime.MaxValue;
        }

        /// <summary>
        ///  Resets the minimum selectable date.  By default value, there is no
        ///  lower limit.
        /// </summary>
        private void ResetMinDate()
        {
            MinDate = DateTime.MinValue;
        }

        private void ResetMonthlyBoldedDates()
        {
            monthlyArrayOfDates.Clear();
        }

        /// <summary>
        ///  Resets the limits of the selection range.  By default value, the upper
        ///  and lower limit is the current date.
        /// </summary>
        private void ResetSelectionRange()
        {
            SetSelectionRange(Now, Now);
        }

        private void ResetTrailingForeColor()
        {
            TrailingForeColor = DEFAULT_TRAILING_FORE_COLOR;
        }

        private void ResetTitleForeColor()
        {
            TitleForeColor = DEFAULT_TITLE_FORE_COLOR;
        }

        private void ResetTitleBackColor()
        {
            TitleBackColor = DEFAULT_TITLE_BACK_COLOR;
        }

        /// <summary>
        ///  Resets the "today"'s date.  By default value, "today" is the
        ///  current date (and is automatically updated when the clock crosses
        ///  over to the next day).
        ///  If you set the today date yourself (using the TodayDate property)
        ///  the control will no longer automatically update the current day
        ///  for you. To re-enable this behavior, ResetTodayDate() is used.
        /// </summary>
        private void ResetTodayDate()
        {
            todayDateSet = false;
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
            if (reqSize * intSize > mdsBufferSize)
            {
                // free and expand the buffer,
                if (mdsBuffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(mdsBuffer);
                    mdsBuffer = IntPtr.Zero;
                }

                // Round up to the nearest multiple of MINIMUM_ALLOC_SIZE
                float quotient = (float)(reqSize - 1) / MINIMUM_ALLOC_SIZE;
                int actualSize = ((int)(quotient + 1)) * MINIMUM_ALLOC_SIZE;
                Debug.Assert(actualSize >= reqSize, "Tried to round up, but got it wrong");

                mdsBufferSize = actualSize * intSize;
                mdsBuffer = Marshal.AllocHGlobal(mdsBufferSize);
                return mdsBuffer;
            }
            return mdsBuffer;
        }

        /// <summary>
        ///  Sends a Win32 message to this control.  If the control does not yet
        ///  have a handle, it will be created.
        /// </summary>
        private IntPtr SendMessage(int msg, int wparam, ref ComCtl32.MCGRIDINFO lparam) =>
            ComCtl32.SendMessage(new HandleRef(this, Handle), msg, wparam, ref lparam);

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
        private void SetControlColor(ComCtl32.MCSC colorIndex, Color value)
        {
            if (IsHandleCreated)
            {
                SendMessage((int)ComCtl32.MCM.SETCOLOR, (int)colorIndex, ColorTranslator.ToWin32(value));
            }
        }

        /// <summary>
        ///  Updates the window handle with the min/max ranges if it has been
        ///  created.
        /// </summary>
        private void SetRange()
        {
            SetRange(DateTimePicker.EffectiveMinDate(minDate), DateTimePicker.EffectiveMaxDate(maxDate));
        }

        private void SetRange(DateTime minDate, DateTime maxDate)
        {
            // Keep selection range within passed in minDate and maxDate
            if (selectionStart < minDate)
            {
                selectionStart = minDate;
            }
            if (selectionStart > maxDate)
            {
                selectionStart = maxDate;
            }
            if (selectionEnd < minDate)
            {
                selectionEnd = minDate;
            }
            if (selectionEnd > maxDate)
            {
                selectionEnd = maxDate;
            }
            SetSelRange(selectionStart, selectionEnd);

            // Updated the calendar range
            //
            if (IsHandleCreated)
            {
                int flag = 0;

                NativeMethods.SYSTEMTIMEARRAY sa = new NativeMethods.SYSTEMTIMEARRAY();
                flag |= NativeMethods.GDTR_MIN | NativeMethods.GDTR_MAX;
                Kernel32.SYSTEMTIME sys = DateTimePicker.DateTimeToSysTime(minDate);
                sa.wYear1 = sys.wYear;
                sa.wMonth1 = sys.wMonth;
                sa.wDayOfWeek1 = sys.wDayOfWeek;
                sa.wDay1 = sys.wDay;
                sys = DateTimePicker.DateTimeToSysTime(maxDate);
                sa.wYear2 = sys.wYear;
                sa.wMonth2 = sys.wMonth;
                sa.wDayOfWeek2 = sys.wDayOfWeek;
                sa.wDay2 = sys.wDay;

                if ((int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), (int)ComCtl32.MCM.SETRANGE, flag, sa) == 0)
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

            if (dimensions.Width != x || dimensions.Height != y)
            {
                dimensions.Width = x;
                dimensions.Height = y;
                AdjustSize();
            }
        }

        /// <summary>
        ///  Sets date as the current selected date.  The start and begin of
        ///  the selection range will both be equal to date.
        /// </summary>
        public void SetDate(DateTime date)
        {
            if (date.Ticks < minDate.Ticks)
            {
                throw new ArgumentOutOfRangeException(nameof(date), date, string.Format(SR.InvalidLowBoundArgumentEx, nameof(date), FormatDate(date), nameof(MinDate)));
            }
            if (date.Ticks > maxDate.Ticks)
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
            if (date1.Ticks < minDate.Ticks)
            {
                throw new ArgumentOutOfRangeException(nameof(date1), date1, string.Format(SR.InvalidLowBoundArgumentEx, nameof(SelectionStart), FormatDate(date1), nameof(MinDate)));
            }
            if (date1.Ticks > maxDate.Ticks)
            {
                throw new ArgumentOutOfRangeException(nameof(date1), date1, string.Format(SR.InvalidHighBoundArgumentEx, nameof(SelectionEnd), FormatDate(date1), nameof(MaxDate)));
            }
            if (date2.Ticks < minDate.Ticks)
            {
                throw new ArgumentOutOfRangeException(nameof(date2), date2, string.Format(SR.InvalidLowBoundArgumentEx, nameof(SelectionStart), FormatDate(date2), nameof(MinDate)));
            }
            if (date2.Ticks > maxDate.Ticks)
            {
                throw new ArgumentOutOfRangeException(nameof(date2), date2, string.Format(SR.InvalidHighBoundArgumentEx, nameof(SelectionEnd), FormatDate(date2), nameof(MaxDate)));
            }

            // If date1 > date2, we just select date2 (compat)
            //
            if (date1 > date2)
            {
                date2 = date1;
            }

            // If the range exceeds maxSelectionCount, compare with the previous range and adjust whichever
            // limit hasn't changed.
            //
            if ((date2 - date1).Days >= maxSelectionCount)
            {

                if (date1.Ticks == selectionStart.Ticks)
                {
                    // Bring start date forward
                    //
                    date1 = date2.AddDays(1 - maxSelectionCount);
                }
                else
                {
                    // Bring end date back
                    //
                    date2 = date1.AddDays(maxSelectionCount - 1);
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
            if (selectionStart != lower || selectionEnd != upper)
            {
                changed = true;
                selectionStart = lower;
                selectionEnd = upper;
            }

            // always set the value on the control, to ensure that
            // it is up to date.
            //
            if (IsHandleCreated)
            {
                NativeMethods.SYSTEMTIMEARRAY sa = new NativeMethods.SYSTEMTIMEARRAY();

                Kernel32.SYSTEMTIME sys = DateTimePicker.DateTimeToSysTime(lower);
                sa.wYear1 = sys.wYear;
                sa.wMonth1 = sys.wMonth;
                sa.wDayOfWeek1 = sys.wDayOfWeek;
                sa.wDay1 = sys.wDay;
                sys = DateTimePicker.DateTimeToSysTime(upper);
                sa.wYear2 = sys.wYear;
                sa.wMonth2 = sys.wMonth;
                sa.wDayOfWeek2 = sys.wDayOfWeek;
                sa.wDay2 = sys.wDay;
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), (int)ComCtl32.MCM.SETSELRANGE, 0, sa);
            }

            if (changed)
            {
                OnDateChanged(new DateRangeEventArgs(lower, upper));
            }
        }

        private bool ShouldSerializeAnnuallyBoldedDates()
        {
            return annualArrayOfDates.Count > 0;
        }

        private bool ShouldSerializeBoldedDates()
        {
            return arrayOfDates.Count > 0;
        }

        private bool ShouldSerializeCalendarDimensions()
        {
            return !dimensions.Equals(new Size(1, 1));
        }

        private bool ShouldSerializeTrailingForeColor()
        {
            return !TrailingForeColor.Equals(DEFAULT_TRAILING_FORE_COLOR);
        }

        private bool ShouldSerializeTitleForeColor()
        {
            return !TitleForeColor.Equals(DEFAULT_TITLE_FORE_COLOR);
        }

        private bool ShouldSerializeTitleBackColor()
        {
            return !TitleBackColor.Equals(DEFAULT_TITLE_BACK_COLOR);
        }

        private bool ShouldSerializeMonthlyBoldedDates()
        {
            return monthlyArrayOfDates.Count > 0;
        }

        /// <summary>
        ///  Retrieves true if the maxDate should be persisted in code gen.
        /// </summary>
        private bool ShouldSerializeMaxDate()
        {
            return maxDate != DateTimePicker.MaximumDateTime && maxDate != DateTime.MaxValue;
        }

        /// <summary>
        ///  Retrieves true if the minDate should be persisted in code gen.
        /// </summary>
        private bool ShouldSerializeMinDate()
        {
            return minDate != DateTimePicker.MinimumDateTime && minDate != DateTime.MinValue;
        }

        /// <summary>
        ///  Retrieves true if the selectionRange should be persisted in code gen.
        /// </summary>
        private bool ShouldSerializeSelectionRange()
        {
            return !DateTime.Equals(selectionEnd, selectionStart);
        }

        /// <summary>
        ///  Retrieves true if the todayDate should be persisted in code gen.
        /// </summary>
        private bool ShouldSerializeTodayDate()
        {
            return todayDateSet;
        }

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
        public void UpdateBoldedDates()
        {
            RecreateHandle();
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

                if (todayDateSet)
                {
                    Kernel32.SYSTEMTIME st = DateTimePicker.DateTimeToSysTime(todayDate);
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), (int)ComCtl32.MCM.SETTODAY, 0, ref st);
                }
                else
                {
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), (int)ComCtl32.MCM.SETTODAY, 0, IntPtr.Zero);
                }
            }
        }

        private void MarshaledUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs pref)
        {
            try
            {
                //use begininvoke instead of invoke in case the destination thread is not processing messages.
                BeginInvoke(new UserPreferenceChangedEventHandler(UserPreferenceChanged), new object[] { sender, pref });
            }
            catch (InvalidOperationException) { } //if the destination thread does not exist, don't send.
        }

        private void UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs pref)
        {
            if (pref.Category == UserPreferenceCategory.Locale)
            {
                // We need to recreate the monthcalendar handle when the locale changes, because
                // the day names etc. are only updated on a handle recreate (comctl32 limitation).
                //
                RecreateHandle();
            }
        }

        /// <summary>
        ///  Handles the MCN_SELCHANGE notification
        /// </summary>
        private void WmDateChanged(ref Message m)
        {
            NativeMethods.NMSELCHANGE nmmcsc = (NativeMethods.NMSELCHANGE)m.GetLParam(typeof(NativeMethods.NMSELCHANGE));
            DateTime start = selectionStart = DateTimePicker.SysTimeToDateTime(nmmcsc.stSelStart);
            DateTime end = selectionEnd = DateTimePicker.SysTimeToDateTime(nmmcsc.stSelEnd);

            AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
            AccessibilityNotifyClients(AccessibleEvents.ValueChange, -1);

            MonthCalendarAccessibleObject calendarAccessibleObject = (MonthCalendarAccessibleObject)AccessibilityObject;
            calendarAccessibleObject.RaiseAutomationEventForChild(NativeMethods.UIA_AutomationFocusChangedEventId, selectionStart, selectionEnd);

            //subhag
            if (start.Ticks < minDate.Ticks || end.Ticks < minDate.Ticks)
            {
                SetSelRange(minDate, minDate);
            }
            else if (start.Ticks > maxDate.Ticks || end.Ticks > maxDate.Ticks)
            {
                SetSelRange(maxDate, maxDate);
            }
            //end subhag
            OnDateChanged(new DateRangeEventArgs(start, end));
        }

        /// <summary>
        ///  Handles the MCN_GETDAYSTATE notification
        /// </summary>
        private void WmDateBold(ref Message m)
        {
            NativeMethods.NMDAYSTATE nmmcds = (NativeMethods.NMDAYSTATE)m.GetLParam(typeof(NativeMethods.NMDAYSTATE));
            DateTime start = DateTimePicker.SysTimeToDateTime(nmmcds.stStart);
            DateBoldEventArgs boldEvent = new DateBoldEventArgs(start, nmmcds.cDayState);
            BoldDates(boldEvent);
            mdsBuffer = RequestBuffer(boldEvent.Size);
            // copy boldEvent into mdsBuffer
            Marshal.Copy(boldEvent.DaysToBold, 0, mdsBuffer, boldEvent.Size);
            // now we replug DateBoldEventArgs info into NMDAYSTATE
            nmmcds.prgDayState = mdsBuffer;
            Marshal.StructureToPtr(nmmcds, m.LParam, false);
        }

        /// <summary>
        ///  Handles the MCN_VIEWCHANGE  notification
        /// </summary>
        private void WmCalViewChanged(ref Message m)
        {
            NativeMethods.NMVIEWCHANGE nmmcvm = (NativeMethods.NMVIEWCHANGE)m.GetLParam(typeof(NativeMethods.NMVIEWCHANGE));
            Debug.Assert(mcCurView == (NativeMethods.MONTCALENDAR_VIEW_MODE)nmmcvm.uOldView, "Calendar view mode is out of sync with native control");
            if (mcCurView != (NativeMethods.MONTCALENDAR_VIEW_MODE)nmmcvm.uNewView)
            {
                mcOldView = mcCurView;
                mcCurView = (NativeMethods.MONTCALENDAR_VIEW_MODE)nmmcvm.uNewView;

                AccessibilityNotifyClients(AccessibleEvents.ValueChange, -1);
                AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
            }
        }
        /// <summary>
        ///  Handles the MCN_SELECT notification
        /// </summary>
        private void WmDateSelected(ref Message m)
        {
            NativeMethods.NMSELCHANGE nmmcsc = (NativeMethods.NMSELCHANGE)m.GetLParam(typeof(NativeMethods.NMSELCHANGE));
            DateTime start = selectionStart = DateTimePicker.SysTimeToDateTime(nmmcsc.stSelStart);
            DateTime end = selectionEnd = DateTimePicker.SysTimeToDateTime(nmmcsc.stSelEnd);

            AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
            AccessibilityNotifyClients(AccessibleEvents.ValueChange, -1);

            //subhag
            if (start.Ticks < minDate.Ticks || end.Ticks < minDate.Ticks)
            {
                SetSelRange(minDate, minDate);
            }
            else if (start.Ticks > maxDate.Ticks || end.Ticks > maxDate.Ticks)
            {
                SetSelRange(maxDate, maxDate);
            }

            //end subhag
            OnDateSelected(new DateRangeEventArgs(start, end));

        }

        /// <summary>
        ///  Handles the WM_GETDLGCODE message
        /// </summary>
        private void WmGetDlgCode(ref Message m)
        {
            // The MonthCalendar does its own handling of arrow keys
            m.Result = (IntPtr)NativeMethods.DLGC_WANTARROWS;
        }

        /// <summary>
        ///  Handles the WM_COMMAND messages reflected from the parent control.
        /// </summary>
        private void WmReflectCommand(ref Message m)
        {
            if (m.HWnd == Handle)
            {
                NativeMethods.NMHDR nmhdr = (NativeMethods.NMHDR)m.GetLParam(typeof(NativeMethods.NMHDR));
                switch (nmhdr.code)
                {
                    case NativeMethods.MCN_SELECT:
                        WmDateSelected(ref m);
                        break;
                    case NativeMethods.MCN_SELCHANGE:
                        WmDateChanged(ref m);
                        break;
                    case NativeMethods.MCN_GETDAYSTATE:
                        WmDateBold(ref m);
                        break;
                    case NativeMethods.MCN_VIEWCHANGE:
                        WmCalViewChanged(ref m);
                        break;
                }
            }
        }

        /// <summary>
        ///  Overrided wndProc
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WindowMessages.WM_LBUTTONDOWN:
                    Focus();
                    if (!ValidationCancelled)
                    {
                        base.WndProc(ref m);
                    }
                    break;
                case WindowMessages.WM_GETDLGCODE:
                    WmGetDlgCode(ref m);
                    break;
                case WindowMessages.WM_REFLECT + WindowMessages.WM_NOTIFY:
                    WmReflectCommand(ref m);
                    base.WndProc(ref m);
                    break;
                case WindowMessages.WM_DESTROY:
                    base.WndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <summary>
        ///  HitTestInfo objects are returned by MonthCalendar in response to the hitTest method.
        ///  HitTestInfo is for informational purposes only; the user should not construct these objects, and
        ///  cannot modify any of the members.
        /// </summary>
        public sealed class HitTestInfo
        {
            readonly Point point;
            readonly HitArea hitArea;
            readonly DateTime time;

            /// <summary>
            /// </summary>
            internal HitTestInfo(Point pt, HitArea area, DateTime time)
            {
                point = pt;
                hitArea = area;
                this.time = time;
            }

            /// <summary>
            ///  This constructor is used when the DateTime member is invalid.
            /// </summary>
            internal HitTestInfo(Point pt, HitArea area)
            {
                point = pt;
                hitArea = area;
            }

            /// <summary>
            ///  The point that was hit-tested
            /// </summary>
            public Point Point
            {
                get { return point; }
            }

            /// <summary>
            ///  Output member that receives an enumeration value from System.Windows.Forms.MonthCalendar.HitArea
            ///  representing the result of the hit-test operation.
            /// </summary>
            public HitArea HitArea
            {
                get { return hitArea; }
            }

            /// <summary>
            ///  The time information specific to the location that was hit-tested.  This value
            ///  will only be valid at certain values of hitArea.
            /// </summary>
            public DateTime Time
            {
                get { return time; }
            }

            /// <summary>
            ///  Determines whether a given HitArea should have a corresponding valid DateTime
            /// </summary>
            internal static bool HitAreaHasValidDateTime(HitArea hitArea)
            {
                switch (hitArea)
                {
                    case HitArea.Date:
                    //case HitArea.DayOfWeek:   comCtl does not provide a valid date
                    case HitArea.WeekNumbers:
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        ///  This enumeration has specific areas of the MonthCalendar control as
        ///  its enumerated values. The hitArea member of System.Windows.Forms.Win32.HitTestInfo
        ///  will be one of these enumerated values, and indicates which portion of
        ///  a month calendar is under a specific point.
        /// </summary>
        public enum HitArea
        {
            /// <summary>
            ///  The given point was not on the month calendar control, or it was
            ///  in an inactive portion of the control.
            /// </summary>
            Nowhere = 0,

            /// <summary>
            ///  The given point was over the background of a month's title
            /// </summary>
            TitleBackground = 1,

            /// <summary>
            ///  The given point was in a month's title bar, over a month name
            /// </summary>
            TitleMonth = 2,

            /// <summary>
            ///  The given point was in a month's title bar, over the year value
            /// </summary>
            TitleYear = 3,

            /// <summary>
            ///  The given point was over the button at the top right corner of
            ///  the control. If the user clicks here, the month calendar will
            ///  scroll its display to the next month or set of months
            /// </summary>
            NextMonthButton = 4,

            /// <summary>
            ///  The given point was over the button at the top left corner of
            ///  the control. If the user clicks here, the month calendar will
            ///  scroll its display to the previous month or set of months
            /// </summary>
            PrevMonthButton = 5,

            /// <summary>
            ///  The given point was in the calendar's background
            /// </summary>
            CalendarBackground = 6,

            /// <summary>
            ///  The given point was on a particular date within the calendar,
            ///  and the time member of HitTestInfo will be set to the date at
            ///  the given point.
            /// </summary>
            Date = 7,

            /// <summary>
            ///  The given point was over a date from the next month (partially
            ///  displayed at the end of the currently displayed month). If the
            ///  user clicks here, the month calendar will scroll its display to
            ///  the next month or set of months.
            /// </summary>
            NextMonthDate = 8,

            /// <summary>
            ///  The given point was over a date from the previous month (partially
            ///  displayed at the end of the currently displayed month). If the
            ///  user clicks here, the month calendar will scroll its display to
            ///  the previous month or set of months.
            /// </summary>
            PrevMonthDate = 9,

            /// <summary>
            ///  The given point was over a day abbreviation ("Fri", for example).
            ///  The time member of HitTestInfo will be set to the corresponding
            ///  date on the top row.
            /// </summary>
            DayOfWeek = 10,

            /// <summary>
            ///  The given point was over a week number.  This will only occur if
            ///  the showWeekNumbers property of MonthCalendar is enabled. The
            ///  time member of HitTestInfo will be set to the corresponding date
            ///  in the leftmost column.
            /// </summary>
            WeekNumbers = 11,

            /// <summary>
            ///  The given point was on the "today" link at the bottom of the
            ///  month calendar control
            /// </summary>
            TodayLink = 12,
        }
    } // end class MonthCalendar
}
